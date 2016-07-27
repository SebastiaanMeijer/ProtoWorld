/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
		[Obsolete("Use OSMPostgresqlSource instead")]
    public class OSMSqlSource
    {
        public String connectionString;
        public SqlConnection conn;
        public OSMSqlSource(String connectionString)
        {
            this.connectionString = connectionString;
            conn = new SqlConnection(connectionString);
        }
        ~OSMSqlSource()
        {
            conn.Close();
        }
        public Bounds Bounds
        {
            get
            {
                Bounds b = new OSMParser.Bounds();
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand com = new SqlCommand("", con);
                com.CommandText = "select * from viewbounds";
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    b.minlat = (double)reader["minLat"];
                    b.maxlat = (double)reader["maxLat"];
                    b.minlon = (double)reader["minLon"];
                    b.maxlon = (double)reader["maxLon"];
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                con.Close();
                return b;
            }
        }
        public String[] GetWayIds(String SearchQuery)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand com = new SqlCommand("", con);
            com.CommandText = "select id from tWay where (" + SearchQuery + ")";
            SqlDataReader reader = com.ExecuteReader();
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
        public String[] GetWayIdsInBound(Bounds bound)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand com = new SqlCommand("", con);
            com.CommandText = "select wayId from WayNodeInfoAram "
                + String.Format("where Latitude between {0} and {1} and Longitude between {2} and {3} ",
                                bound.minlat, bound.maxlat, bound.minlon, bound.maxlon)
                + "group by wayId";
            com.CommandText = com.CommandText.Replace(",", ".");
            SqlDataReader reader = com.ExecuteReader();
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
        public String[] RelationIds
        {
            get
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand com = new SqlCommand("", con);
                com.CommandText = "select id from tRelation";
                SqlDataReader reader = com.ExecuteReader();
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
        public String[] WayIds
        {
            get
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand com = new SqlCommand("", con);
                com.CommandText = "select id from tWay";
                SqlDataReader reader = com.ExecuteReader();
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

        public SqlDataReader Ways
        {
            get
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand com = new SqlCommand("", con);
                com.CommandText = "select * from WayNodeInfoAram";
                SqlDataReader reader = com.ExecuteReader();
                return reader;
            }
        }
        public SqlDataReader Nodes
        {
            get
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand com = new SqlCommand("", con);
                com.CommandText = "select id,Latitude,Longitude from tnode";
                SqlDataReader reader = com.ExecuteReader();
                return reader;
            }
        }

    }
}
