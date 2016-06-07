using System.Xml.Serialization;


/// <summary>
/// Data with a time stamp.
/// </summary>
public class TimedData
{
    [XmlAttribute]
    public float time;

    public virtual string GetAction()
    {
        return null;
    }

    public virtual float GetData()
    {
        return float.NaN;
    }
}

/// <summary>
/// Action (in string) with a time stamp.
/// </summary>
public class TimedAction : TimedData
{
    [XmlAttribute]
    public string action;

    public override string GetAction()
    {
        return action;
    }
}

/// <summary>
/// Value (in float) with a time stamp.
/// </summary>
public class TimedValue : TimedData
{
    [XmlAttribute]
    public float value;

    public override float GetData()
    {
        return value;
    }
}