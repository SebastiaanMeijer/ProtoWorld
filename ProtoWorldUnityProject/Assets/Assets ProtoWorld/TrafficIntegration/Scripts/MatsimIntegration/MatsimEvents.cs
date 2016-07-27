/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using Npgsql;

[XmlType(AnonymousType = true)]
[XmlRoot("events")]
public class MatsimEvents
{
    [XmlElement("event")]
    public List<MatsimEvent> matsimEvents;

    //public static MatsimEvents Load(string path)
    //{
    //    using (var stream = new FileStream(path, FileMode.Open))
    //    {
    //        var serializer = new XmlSerializer(typeof(MatsimEvents));
    //        return serializer.Deserialize(stream) as MatsimEvents;
    //    }
    //}

    public void ExportToPostgreSQL(string connectionString)
    {
        if (matsimEvents == null || matsimEvents.Count == 0)
            return;

        string eventTable = "events";
        
        string commandString = string.Format("DROP TABLE IF EXISTS {0};" +
            "CREATE TABLE {0}(event_time real, type text, pid text, link_id text, veh_id text, line_id text, route_id text, dep_id text, fac_id text, delay real, misc text);",
            eventTable);

        try
        {
            NpgsqlConnection dbConn = new NpgsqlConnection(connectionString);

            var dbCommand = new NpgsqlCommand(commandString, dbConn);

            dbConn.Open();
            dbCommand.ExecuteNonQuery();
            dbConn.Close();

            dbConn.Open();
            foreach (var ev in matsimEvents)
            {
                var insertString = string.Format("INSERT INTO {0} {1};", eventTable, ev.SQLValueString());
                //Debug.Log(ev.ToString());
                //Debug.Log(ev.SQLValueString());
                //Debug.Log(insertString);
                dbCommand = new NpgsqlCommand(insertString, dbConn);
                dbCommand.ExecuteNonQuery();
            }
            dbConn.Close();

        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public List<MatsimEvent> FindEvents(string vehicle)
    {
        return matsimEvents.Where(e => e.GetVehicleString().Equals(vehicle)).ToList();
    }

    void AddEvent(MatsimEvent ev)
    {
        if (matsimEvents != null)
            matsimEvents.Add(ev);
    }

    public static MatsimEvents ReadXml(string path)
    {
        if (!File.Exists(path))
            return null;

        using (XmlReader reader = new XmlTextReader(path))
        {
            var events = new MatsimEvents() { matsimEvents = new List<MatsimEvent>() };
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals("event"))
                        {
                            float time;
                            if (!float.TryParse(reader.GetAttribute("time"), out time))
                                continue;

                            switch (reader.GetAttribute("type"))
                            {
                                case "TransitDriverStarts":
                                    events.AddEvent(new MatsimDriverStarts(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("driverId"),
                                        reader.GetAttribute("vehicleId"),
                                        reader.GetAttribute("transitLineId"),
                                        reader.GetAttribute("transitRouteId"),
                                        reader.GetAttribute("departureId"))
                                        );
                                    break;
                                case "departure":
                                    events.AddEvent(new MatsimDeparture(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("legMode"))
                                        );
                                    break;
                                case "arrival":
                                    events.AddEvent(new MatsimArrival(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("legMode"))
                                        );
                                    break;
                                case "PersonEntersVehicle":
                                    events.AddEvent(new MatsimEntersVehicle(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("vehicle"))
                                        );
                                    break;
                                case "PersonLeavesVehicle":
                                    events.AddEvent(new MatsimLeavesVehicle(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("vehicle"))
                                        );
                                    break;
                                case "vehicle leaves traffic":
                                    events.AddEvent(new MatsimLeavesTraffic(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("vehicle"),
                                        reader.GetAttribute("networkMode"),
                                        float.Parse(reader.GetAttribute("relativePosition")))
                                        );
                                    break;
                                case "wait2link":
                                    events.AddEvent(new MatsimWait2Link(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("vehicle"),
                                        reader.GetAttribute("networkMode"),
                                        float.Parse(reader.GetAttribute("relativePosition")))
                                        );
                                    break;
                                case "entered link":
                                    events.AddEvent(new MatsimEnterLink(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("vehicle"))
                                        );
                                    break;
                                case "left link":
                                    events.AddEvent(new MatsimLeftLink(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("vehicle"))
                                        );
                                    break;
                                case "waitingForPt":
                                    events.AddEvent(new MatsimWaitingForPt(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("agent"),
                                        reader.GetAttribute("atStop"),
                                        reader.GetAttribute("destinationStop"))
                                        );
                                    break;
                                case "actstart":
                                    events.AddEvent(new MatsimActStart(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("actType"))
                                        );
                                    break;
                                case "actend":
                                    events.AddEvent(new MatsimActStart(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        reader.GetAttribute("link"),
                                        reader.GetAttribute("actType"))
                                        );
                                    break;
                                case "VehicleArrivesAtFacility":
                                    events.AddEvent(new MatsimVehicleArrivesAtFacility(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("vehicle"),
                                        reader.GetAttribute("facility"),
                                        float.Parse(reader.GetAttribute("delay")))
                                        );
                                    break;
                                case "VehicleDepartsAtFacility":
                                    events.AddEvent(new MatsimVehicleDepartsAtFacility(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("vehicle"),
                                        reader.GetAttribute("facility"),
                                        float.Parse(reader.GetAttribute("delay")))
                                        );
                                    break;
                                case "travelled":
                                    events.AddEvent(new MatsimTravelled(time,
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("person"),
                                        double.Parse(reader.GetAttribute("distance")))
                                        );
                                    break;
                                case "stuckAndAbort":
                                    var linkStr = reader.GetAttribute("link");
                                    var legStr = reader.GetAttribute("legMode");
                                    var perStr = reader.GetAttribute("person");
                                    if (!string.IsNullOrEmpty(perStr))
                                    {
                                        if (!string.IsNullOrEmpty(linkStr) && !string.IsNullOrEmpty(legStr))
                                        {
                                            events.AddEvent(new MatsimStuck(time,
                                                reader.GetAttribute("type"),
                                                linkStr,
                                                legStr,
                                                perStr));
                                        }
                                        else
                                        {
                                            events.AddEvent(new MatsimStuck(time,
                                                reader.GetAttribute("type"),
                                                perStr));
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        Debug.Log("Events loaded.");
                        break;

                }

            }
            return events;
        }
    }

    public void Save(string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            var serializer = new XmlSerializer(typeof(MatsimEvents));
            serializer.Serialize(stream, this);
        }
    }

    public void CreateTestEvents()
    {
        matsimEvents = new List<MatsimEvent>();
        matsimEvents.Add(new MatsimEvent(0, "basic"));
        matsimEvents.Add(new MatsimDriverStarts(0, "TransitDriverStarts", "did", "vid", "tid", "trid", "dpid"));
        matsimEvents.Add(new MatsimDeparture(0, "departure", "p", "l", "lm"));
    }

    public void DebugLog()
    {
        foreach (var e in matsimEvents)
        {
            Debug.Log(e.ToString());
        }
    }
}

[XmlInclude(typeof(MatsimDriverStarts))]
[XmlInclude(typeof(MatsimDeparture))]
[XmlInclude(typeof(MatsimArrival))]
[XmlInclude(typeof(MatsimEnterLink))]
[XmlInclude(typeof(MatsimEntersVehicle))]
[XmlInclude(typeof(MatsimLeavesTraffic))]
[XmlInclude(typeof(MatsimLeavesVehicle))]
[XmlInclude(typeof(MatsimLeftLink))]
[XmlInclude(typeof(MatsimActEnd))]
[XmlInclude(typeof(MatsimActStart))]
[XmlInclude(typeof(MatsimWait2Link))]
[XmlInclude(typeof(MatsimVehicleArrivesAtFacility))]
[XmlInclude(typeof(MatsimVehicleDepartsAtFacility))]
[XmlInclude(typeof(MatsimWaitingForPt))]
[XmlInclude(typeof(MatsimTravelled))]
[XmlInclude(typeof(MatsimStuck))]
public class MatsimEvent
{
    [XmlAttribute]
    public float time;

    [XmlAttribute]
    public string type;

    public MatsimEvent()
    {

    }

    public MatsimEvent(float evTime, string evType)
    {
        time = evTime;
        type = evType;
    }

    public virtual string SQLValueString()
    {
        return "";
    }

    public override string ToString()
    {
        return time + " " + type;
    }

    public virtual string GetVehicleString()
    {
        return "";
    }
}
[XmlRoot("TransitDriverStarts")]
public class MatsimDriverStarts : MatsimEvent
{
    [XmlAttribute]
    public string driverId;

    [XmlAttribute]
    public string vehicleId;

    [XmlAttribute]
    public string transitLineId;

    [XmlAttribute]
    public string transitRouteId;

    [XmlAttribute]
    public string departureId;

    public MatsimDriverStarts()
    {

    }

    public MatsimDriverStarts(float evTime, string evType, string did, string vid, string tid, string trid, string dpid) : base(evTime, evType)
    {
        driverId = did;
        vehicleId = vid;
        transitLineId = tid;
        transitRouteId = trid;
        departureId = dpid;
    }
    public override string GetVehicleString()
    {
        return vehicleId;
    }

    public override string SQLValueString()
    {
        return base.SQLValueString() + string.Format("(event_time, type, pid, veh_id, line_id, route_id, dep_id)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}','{5}','{6}')", 
            time, type, driverId, vehicleId, transitLineId, transitRouteId, departureId);
    }

    public override string ToString()
    {
        return base.ToString() + " " + driverId + " " + vehicleId + " " + transitLineId + " " + transitRouteId + " " + departureId;
    }
}

[XmlRoot("departure")]
public class MatsimDeparture : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string legMode;

    public MatsimDeparture()
    {

    }

    public MatsimDeparture(float evTime, string evType, string p, string l, string lm) : base(evTime, evType)
    {
        person = p;
        link = l;
        legMode = lm;
    }
    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, misc)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, legMode);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + legMode;
    }
}


[XmlRoot("arrival")]
public class MatsimArrival : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string legMode;

    public MatsimArrival()
    {

    }

    public MatsimArrival(float evTime, string evType, string p, string l, string lm) : base(evTime, evType)
    {
        person = p;
        link = l;
        legMode = lm;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, misc)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, legMode);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + legMode;
    }
}

[XmlRoot("PersonLeavesVehicle")]
public class MatsimLeavesVehicle : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string vehicle;


    public MatsimLeavesVehicle()
    {

    }

    public MatsimLeavesVehicle(float evTime, string evType, string p, string v) : base(evTime, evType)
    {
        person = p;
        vehicle = v;
    }

    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, veh_id)" +
            " VALUES ({0},'{1}','{2}','{3}')",
            time, type, person, vehicle);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + vehicle;
    }
}

[XmlRoot("PersonEntersVehicle")]
public class MatsimEntersVehicle : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string vehicle;


    public MatsimEntersVehicle()
    {

    }

    public MatsimEntersVehicle(float evTime, string evType, string p, string v) : base(evTime, evType)
    {
        person = p;
        vehicle = v;
    }

    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, veh_id)" +
            " VALUES ({0},'{1}','{2}','{3}')",
            time, type, person, vehicle);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + vehicle;
    }
}

[XmlRoot("actend")]
public class MatsimActEnd : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string actType;

    public MatsimActEnd()
    {

    }

    public MatsimActEnd(float evTime, string evType, string p, string l, string at) : base(evTime, evType)
    {
        person = p;
        link = l;
        actType = at;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, misc)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, actType);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + actType;
    }
}

[XmlRoot("actstart")]
public class MatsimActStart : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string actType;

    public MatsimActStart()
    {

    }

    public MatsimActStart(float evTime, string evType, string p, string l, string at) : base(evTime, evType)
    {
        person = p;
        link = l;
        actType = at;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, misc)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, actType);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + actType;
    }
}

[XmlRoot("travelled")]
public class MatsimTravelled : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public double distance;

    public MatsimTravelled()
    {

    }

    public MatsimTravelled(float evTime, string evType, string p, double dist) : base(evTime, evType)
    {
        person = p;
        distance = dist;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, misc)" +
            " VALUES ({0},'{1}','{2}','{3}')",
            time, type, person, distance.ToString());
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + distance;
    }
}

[XmlRoot("entered link")]
public class MatsimEnterLink : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string vehicle;

    public MatsimEnterLink()
    {

    }

    public MatsimEnterLink(float evTime, string evType, string p, string l, string v) : base(evTime, evType)
    {
        person = p;
        link = l;
        vehicle = v;
    }

    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, veh_id)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, vehicle);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + vehicle;
    }
}

[XmlRoot("left link")]
public class MatsimLeftLink : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string vehicle;

    public MatsimLeftLink()
    {

    }

    public MatsimLeftLink(float evTime, string evType, string p, string l, string v) : base(evTime, evType)
    {
        person = p;
        link = l;
        vehicle = v;
    }

    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, veh_id)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, vehicle);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + vehicle;
    }
}

[XmlRoot("wait2link")]
public class MatsimWait2Link : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string vehicle;

    [XmlAttribute]
    public string networkMode;

    [XmlAttribute]
    public float relativePosition;

    public MatsimWait2Link()
    {

    }

    public MatsimWait2Link(float evTime, string evType, string p, string l, string v, string nm, float rp) : base(evTime, evType)
    {
        person = p;
        link = l;
        vehicle = v;
        networkMode = nm;
        relativePosition = rp;
    }

    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, veh_id)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, vehicle);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + vehicle + " " + networkMode + " " + relativePosition;
    }
}

[XmlRoot("vehicle leaves traffic")]
public class MatsimLeavesTraffic : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string vehicle;

    [XmlAttribute]
    public string networkMode;

    [XmlAttribute]
    public float relativePosition;

    public MatsimLeavesTraffic()
    {

    }

    public MatsimLeavesTraffic(float evTime, string evType, string p, string l, string v, string nm, float rp) : base(evTime, evType)
    {
        person = p;
        link = l;
        vehicle = v;
        networkMode = nm;
        relativePosition = rp;
    }
    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, veh_id)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, vehicle);
    }

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + vehicle + " " + networkMode + " " + relativePosition;
    }
}

[XmlRoot("VehicleArrivesAtFacility")]
public class MatsimVehicleArrivesAtFacility : MatsimEvent
{
    [XmlAttribute]
    public string vehicle;

    [XmlAttribute]
    public string facility;

    [XmlAttribute]
    public float delay;

    public MatsimVehicleArrivesAtFacility()
    {

    }

    public MatsimVehicleArrivesAtFacility(float evTime, string evType, string veh, string fac, float del) : base(evTime, evType)
    {
        vehicle = veh;
        facility = fac;
        delay = del;
    }
    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, veh_id, fac_id, delay)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, vehicle, facility, delay);
    }

    public override string ToString()
    {
        return base.ToString() + " " + vehicle + " " + facility + " " + delay;
    }
}

[XmlRoot("VehicleDepartsAtFacility")]
public class MatsimVehicleDepartsAtFacility : MatsimEvent
{
    [XmlAttribute]
    public string vehicle;

    [XmlAttribute]
    public string facility;

    [XmlAttribute]
    public float delay;

    public MatsimVehicleDepartsAtFacility()
    {

    }

    public MatsimVehicleDepartsAtFacility(float evTime, string evType, string veh, string fac, float del) : base(evTime, evType)
    {
        vehicle = veh;
        facility = fac;
        delay = del;
    }
    public override string GetVehicleString()
    {
        return vehicle;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, veh_id, fac_id, delay)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, vehicle, facility, delay);
    }

    public override string ToString()
    {
        return base.ToString() + " " + vehicle + " " + facility + " " + delay;
    }
}

[XmlRoot("waitingForPt")]
public class MatsimWaitingForPt : MatsimEvent
{
    [XmlAttribute]
    public string agent;

    [XmlAttribute]
    public string atStop;

    [XmlAttribute]
    public string destinationStop;

    public MatsimWaitingForPt()
    {

    }

    public MatsimWaitingForPt(float evTime, string evType, string ag, string at, string dest) : base(evTime, evType)
    {
        agent = ag;
        atStop = at;
        destinationStop = dest;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, fac_id, misc)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, agent, atStop, destinationStop);
    }

    public override string ToString()
    {
        return base.ToString() + " " + agent + " " + atStop + " " + destinationStop;
    }
}

[XmlRoot("stuckAndAbort")]
public class MatsimStuck : MatsimEvent
{
    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string legMode;

    [XmlAttribute]
    public string person;

    public MatsimStuck()
    {

    }

    public MatsimStuck(float evTime, string evType, string p) : base(evTime, evType)
    {
        person = p;
    }

    public MatsimStuck(float evTime, string evType, string l, string leg, string p) : base(evTime, evType)
    {
        link = l;
        legMode = leg;
        person = p;
    }

    public override string SQLValueString()
    {
        return string.Format("(event_time, type, pid, link_id, misc)" +
            " VALUES ({0},'{1}','{2}','{3}','{4}')",
            time, type, person, link, legMode);
    }

    public override string ToString()
    {
        return base.ToString() + " " + (string.IsNullOrEmpty(link) ? "" : link + " ") + (string.IsNullOrEmpty(legMode) ? "" : legMode + " ") + person;
    }
}