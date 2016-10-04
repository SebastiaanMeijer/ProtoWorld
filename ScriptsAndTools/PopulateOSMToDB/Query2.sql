-- Specifying the country boundaries
-- NOTE: Modify the given numbers to reflect your own country. This is
-- internally used by the visualization engine to interpolate the coordinate conversions
-- to the 3d coordiante system.
CREATE OR REPLACE VIEW viewbounds AS 
SELECT max(latitude) AS maxlat, 
	min(latitude) AS minlat, 
	max(longitude) AS maxlon, 
	min(longitude) AS minlon FROM node;



-- Populating the selected region into node_stockholm
-- NOTE: Modify the given numbers to reflect your own area of interest.
DELETE FROM node_stockholm;
INSERT INTO node_stockholm 
(
SELECT *
FROM node
-- WHERE 
-- 	latitude BETWEEN 
-- 	(SELECT minlat FROM viewbounds) AND (SELECT maxlat FROM viewbounds)
-- 	AND longitude BETWEEN 
-- 	(SELECT minlon FROM viewbounds) AND (SELECT maxlon FROM viewbounds)
);

-- Populate the boundedlist 

DELETE FROM boundedlist;
INSERT INTO boundedlist 
 (
 SELECT distinct wn.way_id AS wayid--, 
--     n.id AS nodeid, 
--     n.latitude, 
--     n.longitude, 
--     wn.sequence_id AS sort
   FROM way_nodes wn
   RIGHT JOIN node_stockholm n ON wn.node_id = n.id 
 );

-- Populate the way_nodes_stockholm

DELETE FROM way_nodes_stockholm;
INSERT INTO way_nodes_stockholm
(
	SELECT *
	FROM way_nodes w
	WHERE w.way_id IN (SELECT b.way_id FROM boundedlist b)
);


-- Caching the viewwaynodeinfoaram into waynodeinfoaram_cachedfromview

DELETE FROM waynodeinfoaram_cachedfromview;
INSERT INTO waynodeinfoaram_cachedfromview
 (
 SELECT *
 FROM viewwaynodeinfoaram
  );

-- Caching waytaginrelationinfoaram from viewwaytaginrelationinfoaram

DELETE FROM waytaginrelationinfoaram;
INSERT INTO waytaginrelationinfoaram
(
SELECT * FROM viewwaytaginrelationinfoaram
);

-- Populate the way_tags_stockholm

DELETE FROM way_tags_stockholm;
INSERT INTO way_tags_stockholm
(
SELECT * FROM way_tags 
WHERE way_id IN (SELECT way_id FROM boundedlist )
);

-- Populate the nodetaginfoaram_cachedfromview from viewnodetaginfoaram

DELETE FROM nodetaginfoaram_cachedfromview;
INSERT INTO nodetaginfoaram_cachedfromview
 (
 SELECT *
 FROM viewnodetaginfoaram
  );


------------------------------------
-- NOTE: From this point on, all the opeartions are for the 
-- purpose of creating a network from OSM data.
-- These are not used by the visualizer, but the WCF service
-- may in the future provide the corresponding tables for routing algorithms
------------------------------------

-- Populating connection_node from viewconnection_node

DELETE FROM connection_node;
INSERT INTO connection_node
(
SELECT * FROM viewconnection_node
);

-- Populating gapslabs_maxspeed_for_imobility from viewmaxspeed

DELETE FROM gapslabs_maxspeed_for_imobility;
INSERT INTO gapslabs_maxspeed_for_imobility
(
SELECT * FROM viewmaxspeed
);

-- Populating networkways FROM viewnetworkways

DELETE FROM networkways;
INSERT INTO networkways
(
SELECT * FROM viewnetworkways
);


-- Populating way_direction FROM viewway_direction
DELETE FROM  way_direction;
INSERT INTO way_direction
(
SELECT * FROM viewway_direction
);

-- Show the max and min Lat/Lon
SELECT * FROM viewbounds;

------------------------------------------------------------------------------------
-- These need to be run after the road network is generated from the GaPSLabs tools.
------------------------------------------------------------------------------------

-- -- Populating wayextnodeinfoaram_cachedfromview FROM viewwayextnodeinfoaram
-- 
-- DELETE FROM wayextnodeinfoaram_cachedfromview;
-- INSERT INTO wayextnodeinfoaram_cachedfromview
--  (
--  SELECT *
--  FROM viewwayextnodeinfoaram
--   );
-- 
-- -- Populating gapslabs_network_for_imobility FROM viewgapslabs_network
-- 
-- DELETE FROM gapslabs_network_for_imobility;
-- INSERT INTO gapslabs_network_for_imobility
-- (
-- SELECT * FROM viewgapslabs_network
-- );