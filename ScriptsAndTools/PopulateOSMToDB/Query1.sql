-- Dummy call --
SET statement_timeout = 0;

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- TOC entry 215 (class 3079 OID 11750)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;

CREATE EXTENSION IF NOT EXISTS postgis;

--
-- TOC entry 2190 (class 0 OID 0)
-- Dependencies: 215
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 170 (class 1259 OID 16394)
-- Name: node_tags; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE node_tags (
    node_id bigint NOT NULL,
    key character varying(510) NOT NULL,
    value character varying(510)
);


ALTER TABLE public.node_tags OWNER TO postgres;

--
-- TOC entry 171 (class 1259 OID 16400)
-- Name: relation_tags; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE relation_tags (
    relation_id bigint NOT NULL,
    key character varying(510) NOT NULL,
    value character varying(510)
);


ALTER TABLE public.relation_tags OWNER TO postgres;

--
-- TOC entry 172 (class 1259 OID 16406)
-- Name: viewnodetaginfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewnodetaginfoaram AS
 SELECT t.node_id AS nodeid,
    t.key AS name,
    t.value AS info
   FROM node_tags t;


ALTER TABLE public.viewnodetaginfoaram OWNER TO postgres;

--
-- TOC entry 173 (class 1259 OID 16410)
-- Name: viewrelationtaginfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewrelationtaginfoaram AS
 SELECT w.relation_id AS relationid,
    w.key AS name,
    w.value AS info
   FROM relation_tags w;


ALTER TABLE public.viewrelationtaginfoaram OWNER TO postgres;

--
-- TOC entry 174 (class 1259 OID 16414)
-- Name: way_tags; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE way_tags (
    way_id bigint NOT NULL,
    key character varying(510) NOT NULL,
    value character varying(510)
);


ALTER TABLE public.way_tags OWNER TO postgres;

--
-- TOC entry 175 (class 1259 OID 16420)
-- Name: viewwaytaginfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewwaytaginfoaram AS
 SELECT w.way_id AS wayid,
    w.key AS name,
    w.value AS info
   FROM way_tags w;


ALTER TABLE public.viewwaytaginfoaram OWNER TO postgres;

--
-- TOC entry 176 (class 1259 OID 16424)
-- Name: alltags; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW alltags AS
 SELECT alltags.id,
    alltags.name,
    alltags.info,
    alltags.type
   FROM ( SELECT t.nodeid AS id,
            t.name,
            t.info,
            'Node'::text AS type
           FROM viewnodetaginfoaram t
        UNION ALL
         SELECT t.wayid AS id,
            t.name,
            t.info,
            'Way'::text AS type
           FROM viewwaytaginfoaram t
        UNION ALL
         SELECT t.relationid AS id,
            t.name,
            t.info,
            'Relation'::text AS type
           FROM viewrelationtaginfoaram t) alltags;


ALTER TABLE public.alltags OWNER TO postgres;

--
-- TOC entry 177 (class 1259 OID 16428)
-- Name: boundedlist; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE boundedlist (
    way_id bigint
);


ALTER TABLE public.boundedlist OWNER TO postgres;

--
-- TOC entry 178 (class 1259 OID 16431)
-- Name: connection_node; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE connection_node (
    nodeid bigint,
    occurence bigint
);


ALTER TABLE public.connection_node OWNER TO postgres;

--
-- TOC entry 179 (class 1259 OID 16434)
-- Name: gapslabs_maxspeed_for_imobility; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE gapslabs_maxspeed_for_imobility (
    way_id bigint,
    key character varying(510),
    value character varying(510)
);


ALTER TABLE public.gapslabs_maxspeed_for_imobility OWNER TO postgres;

--
-- TOC entry 180 (class 1259 OID 16440)
-- Name: gapslabs_network_for_imobility; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE gapslabs_network_for_imobility (
    wayid bigint,
    nodeid bigint,
    latitude double precision,
    longitude double precision,
    sort integer,
    originalwayid bigint,
    key character varying(510),
    value character varying(510)
);


ALTER TABLE public.gapslabs_network_for_imobility OWNER TO postgres;

--
-- TOC entry 181 (class 1259 OID 16446)
-- Name: sequence_geometry_collection; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE sequence_geometry_collection
    START WITH 0
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.sequence_geometry_collection OWNER TO postgres;

--
-- TOC entry 2191 (class 0 OID 0)
-- Dependencies: 181
-- Name: SEQUENCE sequence_geometry_collection; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON SEQUENCE sequence_geometry_collection IS 'The sequence for the geometryCollection id.';


--
-- TOC entry 182 (class 1259 OID 16448)
-- Name: geometry_collection; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE geometry_collection (
    id bigint DEFAULT nextval('sequence_geometry_collection'::regclass) NOT NULL,
    name text,
    version_title text,
    major integer,
    minor integer,
    format text,
    large_object_reference oid,
    latitude integer,
    longitude integer,
    pivotx numeric,
    pivoty numeric,
    pivotz numeric,
    gis_id integer,
    gis_type text,
    last_update timestamp without time zone
);


ALTER TABLE public.geometry_collection OWNER TO postgres;

--
-- TOC entry 2192 (class 0 OID 0)
-- Dependencies: 182
-- Name: TABLE geometry_collection; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE geometry_collection IS 'This table, stores the meta data regarding the large objects for the geometrical data.
Note that the binary content of those objects are stored in largeobject table while only a reference to those objects is stored in this table.

An example for such a record holds 3D geometry of buildings and roads in OBJ standard format.';


--
-- TOC entry 2193 (class 0 OID 0)
-- Dependencies: 182
-- Name: COLUMN geometry_collection.id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN geometry_collection.id IS 'Unique id.';


--
-- TOC entry 2194 (class 0 OID 0)
-- Dependencies: 182
-- Name: COLUMN geometry_collection.version_title; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN geometry_collection.version_title IS 'This holds the title for the versioning. ex: GaPSlabs OSM';


--
-- TOC entry 2195 (class 0 OID 0)
-- Dependencies: 182
-- Name: COLUMN geometry_collection.format; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN geometry_collection.format IS 'ex: obj, collada, txt, rar, zip, 7z etc.';


--
-- TOC entry 2196 (class 0 OID 0)
-- Dependencies: 182
-- Name: COLUMN geometry_collection.large_object_reference; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN geometry_collection.large_object_reference IS 'The reference to the largeobject';


--
-- TOC entry 2197 (class 0 OID 0)
-- Dependencies: 182
-- Name: COLUMN geometry_collection.pivotx; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN geometry_collection.pivotx IS 'Pivot position of the object in the world units.

this is a floating point number with';


--
-- TOC entry 2198 (class 0 OID 0)
-- Dependencies: 182
-- Name: COLUMN geometry_collection.gis_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN geometry_collection.gis_id IS 'If this object is referencing any gis element (ie, way, node etc in the openStreetmap), it can be stored here.';


--
-- TOC entry 2199 (class 0 OID 0)
-- Dependencies: 182
-- Name: COLUMN geometry_collection.gis_type; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN geometry_collection.gis_type IS 'Type of the gis id. for ex, way, node etc.';


--
-- TOC entry 183 (class 1259 OID 16455)
-- Name: links; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE links (
    id bigint NOT NULL,
    way_ext_id bigint NOT NULL,
    connected_way_ext_id bigint,
    next_prev_enum smallint,
    highway character varying(510)
);


ALTER TABLE public.links OWNER TO postgres;

--
-- TOC entry 2200 (class 0 OID 0)
-- Dependencies: 183
-- Name: TABLE links; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE links IS 'This table holds the graph data for the way_ext table.';


--
-- TOC entry 2201 (class 0 OID 0)
-- Dependencies: 183
-- Name: COLUMN links.connected_way_ext_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN links.connected_way_ext_id IS 'This hold the id of the connected way. It may be the next or previous way according to next_prev_enum';


--
-- TOC entry 2202 (class 0 OID 0)
-- Dependencies: 183
-- Name: COLUMN links.next_prev_enum; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN links.next_prev_enum IS '0 for next
1 for previous

ex:
id - way_ext_id - connected_way_ext_id - next_prev_enum
1 - 123 - 125 - 0
this means the next way at the end of 123 is 125.


for a 2 way street, the id of those streets will be different.';


--
-- TOC entry 184 (class 1259 OID 16461)
-- Name: networkways; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE networkways (
    way_id bigint
);


ALTER TABLE public.networkways OWNER TO postgres;

--
-- TOC entry 185 (class 1259 OID 16464)
-- Name: node; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE node (
    id bigint NOT NULL,
    latitude integer NOT NULL,
    longitude integer NOT NULL,
    changeset_id bigint,
    visible boolean,
    "timestamp" timestamp without time zone,
    tile bigint NOT NULL,
    version integer,
    usr character varying(510),
    usr_id integer
);


ALTER TABLE public.node OWNER TO postgres;

--
-- TOC entry 186 (class 1259 OID 16470)
-- Name: node_stockholm; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE node_stockholm (
    id bigint,
    latitude integer,
    longitude integer,
    changeset_id bigint,
    visible boolean,
    "timestamp" timestamp without time zone,
    tile bigint,
    version integer,
    usr character varying(510),
    usr_id integer
);


ALTER TABLE public.node_stockholm OWNER TO postgres;

--
-- TOC entry 187 (class 1259 OID 16476)
-- Name: nodetaginfoaram_cachedfromview; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE nodetaginfoaram_cachedfromview (
    nodeid bigint,
    name character varying(510),
    info character varying(510)
);


ALTER TABLE public.nodetaginfoaram_cachedfromview OWNER TO postgres;

--
-- TOC entry 188 (class 1259 OID 16482)
-- Name: relation; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE relation (
    id bigint NOT NULL,
    changeset_id bigint,
    "timestamp" timestamp without time zone,
    visible boolean,
    version integer,
    usr character varying(510),
    usr_id integer
);


ALTER TABLE public.relation OWNER TO postgres;

--
-- TOC entry 189 (class 1259 OID 16488)
-- Name: relation_members; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE relation_members (
    relation_id bigint NOT NULL,
    member_type integer NOT NULL,
    member_id bigint NOT NULL,
    member_role character varying(510),
    sequence_id integer NOT NULL
);


ALTER TABLE public.relation_members OWNER TO postgres;

--
-- TOC entry 190 (class 1259 OID 16494)
-- Name: sequence_way_ext_wayid; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE sequence_way_ext_wayid
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.sequence_way_ext_wayid OWNER TO postgres;

--
-- TOC entry 191 (class 1259 OID 16496)
-- Name: way_tags_stockholm; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE way_tags_stockholm (
    way_id bigint,
    key character varying(510),
    value character varying(510)
);


ALTER TABLE public.way_tags_stockholm OWNER TO postgres;

--
-- TOC entry 192 (class 1259 OID 16502)
-- Name: viewallowedtags; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewallowedtags AS
 SELECT DISTINCT way_tags_stockholm.value
   FROM way_tags_stockholm
  WHERE (((way_tags_stockholm.key)::text = 'highway'::text) AND ((way_tags_stockholm.value)::text <> ALL (ARRAY[('footway'::character varying)::text, ('stairs'::character varying)::text, ('cycleway'::character varying)::text, ('raceway'::character varying)::text, ('platform'::character varying)::text, ('pedestrian'::character varying)::text, ('proposed'::character varying)::text, ('ski_jump'::character varying)::text, ('elevator'::character varying)::text, ('escalator'::character varying)::text, ('desire'::character varying)::text])));


ALTER TABLE public.viewallowedtags OWNER TO postgres;

--
-- TOC entry 193 (class 1259 OID 16506)
-- Name: viewbounds; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewbounds AS
 SELECT 691310000 AS maxlat,
    546480000 AS minlat,
    246090000 AS maxlon,
    108980000 AS minlon;


ALTER TABLE public.viewbounds OWNER TO postgres;

--
-- TOC entry 194 (class 1259 OID 16510)
-- Name: viewbounds_real; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewbounds_real AS
 SELECT max(node.latitude) AS maxlat,
    min(node.latitude) AS minlat,
    max(node.longitude) AS maxlon,
    min(node.longitude) AS minlon
   FROM node;


ALTER TABLE public.viewbounds_real OWNER TO postgres;

--
-- TOC entry 195 (class 1259 OID 16514)
-- Name: viewbounds_stockholm; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewbounds_stockholm AS
 SELECT max(node_stockholm.latitude) AS maxlat,
    min(node_stockholm.latitude) AS minlat,
    max(node_stockholm.longitude) AS maxlon,
    min(node_stockholm.longitude) AS minlon
   FROM node_stockholm;


ALTER TABLE public.viewbounds_stockholm OWNER TO postgres;

--
-- TOC entry 196 (class 1259 OID 16518)
-- Name: waynodeinfoaram_cachedfromview; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE waynodeinfoaram_cachedfromview (
    wayid bigint,
    nodeid bigint,
    latitude integer,
    longitude integer,
    sort integer
);


ALTER TABLE public.waynodeinfoaram_cachedfromview OWNER TO postgres;

--
-- TOC entry 197 (class 1259 OID 16521)
-- Name: viewconnection_node; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewconnection_node AS
 SELECT l.nodeid,
    sum(l.occurence) AS occurence
   FROM ( SELECT connections.nodeid,
            connections.occurence
           FROM ( SELECT waynodeinfoaram_cachedfromview.nodeid,
                    count(waynodeinfoaram_cachedfromview.nodeid) AS occurence
                   FROM (waynodeinfoaram_cachedfromview
                     LEFT JOIN way_tags_stockholm ON ((waynodeinfoaram_cachedfromview.wayid = way_tags_stockholm.way_id)))
                  WHERE ((way_tags_stockholm.key)::text = 'highway'::text)
                  GROUP BY waynodeinfoaram_cachedfromview.nodeid) connections
        UNION ALL
         SELECT nodeconnections.node_id AS nodeid,
            nodeconnections.occurence
           FROM ( SELECT DISTINCT node_tags.node_id,
                    1 AS occurence
                   FROM (node_stockholm
                     LEFT JOIN node_tags ON ((node_stockholm.id = node_tags.node_id)))
                  WHERE ((node_tags.key)::text = ANY (ARRAY[('public_transport'::character varying)::text, ('traffic_signals'::character varying)::text]))) nodeconnections) l
  WHERE (l.occurence > 1)
  GROUP BY l.nodeid;


ALTER TABLE public.viewconnection_node OWNER TO postgres;

--
-- TOC entry 198 (class 1259 OID 16526)
-- Name: wayextnodeinfoaram_cachedfromview; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE wayextnodeinfoaram_cachedfromview (
    wayid bigint,
    nodeid bigint,
    latitude integer,
    longitude integer,
    sort integer,
    originalwayid bigint
);


ALTER TABLE public.wayextnodeinfoaram_cachedfromview OWNER TO postgres;

--
-- TOC entry 199 (class 1259 OID 16529)
-- Name: viewgapslabs_network; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewgapslabs_network AS
 SELECT we.wayid,
    we.nodeid,
    ((we.latitude)::double precision / (10000000)::double precision) AS latitude,
    ((we.longitude)::double precision / (10000000)::double precision) AS longitude,
    we.sort,
    we.originalwayid,
    wt.key,
    wt.value
   FROM (wayextnodeinfoaram_cachedfromview we
     JOIN way_tags_stockholm wt ON ((((we.originalwayid = wt.way_id) AND ((wt.key)::text = 'highway'::text)) AND ((wt.value)::text IN ( SELECT viewallowedtags.value
           FROM viewallowedtags)))));


ALTER TABLE public.viewgapslabs_network OWNER TO postgres;

--
-- TOC entry 200 (class 1259 OID 16534)
-- Name: viewmaxspeed; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewmaxspeed AS
 SELECT way_tags_stockholm.way_id,
    way_tags_stockholm.key,
    way_tags_stockholm.value
   FROM way_tags_stockholm
  WHERE ((way_tags_stockholm.key)::text = ANY (ARRAY[('maxspeed:backward'::character varying)::text, ('maxspeed:forward'::character varying)::text, ('maxspeed:sign'::character varying)::text, ('source:maxspeed'::character varying)::text, ('maxspeed'::character varying)::text, ('maxspeed:conditional'::character varying)::text]));


ALTER TABLE public.viewmaxspeed OWNER TO postgres;

--
-- TOC entry 201 (class 1259 OID 16538)
-- Name: viewnetworkways; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewnetworkways AS
 SELECT DISTINCT way_tags_stockholm.way_id
   FROM way_tags_stockholm
  WHERE ((way_tags_stockholm.key)::text = 'highway'::text);


ALTER TABLE public.viewnetworkways OWNER TO postgres;

--
-- TOC entry 214 (class 1259 OID 16909)
-- Name: viewrelationbuildings; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewrelationbuildings AS
 SELECT mn.relation_id AS relationid,
    mn.member_id AS wayid,
    wn.nodeid,
    wn.latitude,
    wn.longitude
   FROM (( SELECT rm.relation_id,
            rm.member_id
           FROM ( SELECT DISTINCT rt_1.relationid
                   FROM viewrelationtaginfoaram rt_1
                  WHERE ((rt_1.name)::text = 'building'::text)) rt,
            relation_members rm
          WHERE ((rm.relation_id = rt.relationid) AND ((rm.member_role)::text = 'outer'::text))) mn
     LEFT JOIN waynodeinfoaram_cachedfromview wn ON ((mn.member_id = wn.wayid)))
  WHERE (wn.wayid IS NOT NULL);


ALTER TABLE public.viewrelationbuildings OWNER TO postgres;

--
-- TOC entry 202 (class 1259 OID 16542)
-- Name: viewrelationinfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewrelationinfoaram AS
 SELECT r.relation_id AS relationid,
    r.member_id AS ref,
    r.member_type AS type,
    r.member_role AS role,
    r.sequence_id AS sort
   FROM relation_members r;


ALTER TABLE public.viewrelationinfoaram OWNER TO postgres;

--
-- TOC entry 203 (class 1259 OID 16546)
-- Name: viewway_direction; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewway_direction AS
 SELECT way_tags_stockholm.way_id,
        CASE
            WHEN (((way_tags_stockholm.value)::text = '1'::text) OR ((way_tags_stockholm.value)::text = 'yes'::text)) THEN '1'::text
            WHEN ((way_tags_stockholm.value)::text = '-1'::text) THEN '-1'::text
            WHEN (((way_tags_stockholm.value)::text = '0'::text) OR ((way_tags_stockholm.value)::text = 'no'::text)) THEN '0'::text
            ELSE '0'::text
        END AS direction
   FROM way_tags_stockholm
  WHERE ((way_tags_stockholm.key)::text = 'oneway'::text);


ALTER TABLE public.viewway_direction OWNER TO postgres;

--
-- TOC entry 204 (class 1259 OID 16550)
-- Name: way_ext; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE way_ext (
    way_id bigint NOT NULL,
    nodes_id bigint NOT NULL,
    sequence_id integer NOT NULL,
    original_way_id bigint NOT NULL,
    original_sequence_id_st integer,
    original_sequence_id_end integer NOT NULL
);


ALTER TABLE public.way_ext OWNER TO postgres;

--
-- TOC entry 2203 (class 0 OID 0)
-- Dependencies: 204
-- Name: TABLE way_ext; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE way_ext IS 'This is the extended way table generated from original openstreetmap data.

It provides atomic links that do not have any other ways connected to them except to the tails.';


--
-- TOC entry 205 (class 1259 OID 16553)
-- Name: viewwayextnodeinfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewwayextnodeinfoaram AS
 SELECT wn.way_id AS wayid,
    n.id AS nodeid,
    n.latitude,
    n.longitude,
    wn.sequence_id AS sort,
    wn.original_way_id AS originalwayid
   FROM (way_ext wn
     LEFT JOIN node n ON ((wn.nodes_id = n.id)));


ALTER TABLE public.viewwayextnodeinfoaram OWNER TO postgres;

--
-- TOC entry 206 (class 1259 OID 16557)
-- Name: way_nodes_stockholm; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE way_nodes_stockholm (
    way_id bigint,
    node_id bigint,
    sequence_id integer
);


ALTER TABLE public.way_nodes_stockholm OWNER TO postgres;

--
-- TOC entry 207 (class 1259 OID 16560)
-- Name: viewwaynodeinfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewwaynodeinfoaram AS
 SELECT wn.way_id AS wayid,
    n.id AS nodeid,
    n.latitude,
    n.longitude,
    wn.sequence_id AS sort
   FROM (way_nodes_stockholm wn
     LEFT JOIN node n ON ((wn.node_id = n.id)));


ALTER TABLE public.viewwaynodeinfoaram OWNER TO postgres;

--
-- TOC entry 208 (class 1259 OID 16564)
-- Name: viewwaytaginrelationinfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewwaytaginrelationinfoaram AS
 SELECT rm.member_id AS wayid,
    rt.key AS name,
    rt.value AS info
   FROM (relation_members rm
     LEFT JOIN relation_tags rt ON ((rm.relation_id = rt.relation_id)))
  WHERE ((((rm.member_type)::text = 'Way'::text) AND ((rm.member_role)::text = 'outer'::text)) AND ((rt.key)::text = 'building'::text));


ALTER TABLE public.viewwaytaginrelationinfoaram OWNER TO postgres;

--
-- TOC entry 209 (class 1259 OID 16568)
-- Name: viewwaytagstockholminfoaram; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW viewwaytagstockholminfoaram AS
 SELECT w.way_id AS wayid,
    w.key AS name,
    w.value AS info
   FROM way_tags_stockholm w;


ALTER TABLE public.viewwaytagstockholminfoaram OWNER TO postgres;

--
-- TOC entry 210 (class 1259 OID 16572)
-- Name: way; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE way (
    id bigint NOT NULL,
    changeset_id bigint,
    "timestamp" timestamp without time zone,
    visible boolean,
    version integer,
    usr character varying(510),
    usr_id integer
);


ALTER TABLE public.way OWNER TO postgres;

--
-- TOC entry 211 (class 1259 OID 16578)
-- Name: way_direction; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE way_direction (
    way_id bigint,
    direction text
);


ALTER TABLE public.way_direction OWNER TO postgres;

--
-- TOC entry 212 (class 1259 OID 16584)
-- Name: way_nodes; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE way_nodes (
    way_id bigint NOT NULL,
    node_id bigint NOT NULL,
    sequence_id integer NOT NULL
);


ALTER TABLE public.way_nodes OWNER TO postgres;

--
-- TOC entry 213 (class 1259 OID 16587)
-- Name: waytaginrelationinfoaram; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE waytaginrelationinfoaram (
    wayid bigint,
    name character varying(510),
    info character varying(510)
);


ALTER TABLE public.waytaginrelationinfoaram OWNER TO postgres;

--
-- TOC entry 2023 (class 2606 OID 16598)
-- Name: pk_geometryCollection; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY geometry_collection
    ADD CONSTRAINT "pk_geometryCollection" PRIMARY KEY (id);


--
-- TOC entry 2029 (class 2606 OID 16600)
-- Name: pk_node; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY node
    ADD CONSTRAINT pk_node PRIMARY KEY (id);


--
-- TOC entry 2034 (class 2606 OID 16602)
-- Name: pk_relation; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY relation
    ADD CONSTRAINT pk_relation PRIMARY KEY (id);


--
-- TOC entry 2050 (class 2606 OID 16604)
-- Name: pk_way; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY way
    ADD CONSTRAINT pk_way PRIMARY KEY (id);


--
-- TOC entry 2025 (class 2606 OID 16606)
-- Name: primary_key_links; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY links
    ADD CONSTRAINT primary_key_links PRIMARY KEY (id);


--
-- TOC entry 2020 (class 1259 OID 16607)
-- Name: index_geometryCollection_gistype; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX "index_geometryCollection_gistype" ON geometry_collection USING btree (gis_type);


--
-- TOC entry 2021 (class 1259 OID 16608)
-- Name: index_geometryCollection_versionTitle_majorminor_format; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX "index_geometryCollection_versionTitle_majorminor_format" ON geometry_collection USING btree (version_title, major, minor, format);


--
-- TOC entry 2026 (class 1259 OID 16609)
-- Name: index_node_latitude; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_node_latitude ON node USING btree (latitude);


--
-- TOC entry 2027 (class 1259 OID 16610)
-- Name: index_node_longitude; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_node_longitude ON node USING btree (longitude);


--
-- TOC entry 2030 (class 1259 OID 16611)
-- Name: index_node_stockholm_latitude; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_node_stockholm_latitude ON node_stockholm USING btree (latitude);


--
-- TOC entry 2031 (class 1259 OID 16613)
-- Name: index_node_stockholm_longitude; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_node_stockholm_longitude ON node_stockholm USING btree (longitude);


--
-- TOC entry 2010 (class 1259 OID 16614)
-- Name: index_node_tags_key; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_node_tags_key ON node_tags USING btree (key);


--
-- TOC entry 2011 (class 1259 OID 16615)
-- Name: index_node_tags_key_value_pair; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_node_tags_key_value_pair ON node_tags USING btree (key, value);


--
-- TOC entry 2012 (class 1259 OID 16616)
-- Name: index_node_tags_value; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_node_tags_value ON node_tags USING btree (value);


--
-- TOC entry 2032 (class 1259 OID 16617)
-- Name: index_relation; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_relation ON relation USING btree (id);


--
-- TOC entry 2035 (class 1259 OID 16618)
-- Name: index_relation_members; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_relation_members ON relation_members USING btree (relation_id, member_id, sequence_id);


--
-- TOC entry 2013 (class 1259 OID 16619)
-- Name: index_relation_tags_key; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_relation_tags_key ON relation_tags USING btree (key);


--
-- TOC entry 2014 (class 1259 OID 16620)
-- Name: index_relation_tags_key_value_pair; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_relation_tags_key_value_pair ON relation_tags USING btree (key, value);


--
-- TOC entry 2015 (class 1259 OID 16621)
-- Name: index_relation_tags_value; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_relation_tags_value ON relation_tags USING btree (value);


--
-- TOC entry 2051 (class 1259 OID 16622)
-- Name: index_way_direction_direction; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_direction_direction ON way_direction USING btree (direction);


--
-- TOC entry 2045 (class 1259 OID 16623)
-- Name: index_way_ext_nodes_id; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_ext_nodes_id ON way_ext USING btree (nodes_id);


--
-- TOC entry 2046 (class 1259 OID 16624)
-- Name: index_way_ext_original_way_id; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_ext_original_way_id ON way_ext USING btree (original_way_id);


--
-- TOC entry 2047 (class 1259 OID 16625)
-- Name: index_way_ext_way_id; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_ext_way_id ON way_ext USING btree (way_id);


--
-- TOC entry 2048 (class 1259 OID 16626)
-- Name: index_way_nodes_stockholm_wayid; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_nodes_stockholm_wayid ON way_nodes_stockholm USING btree (way_id);


--
-- TOC entry 2052 (class 1259 OID 16628)
-- Name: index_way_nodes_wayid; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_nodes_wayid ON way_nodes USING btree (way_id);


--
-- TOC entry 2016 (class 1259 OID 16630)
-- Name: index_way_tags_key; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_key ON way_tags USING btree (key);


--
-- TOC entry 2017 (class 1259 OID 16632)
-- Name: index_way_tags_key_value_pair; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_key_value_pair ON way_tags USING btree (key, value);


--
-- TOC entry 2036 (class 1259 OID 16635)
-- Name: index_way_tags_stockholm_key; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_stockholm_key ON way_tags_stockholm USING btree (key);


--
-- TOC entry 2037 (class 1259 OID 16636)
-- Name: index_way_tags_stockholm_key_value_pair; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_stockholm_key_value_pair ON way_tags_stockholm USING btree (key, value);


--
-- TOC entry 2038 (class 1259 OID 16637)
-- Name: index_way_tags_stockholm_value; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_stockholm_value ON way_tags_stockholm USING btree (value);


--
-- TOC entry 2039 (class 1259 OID 16638)
-- Name: index_way_tags_stockholm_way_id; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_stockholm_way_id ON way_tags_stockholm USING btree (way_id);


--
-- TOC entry 2018 (class 1259 OID 16639)
-- Name: index_way_tags_value; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_value ON way_tags USING btree (value);


--
-- TOC entry 2019 (class 1259 OID 16640)
-- Name: index_way_tags_way_id; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_way_tags_way_id ON way_tags USING btree (way_id);


--
-- TOC entry 2042 (class 1259 OID 16641)
-- Name: index_wayextnodeinfoaram_cachedfromview_nodeid; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_wayextnodeinfoaram_cachedfromview_nodeid ON wayextnodeinfoaram_cachedfromview USING btree (nodeid, latitude, longitude);


--
-- TOC entry 2043 (class 1259 OID 16642)
-- Name: index_wayextnodeinfoaram_cachedfromview_wayid; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_wayextnodeinfoaram_cachedfromview_wayid ON wayextnodeinfoaram_cachedfromview USING btree (wayid);


--
-- TOC entry 2044 (class 1259 OID 16643)
-- Name: index_wayextnodeinfoaram_cachedfromview_wayid_originalwayid; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_wayextnodeinfoaram_cachedfromview_wayid_originalwayid ON wayextnodeinfoaram_cachedfromview USING btree (wayid, originalwayid);


--
-- TOC entry 2040 (class 1259 OID 16644)
-- Name: index_waynodeinfoaram_cachedfromview_nodeid; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_waynodeinfoaram_cachedfromview_nodeid ON waynodeinfoaram_cachedfromview USING btree (nodeid, latitude, longitude);


--
-- TOC entry 2041 (class 1259 OID 16645)
-- Name: index_waynodeinfoaram_cachedfromview_wayid; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_waynodeinfoaram_cachedfromview_wayid ON waynodeinfoaram_cachedfromview USING btree (wayid);


--
-- TOC entry 2055 (class 1259 OID 16646)
-- Name: index_waytaginrelationinfoaram_info; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_waytaginrelationinfoaram_info ON waytaginrelationinfoaram USING btree (name);


--
-- TOC entry 2056 (class 1259 OID 16647)
-- Name: index_waytaginrelationinfoaram_name; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX index_waytaginrelationinfoaram_name ON waytaginrelationinfoaram USING btree (name);


--
-- TOC entry 2053 (class 1259 OID 16648)
-- Name: way_nodes_node_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX way_nodes_node_idx ON way_nodes USING btree (node_id);


--
-- TOC entry 2054 (class 1259 OID 16650)
-- Name: way_nodes_way_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX way_nodes_way_idx ON way_nodes USING btree (way_id);


--
-- TOC entry 2189 (class 0 OID 0)
-- Dependencies: 6
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2014-09-24 14:50:18

--
-- PostgreSQL database dump complete
--

