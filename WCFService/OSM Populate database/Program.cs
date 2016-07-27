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
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using OsmSharp.Osm;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.TSP;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Data.PostgreSQL.Osm.Streams;



namespace OSMPopulatedatabase
{

    class Program
    {
        // 
        // public static string conn = "Server=localhost\\SQLEXPRESS;Database=GIStest;Trusted_Connection=True;";
        public static string conn = "Server=localhost\\SQLEXPRESS;Database=GIStest;User Id=gapslabuser;Password=test;";
        public static string connPostGreSql = "Server=127.0.0.1;Port=5432;Database=GIS;User Id=postgres;Password=test;";
        public static string connPostGreSqlParis = "Server=127.0.0.1;Port=5432;Database=GIS_Paris;User Id=postgres;Password=test;CommandTimeout=60;"; //default CommandTimeout=20;
        //public static string connPostGreSqlTestForDistribution = "Server=127.0.0.1;Port=5432;Database=TestForDistribution;User Id=postgres;Password=test;";
        public static string connPostGreSqlTestForDistribution = "Server=127.0.0.1;Port=5432;Database=GIS_Paris;User Id=postgres;Password=test;CommandTimeout=60;";
        public static string connPostGreSqlNetherlands = "Server=127.0.0.1;Port=5432;Database=GIS_Netherlands;User Id=postgres;Password=test;CommandTimeout=60;";

        public static String pbffile = @"G:\Data miguelrc\Documents\GapsLabs\GaPSLabs Simulator\paris_france.osm.pbf";
        
        // Creates the necessary tables and import the pbf data to database.
        // Note: Run this only once if the tables do not exist.
        static void CreateSqlData(string pbffile, string connectionString)
        {
            Console.WriteLine("Creating SQL Data...");
            Console.WriteLine("pbffile: " + pbffile);
            Console.WriteLine("connectionString: " + connectionString);

            // var source = new PBFOsmStreamSource(new FileInfo(pbffile).OpenRead());
            var target = new PostgreSQLOsmStreamTarget(connectionString, true);

            if (Path.GetExtension(pbffile).ToLower() == ".pbf")
            {
                Console.WriteLine("Registering source for .pbf...");
                target.RegisterSource(new PBFOsmStreamSource(new FileInfo(pbffile).OpenRead()));
            }
            else if (Path.GetExtension(pbffile).ToLower() == ".osm" || Path.GetExtension(pbffile).ToLower() == ".xml")
            {
                Console.WriteLine("Registering source for .osm / .xml...");
                target.RegisterSource(new XmlOsmStreamSource(new FileInfo(pbffile).OpenRead()));
            }
            else
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The file " + pbffile + " does not have a correct extension. Only 'osm', 'xml' or 'pbf' is allowed.");
                Console.ForegroundColor = oldcolor;
                return;
            }
            Console.WriteLine("Pulling...");
            target.Pull();
            Console.WriteLine("It's done");
            Console.ReadKey();
        }

        static void CreateMSSqlData(string pbffile)
        {
            string connMSSQL2012 = "Server=localhost\\SQLEXPRESS;Database=GIS_Paris;Trusted_Connection=True;";
            var source = new PBFOsmStreamSource(new FileInfo(pbffile).OpenRead());
            var target = new OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget(connMSSQL2012, true);
            target.RegisterSource(source);
            target.Pull();
            Console.WriteLine("It's done");
            Console.ReadKey();
        }


        static void Main(string[] args)
        {
            //String testFile = @"C:\Users\admgaming\Desktop\GaPSlabs Simulator\sweden-latest.osm.pbf";
            //String Paris = @"D:\!!Necessary Software\OSM PBF\Paris\ile-de-france-latest.osm.pbf";
            //String Netherlands = @"I:\!!Necessary Software\Netherlands\netherlands-latest.osm.pbf";

            if (args.Length != 2)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("USAGE:");
                Console.WriteLine();
                Console.WriteLine("Osm Populate database.exe" + " file.osm|file.xml|file.pbf ConnectionString");
                Console.WriteLine();

                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EXAMPLE:");
                Console.WriteLine();
                Console.WriteLine("Osm Populate database.exe" + " holland.osm \"Server=127.0.0.1;Port=5432;Database=GIS;User Id=postgres;Password=test;\"");
                Console.ForegroundColor = oldcolor;
            }
            else
            {
                Console.WriteLine("Let's go");

                var oldcolor = Console.ForegroundColor;
                NpgsqlConnection con = new NpgsqlConnection(args[1]);
                try { con.Open(); }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error connecting to the database:");
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = oldcolor;
                    return;
                }
                con.Close();
                if (!File.Exists(args[0]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The file " + args[0] + " cannot be found. Please make sure the path to the file is correct.");
                    Console.ForegroundColor = oldcolor;
                    return;
                }
                CreateSqlData(args[0], args[1]);

                Console.WriteLine("Process completed!");
            }
        }
    }
}
