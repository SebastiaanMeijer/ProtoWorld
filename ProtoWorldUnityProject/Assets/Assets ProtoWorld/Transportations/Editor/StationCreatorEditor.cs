/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// This class read existing stations with tag "TransStation"
/// </summary>
[CustomEditor(typeof(StationCreator))]
public class StationCreatorEditor : Editor
{
    private StationCreator creator;
    private static bool editing = false;
    private static bool adding = false;
    private static string newStationName = "";

    void OnEnable()
    {
        creator = target as StationCreator;
        //creator.SetStations(GameObject.FindGameObjectsWithTag("TransStation"));
        creator.SetStations(FindObjectsOfType<StationController>());
    }

    public override void OnInspectorGUI()
    {
        GUI.color = Color.gray;
        DrawDefaultInspector();
        creator = target as StationCreator;
        GUILayout.Space(10);

        if (editing)
        {
            if (creator.HasEditingStation)
            {
                GUI.color = Color.white;
                GUILayout.Label("Enter the name of the station");
                newStationName = GUILayout.TextField(newStationName);
                creator.SetEditStationName(newStationName);

                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                GUILayout.Label("Press this to remove the station");
                if (GUILayout.Button("Remove"))
                {
                    creator.RemoveStation();
                    editing = false;
                    adding = false;
                }

                if (!adding)
                {
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.white;
                    GUILayout.Label("Press this to undo changes");
                    if (GUILayout.Button("Undo"))
                    {
                        creator.RevertEditStation();
                        newStationName = creator.GetEditStationName();
                    }
                }

                GUILayout.FlexibleSpace();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Save and go back"))
                {
                    creator.ReleaseEditStation();
                    editing = false;
                    adding = false;
                }
            }
            else
            {
                GUI.color = Color.green;
                GUILayout.Label("Click on a station to edit,");
                GUILayout.Label("or click somewhere else to create a station...");

                GUILayout.FlexibleSpace();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Back"))
                {
                    creator.ReleaseEditStation();
                    editing = false;
                    adding = false;
                }
            }

        }
        else
        {
            GUI.color = Color.green;
            GUILayout.Label("Press this button to start");
            if (GUILayout.Button("Start editing"))
            {
                editing = true;
                newStationName = creator.GetEditStationName();
            }

            GUILayout.FlexibleSpace();

            GUI.color = Color.white;
            GUILayout.Label("Press this button to exit 'Station Creator'");
            GUI.color = Color.yellow;
            if (GUILayout.Button("Exit"))
            {
                editing = false;
                DestroyImmediate(creator);
            }
        }
    }

    void OnSceneGUI()
    {
        if (editing)
        {
            var contorlId = GUIUtility.GetControlID(FocusType.Passive);
            Ray worldRay;
            RaycastHit hit;
            if (Event.current.button == 0)
            {
                switch (Event.current.GetTypeForControl(contorlId))
                {
                    case EventType.MouseDown:

                        if (!creator.HasEditingStation)
                        {
                            worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                            if (Physics.Raycast(worldRay, out hit))
                            {
                                // Must use SetEditStation or moving the position will be erroneous
                                creator.SetEditStation(creator.GetClosestStationTo(hit));
                                if (!creator.HasEditingStation)
                                {
                                    // Must use SetEditStation or moving the position will be erroneous
                                    creator.SetEditStation(creator.AddNewStation(hit.point));
                                    adding = true;
                                }
                                else
                                {
                                    newStationName = creator.GetEditStationName();
                                }
                            }
                            Event.current.Use();
                        }
                        GUIUtility.hotControl = contorlId;
                        break;
                    case EventType.MouseUp:
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        break;
                    case EventType.MouseDrag:
                        if (creator.HasEditingStation)
                        {
                            worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                            if (Physics.Raycast(worldRay, out hit))
                            {
                                //Debug.Log(hit.collider.gameObject);
                                creator.MoveEditingStation(hit.point);
                            }
                            Event.current.Use();
                        }
                        GUIUtility.hotControl = contorlId;
                        break;
                }
            }
        }
    }

}
