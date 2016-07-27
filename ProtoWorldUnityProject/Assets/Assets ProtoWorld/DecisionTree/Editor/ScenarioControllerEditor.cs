/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class ScenarioControllerEditor : MonoBehaviour 
{
    [MenuItem("ProtoWorld Editor/Decision Tree Module/Advance/Scenarios/Read Tree (.xml)")]
    public static void ReadTree()
    {
        var path = EditorUtility.OpenFilePanel("Load Tree Data", "", "xml");

        if (path.Length == 0)
        {
            EditorUtility.DisplayDialog("Loading Cancelled", "No file was provided", "OK");
            return;
        }
        var tree = DecisionTree.Load(path);

        var doTest = true;

        if (doTest)
        {
            tree.WriteToDebug();
            var sc = tree.GetFirstScenario();
            Debug.Log(sc.FindSimulationPath());
            Debug.Log(" Time 0: " + sc.FindNextEventIndex(0));
            Debug.Log(" Time 1: " + sc.FindNextEventIndex(1));
            sc = tree.GetScenario(3);
            if (sc != null)
            {
                Debug.Log(sc.FindSimulationPath());
                Debug.Log(" Time 0: " + sc.FindNextEventIndex(0));
                Debug.Log(" Time 1: " + sc.FindNextEventIndex(1));
                Debug.Log(" Time 5.5: " + sc.FindNextEventIndex(5.5f));
                Debug.Log(" Time 30: " + sc.FindNextEventIndex(30));
                Debug.Log(" Time 31: " + sc.FindNextEventIndex(31));
            }
        }
    }
}
