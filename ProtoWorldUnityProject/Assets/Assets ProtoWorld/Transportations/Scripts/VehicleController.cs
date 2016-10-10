/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private static int IdCounter = 0;

    public int id;

    public LineController line; // To SUMO: line.Id = "route id"

    public LineDirection direction;

    public StationController currentStation;

    public StationController nextStation; // To SUMO: nextStation.Id = "route index"

    protected float LegTravelTime { get { return line.GetLegTravelTime(currentStation, nextStation); } }

    protected float DistanceToNextStation { get { return Vector3.Distance(transform.position, nextStation.transform.position); } }

    protected Dictionary<int, List<TravelerController>> disembarkersAtStation = new Dictionary<int, List<TravelerController>>();

    protected Dictionary<int, List<int>> redistributeFromStationToStation = new Dictionary<int, List<int>>();

    protected float startTime;

    public int capacity;

    public int headCount;

    TextMesh[] texts;

    protected TimeController timeController;

    //Delay of the previous leg
    public float delay = 0;

    //Elements for the not moving detector
    public float speed = 0;
    private float current_distance = 0;
    private float previous_distance = 0;

    [System.Serializable]
    public class DisembarkStats
    {
        [HideInInspector]
        public string stationName;
        [HideInInspector]
        public int id;
        public int headcount;
    }

    public List<DisembarkStats> disembarkStats = new List<DisembarkStats>();

    public static GameObject CreateGameObject(LineController line, LineDirection direction)
    {
        GameObject obj = obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale *= 0.6f;


        if (line.category == LineCategory.Train)
        {
            var comp = line.gameObject.GetComponent<SpawnerLineController>();
            if (comp != null && comp.modelToSpawn != null)
            {
                obj = Instantiate(comp.modelToSpawn);
            }
        }

        var ctrl = obj.AddComponent<VehicleController>();
        ctrl.id = IdCounter++;
        ctrl.line = line;
        ctrl.direction = direction;
        ctrl.currentStation = line.GetFirstStop(direction);
        ctrl.nextStation = ctrl.currentStation;
        ctrl.transform.position = ctrl.currentStation.transform.position;

        ctrl.capacity = line.GetVehicleCapacity();

        obj.name = "[V]" + line.category.ToString() + ctrl.id;

        obj.GetComponent<Collider>().enabled = false;
        var renderer = obj.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        renderer.useLightProbes = false;
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = Color.yellow;
        return obj;
    }

    /// <summary>
    /// Reset the timer.
    /// </summary>
    public void ResetTimer()
    {
        startTime = timeController.gameTime;// + UnityEngine.Random.Range(-.1f, .1f) * LegTravelTime;
    }

    /// <summary>
    /// To reuse the vehicle.
    /// Override by SumoBusController so that the gameobject position is not set to the currentStation at reset.
    /// </summary>
    /// <param name="direction"></param>
    public virtual void ResetVehicle(LineDirection direction)
    {
        this.direction = direction;
        currentStation = line.GetFirstStop(direction);
        nextStation = currentStation;
        transform.position = currentStation.transform.position;
        capacity = line.GetVehicleCapacity();
        InitLists();
        ResetTimer();
    }

    protected void InitLists()
    {
        foreach (var station in line.stations)
        {
            if (!disembarkersAtStation.ContainsKey(station.id))
            {
                disembarkersAtStation.Add(station.id, new List<TravelerController>());
                disembarkStats.Add(new DisembarkStats { id = station.id, stationName = station.stationName });
            }
        }
    }

    void Awake()
    {
        timeController = GameObject.FindObjectOfType<TimeController>();
        if (timeController == null)
            Debug.LogError("No timeController found!");

    }

    /// <summary>
    /// Initiate the list for disembarkers, statistics and reset timer.
    /// </summary>
    public virtual void Start()
    {
        //Debug.Log(name + ": veh start called.");
        InitLists();
        ResetTimer();
        if (line.useDefaultVehicleSymbol)
            TryAttachVehiclePrefab();
}

void TryAttachVehiclePrefab()
    {
        var template = GameObject.Find("VehicleSymbol");
        if (template != null)
        {
            var obj = Instantiate(template, Vector3.zero, Quaternion.identity) as GameObject;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;

            texts = GetComponentsInChildren<TextMesh>();
            foreach (var text in texts)
            {
                //text.text = line.category.ToString() + "\n" + line.name;
                text.text = line.name;
            }
        }
    }

    /// <summary>
    /// Update the position of the vehicle based on the total travel time defined in the xml-file
    /// and how much time has elapsed since leaving the last station.
    /// </summary>
    public virtual void Update()
    {
        if (gameObject.activeInHierarchy)
        {

            updateSpeed();

            if (DistanceToNextStation < 1)
            {
                //Calucate delay
                float endtime = timeController.gameTime;
                float duration = endtime - startTime;
                delay = duration - LegTravelTime;

                ArrivedAtNextStation();
                //startTime = Time.time;
                ResetTimer();
                //Debug.LogFormat("{0} -> {1}", CurrentStation, NextStation);
                headCount = GetHeadCount();
            }

            if (nextStation == null)
            {
                headCount = GetHeadCount();
                foreach (var list in disembarkersAtStation.Values)
                {
                    foreach (var traveler in list)
                    {
                        traveler.ArrivedAt(currentStation);
                    }
                    list.Clear();
                }
                gameObject.SetActive(false);
                return;
            }

            float journeyTime = timeController.gameTime - startTime;
            float fracTime = journeyTime / LegTravelTime;
            transform.position = Vector3.Lerp(currentStation.transform.position, nextStation.transform.position, fracTime);
        }
    }

    public void updateSpeed()
    {
        speed = (previous_distance - current_distance) / Time.deltaTime;

        previous_distance = current_distance;
        current_distance = DistanceToNextStation;

        //Transform symbol = transform.FindChild("VehicleSymbol(Clone)");
        //Transform cylinder_l = symbol.FindChild("Cylinder_L");


        //Check if the to be set shader is different, setting a shader every frame is not nice.
        if (Math.Abs(speed) < 1)
        {
           //Set cylinder_l to alternative (red) shader/material
        }
        else
        {
            //Set it to the original white shader/material
        }
        

    }

    /// <summary>
    /// If this station is closed then it will look for another station to disembark the travelers,
    /// if no remaing station is opened then it will disembark at this station.
    /// If this stations is not closed then it disembarks travelers 
    /// and will look for others that might want to disembarks here due to closed stations.
    /// </summary>
    public virtual void ArrivedAtNextStation()
    {
        if (nextStation.outOfService)
        {
            var nextAvailableStation = line.GetNextAvailableStation(nextStation, direction);
            if (nextAvailableStation != null)
            {
                if (!redistributeFromStationToStation.ContainsKey(nextAvailableStation.id))
                    redistributeFromStationToStation.Add(nextAvailableStation.id, new List<int>());
                redistributeFromStationToStation[nextAvailableStation.id].Add(nextStation.id);
            }
            else
            {
                List<TravelerController> disembarkers;
                if (disembarkersAtStation.TryGetValue(nextStation.id, out disembarkers))
                    nextStation.HandleDisembarkers(disembarkers);
            }
        }
        else
        {
            List<int> closedStations;
            if (redistributeFromStationToStation.TryGetValue(nextStation.id, out closedStations))
            {
                foreach (var stationId in closedStations)
                {
                    List<TravelerController> disembarkersFromClosedStation;
                    if (disembarkersAtStation.TryGetValue(stationId, out disembarkersFromClosedStation))
                        nextStation.HandleDisembarkers(disembarkersFromClosedStation);
                }
            }
            List<TravelerController> disembarkers;
            if (disembarkersAtStation.TryGetValue(nextStation.id, out disembarkers))
                nextStation.HandleDisembarkers(disembarkers);
            if (nextStation != line.GetLastStop(direction))
                EmbarkFrom(nextStation);
        }

        //Debug.LogFormat("Arrived at {0}, Train info:{1}", NextStation, this);
        headCount = GetHeadCount();
        currentStation = nextStation;
        nextStation = line.GetNextStop(currentStation, direction);
    }

    /// <summary>
    /// While leaving the station, the vehicle will take on some travelers
    /// based on its capacity.
    /// </summary>
    /// <param name="station"></param>
    public void EmbarkFrom(StationController station)
    {
        int available = capacity - GetHeadCount();
        Queue<TravelerController> travelers;
        string key = LineController.MakeKeyString(line.id, direction);
        if (station.lineQueues.TryGetValue(key, out travelers))
        {
            for (int i = 0; i < available && travelers.Count > 0; i++)
            {
                TravelerController tc = travelers.Dequeue();
                SortTravelersToDisembarkStation(tc);
                tc.Embark();
            }
        }
    }

    /// <summary>
    /// At the station, the travelers are sorted to different list based on which station they are getting off.
    /// </summary>
    /// <param name="traveler"></param>
    public void SortTravelersToDisembarkStation(TravelerController traveler)
    {
        foreach (var transit in traveler.GetStagesTransits())
        {
            if (line.stations.Contains(transit))
            {
                //Debug.LogFormat("{0}, {1}", transit, NextStation);
                if (transit == nextStation)
                {
                    continue;
                }

                //if (line.category == LineCategory.Bus)
                //{
                //    foreach (var item in disembarkersAtStation)
                //    {
                //        Debug.Log(item.Key);
                //    }
                //    Debug.LogFormat("line: {0}, transit: {1}, traveler: {2}", line, transit, traveler);
                //}
                disembarkersAtStation[transit.id].Add(traveler);
            }
            else
            {
                //Debug.LogWarningFormat("PassengerController does not transit at all in this line?!");
            }
        }
        //Debug.LogFormat("{0}, station: {1}", ToString(), NextStation);
    }

    /// <summary>
    /// Update the total head count as well as the how they are distibuted to each stations.
    /// </summary>
    /// <returns></returns>
    protected int GetHeadCount()
    {
        int headCount = 0;
        List<TravelerController> list;
        foreach (var stats in disembarkStats)
        {
            if (disembarkersAtStation.TryGetValue(stats.id, out list))
            {
                stats.headcount = list.Count;
                headCount += list.Count;
            }
        }
        return headCount;
        //return disembarkersAtStation.Sum(x => x.Value.Count);
    }

    public override string ToString()
    {
        return String.Format("{0}: pos: {1}; Next: {2}; #Pass: {3}",
            name, transform.position, nextStation, GetHeadCount());
    }


}