/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class RoutingController : MonoBehaviour
{
    public static char[] DelimiterChars = { ' ', ',', ';', '\t' };

    public int StationCount { get { return Stations.Count; } }

    Dictionary<string, StationController> Stations { get; set; }

    /// <summary>
    /// All the transport lines in the scene.
    /// </summary>
    Dictionary<int, LineController> Lines { get; set; }

    Dictionary<string, List<Itinerary>> Itineraries { get; set; }

    Dictionary<string, StageInfo> StageInfos { get; set; }

    public int busCapacity = 50;
    public int tramCapacity = 100;
    public int metroCapacity = 200;
    public int trainCapacity = 300;

    [Range(5, 20)]
    public float busSpawnFrequency = 10;
    [Range(5, 20)]
    public float tramSpawnFrequency = 20;
    [Range(5, 20)]
    public float metroSpawnFrequency = 5;
    [Range(5, 20)]
    public float trainSpawnFrequency = 15;

    [Range(1, 2000)]
    public float walkingDistance = 1000;

    // Use this for initialization
    void Start()
    {
        GatherStations();
        CreateWalkingRoutes();
        GatherLines();
        SetupLinesAtStations();
        CreateRoutes();

        //Debug.Log(GetItinerary(0, 4, TravelPreference.none));
        //Debug.Log(GetItinerary(7, 0, TravelPreference.none));

    }

    private void GatherStations()
    {
        var sgos = GameObject.FindGameObjectsWithTag("TransStation");

        Stations = new Dictionary<string, StationController>();
        foreach (var go in sgos)
        {
            var station = go.GetComponent<StationController>();
            Stations.Add(station.GetId(), station);
        }
    }

    public void CreateWalkingRoutes()
    {
        TransLineCreator lineCreator = TransLineCreator.AttachToGameObject();
        lineCreator.SetLines(GameObject.FindGameObjectsWithTag("TransLine"));
        var stations = FindObjectsOfType<StationController>();

        if (stations.Length < 1)
            return;

        float speed = 4;
        var globalParams = FindObjectOfType<FlashPedestriansGlobalParameters>();
        if (globalParams != null)
            speed = globalParams.averageSpeed;
        for (int i = 0; i < stations.Length; i++)
        {
            for (int j = i + 1; j < stations.Length; j++)
            {
                var pos1 = stations[i].transform.position;
                var pos2 = stations[j].transform.position;
                var dist = Vector3.Distance(pos1, pos2);
                if (dist <= walkingDistance)
                {
                    lineCreator.ResetEditingInfo();
                    lineCreator.editLineName = stations[i].stationName + "-" + stations[j].stationName;
                    lineCreator.lineCategory = LineCategory.Walk;
                    lineCreator.AddStationToNewLine(stations[i]);
                    lineCreator.AddStationToNewLine(stations[j]);
                    lineCreator.travelTimes[0] = dist / speed;
                    lineCreator.CreateNewLine();
                }
            }
        }
        DestroyImmediate(lineCreator);
    }

    private void GatherLines()
    {
        var lgos = GameObject.FindGameObjectsWithTag("TransLine");
        Lines = new Dictionary<int, LineController>();
        foreach (var go in lgos)
        {
            var line = go.GetComponent<LineController>();
            //Debug.Log(line.id);
            Lines.Add(line.id, line);
        }
    }

    private void SetupLinesAtStations()
    {
        foreach (var line in Lines.Values)
        {
            // Could add a filter here to exclude Walk and Bike but it might affect FindPath!!!
            // So don't do it!!!!
            foreach (var station in line.stations)
            {
                //Stations[station.name].lineIds.Add(line.id);
                station.lineThruThisStation.Add(line);
            }
        }
        foreach (var station in Stations.Values)
        {
            station.InitLineQueues();
        }
    }

    #region CreateRoutes

    private void CreateRoutes()
    {
        //Debug.Log("CreateRoutes");
        Itineraries = new Dictionary<string, List<Itinerary>>();
        StageInfos = new Dictionary<string, StageInfo>();

        foreach (var s1 in Stations.Values)
        {
            foreach (var s2 in Stations.Values)
            {
                string key = MakeKeyString(s1, s2);
                Itineraries[key] = new List<Itinerary>();
                FindPath(s1, s2);
            }
        }
    }

    private void FindPath(StationController s1, StationController s2)
    {
        if (s1 == s2)
            return;
        List<StationController> visited = new List<StationController>();
        visited.Add(s1);
        BreadthFirst(visited, s2);
    }

    private void BreadthFirst(List<StationController> visited, StationController end)
    {
        List<StationController> adjacents;
        if (visited.Count > 1)
        {
            int travelingLineId = StationController.GetCommonLineId(visited[visited.Count - 2], visited.Last());
            adjacents = GetAdjacentStations(visited.Last(), travelingLineId);
        }
        else
        {
            adjacents = GetAdjacentStations(visited.Last());
        }

        foreach (var seed in adjacents)
        {
            if (visited.Contains(seed))
            {
                continue;
            }

            if (seed.Equals(end))
            {
                List<StationController> visitedCopy = new List<StationController>(visited);
                visitedCopy.Add(end);

                var itinerary = new Itinerary(visitedCopy);

                // Generate the total traveling time for that route.
                GenerateTravelInfoForRoute(itinerary);

                string key = MakeKeyString(visitedCopy.First(), visitedCopy.Last());
                Itineraries[key].Add(itinerary);
            }
        }

        foreach (var seed in adjacents)
        {
            if (visited.Contains(seed) || seed.Equals(end))
            {
                continue;
            }

            List<StationController> visitedCopy = new List<StationController>(visited);
            visitedCopy.Add(seed);
            BreadthFirst(visitedCopy, end);
        }
    }

    private void GenerateTravelInfoForRoute(Itinerary itinerary)
    {
        List<StationController> stops = itinerary.WayPoints;

        List<StageInfo> stageInfos = new List<StageInfo>();
        for (int i = 1; i < stops.Count; i++)
        {
            StationController first = stops[i - 1];
            StationController second = stops[i];
            int lineId = StationController.GetCommonLineId(first, second);

            if (lineId < 0)
            {
                Debug.LogErrorFormat("no common line between {0}, {1}", first, second);
            }

            LineController line = Lines[lineId];
            string key = MakeKeyString(first, second);
            StageInfo stageInfo;
            if (!StageInfos.TryGetValue(key, out stageInfo))
            {
                stageInfo = new StageInfo();
                stageInfo.Start = first;
                stageInfo.End = second;
                stageInfo.Line = line;
                stageInfo.TravelTime = line.GetLegTravelTime(first, second);
                stageInfo.Direction = line.GetDirection(first, second);

                StageInfos.Add(key, stageInfo);
            }
            stageInfos.Add(stageInfo);
        }

        itinerary.StageInfos = stageInfos;

        List<StationController> transits = new List<StationController>();
        for (int i = 1; i < stops.Count - 1; i++)
        {
            StationController current = stops[i];

            int firstLineId = stageInfos[i - 1].Line.id;
            int secondLineId = stageInfos[i].Line.id;
            if (firstLineId != secondLineId)
                transits.Add(current);
        }
        transits.Add(stops.Last());
        itinerary.Transits = transits;
    }

    /// <summary>
    /// Return adjacent stations. 
    /// If the station is closed, it will only return stations on the same line 
    /// you are traveling on.
    /// </summary>
    /// <param name="station"></param>
    /// <param name="lineId"></param>
    /// <returns></returns>
    private List<StationController> GetAdjacentStations(StationController station, int travelingOnLineId)
    {
        List<StationController> adjacents = new List<StationController>();
        foreach (var line in station.lineThruThisStation)
        {
            if (line.id != travelingOnLineId && station.outOfService)
            {
                continue;
            }
            //LineController line = Lines[lineId];
            var stations = line.GetAdjacentStations(station);
            foreach (var s in stations)
            {
                adjacents.Add(s);
            }
        }
        return adjacents;
    }

    private List<StationController> GetAdjacentStations(StationController station)
    {
        List<StationController> adjacents = new List<StationController>();
        foreach (var line in station.lineThruThisStation)
        {
            //LineController line = Lines[lineId];
            var stations = line.GetAdjacentStations(station);
            foreach (var s in stations)
            {
                adjacents.Add(s);
            }
        }
        return adjacents;
    }

    #endregion //CreateRoutes

    /// <summary>
    /// Get a itinerary from stationId 'start' to stationId 'end' based on the 'preference'.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="preference"></param>
    /// <returns></returns>
    public Itinerary GetItinerary(int start, int end, TravelPreference preference)
    {
        return GetItinerary(Stations[start.ToString()], Stations[end.ToString()], preference);
    }

    /// <summary>
    /// Get a itinerary from station 'start' to station 'end' based on the 'preference'.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="preference"></param>
    /// <returns></returns>
    public Itinerary GetItinerary(StationController start, StationController end, TravelPreference preference)
    {
        if (start.Equals(end))
            return null;

        if (start.outOfService || end.outOfService)
            return null;

        var itineraries = new List<Itinerary>(GetItineraries(start, end));
        switch (preference)
        {
            case TravelPreference.none:
                break;
            case TravelPreference.transit:
                itineraries.Sort((x, y) => x.Transits.Count.CompareTo(y.Transits.Count));
                break;
            default:
            case TravelPreference.time:
                itineraries.Sort((x, y) => x.GetTotalTravelTime().CompareTo(y.GetTotalTravelTime()));
                break;
        }
        if (itineraries.Count < 1)
        {
            //Debug.LogFormat("No itinery between {0}, {1}", start, end);
            return null;
        }
        return itineraries[0];
    }

    /// <summary>
    /// Get a list of itineraries (not sorted) from station 'start' to station 'end'.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private List<Itinerary> GetItineraries(StationController start, StationController end)
    {
        string key = MakeKeyString(start, end);
        var itineraries = Itineraries[key];
        //foreach (var i in itineraries)
        //{
        //    Debug.Log(i);
        //}
        return itineraries;
    }

    /// <summary>
    /// Add SumoBusController to the buses coming from Sumo.
    /// </summary>
    /// <param name="busObject"></param>
    /// <param name="id"></param>
    public void HandleBusObjectsFromSumo(GameObject busObject, string id)
    {

        string[] firstSplits = id.Split('.');
        string[] secondSplits = firstSplits[0].Split('_');
        string busNumber = secondSplits[0];

        int busNum;
        if (!Int32.TryParse(busNumber, out busNum))
        {
            busNumber = Regex.Match(id, @"\d+").Value;
            //Debug.Log("[Regex] busNum: " + busNumber);
        }
        
        LineController lineController = null;
        foreach (var line in Lines.Values)
        {
            if (line.category != LineCategory.Bus)
                continue;

            var lineNums = line.lineName.Split('_').ToList();

            //if (busNumber.Equals(line.lineName))
            if (lineNums.Contains(busNumber))
            {
                lineController = line;
                break;
            }
        }
        //Debug.Log(lineController);

        // Found the line with the same busNumber.
        if (lineController != null)
        {
            //Debug.Log(busObject.name + " is added to " + lineController.name);
            lineController.AddVehicle(busObject);
            var busController = busObject.AddComponent<SumoBusController>();
            busController.line = lineController;
            //busController.capacity = lineController.vehicleCapacity;

            var s1 = lineController.stations.First();
            var s2 = lineController.stations.Last();
            var d1 = Vector3.Distance(busObject.transform.position, s1.transform.position);
            var d2 = Vector3.Distance(busObject.transform.position, s2.transform.position);

            // Check if the bus is going outbound or inbound
            // by comparing the distance to the first and the last station.
            // If closer to the first station => outbound,
            // if closer to the last station => inbound.
            if (d1 < d2)
            {
                //busController.currentStation = s1;
                //busController.nextStation = s1;
                //busController.direction = LineDirection.OutBound;
                busController.ResetVehicle(LineDirection.OutBound);
            }
            else
            {
                //busController.currentStation = s2;
                //busController.nextStation = s2;
                //busController.direction = LineDirection.InBound;
                busController.ResetVehicle(LineDirection.InBound);

            }

            // Obsolete: should be taken care of by the SumoBusController.
            // busController.ArrivedAtNextStation();
        }
    }

    /// <summary>
    /// Make a keyString to store itinerary between station s1 and station s2.
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static string MakeKeyString(StationController s1, StationController s2)
    {
        return s1.id + "-" + s2.id;
    }

    /// <summary>
    /// Get a random station in the scene for testing.
    /// </summary>
    /// <returns></returns>
    public int GetRandomStationId()
    {
        StationController station;
        do
        {
            int index = Mathf.RoundToInt(UnityEngine.Random.Range(0, StationCount - 1));
            station = Stations.ElementAt(index).Value;
        } while (station.outOfService);
        return int.Parse(station.name);
    }

    /// <summary>
    /// Call this to set a station in or out of service.
    /// This will notify the affected lines and vehicles
    /// and empty the station from travelers if outOfService = true.
    /// </summary>
    /// <param name="station"></param>
    /// <param name="outOfService"></param>
    public void SetStationOutOfService(StationController station, bool outOfService)
    {
        // MAYBE: Make a check and warn if all remaining stations for a vehicle are closed.
        station.outOfService = outOfService;

        CreateRoutes();

        if (outOfService)
            station.EmptyTravelersAtStation(); // Needs to call this after CreateRoutes or else the travelers might get the same route.
    }


}
