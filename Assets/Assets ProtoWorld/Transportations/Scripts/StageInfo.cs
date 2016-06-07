using System;
using UnityEngine;

public class StageInfo
{
    public StationController Start { get; set; }

    public StationController End { get; set; }

    public float TravelTime { get; set; }

    public LineController Line { get; set; }

    public LineCategory Category { get { return Line.category; } }

    public LineDirection Direction { get; set; }

    public bool Equals(StageInfo other)
    {
        if (other == null)
            return false;

        if (this.Start.Equals(other.Start) && this.End.Equals(other.End))
            return true;
        else
            return false;
    }

    public override string ToString()
    {
        return String.Format("{0}-{1}: {2}s {3}, {4}",
            Start.stationName, End.stationName, TravelTime, Category.ToString(), LineController.MakeKeyString(Line.id, Direction));
    }

}