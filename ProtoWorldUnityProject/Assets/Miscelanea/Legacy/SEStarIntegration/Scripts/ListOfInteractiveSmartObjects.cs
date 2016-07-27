/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * SESTar Integration
 * ListOfInteractiveSmartObjects.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to keep track of all the SEStar SmartObject that are currently interactive in the simulation. The interactive SmartObjects are responsible of registering themselves in this data structure. 
/// </summary>
public class ListOfInteractiveSmartObjects : MonoBehaviour
{
    public Dictionary<string, GameObject> interactiveSmartObjects = new Dictionary<string, GameObject>();
}
