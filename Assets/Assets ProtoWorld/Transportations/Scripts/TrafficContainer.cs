using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System;

[XmlRoot("root")]
public class TrafficContainer
{
    [XmlArray("stations")]
    [XmlArrayItem("station")]
    public List<BaseStation> stations = new List<BaseStation>();

    [XmlArray("transportlines")]
    [XmlArrayItem("line")]
    public List<BaseLine> lines = new List<BaseLine>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(TrafficContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static TrafficContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(TrafficContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as TrafficContainer;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static TrafficContainer LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(TrafficContainer));
        return serializer.Deserialize(new StringReader(text)) as TrafficContainer;
    }

}

public class BaseStation
{
    [XmlAttribute("stationId")]
    public int id;

    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    [XmlAttribute("z")]
    public float z;

    [XmlAttribute("name")]
    public string name;

    public Vector3 GetPoint()
    {
        return new Vector3(x, y, z);
    }
}

public class BaseLine
{
    [XmlAttribute("transportId")]
    public int id;

    [XmlAttribute("category")]
    public string category;

    [XmlAttribute("name")]
    public string name;

    [XmlAttribute("stationIds")]
    public string stationIds;

    [XmlAttribute("travelingTimes")]
    public string travelingTimes;

    public int[] GetStationIds()
    {
        var splits = stationIds.Split(',');
        var ids = new int[splits.Length];
        for (int i = 0; i < splits.Length; i++)
        {
            ids[i] = int.Parse(splits[i]);
        }
        return ids;
    }

    public LineCategory GetCategory()
    {
        return (LineCategory)Enum.Parse(typeof(LineCategory), category);
    }
}



