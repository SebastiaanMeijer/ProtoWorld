/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This controller is only suitable for buses from Sumo-simulations.
/// </summary>
public class SumoBusController : VehicleController
{

    /// <summary>
    /// Calling the base class to initate [disembarkersAtStation]
    /// Should not be needed to call.
    /// </summary>
    //void start()
    //{
    //    base.Start();
    //}

    /// <summary>
    /// To reuse the vehicle.
    /// Override so that the gameobject position is not set to the currentStation at reset.
    /// </summary>
    /// <param name="direction"></param>
    public override void ResetVehicle(LineDirection direction)
    {
        this.direction = direction;
        currentStation = line.GetFirstStop(direction);
        nextStation = currentStation;
        capacity = line.GetVehicleCapacity();
        InitLists();
        ResetTimer();
    }

    /// <summary>
    /// Override VehicleController so that the position update is done in Sumo.
    /// </summary>
	public override void Update()
    {
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
            return;
        }

        float distToNext = Vector3.Distance(transform.position, nextStation.transform.position);
        if (distToNext < line.stationRadius)
        {
            //Debug.Log(name + ": close to a station!");
            ArrivedAtNextStation();
        }
    }

    // Obsolete: should be taken care of by Update().
    //public override void ArrivedAtNextStation()
    //{
    //    base.ArrivedAtNextStation();
    //    //Debug.LogFormat("bus {0} arrived to {1}, heading {2}", this, currentStation, nextStation);


    //    //foreach (var list in disembarkersAtStation.Values)
    //    //{
    //    //    foreach (var traveler in list)
    //    //    {
    //    //        traveler.ArrivedAt(nextStation);
    //    //    }
    //    //    list.Clear();
    //    //}
    //}

    // Obsolete: should be taken care of in Update()
    //void OnDisable()
    //{
    //    ArrivedAtNextStation();
    //}
}
