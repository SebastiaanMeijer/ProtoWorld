/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Aram.OSMParser
{
	/// <summary>
	/// The <see cref="Way"/> class is a parser for OpenStreetMap standard xml data, as well as the GaPSLabs interface for
	/// the specific data structure that is stored in the PostgreSQL DBMS.
	/// </summary>
	/// <remarks>The method names with the letters "Ext" in their names refer to the generated tables that represent
	/// a road network traffic. This is different from the original way database of OSM. You may use these particular methods
	/// to retrieve data for traffic operations such as routing.</remarks>
	[Serializable]
	public class Way
	{
		/// <summary>
		/// Gets or sets the OSM xml element that represents the way.
		/// </summary>
		public XElement OsmElement { get; set; }
		/// <summary>
		/// Gets or sets the root of the xml stream that contains OSM elements.
		/// </summary>
		public XDocument Root { get; set; }
		/// <summary>
		/// The scale for the latitude / longitude values over the database.
		/// <br/><strong>Note:</strong>By default all the geo position values in the database are stored as
		/// a 9 digit integer number, that should be divided by this number to get the real value.
		/// <br/><br/>For example: 584111023 becomes 584111023/ 10000000 = 58.4111023
		/// </summary>
		public readonly double scale = 10000000d;
		/// <summary>
		/// The default constructor for Way
		/// </summary>
		public Way() { }
		/// <summary>
		/// The constructor that sets the OSM <see cref="id"/> of the way.
		/// </summary>
		/// <param name="id">OSM id of way</param>
		public Way(String id)
		{
			this.id = id;
		}
		/// <summary>
		/// The constructor that sets the <see cref="OsmElement"/> of the way.
		/// </summary>
		/// <param name="way">The OSM xml element for the way</param>
		public Way(XElement way)
		{
			this.id = way.Attribute("id").Value;
			OsmElement = way;
		}
		/// <summary>
		/// The constructor that sets the <see cref="OsmElement"/> and <see cref="Root"/> of the way.
		/// </summary>
		/// <param name="way">The OSM xml element for the way</param>
		/// <param name="Root">The root of the xml stream that contains OSM elements</param>
		public Way(XElement way, XDocument Root)
		{
			this.id = way.Attribute("id").Value;
			OsmElement = way;
			this.Root = Root;
		}
		/// <summary>
		/// Gets or sets the OSM id of the way.
		/// </summary>
		public String id { get; set; }
		// public String User {get;set;}
		/// <summary>
		/// Gets the list of <see cref="OsmNode"/> elements for this way from the original OSM data on the database.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for MSSQL for format specifications</param>
		/// <returns>Returns a list of <see cref="OsmNode"/>s.</returns>
		public List<OsmNode> GetNodesPostgreSQL(String connectionString)
		{
			return GetNodesPostgreSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the list of <see cref="OsmNode"/> for this way from the wayextnodeinfoaram_cachedfromview view in the database.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns a list of <see cref="OsmNode"/>s.</returns>
		public List<OsmNode> GetNodesFromExtPostgreSQL(String connectionString)
		{
			return GetNodesFromExtPostgreSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the list of <see cref="OsmNode"/> for this way from the wayextnodeinfoaram_cachedfromview view in the PostgreSql database.
		/// </summary>
		/// <param name="wayId">The way id to look for</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns a list of <see cref="OsmNode"/>s.</returns>
		public List<OsmNode> GetNodesFromExtPostgreSQL(String wayId, String connectionString)
		{
			Way temp = new Way();
			List<OsmNode> NodesSQLtemp = new List<OsmNode>();
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select wayId,nodeId,Latitude,Longitude,sort,originalwayid from wayextnodeinfoaram_cachedfromview where wayId='" + wayId + "' order by sort";
			// com.CommandText = "select wayId,nodeId,Latitude,Longitude,sort from viewWayNodeInfoAram where wayId='" + wayId + "' order by sort";
			NpgsqlDataReader reader = com.ExecuteReader();
			if (reader.HasRows)
			{
				while (reader.Read())
				{
					OsmNode tempNode = new OsmNode();
					tempNode.id = reader["nodeId"].ToString();
					tempNode.PositionSQL = new GeoPosition((int)reader["Latitude"] / scale, (int)reader["Longitude"] / scale);
					tempNode.order = int.Parse(reader["sort"].ToString(), CultureInfo.InvariantCulture);
					NodesSQLtemp.Add(tempNode);
				}
				if (!reader.IsClosed)
					reader.Close();
				con.Close();
				return NodesSQLtemp;
			}
			else
			{
				if (!reader.IsClosed)
					reader.Close();
				con.Close();
				return null;
			}
		}
		/// <summary>
		/// Gets the list of <see cref="OsmNode"/> elements for the given way id from the original OSM data on the database.
		/// </summary>
		/// <param name="wayId">The way id to look for</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for PostgreSql for format specifications</param>
		/// <returns>Returns a list of <see cref="OsmNode"/>s that is ordered node sequence.</returns>
		public List<OsmNode> GetNodesPostgreSQL(String wayId, String connectionString)
		{
			Way temp = new Way();
			List<OsmNode> NodesSQLtemp = new List<OsmNode>();
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select wayId,nodeId,Latitude,Longitude,sort from waynodeinfoaram_cachedfromview where wayId='" + wayId + "' order by sort";
			// com.CommandText = "select wayId,nodeId,Latitude,Longitude,sort from viewWayNodeInfoAram where wayId='" + wayId + "' order by sort";
			NpgsqlDataReader reader = com.ExecuteReader();
			if (reader.HasRows)
			{
				while (reader.Read())
				{
					OsmNode tempNode = new OsmNode();
					tempNode.id = reader["nodeId"].ToString();
					tempNode.PositionSQL = new GeoPosition((int)reader["Latitude"] / scale, (int)reader["Longitude"] / scale);
					tempNode.order = int.Parse(reader["sort"].ToString(), CultureInfo.InvariantCulture);
					NodesSQLtemp.Add(tempNode);
				}
				if (!reader.IsClosed)
					reader.Close();
				con.Close();
				return NodesSQLtemp;
			}
			else
			{
				if (!reader.IsClosed)
					reader.Close();
				con.Close();
				return null;
			}
		}
        ///<summary>
        /// Gets the list of <see cref="OsmNode"/> elements for the given way id from the original OSM data on the database.
        /// <strong>Note:</strong>The connection will not be closed after the process is done.
        /// </summary>
        /// <param name="wayId">The way id to look for</param>
        /// <param name="con"> The connection to the database. Please refer to .Net Data Provider for PostgreSql for format specifications</param>
        /// <returns>Returns a list of <see cref="OsmNode"/>s that is ordered node sequence.</returns>
        public List<OsmNode> GetNodesPostgreSQL(String wayId, NpgsqlConnection con)
        {
            Way temp = new Way();
            List<OsmNode> NodesSQLtemp = new List<OsmNode>();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select wayId,nodeId,Latitude,Longitude,sort from waynodeinfoaram_cachedfromview where wayId='" + wayId + "' order by sort";
            // com.CommandText = "select wayId,nodeId,Latitude,Longitude,sort from viewWayNodeInfoAram where wayId='" + wayId + "' order by sort";
            NpgsqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    OsmNode tempNode = new OsmNode();
                    tempNode.id = reader["nodeId"].ToString();
                    tempNode.PositionSQL = new GeoPosition((int)reader["Latitude"] / scale, (int)reader["Longitude"] / scale);
                    tempNode.order = int.Parse(reader["sort"].ToString(), CultureInfo.InvariantCulture);
                    NodesSQLtemp.Add(tempNode);
                }
                if (!reader.IsClosed)
                    reader.Close();
                return NodesSQLtemp;
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();
                return null;
            }
        }
		/// <summary>
		/// Gets the list of <see cref="OsmNode"/> elements for this way from the original OSM data on the database.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for MSSQL for format specifications</param>
		/// <returns>Returns a list of <see cref="OsmNode"/>s.</returns>
		[Obsolete("Use GetNodesPostgreSQL instead.",true)]
		public List<OsmNode> GetNodesSQL(String connectionString)
		{
			return GetNodesSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the list of <see cref="OsmNode"/> elements for the given way id from the original OSM data on the database.
		/// </summary>
		/// <param name="wayId">The way id to look for</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns a list of <see cref="OsmNode"/>s.</returns>
		[Obsolete("Use GetNodesPostgreSQL instead.", true)]
		public List<OsmNode> GetNodesSQL(String wayId, String connectionString)
		{
			Way temp = new Way();
			List<OsmNode> NodesSQLtemp = new List<OsmNode>();
			SqlConnection con = new SqlConnection(connectionString);
			con.Open();
			SqlCommand com = new SqlCommand("", con);
			com.CommandText = "select wayId,nodeId,Latitude,Longitude,sort from WayNodeInfoAram where wayId='" + wayId + "' order by sort";
			SqlDataReader reader = com.ExecuteReader();
			if (reader.HasRows)
			{
				while (reader.Read())
				{
					OsmNode tempNode = new OsmNode();
					tempNode.id = reader["nodeId"].ToString();
					tempNode.PositionSQL = new GeoPosition((double)reader["Latitude"], (double)reader["Longitude"]);
					tempNode.order = int.Parse(reader["sort"].ToString(), CultureInfo.InvariantCulture);
					NodesSQLtemp.Add(tempNode);
				}
				if (!reader.IsClosed)
					reader.Close();
				con.Close();
				return NodesSQLtemp;
			}
			else
			{
				if (!reader.IsClosed)
					reader.Close();
				con.Close();
				return null;
			}
		}
		/// <summary>
		/// Gets a list of <see cref="OsmNode"/> elements for this way.
		/// <strong>Note:</strong> This retrieves the data from the xml elements. Do not use this if you are getting the data from the database.
		/// </summary>
		public List<OsmNode> Nodes
		{
			get
			{
				if (Root == null)
					throw new NullReferenceException("The Root propery of way is null.");
				var allnodes = Root.Descendants("osm").Select(i => i.Elements("node")).First().ToList();
				var nodePositions = from s in allnodes
														from w in NodesRaw
														where s.Attribute("id").Value == w
														select new OsmNode(s);
				return nodePositions.ToList();
			}
		}
		/// <summary>
		/// Gets a raw string of OSM xml nodes that belong to this way.
		/// <strong>Note:</strong> This retrieves the data from the xml elements. Do not use this if you are getting the data from the database.
		/// </summary>
		public IEnumerable<String> NodesRaw
		{
			get
			{
				var WayNodes = from w in OsmElement.Elements("nd")
											 select w.Attribute("ref").Value;
				return WayNodes;
			}
		}
		/// <summary>
		/// Gets the list of OSM <see cref="Tag"/>s for this way from the original OSM database.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		public List<Tag> GetTagsPostgreSQL(String connectionString)
		{
			return GetTagsPostgreSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the list of OSM <see cref="Tag"/>s for the given original OSM way id from the database.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="OriginalWayId">The original OSM way id</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		public List<Tag> GetTagsExtPostgreSQL(String connectionString, String OriginalWayId)
		{
			return GetTagsPostgreSQL(OriginalWayId, connectionString);
		}
		/// <summary>
		/// Gets the list of OSM <see cref="Tag"/>s for the given original OSM way id from the database for the area of Stockholm.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="OriginalWayId">The original OSM way id</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		public List<Tag> GetTagsExtStockholmPostgreSQL(String connectionString, String OriginalWayId)
		{
			return GetTagsStockholmPostgreSQL(OriginalWayId, connectionString);
		}
		/// <summary>
		/// Gets the list of OSM <see cref="Tag"/>s for this way (should be an original way of the OSM) from the database for the area of Stockholm.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		public List<Tag> GetTagsStockholmPostgreSQL(String connectionString)
		{
			return GetTagsStockholmPostgreSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the list of OSM <see cref="Tag"/>s for the given original OSM way id from the database for the area of Stockholm.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="wayId">The original OSM way id</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		public List<Tag> GetTagsStockholmPostgreSQL(String wayId, String connectionString)
		{
			List<Tag> temp = new List<Tag>();

			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			//com.CommandText = "select WayId,Name,Info from viewwaytagstockholminfoaram where WayId='" + wayId + "'";
			// NOTE: The buildings with inner holes are only listed as buildings within Relation table.
			com.CommandText = "select WayId,Name,Info from viewwaytagstockholminfoaram where WayId='" + wayId + "'"
											+ " union "
											+ "select WayId,Name,Info from waytaginrelationinfoaram where WayId='" + wayId + "'";
			//com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'";
			NpgsqlDataReader reader = com.ExecuteReader();
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
		/// Gets the list of OSM <see cref="Tag"/>s for the given original OSM way id from the database for the area of Stockholm.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="wayId">The original OSM way id</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		public List<Tag> GetTagsPostgreSQL(String wayId, String connectionString)
		{
			List<Tag> temp = new List<Tag>();

			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			//com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'";
			// NOTE: The buildings with inner holes are only listed as buildings within Relation table.
			com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'"
											+ " union "
											+ "select WayId,Name,Info from waytaginrelationinfoaram where WayId='" + wayId + "'";
			//com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'";
			NpgsqlDataReader reader = com.ExecuteReader();
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
        /// Gets the list of OSM <see cref="Tag"/>s for the given original OSM way id from the database for the area of Stockholm.
        /// <strong>Note:</strong>The connection will not be closed after the process is done.
        /// </summary>
        /// <param name="con"> The connection to the database. Please refer to .Net Data Provider for PostgreSql for format specifications</param>
        /// <param name="wayId">The original OSM way id</param>
        /// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
        public List<Tag> GetTagsPostgreSQL(String wayId, NpgsqlConnection con)
        {
            List<Tag> temp = new List<Tag>();

            NpgsqlCommand com = new NpgsqlCommand("", con);
            //com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'";
            // NOTE: The buildings with inner holes are only listed as buildings within Relation table.
            com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'"
                                            + " union "
                                            + "select WayId,Name,Info from waytaginrelationinfoaram where WayId='" + wayId + "'";
            //com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'";
            NpgsqlDataReader reader = com.ExecuteReader();
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
            return temp;
        }
		/// <summary>
		/// Gets the list of OSM <see cref="Tag"/>s for this way from the database.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		[Obsolete("Use GetTagsPostgreSQL instead.",true)]
		public List<Tag> GetTagsSQL(String connectionString)
		{
			return GetTagsSQL(id, connectionString);
		}
		/// <summary>
		/// Gets the list of OSM <see cref="Tag"/>s for the given way id from the database.
		/// </summary>
		/// <param name="wayId"></param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns a list of OSM <see cref="Tag"/>s.</returns>
		[Obsolete("Use GetTagsPostgreSQL instead.", true)]
		public List<Tag> GetTagsSQL(String wayId, String connectionString)
		{
			List<Tag> temp = new List<Tag>();

			SqlConnection con = new SqlConnection(connectionString);
			con.Open();
			SqlCommand com = new SqlCommand("", con);
			com.CommandText = "select WayId,Name,Info from WayTagInfoAram where WayId='" + wayId + "'";
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
		/// Gets the list of <see cref="Tag"/>s from the OSM xml element.
		/// </summary>
		/// <strong>Note:</strong> This retrieves the data from the xml elements. Do not use this if you are getting the data from the database.
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
