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

public class StationStatistics : MonoBehaviour {

    [System.Serializable]
    public class StationStats
    {
        [HideInInspector]
        public string name;
        public int headcount;
        public StationController controller;
    }

	public int[] QuePerStation;
	public int stationNR = 0;

    public int totalQueuing;
    public List<StationStats> queues;

    void Awake()
    {
        QuePerStation = new int[this.gameObject.transform.childCount];
    }

    // Use this for initialization
    void Start () {
        var stations = GetComponentsInChildren<StationController>();
        queues = new List<StationStats>();
        foreach (var station in stations)
        {
            queues.Add(new StationStats
            {
                controller = station,
                name = station.GetIdAndName(),
            });
        }
    }
	
	// Update is called once per frame
	void Update () {
        totalQueuing = 0;
        foreach (var station in queues)
        {
			
            station.headcount = station.controller.GetHeadCount();
			QuePerStation[stationNR] = station.headcount;
			stationNR = stationNR +1;
            totalQueuing += station.headcount;
        }
		stationNR = 0;
	}
}
