/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Npgsql;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

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
            Debug.Log("Creating tables...");
            dbConn.Open();
            dbCommand.ExecuteNonQuery();
            dbConn.Close();

            // Adding nodes to the postgre DB.
            Debug.Log("Adding nodes...");
            dbConn.Open();

            foreach (var n in nodes)
            {
                insertString = string.Format("INSERT INTO {0} VALUES ('{1}',{2},{3},ST_Transform(ST_SetSRID(ST_MakePoint({2}, {3}), 3006), 4326));",
                    nodeTable, n.id, n.x, n.y);
                byte[] bytes = Encoding.Default.GetBytes(insertString);
                insertString = Encoding.UTF8.GetString(bytes);
                dbCommand = new NpgsqlCommand(insertString, dbConn);
                dbCommand.ExecuteNonQuery();
            }

            // Adding links to the postgre DB.
            foreach (var l in links)
            {
                insertString = string.Format("INSERT INTO {0} VALUES ('{1}','{2}','{3}',{4},{5},'{6}');",
                    linkTable,
                    l.id,
                    l.from,
                    l.to,
                    l.length,
                    l.freespeed,
                    l.modes);
                byte[] bytes = Encoding.Default.GetBytes(insertString);
                insertString = Encoding.UTF8.GetString(bytes);
                dbCommand = new NpgsqlCommand(insertString, dbConn);
                dbCommand.ExecuteNonQuery();
            }
            dbConn.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex + " " + insertString);
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
