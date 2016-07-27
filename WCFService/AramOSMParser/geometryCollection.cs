/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// TODO: 
// UpdateGeometryCollectionOnDatabase (
// DeleteGeometryCollectionFromDatabase (bool deleteConnectedLargeObject)
namespace Aram.OSMParser
{


	/// <summary>
	/// The class responsible for reading and writing geometry data from/to the database. This class uses LargeObjects that is available in PostgreSQL.
	/// <br/>You may modify this class to match your own database if other than PostgreSQL.
	/// </summary>
	/// <remarks>
	/// This geometry_collection table in the database, stores the meta data regarding the large objects for the geometrical data.
	/// Note that the binary content of those objects are stored in largeobject table while only a reference to those objects is stored in this table.
	/// An example for such a record holds 3D geometry of buildings and roads in OBJ standard format.
	/// </remarks>
	/// <example>
	/// <code>
	/// var geometryRetrieval = geometryCollection.GetSingleObjectWithId("8", true, connPostGreSql);
	///	// testing GeometryCollection
	///	Aram.OSMParser.geometryCollection col = new Aram.OSMParser.geometryCollection();
	///
	/// // col.gisId = 
	/// col.gisType = "dummy";
	/// col.format = "txt";
	/// col.largeObject = null;
	/// col.lastUpdate = DateTime.Now;
	/// col.latitude = 563213212;
	/// col.longitude = 171231231;
	/// col.name = "Test2";
	///  col.pivot = new Aram.OSMParser.Vector3() { x = 1f, y = 2f, z = 3f };
	/// col.version = new Aram.OSMParser.GaPSlabsVersion() { versionTitle = "development", major = 0, minor = 1 };
	///
	/// col.AddGeometryCollectionToDatabase(connPostGreSql, false);
	/// var bytes = File.ReadAllBytes(@"C:\Users\admgaming\Documents\Visual Studio 2012\Projects\GaPSLabs\AramOSMParser\OsmParserTestApplication\bin\Debug\Npgsql.xml");
	/// col.largeObject = bytes;
	/// col.UpdateThisGeometryOnDatabase(connPostGreSql, true);
	/// var resultBytes = geometryCollection.GetLargeObject(col.largeObjectReference, connPostGreSql);
	/// File.WriteAllBytes("c:\\dummy", resultBytes);</code>
	/// </example>
	public class geometryCollection
	{
		/// <summary>
		/// The constructor for the geometry collection.
		/// </summary>
		public geometryCollection()
		{

		}
		/// <summary>
        /// Gets or sets the id of the geometry collection. Note that this is automatically set after using AddGeometryCollectionToDatabase() method.
		/// </summary>
		public long id { get; set; }
		/// <summary>
		/// Gets or sets the name of the geometry collection.
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Gets or sets the version of the geometry collection.
		/// </summary>
		public GaPSlabsVersion version { get; set; }
		/// <summary>
		/// Gets or set the format of the geometry collection.
		/// </summary>
		/// <example>
		///	<code>geometryCollection gem = new geometryCollection();
		///	gem.format = "collada";</code>
		/// </example>
		public string format { get; set; }

		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		/// <remarks>
		/// This is the latitude * 10 to the power 7.
		/// To get the float, divide this number by 10000000.
		/// That way, you'll get two digit of integers and 7 digits of fractional parts.
		/// For example if the latitude is 584118588 , then the real value will be 584118588 divided by 10^7 = 58.4118588
		/// </remarks>
		public int latitude { get; set; }
		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		/// <remarks>
		/// This is the longitude * 10 to the power 7.
		/// To get the float, divide this number by 10000000.
		/// That way, you'll get two digit of integers and 7 digits of fractional parts.
		/// For example if the longitude is 155880942 , then the real value will be 155880942 divided by 10^7 = 15.5880942
		/// </remarks>
		public int longitude { get; set; }
		/// <summary>
		/// Gets or sets the pivot point of a 3d geometry.
		/// </summary>
		/// <remarks>
		/// This is the pivot point of the 3d geometry (if the format is a 3d geometry).
		/// It is where the object model should be loaded at 3d coordinates.
		/// </remarks>
		public Vector3GaPS pivot { get; set; }

		/// <summary>
		/// Gets or sets the gis id of the object.
		/// </summary>
		/// <remarks>
		/// If this object is referencing any gis element (ie, way, node etc in the openStreetmap),
		/// it can be stored here.
		/// </remarks>
		public int gisId { get; set; }

		/// <summary>
		/// Gets or sets the gis element type, ie. way, node etc.
		/// </summary>
		public string gisType { get; set; }

		/// <summary>
		/// Gets or sets the Utc datetime.
		/// </summary>
		public DateTime lastUpdate { get; set; }

		/// <summary>
		/// Gets or sets the content of the actual object as an array of bytes.
		/// </summary>
		public byte[] largeObject { get; set; }
		/// <summary>
		/// Gets the reference id for the large object.
		/// </summary>
		public int largeObjectReference { get { return large_object_reference; } }
		private int large_object_reference;

		/// <summary>
		/// Adds a new record to the geometry_collection table in the database from the current geometryCollection object.
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="IncludeLargeObjectData">If true, it will store the largeObject with the actual data.</param>
		/// <remarks>In case you want to add the meta information about an object and populate the actual data later,
		/// you may set <paramref name="IncludeLargeObjectData"/> to false.</remarks>
		public void AddGeometryCollectionToDatabase(string connectionString, bool IncludeLargeObjectData)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);

			/*
			 INSERT INTO geometry_collection(
            id, name, version_title, major, minor, format, large_object_reference, 
            latitude, longitude, pivotx, pivoty, pivotz, gis_id, gis_type, 
            last_update)
				VALUES (?, ?, ?, ?, ?, ?, ?, 
								?, ?, ?, ?, ?, ?, ?, 
								?);
			*/
			int LOid = -1;
            if (IncludeLargeObjectData)
                if (largeObject != null)
                    LOid = CreateLargeObject(largeObject, connectionString);
			com.CommandText =
				"INSERT INTO geometry_collection(" +
				"name, version_title, major, minor, format, large_object_reference, " +
				"latitude, longitude, pivotx, pivoty, pivotz, gis_id, gis_type, " +
				"last_update) " +
				"VALUES ('{0}', '{1}', {2}, {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, '{11}', '{12}', '{13}') " +
				"RETURNING id";
			com.CommandText = string.Format(com.CommandText,
				name, version.versionTitle, version.major, version.minor, format, LOid == -1 ? "null" : LOid.ToString(),
				latitude, longitude, pivot.x.ToString().Replace(",", "."), pivot.y.ToString().Replace(",", "."), pivot.z.ToString().Replace(",", "."), gisId, gisType,
				NpgsqlTypes.NpgsqlTimeStamp.Now);
			var result = com.ExecuteScalar();
			if (result != null)
				id = int.Parse(result.ToString());

			con.Close();

		}

		/// <summary>
		/// Deletes a geometry_collection record from the database. You can choose whether to delete the large object.
		/// WHERE 
		/// id=? AND name=? AND version_title=? AND major=? AND minor=? AND format=? AND large_object_reference=? AND 
		/// latitude=? AND longitude=? AND pivotx=? AND pivoty=? AND pivotz=? AND gis_id=? AND 
		/// gis_type=? AND last_update=?
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="deleteLargeObject">If true, it will delete the actual object from the database.</param>
		public void DeleteThisGeometryFromDatabase(string connectionString, bool deleteLargeObject)
		{

			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			if (deleteLargeObject)
				DeleteLargeObject(large_object_reference, connectionString);

			com.CommandText =
				"DELETE FROM geometry_collection " +
				"WHERE id=" + id;

			int result = com.ExecuteNonQuery();

			con.Close();
		}

		/// <summary>
		/// Deletes a geometry_collection record from the database. You can choose whether to delete the large object.
		/// WHERE 
		/// id=? AND name=? AND version_title=? AND major=? AND minor=? AND format=? AND large_object_reference=? AND 
		/// latitude=? AND longitude=? AND pivotx=? AND pivoty=? AND pivotz=? AND gis_id=? AND 
		/// gis_type=? AND last_update=?
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="searchCriteria">This is the where clause of the query without the 'WHERE' keyword.</param>
		/// <param name="deleteLargeObject">If true, it will delete the actual objects from the database.</param>
		public static void DeleteGeometryCollectionFromDatabase(string connectionString, string searchCriteria, bool deleteLargeObject)
		{
			if (!string.IsNullOrEmpty(searchCriteria))
			{
				NpgsqlConnection con = new NpgsqlConnection(connectionString);
				con.Open();
				NpgsqlCommand com = new NpgsqlCommand("", con);

				if (deleteLargeObject)
				{
					com.CommandText = "SELECT large_object_reference FROM geometry_collection"
						+ "WHERE " + searchCriteria;
					List<int> oidList = new List<int>();
					NpgsqlDataReader reader = com.ExecuteReader();
					if (reader.HasRows)
					{
						while (reader.Read())
						{
                            if (!reader.IsDBNull(0))
                            {
                                var current = reader.GetInt64(0);
                                oidList.Add((int)current);
                            }
						}
					}
					if (!reader.IsClosed)
						reader.Close();
					reader.Dispose();

					for (int i = 0; i < oidList.Count; i++)
						DeleteLargeObject(oidList[i], connectionString);
				}

				com.CommandText =
					"DELETE FROM geometry_collection " +
					"WHERE " + searchCriteria;

				int result = com.ExecuteNonQuery();

				con.Close();
			}
		}

		/// <summary>
		/// Updates a geometry_collection record on the database.
		/// UPDATE geometry_collection
		/// SET id=?, name=?, version_title=?, major=?, minor=?, format=?, large_object_reference=?, 
		/// latitude=?, longitude=?, pivotx=?, pivoty=?, pivotz=?, gis_id=?, 
		/// gis_type=?, last_update=?
		/// WHERE 
		/// id=? AND name=? AND version_title=? AND major=? AND minor=? AND format=? AND large_object_reference=? AND 
		/// latitude=? AND longitude=? AND pivotx=? AND pivoty=? AND pivotz=? AND gis_id=? AND 
		/// gis_type=? AND last_update=?
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="searchCriteria">This is the where clause of the query without the 'WHERE' keyword.</param>
		/// <param name="updateInstructions">This is the set clause of the query without the 'SET' keyword.</param>
		public static void UpdateGeometryCollectionOnDatabase(string connectionString, string searchCriteria, string updateInstructions)
		{
			if (!string.IsNullOrEmpty(searchCriteria) && !string.IsNullOrEmpty(updateInstructions))
			{
				NpgsqlConnection con = new NpgsqlConnection(connectionString);
				con.Open();
				NpgsqlCommand com = new NpgsqlCommand("", con);

				com.CommandText =
					"UPDATE geometry_collection " +
					"SET " + updateInstructions +
					" WHERE " + searchCriteria;

				int result = com.ExecuteNonQuery();

				con.Close();
			}
		}

		/// <summary>
		/// Updates this geometry_collection record on the database.
		/// UPDATE geometry_collection
		/// SET id=?, name=?, version_title=?, major=?, minor=?, format=?, large_object_reference=?, 
		/// latitude=?, longitude=?, pivotx=?, pivoty=?, pivotz=?, gis_id=?, 
		/// gis_type=?, last_update=?
		/// WHERE 
		/// id= The current object id
		/// </summary>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <param name="updateLargeObject">If true, it will also update the actual object on the database.</param>
		public void UpdateThisGeometryOnDatabase(string connectionString, bool updateLargeObject)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);

			string updateInstructions =
				"name='{0}', version_title='{1}', major={2}, minor={3}, format='{4}', " +
				"latitude={5}, longitude={6}, pivotx={7}, pivoty={8}, pivotz={9}, gis_id={10}, " +
				"gis_type='{11}', last_update='{12}'";
			if (updateLargeObject && large_object_reference == 0)
			{
				var LOid = CreateLargeObject(largeObject, connectionString);
				large_object_reference = LOid;
				updateInstructions += ", large_object_reference=" + large_object_reference;
			}
			else UpdateLargeObject(large_object_reference, largeObject, connectionString);
			updateInstructions = string.Format(updateInstructions,
				name, version.versionTitle, version.major, version.minor, format, latitude, longitude,
				pivot.x, pivot.y, pivot.z, gisId, gisType, lastUpdate.ToPostgreTimeStamp());

			com.CommandText =
				"UPDATE geometry_collection " +
				"SET " + updateInstructions +
				" WHERE id=" + id;
			int result = com.ExecuteNonQuery();



			con.Close();
		}
		/// <summary>
		/// Gets the geometry object with the matching id
		/// </summary>
		/// <param name="id">The id of the object in the geometry_collection table.</param>
		/// <param name="includeLargeObject">If true, it also gets the actual object data from the database.</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns the geometry.</returns>
		public static geometryCollection GetSingleObjectWithId(string id, bool includeLargeObject, string connectionString)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select id,name,version_title,major,minor,format,large_object_Reference,latitude,longitude,pivotx,pivoty,pivotz,gis_id,gis_type,last_update from geometry_collection where id=" + "'" + id + "'";

			NpgsqlDataReader reader = com.ExecuteReader();
			//List<String> temp = new List<string>();
			geometryCollection singleObject = new geometryCollection();
			while (reader.Read())
			{
				//temp.Add(reader[0].ToString());
				singleObject.id = reader.GetInt64(0);
				singleObject.name = reader.GetString(1);
				singleObject.version = new GaPSlabsVersion() { versionTitle = reader.GetString(2), major = reader.GetInt32(3), minor = reader.GetInt32(4) };
				singleObject.format = reader.GetString(5);
				if (includeLargeObject && reader.GetInt64(6) != 0)
					singleObject.largeObject = GetLargeObject((int)reader.GetInt64(6), connectionString);
				else
					singleObject.largeObject = null;
				singleObject.large_object_reference = (int)reader.GetInt64(6);
				singleObject.latitude = reader.GetInt32(7);
				singleObject.longitude = reader.GetInt32(8);
				singleObject.pivot = new Vector3GaPS() { x = (float)reader.GetDecimal(9), y = (float)reader.GetDecimal(10), z = (float)reader.GetDecimal(11) };
				singleObject.gisId = reader.GetInt32(12);
				singleObject.gisType = reader.GetString(13);
				singleObject.lastUpdate = reader.GetTimeStamp(14).ToDateTime();

				break;
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			con.Close();
			return singleObject;
		}
		/// <summary>
		/// Gets an array of geometryCollection objects matching the given criteria.
		/// </summary>
		/// <param name="includeLargeObject">If true, it also gets the actual object data from the database.</param>
		/// <param name="whereClause">The matching criteria for the where clause of the database query.
		/// Please refer to geometry_collection table in the database for the definitions.</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns an array of geometry collection.</returns>
		/// <seealso cref="GetObjectsWithIdAndCriteriaMinimal"/>
		public static geometryCollection[] GetObjectsWithIdAndCriteria(bool includeLargeObject, string whereClause, string connectionString)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select id,name,versionTitle,major,minor,format,largeobjectReference,latitude,longitude,pivotX,pivotY,pivotZ,gisId,gistype,lastUpdate from geometryCollection";
			if (!string.IsNullOrEmpty(whereClause))
				com.CommandText += " where " + whereClause;
			//com.CommandText = com.CommandText.Replace(",", ".");
			NpgsqlDataReader reader = com.ExecuteReader();
			List<geometryCollection> objectcollection = new List<geometryCollection>();
			while (reader.Read())
			{
				geometryCollection singleObject = new geometryCollection();
				singleObject.id = reader.GetInt64(0);
				singleObject.name = reader.GetString(1);
				singleObject.version = new GaPSlabsVersion() { versionTitle = reader.GetString(2), major = reader.GetInt32(3), minor = reader.GetInt32(4) };
				singleObject.format = reader.GetString(5);
				if (includeLargeObject && reader.GetInt32(6) != 0)
					singleObject.largeObject = GetLargeObject((int)reader.GetInt64(6), connectionString);
				else
					singleObject.largeObject = null;
				singleObject.large_object_reference = (int)reader.GetInt64(6);
				singleObject.latitude = reader.GetInt32(7);
				singleObject.longitude = reader.GetInt32(8);
				singleObject.pivot = new Vector3GaPS() { x = (float)reader.GetDecimal(9), y = (float)reader.GetDecimal(10), z = (float)reader.GetDecimal(11) };
				singleObject.gisId = reader.GetInt32(12);
				singleObject.gisType = reader.GetString(13);
				singleObject.lastUpdate = reader.GetTimeStamp(14).ToDateTime();

				objectcollection.Add(singleObject);
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			con.Close();
			return objectcollection.ToArray();
		}
		/// <summary>
		/// Gets an array of ids belonging to the geometry objects that match the given criteria.
		/// </summary>
		/// <param name="whereClause">The matching criteria for the where clause of the database query.
		/// Please refer to geometry_collection table in the database for the definitions.</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns an array of geometry ids.</returns>
		/// <seealso cref="GetObjectsWithIdAndCriteria"/>
		public static long[] GetObjectsWithIdAndCriteriaMinimal(string whereClause, string connectionString)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select id from geometryCollection";
			if (!string.IsNullOrEmpty(whereClause))
				com.CommandText += " where " + whereClause;
			//com.CommandText = com.CommandText.Replace(",", ".");
			NpgsqlDataReader reader = com.ExecuteReader();
			List<long> objectcollection = new List<long>();
			while (reader.Read())
			{
				objectcollection.Add(reader.GetInt64(0));
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			con.Close();
			return objectcollection.ToArray();
		}
		/// <summary>
		/// Gets the large object content from the database for the given large object id.
		/// </summary>
		/// <param name="largeObjectId">The id of the largeobject.</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns the content of the large object.</returns>
		/// <seealso cref="CreateLargeObject"/>
		/// <seealso cref="DeleteLargeObject"/>
		/// <seealso cref="UpdateLargeObject"/>
		public static byte[] GetLargeObject(int largeObjectId, string connectionString)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlTypes.LargeObjectManager lm = new NpgsqlTypes.LargeObjectManager(con);
			var TransRead = con.BeginTransaction();
			var readlo = lm.Open(largeObjectId, LargeObjectManager.READWRITE);
			var resultLo = readlo.Read(readlo.Size());
			TransRead.Commit();
			con.Close();
			return resultLo;
		}
		/// <summary>
		/// Creates a large object with the given data at the database and returns the id of that object.
		/// </summary>
		/// <param name="data">The data for the large object.</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <returns>Returns the id of the large object at the database.</returns>
		/// <seealso cref="GetLargeObject"/>
		/// <seealso cref="DeleteLargeObject"/>
		/// <seealso cref="UpdateLargeObject"/>
		public static int CreateLargeObject(byte[] data, string connectionString)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlTypes.LargeObjectManager lm = new NpgsqlTypes.LargeObjectManager(con);

			var generatedLO = lm.Create(NpgsqlTypes.LargeObjectManager.READWRITE);
			var TransWrite = con.BeginTransaction();
			LargeObject lo = lm.Open(generatedLO, LargeObjectManager.READWRITE);
			lo.Write(data);
			lo.Close();
			TransWrite.Commit();
			con.Close();
			return generatedLO;
		}
		/// <summary>
		/// Deletes a large object matching the id from the database.
		/// </summary>
		/// <param name="largeObjectId">The id of the largeobject.</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <seealso cref="CreateLargeObject"/>
		/// <seealso cref="CreateLargeObject"/>
		/// <seealso cref="UpdateLargeObject"/>
		public static void DeleteLargeObject(int largeObjectId, string connectionString)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlTypes.LargeObjectManager lm = new NpgsqlTypes.LargeObjectManager(con);

			var TransRead = con.BeginTransaction();
			lm.Delete(largeObjectId);
			TransRead.Commit();
			con.Close();
		}
		/// <summary>
		/// Updates a large object on the database with matching the id.
		/// </summary>
		/// <param name="largeObjectId">The id of the largeobject.</param>
		/// <param name="data">The updated data</param>
		/// <param name="connectionString">The connectionString to the database. Please refer to .Net Data Provider for Postgresql for format specifications</param>
		/// <seealso cref="CreateLargeObject"/>
		/// <seealso cref="DeleteLargeObject"/>
		/// <seealso cref="GetLargeObject"/>
		public static void UpdateLargeObject(int largeObjectId, byte[] data, string connectionString)
		{
			NpgsqlConnection con = new NpgsqlConnection(connectionString);
			con.Open();
			NpgsqlTypes.LargeObjectManager lm = new NpgsqlTypes.LargeObjectManager(con);

			var generatedLO = lm.Create(NpgsqlTypes.LargeObjectManager.READWRITE);
			var TransWrite = con.BeginTransaction();
			var readlo = lm.Open(largeObjectId, LargeObjectManager.READWRITE);
			readlo.Write(data);
			readlo.Close();
			TransWrite.Commit();
			con.Close();
		}
	}

	/// <summary>
	/// A simple versioning class for the <see cref="geometryCollection"/> objects.
	/// </summary>
	public class GaPSlabsVersion
	{
        /// <summary>
        /// Default constructor for GaPSlabsVersion
        /// </summary>
        public GaPSlabsVersion() { }
        /// <summary>
        /// Constructor for GaPSlabsVersion
        /// </summary>
        /// <param name="versionTitle">Sets the versoinTitle property.</param>
        /// <param name="major">Sets the major property</param>
        /// <param name="minor">Sets the minor property</param>
        public GaPSlabsVersion(string versionTitle,int major,int minor)
        {
            this.versionTitle = versionTitle;
            this.major = major;
            this.minor = minor;
        }
		/// <summary>
		/// Gets or sets the version title.
		/// </summary>
		public string versionTitle { get; set; }
		/// <summary>
		/// Gets or sets the major revision number.
		/// </summary>
		public int major { get; set; }
		/// <summary>
		/// Gets or sets the minor revision number.
		/// </summary>
		public int minor { get; set; }
	}

	/// <summary>
	/// A datetime class with an offset.
	/// </summary>
	public class DateTimeWithOffset
	{
		/// <summary>
		/// Gets or sets the datatime.
		/// </summary>
		public DateTime dateTime { get; set; }
		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		public DateTimeOffset offset { get; set; }
	}
	/// <summary>
	/// A datetime converter class between .NET datetime and postgresql datetime formats.
	/// </summary>
	public static class GaPSlabsTimeStamp
	{
		/// <summary>
		/// Converts .NET <see cref="DateTime"/> to <see cref="NpgsqlTypes.NpgsqlTimeStamp"/> format.
		/// </summary>
		/// <param name="datetime">The .NET datetime to convert from.</param>
		/// <returns>Returns the datetime in <see cref="NpgsqlTypes.NpgsqlTimeStamp"/> format.</returns>
		public static NpgsqlTypes.NpgsqlTimeStamp ToPostgreTimeStamp(this DateTime datetime)
		{
			NpgsqlTypes.NpgsqlTimeStamp t =
				new NpgsqlTypes.NpgsqlTimeStamp(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
			return t;
		}
		/// <summary>
		/// Converts <see cref="NpgsqlTypes.NpgsqlTimeStamp"/> to .NET <see cref="DateTime"/> format.
		/// </summary>
		/// <param name="dt">The datetime to convert from.</param>
		/// <returns>Returns the datetime in <see cref="DateTime"/> format.</returns>
		public static DateTime ToDateTime(this NpgsqlTypes.NpgsqlTimeStamp dt)
		{
			DateTime dtRet = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hours, dt.Minutes, dt.Seconds);
			return dtRet;
		}
	}

	/// <summary>
	/// A 3d coordinate vector.
	/// </summary>
	/// <remarks>Sometimes the orientation of the vector components may be understood differently, ie. 
	/// z component may point upwards in one system, and downwards in another.
	/// It is the programmer's responsibility to make the correct assignments.</remarks>
	public class Vector3GaPS
	{
        /// <summary>
        /// Default constructor for Vector3GaPS
        /// </summary>
        public Vector3GaPS() { }
        /// <summary>
        /// Constructor for Vector3GaPS
        /// </summary>
        /// <param name="x">x value</param>
        /// <param name="y">y value</param>
        /// <param name="z">z value</param>
        public Vector3GaPS(float x,float y,float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
		/// <summary>
		/// value on x axis
		/// </summary>
		public float x;
		/// <summary>
		/// value on y axis
		/// </summary>
		public float y;
		/// <summary>
		/// value on z axis
		/// </summary>
		public float z;
	}


}
