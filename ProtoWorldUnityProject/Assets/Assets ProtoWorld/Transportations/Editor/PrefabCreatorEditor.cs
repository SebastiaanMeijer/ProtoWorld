/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PrefabCreatorEditor : Editor
{
    private static bool __editMode = false; // When we are in edit mode (Raycast mode)
    //private static int __count = 0; // Counter of the placed items (Not really used right now)
    private GameObject stationCreatorObject; // GameObject containing the group of the anchorPoints group.
    private int selectedType = 0;
    
    private static List<string> visiblePrefabs;
    private static Dictionary<string, string> prefabs;
    private static string filterString = "";

    void OnEnable()
    {
        string[] assetsPaths = AssetDatabase.GetAllAssetPaths();
        prefabs = new Dictionary<string, string>();
        visiblePrefabs = new List<string>();
        int count = 0;
        foreach (var item in assetsPaths)
        {
            if (item.Contains(".prefab"))
            {

                prefabs.Add(count++ + ")" + System.IO.Path.GetFileNameWithoutExtension(item), item);

            }
            //Debug.Log(System.IO.Path.GetFileNameWithoutExtension(item));
        }

    }
    void OnSceneGUI()
    {
        // If we are in edit mode and the user clicks (right click, middle click or alt+left click)
        if (__editMode)
        {
            //Debug.Log("edit mode");
            if (Event.current.type == EventType.MouseUp)
            {
                Debug.Log("mouse up");

                // Shoot a ray from the mouse position into the world
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;
                // Shoot this ray. check in a distance of 10000. 
                if (Physics.Raycast(worldRay, out hitInfo, 10000))
                {
                    // Load the current prefab

                    var path = prefabs[visiblePrefabs[selectedType]];
                    GameObject anchor_point = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    Debug.Log(anchor_point);
                    // Instance this prefab
                    GameObject prefab_instance = PrefabUtility.InstantiatePrefab(anchor_point) as GameObject;

                    // Place the prefab at correct position (position of the hit).
                    prefab_instance.transform.position = hitInfo.point;
                    prefab_instance.transform.parent = stationCreatorObject.transform;
                    // Mark the instance as dirty because we like dirty
                    //EditorUtility.SetDirty(prefab_instance);

                    Undo.RegisterCreatedObjectUndo(prefab_instance, "Prefab instantiated...");

                }
            }
            // Mark the event as used
            Event.current.Use();
        } // End if __editMode
    } // End OnSceneGUI

    public override void OnInspectorGUI()
    {
        GUILayout.Space(5);
        GUILayout.Label("Choose a word to filter.");
        filterString = GUILayout.TextField(filterString, 25);

        visiblePrefabs.Clear();
        foreach (var item in prefabs.Keys)
        {
            if (filterString.Length > 0)
            {
                if (item.Contains(filterString))
                {
                    visiblePrefabs.Add(item);
                }
            }
            else
            {
                visiblePrefabs.Add(item);
            }
        }

        GUILayout.Space(5);
        // Toggle edit mode
        if (__editMode)
        {// If we are in editing mode, make the button green and change the label
            GUI.color = Color.green;
            if (GUILayout.Button("Disable Editing")) { __editMode = false; }
        }
        else
        {
            GUI.color = Color.white; // Normal color if w're not in editing mode
            if (GUILayout.Button("Enable Editing"))
            {
                __editMode = true;
                // Get the objectGroup (Active selection)
                stationCreatorObject = Selection.activeGameObject;
            }
        }
        GUI.color = Color.white;
        GUILayout.Label("Choose a type to place.");
        // Create a selection grid where the user can select the current type
        selectedType = GUILayout.SelectionGrid(selectedType, visiblePrefabs.ToArray(), 1);

    }
}