/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Aram.OSMParser;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.PostgreSQL.SimpleSchema;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Data.Source;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Routing.Route;
using Npgsql;
using NpgsqlTypes;



namespace GapslabWCFservice
{
    /// <summary>
    /// The GaPSLabs Windows Communication Foundation Service Namespace
    /// </summary>
    internal class NamespaceDoc
    {
    }
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ServiceGapslabs.svc or ServiceGapslabs.svc.cs at the Solution Explorer and start debugging.
    /// <summary>
    /// The main class for the GaPSLabs WCF Service. Any method inside this class with the [OperationContract]
    /// attribute will be available to the consumers. Please refer to .NET ServiceModel for details.
    /// </summary>
    [ServiceContract, ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class ServiceGapslabs // : IServiceGapslabs
    {
        /// <summary>
        /// A connection string to MSSQL. All methods in ServiceGapslabs class are required to receive a connection string.
        /// However, The methods can be modified in a way that they would use a global variable that is set at initialization.
        /// </summary>
        public string conn = "Server=localhost\\SQLEXPRESS;Database=GIStest;User Id=gapslabuser;Password=test;";
        /// <summary>
        /// A connection string to PostgreSql. All methods in ServiceGapslabs class are required to receive a connection string, 
        /// so this parameter should be set by the client that is using the services.
        /// However, the methods can also be modified in a way that they would use a global variable that is set at initialization.
        /// </summary>
          public string connPostgre = "Server=127.0.0.1;Port=5432;Database=GIS_Stockholm;User Id=postgres;Password=test;";
        //public string connPostgre = "Server=127.0.0.1;Port=5432;Database=GIS_Paris;User Id=postgres;Password=test;";
        //public string connPostgre = "Server=127.0.0.1;Port=5432;Database=GIS_Parc_des_Princes;User Id=postgres;Password=test;"
        //public string connPostgre = "Server=127.0.0.1;Port=5432;Database=GIS_Vatican;User Id=postgres;Password=test;";


        #region OpenStreetMaps Related Methods
        /// <summary>
        /// Generates the mesh for a given openstreetmap element
        /// </summary>
        /// <param name="OSMid">The id of the OSM element</param>
        /// <param name="MinPointOnMap">Minimum point on the target map (to calculate the offset)</param>
        /// <param name="bounds">The boundaries of the used OSM map</param>
        /// <param name="properties">The details of the target scene generation.</param>
        /// <param name="connPostGreSql">The connection string to the database. Please refer to .Net Data Provider for MSSQL for format specifications</param>
        /// <param name="OSMObjectType">The type of the generated mesh. Refer to <see cref="GaPSLabs.Geometry.MeshGenerator.OSMShape"/> </param>
        /// <param name="RemoveRedundantPointsOnTheSameLine">If true, it will remove redundant points on straight parts of the shape according to<paramref name="RedundantPointErrorThreshold"/>.</param>
        /// <param name="RedundantPointErrorThreshold">The error toleration when removing redundant points. The default is 0.001f</param>
        /// <param name="SegmentLines">If true, it will segment the road geometry so that the texture will look uniform over the whole mesh.</param>
        /// <returns>Returns a <see cref="GaPSLabs.Geometry.GameObject"/> of the generated mesh.</returns>
        /// <remarks>
        /// <para>Removing the redundant points happen before the line segmentation. Therefore the points that were generated when setting <paramref name="SegmentLines"/> to true will not be removed.</para>
        /// <para>The <paramref name="RedundantPointErrorThreshold"/> specifies the difference in slope of the line segments that can be ignored.</para>
        /// </remarks>
        [OperationContract]
        public GaPSLabs.Geometry.GameObject GetOSMmeshFromOsmId(string OSMid, GaPSLabs.Geometry.Vector3 MinPointOnMap, BoundsWCF bounds, GaPSLabs.Geometry.MapProperties properties, string connPostGreSql, string OSMObjectType
            , bool RemoveRedundantPointsOnTheSameLine = true, float RedundantPointErrorThreshold = 0.001f, bool SegmentLines = true)
        {
            if (properties != null)
                GaPSLabs.Geometry.MeshGenerator.Properties = properties;
            Aram.OSMParser.Bounds b = new Bounds();
            b.minlat = bounds.minlat;
            b.maxlat = bounds.maxlat;
            b.minlon = bounds.minlon;
            b.maxlon = bounds.maxlon;
            var typee = (GaPSLabs.Geometry.MeshGenerator.OSMShape)Enum.Parse(typeof(GaPSLabs.Geometry.MeshGenerator.OSMShape), OSMObjectType, true);
            try
            {
                if (!string.IsNullOrEmpty(OSMid))
                {
                    var res = GaPSLabs.Geometry.MeshGenerator.OSMtoGameObject(OSMid, MinPointOnMap, b, null, connPostGreSql, typee
                        , RemoveRedundantPointsOnTheSameLine, RedundantPointErrorThreshold, SegmentLines);
                    // NOTE: this is an experimental task that generates json data out of the generated geometry.
                    //if (res != null)
                    //{
                    //    var jsoni = Newtonsoft.Json.JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented);
                    //    var jsonName = "C:\\Geometry Test\\" + res.Name.Replace("|", "_") + ".json";
                    //    System.IO.File.WriteAllText(jsonName, jsoni);
                    //}
                    return res;

                }
            }
            catch (Exception e)
            {
                //throw e;
            }
            return null;
        }
        /// <summary>
        /// Gets all OSM relation ids from the database.
        /// Note: It may take a long time.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns array of Ids for the OSM Relations.</returns>
        [OperationContract]
        public String[] GetRelationIds(String connectionString)
        {
            //return new OSMSqlSource(connectionString).RelationIds;
            return new OSMPostgresqlSource(connectionString).RelationIds;
        }
        /// <summary>
        /// Gets the OSM relations that contain the provided members with criteria.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="memberIds">The ids of the member to look for</param>
        /// <param name="memberType">The type of the members to look for</param>
        /// <param name="memberRole">The role of the members to look for. Leave with an empty string to ignore.</param>
        /// <returns>Returns a list of relation ids</returns>
        [OperationContract]
        public List<string> GetRelationsContainingMembers(String connectionString, String[] memberIds, int memberType, string memberRole)
        {
            return Aram.OSMParser.Relation.GetRelationsPostgreSQL(memberIds, memberType, memberRole, connectionString);
        }
        /// <summary>
        /// Gets the OSM relations based on a relation id.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="relationId">The relation id to be used in the search criteria.</param>
        /// <returns>Returns the list of OSM relations</returns>
        [OperationContract]
        public List<MemberWCF> GetRelationMembers(String connectionString, String relationId)
        {
            Aram.OSMParser.Relation rel = new Aram.OSMParser.Relation(relationId);
            // return rel.GetMembersSQL(connectionString);
            var members = rel.GetMembersPostgreSQL(connectionString);
            List<MemberWCF> ret = new List<MemberWCF>();
            foreach (var member in members)
                ret.Add(
                    new MemberWCF
                    {
                        order = member.order,
                        ReferenceId = member.ReferenceId,
                        RelationId = member.RelationId,
                        Role = member.Role,
                        Type = member.Type
                    });
            return ret;
        }
        /// <summary>
        /// Gets all the OSM way ids from the database.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns an array of way ids</returns>
        [OperationContract]
        public String[] GetWayIds(String connectionString)
        {
            //return new OSMSqlSource(connectionString).WayIds;
            return new OSMPostgresqlSource(connectionString).WayIds;
        }
        /// <summary>
        /// Gets an array of OSM way ids with the given criteria from the database.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="IdCriteria">The search criteria for the database. (Where clause)</param>
        /// <returns>Returns an array of way ids.</returns>
        [OperationContract]
        public String[] GetWayIdsWithIdCriteria(String connectionString, String IdCriteria)
        {
            //return new OSMSqlSource(connectionString).GetWayIds(IdCriteria);
            return new OSMPostgresqlSource(connectionString).GetWayIds(IdCriteria);
        }
        /// <summary>
        /// Gets an array of all OSM way ids in Stockholm from the database.
        /// Note: You may develop your own methods to target the desired cities. 
        /// Alternatively, you can use <see cref="GetWayIdsWithIdCriteria"/> to do the same without the need to write a new method.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns an array of way ids.</returns>
        /// <seealso cref="GetWayIdsWithIdCriteria"/>
        [OperationContract]
        public String[] GetWayIdsInStockholm(String connectionString)
        {
            //return new OSMSqlSource(connectionString).GetWayIdsInBound(b);
            return new OSMPostgresqlSource(connectionString).GetWayIdsInStockholm();
        }
        /// <summary>
        /// Gets the OSM way ids that are in the given bound from the database.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="bound">The area bounds in latitudes and longitudes.</param>
        /// <returns>Returns an array of way ids.</returns>
        [OperationContract]
        public String[] GetWayIdsInBound(String connectionString, BoundsWCF bound)
        {
            Bounds b = new Bounds() { minlat = bound.minlat, maxlat = bound.maxlat, minlon = bound.minlon, maxlon = bound.maxlon };
            //return new OSMSqlSource(connectionString).GetWayIdsInBound(b);
            var result = new OSMPostgresqlSource(connectionString).GetWayIdsInBound(b);
            if (result.Length == 0)
                return null;
            else return result;
        }
        /// <summary>
        /// Gets the OSM relation ids of building that are in the given bound from the relation table of the database.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="bound">The area bounds in latitudes and longitudes.</param>
        /// <returns>Returns an array of relation ids.</returns>
        /// <remarks>The building retrived using this method are usually multi-polygon buildings with holes in them.</remarks>
        [OperationContract]
        public String[] GetRelationBuildingsInBound(String connectionString, BoundsWCF bound)
        {
            Bounds b = new Bounds() { minlat = bound.minlat, maxlat = bound.maxlat, minlon = bound.minlon, maxlon = bound.maxlon };
            //return new OSMSqlSource(connectionString).GetWayIdsInBound(b);
            var result = new OSMPostgresqlSource(connectionString).GetRelationBuildingsInBoundPostgreSQL(b, connectionString);
            if (result.Length == 0)
                return null;
            else return result;
        }

        /// <summary>
        /// Gets the OSM way ids that are in the given bound from the database.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="bound">The area bounds in latitudes and longitudes.</param>
        /// <param name="tags">The key-value pair to look for.
        /// <para>Note: If even one of the tags match, the way is returned.</para></param>
        /// <returns>Returns an array of way ids.</returns>
        [OperationContract]
        public String[] GetWayIdsWithTags(String connectionString, BoundsWCF bound, string[][] tags)
        {
            Bounds b = new Bounds() { minlat = bound.minlat, maxlat = bound.maxlat, minlon = bound.minlon, maxlon = bound.maxlon };
            //return new OSMSqlSource(connectionString).GetWayIdsInBound(b);
            var result = new OSMPostgresqlSource(connectionString).GetWayIdsWithTags(b, tags);
            if (result.Length == 0)
                return null;
            else return result;
        }
        /// <summary>
        /// Get the extended way ids in the bound from way_ext table in the database.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="bound">The bound area in latitudes and longitudes.</param>
        /// <returns>Array of  (wayid, originalwayid)</returns>
        /// 
        [OperationContract]
        public String[][] GetWayExtIdsInBound(String connectionString, BoundsWCF bound)
        {
            Bounds b = new Bounds() { minlat = bound.minlat, maxlat = bound.maxlat, minlon = bound.minlon, maxlon = bound.maxlon };
            //return new OSMSqlSource(connectionString).GetWayIdsInBound(b);
            return new OSMPostgresqlSource(connectionString).GetWayExtIdsInBound(b);
        }
        /// <summary>
        /// Gets all the OSM node ids in bound that match the mentioned OSM tag criteria.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="bound">The bound area in latitudes and longitudes.</param>
        /// <param name="infoTag">The tag criteria to match against the database.</param>
        /// <returns>Returns an array of node ids.</returns>
        [OperationContract]
        public String[] GetNodeIdsInBoundWithInfo(String connectionString, BoundsWCF bound, String infoTag)
        {
            // "traffic_signals"
            Bounds b = new Bounds() { minlat = bound.minlat, maxlat = bound.maxlat, minlon = bound.minlon, maxlon = bound.maxlon };
            return new OSMPostgresqlSource(connectionString).GetNodeIdsInBoundWithInfo(b, infoTag);
        }
        /// <summary>
        /// Gets all the OSM node ids in bound that match the mentioned OSM tag criteria.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <param name="bound">The bound area in latitudes and longitudes.</param>
        /// <param name="infoTag">The tag pairs to match against.
        /// <para/><strong>Note:</strong> infoTag is a n by 2 array. 
        /// <para/>example: infoTag= new String[3][];
        /// <para/>infoTag[0]=new String[] {"bus", "yes"};</param>
        /// <returns>Returns an array of node ids.</returns>
        [OperationContract]
        public String[] GetNodeIdsInBoundWithKeyValueTag(String connectionString, BoundsWCF bound, String[][] infoTag)
        {
            Bounds b = new Bounds() { minlat = bound.minlat, maxlat = bound.maxlat, minlon = bound.minlon, maxlon = bound.maxlon };
            return new OSMPostgresqlSource(connectionString).GetNodeIdsInBoundWithKeyValueTag(b, infoTag);
        }
        /// <summary>
        /// Gets the OSM node position from the database that matches the nodeid.
        /// <br/>If the node cannot be found then it returns null.
        /// </summary>
        /// <param name="nodeid">The id to search for.</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns the node info.</returns>
        [OperationContract]
        public OsmNodeWCF GetNodeInfo(String nodeid, String connectionString)
        {
            OsmNode n = new OsmNode();
            n.id = nodeid;
            // var position = n.GetPositionSQL(connectionString);
            var position = n.GetPositionPostgreSQL(connectionString);
            OsmNodeWCF ret = new OsmNodeWCF(nodeid, -1, position.Lat, position.Lon);
            return ret;
        }
        /// <summary>
        /// Gets the extended way from way_ext table in the database.
        /// </summary>
        /// <param name="wayid">The id to match.</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns a list of nodes if found.
        /// <br/>Returns null if not found.</returns>
        /// <seealso cref="GetWayExtIdsInBound"/>
        [OperationContract]
        public List<OsmNodeWCF> GetWayExtNodes(String wayid, String connectionString)
        {
            Aram.OSMParser.Way w = new Aram.OSMParser.Way(wayid);
            // var nodes = w.GetNodesSQL(wayid, connectionString);
            var nodes = w.GetNodesFromExtPostgreSQL(wayid, connectionString);
            List<OsmNodeWCF> ret = new List<OsmNodeWCF>();
            foreach (var n in nodes)
            {
                ret.Add(new OsmNodeWCF() { id = n.id, order = n.order, lat = n.PositionSQL.Lat, lon = n.PositionSQL.Lon });
            }
            return ret;
        }
        /// <summary>
        /// Gets the OSM way from the database with the matching wayid.
        /// </summary>
        /// <param name="wayid">The way id to find</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns a list of nodes if found.
        /// <br/>Returns null if not found.</returns>
        /// <seealso cref="GetWayExtNodes"/>
        [OperationContract]
        public List<OsmNodeWCF> GetWayNodes(String wayid, String connectionString)
        {
            Aram.OSMParser.Way w = new Aram.OSMParser.Way(wayid);
            // var nodes = w.GetNodesSQL(wayid, connectionString);
            var nodes = w.GetNodesPostgreSQL(wayid, connectionString);
            List<OsmNodeWCF> ret = new List<OsmNodeWCF>();
            foreach (var n in nodes)
            {
                ret.Add(new OsmNodeWCF() { id = n.id, order = n.order, lat = n.PositionSQL.Lat, lon = n.PositionSQL.Lon });
            }
            return ret;
        }
        /// <summary>
        /// Gets the OSM tags for a relation with the matching id from the database.
        /// </summary>
        /// <param name="relationid">The id of the relation</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns the list of tags.</returns>
        /// <seealso cref="GetWayTags"/>
        /// <seealso cref="GetNodeTags"/>
        [OperationContract]
        public List<TagWCF> GetRelationTags(String relationid, String connectionString)
        {
            Aram.OSMParser.Relation r = new Aram.OSMParser.Relation(relationid);
            //var tags = r.GetTagsSQL(connectionString);
            var tags = r.GetTagsPostgreSQL(connectionString);
            List<TagWCF> ret = new List<TagWCF>();
            foreach (var t in tags)
            {
                ret.Add(new TagWCF() { KeyValue = t.KeyValueSQL });
            }
            if (ret.Count == 0)
                ret.Add(new TagWCF() { KeyValue = new String[] { "NoTag", "NoTagValue" } });
            return ret;
        }
        /// <summary>
        /// Gets the OSM tags for a way with the matching id from the database.
        /// </summary>
        /// <param name="wayid">The id of the way</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns the list of tags.</returns>
        /// <seealso cref="GetNodeTags"/>
        /// <seealso cref="GetRelationTags"/>
        [OperationContract]
        public List<TagWCF> GetWayTags(String wayid, String connectionString)
        {
            Aram.OSMParser.Way w = new Aram.OSMParser.Way(wayid);
            //var tags = w.GetTagsSQL(connectionString);
            var tags = w.GetTagsPostgreSQL(connectionString);
            List<TagWCF> ret = new List<TagWCF>();
            foreach (var t in tags)
            {
                ret.Add(new TagWCF() { KeyValue = t.KeyValueSQL });
            }
            if (ret.Count == 0)
                ret.Add(new TagWCF() { KeyValue = new String[] { "NoTag", "NoTagValue" } });
            return ret;
        }
        /// <summary>
        /// Gets the OSM tags for a way in the Stockholm city with the matching id from the database.
        /// </summary>
        /// <param name="wayid">The id of the way</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns a list of tags</returns>
        /// <seealso cref="GetWayTags"/>
        [OperationContract]
        public List<TagWCF> GetWayTagsStockholm(String wayid, String connectionString)
        {
            Aram.OSMParser.Way w = new Aram.OSMParser.Way(wayid);
            //var tags = w.GetTagsSQL(connectionString);
            var tags = w.GetTagsStockholmPostgreSQL(connectionString);
            List<TagWCF> ret = new List<TagWCF>();
            foreach (var t in tags)
            {
                ret.Add(new TagWCF() { KeyValue = t.KeyValueSQL });
            }
            if (ret.Count == 0)
                ret.Add(new TagWCF() { KeyValue = new String[] { "NoTag", "NoTagValue" } });
            return ret;
        }
        /// <summary>
        /// Gets the OSM tags for a node with the matching id from the database.
        /// </summary>
        /// <param name="nodeid">The id of the node</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns a list of tags</returns>
        /// <seealso cref="GetRelationTags"/>
        /// <seealso cref="GetWayTags"/>
        [OperationContract]
        public List<TagWCF> GetNodeTags(String nodeid, String connectionString)
        {
            OsmNode node = new OsmNode();
            node.id = nodeid;
            //var tags = node.GetTagsSQL(connectionString);
            var tags = node.GetTagsPostgreSQL(connectionString);
            List<TagWCF> ret = new List<TagWCF>();
            foreach (var t in tags)
            {
                ret.Add(new TagWCF() { KeyValue = t.KeyValueSQL });
            }
            if (ret.Count == 0)
                ret.Add(new TagWCF() { KeyValue = new String[] { "NoTag", "NoTagValue" } });
            return ret;
        }
        /// <summary>
        /// Gets the global geo boundary of the OSM elements in the database.
        /// <br/><strong>Note:</strong>This is intended to be used when interpolating between coordinates of geolocations and visualization engines.
        /// <br/>The main reason is that most of the game engine worlds are coordinated around a single-precision float range, and given the fact
        /// that the accuracy of the floating numbers drop when they get large, we recommend that you place the geometries close to the origin,
        ///  rather than their real position.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns the minimum and maximum latitude/longitudes</returns>
        [OperationContract]
        public BoundsWCF GetBounds(String connectionString)
        {
            //OSMSqlSource source = new OSMSqlSource(connectionString);
            OSMPostgresqlSource source = new OSMPostgresqlSource(connectionString);
            var bounds = source.Bounds;
            BoundsWCF ret = new BoundsWCF() { minlat = bounds.minlat, maxlat = bounds.maxlat, minlon = bounds.minlon, maxlon = bounds.maxlon };
            return ret;
        }
        /// <summary>
        /// Returns the list of nodes of a certain type in the route relation.
        /// </summary>
        /// <param name="routeId">The relation id of the route.</param>
        /// <param name="memberType">0 for node, 1 for way, 2 for relation</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns a list of nodes.</returns>
        [OperationContract]
        public List<OsmNodeWCF> GetRoutePath(string routeId, int memberType, String connectionString)
        {
            var res = Aram.OSMParser.Relation.GetRoutePath(routeId, memberType, connectionString);
            List<OsmNodeWCF> result = new List<OsmNodeWCF>();
            foreach (var node in res)
            {
                result.Add(new OsmNodeWCF() { id = node.id, order = node.order, lat = node.PositionSQL.Lat, lon = node.PositionSQL.Lon });
            }
            return result;
        }
        /// <summary>
        /// Returns the list of nodes of a certain type in the route relation.
        /// </summary>
        /// <param name="routeId">The relation id of the route.</param>
        /// <param name="memberType">0 for node, 1 for way, 2 for relation</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns a list of nodes.</returns>
        [OperationContract]
        public List<OsmNodeWCF> GetRoutePathCustom(string routeId, int memberType, String connectionString)
        {
            var res = Aram.OSMParser.Relation.GetRoutePathExperimental(routeId, memberType, connectionString);
            List<OsmNodeWCF> result = new List<OsmNodeWCF>();
            foreach (var node in res)
            {
                result.Add(new OsmNodeWCF() { id = node.id, order = node.order, lat = node.PositionSQL.Lat, lon = node.PositionSQL.Lon });
            }
            return result;
        }

        /// <summary>
        /// An example router engine from OSMSharp package.
        /// Visit <a href="http://www.osmsharp.com">OsmSharp website</a> for more.
        /// </summary>
        public static IRouter<RouterPoint> router;
        /// <summary>
        /// Initializes the example router.
        /// Visit <a href="http://www.osmsharp.com">OsmSharp website</a> for more.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns the success or error message.</returns>
        [OperationContract]
        public String InitializeRouter(String connectionString)
        {
            try
            {
                OsmGeo.ShapeInterperter = new SimpleShapeInterpreter();
                PostgreSQLSimpleSchemaSource source = new PostgreSQLSimpleSchemaSource(connectionString);

                // keeps a memory-efficient version of the osm-tags.
                OsmTagsIndex tags_index = new OsmTagsIndex();

                // creates a routing interpreter. (used to translate osm-tags into a routable network)
                OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();

                // create routing inter
                OsmSourceRouterDataSource routing_data = new OsmSourceRouterDataSource(
                interpreter, tags_index, source);

                // create the router object.
                //IRouter<RouterPoint> router = new Router<PreProcessedEdge>(routing_data, interpreter,
                //    new DykstraRoutingPreProcessed(routing_data.TagsIndex));
                router = new Router<PreProcessedEdge>(routing_data, interpreter
                        , new DykstraRoutingPreProcessed(routing_data.TagsIndex));

                return "Successfully initialized.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Checks whether the example router is initializaed
        /// Visit <a href="http://www.osmsharp.com">OsmSharp website</a> for more.
        /// </summary>
        public void CheckRouter()
        {
            if (router == null)
                throw new NullReferenceException("The router is not initialized. Please use InitializeRouter() method to prepare the router");
        }
        /// <summary>
        /// Resolves a point on the example router.
        /// Visit <a href="http://www.osmsharp.com">OsmSharp website</a> for more.
        /// </summary>
        /// <param name="vehicle">The type of vehicle</param>
        /// <param name="currentNode">The node to be resolved</param>
        /// <returns>Returns the resolved node.
        /// <br/>Returns null if the node cannot be resolved.</returns>
        [OperationContract]
        public OsmNodeWCF ResolvePoint(VehicleEnum vehicle, OsmNodeWCF currentNode)
        {
            CheckRouter();
            var result = router.Resolve(vehicle, new OsmSharp.Tools.Math.Geo.GeoCoordinate(currentNode.lat, currentNode.lon));
            OsmNodeWCF temp = new OsmNodeWCF(result.Id.ToString(), 0, result.Location.Latitude, result.Location.Longitude);
            return temp;
        }
        /// <summary>
        /// Performs a Dykstra routing on the example router.
        /// Visit <a href="http://www.osmsharp.com">OsmSharp website</a> for more.
        /// </summary>
        /// <param name="vehicle">The type of vehicle.</param>
        /// <param name="pointA">Starting point</param>
        /// <param name="pointB">Destination point</param>
        /// <returns>Returns the list of nodes for the route.
        /// <br/>Returns empty list if no route is found.</returns>
        [OperationContract]
        public List<OsmNodeWCF> RouteUsingDykstra(VehicleEnum vehicle, OsmNodeWCF pointA, OsmNodeWCF pointB)
        {
            CheckRouter();
            List<OsmNodeWCF> ret = new List<OsmNodeWCF>();
            RouterPoint a = router.Resolve(vehicle, new OsmSharp.Tools.Math.Geo.GeoCoordinate(pointA.lat, pointA.lon));
            RouterPoint b = router.Resolve(vehicle, new OsmSharp.Tools.Math.Geo.GeoCoordinate(pointB.lat, pointB.lon));
            OsmSharpRoute route = router.Calculate(vehicle, a, b);
            if (route != null)
            {
                var result = route.Entries; //route.GetPoints();
                foreach (var res in result)
                    ret.Add(new OsmNodeWCF("someid", -1, res.Latitude, res.Longitude));
            }
            return ret;
        }
        /// <summary>
        /// Gets a <see cref="geometryCollection"/> from an object in the database matching the given id.
        /// </summary>
        /// <param name="id">The id to match against</param>
        /// <param name="includeLargeObject">If true, the returned geometryCollection will include the content of 
        /// largeObject byte array</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns a geometryCollection</returns>
        /// <seealso cref="GetGeometryCollection"/>
        /// <seealso cref="GetGeometryCollectionMinimal"/>
        [OperationContract]
        public geometryCollection GetGeometryCollectionSingleObject(String id, bool includeLargeObject, String connectionString)
        {
            return geometryCollection.GetSingleObjectWithId(id, includeLargeObject, connectionString);
        }
        /// <summary>
        /// Gets an array of <see cref="geometryCollection"/> from an object in the database matching the search criteria.
        /// </summary>
        /// <param name="includeLargeObject">If true, the returned geometryCollections will include the content of 
        /// largeObject byte array</param>
        /// <param name="SearchCriteriaWhereClause">The search criteria for the geometryCollection objects.
        /// Please take a look at geometryCollection table in the database for serach parameters.</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns an array of geometryCollection</returns>
        /// <seealso cref="GetGeometryCollectionSingleObject"/>
        /// <seealso cref="GetGeometryCollectionMinimal"/>
        [OperationContract]
        public geometryCollection[] GetGeometryCollection(bool includeLargeObject, string SearchCriteriaWhereClause, String connectionString)
        {
            return geometryCollection.GetObjectsWithIdAndCriteria(includeLargeObject, SearchCriteriaWhereClause, connectionString);
        }
        /// <summary>
        /// Gets the ids of <see cref="geometryCollection"/> matching the search criteria in the database.
        /// </summary>
        /// <param name="SearchCriteriaWhereClause">The search criteria for the geometryCollection objects.
        /// Please take a look at geometryCollection table in the database for serach parameters.</param>
        /// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
        /// <returns>Returns an array of ids.</returns>
        /// <seealso cref="GetGeometryCollectionSingleObject"/>
        /// <seealso cref="GetGeometryCollection"/>
        [OperationContract]
        public long[] GetGeometryCollectionMinimal(string SearchCriteriaWhereClause, String connectionString)
        {
            return geometryCollection.GetObjectsWithIdAndCriteriaMinimal(SearchCriteriaWhereClause, connectionString);
        }
        #endregion

        #region Simulation Related
        /// <summary>
        /// This is the static object for a single SUMO simulation scenario in Floating Car Data mode.
        /// </summary>
        public static SUMOSimulationFCD SUMOsimulationFCDInstance;
        /// <summary>
        /// This is the static object for a list of SUMO simulations in Floating Car Data mode.
        /// </summary>
        public static List<SUMOSimulationFCD> SUMOsimulationFCDList;
        /// <summary>
        /// The boolean for the status of a loading simulation when using <see cref="SUMOsimulationFCDInstance"/>.
        /// </summary>
        private static bool SimulationLoaded = false;
        /// <summary>
        /// The list of booleans for the status of loading simulations when using <see cref="SUMOsimulationFCDList"/>.
        /// </summary>
        private static List<bool> SimulationLoadedList;

        /// <summary>
        /// Initializes the simulation services.
        /// </summary>
        /// <seealso cref="DisposeSimulation"/>
        [OperationContract]
        public void InitializeSimulationList()
        {
            if (SUMOsimulationFCDList == null)
                SUMOsimulationFCDList = new List<SUMOSimulationFCD>();
            if (SimulationLoadedList == null)
                SimulationLoadedList = new List<bool>();
        }
        /// <summary>
        /// Disposes a selected simulation.
        /// </summary>
        /// <param name="SimulationID">The simulation id to dispose.</param>
        /// <returns>True if the dispose is successful.</returns>
        [OperationContract]
        public bool DisposeSimulation(string SimulationID)
        {
            lock (this)
            {
                SUMOsimulationFCDList[int.Parse(SimulationID)] = null;
                SimulationLoadedList[int.Parse(SimulationID)] = false;
            }
            return true;
        }
        /// <summary>
        /// Gets generated a random id for a specific simulation.
        /// </summary>
        /// <param name="initial">Prefix of the id</param>
        /// <returns>Returns the generated id</returns>
        [OperationContract]
        public String GenerateRandomID(String initial)
        {
            return initial + "_" + Guid.NewGuid().ToString().Replace("-", "");
        }
        /// <summary>
        /// Gets the numbers of time steps available in <see cref="SUMOsimulationFCDInstance"/>.
        /// </summary>
        /// <returns>Returns the number of time steps.</returns>
        /// <seealso cref="SUMOsimulationFCDInstance"/>
        [OperationContract]
        public int GetTotalTimesteps()
        {
            return SUMOsimulationFCDInstance.TimeStep.Length;
        }
        /// <summary>
        /// Gets the numbers of time steps available for a simulation in <see cref="SUMOsimulationFCDList"/> matching the given id.
        /// </summary>
        /// <param name="SimulationID">The id of the simulation</param>
        /// <returns>Returns the number of time steps.</returns>
        /// <seealso cref="SUMOsimulationFCDList"/>
        [OperationContract]
        public int GetTotalTimestepsList(int SimulationID)
        {
            return SUMOsimulationFCDList[SimulationID].TimeStep.Length;
        }
        /// <summary>
        /// Gets a time step at a specified index in time in <see cref="SUMOsimulationFCDInstance"/>.
        /// </summary>
        /// <param name="index">The index of the time step.</param>
        /// <returns>Returns the TimeStep.</returns>
        /// <seealso cref="SUMOsimulationFCDInstance"/>
        [OperationContract]
        public TimeStep GetTimestepAt(int index)
        {
            return SUMOsimulationFCDInstance.TimeStep[index];
        }
        /// <summary>
        /// Gets a time step at a specified index in time of a matching simulation id in <see cref="SUMOsimulationFCDList"/>.
        /// </summary>
        /// <param name="index">The index of the time step.</param>
        /// <param name="SimulationID">The id of the simulation</param>
        /// <returns>Returns the TimeStep.</returns>
        /// <seealso cref="SUMOsimulationFCDList"/>
        [OperationContract]
        public TimeStep GetTimestepAtList(int index, int SimulationID)
        {
            return SUMOsimulationFCDList[SimulationID].TimeStep[index];
        }
        /// <summary>
        /// Gets a time step at a specified index in time of a matching simulation id in <see cref="SUMOsimulationFCDList"/>.
        /// </summary>
        /// <param name="index">The index of the time step.</param>
        /// <param name="SimulationID">The id of the simulation</param>
        /// <returns>Returns the SimpleTimeStep.</returns>
        /// <seealso cref="SUMOsimulationFCDList"/>
        [OperationContract]
        public SimpleTimeStep GetSimpleTimestepAtList(int index, int SimulationID)
        {
            var temp = new SimpleTimeStep();
            var currentStep = SUMOsimulationFCDList[SimulationID].TimeStep[index];
            temp.index = currentStep.index;
            temp.time = currentStep.time;
            temp.iMobilityTime = currentStep.iMobilityTime;
            temp.VehicleIds = currentStep.Vehicles.Select(i => i.Id).ToArray();
            temp.VehicleLats = currentStep.Vehicles.Select(i => i.Latitude).ToArray();
            temp.VehicleLongs = currentStep.Vehicles.Select(i => i.Longitude).ToArray();
            return temp;
        }
        /// <summary>
        /// Gets the simulation name in <see cref="SUMOsimulationFCDInstance"/>.
        /// </summary>
        /// <returns>Returns the simulation name.</returns>
        [OperationContract]
        public String GetSimulationName()
        {
            return SUMOsimulationFCDInstance.Name;
        }
        /// <summary>
        /// Gets the simulation name, matching an id in <see cref="SUMOsimulationFCDList"/>.
        /// </summary>
        /// <param name="SimulationID">The id of the simulation</param>
        /// <returns>Returns the simulation name.</returns>
        [OperationContract]
        public String GetSimulationNameList(int SimulationID)
        {
            return SUMOsimulationFCDList[SimulationID].Name;
        }
        /// <summary>
        /// Gets the description of the simulation in <see cref="SUMOsimulationFCDInstance"/>.
        /// </summary>
        /// <returns>Returns the description.</returns>
        [OperationContract]
        public String GetSimulationDescription()
        {
            return SUMOsimulationFCDInstance.Description;
        }
        /// <summary>
        /// Gets the description of the simulation, matching an id in <see cref="SUMOsimulationFCDList"/>.
        /// </summary>
        /// <param name="SimulationID">The id of the simulation</param>
        /// <returns>Returns the description.</returns>
        [OperationContract]
        public String GetSimulationDescriptionList(int SimulationID)
        {
            return SUMOsimulationFCDList[SimulationID].Description;
        }
        /// <summary>
        /// Gets whether the simulation is fully loaded in <see cref="SUMOsimulationFCDInstance"/>.
        /// </summary>
        /// <returns>True if simulation loading is complete<br/>False otherwise.</returns>
        [OperationContract]
        public bool IsSimulationLoaded()
        {
            lock (this)
            {
                return SimulationLoaded;
            }
        }
        /// <summary>
        /// Gets whether the simulation is fully loaded, matching an id in <see cref="SUMOsimulationFCDList"/>.
        /// </summary>
        /// <param name="SimulationID">The id of the simulation.</param>
        /// <returns>True if simulation loading is complete<br/>False otherwise.</returns>
        [OperationContract]
        public bool IsSimulationLoadedList(int SimulationID)
        {
            return SimulationLoadedList[SimulationID];
        }
        /// <summary>
        /// Starts loading of a simulation asynchronously, sets it to <see cref="SUMOsimulationFCDInstance"/> and returns the simulation id immediately.
        /// </summary>
        /// <param name="filename">The simulation file to load in FCD format. Please refer to floating car data format.</param>
        /// <example>
        /// <code>
        /// LoadSUMOFCDSimulation("C:\\test.xml");
        /// while (!IsSimulationLoaded()){};
        /// var i=GetTotalTimeSteps();
        /// </code>
        /// </example>
        [OperationContract]
        public void LoadSUMOFCDSimulation(String filename)
        {
            Thread t = new Thread(new ParameterizedThreadStart(LoadSimulationTask));
            t.Start(filename);
        }
        /// <summary>
        /// Starts loading of a simulation asynchronously, adds it to <see cref="SUMOsimulationFCDList"/> and returns the simulation id immediately.
        /// </summary>
        /// <param name="ObjectIDPostfix">The postfix id for the loading objects.</param>
        /// <param name="filename">The simulation file to load in FCD format. Please refer to floating car data format.</param>
        /// <returns>Returns the id of the simulation.</returns>
        [OperationContract]
        public int LoadSUMOFCDSimulationList(String filename, String ObjectIDPostfix)
        {
            InitializeSimulationList();

            lock (this)
            {
                SimulationLoadedList.Add(false);
                SUMOsimulationFCDList.Add(new SUMOSimulationFCD());
            }
            Thread t = new Thread(new ParameterizedThreadStart(LoadSimulationListTask));
            t.Start(new string[] { filename, (SimulationLoadedList.Count - 1).ToString(), ObjectIDPostfix });
            return SimulationLoadedList.Count - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">parameters[0] should be filename, parameters[1] should be SimulationID</param>
        private void LoadSimulationListTask(object parameters)
        {
            string[] pars = (string[])parameters;
            var filename = pars[0];
            var SimulationID = int.Parse(pars[1]);
            var ObjectIDPostfix = pars[2];

            GaPSlabsSimulationLibrary.SUMOSimulationFCD simulation = new GaPSlabsSimulationLibrary.SUMOSimulationFCD();
            if (String.IsNullOrEmpty(filename))
                filename = @"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMOData\fcdoutput.xml";
            bool isMinimal = false;
            switch (System.IO.Path.GetExtension(filename).ToLower())
            {
                case ".xml":
                    {
                        simulation.LoadFromXML(filename);
                        break;
                    }
                case ".csv":
                    {
                        isMinimal = true;
                        simulation.LoadFromCSV(filename);
                        break;
                    }
            }

            SUMOSimulationFCD temp = new SUMOSimulationFCD();
            temp.Name = simulation.Name;
            temp.Description = simulation.Description;
            List<TimeStep> tempTimeStepList = new List<TimeStep>();


            foreach (var t in simulation.TimeStep)
            {
                var tt = new TimeStep();
                tt.time = t.time;
                tt.index = t.index;
                if (isMinimal)
                    tt.iMobilityTime = t.iMobilityTime;
                List<VehicleFCD> vlist = new List<VehicleFCD>();
                foreach (var v in t.Vehicles)
                {
                    var tempV = v as GaPSlabsSimulationLibrary.VehicleFCD;
                    VehicleFCD vv = new VehicleFCD();
                    vv.Id = tempV.Id + ObjectIDPostfix + SimulationID;
                    vv.Latitude = tempV.Latitude;
                    vv.Longitude = tempV.Longitude;
                    if (!isMinimal)
                    {
                        vv.Angle = tempV.Angle;
                        vv.Lane = tempV.Lane;
                        vv.Pos = tempV.Pos;
                        vv.Speed = tempV.Speed;
                        vv.Slope = tempV.Slope;
                    }
                    vv.VehicleType = (SUMOSimulationFCD.VehicleType)Enum.Parse(typeof(SUMOSimulationFCD.VehicleType), tempV.VehicleType.ToString());
                    vlist.Add(vv);
                }
                if (vlist.Count > 0)
                    tt.Vehicles = vlist.ToArray();
                lock (this)
                {
                    tempTimeStepList.Add(tt);
                }
            }
            temp.TimeStep = tempTimeStepList.ToArray();
            SUMOsimulationFCDList[SimulationID] = temp;

            SimulationLoadedList[SimulationID] = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenamestring"></param>
        private void LoadSimulationTask(object filenamestring)
        {
            string filename = filenamestring.ToString();
            lock (this)
            {
                SimulationLoaded = false;
            }
            GaPSlabsSimulationLibrary.SUMOSimulationFCD simulation = new GaPSlabsSimulationLibrary.SUMOSimulationFCD();
            if (String.IsNullOrEmpty(filename))
                filename = @"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMOData\fcdoutput.xml";
            bool isMinimal = false;
            switch (System.IO.Path.GetExtension(filename).ToLower())
            {
                case ".xml":
                    {
                        simulation.LoadFromXML(filename);
                        break;
                    }
                case ".csv":
                    {
                        isMinimal = true;
                        simulation.LoadFromCSV(filename);
                        break;
                    }
            }

            SUMOSimulationFCD temp = new SUMOSimulationFCD();
            temp.Name = simulation.Name;
            temp.Description = simulation.Description;
            List<TimeStep> tempTimeStepList = new List<TimeStep>();
            foreach (var t in simulation.TimeStep)
            {
                var tt = new TimeStep();
                tt.time = t.time;
                tt.index = t.index;
                if (isMinimal)
                    tt.iMobilityTime = t.iMobilityTime;
                List<VehicleFCD> vlist = new List<VehicleFCD>();
                foreach (var v in t.Vehicles)
                {
                    var tempV = v as GaPSlabsSimulationLibrary.VehicleFCD;
                    VehicleFCD vv = new VehicleFCD();
                    vv.Id = tempV.Id;
                    vv.Latitude = tempV.Latitude;
                    vv.Longitude = tempV.Longitude;
                    if (!isMinimal)
                    {
                        vv.Angle = tempV.Angle;
                        vv.Lane = tempV.Lane;
                        vv.Pos = tempV.Pos;
                        vv.Speed = tempV.Speed;
                        vv.Slope = tempV.Slope;
                    }
                    vv.VehicleType = (SUMOSimulationFCD.VehicleType)Enum.Parse(typeof(SUMOSimulationFCD.VehicleType), tempV.VehicleType.ToString());
                    vlist.Add(vv);
                }
                if (vlist.Count > 0)
                    tt.Vehicles = vlist.ToArray();
                tempTimeStepList.Add(tt);
            }
            temp.TimeStep = tempTimeStepList.ToArray();
            SUMOsimulationFCDInstance = temp;
            lock (this)
            {
                SimulationLoaded = true;
            }
        }


        #endregion



    }
}
