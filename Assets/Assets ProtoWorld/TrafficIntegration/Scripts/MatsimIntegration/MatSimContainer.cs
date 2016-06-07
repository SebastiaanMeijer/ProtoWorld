using UnityEngine;
using MightyLittleGeodesy.Positions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[ExecuteInEditMode]
public class MatSimContainer : MonoBehaviour
{
    Dictionary<string, Vector3> nodes = new Dictionary<string, Vector3>();

    public Dictionary<string, MatSimLink> carLinks = new Dictionary<string, MatSimLink>();

    public Dictionary<string, MatSimLink> ptLinks = new Dictionary<string, MatSimLink>();

    MatSimNetwork matSimNetwork;

    MatSimSchedule matSimSchedule;

    MatsimEvents matSimEvents;

    void Awake()
    {
        matSimNetwork = null;
        matSimSchedule = null;
    }

    public void LoadEvents(string path)
    {
        var eventsPath = Path.Combine(path, "500.events.xml");
        matSimEvents = MatsimEvents.Load(eventsPath);
        Debug.Log("events loaded!");

        Debug.Log(matSimEvents.lines[0]);
        Debug.Log(matSimEvents.lines[1]);

    }

    public void LoadSchedule(string path)
    {
        var schedulePath = Path.Combine(path, "transitSchedule.xml");
        matSimSchedule = MatSimSchedule.Load(schedulePath);
        Debug.Log("schedule loaded!");

    }

    public void ExportScheduleToPostgre()
    {
        //if (true)
        //{
        //    var stop = matSimSchedule.stops[0];
        //    Debug.Log(string.Format("({0},'{1}',{2},{3})", stop.id, stop.name, stop.x, stop.y));
        //    return;
        //}

        if (matSimSchedule == null)
            return;
        matSimSchedule.ExportToPostgreSQL("gis_stockholm_lan");

        Debug.Log("Export done!");
    }

    public void Load(string path)
    {
        var networkPath = Path.Combine(path, "network-plain.xml");
        var schedulePath = Path.Combine(path, "transitSchedule.xml");
        var vehiclePath = Path.Combine(path, "vehicles.xml");

        Debug.Log("Reading matsim network...");
        matSimNetwork = MatSimNetwork.Load(networkPath);

        Debug.Log("no. of links: " + carLinks.Count);

        nodes = matSimNetwork.nodes.ToDictionary(n => n.id, n => new Vector3(n.x, 0, n.y));

        foreach (var link in matSimNetwork.links)
        {
            switch (link.modes)
            {
                case "car":
                    carLinks.Add(link.id, link);
                    break;
                case "pt":
                    ptLinks.Add(link.id, link);
                    break;
                default:
                    break;
            }
        }

        MatSimSchedule matSimSchedule = null;
        //matSimSchedule = MatSimSchedule.Load(schedulePath);

        //MatSimsVehicles vehicleDefinition;
        //vehicleDefinition = MatSimsVehicles.Load(vehiclePath);

        if (matSimNetwork != null)
            return;

        var networkMinMax = matSimNetwork.GetMinMaxXY();

        SWEREF99Position minSwe = new SWEREF99Position(networkMinMax[1], networkMinMax[0]);
        SWEREF99Position maxSwe = new SWEREF99Position(networkMinMax[3], networkMinMax[2]);

        WGS84Position netMinWgs = minSwe.ToWGS84();
        WGS84Position netMaxWgs = maxSwe.ToWGS84();
        Debug.Log("Network min max:");
        Debug.Log("Min Lat: " + netMinWgs.Latitude);
        Debug.Log("Min Lon: " + netMinWgs.Longitude);
        Debug.Log("Max Lat: " + netMaxWgs.Latitude);
        Debug.Log("Max Lon: " + netMaxWgs.Longitude);

        if (matSimSchedule == null)
            return;

        var scheduleMinMax = matSimSchedule.GetMinMaxXY();

        minSwe = new SWEREF99Position(scheduleMinMax[1], scheduleMinMax[0]);
        maxSwe = new SWEREF99Position(scheduleMinMax[3], scheduleMinMax[2]);

        WGS84Position schMinWgs = minSwe.ToWGS84();
        WGS84Position schMaxWgs = maxSwe.ToWGS84();
        Debug.Log("Schedule min max:");
        Debug.Log("Min Lat: " + schMinWgs.Latitude);
        Debug.Log("Min Lon: " + schMinWgs.Longitude);
        Debug.Log("Max Lat: " + schMaxWgs.Latitude);
        Debug.Log("Max Lon: " + schMaxWgs.Longitude);

        //minLat = Math.Min(netMinWgs.Latitude, schMinWgs.Latitude);
        //minLon = Math.Min(netMinWgs.Longitude, schMinWgs.Longitude);
        //maxLat = Math.Max(netMaxWgs.Latitude, schMaxWgs.Latitude);
        //maxLon = Math.Max(netMaxWgs.Longitude, schMaxWgs.Longitude);

        //Console.WriteLine(matSimSchedule.GetStopString("99999"));
        //Console.WriteLine(matSimSchedule.GetLineString("line1_r"));
        //Console.WriteLine(matSimSchedule.GetLineString("line10_r"));

        //Console.WriteLine(GetLinkString("100004_AB"));
        //Console.WriteLine(GetNodeString("tr_65905"));

        //Console.WriteLine(vehicleDefinition.GetVehicleTypeString("BUS"));
        //Console.WriteLine(vehicleDefinition.GetVehicleString("Veh144931"));
        //Console.WriteLine($"vehicle list count: {vehicleDefinition.vehicles.Count}");
    }

    public Vector3 GetNodePositionInUnity(string nodeId)
    {
        return new Vector3(nodes[nodeId].x, 0, nodes[nodeId].y);
    }

    public void BuildNetwork()
    {
        //var parameters = FindObjectOfType<MatSimParameters>();
        //if (parameters != null)
        //{
        //    int counter = 0;
        //    float total = carLinks.Count;
        //    foreach (var link in carLinks.Values)
        //    {
        //        var go = CreateLinkObject(link.from, link.to, parameters.scale, parameters.roadWidthInMeter);
        //    }
        //}
    }
}

