/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Aram.OSMParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GaPSLabs.Geometry;

namespace OSM_3D_City_Generator
{
    class Program
    {
        public static string connPostGreSql = "Server=127.0.0.1;Port=5432;Database=GIS;User Id=postgres;Password=test;";
        public static string DefaultExportPath = "H:\\Exported\\"; //H:\Exported
        public static string argConfigfile = "configfile=";
        public static string argParallel = "parallel=";
        public static string argExportTo = "exportto=";
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {

                var parallel = args.Where(a => a.ToLower().StartsWith("parallel=")).FirstOrDefault();
                var configFile = args.Where(a => a.ToLower().StartsWith("configfile=")).FirstOrDefault();
                var exportTo = args.Where(a => a.ToLower().StartsWith("exportto=")).FirstOrDefault();
                if (string.IsNullOrEmpty(configFile))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("You need to specify the path of the configuation file. \nExample: OSM 3D City Generator.exe configfile=\"c:\\test.json\" parallel=yes");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    var jsonfile = configFile.ToLower().Replace(argConfigfile, "");
                    if (string.IsNullOrEmpty(jsonfile))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("the config file could not be found, please check that you have correctly specified the config file path");
                        Console.ReadKey();
                        return;
                    }
                    var json = "";
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(jsonfile))
                    {
                        json = reader.ReadToEnd();
                    }
                    var mapProperties = JsonConvert.DeserializeObject<MapProperties>(json);

                    bool isParallel = false;
                    if (!string.IsNullOrEmpty(parallel))
                    {
                        var parallelValue = parallel.ToLower().Replace("parallel=", "");
                        if (!string.IsNullOrEmpty(parallelValue))
                            if (parallelValue == "yes")
                                isParallel = true;
                    }
                    var exportPath = "";
                    if (!string.IsNullOrEmpty(exportTo))
                    {
                        exportPath = exportTo.ToLower().Replace("exportto=", "");
                    }
                    if (string.IsNullOrEmpty(exportPath))
                    { exportPath = DefaultExportPath; }
                    if (isParallel)
                        MainParallel(null, mapProperties, exportPath);
                    else
                        MainSingle(null, mapProperties, exportPath);
                    Console.ReadLine();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You need to specify the path of the configuation file. \nExample: OSM 3D City Generator.exe configfile=\"c:\\test.json\" parallel=yes");
                Console.ReadKey();
                return;
            }
        }
        static void MainParallel(string[] args, MapProperties properties , string exportPath)
        {
            int max = 25;
            int current = 0;

            GaPSlabsVersion version = new GaPSlabsVersion("GaPSLabs 3D City Generator 1.0", 1, 0);

            if (!System.IO.Directory.Exists(exportPath))
                System.IO.Directory.CreateDirectory(exportPath);

            Random rand = new Random((int)DateTime.Now.Ticks);
            MapProperties mapboundaries = new MapProperties();
            if (properties != null)
                mapboundaries = properties;
            else
            {
                //mapboundaries.minLat = 59.3457;
                //mapboundaries.maxLat = 59.3527;
                //mapboundaries.minLon = 18.0609;
                //mapboundaries.maxLon = 18.0765;
                //mapboundaries.Name = "KTH Area";
                mapboundaries.minLat = 59.2294;
                mapboundaries.maxLat = 59.4800;
                mapboundaries.minLon = 17.7649;
                mapboundaries.maxLon = 18.2977;
                mapboundaries.Name = "KTH Area";
                mapboundaries.BuildingLineThickness = 0.6f;
                mapboundaries.RoadLineThickness = 0.2f;
                mapboundaries.Scale = new Vector2(16, 16);
                mapboundaries.BuildingColor = new Color(0, 255, 0);
                mapboundaries.LineColorStart = new Color(255, 255, 0);
                mapboundaries.LineColorEnd = new Color(255, 255, 0);
                mapboundaries.BuildingMaterial = null;
                mapboundaries.RoadMaterial = new Material("Route");
                mapboundaries.CycleWayMaterial = new Material("RouteCycleway");
                mapboundaries.FootWayMaterial = new Material("RouteFootway");
                mapboundaries.RailWayMaterial = null;
                mapboundaries.StepsMaterial = new Material("RouteSteps");
                mapboundaries.RoadWidth = 1;// 0.05f;
                mapboundaries.CyclewayWidth = 0.05f;
                mapboundaries.FootwayWidth = 0.05f;
                mapboundaries.BuildingHeight = 7.5f;
                mapboundaries.CombinationOptimizationSize = new Vector2(100, 100);
            }

            bool GenerateBuildingShapes = true;
            bool GenerateRoads = true;
            bool GenerateBuildings = true;
            bool CorrectAspectRatio = false;
            
            OSMPostgresqlSource source = new OSMPostgresqlSource(connPostGreSql);
            var bounds = source.Bounds;

            float[] minmaxX;
            float[] minmaxY;

            minmaxX = new float[] { 0, 5000 };
            minmaxY = new float[] { 0, 5000 };

            Bounds SelectedArea = new Bounds();
            float LineWidth = 0.4f;
            float BuildingWidth = 0.6f;

            float height;
            height = 7.5f;

            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            if (CorrectAspectRatio)
            {
                var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
                minmaxY[1] = (float)(minmaxX[1] * aspectRatio);
            }

            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            height = mapboundaries.BuildingHeight;
            if (height < 4)
                height = 4;

            string[] ways = null;
            if (!GenerateRoads)
            {
                string[][] buildingtag = new string[1][];
                buildingtag[0] = new string[] { "building", "" }; // NOTE: building tag in OSM is in lower case.
                ways = source.GetWayIdsWithTags(SelectedArea, buildingtag);
            }
            else
                if (!GenerateBuildings)
                {
                    string[][] roadtag = new string[1][];
                    roadtag[0] = new string[] { "highway", "" }; // NOTE: highway tag in OSM is in lower case.
                    ways = source.GetWayIdsWithTags(SelectedArea, roadtag);
                }
                else
                    ways = source.GetWayIdsInBound(SelectedArea);
            
            float[] MinPointOnArea = CoordinateConvertor.SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, bounds, minmaxX, minmaxY);

            int direction = -1;
            Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
            mapboundaries.MinPointOnMap = MinPointOnMap;

            int totalWays = ways.Length;
            int progress = 0;
            //List<OsmNode> WayNodes;
            //List<Tag> WayTags;
            //Vector3[] tempPoints;
            //PolygonCuttingEar.CPolygonShape shp;
            Console.WriteLine("Started at " + DateTime.Now + " for " + ways.Length + " objects.");
            var duration = Stopwatch.StartNew();
            Parallel.ForEach<string>(ways, (FirstWay, state) =>
            //foreach (var FirstWay in ways)
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
                        if (GenerateBuildings)
                        {
                            //Debug.Log("Current building: "+FirstWay);
                            if (GenerateBuildingShapes)
                            {
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
                                    //Console.WriteLine("Generating building..id=" + FirstWay);

                                    // To file:
                                    // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));
                                    
                                    // To PostGreSql LargeObjects:
                                    var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                    var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                    AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                    // gc.id , gc is now set
                                }
                            }
                            else
                                Console.WriteLine("TODO");
                            // draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay, "Building", "Building");
                        }
                    }
                    else
                    {
                        if (highwayType.Count() != 0)
                        {
                            if (GenerateRoads)
                            {
                                var hwtype = highwayType.First();
                                //Console.WriteLine("Generating roads..id=" + FirstWay);
                                switch (hwtype.KeyValueSQL[1])
                                {
                                    case "cycleway":
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapboundaries.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.CycleWayMaterial.Name, -0.01f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                    case "footway":
                                    case "path":
                                    case "pedestrian":
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapboundaries.FootwayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.FootWayMaterial.Name, -0.01f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                    case "steps":
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapboundaries.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.StepsMaterial.Name, -0.01f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                    case "motorway":
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapboundaries.RoadWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.RoadMaterial.Name, 0f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                }
                                //current++;
                                //if (current > max)
                                //    state.Break();
                            }
                        }
                    }
                }
            });
            duration.Stop();
            Console.WriteLine("Finished in " + duration.Elapsed.Minutes + " minutes, " + duration.Elapsed.Seconds + " seconds, " + duration.Elapsed.Milliseconds + " milliseconds.");
            Console.ReadLine();
        }

        static geometryCollection AddToDatabase(byte[] binaryData,string GisId,string name,GameObject gameObject,GaPSlabsVersion version,string connectionPostgreSql)
        {
            // To PostGreSql LargeObjects
            geometryCollection gc = new geometryCollection();
            gc.format = "obj";
            gc.name = name;
            gc.gisId = int.Parse(GisId);
            gc.gisType = "way";
            gc.largeObject = binaryData;
            gc.lastUpdate = DateTime.Now;
            // gc.latitude= not applicable
            // gc.longitude= not applicable
            gc.pivot = gameObject.position;
            gc.version = version;
            gc.AddGeometryCollectionToDatabase(connPostGreSql, true);
            return gc;
            // gc.id , gc is now set
        }
        static object locking = new object();
        static void MainSingle(string[] args, MapProperties properties, string exportPath)
        {
            int max = 25;
            int current = 0;

            GaPSlabsVersion version = new GaPSlabsVersion("GaPSLabs 3D City Generator 1.0", 1, 0);

            if (!System.IO.Directory.Exists(exportPath))
                System.IO.Directory.CreateDirectory(exportPath);

            Random rand = new Random((int)DateTime.Now.Ticks);
            MapProperties mapboundaries = new MapProperties();
            if (properties != null)
                mapboundaries = properties;
            else
            {
                //mapboundaries.minLat = 59.3457;
                //mapboundaries.maxLat = 59.3527;
                //mapboundaries.minLon = 18.0609;
                //mapboundaries.maxLon = 18.0765;
                //mapboundaries.Name = "KTH Area";
                mapboundaries.minLat = 59.2294;
                mapboundaries.maxLat = 59.4800;
                mapboundaries.minLon = 17.7649;
                mapboundaries.maxLon = 18.2977;
                mapboundaries.Name = "KTH Area";
                mapboundaries.BuildingLineThickness = 0.6f;
                mapboundaries.RoadLineThickness = 0.2f;
                mapboundaries.Scale = new Vector2(16, 16);
                mapboundaries.BuildingColor = new Color(0, 255, 0);
                mapboundaries.LineColorStart = new Color(255, 255, 0);
                mapboundaries.LineColorEnd = new Color(255, 255, 0);
                mapboundaries.BuildingMaterial = null;
                mapboundaries.RoadMaterial = new Material("Route");
                mapboundaries.CycleWayMaterial = new Material("RouteCycleway");
                mapboundaries.FootWayMaterial = new Material("RouteFootway");
                mapboundaries.RailWayMaterial = null;
                mapboundaries.StepsMaterial = new Material("RouteSteps");
                mapboundaries.RoadWidth = 1;// 0.05f;
                mapboundaries.CyclewayWidth = 0.05f;
                mapboundaries.FootwayWidth = 0.05f;
                mapboundaries.BuildingHeight = 7.5f;
                mapboundaries.CombinationOptimizationSize = new Vector2(100, 100);
            }

            bool GenerateBuildingShapes = true;
            bool GenerateRoads = true;
            bool GenerateBuildings = true;
            bool CorrectAspectRatio = false;

            OSMPostgresqlSource source = new OSMPostgresqlSource(connPostGreSql);
            var bounds = source.Bounds;

            float[] minmaxX;
            float[] minmaxY;

            minmaxX = new float[] { 0, 5000 };
            minmaxY = new float[] { 0, 5000 };

            Bounds SelectedArea = new Bounds();
            float LineWidth = 0.4f;
            float BuildingWidth = 0.6f;

            float height;
            height = 7.5f;

            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            if (CorrectAspectRatio)
            {
                var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
                minmaxY[1] = (float)(minmaxX[1] * aspectRatio);
            }

            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            height = mapboundaries.BuildingHeight;
            if (height < 4)
                height = 4;

            string[] ways = null;
            if (!GenerateRoads)
            {
                string[][] buildingtag = new string[1][];
                buildingtag[0] = new string[] { "building", "" }; // NOTE: building tag in OSM is in lower case.
                ways = source.GetWayIdsWithTags(SelectedArea, buildingtag);
            }
            else
                if (!GenerateBuildings)
                {
                    string[][] roadtag = new string[1][];
                    roadtag[0] = new string[] { "highway", "" }; // NOTE: highway tag in OSM is in lower case.
                    ways = source.GetWayIdsWithTags(SelectedArea, roadtag);
                }
                else
                    ways = source.GetWayIdsInBound(SelectedArea);

            float[] MinPointOnArea = CoordinateConvertor.SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, bounds, minmaxX, minmaxY);

            int direction = -1;
            Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
            mapboundaries.MinPointOnMap = MinPointOnMap;

            int totalWays = ways.Length;
            int progress = 0;
            List<OsmNode> WayNodes;
            List<Tag> WayTags;
            Vector3[] tempPoints;
            PolygonCuttingEar.CPolygonShape shp;
            Console.WriteLine("Started at " + DateTime.Now + " for " + ways.Length + " objects in Single-Threaded environment.");
            Stopwatch duration = Stopwatch.StartNew();
            foreach (var FirstWay in ways)
            {
                Way w = new Way(FirstWay);
                WayNodes = w.GetNodesPostgreSQL(FirstWay, connPostGreSql);
                WayTags = w.GetTagsPostgreSQL(FirstWay, connPostGreSql);
                if (WayTags.Where(i => i.KeyValueSQL[0].ToLower() == "landuse" || i.KeyValueSQL[0].ToLower() == "building" || i.KeyValueSQL[0].ToLower() == "highway").Count() != 0)
                {
                    tempPoints = new Vector3[WayNodes.Count];

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
                        if (GenerateBuildings)
                        {
                            //Debug.Log("Current building: "+FirstWay);
                            if (GenerateBuildingShapes)
                            {
                                // Check if it has overlapping start and ending points.
                                // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                // Checking for duplicates:
                                tempPoints = tempPoints.ToArray().RemoveDuplicates();
                                if (tempPoints.Length <= 2)
                                {
                                    // Buildings that are too small to show such as 76844368
                                    // http://www.openstreetmap.org/browse/way/76844368
                                    // "A weird building were found and ignored. FirstWay \nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                    continue;
                                }
                                var p2d = tempPoints.ToCPoint2D();
                                // TODO bug in the cpolygon, probably duplicates
                                shp = new PolygonCuttingEar.CPolygonShape(p2d);
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
                                // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                // To PostGreSql LargeObjects:
                                var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                // gc.id , gc is now set

                            }
                            else
                                Console.Write("TODO");
                            // draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay, "Building", "Building");
                        }
                    }
                    else
                    {
                        if (highwayType.Count() != 0)
                        {
                            if (GenerateRoads)
                            {
                                var hwtype = highwayType.First();
                                switch (hwtype.KeyValueSQL[1])
                                {
                                    case "cycleway":
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapboundaries.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.CycleWayMaterial.Name, -0.01f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                    case "footway":
                                    case "path":
                                    case "pedestrian":
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapboundaries.FootwayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.FootWayMaterial.Name, -0.01f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));
                                            
                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                    case "steps":
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapboundaries.CyclewayWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.StepsMaterial.Name, -0.01f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                    case "motorway":
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            var resultedGameObject = CoordinateConvertor.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapboundaries.RoadWidth, CoordinateConvertor.OSMType.Line, FirstWay, hwtype.KeyValueSQL[1], "Line", mapboundaries.RoadMaterial.Name, 0f);
                                            // ObjFormat.MeshToFile(resultedGameObject,System.IO.Path.Combine( exportPath , resultedGameObject.Name.Replace("|", "-") + ".obj"));

                                            // To PostGreSql LargeObjects:
                                            var objData = ObjFormat.GameObjectToString(resultedGameObject);
                                            var binaryData = System.Text.Encoding.ASCII.GetBytes(objData);
                                            AddToDatabase(binaryData, FirstWay, resultedGameObject.Name, resultedGameObject, version, connPostGreSql);
                                            // gc.id , gc is now set
                                            break;
                                        }
                                }
                                //current++;
                                //if (current > max)
                                //    break;
                            }
                        }
                    }
                }
            }
            duration.Stop();
            Console.WriteLine("Finished in " + duration.Elapsed.Minutes + " minutes, " + duration.Elapsed.Seconds + " seconds, " + duration.Elapsed.Milliseconds + " milliseconds.");
            Console.ReadLine();
        }
    }
}
