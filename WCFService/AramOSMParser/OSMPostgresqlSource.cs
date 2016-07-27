/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using Npgsql;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

/*
 
Using System.Xml.Linq in Unity and on iOS
Posted: 05:40 PM 1 Week Ago

    I am so relieved that this works (at least what I've tested so far.)

    People should know, though, that in Unity 4.0 I didn't just have to add a reference to the System.Xml.Linq assembly in MonoDevelop - but I had to actually locate the assembly itself and drag it into my Assets folder.

    That took me a while to figure out because Mono would build just fine but I would get errors about missing namespace System.Xml.Linq when trying to run it in the Editor (much less iOS.)

    I did have a hiccup at one time where Unity reported that the AOT compile failed relating to link but I shut down Mono and Unity and did a complete rebuild and it worked just fine again.

    Linq is a great time saver Thanks for the great work Unity team!

    Hans

    P.S. I used the version 3.5 System.Xml.Linq assembly. 


 */
namespace Aram.OSMParser
{
    /// <summary>
    /// The Aram.OSMParser namespace is a collection of classes that will help retrieve data from the database,
    ///  regarding the OpenStreetMaps elements such as nodes, ways and relations.
    /// </summary>
    /// <remarks>Please refer to <a href="http://www.openstreetmap.org/help"> for more information.</a></remarks>
    internal class NamespaceDoc
    {
    }
    /// <summary>
    /// This is the OpenStreetMaps database connection class. It provides methods to retrive information regarding nodes, ways and relation and
    /// their underlying tags.
    /// </summary>
    public class OSMPostgresqlSource
    {
        /// <summary>
        /// The connectionString to the database. Please refer to <a href="http://www.npgsql.org">.Net Data Provider for Postgresql</a> for format specifications.
        /// </summary>
        public String connectionString;
        /// <summary>
        /// The Postgresql connection.
        /// </summary>
        public NpgsqlConnection conn;
        /// <summary>
        /// The scale for the latitude / longitude values over the database.
        /// <br/><strong>Note:</strong>By default all the geo position values in the database are stored as
        /// a 9 digit integer number, that should be divided by this number to get the real value.
        /// <br/><br/>For example: 584111023 becomes 584111023/ 10000000 = 58.4111023
        /// </summary>
        public static readonly double scale = 10000000d;
        /// <summary>
        /// The constructor of the OSMPostgresqlSource accepting a connection string for initialization.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database</param>
        public OSMPostgresqlSource(String connectionString)
        {
            this.connectionString = connectionString;
            conn = new NpgsqlConnection(connectionString);
        }
        /// <summary>
        /// The destructor of the OSMPostgresqlSource. This will close the opened database connection.
        /// </summary>
        ~OSMPostgresqlSource()
        {
            conn.Close();
        }
        /// <summary>
        /// Gets the geo boundaries of the available data in the database.
        /// Note that by default this property is loading the values from viewbounds_stockholm table,
        /// scales them to real values. You may change this to reflect your own region.
        /// </summary>
        /// <seealso cref="scale"/>
        public Bounds Bounds
        {
            get
            {
                Bounds b = new OSMParser.Bounds();
                NpgsqlConnection con = new NpgsqlConnection(connectionString);
                try
                {
                    con.Open();
                    NpgsqlCommand com = new NpgsqlCommand("", con);
                    com.CommandText = "select * from viewbounds_stockholm"; // viewbounds_stockholm "select * from viewbounds";
                    NpgsqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        // NOTE: The bounds data is already in scaled format.
                        b.minlat = (int)reader["minLat"] / scale;
                        b.maxlat = (int)reader["maxLat"] / scale;
                        b.minlon = (int)reader["minLon"] / scale;
                        b.maxlon = (int)reader["maxLon"] / scale;
                    }
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    con.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return b;
            }
        }
        /// <summary>
        /// Gets the way ids that match the criteria.
        /// </summary>
        /// <param name="SearchQuery">The search query to match against.
        /// Take a look at Way table in the database for definitions.</param>
        /// <returns>Returns an array of string way ids.</returns>
        public String[] GetWayIds(String SearchQuery)
        {
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select id from Way where (" + SearchQuery + ")";
            NpgsqlDataReader reader = com.ExecuteReader();
            List<String> temp = new List<string>();
            while (reader.Read())
            {
                temp.Add(reader[0].ToString());
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            return temp.ToArray();
        }

        /// <summary>
        /// Gets the way ids that match the given tags and the criteria.
        /// </summary>
        /// <param name="bound">The bounds to get the ids from.</param>
        /// <param name="tags">The key-value pair to look for.</param>
        /// <returns>Returns an array of string way ids.</returns>
        public String[] GetWayIdsWithTags(Bounds bound, string[][] tags)
        {
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText =
            "SELECT DISTINCT w.way_id AS wayid " +
            "FROM way_tags w " +
            "WHERE w.way_id " +
            "IN ( " +
            "SELECT DISTINCT wayId FROM WayNodeInfoAram_cachedFromView WHERE latitude BETWEEN {0} AND {1} AND longitude BETWEEN {2} AND {3} " +
            ") ";
            com.CommandText =
                string.Format(com.CommandText, (int)(bound.minlat * scale), (int)(bound.maxlat * scale), (int)(bound.minlon * scale), (int)(bound.maxlon * scale));
            string tagString = "";
            if (tags != null)
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    if (string.IsNullOrEmpty(tags[i][1]) && string.IsNullOrEmpty(tags[i][0]))
                        continue;
                    if (string.IsNullOrEmpty(tags[i][1]))
                        tagString += "AND (w.key=" + "'" + tags[i][0] + "') ";
                    else if (string.IsNullOrEmpty(tags[i][0]))
                        tagString += "AND (w.value=" + "'" + tags[i][1] + "') ";
                    else
                        tagString += "AND (w.key=" + "'" + tags[i][0] + "' AND w.value=" + "'" + tags[i][1] + "') ";
                }
            }
            com.CommandText += tagString;
            //"AND (w.key = AND w.value)";


            NpgsqlDataReader reader = com.ExecuteReader();
            List<String> temp = new List<string>();
            while (reader.Read())
            {
                temp.Add(reader[0].ToString());
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            return temp.ToArray();
        }
        public string[] GetRelationBuildingsInBoundPostgreSQL(Bounds bound, String connectionString)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                var res = GetRelationBuildingsInBoundPostgreSQL(bound, con);
                con.Close();
                return res;
            }
            List<string> empty=new List<string>();
            return empty.ToArray();
        }
        public string[] GetRelationBuildingsInBoundPostgreSQL(Bounds bound, NpgsqlConnection con)
        {
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "SELECT DISTINCT relationid FROM viewrelationbuildings  vrb " +
                "WHERE vrb.latitude BETWEEN {0} AND {1} AND vrb.longitude BETWEEN {2} AND {3}";

            com.CommandText = string.Format(com.CommandText, (int)(bound.minlat * scale), (int)(bound.maxlat * scale), (int)(bound.minlon * scale), (int)(bound.maxlon * scale));

            List<string> result = new List<string>();
            NpgsqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.Add(reader["relationid"].ToString());
                }
                if (!reader.IsClosed)
                    reader.Close();
                return result.ToArray();
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();
                return null;
            }
        }

        /// <summary>
        /// Get the node ids in the given bound that have the matching tags.
        /// </summary>
        /// <param name="bound">The boundary to search in.</param>
        /// <param name="infoTag">The tag to match against.</param>
        /// <returns>Returns an array of string node ids.</returns>
        public String[] GetNodeIdsInBoundWithInfo(Bounds bound, String infoTag)
        {
            /*
                    select n.id from node n left join viewnodetaginfoaram i
                    on i.nodeid=n.id
                    where n.latitude between 593270000 and 593505000 and n.longitude between 180242000 and 181029000
                    and i.info like '%traffic_signals%' and i.name='highway'
             */
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select n.id from node_stockholm n left outer join viewnodetaginfoaram i on i.nodeid=n.id "
                    + String.Format("where n.latitude between {0} and {1} and n.longitude between {2} and {3} ",
                                                    (int)(bound.minlat * scale), (int)(bound.maxlat * scale), (int)(bound.minlon * scale), (int)(bound.maxlon * scale))
                    + String.Format("and i.info like '%{0}%'", infoTag);
            com.CommandText = com.CommandText.Replace(",", ".");
            NpgsqlDataReader reader = com.ExecuteReader();
            List<String> temp = new List<string>();
            while (reader.Read())
            {
                temp.Add(reader[0].ToString());
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            return temp.ToArray();
        }
        /// <summary>
        /// Get the node ids in the given bound that have the matching tags pairs.
        /// </summary>
        /// <param name="bound">The boundary to search in.</param>
        /// <param name="infoTag">The tag pairs to match against.
        /// <para/><strong>Note:</strong> infoTag is a n by 2 array. 
        /// <para/>example: infoTag= new String[3][];
        /// <para/>infoTag[0]=new String[] {"bus", "yes"};</param>
        /// <returns>Returns an array of string node ids.</returns>
        public String[] GetNodeIdsInBoundWithKeyValueTag(Bounds bound, String[][] infoTag)
        {
            /*
                    select distinct n.id from node_stockholm n left outer join nodetaginfoaram_cachedfromview i 
                on i.nodeid=n.id 
                where n.latitude between 593234000 and 593535000 and n.longitude between 180272000 and 180921000 
                and ( (i.name = 'highway' and i.info='bus_stop') or (i.name = 'bus' and i.info='yes') )
             */
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select distinct n.id from node_stockholm n left outer join nodetaginfoaram_cachedfromview i on i.nodeid=n.id "
                    + String.Format("where n.latitude between {0} and {1} and n.longitude between {2} and {3} ",
                                                    (int)(bound.minlat * scale), (int)(bound.maxlat * scale), (int)(bound.minlon * scale), (int)(bound.maxlon * scale));
            if (infoTag.Length != 0)
            {
                com.CommandText += "and (";
                foreach (var keyValuePair in infoTag)
                    com.CommandText += String.Format(" (i.name = '{0}' and i.info='{1}') or", keyValuePair[0], keyValuePair[1]);
                com.CommandText += ")";
                com.CommandText = com.CommandText.Replace("or)", ")");
            }

            com.CommandText = com.CommandText.Replace(",", ".");
            NpgsqlDataReader reader = com.ExecuteReader();
            List<String> temp = new List<string>();
            while (reader.Read())
            {
                temp.Add(reader[0].ToString());
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            // Debug
            System.IO.File.AppendAllText("c:\\QueryLog.txt", com.CommandText);
            return temp.ToArray();
        }
        /// <summary>
        /// Gets all the way ids in stockholm area. You may implement your own methods to target a specific region.
        /// </summary>
        /// <returns>Returns an array of string way ids.</returns>
        /// <seealso cref="GetWayIds"/>
        public String[] GetWayIdsInStockholm()
        {
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select way_Id from boundedlist " + "group by way_Id";
            com.CommandText = com.CommandText.Replace(",", ".");
            NpgsqlDataReader reader = com.ExecuteReader();
            List<String> temp = new List<string>();
            while (reader.Read())
            {
                temp.Add(reader[0].ToString());
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            return temp.ToArray();
        }
        /// <summary>
        /// Gets the way extended ids in the given boundary. 
        /// </summary>
        /// <param name="bound">The boundary of the search.</param>
        /// <returns>Returns an array of (wayid, original OSM way id) pairs.</returns>
        /// <remarks>For more details, take a look at wayextnodeinfoaram_cachedfromview view in the database.</remarks>
        public String[][] GetWayExtIdsInBound(Bounds bound)
        {
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select wayId[comma]originalwayid from wayextnodeinfoaram_cachedfromview "
                    + String.Format("where Latitude between {0} and {1} and Longitude between {2} and {3} ",
                                                    (int)(bound.minlat * scale), (int)(bound.maxlat * scale), (int)(bound.minlon * scale), (int)(bound.maxlon * scale))
                    + "group by wayId[comma]originalwayid";
            com.CommandText = com.CommandText.Replace(",", ".").Replace("[comma]", ",");
            //System.IO.File.WriteAllText("c:\\gislog.txt",com.CommandText);
            NpgsqlDataReader reader = com.ExecuteReader();
            List<String[]> temp = new List<string[]>();
            while (reader.Read())
            {
                temp.Add(new String[] { reader[0].ToString(), reader[1].ToString() });
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            return temp.ToArray();
        }
        /// <summary>
        /// Gets the way ids within the boundary.
        /// </summary>
        /// <param name="bound">The boundary of the search</param>
        /// <returns>Returns an array of string way ids.</returns>
        /// <seealso cref="GetWayIds"/>
        public String[] GetWayIdsInBound(Bounds bound)
        {
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select wayId from WayNodeInfoAram_cachedFromView " //"select wayId from viewWayNodeInfoAram "
                    + String.Format("where Latitude between {0} and {1} and Longitude between {2} and {3} ",
                                                    (int)(bound.minlat * scale), (int)(bound.maxlat * scale), (int)(bound.minlon * scale), (int)(bound.maxlon * scale))
                    + "group by wayId";
            com.CommandText = com.CommandText.Replace(",", ".");
            NpgsqlDataReader reader = com.ExecuteReader();
            List<String> temp = new List<string>();
            while (reader.Read())
            {
                temp.Add(reader[0].ToString());
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            con.Dispose();
            return temp.ToArray();
        }
        /// <summary>
        /// Gets all the relations ids in the database.
        /// Warning: It may be a exhaustive operation. Use more finegrained methods to retrieve the ids.
        /// </summary>
        public String[] RelationIds
        {
            get
            {
                NpgsqlConnection con = new NpgsqlConnection(connectionString);
                con.Open();
                NpgsqlCommand com = new NpgsqlCommand("", con);
                com.CommandText = "select id from Relation";
                NpgsqlDataReader reader = com.ExecuteReader();
                List<String> temp = new List<string>();
                while (reader.Read())
                {
                    temp.Add(reader[0].ToString());
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                con.Close();
                return temp.ToArray();
            }
        }
        /// <summary>
        /// Gets all the way ids in the database.
        /// Note: It may be a exhaustive operation. Use more finegrained methods to retrieve the ids.
        /// </summary>
        public String[] WayIds
        {
            get
            {
                NpgsqlConnection con = new NpgsqlConnection(connectionString);
                con.Open();
                NpgsqlCommand com = new NpgsqlCommand("", con);
                com.CommandText = "select id from Way";
                NpgsqlDataReader reader = com.ExecuteReader();
                List<String> temp = new List<string>();
                while (reader.Read())
                {
                    temp.Add(reader[0].ToString());
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                con.Close();
                return temp.ToArray();
            }
        }
        /// <summary>
        /// Gets the <see cref="NpgsqlDataReader"/> to all the ways in the database.
        /// For more details, take a look at WayNodeInfoAram_cachedFromView view in the database.
        /// </summary>
        public NpgsqlDataReader Ways
        {
            get
            {
                NpgsqlConnection con = new NpgsqlConnection(connectionString);
                con.Open();
                NpgsqlCommand com = new NpgsqlCommand("", con);
                com.CommandText = "select * from WayNodeInfoAram_cachedFromView";
                NpgsqlDataReader reader = com.ExecuteReader();
                return reader;
            }
        }
        /// <summary>
        /// Gets the <see cref="NpgsqlDataReader"/> to all the nodes in Stockholm in the database.
        /// For more details, take a look at node_stockholm table in the database.
        /// </summary>
        public NpgsqlDataReader Nodes
        {
            get
            {
                NpgsqlConnection con = new NpgsqlConnection(connectionString);
                con.Open();
                NpgsqlCommand com = new NpgsqlCommand("", con);
                com.CommandText = "select id,Latitude,Longitude from node_stockholm";
                NpgsqlDataReader reader = com.ExecuteReader();
                return reader;
            }
        }

    }
}
