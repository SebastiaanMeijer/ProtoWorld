/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aram.OSMParser;


namespace GaPSLabs.Geometry
{
    /// <summary>
    /// The GaPSLabs Geometry Namespace
    /// </summary>
    internal class NamespaceDoc
    {
    }
    /// <summary>
    /// The class that generates mesh for the given openstreetmap objects.
    /// </summary>
    public static class MeshGenerator
    {
        /// <summary>
        /// Properties used to generated the objects. Note that this is a static variable.
        /// </summary>
        public static MapProperties Properties;
        /// <summary>
        /// The random generator that is used throughout the mesh generation process (mostly for building heights).
        /// </summary>
        public static Random rand;
        /// <summary>
        /// The type of OSM shape.
        /// </summary>
        public enum OSMShape
        {
            /// <summary>
            /// When the osm shape is a way, it could either refer to a road or a building.
            /// </summary>
            Way,
            /// <summary>
            /// When the osm shape is a relation, it refers to a multi-polygon building with possible holes in it.
            /// </summary>
            Relation
        };
        /// <summary>
        /// Generates the mesh for a given openstreetmap element
        /// </summary>
        /// <param name="OSMid">The id of the OSM element</param>
        /// <param name="MinPointOnMap">Minimum point on the target map (to calculate the offset)</param>
        /// <param name="bounds">The boundaries of the used OSM map</param>
        /// <param name="properties">The details of the target scene generation.</param>
        /// <param name="connPostGreSql">The connection string to the database. Please refer to .Net Data Provider for MSSQL for format specifications</param>
        /// <param name="shape">The type of the generated mesh. Refer to <see cref="GaPSLabs.Geometry.MeshGenerator.OSMShape"/></param>
        /// <param name="RemoveRedundantPointsOnTheSameLine">If true, it will remove redundant points on straight parts of the shape according to<paramref name="RedundantPointErrorThreshold"/>.</param>
        /// <param name="RedundantPointErrorThreshold">The error toleration when removing redundant points. The default is 0.001f</param>
        /// <param name="SegmentLines">If true, it will segment the road geometry so that the texture will look uniform over the whole mesh.</param>
        /// <returns>Returns a <see cref="GaPSLabs.Geometry.GameObject"/> of the generated mesh.</returns>
        /// <remarks>
        /// <para>Removing the redundant points happen before the line segmentation. Therefore the points that were generated when setting <paramref name="SegmentLines"/> to true will not be removed.</para>
        /// <para>The <paramref name="RedundantPointErrorThreshold"/> specifies the difference in slope of the line segments that can be ignored.</para>
        /// </remarks>
        public static GameObject OSMtoGameObject(string OSMid, Vector3 MinPointOnMap, Bounds bounds, MapProperties properties, string connPostGreSql, OSMShape shape = OSMShape.Way, bool RemoveRedundantPointsOnTheSameLine = true, float RedundantPointErrorThreshold = 0.001f, bool SegmentLines = true)
        {
            if (rand == null)
                rand = new Random((int)DateTime.Now.Ticks);
            if (properties == null)
                properties = Properties;
            var minmaxX = properties.minMaxX;
            var minmaxY = properties.minMaxY;
            int direction = -1;
            float LineWidth = properties.RoadLineThickness;
            float BuildingWidth = properties.BuildingLineThickness;
            float height = properties.BuildingHeight;
            if (height < 4)
                height = 4;
            var FirstWay = OSMid;

            if (shape == OSMShape.Way)
            {
                if (FirstWay.Equals("41018641"))
                {
                    System.Console.WriteLine("Missing road found in WCF");
                }

                var w = new Way(FirstWay);
                List<OsmNode> WayNodes;
                List<Tag> WayTags;
                using (Npgsql.NpgsqlConnection con = new Npgsql.NpgsqlConnection(connPostGreSql))
                {
                    con.Open();
                    WayNodes = w.GetNodesPostgreSQL(FirstWay, con);
                    WayTags = w.GetTagsPostgreSQL(FirstWay, con);
                    con.Close();
                }
                if (WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "landuse" || i.KeyValueSQL[0].ToLower() == "building" || i.KeyValueSQL[0].ToLower() == "highway").Count() != 0)
                {
                    var tempPoints = new Vector3[WayNodes.Count];

                    int counter = 0;
                    foreach (var node in WayNodes)
                    {
                        var result = CoordinateConvertor.SimpleInterpolation((float)node.PositionSQL.Lat, (float)node.PositionSQL.Lon, bounds, minmaxX, minmaxY);
                        // Testing the correct direction
                        tempPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                        counter++;
                    }
                    if (RemoveRedundantPointsOnTheSameLine)
                        tempPoints = tempPoints.RemoveRedundantPointsInTheSameLines();
                    WayNodes = null;
                    var building = WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "building");
                    var highwayType = WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "highway");
                    var onewayType = WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "oneway").FirstOrDefault();
                    var lanesType = WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "lanes").FirstOrDefault();
                    var areaType = WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "area").FirstOrDefault();
                    WayTags = null;
                    if (building.Count() != 0)
                    {
                        #region Buildings
                        // Check if it has overlapping start and ending points.
                        // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                        // Checking for duplicates:
                        tempPoints = tempPoints.ToArray().RemoveDuplicates();
                        var Skip = false;
                        if (tempPoints.Length <= 2)
                        {
                            // Buildings that are too small to show such as 76844368
                            // http://www.openstreetmap.org/browse/way/76844368
                            // "A weird building were found and ignored. FirstWay \nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                            Skip = true; // continue;
                        }
                        if (!Skip)
                        {
                            var p2d = tempPoints.ToCPoint2D();
                            // TODO bug in the cpolygon, probably duplicates
                            var shp = new PolygonCuttingEar.CPolygonShape(p2d);
                            shp.CutEar();
                            p2d = null;
                            GC.Collect();
                            // TODO:
                            //var randHeight = CoordinateConvertor.linear((float)rand.NextDouble(), 0, 1, -3f, height);
                            // var randMaterial = (randHeight > height / 2f) ? "BuildingTall" : randHeight < height / 2f ? "Building2" : "Building";
                            var randHeight = properties.BuildingHeightVariation.GetNextCircular();
                            var randMaterial = (randHeight > (properties.BuildingHeightVariation.Average + properties.BuildingHeightVariation.Max) / 2f) ? "BuildingTall" : ("Building" + rand.Next(1, 5));
                            var resultedGameObject = shp.GenerateShapeUVedWithWalls_Balanced(
                                    CoordinateConvertor.OSMType.Polygon, FirstWay,
                                    "Building", "Building", randMaterial,
                                /*height +*/ randHeight,/* height + 7*/properties.BuildingHeightVariation.Max, true);
                            return resultedGameObject;
                        }
                        #endregion

                    }
                    else
                    {
                        if (highwayType.Count() != 0)
                        {
                            #region Roads
                            var hwtype = highwayType.First();
                            //Console.WriteLine("Generating roads..id=" + FirstWay);
                            switch (hwtype.KeyValueSQL[1])
                            {
                                case "cycleway":
                                    {
                                        var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(SegmentLines ? tempPoints.ToSegmentedPoints(2f) : tempPoints, properties.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", properties.CycleWayMaterial.Name, -0.01f);
                                        // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                        return resultedGameObject;
                                    }
                                case "footway":
                                case "path":
                                case "pedestrian":
                                    {
                                        if (areaType != null)
                                        {
                                            tempPoints = tempPoints.ToArray().RemoveDuplicates();
                                            var Skip = false;
                                            if (tempPoints.Length <= 2)
                                            {
                                                // Buildings that are too small to show such as 76844368
                                                // http://www.openstreetmap.org/browse/way/76844368
                                                // "A weird building were found and ignored. FirstWay \nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                Skip = true; // continue;
                                            }
                                            if (!Skip)
                                            {
                                                var p2d = tempPoints.ToCPoint2D();
                                                // TODO bug in the cpolygon, probably duplicates
                                                var shp = new PolygonCuttingEar.CPolygonShape(p2d);
                                                shp.CutEar();
                                                p2d = null;
                                                GC.Collect();
                                                var resultedGameObject = shp.GenerateShapeUVedPlanar_Balanced(
                                                        CoordinateConvertor.OSMType.Polygon, FirstWay,
                                                        "FootwayArea", "Area", properties.AreaMaterial.Name,
                                                        0, true);
                                                return resultedGameObject;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(SegmentLines ? tempPoints.ToSegmentedPoints(4f) : tempPoints, properties.FootwayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", properties.FootWayMaterial.Name, -0.01f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));
                                            return resultedGameObject;
                                        }
                                    }
                                case "steps":
                                    {
                                        var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(SegmentLines ? tempPoints.ToSegmentedPoints(4f) : tempPoints, properties.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", properties.StepsMaterial.Name, -0.01f);
                                        return resultedGameObject;
                                    }

                                // Miguel R.C. commented these lines. 
                                // Reason: Why breaking in the case "motorway"? That results in some roads missing when generating scenarios.
                                // With these lines commented, the motorways will be generated in the default case. 
                                //case "motorway":
                                //    {
                                //        break;
                                //    }

                                default:
                                    {
                                        // Calculating the corrent road width based on the available information
                                        // Parameters affecting the width are:
                                        // 1- road direction: 'oneway' tag
                                        // 2- number of lanes: 'lanes' tag. The defaults are: http://wiki.openstreetmap.org/wiki/Lane#Assumptions
                                        //
                                        // |If two-way then # of lanes is 2, and if one-way then # of lanes is 1|
                                        // highway=residential
                                        // highway=tertiary
                                        // highway=secondary
                                        // highway=primary 
                                        //
                                        // |If two-way then # of lanes is 1, and if one-way then # of lanes is 1|
                                        // highway=service
                                        // highway=track
                                        // highway=path 
                                        // The actual number of lanes for the following must always be tagged in the lanes value.
                                        // highway=motorway
                                        // highway=trunk

                                        //TODO: USE lanes value.
                                        var calculatedRoadWidth = properties.RoadWidth;
                                        bool useLaneInfo = false;
                                        if (lanesType != null)
                                        {
                                            var laneNumbers = lanesType.KeyValueSQL[1].Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                                            float lanes = 0;
                                            foreach (var lane in laneNumbers)
                                            {
                                                float templane;
                                                if (float.TryParse(lane, out templane))
                                                    lanes += templane;
                                            }
                                            if (lanes != 0)
                                            {
                                                useLaneInfo = true;
                                                calculatedRoadWidth = properties.RoadWidth * lanes;
                                            }
                                        }
                                        if (!useLaneInfo)
                                        {
                                            if (onewayType == null) // It is a 2-way by default
                                            {
                                                if (hwtype.KeyValueSQL[1] == Tags.Highways.residential ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.tertiary ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.secondary ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.primary)
                                                    calculatedRoadWidth = properties.RoadWidth * 2;
                                                else if (hwtype.KeyValueSQL[1] == Tags.Highways.service ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.track ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.path)
                                                    calculatedRoadWidth = properties.RoadWidth;
                                            }
                                            else if (onewayType.KeyValueSQL[1].ToLower() == "0" | onewayType.KeyValueSQL[1].ToLower() == "no") // It is still a 2-way
                                            {
                                                if (hwtype.KeyValueSQL[1] == Tags.Highways.residential ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.tertiary ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.secondary ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.primary)
                                                    calculatedRoadWidth = properties.RoadWidth * 2;
                                                else if (hwtype.KeyValueSQL[1] == Tags.Highways.service ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.track ||
                                                    hwtype.KeyValueSQL[1] == Tags.Highways.path)
                                                    calculatedRoadWidth = properties.RoadWidth;
                                            }
                                            else // It is a one-way
                                            {
                                                calculatedRoadWidth = properties.RoadWidth;
                                            }
                                        }
                                        var matName = properties.RoadMaterial.Name;
                                        if (hwtype.KeyValueSQL[1] == Tags.Highways.residential)
                                            matName = properties.ResidentialMaterial.Name;
                                        else if (hwtype.KeyValueSQL[1] == Tags.Highways.service)
                                            matName = properties.ServiceMaterial.Name;
                                        var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(SegmentLines ? tempPoints.ToSegmentedPoints(0.5f) : tempPoints, calculatedRoadWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", matName, 0f);

                                        return resultedGameObject;

                                    }
                            }
                            #endregion
                        }
                    }
                }

            }
            else if (shape == OSMShape.Relation) // It is probably a multi-polygon building defined in a relation.
            {
                List<OsmNode[]> OuterNodes;
                //List<OsmNode> OuterWay;
                List<OsmNode[]> InnerNodes;
                List<Tag> tags;
                Relation r = new Relation(FirstWay);
                Way w = new Way();
                using (Npgsql.NpgsqlConnection con = new Npgsql.NpgsqlConnection(connPostGreSql))
                {
                    con.Open();

                    tags = r.GetTagsPostgreSQL(con);
                    var isMultiPolygon = tags.Exists(t => t.KeyValueSQL[0] == "type" && t.KeyValueSQL[1] == "multipolygon");
                    if (isMultiPolygon)
                    {
                        var Members = r.GetMembersPostgreSQLByType(r.id, 1, con);

                        OuterNodes = Members
                             .Where(m => m.Role.ToLower() == Member.RoleOuter)
                             .OrderBy(o => o.order)
                             .Select(m => w.GetNodesPostgreSQL(m.ReferenceId, connPostGreSql).ToArray()).ToList();
                        InnerNodes = Members
                             .Where(m => m.Role.ToLower() == Member.RoleInner)
                             .OrderBy(o => o.order)
                             .Select(m => w.GetNodesPostgreSQL(m.ReferenceId, connPostGreSql).ToArray()).ToList();


                        //OuterWay = new List<OsmNode>();
                        //foreach (var nodelist in OuterNodes)
                        //    OuterWay.AddRange(nodelist);


                        List<Vector3[]> tempPointsOuterPoints = new List<Vector3[]>();
                        int counter = 0;
                        foreach (var node in OuterNodes)
                        {
                            Vector3[] currrentOuter = new Vector3[node.Length];
                            for (int i = 0; i < node.Length; i++)
                            {
                                var result = CoordinateConvertor.SimpleInterpolation((float)node[i].PositionSQL.Lat, (float)node[i].PositionSQL.Lon, bounds, minmaxX, minmaxY);
                                // Testing the correct direction
                                currrentOuter[i] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                            }
                            tempPointsOuterPoints.Add(currrentOuter.RemoveDuplicates());
                        }

                        List<Vector3[]> holes = new List<Vector3[]>();
                        for (int i = 0; i < InnerNodes.Count; i++)
                        {
                            holes.Add(new Vector3[InnerNodes[i].Length]);

                            for (int j = 0; j < InnerNodes[i].Length; j++)
                            {
                                var result = CoordinateConvertor.SimpleInterpolation((float)InnerNodes[i][j].PositionSQL.Lat, (float)InnerNodes[i][j].PositionSQL.Lon, bounds, minmaxX, minmaxY);
                                // Testing the correct direction
                                holes[i][j] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                            }
                            holes[i] = holes[i].RemoveDuplicates();
                        }

                        if (RemoveRedundantPointsOnTheSameLine)
                        {
                            for (int i = 0; i < tempPointsOuterPoints.Count; i++)
                            {
                                tempPointsOuterPoints[i] = tempPointsOuterPoints[i].RemoveRedundantPointsInTheSameLines();
                            }
                            for (int i = 0; i < holes.Count; i++)
                            {
                                holes[i] = holes[i].RemoveRedundantPointsInTheSameLines();
                            }
                        }
                        // Check the holes against all outers
                        List<IEnumerable<Poly2Tri.PolygonPoint>> outerpolypoints = new List<IEnumerable<Poly2Tri.PolygonPoint>>();
                        for (int i = 0; i < tempPointsOuterPoints.Count; i++)
                        {
                            outerpolypoints.Add(tempPointsOuterPoints[i].Select(s => new Poly2Tri.PolygonPoint(s.x, s.z)));
                        }

                        //= tempPointsOuterPoints
                        var Polies = outerpolypoints.Select(opp => new Poly2Tri.Polygon(opp)).ToArray();
                        //Poly2Tri.Polygon poly = new Poly2Tri.Polygon(outerpolypoints);
                        // TODO: poly.IsPointInside() check the holes against all pieces

                        var innerspolypoints = new List<Poly2Tri.PolygonPoint[]>();
                        for (int i = 0; i < holes.Count; i++)
                        {
                            var currentInner = holes[i].Select(s => new Poly2Tri.PolygonPoint(s.x, s.z)).ToArray();
                            innerspolypoints.Add(currentInner);
                            for (int j = 0; j < Polies.Length; j++)
                            {
                                if (Polies[j].IsPointInside(new Poly2Tri.TriangulationPoint(currentInner[0].X, currentInner[0].Y)))
                                    Polies[j].AddHole(new Poly2Tri.Polygon(currentInner));
                            }
                        }
                        for (int i = 0; i < Polies.Length; i++)
                        {
                            try
                            {

                                Poly2Tri.P2T.Triangulate(Polies[i]);
                            }
                            catch (Exception e)
                            {
                                //throw e;
                            }
                        }

                        //var randHeight = CoordinateConvertor.linear((float)rand.NextDouble(), 0, 1, -3f, height);
                        //var randMaterial = (randHeight > height / 2f) ? "BuildingTall" : randHeight < height / 2f ? "Building2" : "Building";
                        var randHeight = properties.BuildingHeightVariation.GetNextCircular();
                        var randMaterial = (randHeight > (properties.BuildingHeightVariation.Average + properties.BuildingHeightVariation.Max) / 2f) ? "BuildingTall" : ("Building" + rand.Next(1, 5));

                        List<GameObject> resultedGameObjects = new List<GameObject>();
                        for (int i = 0; i < Polies.Length; i++)
                        {
                            var currentGameObject = Polies[i].GenerateShapeUVedWithWalls_Balanced(
                                    CoordinateConvertor.OSMType.Relation, FirstWay,
                                    "Building", "Building", randMaterial,
                                /*height +*/ randHeight,/* height + 7*/properties.BuildingHeightVariation.Max, true);
                            resultedGameObjects.Add(currentGameObject);
                        }
                        return resultedGameObjects.CombineGameObjectsOfTheSameMaterial();
                    }
                    con.Close();
                }
            }
            return null;

        }
        [Obsolete("TODO: Remove this.", true)]
        public static GameObject OSMtoGameObject2(string OSMid, Vector3 MinPointOnMap, Bounds bounds, MapProperties properties, string connPostGreSql, OSMShape shape = OSMShape.Way)
        {
            if (rand == null)
                rand = new Random((int)DateTime.Now.Ticks);
            if (properties == null)
                properties = Properties;
            var minmaxX = properties.minMaxX;
            var minmaxY = properties.minMaxY;
            int direction = -1;
            float LineWidth = properties.RoadLineThickness;
            float BuildingWidth = properties.BuildingLineThickness;
            float height = properties.BuildingHeight;
            if (height < 4)
                height = 4;
            var FirstWay = OSMid;

            if (shape == OSMShape.Way)
            {
                var w = new Way(FirstWay);
                List<OsmNode> WayNodes;
                List<Tag> WayTags;
                using (Npgsql.NpgsqlConnection con = new Npgsql.NpgsqlConnection(connPostGreSql))
                {
                    con.Open();
                    WayNodes = w.GetNodesPostgreSQL(FirstWay, con);
                    WayTags = w.GetTagsPostgreSQL(FirstWay, con);
                    con.Close();
                }
                if (WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "landuse" || i.KeyValueSQL[0].ToLower() == "building" || i.KeyValueSQL[0].ToLower() == "highway").Count() != 0)
                {
                    var tempPoints = new Vector3[WayNodes.Count];

                    int counter = 0;
                    foreach (var node in WayNodes)
                    {
                        var result = CoordinateConvertor.SimpleInterpolation((float)node.PositionSQL.Lat, (float)node.PositionSQL.Lon, bounds, minmaxX, minmaxY);
                        // Testing the correct direction
                        tempPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                        counter++;
                    }
                    WayNodes = null;
                    var building = WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "building");
                    var highwayType = WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "highway");
                    WayTags = null;
                    if (building.Count() != 0)
                    {
                        #region Buildings
                        // Check if it has overlapping start and ending points.
                        // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                        // Checking for duplicates:
                        tempPoints = tempPoints.ToArray().RemoveDuplicates();
                        var Skip = false;
                        if (tempPoints.Length <= 2)
                        {
                            // Buildings that are too small to show such as 76844368
                            // http://www.openstreetmap.org/browse/way/76844368
                            // "A weird building were found and ignored. FirstWay \nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                            Skip = true; // continue;
                        }
                        if (!Skip)
                        {
                            var p2d = tempPoints.ToCPoint2D();
                            // TODO bug in the cpolygon, probably duplicates
                            var shp = new PolygonCuttingEar.CPolygonShape(p2d);
                            shp.CutEar();
                            p2d = null;
                            GC.Collect();
                            // TODO:
                            var randHeight = CoordinateConvertor.linear((float)rand.NextDouble(), 0, 1, -3f, height);
                            var randMaterial = (randHeight > height / 2f) ? "BuildingTall" : randHeight < height / 2f ? "Building2" : "Building";
                            var resultedGameObject = shp.GenerateShapeUVedWithWalls_Balanced(
                                    CoordinateConvertor.OSMType.Polygon, FirstWay,
                                    "Building", "Building", randMaterial,
                                    height + randHeight, height + 7, true);
                            return resultedGameObject;
                        }
                        #endregion

                    }
                    else
                    {
                        if (highwayType.Count() != 0)
                        {
                            #region Roads
                            var hwtype = highwayType.First();
                            //Console.WriteLine("Generating roads..id=" + FirstWay);
                            switch (hwtype.KeyValueSQL[1])
                            {
                                case "cycleway":
                                    {
                                        var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), properties.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", properties.CycleWayMaterial.Name, -0.01f);
                                        // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                        return resultedGameObject;
                                    }
                                case "footway":
                                case "path":
                                case "pedestrian":
                                    {
                                        var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), properties.FootwayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", properties.FootWayMaterial.Name, -0.01f);
                                        // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));
                                        return resultedGameObject;
                                    }
                                case "steps":
                                    {
                                        var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), properties.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", properties.StepsMaterial.Name, -0.01f);
                                        return resultedGameObject;
                                    }

                                // Miguel R.C. commented these lines. 
                                // Reason: Why breaking in the case "motorway"? That results in some roads missing when generating scenarios.
                                // With these lines commented, the motorways will be generated in the default case. 
                                //case "motorway":
                                //    {
                                //        break;
                                //    }

                                default:
                                    {
                                        var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), properties.RoadWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", properties.RoadMaterial.Name, 0f);

                                        return resultedGameObject;

                                    }
                            }
                            #endregion
                        }
                    }
                }

            }
            else if (shape == OSMShape.Relation) // It is probably a multi-polygon building defined in a relation.
            {
                List<OsmNode[]> OuterNodes;
                List<OsmNode> OuterWay;
                List<OsmNode[]> InnerNodes;
                List<Tag> tags;
                Relation r = new Relation(FirstWay);
                Way w = new Way();
                using (Npgsql.NpgsqlConnection con = new Npgsql.NpgsqlConnection(connPostGreSql))
                {
                    con.Open();

                    tags = r.GetTagsPostgreSQL(con);
                    var isMultiPolygon = tags.Exists(t => t.KeyValueSQL[0] == "type" && t.KeyValueSQL[1] == "multipolygon");
                    if (isMultiPolygon)
                    {
                        var Members = r.GetMembersPostgreSQLByType(r.id, 1, con);

                        OuterNodes = Members
                             .Where(m => m.Role.ToLower() == Member.RoleOuter)
                             .OrderBy(o => o.order)
                             .Select(m => w.GetNodesPostgreSQL(m.ReferenceId, connPostGreSql).ToArray()).ToList();
                        InnerNodes = Members
                             .Where(m => m.Role.ToLower() == Member.RoleInner)
                             .OrderBy(o => o.order)
                             .Select(m => w.GetNodesPostgreSQL(m.ReferenceId, connPostGreSql).ToArray()).ToList();


                        OuterWay = new List<OsmNode>();
                        foreach (var nodelist in OuterNodes)
                            OuterWay.AddRange(nodelist);


                        var tempPointsOuterPoints = new Vector3[OuterWay.Count];
                        int counter = 0;
                        foreach (var node in OuterWay)
                        {
                            var result = CoordinateConvertor.SimpleInterpolation((float)node.PositionSQL.Lat, (float)node.PositionSQL.Lon, bounds, minmaxX, minmaxY);
                            // Testing the correct direction
                            tempPointsOuterPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                            counter++;
                        }

                        List<Vector3[]> holes = new List<Vector3[]>();
                        for (int i = 0; i < InnerNodes.Count; i++)
                        {
                            holes.Add(new Vector3[InnerNodes[i].Length]);

                            for (int j = 0; j < InnerNodes[i].Length; j++)
                            {
                                var result = CoordinateConvertor.SimpleInterpolation((float)InnerNodes[i][j].PositionSQL.Lat, (float)InnerNodes[i][j].PositionSQL.Lon, bounds, minmaxX, minmaxY);
                                // Testing the correct direction
                                holes[i][j] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                            }
                        }


                        var outerpolypoints = tempPointsOuterPoints.Select(s => new Poly2Tri.PolygonPoint(s.x, s.z));
                        Poly2Tri.Polygon poly = new Poly2Tri.Polygon(outerpolypoints);
                        // TODO: poly.IsPointInside() check the holes against all pieces

                        var innerspolypoints = new List<Poly2Tri.PolygonPoint[]>();
                        for (int i = 0; i < holes.Count; i++)
                        {
                            var currentInner = holes[i].Select(s => new Poly2Tri.PolygonPoint(s.x, s.z)).ToArray();
                            innerspolypoints.Add(currentInner);
                            poly.AddHole(new Poly2Tri.Polygon(currentInner));
                        }
                        Poly2Tri.P2T.Triangulate(poly);

                        var randHeight = CoordinateConvertor.linear((float)rand.NextDouble(), 0, 1, -3f, height);
                        var randMaterial = (randHeight > height / 2f) ? "BuildingTall" : randHeight < height / 2f ? "Building2" : "Building";
                        var resultedGameObject = poly.GenerateShapeUVedWithWalls_Balanced(
                                CoordinateConvertor.OSMType.Relation, FirstWay,
                                "Building", "Building", randMaterial,
                                height + randHeight, height + 7, true);
                        return resultedGameObject;
                    }
                    con.Close();
                }
            }
            return null;

        }

    }

}
