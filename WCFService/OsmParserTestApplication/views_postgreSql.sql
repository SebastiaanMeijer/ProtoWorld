-- View: viewnodetaginfoaram

-- DROP VIEW viewnodetaginfoaram;

create or replace view viewBounds as
select 
	max(latitude) maxLat,
	min(latitude) minLat,
	max(longitude) maxLon,
	min(longitude) minLon 
from node;

ALTER TABLE viewBounds
  OWNER TO postgres;

CREATE OR REPLACE VIEW viewnodetaginfoaram AS 
 SELECT t.node_id AS nodeid, t.key AS "Name", t.value AS info
   FROM node_tags t;

ALTER TABLE viewnodetaginfoaram
  OWNER TO postgres;



-- View: viewrelationinfoaram

-- DROP VIEW viewrelationinfoaram;

CREATE OR REPLACE VIEW viewrelationinfoaram AS 
 SELECT r.relation_id AS relationid, r.member_id AS "Ref", 
    r.member_type AS "Type", r.member_role AS "Role", r.sequence_id AS "Sort"
   FROM relation_members r;

ALTER TABLE viewrelationinfoaram
  OWNER TO postgres;




-- View: viewrelationtaginfoaram

-- DROP VIEW viewrelationtaginfoaram;

CREATE OR REPLACE VIEW viewrelationtaginfoaram AS 
 SELECT w.relation_id AS relationid, w.key AS "Name", w.value AS info
   FROM relation_tags w;

ALTER TABLE viewrelationtaginfoaram
  OWNER TO postgres;



-- View: viewwaynodeinfoaram

-- DROP VIEW viewwaynodeinfoaram;

CREATE OR REPLACE VIEW viewwaynodeinfoaram AS 
 SELECT w.way_id AS wayid, n.id AS nodeid, n.latitude/10000000::float, n.longitude/10000000::float, 
    w.sequence_id AS sort
   FROM way_nodes w
   LEFT JOIN node n ON w.node_id = n.id;

ALTER TABLE viewwaynodeinfoaram
  OWNER TO postgres;



-- View: viewwaytaginfoaram

-- DROP VIEW viewwaytaginfoaram;

CREATE OR REPLACE VIEW viewwaytaginfoaram AS 
 SELECT w.way_id AS wayid, w.key AS "Name", w.value AS info
   FROM way_tags w;

ALTER TABLE viewwaytaginfoaram
  OWNER TO postgres;




-- View: viewwaynodeinfoaram

-- DROP VIEW viewwaynodeinfoaram;

CREATE OR REPLACE VIEW viewwaynodeinfoaram AS 
 SELECT wn.way_id AS wayid, n.id AS nodeif, n.latitude, n.longitude, 
    wn.sequence_id AS sort
   FROM way_nodes wn
   LEFT JOIN node n ON wn.node_id = n.id;

ALTER TABLE viewwaynodeinfoaram
  OWNER TO postgres;

