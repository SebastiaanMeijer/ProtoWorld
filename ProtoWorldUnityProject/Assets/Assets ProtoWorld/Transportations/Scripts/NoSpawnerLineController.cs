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

/// <summary>
/// Controller to accomodate Sumo vehicles that needs to carry traveler in Unity-scene.
/// </summary>
public class NoSpawnerLineController : LineController
{
    //List<GameObject> toBeRemoved = new List<GameObject>();

    /// <summary>
    /// Assuming that the vechicle only has 2 stops.
    /// SumoTrafficSpawner will set gameobject to false once it reached its destination,
    /// destination is the end-station so ArrivedAtNextStation() should be called.
    /// </summary>
    //public override void LateUpdate()
    //{
        
    //    foreach (var child in vehicles)
    //    {
    //        if (child.gameObject.activeInHierarchy == false)
    //        {
    //            child.gameObject.SetActive(true);
    //            child.GetComponent<VehicleController>().ArrivedAtNextStation();
    //            child.gameObject.SetActive(false);
    //            toBeRemoved.Add(child);
    //        }
    //    }
    //    foreach (var go in toBeRemoved)
    //    {
    //        vehicles.Remove(go);
    //    }
    //    toBeRemoved.Clear();
    //}
}
