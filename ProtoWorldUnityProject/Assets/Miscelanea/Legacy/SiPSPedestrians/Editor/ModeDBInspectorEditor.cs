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
 * ModeDBInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI in the ModeDB Inspector.
/// </summary>
[CustomEditor(typeof(ModeDB))]
public class ModeDBInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ModeDB script = (ModeDB)target;

        if (GUILayout.Button("Introduce 30 min delay in buses"))
        {
           script.DelayBuses(1800.0f); 
        }

        if (GUILayout.Button("Introduce 30 min delay in metros"))
        {
            script.DelayMetros(1800.0f);
        }

        if (GUILayout.Button("Cancel next bus"))
        {
            script.CancelNextBus();
        }

        if (GUILayout.Button("Cancel next metro"))
        {
            script.CancelNextMetro();
        }

        if (GUILayout.Button("Print metro schedule"))
        {
            script.PrintMetroSchedule();
        }

        if (GUILayout.Button("Print bus schedule"))
        {
            script.PrintBusSchedule();
        }
    }
}