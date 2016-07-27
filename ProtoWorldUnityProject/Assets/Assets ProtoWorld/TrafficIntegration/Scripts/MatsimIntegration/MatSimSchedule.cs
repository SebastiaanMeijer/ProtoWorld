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

[XmlRoot("transitSchedule")]
public class MatSimSchedule
{
    [XmlArray("transitStops")]
    [XmlArrayItem("stopFacility")]
    public List<MatSimStop> stops;

    [XmlElement("transitLine")]
    public List<MatSimLine> lines;

    public static MatSimSchedule Load(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(MatSimSchedule));
            return serializer.Deserialize(stream) as MatSimSchedule;
        }
    }

    public void Save(string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            var serializer = new XmlSerializer(typeof(MatSimSchedule));
            serializer.Serialize(stream, this);
        }
    }


    public void ExportToPostgreSQL(string connectionString)
    {
        if (stops == null || lines == null)
            return;
        if (stops.Count == 0 || lines.Count == 0)
            return;

        string scheduleTable = "matsimSchedule";
        string routeTable = "matsimRoute";
        string departTable = "matsimDeparture";

        string commandString = string.Format("DROP TABLE IF EXISTS {0};" +
            "CREATE TABLE {0}(id integer, name text, x numeric, y numeric, geom geometry);" +
            "DROP TABLE IF EXISTS {1};" +
            "CREATE TABLE {1}(lineroute_id integer, line_id text, route_id text, leg_id text, start_id integer, end_id integer, seq_id integer);" +
            "DROP TABLE IF EXISTS {2};" +
            "CREATE TABLE {2}(lineroute_id integer, dep_id text, veh_id text, time interval);",
            scheduleTable, routeTable, departTable);

        try
        {
            NpgsqlConnection dbConn = new NpgsqlConnection(connectionString);

            var dbCommand = new NpgsqlCommand(commandString, dbConn);

            dbConn.Open();
            dbCommand.ExecuteNonQuery();
            dbConn.Close();

            dbConn.Open();
            foreach (var s in stops)
            {
                var insertString = string.Format("INSERT INTO {0} VALUES ({1},'{2}',{3},{4}, ST_Transform(ST_SetSRID(ST_MakePoint({3}, {4}), 3006), 4326));",
                    scheduleTable, s.id, s.name, s.x, s.y);
                byte[] bytes = Encoding.Default.GetBytes(insertString);
                insertString = Encoding.UTF8.GetString(bytes);
                dbCommand = new NpgsqlCommand(insertString, dbConn);
                dbCommand.ExecuteNonQuery();
            }
            var counter = 0;
            foreach (var line in lines)
            {
                var line_id = line.id;
                foreach (var route in line.routes)
                {
                    var route_id = route.id;
                    //for (int i = 0; i < route.stops.Count; i++)
                    //{
                    //    var ref_id = route.stops[i].refId;
                    //    var insertString = string.Format("INSERT INTO {0} VALUES ('{1}',{2},{3});", routeTable, route_id, ref_id, i);
                    //    dbCommand = new NpgsqlCommand(insertString, dbConn);
                    //    dbCommand.ExecuteNonQuery();
                    //}
                    for (int i = 1; i < route.stops.Count; i++)
                    {
                        var start_id = route.stops[i - 1].refId;
                        var end_id = route.stops[i].refId;
                        var leg_id = start_id + "to" + end_id;
                        var insertString = string.Format("INSERT INTO {0} VALUES ({1},'{2}','{3}','{4}',{5},{6},{7});", routeTable, counter, line_id, route_id, leg_id, start_id, end_id, (i-1));
                        dbCommand = new NpgsqlCommand(insertString, dbConn);
                        dbCommand.ExecuteNonQuery();
                    }
                    for (int i = 0; i < route.departures.Count; i++)
                    {
                        var dep = route.departures[i];
                        var insertString = string.Format("INSERT INTO {0} VALUES ({1},'{2}','{3}',justify_hours(interval '{4}'));", departTable, counter, dep.id, dep.vehicleRefId, dep.departureTime);
                        dbCommand = new NpgsqlCommand(insertString, dbConn);
                        dbCommand.ExecuteNonQuery();
                    }
                    ++counter;
                }
            }
            dbConn.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

    }

    public float[] GetMinMaxXY()
    {
        return MatSimUtils.GetMinMaxXY(stops);
    }

    public string GetStopString(string id)
    {
        return stops.Find(s => s.id == id).ToString();
    }

    public string GetLineString(string id)
    {
        return lines.Find(line => line.id == id).ToString();
    }
}

/// <summary>
/// Inherit x, y, id from MatsimNode
/// </summary> 
public class MatSimStop : MatSimNode
{
    [XmlAttribute]
    public string name;

    [XmlAttribute]
    public bool isBlocking;

    public override string ToString()
    {
        //return $"{base.ToString()}, name: {name}, block?: {isBlocking}";
        return string.Format("{0}, name: {1}, block?:{2}", base.ToString(), name, isBlocking);

    }


}
/// <summary>
/// Correspond to transitLine tag in transitSchedule
/// </summary>
public class MatSimLine
{
    [XmlAttribute]
    public string id;

    [XmlElement("transitRoute")]
    public List<MatSimRoute> routes;

    public override string ToString()
    {
        //return $"id: {id} {routes[0].stops[0]}";
        return string.Format("id: {0} {1}", id, routes[0].stops[0]);

    }
}

/// <summary>
/// Correspond to transitRoute tag in transitSchedule
/// </summary>
public class MatSimRoute
{
    [XmlAttribute]
    public string id;

    [XmlElement("transportMode")]
    public string mode;

    [XmlArray("routeProfile")]
    [XmlArrayItem("stop")]
    public List<MatSimProfileStop> stops;

    [XmlArray("departures")]
    [XmlArrayItem("departure")]
    public List<MatSimDeparture> departures;

    public override string ToString()
    {
        //return $"id: {id}, mode: {mode}";
        return string.Format("id: {0}, mode: {1}", id, mode);
    }
}

public class MatSimDeparture
{
    [XmlAttribute]
    public string id;

    [XmlAttribute]
    public string vehicleRefId;

    [XmlAttribute]
    public string departureTime;

    public override string ToString()
    {
        return string.Format("id: {0}, vehId: {1}, depT: {2}", id, vehicleRefId, departureTime);
    }
}

public class MatSimProfileStop
{
    [XmlAttribute]
    public int refId;

    [XmlAttribute]
    public string departureOffset;

    [XmlAttribute]
    public string arrivalOffset;

    [XmlAttribute]
    public bool awaitDeparture;


    public override string ToString()
    {
        //return $"refId: {refId}, dOff: {departureOffset}, aOff: {arrivalOffset}, await: {awaitDeparture}";
        return string.Format("refId: {0}, dOff: {1}, aOff: {2}, await: {3}", refId, departureOffset, arrivalOffset, awaitDeparture);
    }
}


