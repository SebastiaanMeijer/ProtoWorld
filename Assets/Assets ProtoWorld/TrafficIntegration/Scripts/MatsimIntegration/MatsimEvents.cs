using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

[XmlRoot("events")]
public class MatsimEvents
{
    [XmlElement("event")]
    public List<MatsimEvent> lines;

    public static MatsimEvents Load(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(MatsimEvents));
            return serializer.Deserialize(stream) as MatsimEvents;
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
}

public class MatsimEvent
{
    [XmlAttribute]
    public float time;

    [XmlAttribute]
    public string type;

    public override string ToString()
    {
        return time + " " + type;
    }
}

public class TransitDriverStarts : MatsimEvent
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

    public override string ToString()
    {
        return base.ToString() + " " + driverId + " " + vehicleId + " " + transitLineId + " " + transitRouteId + " " + departureId;
    }
}

public class departure : MatsimEvent
{
    [XmlAttribute]
    public string person;

    [XmlAttribute]
    public string link;

    [XmlAttribute]
    public string legMode;

    public override string ToString()
    {
        return base.ToString() + " " + person + " " + link + " " + legMode;
    }
}