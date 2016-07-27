/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * SESTAR INTEGRATION
 * SEStarSyntheticEntity.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class identifies the generic parameters and methods of a SyntheticEntity placed in the Unity scene. Every SyntheticEntity in the Unity scene should have an instance of this class as a component. 
/// </summary>
public class SEStarSyntheticEntity : MonoBehaviour
{
    /// <summary>
    /// Type of the pedestrian.
    /// </summary>
    public string pedestrianType;

    /// <summary>
    /// Constructor of a Synthetic Entity. 
    /// </summary>
    /// <param name="pedestrianType">Type of the pedestrian.</param>
    SEStarSyntheticEntity(string pedestrianType)
    {
        pedestrianType = this.pedestrianType;
    }
}
