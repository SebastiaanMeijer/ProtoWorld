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
 * SEStarSmartObject.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class identifies the generic parameters and methods of a SmartObject placed in the Unity scene. Every SmartObject in the Unity scene should have an instance of this class as a component. 
/// </summary>
public class SEStarSmartObject : MonoBehaviour
{    
    /// <summary>
    /// Name of the object.
    /// </summary>
    public string objectName { get; set; }

    /// <summary>
    /// Id of the object.
    /// </summary>
    public uint objectId { get; set; }

    /// <summary>
    /// Type of the object.
    /// </summary>
    public uint objectType { get; set; }

    /// <summary>
    /// Constructor of a Smart Object. 
    /// </summary>
    /// <param name="objectId">Id of the smartObject.</param>
    SEStarSmartObject(uint objectId)
    {
        objectId = this.objectId;
    }
}
