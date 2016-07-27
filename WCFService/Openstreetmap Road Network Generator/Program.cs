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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Openstreetmap_Road_Network_Generator
{
	class Program
	{
		public static readonly double scale = 10000000d;
		static string conPostGreGIS = "Server=127.0.0.1;Port=5432;Database=GIS;User Id=postgres;Password=test;";
		static string conPostGreLINKEDOSM = "Server=127.0.0.1;Port=5432;Database=LINKEDOSM;User Id=postgres;Password=test;";
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			Stopwatch recordmeter = new Stopwatch();

			Aram.OSMParser.Bounds b = new Aram.OSMParser.Bounds();
			b.minlat = 59.32973;
			b.maxlat = 59.34481;
			b.minlon = 18.07556;
			b.maxlon = 18.1062;

			if (args.Length == 1)
				conPostGreGIS = conPostGreGIS.Replace("127.0.0.1", args[0]);

			if (args.Length == 5)
			{
				conPostGreGIS = conPostGreGIS.Replace("127.0.0.1", args[0]);
				b.minlat = double.Parse(args[1]);
				b.maxlat = double.Parse(args[2]);
				b.minlon = double.Parse(args[3]);
				b.maxlon = double.Parse(args[4]);
			}

			Console.WriteLine("Preprocessing...");
			ClearWayExtData();
			PerformPreprocessingForWay_ext();
			Console.WriteLine("Preprocessing is finished.");
			//conn.Open();

			var ids = GetWayIdsInBound(b); //GetWayIdsInStockholm();
			int idcount = 0;


			int progress = 1;
			object lockTransaction = new object();
			object lockCounter = new object();
			watch.Start();
			Parallel.ForEach<String>(ids, (id) =>
			{
				//lock (lockTransaction)
				//{
				//		Console.Clear();
				//		Console.WriteLine(
				//				"Generating segments for " + id + "\n" + progress + "/" + ids.Length + "\t" + (100 * progress / (float)ids.Length).ToString("0.00") + "%");
				//}
				NpgsqlConnection conn = new NpgsqlConnection(conPostGreGIS);
				conn.Open();
				NpgsqlTransaction trans = conn.BeginTransaction();
				Interlocked.Increment(ref progress);
				//progress++;
				if (IsHighway(id, conn))
				{

					var nodes = GetWay(conn, id);
					var lastnode = 0;
					var oneway = IsOneWay(conn, id);
					for (int i = 1; i < nodes.Count; i++) // we skip the first nodes in the way.
					{
						if (CheckConnectivity(conn, nodes[i][0])) // if it is connected, start a way
						{
							int newseqforward = 0;
							int newseqreverse = 0;
							if (oneway == 1 || oneway == 0)
							{
								int localcount;
								lock (lockCounter)
								{
									localcount = idcount;
									idcount++;
								}
								for (int j = lastnode; j <= i; j++)
								{
									InsertNew_Way_ext(conn, localcount, nodes[j][0], newseqforward, id, lastnode, i);
									newseqforward++;
								}

							}

							if (oneway == -1 || oneway == 0)
							{
								int localcount;
								lock (lockCounter)
								{
									localcount = idcount;
									idcount++;
								}
								for (int j = i; j >= lastnode; j--)
								{
									InsertNew_Way_ext(conn, localcount, nodes[j][0], newseqreverse, id, i, lastnode);
									newseqreverse++;
								}
							}
							lastnode = i;

						}
					}
				}
				else
				{
					// just copy to destination
					var nodes = GetWay(conn, id);
					int localcount;
					lock (lockCounter)
					{
						localcount = idcount;
						idcount++;
					}
					for (int i = 0; i < nodes.Count; i++) // we skip the first nodes in the way.
					{
						InsertNew_Way_ext(conn, localcount, nodes[i][0], i, id, 0, nodes.Count - 1);
					}
				}
				trans.Commit();
				conn.Close();
			});
			Console.WriteLine("Postprocessing...");
			PerformPostprocessingForWay_ext();
			Console.WriteLine("Postprocessing is finished.");
			watch.Stop();
			Console.WriteLine(
					String.Format(
					"Segment generation was completed in {0} hrs and {1} mins and {2} seconds", watch.Elapsed.Hours, watch.Elapsed.Minutes, watch.Elapsed.Seconds));
			Console.ReadKey();

		}
		public static int InsertNew_Way_ext(NpgsqlConnection connection, int wayid, string nodeid, int seqid, string originalway, int original_seq_start, int original_seq_end)
		{
			// insert into way_ext values (id,nodeid,seqid,origway,seqst,seqend)
			NpgsqlCommand com = new NpgsqlCommand("", connection);
			com.CommandText =
					String.Format(
					"insert into way_ext (way_id,nodes_id,sequence_id,original_way_id,original_sequence_id_st,original_sequence_id_end) "
					+ "values ({0},{1},{2},{3},{4},{5})"
					, wayid, nodeid, seqid, originalway, original_seq_start, original_seq_end);
			var res = com.ExecuteNonQuery();
			return res;
		}
		public static int IsOneWay(NpgsqlConnection connection, string wayid)
		{
			var ret = 0;
			NpgsqlCommand com = new NpgsqlCommand("", connection);
			com.CommandText = "select direction from way_direction where way_id=" + wayid;
			NpgsqlDataReader reader = com.ExecuteReader();
			if (reader.HasRows)
			{
				reader.Read();
				switch (reader[0].ToString())
				{
					case "1":
						{
							ret = 1;
							break;
						}
					case "0":
					default:
						{
							ret = 0;
							break;
						}
					case "-1":
						{
							ret = -1;
							break;
						}
				}
			}
			else
				ret = 0;
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			return ret;
		}
		public static bool CheckConnectivity(NpgsqlConnection connection, string nodeid)
		{
			var ret = false;
			NpgsqlCommand com = new NpgsqlCommand("", connection);
			com.CommandText = String.Format("select nodeid from connection_node where nodeid='{0}'", nodeid);
			//com.CommandText = String.Format("select  wayid from waynodeinfoaram_cachedfromview"
			//    + " where wayid!='{0}' and nodeid='{1}'", excludedwayid,nodeid);
			NpgsqlDataReader reader = com.ExecuteReader();
			if (reader.HasRows)
				ret = true;
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			return ret;
		}
		public static int ClearWayExtData()
		{
			NpgsqlConnection con = new NpgsqlConnection(conPostGreGIS);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "delete from way_ext";
			var res = com.ExecuteNonQuery();
			con.Close();
			return res;
		}
		// List of String[] {nodeid,sequenceid}
		public static List<String[]> GetWay(NpgsqlConnection connection, string id)
		{
			NpgsqlCommand com = new NpgsqlCommand("", connection);
			com.CommandText = String.Format("select  nodeid,sort from waynodeinfoaram_cachedfromview"
					+ " where wayid='{0}' order by sort", id);
			NpgsqlDataReader reader = com.ExecuteReader();
			List<String[]> temp = new List<String[]>();
			while (reader.Read())
			{
				temp.Add(new String[] { reader[0].ToString(), reader[1].ToString() });
			}
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			return temp;
		}
		public static bool IsHighway(String wayId, NpgsqlConnection con)
		{
			Boolean ret = false;
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select 1 from viewwaytagstockholminfoaram where WayId='" + wayId + "' and name='highway'";
			//com.CommandText = "select WayId,Name,Info from viewWayTagInfoAram where WayId='" + wayId + "'";
			NpgsqlDataReader reader = com.ExecuteReader();
			if (reader.HasRows)
				ret = true;
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			return ret;
		}
		public static void PerformPreprocessingForWay_ext()
		{
			NpgsqlConnection con = new NpgsqlConnection(conPostGreGIS);
			con.Open();
			var text = System.IO.File.ReadAllText(Application.StartupPath + "\\" + "PreprocessingForWay_Ext.sql");
			NpgsqlCommand com = new NpgsqlCommand(text, con);
			com.CommandTimeout = 180; //Default is 20
			com.ExecuteNonQuery();
			con.Close();
		}
		public static void PerformPostprocessingForWay_ext()
		{
			NpgsqlConnection con = new NpgsqlConnection(conPostGreGIS);
			con.Open();
			var text = System.IO.File.ReadAllText(Application.StartupPath + "\\" + "PostprocessingForWay_Ext.sql");
			NpgsqlCommand com = new NpgsqlCommand(text, con);
			com.CommandTimeout = 180; //Default is 20
			com.ExecuteNonQuery();
			con.Close();
		}
		public static String[] GetWayIdsInStockholm()
		{
			NpgsqlConnection con = new NpgsqlConnection(conPostGreGIS);
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
		public static String[] GetWayIdsInBound(Aram.OSMParser.Bounds bound)
		{
			NpgsqlConnection con = new NpgsqlConnection(conPostGreGIS);
			con.Open();
			NpgsqlCommand com = new NpgsqlCommand("", con);
			com.CommandText = "select wayId from WayNodeInfoAram_cachedFromView " //"select wayId from viewWayNodeInfoAram "
				// + String.Format("where wayid in (select way_id from networkways) and Latitude between {0} and {1} and Longitude between {2} and {3} ",
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
			return temp.ToArray();
		}

		static void Main2(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			Stopwatch recordmeter = new Stopwatch();
			NpgsqlConnection conn = new NpgsqlConnection(conPostGreGIS);

			ClearWayExtData();
			conn.Open();
			var ids = GetWayIdsInStockholm();
			int idcount = 0;
			int oneway = 0;

			int progress = 0;
			int waysPertransaction = 100;
			int transactioncounter = 0;
			NpgsqlTransaction trans = null; // conn.BeginTransaction();
			float recordsPerSecond = 0;
			watch.Start();
			foreach (var id in ids)
			{
				Console.Clear();
				Console.WriteLine(
						"Generating segments for " + id + " " + progress + "/" + ids.Length + " ...." + (100 * progress / (float)ids.Length).ToString("0.00") + "%");
				Console.WriteLine(recordsPerSecond.ToString("0.00") + " ways/sec");
				if (transactioncounter == 0)
					recordmeter.Restart();

				progress++;
				var nodes = GetWay(conn, id);
				int lastnode = 0;
				oneway = IsOneWay(conn, id);
				if (transactioncounter == 0)
					trans = conn.BeginTransaction();
				for (int i = 1; i < nodes.Count; i++) // we skip the first nodes in the way.
				{
					if (CheckConnectivity(conn, nodes[i][0])) // if it is connected, start a way
					{
						// if it is start of the line
						// TODO:
						// - Create a new way
						// - Add points from last node up to this index (i).
						// - lastnode = i
						int newseqforward = 0;
						int newseqreverse = 0;
						if (oneway == 1 || oneway == 0)
							for (int j = lastnode; j <= i; j++)
							{
								InsertNew_Way_ext(conn, idcount, nodes[j][0], newseqforward, id, lastnode, i);
								newseqforward++;
							}
						if (oneway == -1 || oneway == 0)
							for (int j = i; j >= lastnode; j--) // TODO Reverse the indices
							{
								InsertNew_Way_ext(conn, idcount, nodes[j][0], newseqreverse, id, i, lastnode);
								newseqreverse++;
							}
						lastnode = i;
						idcount++;
					}
				}
				if (transactioncounter++ > waysPertransaction)
				{
					trans.Commit();
					recordmeter.Stop();
					recordsPerSecond = transactioncounter / (float)recordmeter.Elapsed.TotalSeconds;
					transactioncounter = 0;
				}


			}
			watch.Stop();
			Console.WriteLine(
					String.Format(
					"Segment generation was completed in {0} hrs and {1} mins and {2} seconds", watch.Elapsed.Hours, watch.Elapsed.Minutes, watch.Elapsed.Seconds));

			conn.Close();
		}
	}
}

