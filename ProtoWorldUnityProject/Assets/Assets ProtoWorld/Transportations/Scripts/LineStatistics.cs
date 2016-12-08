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

public class LineStatistics : MonoBehaviour {

    [System.Serializable]
    public class LineStats
    {
        [HideInInspector]
        public string name;
        public int queuing;
        public int traveling;
        public LineController controller;
    }

    public int totalQueuing;
    public int totalTraveling;

    public int trainQueuing = 0;
    public int tramQueuing = 0;
    public int busQueuing = 0;
    public int metroQueuing = 0;

    public List<LineStats> lineStats;

    // Use this for initialization
    void Start () {
        var lines = GetComponentsInChildren<LineController>();
        lineStats = new List<LineStats>();
        foreach (var line in lines)
        {
            lineStats.Add(new LineStats
            {
                controller = line,
                name = line.gameObject.name,
            });
        }
    }
	
	// Update is called once per frame
	void Update () {
        totalQueuing = 0;
        totalTraveling = 0;
        trainQueuing = 0;
        tramQueuing = 0;
        busQueuing = 0;
        metroQueuing = 0;

        foreach (var stats in lineStats)
        {
            stats.queuing = stats.controller.queuingHeadCount;
            stats.traveling = stats.controller.travelingHeadCount;
            totalQueuing += stats.queuing;
            totalTraveling += stats.traveling;

            if (stats.controller.gameObject.name.Contains("Train_"))
            {
                trainQueuing += stats.queuing;
            }
            else if (stats.controller.gameObject.name.Contains("Bus_"))
            {
                busQueuing += stats.queuing;
            }
            else if (stats.controller.gameObject.name.Contains("Tram_"))
            {
                tramQueuing += stats.queuing;
            }
            else if (stats.controller.gameObject.name.Contains("Metro_"))
            {
                metroQueuing += stats.queuing;
            }
        }
    }
}
