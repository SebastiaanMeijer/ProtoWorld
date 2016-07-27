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
 * SumoTrafficDB.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Implements the traffic DB generated from SUMO.
/// The traffic DB is composed of a list of <see cref=" TimeStepTDB"/>, each of them containing a list of <see cref="VehicleTDB"/>.
/// </summary>
/// <remarks>It inheritates <see cref="TrafficIntegrationData"/> to keep backward compatibility with the old SUMO module.</remarks>
public class SumoTrafficDB : TrafficIntegrationData { }