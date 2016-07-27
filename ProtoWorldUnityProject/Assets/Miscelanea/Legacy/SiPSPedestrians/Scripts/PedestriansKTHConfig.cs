/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * PEDESTRIANS KTH
 * PedestriansKTHConfig.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Sets the configuration parameters for the pedestrian simulation. 
/// </summary> 
public class PedestriansKTHConfig : MonoBehaviour 
{
    [Range(0, 1000)]
    public int numberOfPedestrians = 100;
    
    [Range(0.1f, 1.0f)]
    public float speedVariability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float studyInterestVariability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float subscriptionProbability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float rumourSusceptibilityVariability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float delayToleranceVariability = 0.6f;
    
    [Range(0.1f,1.0f)]
    public float spawningFrequency = 0.5f;

    public bool useCoordinateConversion = true;
    public bool showFloatingBalloons = true;
    public bool useDatabaseForSpawning = true;
    public bool spawnOnlyDefaultModel = true;
    public bool sendInfoToPedestriansSuscribed = true;
}
