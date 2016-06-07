/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedestriansRouting.cs
 * Miguel Ramos Carretero
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Note: This object can be referenced by several pedestrians, so the methods developed should not alter or modify any parameter.
/// </summary>
public class FlashPedestriansRouting {

    public FlashPedestriansDestination destinationPoint;
    public Itinerary itinerary;

    /// <summary>
    /// Constructor of the class.
    /// </summary>
    /// <param name="destinationPoint"></param>
    /// <param name="itinerary"></param>
    public FlashPedestriansRouting(FlashPedestriansDestination destinationPoint, Itinerary itinerary)
    {
        this.destinationPoint = destinationPoint;
        this.itinerary = itinerary;
    }

    /// <summary>
    /// Gets the next point to go in the itinerary.
    /// </summary>
    /// <returns>Transform with the next point to go in the itinerary, 
    /// or null if there are no more.</returns>
    //internal Transform GetNextPointOfTheItinerary()
    //{
    //    StationController stop = itinerary.GetStop(++stopIndex);

    //    if (stop != null)
    //        return stop.gameObject.transform;
    //    else
    //        return null; 
    //}

    /// <summary>
    /// Gets the current point selected.
    /// </summary>
    /// <returns>Transform with the current point to go in the itinerary, 
    /// or null if there is no current point.</returns>
    //internal Transform GetCurrentPointOfTheItinerary()
    //{
    //    StationController stop = itinerary.GetStop(stopIndex);

    //    if (stop != null)
    //        return stop.gameObject.transform;
    //    else
    //        return null;
    //}

    /// <summary>
    /// Get the current station where the pedestrian is heading.
    /// </summary>
    /// <returns>StationController that represents the station, or null if 
    /// there is no current station.</returns>
    internal StationController GetStationAt(int index)
    {
        return itinerary.GetStop(index);
    }

    /// <summary>
    /// Get the route information from the given station.
    /// </summary>
    /// <param name="station">Station from where the information is requested.</param>
    /// <returns>StageInfo object with the information from the given station.</returns>
    internal StageInfo GetRouteInfoFrom(StationController station)
    {
        int stationIndex = itinerary.FindStopIndex(station);

        if (stationIndex < itinerary.StageInfos.Count)
            return itinerary.StageInfos[stationIndex];
        else
            return null;
    }

    /// <summary>
    /// Get the id of the queue of the current stop where the pedestrian is heading.
    /// </summary>
    /// <returns></returns>
    internal string GetNextLineQueueId(int index)
    {
        var info = itinerary.StageInfos[index];
        return LineController.MakeKeyString(info.Line.id, info.Direction);
    }

    //internal void UpdateCurrentStop(StationController station)
    //{
    //    stopIndex = itinerary.FindStopIndex(station);
    //}

    /// <summary>
    /// Gets the point on the itinerary indicated by the given index.
    /// </summary>
    /// <returns>Transform with the point requested, or null if there is no point 
    /// at that index.</returns>
    internal Transform GetPointOfTheItinerary(int index)
    {
        StationController stop = itinerary.GetStop(index);

        if (stop != null)
            return stop.gameObject.transform;
        else
            return null;
    }

    internal int GetIndexFrom(StationController station)
    {
        return itinerary.FindStopIndex(station);
    }

    internal System.Collections.Generic.List<StationController> GetStatesTransits()
    {
        return itinerary.Transits;
    }
}
