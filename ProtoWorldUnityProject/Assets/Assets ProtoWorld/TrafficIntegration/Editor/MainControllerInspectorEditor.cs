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
 * MainControllerInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI in the MainController Inspector for communication with SUMO.
/// </summary>
[CustomEditor(typeof(SumoMainController))]
public class MainControllerInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SumoMainController script = (SumoMainController)target;

        if (GUILayout.Button("Run simulation"))
        {
           script.RunSimulation(); 
        }

        if (GUILayout.Button("Pause simulation"))
        {
            script.PauseSimulation();
        }

        if (GUILayout.Button("Run single step"))
        {
            script.RunSingleStep();
        }
    }
}