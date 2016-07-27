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
using System.IO;
using Aram.OSMParser;
using System.Xml;
using System.Data.SqlClient;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using OsmParserTestApplication.ServiceReference1;
using OsmSharp.Osm.Data.SQLServer.SimpleSchema;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.SQLServer.SimpleSchema.Processor;
using OsmSharp.Osm.Data.PostgreSQL.SimpleSchema.Processor;
using OsmSharp.Osm.Data.PostgreSQL.SimpleSchema;
using OsmSharp.Osm;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Data.Source;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Route;
using OsmSharp.Routing.TSP;
using OsmSharp.Routing.Router;
using System.Collections;
using OsmSharp.Routing.Osm.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Wrappers;
using System.Drawing;
using ProtoBuf;


namespace OsmParserTestApplication
{

    class Program
    {
        // 
        // public static string conn = "Server=localhost\\SQLEXPRESS;Database=GIStest;Trusted_Connection=True;";
        public static string conn = "Server=localhost\\SQLEXPRESS;Database=GIStest;User Id=gapslabuser;Password=test;";
        public static string connPostGreSql = "Server=127.0.0.1;Port=5432;Database=GIS;User Id=postgres;Password=test;";
        public static string connPostGreSqlParis = "Server=127.0.0.1;Port=5432;Database=GIS_Paris;User Id=postgres;Password=test;";
        public static string connPostGreSqlGSSAPI = "Server=k05n02.pdc.kth.se;Port=29988;Database=azhari;User Id=azhari;";
        public static string connPostGreSqlTestForDistribution = "Server=127.0.0.1;Port=5432;Database=TestForDistribution;User Id=postgres;Password=test;";
        static string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase;
        static string wcfConmssql = ServicePropertiesClass.ConnectionDatabase;
        // Creates the necessary tables and import the pbf data to database.
        // Note: Run this only once if the tables do not exist.
        static void CreateSqlData()
        {
            String pbffile = @"C:\Users\admgaming\Desktop\Notable Software\Stockholm Data\sweden-latest.osm.pbf";
            String pbffile2 = @"D:\!!Necessary Software\OSM PBF\Iran\Iran-latest.osm.pbf";
            PBFDataProcessorSource source = new PBFDataProcessorSource(new FileInfo(pbffile2).OpenRead());

            // create the PostgreSQL processor target.
            // SQLServerSimpleSchemaDataProcessorTarget test_target = new SQLServerSimpleSchemaDataProcessorTarget(conn, true);
            PostgreSQLSimpleSchemaDataProcessorTarget test_target = new PostgreSQLSimpleSchemaDataProcessorTarget(connPostGreSqlTestForDistribution, true);
            test_target.RegisterSource(source); // register the source.
            test_target.Pull(); // pull the data from source to target.
        }

        public static void testPostgreSQL()
        {
            OSMPostgresqlSource sourcePostgre = new OSMPostgresqlSource(connPostGreSql);
            var bbbbpostgre = sourcePostgre.Bounds;
            OsmNode nooode = new OsmNode();
            nooode.id = "81069";
            var taagspg = nooode.GetTagsPostgreSQL(connPostGreSql);
            Aram.OSMParser.Relation rel = new Aram.OSMParser.Relation("2340");
            var members = rel.GetMembersPostgreSQL(connPostGreSql);

            Console.WriteLine("Completed without errors.");
            Console.ReadLine();
        }

        [DllImport("CUDA Runtime.dll", EntryPoint = "GPU_Add", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GPU_Add(IntPtr a, IntPtr b, IntPtr c, int size, int blocksize);
        [DllImport("CUDA Runtime.dll", EntryPoint = "CPU_Add", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CPU_Add(IntPtr a, IntPtr b, IntPtr c, int size);
        [DllImport("CUDA Runtime.dll", EntryPoint = "CPU_AddParallel", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CPU_AddParallel(IntPtr a, IntPtr b, IntPtr c, int size);
        [DllImport("CUDA Runtime.dll", EntryPoint = "GPU_WarmUp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GPU_WarmUp();
        public static double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
        static void Main(string[] args)
        {
            ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService();
                BoundsWCF b = new BoundsWCF();
            b.minlat = 59.32973;
            b.maxlat = 59.34481;
            b.minlon = 18.07556;
            b.maxlon = 18.1062;
            //client.GetWayIdsWithTags(connPostGreSql, b, new string[][] { new string[] { "type", "multipolygon" } });
            var relationbuildings= client.GetRelationBuildingsInBound(connPostGreSql,b);
            return;




            TestStruct ts1 = new TestStruct();
            ts1.ids = 5;
            ts1.s = new string[] { "abc", "DEF" };



            ProtoBuf.Serializer.PrepareSerializer<TestStruct>();
            using (StreamWriter writer = new StreamWriter("c:\\test.txt"))
            {
                Serializer.Serialize<TestStruct>(writer.BaseStream, ts1);
            }
            return;


            


            var testData=new string[] {"Testing","Gintset"};
            TestClass tc = new TestClass();
            tc.content = testData;
            var ptr1=Marshal.AllocHGlobal(Marshal.SizeOf(tc));
            
            
            Marshal.StructureToPtr(tc,ptr1,false);
            byte[] buffered=new byte[Marshal.SizeOf(tc)];

            Marshal.Copy(ptr1, buffered, 0, buffered.Length);
            var ptr2=Marshal.AllocHGlobal(Marshal.SizeOf(tc));
            Marshal.Copy(buffered, 0, ptr2, buffered.Length);
            TestClass ds=new TestClass();
            Marshal.PtrToStructure(ptr2, ds);


            

            IntPtr ts1ptr = Marshal.AllocHGlobal(Marshal.SizeOf(ts1));
            try
            {
                // SERVER
                // A pointer to the content of a class
                Marshal.StructureToPtr(ts1, ts1ptr, false);
                // Allocating the buffer to hold that content
                byte[] buffer=new byte[Marshal.SizeOf(ts1)];
                // copying the class content to a buffer array
                Marshal.Copy(ts1ptr, buffer, 0, buffer.Length);

                // CLIENT
                IntPtr ts2ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buffer.Length));
                Marshal.Copy(buffer, 0, ts2ptr, buffer.Length);

                var check = Marshal.PtrToStructure<TestStruct2>(ts2ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ts1ptr);
            }
            
            var error = client.InitializeRouter(connPostGreSql);
            //var membersWCF = client.GetRoutePath("1246645", 1, connPostGreSqlParis);
            //var membersWCF = client.GetRoutePath("2370570", 1, connPostGreSql);
            var members = Aram.OSMParser.Relation.GetRoutePathExperimental("2370570", 1, connPostGreSql);
            // Fix the line reverse problems.

            return;

            //var result = client.GetRelationsContainingMembers(connPostGreSql, new string[] { "1344331354" }, 0, "");

            //OSMPostgresqlSource sourcePostgre = new OSMPostgresqlSource(connPostGreSql);
            //Aram.OSMParser.Relation.GetRelationsPostgreSQL(new string[] { "1344331354" }, 0, "", connPostGreSql);

           

            //var connectionString = "mongodb://localhost";
            //var client = new MongoClient(connectionString);
            //var server = client.GetServer();
            //var trafiklab = server.GetDatabase("trafiklab");
            //var trafficdata = trafiklab.GetCollection<Trafiklab.MongodbBusses>("trafficdatatest");
            //var q = Query<Trafiklab.MongodbBusses>.EQ(i => i.LatestUpdate, "2014-05-07T11:26:41.7801525+02:00");
            //var result = trafficdata.Find(q);
            //var f = result.First();

        }
        static void Main2(string[] args)
        {
            CreateSqlData();
        }
        static void MainTemp(string[] args)
        {
            NpgsqlConnection schemaConnection = new NpgsqlConnection(connPostGreSql);
            schemaConnection.Open();
            var databaseName = "GIS";
            DataTable dataTables = schemaConnection.GetSchema("Tables", new string[] { databaseName, "public", null, null });

            foreach (DataRow rowTable in dataTables.Rows)
            {
                string tableName = rowTable["table_name"].ToString();
                if (tableName != "geometry_collection")
                    continue;
                DataTable dataColumns = schemaConnection.GetSchema("Columns", new string[] { databaseName, "public", tableName });
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("public class " + tableName);
                sb.AppendLine("{");
                sb.AppendLine("\tpublic " + tableName + "(){}");

                foreach (DataRow rowColumn in dataColumns.Rows)
                {
                    string columnName = rowColumn["column_name"].ToString();
                    string type = rowColumn["data_type"].ToString();
                    sb.AppendLine("\tpublic " + type + " " + columnName + " {get;set;}");
                }
                sb.AppendLine("}");
                sb.Replace("int8", "long");
                sb.Replace("int4", "int");
                sb.Replace("text", "string");
                sb.Replace("oid", "long");
                sb.Replace("numeric", "float");
                sb.Replace("timestamp", "DateTime");
                var def = sb.ToString();

            }

            schemaConnection.Close();
            return;

            var geometryRetrieval = geometryCollection.GetSingleObjectWithId("8", true, connPostGreSql);

            // testing GeometryCollection
            Aram.OSMParser.geometryCollection col = new Aram.OSMParser.geometryCollection();

            // col.gisId = 
            col.gisType = "dummy";
            col.format = "txt";
            col.largeObject = null;
            col.lastUpdate = DateTime.Now;
            col.latitude = 563213212;
            col.longitude = 171231231;
            col.name = "Test2";
            col.pivot = new Aram.OSMParser.Vector3GaPS() { x = 1f, y = 2f, z = 3f };
            col.version = new Aram.OSMParser.GaPSlabsVersion() { versionTitle = "development", major = 0, minor = 1 };

            col.AddGeometryCollectionToDatabase(connPostGreSql, false);
            var bytes = File.ReadAllBytes(@"C:\Users\admgaming\Documents\Visual Studio 2012\Projects\GaPSLabs\AramOSMParser\OsmParserTestApplication\bin\Debug\Npgsql.xml");
            col.largeObject = bytes;
            col.UpdateThisGeometryOnDatabase(connPostGreSql, true);
            var resultBytes = geometryCollection.GetLargeObject(col.largeObjectReference, connPostGreSql);
            File.WriteAllBytes("c:\\dummy", resultBytes);

            return;
            // ERROR: 42704: invalid large-object descriptor: 0 ??
            // largeobject only works within a transaction. Use bytea as an alternative to large objects.
            // http://www.postgresql.org/message-id/002701c49d7e$0f059240$d604460a@zaphod
            NpgsqlConnection testConnection = new NpgsqlConnection(connPostGreSql);
            testConnection.Open();

            NpgsqlTypes.LargeObjectManager lm = new NpgsqlTypes.LargeObjectManager(testConnection);

            var generatedLO = lm.Create(NpgsqlTypes.LargeObjectManager.READWRITE);

            // It must be within a transaction
            var TransWrite = testConnection.BeginTransaction();
            LargeObject lo = lm.Open(generatedLO, LargeObjectManager.READWRITE);
            lo.Write(new byte[] { 0, 10, 50, 24 });
            lo.Close();
            TransWrite.Commit();

            var TransRead = testConnection.BeginTransaction();
            var loOid = lo.GetOID();
            var readlo = lm.Open(loOid, LargeObjectManager.READWRITE);
            var resultLo = readlo.Read(readlo.Size());
            lm.Delete(generatedLO);
            TransRead.Commit();

            testConnection.Close();
            return;

            OSMPostgresqlSource sourceVisTest = new OSMPostgresqlSource(connPostGreSql);
            var bounds = sourceVisTest.Bounds;





            return;
            GaPSlabsSimulationLibrary.SUMOSimulationFCD df = new GaPSlabsSimulationLibrary.SUMOSimulationFCD();
            //df.LoadFromXML(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\Pedestrians.xml");
            //df.LoadFromXML(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\bufferoutput - 500.xml");

            ServiceGapslabsClient client2 = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
            int id = client2.LoadSUMOFCDSimulationList(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\bufferoutput - 500.xml", "__POSTFIX");
            //client.LoadSUMOFCDSimulation(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\bufferoutput - 500.xml");
            while (!client2.IsSimulationLoadedList(id))
            { }
            var vvvv = client2.GetTimestepAtList(6, id);
            var vvvv2 = client2.GetTimestepAtList(7, id);


            return;


            int size = 16777216;
            int[] aa = new int[size];
            int[] bbb = new int[size];
            int[] cccc = new int[size];
            for (int i = 0; i < size; i++)
            {
                aa[i] = i;
                bbb[i] = i;
            }
            var apointer = aa.ToIntPtr<int[]>();
            var bpointer = bbb.ToIntPtr<int[]>();
            var cpointer = cccc.ToIntPtr<int[]>();

            long MinGPU = 1000000;
            long MinCPU = 1000000;
            long MinCPUParallel = 100000;
            Stopwatch watch = new Stopwatch();

            bool SkipCpu = false;
            GPU_WarmUp();
            int TestCounter = 0;
            int blockSize = 16;
            while (TestCounter++ < 7)
            {
                watch.Restart();
                GPU_Add(apointer, bpointer, cpointer, size, blockSize);
                watch.Stop();
                Console.WriteLine("Total GPU" + "(" + blockSize + ")" + ": " + watch.ElapsedMilliseconds);
                if (watch.ElapsedMilliseconds < MinGPU)
                    MinGPU = watch.ElapsedMilliseconds;
                blockSize *= 2;
            }
            Console.WriteLine("Minimum GPU was " + MinGPU);

            if (!SkipCpu)
            {
                TestCounter = 0;
                while (TestCounter++ < 10)
                {
                    watch.Restart();
                    CPU_AddParallel(apointer, bpointer, cpointer, size);
                    watch.Stop();
                    Console.WriteLine("Total CPU Parallel: " + watch.ElapsedMilliseconds);
                    if (watch.ElapsedMilliseconds < MinCPUParallel)
                        MinCPUParallel = watch.ElapsedMilliseconds;
                }
                Console.WriteLine("Minimum CPU was " + MinCPU);

                TestCounter = 0;
                while (TestCounter++ < 10)
                {
                    watch.Restart();
                    CPU_Add(apointer, bpointer, cpointer, size);
                    watch.Stop();
                    Console.WriteLine("Total CPU: " + watch.ElapsedMilliseconds);
                    if (watch.ElapsedMilliseconds < MinCPU)
                        MinCPU = watch.ElapsedMilliseconds;
                }
                Console.WriteLine("Minimum CPU was " + MinCPU);
            }
            //apointer.Free();
            //bpointer.Free();
            //cpointer.Free();
            Console.ReadLine();
            return;
            //GaPSlabsSimulationLibrary.SUMOSimulationFCD simulation = new GaPSlabsSimulationLibrary.SUMOSimulationFCD();
            //simulation.LoadFromXML(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMOData\fcdoutput.xml");
            //simulation.LoadFromCSV(@"C:\Users\admgaming\Desktop\Notable Software\iMobility\stkhlm-taxi.csv");

            ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
            //client.LoadSUMOFCDSimulation(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\bufferoutput - 500.xml");
            //while (!client.IsSimulationLoaded())
            //    Console.WriteLine("Loading...");
            //Console.WriteLine("Load finished");
            //Console.ReadLine();
            //return;









            OSMPostgresqlSource sour = new OSMPostgresqlSource(connPostGreSql);
            // var TrafficNodes = sour.GetNodeIdsInBoundWithInfo(sour.Bounds, "traffic_signals");



            var result = client.GetWayTags("134972364", connPostGreSql);



            BoundsWCF b = new BoundsWCF();
            b.minlat = 59.32973;
            b.maxlat = 59.34481;
            b.minlon = 18.07556;
            b.maxlon = 18.1062;
            client.GetWayExtIdsInBound(connPostGreSql, b);

            client.InitializeRouter(connPostGreSql);

            OsmNodeWCF n1 = new OsmNodeWCF();
            n1.id = "none";
            n1.order = -1;
            n1.lat = 59.330957;
            n1.lon = 18.059285;
            //n1.lat = 59.374563;
            //n1.lon = 18.0135727;
            OsmNodeWCF n2 = new OsmNodeWCF();
            n2.id = "none";
            n2.order = -1;
            n2.lat = 59.33784;
            n2.lon = 18.088558;
            //n2.lat = 59.37225;
            //n2.lon = 18.00733;


            var RouterResult = client.RouteUsingDykstra(VehicleEnum.Car, n1, n2);

            OsmGeo.ShapeInterperter = new SimpleShapeInterpreter();
            PostgreSQLSimpleSchemaSource source = new PostgreSQLSimpleSchemaSource(connPostGreSql);

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
            IRouter<RouterPoint> router = new Router<PreProcessedEdge>(routing_data, interpreter
                    , new DykstraRoutingPreProcessed(routing_data.TagsIndex));


            // resolve both points; find the closest routable road.

            //RouterPoint point1 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(60.1674654,18.454302));
            // RouterPoint point2 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(60.1673373,18.4541732));

            // Working
            //RouterPoint point1 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(59.3863281, 18.0176665));
            //RouterPoint point2 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(59.3675634, 18.0140447));

            // Working
            RouterPoint point1 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(59.374563, 18.0135727));
            RouterPoint point2 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(59.37225, 18.00733));

            //ArrayList al=new ArrayList();
            //foreach (var en in Enum.GetValues(typeof(VehicleEnum)))
            //{
            //    al.Add(Enum.GetName(typeof(VehicleEnum), (VehicleEnum)en) + "=" + router.SupportsVehicle((VehicleEnum)en));
            //}

            // calculate route.
            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, point1, point2);

            route.SaveAsGpx(new FileInfo("route.gpx"));



            Console.ReadLine();
        }

        //public static void MainWcf()
        //{
        //	int count = 0;

        //	ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        //	//		int StressTest=0;
        //	//Color buildingColor=new Color(0,0,255,255);

        //	var boundsTemp = client.GetBounds(wcfCon);
        //	Aram.OSMParser.Bounds bounds = new Aram.OSMParser.Bounds();
        //	bounds.minlat = boundsTemp.minlat;
        //	bounds.maxlat = boundsTemp.maxlat;
        //	bounds.minlon = boundsTemp.minlon;
        //	bounds.maxlon = boundsTemp.maxlon;

        //	// KTH minlat="59.3472200" minlon="18.0653800" maxlat="59.3520400" maxlon="18.0759300"
        //	// Amsterdam airport <bounds minlat="52.2861000" minlon="4.7294000" maxlat="52.3183000" maxlon="4.7813000"/>
        //	BoundsWCF SelectedArea = new BoundsWCF();
        //	//		KTH
        //	//		SelectedArea.minlat=59.3472200;
        //	//		SelectedArea.maxlat=59.3520400;
        //	//		SelectedArea.minlon=18.0653800;
        //	//		SelectedArea.maxlon=18.0759300;
        //	//		 <bounds minlat="59.3675700" minlon="18.0044500" maxlat="59.3757400" maxlon="18.0175600"/>
        //	SelectedArea.minlat = 59.3675700;
        //	SelectedArea.minlon = 18.0044500;
        //	SelectedArea.maxlat = 59.3757400;
        //	SelectedArea.maxlon = 18.0175600;



        //	string[] ways = new string[] { "79642106" }; //client.GetWayIdsInBound(wcfCon,SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        //	//Debug.Log("Number of Ways:"+ways.Length);


        //	//LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        //	int totalWays = ways.Length;
        //	float progress = 0f;
        //	float[] minmaxX = new float[] { -50000f, 50000f };
        //	float[] minmaxY = new float[] { -50000f, 50000f };
        //	foreach (var FirstWay in ways)
        //	{
        //		var WayNodes = client.GetWayNodes(FirstWay, wcfCon); //new Way(FirstWay);
        //		var WayTags = client.GetWayTags(FirstWay, wcfCon);

        //		if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
        //		{
        //			Vector3[] tempPoints = new Vector3[WayNodes.Length];
        //			int counter = 0;

        //			foreach (var node in WayNodes)
        //			{
        //				var result = StandardConverters.WGS84toXY_SimpleInterpolation(new GeoPosition(node.lat, node.lon), bounds, minmaxX, minmaxY);
        //				tempPoints[counter] = new Vector3(result[0], 0, result[1]);
        //				counter++;
        //			}

        //			var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
        //			//if (building.Count()!=0)
        //			//    draw.Draw(tempPoints, buildingColor,buildingColor,1.2f,1.2f,LineDraw.OSMType.Line,FirstWay,"Building");
        //			//else
        //			//    draw.Draw(tempPoints, Color.red,Color.yellow,1f,1f,LineDraw.OSMType.Line,FirstWay,null);
        //		}

        //	}


        //}
        public void OldMainSql()
        {
            //OSMSqlSource source = new OSMSqlSource(conn);
            //var bbbb = source.Bounds;
            //var ways = source.WayIds;
            //int count = 0;
            //foreach (var s in ways)
            //{
            //	Aram.OSMParser.Way w = new Aram.OSMParser.Way(s);
            //	Console.WriteLine("WayID:" + s);
            //	var nodes = w.GetNodesSQL(conn);
            //	//var tags =nodes[0].GetTagsSQL(conn); //.GetPositionSQL(conn);
            //	foreach (var n in nodes)
            //	{
            //		if (n.GetTagsSQL(conn).Count != 0)
            //		{
            //			Console.WriteLine("Order:" + n.order + "\tPosition:" + n.PositionSQL.Lat);
            //			Console.WriteLine(n.GetTagsSQL(conn)[0].KeyValueSQL[0]);
            //		}
            //	}
            //	if (count++ == 100)
            //		break;

            //}
        }
        //public void OldMain()
        //{
        //	OSMSource source = new OSMSource(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "mapKTH.osm"));
        //	OSMSourceNonLinq sourcepath = new OSMSourceNonLinq(source.filename);
        //	var boundsTest = sourcepath.XmlDocument.SelectSingleNode("./osm/bounds");

        //	XmlNodeList wwww = sourcepath.Ways;



        //	Aram.OSMParser.Way way1 = new Aram.OSMParser.Way(source.Ways.Last(), source.OsmDocument);
        //	var startPosition = way1.Nodes[0].Position;
        //	var nodesWithTags = way1.Nodes.Where(i => i.Tags.Count != 0);
        //	// Finding a node with id
        //	var nnnnn = source.Nodes.Where(i => i.Attribute("id").Value == "1604976405").Single();
        //	var tagsOfNode = (from n in nnnnn.Elements("tag")
        //										select new Tag(n));
        //	// Find nodes with the tag 'waste_basket'
        //	var wasteBaskets = source.Nodes.Where(
        //			i => i.Elements("tag").Any(ii => ii.Attribute("v").Value == "waste_basket"));

        //	//////

        //	var bounds = (from s in source.OsmDocument.Descendants("osm")
        //								select s.Element("bounds")).Single();

        //	var ways = (from s in source.OsmDocument.Descendants("osm")
        //							select s.Descendants("way")).Single();
        //	var testttsdfsft = source.OsmDocument.Descendants("osm").Select(i => i.Descendants("way")).Single();
        //	var selectedWay = source.Ways.Where(i => i.Attribute("id").Value == "11397176");
        //	// First way
        //	var FirstWay = selectedWay.Single();// ways.First();
        //	Console.WriteLine("Information about way {0} created by {1}:",
        //			FirstWay.Attribute("id").Value, FirstWay.Attribute("user").Value);
        //	var WayNodes = from w in FirstWay.Elements("nd")
        //								 select w.Attribute("ref").Value;


        //	var WayTags = from w in FirstWay.Elements("tag")
        //								select new
        //								{
        //									Key = w.Attribute("k").Value,
        //									Value = w.Attribute("v").Value
        //								};
        //	Console.WriteLine("Tags:");
        //	foreach (var tag in WayTags)
        //		Console.WriteLine("{0} = {1}", tag.Key, tag.Value);

        //	var allNodes = source.OsmDocument.Descendants("osm").Select(i => i.Elements("node")).First();
        //	// A VERY IMPORTANT NOTE:
        //	// Since the way nodes in OSM files are in the correct order, they have to be read in the same order.
        //	// Otherwise, you will get confusing results.
        //	// Since querying is a cartesian product, the selected nodes in the current Way should appear first 
        //	// in the LINQ query to preserve the correct order of the nodes. (ie. From w in WayNodes , before from s in allNodes)
        //	var nodePositions = from w in WayNodes
        //											from s in allNodes
        //											where s.Attribute("id").Value == w
        //											select new
        //											{
        //												id = s.Attribute("id").Value,
        //												lat = s.Attribute("lat").Value,
        //												lon = s.Attribute("lon").Value
        //											};
        //	Console.WriteLine("Nodes:");

        //	foreach (var position in nodePositions)
        //	{
        //		Console.WriteLine("id={0}\tLatitude={1}\tLongtitude={2}", position.id, position.lat, position.lon);
        //		var vector = StandardConverters.ConvertWGS84toECEF_NoAltitude(new GeoPosition(position.lat, position.lon));
        //		Console.WriteLine("x={0},y={1},z={2}", vector[0], vector[1], vector[2]);
        //	}
        //}
    }

    [StructLayout(LayoutKind.Sequential)]
    public class TestClass
    {
        public TestClass() { }
        public string[] content;
    }
    [StructLayout(LayoutKind.Sequential),ProtoContract]
    public class TestStruct
    {
        public TestStruct() { }
        [ProtoMember(1)]
        public int ids;
        [ProtoMember(2)]
        public float j;
        [ProtoMember(3)]
        public string[] s;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class TestStruct2
    {
        public TestStruct2() { }
        public int iasf;
        public float jsghhjfgh;
        public string[] s;
    }

}
