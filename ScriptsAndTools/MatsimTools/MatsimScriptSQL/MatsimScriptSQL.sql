-----------------------------------------------
-- Preparation for mapping nodes and routing
-----------------------------------------------

-- extract car ways only --
drop table if exists carways;
create table carways as
select distinct wt.way_id as osm_way_id 
from way_tags wt
where key = 'highway'
and value in (
'motorway',
'motorway_link',
'motorway_junction',
'trunk',
'trunk_link',
'primary',
'primary_link',
'secondary',
'secondary_link',
'tertiary',
'tertiary_link',
'residential',
'living_street',
'service',
'unclassified',
'road'
);

-- nodes in car ways with geom --
drop table if exists node_carways_ext;
create table node_carways_ext as
select distinct wn.node_id  as osm_node_id, latitude, longitude,
st_setsrid(ST_MakePoint(longitude/10000000::float8, latitude/10000000::float8), 4326) as the_geom
from carways cw
join way_nodes wn
on cw.osm_way_id = wn.way_id
join node n
on n.id = wn.node_id;

DROP INDEX IF EXISTS idx_ncx_geom;
CREATE INDEX idx_ncx_geom ON node_carways_ext USING GIST(the_geom);

drop table if exists node_lut;
create table node_lut as
select row_number() over (order by osm_node_id) as node_id, * 
from node_carways_ext;

drop sequence if exists seq_node_id;
create sequence seq_node_id;
select setval('seq_node_id', max(node_id)) from node_lut;

-- select * from node_lut order by node_id desc limit 1;
-- select * from node_lut where osm_node_id = 303849290;
-- select count(*) from node_lut limit 100;

-- create car ways information for later process --
drop table if exists road_ext;
create table road_ext as 
select row_number() over (order by osm_way_id, sequence_id) as gid, *
from (
select 
aa.node_id as start_id,
bb.node_id as end_id,
a.way_id as osm_way_id, a.node_id as osm_source, b.node_id as osm_target, a.sequence_id,
aa.the_geom as source_geom, bb.the_geom as target_geom,
ST_MakeLine(aa.the_geom, bb.the_geom) as way_geom,
st_distance(aa.the_geom, bb.the_geom) as length,
ST_Distance_Spheroid(aa.the_geom, bb.the_geom, 'SPHEROID["WGS 84",6378137,298.257223563]') as length_m
from way_nodes a
join way_nodes b
on a.sequence_id +1 = b.sequence_id
join node_lut aa
on aa.osm_node_id = a.node_id
join node_lut bb
on bb.osm_node_id = b.node_id
join carways cw
on a.way_id = cw.osm_way_id
where a.way_id = b.way_id
) as foo;

DROP INDEX IF EXISTS idx_way_geom;
CREATE INDEX idx_way_geom ON road_ext USING GIST(way_geom);

drop index if exists idx_start_id;
create index idx_start_id on road_ext using btree(start_id);

drop index if exists idx_end_id;
create index idx_end_id on road_ext using btree(end_id);

drop index if exists idx_gid;
create index idx_gid on road_ext using btree(gid);

drop sequence if exists seq_gid;
create sequence seq_gid;
select setval('seq_gid', max(gid)) from road_ext;

-- select st_astext(way_geom), * from road_ext where gid = 153
-- select * from view_bus_events limit 100;
-- select * from matsim_node_lut where gid is not null and node_id = 545988 ;
-- select * from road_ext where gid = 158304;

-- select minlat/10000000::float8 as minlat, maxlat/10000000::float8 as maxlat, minlon/10000000::float8 as minlon, maxlon/10000000::float8 as maxlon from viewbounds_stockholm ;


-- Read the .poly file of the admin boundary of stockholms lan --
-- remarks: change the path to the polybound. --
drop table if exists polybound;
create table polybound (content text, lon numeric, lat numeric);
--copy polybound from'C:\Users\protoworld\Desktop\MatsimFiles\osmFiles/stockholm_lan.pgpoly'
-- with (DELIMITER E'\t');

-- Hack because the above mentioned file is missing, mirrors OSM import boundaries.
INSERT INTO polybound (lon, lat) VALUES (17.868, 59.234);
INSERT INTO polybound (lon, lat) VALUES (18.175, 59.234);
INSERT INTO polybound (lon, lat) VALUES (18.175, 59.415);
INSERT INTO polybound (lon, lat) VALUES (17.868, 59.415);
INSERT INTO polybound (lon, lat) VALUES (17.868, 59.234);

-- select * from polybound;

-- Create the region of interest bounaries from polybound --
drop table if exists roi_bounds;
create table roi_bounds as
WITH line_geoms AS 
(
SELECT ST_LineFromMultiPoint(ST_Collect(ST_SetSRID(ST_MakePoint(lon, lat), 4326))) as line 
FROM polybound
)
SELECT ST_MakePolygon(ST_AddPoint(line, ST_StartPoint(line))) as polygon
FROM line_geoms;

-- select * from roi_bounds ;

-- save the matsimnodes which is outside of stockholms lan --
drop table if exists matsimnodes_out_of_bounds;
create table matsimnodes_out_of_bounds as 
select mn.id as matsim_id, st_x(mn.geom), st_y(mn.geom), mn.geom 
from matsimnodes mn, roi_bounds rb
where NOT ST_Contains(rb.polygon, mn.geom);

-----------------------------------------------
-- Mapping nodes 
-----------------------------------------------

drop view if exists view_car_events ;
create or replace view view_car_events as
select ml.*, a.geom as from_geom, b.geom as to_geom,
ev.event_time, ev.type as ev_type, ev.veh_id, row_number() over (partition by veh_id order by event_time, ev.type) as row
from events ev
join matsimlinks ml
on ml.link_id = ev.link_id
join matsimnodes a
on a.id = ml.from_id
join matsimnodes b
on b.id = ml.to_id
where ev.veh_id not like 'Veh%'
and
ev.type in ('wait2link', 'entered link', 'vehicle leaves traffic');

drop view if exists view_bus_events ;
create or replace view view_bus_events as
select ml.*, a.geom as from_geom, b.geom as to_geom,
ev.event_time, ev.type as ev_type, ev.veh_id, row_number() over (partition by veh_id order by event_time, ev.type) as row
from events ev
join matsimvehicle mv
on mv.id = ev.veh_id
join matsimlinks ml
on ml.link_id = ev.link_id
join matsimnodes a
on a.id = ml.from_id
join matsimnodes b
on b.id = ml.to_id
where mv.type in ('BUS')
and
ev.type in ('wait2link', 'entered link', 'vehicle leaves traffic');

-- gather nodes from view_car_events & view_bus_events --
drop table if exists matsim_nodes_road;
create table matsim_nodes_road as
select from_id as matsim_id, from_geom as geom from view_car_events
union
select to_id as matsim_id, to_geom as geom from view_car_events
union
select from_id as matsim_id, from_geom as geom from view_bus_events
union
select to_id as matsim_id, to_geom as geom from view_bus_events;

DROP INDEX IF EXISTS idx_nodes_road_geom;
CREATE INDEX idx_nodes_road_geom ON matsim_nodes_road USING GIST(geom);

-- map matsim_nodes_road to road_ext --
drop table if exists matsim_road_ext;
select * into matsim_road_ext from(
select mn.matsim_id, 
	(select rx.gid
	from road_ext as rx
	where ST_DWithin(mn.geom, rx.way_geom, .006)
	order by mn.geom <#> rx.way_geom limit 1)
from matsim_nodes_road as mn
) as foo;

-- map matsim_nodes_road to node_carways_ext --
drop table if exists matsim_carways_ext;
select * into matsim_carways_ext from(
select mn.matsim_id, 
	(select rx.osm_node_id
	from node_carways_ext as rx
	where ST_DWithin(mn.geom, rx.the_geom, .007)
	order by mn.geom <#> rx.the_geom limit 1)
from matsim_nodes_road as mn
) as foo;


-- compare distance between matsim node and road or osm node inside stockholms lan--
drop table if exists mapping_compare;
create table mapping_compare as
select a.*, b.gid, 
ST_Distance_Spheroid(e.geom, ST_ClosestPoint(c.the_geom, e.geom), 'SPHEROID["WGS 84",6378137,298.257223563]') as node_dist_m,
ST_Distance_Spheroid(e.geom, ST_ClosestPoint(d.way_geom, e.geom), 'SPHEROID["WGS 84",6378137,298.257223563]') as road_dist_m,
ST_ClosestPoint(d.way_geom, e.geom) as closest_road_point
from matsim_carways_ext a
join matsim_road_ext b
on a.matsim_id = b.matsim_id
join node_carways_ext c
on c.osm_node_id = a.osm_node_id
join road_ext d
on d.gid = b.gid
join matsimnodes e
on e.id = a.matsim_id
where a.matsim_id not in (select matsim_id from matsimnodes_out_of_bounds)
order by a.matsim_id;

-- remove the gid or osm_node_id if it is farther away --
-- also set the node_id from node_lut for those that osm_node_id is closer --
drop table if exists matsim_node_lut;
create table matsim_node_lut as
select matsim_id, 
case when node_dist_m <= road_dist_m then a.osm_node_id end as osm_node_id,
case when node_dist_m > road_dist_m then gid end as gid,
node_dist_m, road_dist_m, 
case when node_dist_m > road_dist_m then closest_road_point 
else b.the_geom
end as the_geom,
case when node_dist_m <= road_dist_m then b.node_id end as node_id
from mapping_compare a
left join node_lut b
on a.osm_node_id = b.osm_node_id
order by matsim_id;

-- set a corresponding node_id to closest_road_point (if node_id is not null) --
update matsim_node_lut
set node_id = nextval('seq_node_id') 
where node_id is null;

drop table if exists node_geom;
create table node_geom as
(
select node_id, the_geom from node_lut
union
select node_id, the_geom from matsim_node_lut
);

-- add extra roads to road_ext --
-- split the original road, this is the first half --
insert into road_ext
select nextval('seq_gid') as gid, b.start_id, a.node_id as end_id, 
b.osm_way_id, b.osm_source, null, b.sequence_id,
b.source_geom, a.the_geom as target_geom, 
ST_MakeLine(b.source_geom, a.the_geom) as way_geom,
st_distance(b.source_geom, a.the_geom) as length,
ST_Distance_Spheroid(b.source_geom, a.the_geom, 'SPHEROID["WGS 84",6378137,298.257223563]') as length_m
from matsim_node_lut a
join road_ext b
on a.gid = b.gid
order by gid;

-- split the original road, this is the second half --
insert into road_ext
select nextval('seq_gid') as gid, a.node_id as start_id, b.end_id, 
b.osm_way_id, null, b.osm_target, b.sequence_id,
a.the_geom as source_geom, b.target_geom, 
ST_MakeLine(a.the_geom, b.target_geom) as way_geom,
st_distance(a.the_geom, b.target_geom) as length,
ST_Distance_Spheroid(a.the_geom, b.target_geom, 'SPHEROID["WGS 84",6378137,298.257223563]') as length_m
from matsim_node_lut a
join road_ext b
on a.gid = b.gid
order by gid;

-- delete the original road --
delete from road_ext
where gid in (select gid from matsim_node_lut where gid is not null);

-- may be speed up the routing process --
SELECT pgr_createVerticesTable('road_ext','way_geom','start_id','end_id');
-- SELECT pgr_analyzeGraph ('road_ext', 0.001, 'way_geom', 'gid', 'start_id', 'end_id');

-----------------------------------------------
-- Interpolate positions for trains and ferries
-----------------------------------------------

-- prepare the data of events of ferries, trains and trams --
drop view if exists view_pt_events;
create or replace view view_pt_events as
select ml.*, a.geom as from_geom, b.geom as to_geom,
ev.event_time, ev.type as ev_type, ev.veh_id, mv.type as veh_type, 
row_number() over (partition by veh_id order by event_time, ev.type) as row
from events ev
join matsimvehicle mv
on mv.id = ev.veh_id
join matsimlinks ml
on ml.link_id = ev.link_id
join matsimnodes a
on a.id = ml.from_id
join matsimnodes b
on b.id = ml.to_id
where mv.type in ('FERRY', 'TRAIN', 'PENDELTAG', 'TRAM')
and
ev.type in ('wait2link', 'entered link', 'vehicle leaves traffic');

-- select * from view_pt_events limit 10;

-- convert the position to wgs84, make line geometry from link and degree heading --
drop table if exists pt_positions;
create table pt_positions as
select veh_type, veh_id, event_time as eventtime,
row as leg_id, 
case when ev_type = 'vehicle leaves traffic' then st_x(to_geom) 
else st_x(from_geom) 
end as longitude,
case when ev_type = 'vehicle leaves traffic' then st_y(to_geom)
else st_y(from_geom) 
end as latitude,
case when ev_type = 'vehicle leaves traffic' then to_geom
else ST_MakeLine(from_geom, to_geom)
end as the_geom,
case when ST_Azimuth(from_geom, to_geom) > 0 then degrees(ST_Azimuth(from_geom, to_geom))
else 0
end as deg
from view_pt_events;

-- select * from pt_positions where veh_type = 'PENDELTAG' limit 10;

-- interpolate time between the position snapshots --
drop view if exists view_pt_interp;
create or replace view view_pt_interp as 
WITH RECURSIVE interp(veh_type, veh_id, eventtime, leg_id, geom, deg) AS (
    SELECT veh_type, veh_id, eventtime, leg_id, the_geom, deg from pt_positions
  UNION ALL
    SELECT t.veh_type, t.veh_id, t.eventtime::integer+1, t.leg_id, t.geom, t.deg 
	FROM pt_positions as ptp, interp as t
    where t.eventtime::integer+1 < ptp.eventtime 
	and t.leg_id+1 = ptp.leg_id
	and t.veh_id = ptp.veh_id
)
SELECT t.eventtime, t.veh_id, t.veh_type, t.leg_id, 
b.eventtime - c.eventtime as diff1, 
t.geom, t.deg
FROM interp t
left join pt_positions b
on t.leg_id + 1 = b.leg_id 
left join pt_positions c
on b.leg_id - 1 = c.leg_id
where t.veh_id = b.veh_id 
and b.veh_id = c.veh_id;

-- interpolate the position --
drop table if exists pt_position_interps;
create table pt_position_interps as
select a.eventtime, a.veh_id, a.veh_type, a.leg_id, 
case when diff1 is null then geom 
else ST_Line_Interpolate_Point(geom, (a.eventtime-b.eventtime)/diff1::float8)
end as interp_geom,
a.deg
from view_pt_interp a
left join pt_positions b
on a.leg_id = b.leg_id
where a.veh_id = b.veh_id;

-- select * from pt_position_interps order by veh_id, leg_id limit 1000;

-- convert geom to longitude (y), latitude (x) --
drop table if exists event_position;
create table event_position as
select veh_type, veh_id, eventtime, st_y(interp_geom) as y, st_x(interp_geom) as x, deg 
from pt_position_interps;

-- select count(*) from event_position;
-- select * from event_position limit 100;

-----------------------------------------------
-- Routing and interpolate positions for cars
-----------------------------------------------

-- create 'car_events_osm' FROM 'view_car_events' for routing --
drop table if exists car_events_osm;
CREATE TABLE car_events_osm as
SELECT a.veh_id, a.ev_type, a.event_time, a.from_id, a.to_id, a.row as leg_id,
aa.node_id as source, ab.node_id as target
FROM view_car_events a
LEFT JOIN matsim_node_lut aa
ON a.from_id = aa.matsim_id
LEFT JOIN matsim_node_lut ab
ON a.to_id = ab.matsim_id;

-- rearrange so that vehicle joins AND leaves traffic att a single point. --
update car_events_osm 
set source = target, from_id = to_id
WHERE ev_type in ('wait2link', 'vehicle leaves traffic');

-- create unique legs for not routing multiple times --
drop table if exists unique_legs;
CREATE TABLE unique_legs as
SELECT distinct source, target, ev_type 
FROM car_events_osm
WHERE ev_type not in ('wait2link', 'vehicle leaves traffic');

-- create routings FROM unique legs --
drop table if exists unique_routings;
CREATE TABLE unique_routings as
SELECT a.*,
seq, id1 AS node, id2 AS edge, cost 
FROM unique_legs as a,
pgr_dijkstra('
SELECT gid::integer AS id,
start_id::integer as source,
end_id::integer as target,
length_m::double precision AS cost
FROM road_ext',
a.source::integer, a.target::integer, false, false);

-- SELECT * FROM unique_routings;

-- SELECT * FROM car_events_osm WHERE veh_id in ('1002006', '1003127' ,'1003620') ORDER BY veh_id, leg_id;

-- putting together the routing with the event -- 
drop table if exists car_events_routing;
CREATE TABLE car_events_routing as
SELECT 
veh_id, a.ev_type, event_time, from_id, to_id, leg_id, seq, node as node_id, the_geom
FROM car_events_osm a, unique_routings b
join node_geom c
ON b.node = c.node_id
WHERE a.source = b.source
AND a.target = b.target;

-- SELECT * FROM car_events_routing ORDER BY veh_id, leg_id, seq;

-- extract elapsed time for each leg --
drop table if exists car_events_routing_elapsed_time;
CREATE TABLE car_events_routing_elapsed_time as 
SELECT a.veh_id, a.leg_id, b.event_time - a.event_time as elapsed_time
FROM car_events_osm a
LEFT JOIN car_events_osm b
ON a.leg_id + 1 = b.leg_id
WHERE a.veh_id = b.veh_id;

-- SELECT * FROM car_events_routing_elapsed_time WHERE elapsed_time = 0 ORDER BY veh_id, leg_id;

-- prepare the interpolation --
drop table if exists car_events_routing_interp_prepare; 
CREATE TABLE car_events_routing_interp_prepare as 
SELECT foo.*, boo.event_time - 1 as next_time FROM 
(
SELECT a.veh_id, a.ev_type, a.leg_id, a.event_time as event_time, 
ST_MakeLine(the_geom ORDER BY seq) as line_geom
FROM car_events_routing a
group by a.veh_id, a.ev_type, a.leg_id, event_time
) as foo, car_events_osm as boo
WHERE foo.veh_id = boo.veh_id 
AND foo.leg_id + 1 = boo.leg_id;

-- interpolate car positions for pgrouting --
drop table if exists car_events_routing_interp; 
CREATE TABLE car_events_routing_interp as
with recursive interp as
(
    SELECT veh_id, ev_type, leg_id, event_time, line_geom, 0 as n, next_time
        FROM car_events_routing_interp_prepare
    UNION all
      SELECT a.veh_id, a.ev_type, a.leg_id, a.event_time, a.line_geom, n+1 as n, a.next_time
      FROM interp a
      WHERE a.n + a.event_time < a.next_time
      AND a.ev_type not like '%leaves%'
)
SELECT
a.veh_id, a.ev_type, a.leg_id, a.event_time+a.n as event_time, 
CASE WHEN eet.elapsed_time > 0 then ST_LineInterpolatePoint(a.line_geom, (a.n)/eet.elapsed_time) 
ELSE ST_StartPoint(line_geom) END as the_geom
FROM interp a
LEFT JOIN car_events_routing_elapsed_time eet
ON a.veh_id = eet.veh_id
AND a.leg_id = eet.leg_id;

-- SELECT * FROM car_events_routing_interp WHERE veh_id in ('1002006', '1003127' ,'1003620') ORDER BY veh_id, leg_id, event_time;

-- put together events with and without routings --
drop table if exists car_events_total;
CREATE TABLE car_events_total as
SELECT dense_rank() over (ORDER BY veh_id, leg_id, event_time) as row, * FROM 
(
SELECT * FROM car_events_routing_interp
UNION
SELECT veh_id, ev_type, leg_id, event_time,
CASE WHEN c.node_id is null then b.geom
ELSE c.the_geom END as the_geom
FROM car_events_osm a 
LEFT JOIN matsimnodes_out_of_bounds b
ON a.from_id = b.matsim_id 
LEFT JOIN matsim_node_lut c
ON a.from_id = c.matsim_id
WHERE not exists
(
  SELECT null FROM unique_routings d WHERE a.source = d.source AND a.target = d.target
)
) as foo;

-- SELECT * FROM car_events_total WHERE veh_id in ('1002006', '1003127' ,'1003620') ORDER BY veh_id, leg_id, event_time;

-- prepare the second part of interpolation. -- 
drop table if exists car_events_interp_second;
CREATE TABLE car_events_interp_second as
SELECT a.veh_id, a.ev_type, a.leg_id, a.event_time, b.event_time - 1 as next_time,
st_makeline(a.the_geom, b.the_geom) as line_geom
FROM car_events_total a
join car_events_total b
ON a.row + 1 = b.row
AND a.veh_id = b.veh_id
AND a.event_time + 1 != b.event_time
AND a.ev_type not like '%leaves%';

-- SELECT * FROM car_events_interp_second WHERE ev_type not like '%enter%' ORDER BY veh_id, leg_id, event_time ;

-- interpolate the remaining car positions. --
drop table if exists car_events_interp_total;
CREATE TABLE car_events_interp_total as
WITH RECURSIVE interp AS 
(
    SELECT veh_id, ev_type, event_time, 0 as n, leg_id, line_geom, next_time
    FROM car_events_interp_second
  UNION ALL
    SELECT t.veh_id, t.ev_type, t.event_time, n+1 as n, t.leg_id, t.line_geom, t.next_time
	FROM interp as t
    WHERE t.event_time + n < t.next_time 
)
-- SELECT * FROM interp ORDER BY veh_id, leg_id, event_time, n ;
SELECT veh_id, ev_type, leg_id, event_time + n as event_time,
ST_LineInterpolatePoint(line_geom, n/(next_time-event_time)) as the_geom
FROM interp;

-- select st_y(the_geom), st_x(the_geom), * from car_events_interp_total ORDER BY veh_id, leg_id, event_time ;

-- put together the interpolation from the 2 passes --
drop table if exists car_events_interp_final;
CREATE TABLE car_events_interp_final as
SELECT st_y(the_geom) as y, st_x(the_geom) as x, * FROM 
(
SELECT * FROM car_events_routing_interp
UNION
SELECT * FROM car_events_interp_total
) as foo;

-- select * from car_events_interp_final where veh_id in ('1002006', '1003127' ,'1003620') ORDER BY veh_id, leg_id, event_time ;

-- insert the position into the main table --
INSERT INTO event_position
SELECT 'CAR'::text as veh_type, a.veh_id, a.event_time, a.y, a.x, 
CASE WHEN ST_Azimuth(a.the_geom, b.the_geom) > 0 then degrees(ST_Azimuth(a.the_geom, b.the_geom))
ELSE 0
END as deg
FROM car_events_interp_final a
LEFT JOIN car_events_interp_final b
ON a.veh_id = b.veh_id
AND a.event_time + 1 = b.event_time
WHERE a.ev_type LIKE '%enter%';

-- SELECT * FROM event_position 
-- WHERE veh_type like 'CAR'

-- create 'bus_events_osm' from 'view_bus_events' for routing --
drop table if exists bus_events_osm;
create table bus_events_osm as
select a.veh_id, a.ev_type, a.event_time, a.from_id, a.to_id, a.row as leg_id,
aa.node_id as source, ab.node_id as target
from view_bus_events a
left join matsim_node_lut aa
on a.from_id = aa.matsim_id
left join matsim_node_lut ab
on a.to_id = ab.matsim_id;
-- order by veh_id, leg_id;

update bus_events_osm 
set source = target, from_id = to_id
where ev_type in ('wait2link', 'vehicle leaves traffic');

-- select * from bus_events_osm where veh_id = 'Veh1001';

-- select * from bus_events_osm where from_id = 'tr_10736';

-- create bus unique legs for not routing multiple times --
drop table if exists bus_unique_legs;
create table bus_unique_legs as
select distinct source, target, ev_type 
from bus_events_osm
where ev_type not in ('wait2link', 'vehicle leaves traffic');

-- create routings from unique legs --
drop table if exists bus_unique_routings;
create table bus_unique_routings as
select a.*,
seq, id1 AS node, id2 AS edge, cost 
from bus_unique_legs as a,
pgr_dijkstra('
SELECT gid::integer AS id,
start_id::integer as source,
end_id::integer as target,
length_m::double precision AS cost
FROM road_ext',
a.source::integer, a.target::integer, false, false);
-- order by source, target, seq;

-- putting together the routing with the event -- 
drop table if exists bus_events_routing;
create table bus_events_routing as
select veh_id, a.ev_type, event_time, from_id, to_id, leg_id, seq, node as node_id, the_geom
from bus_events_osm a, bus_unique_routings b
join node_geom c
on b.node = c.node_id
where a.source = b.source
and a.target = b.target;
-- and veh_id = 'Veh01';

-- select * from bus_events_routing where veh_id = 'Veh01' ORDER BY veh_id, leg_id, seq;

-- extract elapsed time for each leg --
drop table if exists bus_events_routing_elapsed_time;
create table bus_events_routing_elapsed_time as 
SELECT a.veh_id, a.leg_id, b.event_time - a.event_time as elapsed_time
FROM bus_events_osm a
LEFT JOIN bus_events_osm b
ON a.leg_id + 1 = b.leg_id
WHERE a.veh_id = b.veh_id;

-- select * from bus_events_routing_elapsed_time ORDER BY veh_id, leg_id;

drop table if exists bus_events_routing_interp_prepare; 
create table bus_events_routing_interp_prepare as 
SELECT foo.*, boo.event_time - 1 as next_time FROM 
(
SELECT a.veh_id, a.ev_type, a.leg_id, a.event_time as event_time, 
ST_MakeLine(the_geom ORDER BY seq) as line_geom
FROM bus_events_routing a
group by a.veh_id, a.ev_type, a.leg_id, event_time
) as foo, bus_events_osm as boo
WHERE foo.veh_id = boo.veh_id 
AND foo.leg_id + 1 = boo.leg_id;

-- select * from bus_events_routing_interp_prepare where ev_type like '%leaves%' ORDER BY veh_id, leg_id;

-- interpolate bus positions for pgrouting -- -- 16 hours --
drop table if exists bus_events_routing_interp; 
CREATE TABLE bus_events_routing_interp as
with recursive interp as
(
    SELECT veh_id, ev_type, leg_id, event_time, line_geom, 0 as n, next_time
        FROM bus_events_routing_interp_prepare
    UNION all
      SELECT a.veh_id, a.ev_type, a.leg_id, a.event_time, a.line_geom, n+1 as n, a.next_time
      FROM interp a
      WHERE a.n + a.event_time < a.next_time
      AND a.ev_type not like '%leaves%'
)
SELECT
a.veh_id, a.ev_type, a.leg_id, a.event_time+a.n as event_time, 
CASE WHEN eet.elapsed_time > 0 then ST_LineInterpolatePoint(a.line_geom, (a.n)/eet.elapsed_time) 
ELSE ST_StartPoint(line_geom) END as the_geom
FROM interp a
LEFT JOIN bus_events_routing_elapsed_time eet
ON a.veh_id = eet.veh_id
AND a.leg_id = eet.leg_id;

-- select * from bus_events_routing_interp ORDER BY veh_id, leg_id, event_time limit 100 ;

-- put together events with and without routings -- -- 25 min --
drop table if exists bus_events_total;
CREATE TABLE bus_events_total as
SELECT dense_rank() over (ORDER BY veh_id, leg_id, event_time) as row, * FROM 
(
SELECT * FROM bus_events_routing_interp
UNION
SELECT veh_id, ev_type, leg_id, event_time,
CASE WHEN c.node_id is null then b.geom
ELSE c.the_geom END as the_geom
FROM bus_events_osm a 
LEFT JOIN matsimnodes_out_of_bounds b
ON a.from_id = b.matsim_id 
LEFT JOIN matsim_node_lut c
ON a.from_id = c.matsim_id
WHERE not exists
(
  SELECT null FROM bus_unique_routings d WHERE a.source = d.source AND a.target = d.target
)
) as foo;

-- prepare the second part of interpolation. -- -- 11 min --
drop table if exists bus_events_interp_second;
create table bus_events_interp_second as
select a.veh_id, a.ev_type, a.leg_id, a.event_time, b.event_time - 1 as next_time,
st_makeline(a.the_geom, b.the_geom) as line_geom
from bus_events_total a
join bus_events_total b
ON a.row + 1 = b.row
AND a.veh_id = b.veh_id
AND a.event_time + 1 != b.event_time
AND a.ev_type not like '%leaves%';

-- select * from bus_events_interp_second;

-- interpolate the remaining car positions. --
drop table if exists bus_events_interp_total;
create table bus_events_interp_total as
WITH RECURSIVE interp AS 
(
    SELECT veh_id, ev_type, event_time, 0 as n, leg_id, line_geom, next_time
    FROM bus_events_interp_second
  UNION ALL
    SELECT t.veh_id, t.ev_type, t.event_time, n+1 as n, t.leg_id, t.line_geom, t.next_time
	FROM interp as t
    WHERE t.event_time + n < t.next_time 
)
-- SELECT * FROM interp ORDER BY veh_id, leg_id, event_time, n ;
SELECT veh_id, ev_type, leg_id, event_time + n as event_time,
ST_LineInterpolatePoint(line_geom, n/(next_time-event_time)) as the_geom
FROM interp;

-- select st_y(the_geom), st_x(the_geom), * from bus_events_interp_total ORDER BY veh_id, leg_id, event_time ;

-- put together the interpolation from the 2 passes --
drop table if exists bus_events_interp_final;
CREATE TABLE bus_events_interp_final as
SELECT st_y(the_geom) as y, st_x(the_geom) as x, * FROM 
(
SELECT * FROM bus_events_routing_interp
UNION
SELECT * FROM bus_events_interp_total
) as foo;

-- insert the bus position in the 
INSERT INTO event_position
SELECT 'BUS'::text as veh_type, a.veh_id, a.event_time, a.y, a.x, 
CASE WHEN ST_Azimuth(a.the_geom, b.the_geom) > 0 then degrees(ST_Azimuth(a.the_geom, b.the_geom))
ELSE 0
END as deg
FROM bus_events_interp_final a
LEFT JOIN bus_events_interp_final b
ON a.veh_id = b.veh_id
AND a.event_time + 1 = b.event_time
WHERE a.ev_type LIKE '%enter%'
ORDER BY veh_id, event_time ;

-- select * from event_position where veh_type like 'BUS' limit 1;

-- osm_nodes that are used in routing.
-- select * from 
-- (
-- select a.node as node_id, b.osm_source as osm_node_id from unique_routings a
-- join road_ext b
-- on a.node = b.start_id
-- union
-- select a.node as node_id, b.osm_target as osm_node_id from unique_routings a
-- join road_ext b
-- on a.node = b.end_id
-- union
-- select a.node as node_id, b.osm_source as osm_node_id from bus_unique_routings a
-- join road_ext b
-- on a.node = b.start_id
-- union
-- select a.node as node_id, b.osm_target as osm_node_id from bus_unique_routings a
-- join road_ext b
-- on a.node = b.end_id
-- ) as foo
-- where osm_node_id is not null;