using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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
