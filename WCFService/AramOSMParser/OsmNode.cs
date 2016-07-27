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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Npgsql;
using System.Runtime.InteropServices;

namespace Aram.OSMParser
{
	/// <summary>
	/// A class that represents an Openstreetmaps node element.
	/// </summary>
			/// <remarks>The method names with the letters "Ext" in their names refer to the generated tables that represent
		/// a road network traffic. This is different from the original way database of OSM. You may use these particular methods
		/// to retrieve data for traffic operations such as routing.</remarks>
    [StructLayout(LayoutKind.Sequential)]
	public class OsmNode
	{
		/// <summary>
		/// The scale for the latitude / longitude values over the database.
		/// <br/><strong>Note:</strong>By default all the geo position values in the database are stored as
		/// a 9 digit integer number, that should be divided by this number to get the real value.
		/// <br/><br/>For example: 584111023 becomes 584111023/ 10000000 = 58.4111023
		/// </summary>
		public readonly double scale = 10000000d;
		/// <summary>
		/// Gets or sets the OSM xml element that represents the node.
		/// </summary>
        public XElement OsmElement { get; set; }
		/// <summary>
		/// The default constructor for OsmNode
		/// </summary>
		public OsmNode() { }
		/// <summary>
		/// Initializes the classes with the given OSM xml element.
		/// </summary>
		/// <param name="node">The xml element</param>
		public OsmNode(XElement node)
		{
			this.OsmElement = node;
			this.id = node.Attribute("id").Value;
		}
		/// <summary>
		/// The OSM id of the node
		/// </summary>
		public String id;
		/// <summary>
		/// Gets or sets the order of the node.
		/// </summary>
		public int order { get; set; }
		/// <summary>
		/// The geoposition in the MSSQL database.
		/// Warning : only works when populated with GetNodesSQL() method of Way. Otherwise, use GetPositionSQL() Method.
		/// </summary>
		public GeoPosition PositionSQL { get; set; }
		/// <summary>
		/// Gets or sets the geoposition in the postgresql database. To be used with way GetNodesPostgreSQL() method.
		/// </summary>
		public GeoPosition PositionPostgreSQL { get; set; }
		/// <summary>
		/// Gets the geoposition in the postgresql database. To be used with way GetNodesPostgreSQL() method.
		/// </summary>
		/// <param name="connectionString">The connection string to the Postgresql database. Please refer to <a href="http://www.openstreetmap.org/help">OSM Help</a> for more information.</param>
		/// <returns>Returns the position of the node.</returns>
		public GeoPosition GetPositionPostgreSQL(String connectionString)
		{
			return GetPositionPostgreSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the position in the MSSQL databaes.
		/// </summary>
		/// <param name="connectionString">The connection string to the MSSQL database.</param>
		/// <returns>Returns the position of the node.</returns>
		[Obsolete("Use GetPositionPostgreSQL instead",true)]
		public GeoPosition GetPositionSQL(String connectionString)
		{
			return GetPositionSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the geoposition of a given node in the postgresql database. To be used with way GetNodesPostgreSQL() method.
		/// </summary>
		/// <param name="nodeId">The id of the node</param>
		/// <param name="connectionString">The connection string to the Postgresql database. Please refer to <a href="http://www.openstreetmap.org/help">OSM Help</a> for more information.</param>
		/// <returns>Returns the position of the node.</returns>
		public GeoPosition GetPositionPostgreSQL(String nodeId, String connectionString)
		{
			GeoPosition temp = null;

			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select id,Latitude,Longitude from node where id='" + nodeId + "'";
			NpgsqlDataReader reader = com.ExecuteReader();
			while (reader.Read())
			{
				temp = new GeoPosition((int)reader["Latitude"] / scale, (int)reader["Longitude"] / scale);
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			con.Close();
			return temp;
		}
		/// <summary>
		/// Gets the geoposition of a given node in the MSSQL database. To be used with way GetNodesPostgreSQL() method.
		/// </summary>
		/// <param name="nodeId">The id of the node</param>
		/// <param name="connectionString">The connection string to the MSSQL database.</param>
		/// <returns>Returns the position of the node.</returns>
		[Obsolete("Use GetPositionPostgreSQL instead", true)]
		public GeoPosition GetPositionSQL(String nodeId, String connectionString)
		{
			GeoPosition temp = null;

			SqlConnection con = new SqlConnection(connectionString);
			con.Open();
			SqlCommand com = new SqlCommand("", con);
			com.CommandText = "select id,Latitude,Longitude from tnode where id='" + nodeId + "'";
			SqlDataReader reader = com.ExecuteReader();
			while (reader.Read())
			{
				temp = new GeoPosition((double)reader["Latitude"], (double)reader["Longitude"]);
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			con.Close();
			return temp;
		}

		/// <summary>
		/// Gets all the tags for this node.
		/// </summary>
		/// <param name="connectionString">The connection string to the Postgresql database. Please refer to <a href="http://www.openstreetmap.org/help">OSM Help</a> for more information.</param>
		/// <returns></returns>
		public List<Tag> GetTagsPostgreSQL(String connectionString)
		{
			return GetTagsPostgreSQL(id, connectionString);
		}
		/// <summary>
		/// Gets a list of OSM <see cref="Tag"/>s for the given node id from the database.
		/// </summary>
		/// <param name="nodeId">The id of the node to look for</param>
		/// <param name="connectionString">The connection string to the Postgresql database. Please refer to <a href="http://www.openstreetmap.org/help">OSM Help</a> for more information.</param>
		/// <returns>Returns a list of <see cref="Tag"/>s.</returns>
		public List<Tag> GetTagsPostgreSQL(String nodeId, String connectionString)
		{
			List<Tag> temp = new List<Tag>();

			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select NodeId,\"name\",Info from viewNodeTagInfoAram where NodeId='" + nodeId + "'";
			NpgsqlDataReader reader = com.ExecuteReader();
			while (reader.Read())
			{
				Tag tempTag = new Tag();
				tempTag.KeyValueSQL = new string[] { reader["name"].ToString(), reader["Info"].ToString() };
				temp.Add(tempTag);
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			con.Close();
			return temp;
		}

		/// <summary>
		/// Gets a list of OSM <see cref="Tag"/>s for the this node  from the database.
		/// </summary>
		/// <param name="connectionString">The connection string to the MSSQL database. </param>
		/// <returns>Returns a list of <see cref="Tag"/>s.</returns>
		[Obsolete("Use GetTagsPostgreSQL instead", true)]
		public List<Tag> GetTagsSQL(String connectionString)
		{
			return GetTagsSQL(id, connectionString);
		}
		/// <summary>
		/// Gets a list of OSM <see cref="Tag"/>s for the given node id from the database.
		/// </summary>
		/// <param name="nodeId">The node id to look for</param>
		/// <param name="connectionString">The connection string to the Postgresql database. Please refer to <a href="http://www.openstreetmap.org/help">OSM Help</a> for more information.</param>
		/// <returns>Returns a list of <see cref="Tag"/>s.</returns>
		[Obsolete("Use GetTagsPostgreSQL instead",true)]
		public List<Tag> GetTagsSQL(String nodeId, String connectionString)
		{
			List<Tag> temp = new List<Tag>();

			SqlConnection con = new SqlConnection(connectionString);
			con.Open();
			SqlCommand com = new SqlCommand("", con);
			com.CommandText = "select NodeId,Name,Info from NodeTagInfoAram where NodeId='" + nodeId + "'";
			SqlDataReader reader = com.ExecuteReader();
			while (reader.Read())
			{
				Tag tempTag = new Tag();
				tempTag.KeyValueSQL = new string[] { reader["Name"].ToString(), reader["Info"].ToString() };
				temp.Add(tempTag);
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			con.Close();
			return temp;
		}
		/// <summary>
		/// Gets the <see cref="GeoPosition"/> of the current node from OSM xml element.
		/// <strong>Note:</strong> This retrieves the data from the xml elements. Do not use this if you are getting the data from the database.
		/// </summary>
		public GeoPosition Position
		{
			get
			{
				return new GeoPosition(OsmElement.Attribute("lat").Value, OsmElement.Attribute("lon").Value);
			}
		}
		/// <summary>
		/// Gets the list of <see cref="Tag"/>s of the current node from OSM xml element.
		/// <strong>Note:</strong> This retrieves the data from the xml elements. Do not use this if you are getting the data from the database.
		/// </summary>
		public List<Tag> Tags
		{
			get
			{
				var WayTags = from w in OsmElement.Elements("tag")
											select new Tag(w);
				return WayTags.ToList();
			}
		}
	}
}
