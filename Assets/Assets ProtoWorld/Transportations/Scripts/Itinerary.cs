using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TravelPreference { none, time, transit, frequency, preferTrain, preferBus, preferMetro }

public class Itinerary 
{
    public List<StationController> WayPoints { get; set; }

    public StationController FirstStop { get { return WayPoints.First(); } }

    public StationController LastStop { get { return WayPoints.Last(); } }

    public List<StationController> Transits { get; set; }

    public List<StageInfo> StageInfos { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="stations"></param>
    public Itinerary(List<StationController> stations)
    {
        WayPoints = stations;
    }

    /// <summary>
    /// Get the total travel time of the route.
    /// </summary>
    /// <returns></returns>
    public float GetTotalTravelTime()
    {
        float totalTime = 0;
        foreach (var info in StageInfos)
        {
            totalTime += info.TravelTime;
        }
        return totalTime;
    }

    /// <summary>
    /// Get the station at the index-position of the WayPoint array.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public StationController GetStop(int index)
    {
        if (index >= 0 && index < WayPoints.Count)
        {
            return WayPoints[index];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get the index of the station in the WayPoint array.
    /// </summary>
    /// <param name="station"></param>
    /// <returns></returns>
    public int FindStopIndex(StationController station)
    {
        return WayPoints.FindIndex(x => x.Equals(station));
    }

    /// <summary>
    /// Listing the waypoints and the total travel time.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string str = "";
        foreach (var station in WayPoints)
        {
            str += station + ",";
        }
        return str.Remove(str.Length - 1) + "TT: " + GetTotalTravelTime();
    }

    /// <summary>
    /// Get remaining waypoints from index to the end.
    /// If index = 0, then the complete list is returned.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public List<StationController> GetRemainingWayPoints(int index)
    {
        if (index == 0)
            return WayPoints;
        if (index > 0 && index < WayPoints.Count)
        {
            return WayPoints.GetRange(index, WayPoints.Count - index);
        }
        return null;
    }

    /// <summary>
    /// Check if any of the remaing waypoints (from index) in this itinerary is out of service
    /// which happens to be a transit in this itinerary, it will return false.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsValid(int index = 0)
    {
        var remaining = GetRemainingWayPoints(index);
        if (remaining == null)
        {
            Debug.LogWarning("index out of range.");
            return false;
        }
        foreach (var station in Transits)
        {
            if (remaining.Contains(station) && station.outOfService)
            {
                if (station.Equals(LastStop) && IsStationAtEndOfLine(station))
                {
                    return true;
                }
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Find out whether the station is at the end.
    /// </summary>
    /// <param name="station"></param>
    /// <returns></returns>
    public bool IsStationAtEndOfLine(StationController station)
    {
        var info = GetStageInfoTo(station);
        if (info != null && info.Line.GetEndStations().Contains(station))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get the stageinfo that is heading towards the station.
    /// </summary>
    /// <param name="station"></param>
    /// <returns></returns>
    public StageInfo GetStageInfoTo(StationController station)
    {
        int index = FindStopIndex(station);
        return GetStageInfo(index - 1);
    }

    /// <summary>
    /// Get the stageinfo that is starting from the station.
    /// </summary>
    /// <param name="station"></param>
    /// <returns></returns>
    public StageInfo GetStageInfoFrom(StationController station)
    {
        int index = FindStopIndex(station);
        return GetStageInfo(index);
    }

    /// <summary>
    /// Get the stageinfo based on the index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public StageInfo GetStageInfo(int index)
    {
        if (index >= 0 && index < StageInfos.Count)
        {
            return StageInfos[index];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Comparing the waypoints of this itinerary with the other.
    /// False if other == null
    /// False if the lengths are different.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Itinerary other)
    {
        if (other == null)
            return false;

        if (this.WayPoints.Count != other.WayPoints.Count)
            return false;

        for (int i = 0; i < this.WayPoints.Count; i++)
        {
            if (this.WayPoints[i] != other.WayPoints[i])
                return false;
        }

        return true;
    }

}

