using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExportMatsimFilesToDB
{
    [XmlRoot("network")]
    public class MatSimNetwork
    {
        [XmlArrayItem("node")]
        public List<MatSimNode> nodes;

        [XmlArrayItem("link")]
        public List<MatSimLink> links;

        public static MatSimNetwork Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(MatSimNetwork));
                return serializer.Deserialize(stream) as MatSimNetwork;
            }
        }

        public void Save(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(MatSimNetwork));
                serializer.Serialize(stream, this);
            }
        }

        public void ExportToPostgreSQL(string connectionString)
        {
            if (nodes == null || nodes.Count == 0)
                return;
            if (links == null || links.Count == 0)
                return;

            string nodeTable = "matsimnodes";
            string linkTable = "matsimlinks";

            string commandString = string.Format("DROP TABLE IF EXISTS {0};" +
                "CREATE TABLE {0}(id text, x numeric, y numeric, geom geometry);" +
                "DROP TABLE IF EXISTS {1};" +
                "CREATE TABLE {1}(link_id text, from_id text, to_id text, length numeric, freespeed numeric, modes text);",
                nodeTable, linkTable);

            string insertString = "";
            try
            {
                NpgsqlConnection dbConn = new NpgsqlConnection(connectionString);

                var dbCommand = new NpgsqlCommand(commandString, dbConn);

                // Creating the tables for nodes and links.
                //Console.WriteLine("Creating tables...");
                dbConn.Open();
                dbCommand.ExecuteNonQuery();
                dbConn.Close();

                // Adding nodes to the postgre DB.
                //Console.WriteLine("Adding nodes...");
                dbConn.Open();

                int counterNodes = 0;
                int totalNodes = nodes.Count;

                foreach (var n in nodes)
                {
                    Console.Write("\r{0} of {1} nodes exported", ++counterNodes, totalNodes);

                    insertString = string.Format("INSERT INTO {0} VALUES ('{1}',{2},{3},ST_Transform(ST_SetSRID(ST_MakePoint({2}, {3}), 3006), 4326));",
                        nodeTable, n.id, n.x.ToString(CultureInfo.InvariantCulture), n.y.ToString(CultureInfo.InvariantCulture));
                    byte[] bytes = Encoding.Default.GetBytes(insertString);
                    insertString = Encoding.UTF8.GetString(bytes);
                    dbCommand = new NpgsqlCommand(insertString, dbConn);
                    dbCommand.ExecuteNonQuery();
                }

                Console.WriteLine();

                // Adding links to the postgre DB.

                int counterLinks = 0;
                int totalLinks = links.Count;

                foreach (var l in links)
                {
                    Console.Write("\r{0} of {1} links exported", ++counterLinks, totalLinks);

                    insertString = string.Format("INSERT INTO {0} VALUES ('{1}','{2}','{3}',{4},{5},'{6}');",
                        linkTable,
                        l.id,
                        l.from,
                        l.to,
                        l.length.ToString(CultureInfo.InvariantCulture),
                        l.freespeed.ToString(CultureInfo.InvariantCulture),
                        l.modes);
                    byte[] bytes = Encoding.Default.GetBytes(insertString);
                    insertString = Encoding.UTF8.GetString(bytes);
                    dbCommand = new NpgsqlCommand(insertString, dbConn);
                    dbCommand.ExecuteNonQuery();
                }

                Console.WriteLine();

                dbConn.Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex + " " + insertString);
            }
        }

        public MatSimLink GetClosestLink(float x, float y)
        {
            MatSimLink closest = null;
            var min = double.MaxValue;
            var point = new double[] { x, y };
            foreach (var link in links)
            {
                if (min > LinkToPointDistance(link, point))
                    closest = link;
            }
            return closest;
        }

        double LinkToPointDistance(MatSimLink link, double[] point)
        {
            return MatSimUtils.LineToPointDistance2D(GetNode(link.from).Point, GetNode(link.to).Point, point);
        }

        /// <summary>
        /// minx, miny, maxx, maxy.
        /// </summary>
        /// <returns></returns>
        public float[] GetMinMaxXY()
        {
            return MatSimUtils.GetMinMaxXY(nodes);
        }

        public MatSimNode GetNode(string nodeId)
        {
            return nodes.Find(node => node.id == nodeId);
        }

        public string GetNodeString(string id)
        {
            return GetNode(id).ToString();
        }

        public string GetLinkString(string id)
        {
            return links.Find(link => link.id == id).ToString();

        }
    }

    public class MatSimLink
    {
        [XmlAttribute]
        public string id;

        [XmlAttribute]
        public string from;

        [XmlAttribute]
        public string to;

        [XmlAttribute]
        public float length;

        [XmlAttribute]
        public float freespeed;

        [XmlAttribute]
        public string modes;

        public override string ToString()
        {
            //return $"{id}: {from}->{to}; {modes}";
            return string.Format("{0}: {1}->{2} {3}", id, from, to, modes);
        }
    }

    public class MatSimNode
    {
        [XmlAttribute]
        public string id;

        [XmlAttribute]
        public float x;

        [XmlAttribute]
        public float y;

        public double[] Point { get { return new double[] { x, y }; } }

        public override string ToString()
        {
            //return $"{id}: {x}, {y}";
            return string.Format("{0}: {1}, {2}", id, x, y);

        }

        public double[] GetLatLon()
        {
            return MatSimUtils.GetLatLon(x, y);
        }
    }
}
