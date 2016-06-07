using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System;

public class SumoContainer : MonoBehaviour
{
    public AdditionalContainer busInfo;
    public NetContainer netInfo;

    public void CreateBusStops()
    {
        if (busInfo == null)
        {
            Debug.Log("no busInfo loaded.");
            return;
        }
        if (netInfo == null)
        {
            Debug.Log("no netInfo loaded.");
            return;
        }
        var mb = FindObjectOfType<MapBoundaries>();
        if (mb == null)
        {
            Debug.Log("no MapBoundaries found.");
            return;
        }

        //ArrowUtils.CheckTransportationModuleExist();
        StationCreator creator = StationCreator.AttachToGameObject();
        creator.SetStations(FindObjectsOfType<StationController>());

        // SumoLocation used in UTM to WGS-84 conversion.
        var location = netInfo.location;
        // Go through the busStops in busInfo.
        foreach (var busStop in busInfo.busStops)
        {
            // Get the polyline of the lane of the busStop
            var lane = netInfo.GetLane(busStop.lane);
            // Get the midpoint of the first 2 positions of shape in lane.
            var midPoint = lane.GetMidPoint();
            //Debug.Log(midPoint);

            // Get the UTM corrdinates by subtracting the netOffset in location-element.
            var utm = location.GetUTM(midPoint.x, midPoint.y);
            // Convert the UTM corrdinates to Latitude Longitude.
            var latlon = UTMtoLatLon(utm[0], utm[1], location.GetZone());
            //Debug.Log(latlon[0] + ", " + latlon[1]);

            // Convert the Latitude Longitude to coordinates in Unity.
            var convVec = CoordinateConvertor.LatLonToVector3(latlon[0], latlon[1], 0, mb);
            //Debug.Log(convVec);

            // Create a busStop based on the converted coordinate.
            //var go = new GameObject(busStop.id);
            //go.transform.position = convVec;
            //go.transform.SetParent(this.transform);
            creator.AddNewStation(convVec, busStop.id);
        }

        DestroyImmediate(creator);
    }

    public void CreateBusLines()
    {
        if (busInfo == null)
        {
            Debug.Log("no busInfo loaded.");
            return;
        }

        TransLineCreator creator = TransLineCreator.AttachToGameObject();
        creator.SetLines(GameObject.FindGameObjectsWithTag("TransLine"));
        creator.SetStations(FindObjectsOfType<StationController>());

        // Go through the flows (bus lines) in busInfo.
        foreach (var flow in busInfo.flows)
        {
            creator.ResetEditingInfo();
            creator.editLineName = flow.id;
            creator.lineCategory = LineCategory.Bus;
            foreach (var stop in flow.stops)
            {
                var go = GameObject.Find(stop.busStop);
                creator.AddStationToNewLine(go.GetComponentInParent<StationController>());
                
            }
            // LATER TODO: Calculate the traveling time in Sumo based on maxSpeed and length of the edges.
            creator.CreateNewLine();
        }

        DestroyImmediate(creator);

    }

    public static GameObject CreateGameObject(AdditionalContainer ac, NetContainer nc)
    {
        var container = FindObjectOfType<SumoContainer>();
        if (container != null)
        {
            DestroyImmediate(container.gameObject);
        }
        container = new GameObject("SumoContainer").AddComponent<SumoContainer>();
        container.busInfo = ac;
        container.netInfo = nc;
        return container.gameObject;
    }

    public static void LoadSumoCFG(string path)
    {

        string dirPath = Path.GetDirectoryName(path);
        //Debug.Log(dirPath);

        var doc = new XmlDocument();
        doc.Load(path);
        var node = doc.GetElementsByTagName("net-file")[0];
        var netPath = node.Attributes["value"].Value;

        node = doc.GetElementsByTagName("route-files")[0];
        var routePath = node.Attributes["value"].Value;

        node = doc.GetElementsByTagName("additional-files")[0];
        var additionalPath = node.Attributes["value"].Value;

        netPath = Path.Combine(dirPath, netPath);
        routePath = Path.Combine(dirPath, routePath);
        additionalPath = Path.Combine(dirPath, additionalPath);

        //Debug.Log(netPath);
        //Debug.Log(routePath);
        //Debug.Log(additionalPath);

        //string lane;
        AdditionalContainer addContainer;
        using (var stream = new FileStream(additionalPath, FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(AdditionalContainer));
            addContainer = serializer.Deserialize(stream) as AdditionalContainer;
            //lane = addContainer.busStops[0].lane;
            //Debug.Log(lane);
            //foreach (var item in container.busStops)
            //{
            //    Debug.Log(item.id + ", " + item.lane);
            //}
        }
        NetContainer netContainer;
        using (var stream = new FileStream(netPath, FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(NetContainer));
            netContainer = serializer.Deserialize(stream) as NetContainer;
            //Debug.Log(container.GetLane(lane));
            //Debug.Log(container.location.origBoundary);
        }

        var go = SumoContainer.CreateGameObject(addContainer, netContainer);
        go.transform.SetParent(GameObject.Find("Transportation").transform);

        var sumoContainer = go.GetComponent<SumoContainer>();
        sumoContainer.CreateBusStops();
        sumoContainer.CreateBusLines();
    }

    /// <summary>
    /// Default method to convert UTM to WGS-84. 
    /// </summary>
    /// <param name="easting"></param>
    /// <param name="northing"></param>
    /// <param name="utmZone"></param>
    /// <param name="northernHemisphere"></param>
    /// <returns></returns>
    public static double[] UTMtoLatLon(double easting, double northing, int utmZone, bool northernHemisphere = true)
    {

        double radius = 6378137d;
        double eccSquared = 0.00669438d;
        double rad2deg = 180d / Math.PI;
        double k0 = 0.9996d;
        double e1 = (1d - Math.Sqrt(1d - eccSquared)) / (1d + Math.Sqrt(1d - eccSquared));
        double x = easting - 500000d;
        double y = northing;

        if (!northernHemisphere)
            y -= 10000000d;

        double LongOrigin = (utmZone - 1d) * 6d - 180d + 3d;  //+3 puts origin in middle of zone
        double eccPrimeSquared = eccSquared / (1d - eccSquared);
        double m = y / k0;
        double mu = m / (radius * (1d - eccSquared / 4d - 3d * eccSquared * eccSquared / 64d - 5d * eccSquared * eccSquared * eccSquared / 256d));
        double phi1Rad = mu + (3d * e1 / 2d - 27d * e1 * e1 * e1 / 32d) * Math.Sin(2d * mu)
                + (21d * e1 * e1 / 16d - 55d * e1 * e1 * e1 * e1 / 32d) * Math.Sin(4d * mu)
                + (151d * e1 * e1 * e1 / 96d) * Math.Sin(6d * mu);
        double phi1 = phi1Rad * rad2deg;
        var N1 = radius / Math.Sqrt(1d - eccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad));
        var T1 = Math.Tan(phi1Rad) * Math.Tan(phi1Rad);
        var C1 = eccPrimeSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad);
        var R1 = radius * (1d - eccSquared) / Math.Pow(1d - eccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad), 1.5d);
        var D = x / (N1 * k0);
        var Lat = phi1Rad - (N1 * Math.Tan(phi1Rad) / R1) * (D * D / 2d - (5d + 3d * T1 + 10d * C1 - 4d * C1 * C1 - 9d * eccPrimeSquared) * D * D * D * D / 24d
                + (61d + 90d * T1 + 298d * C1 + 45d * T1 * T1 - 252d * eccPrimeSquared - 3d * C1 * C1) * D * D * D * D * D * D / 720d);
        Lat = Lat * rad2deg;

        var Long = (D - (1d + 2d * T1 + C1) * D * D * D / 6d + (5d - 2d * C1 + 28d * T1 - 3d * C1 * C1 + 8d * eccPrimeSquared + 24d * T1 * T1)
                        * D * D * D * D * D / 120d) / Math.Cos(phi1Rad);
        Long = LongOrigin + Long * rad2deg;
        return new double[] { Lat, Long };
    }
}

[XmlRoot("additional")]
public class AdditionalContainer
{
    [XmlElement("busStop")]
    public List<SumoBusStop> busStops = new List<SumoBusStop>();

    [XmlElement("flow")]
    public List<SumoFlow> flows = new List<SumoFlow>();
}

public class SumoFlow
{
    [XmlAttribute]
    public string id;

    [XmlAttribute]
    public string period;

    [XmlElement("route")]
    public SumoFlowRoute route;

    [XmlElement("stop")]
    public List<SumoFlowStop> stops = new List<SumoFlowStop>();
}

public class SumoFlowRoute
{
    [XmlAttribute]
    public string edges;
}

public class SumoFlowStop
{
    [XmlAttribute]
    public string busStop;

    [XmlAttribute]
    public string duration;

    [XmlAttribute]
    public string triggered;
}

public class SumoBusStop
{
    [XmlAttribute]
    public string id;

    [XmlAttribute]
    public string lane;

    [XmlAttribute]
    public string lines;
}

[XmlRoot("net")]
public class NetContainer
{
    [XmlElement("edge")]
    public List<SumoEdge> edges = new List<SumoEdge>();

    [XmlElement("location")]
    public SumoLocation location;

    public SumoLane GetLane(string lane_id)
    {
        return edges.Find(e => e.lane.id.Equals(lane_id)).lane;
    }
}

public class SumoEdge
{
    [XmlAttribute]
    public string id;

    [XmlElement("lane")]
    public SumoLane lane;

    [XmlAttribute]
    public string shape;
}

public class SumoLane
{
    [XmlAttribute]
    public string id;

    [XmlAttribute]
    public string length;

    [XmlAttribute]
    public string shape;

    /// <summary>
    /// Return the midpoint of the first section in shape.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMidPoint()
    {
        // Convert shape from string to Vector2s
        string[] lines = shape.Split(' ');
        string[] p1 = lines[0].Split(',');
        string[] p2 = lines[1].Split(',');
        Vector2 v1 = new Vector2(float.Parse(p1[0]), float.Parse(p1[1]));
        Vector2 v2 = new Vector2(float.Parse(p2[0]), float.Parse(p2[1]));
        return Vector2.Lerp(v1, v2, 0.5f);
    }
}

public class SumoLocation
{
    [XmlAttribute]
    public string netOffset;

    [XmlAttribute]
    public string convBoundary;

    [XmlAttribute]
    public string origBoundary;

    [XmlAttribute]
    public string projParameter;

    /// <summary>
    /// First Vector2 is min point. Second Vector2 is max point.
    /// </summary>
    /// <returns></returns>
    public Vector2[] GetConvertedBoundary()
    {
        string[] values = convBoundary.Split(',');
        Vector2 min = new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        Vector2 max = new Vector2(float.Parse(values[2]), float.Parse(values[3]));
        Debug.Log("min: (" + min.x + "," + min.y + ") max: (" + max.x + "," + max.y + ")");

        return new Vector2[] { min, max };
    }
    
    /// <summary>
    /// First Vector2 is min point. Second Vector2 is max point. Convention in Sumo is Longitude-Latitude.
    /// </summary>
    /// <returns></returns>
    public Vector2[] GetOriginalBoundary()
    {
        string[] values = origBoundary.Split(',');
        // Longitude in pos 0 and 2, Latitude in pos 1 and 3;
        Vector2 min = new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        Vector2 max = new Vector2(float.Parse(values[2]), float.Parse(values[3]));
        Debug.Log("min: (" + min.x + "," + min.y + ") max: (" + max.x + "," + max.y + ")");

        return new Vector2[] { min, max };
    }

    /// <summary>
    /// Return the UTM zone. 
    /// </summary>
    /// <returns></returns>
    public int GetZone()
    {
        string[] values = projParameter.Split(' ');
        foreach (var value in values)
        {
            if (value.Contains("zone"))
            {
                var strs = value.Split('=');
                return int.Parse(strs[1]);
            }
        }
        return -1;
    }

    /// <summary>
    /// Return the UTM coordinate by subtracting the netOffset.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public double[] GetUTM(double x, double y)
    {
        string[] values = netOffset.Split(',');
        double xOffset = double.Parse(values[0]);
        double yOffset = double.Parse(values[1]);
        //Debug.Log(xOffset + ", " + yOffset);
        return new double[] { x - xOffset, y - yOffset };
    }
}
