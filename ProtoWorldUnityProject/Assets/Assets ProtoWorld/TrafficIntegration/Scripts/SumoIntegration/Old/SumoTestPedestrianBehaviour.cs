/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * SUMO COMMUNICATION
 * SumoTestPedestrianBehaviour.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class defines the basic controller of a default pedestrian for testing the interaction
/// pedestrian-vehicle in Unity.
/// </summary>
public class SumoTestPedestrianBehaviour : MonoBehaviour
{
    public float speed = 5.0f;

    /// <summary>
    /// Updates the position of the pedestrian according to the arrow keys.
    /// </summary>
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position =
                new Vector3(transform.position.x, 
                    transform.position.y, transform.position.z + speed*Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position =
                new Vector3(transform.position.x,
                    transform.position.y, transform.position.z - speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position =
                new Vector3(transform.position.x + speed * Time.deltaTime, 
                    transform.position.y, transform.position.z);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position =
                new Vector3(transform.position.x - speed * Time.deltaTime, 
                    transform.position.y, transform.position.z);
        }
    }
}
