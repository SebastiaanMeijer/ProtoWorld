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
using System.Collections.Generic;

[CustomEditor(typeof(TransLineCreator))]
public class TransLineCreatorEditor : Editor
{
    private TransLineCreator creator;

    private static bool editing = false;
    private static bool creating = false;
    bool groupEnabled;

    void OnEnable()
    {
        creator = target as TransLineCreator;
        creator.SetLines(GameObject.FindGameObjectsWithTag("TransLine"));
        creator.SetStations(FindObjectsOfType<StationController>());
        creator.mainRouter = FindObjectOfType<RoutingController>();
    }

    public override void OnInspectorGUI()
    {
        GUI.color = Color.gray;
        DrawDefaultInspector();
        creator = target as TransLineCreator;
        GUILayout.Space(5);

        if (editing || creating)
        {
            if (creating)
            {
                GUI.color = Color.white;
                GUILayout.Label("Enter the name of the new line");
                creator.editLineName = GUILayout.TextField(creator.editLineName);
                creator.lineCategory = (LineCategory)EditorGUILayout.EnumPopup("Line Category:", creator.lineCategory);

                GUILayout.Space(5);
                GUI.color = Color.green;
                GUILayout.Label("Add a station by clicking on one...");
                GUILayout.Space(5);

                GUI.color = Color.white;
                StationsEditor();

                if (creator.editingLineStations.Count > 1)
                {
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.green;
                    if (GUILayout.Button("Create"))
                    {
                        creator.CreateNewLine();
                        
                        creator.ResetEditingInfo();
                        creating = false;
                    }
                }

                GUILayout.FlexibleSpace();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Back"))
                {
                    
                    creator.ResetEditingInfo();
                    creating = false;
                }
            }


            if (editing)
            {
                GUI.color = Color.white;
                if (creator.editingLine == null)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("Lines in this scene:");
                    
                    foreach (var line in creator.lines.Values)
                    {
                        
                        GUI.backgroundColor = line.lineColor;
                        if (GUILayout.Button(line.lineName))
                        {
                            creator.editingLine = line;
                            creator.PrepareLineForEdit(line);
                        }
                    }
 
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.yellow;
                    GUI.backgroundColor = Color.yellow;
                    if (GUILayout.Button("Back"))
                    {
                        
                        creator.ResetEditingInfo();
                        editing = false;
                    }
                }
                else
                {
                    GUI.color = Color.white;
                    GUILayout.Label("Enter the name of the new line");
                    creator.editLineName = GUILayout.TextField(creator.editLineName);
                    creator.lineCategory = (LineCategory)EditorGUILayout.EnumPopup("Line Category:", creator.lineCategory);

                    GUILayout.Space(10);
                    GUILayout.Label("Stations in this line:");
                    StationsEditor();

                    GUILayout.FlexibleSpace();
                    GUI.color = Color.cyan;
                    if (GUILayout.Button("Update Line"))
                    {
                        creator.SaveEditingLine();
                        creator.ResetEditingInfo();
                    }
                    GUILayout.Space(10);
                    GUI.color = Color.red;
                    if (GUILayout.Button("Remove this line"))
                    {
                        creator.RemoveEditLine();
                        creator.ResetEditingInfo();
                        //editing = false;
                    }

                    GUILayout.FlexibleSpace();
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("Back"))
                    {
                        creator.ResetEditingInfo();
                    }
                }
            }
        }
        else
        {
            WalkingDistanceEditor();

            GUI.color = Color.white;
            GUILayout.Label("Choose one of the alternatives to start:");
            GUI.color = Color.green;
            if (GUILayout.Button("Create new line"))
            {
                creator.ResetEditingInfo();
                creating = true;
            }
            GUILayout.Space(10);
            GUI.color = Color.cyan;
            if (GUILayout.Button("Edit existing line"))
            {
                creator.ResetEditingInfo();
                editing = true;
            }
            GUILayout.FlexibleSpace();

            GUI.color = Color.white;
            GUILayout.Label("Press this button to exit 'Transportation Line Creator'");
            GUI.color = Color.yellow;
            if (GUILayout.Button("Exit"))
            {
                editing = false;
                DestroyImmediate(creator);
            }
        }

    }

    void WalkingDistanceEditor()
    {
        GUI.color = Color.white;
        EditorGUILayout.BeginVertical("box");
        var label = new GUIContent("Walking distance", "Enable this to adjust the walking distance.");
        creator.showWalkingDistance = EditorGUILayout.BeginToggleGroup(label, creator.showWalkingDistance );
        creator.mainRouter.walkingDistance = EditorGUILayout.Slider("value:", creator.mainRouter.walkingDistance, 1, 2000);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndVertical();
    }

    void StationsEditor()
    {
        StationController toBeRemoved = null;
        for (int i = 0; i < creator.editingLineStations.Count; i++)
        {
            if (GUILayout.Button(creator.editingLineStations[i].GetIdAndName()))
            {
                toBeRemoved = creator.editingLineStations[i];
            }
            if (creator.travelTimes.Count > 0 && i < creator.travelTimes.Count)
                creator.travelTimes[i] = EditorGUILayout.FloatField("travel time [sec]:", creator.travelTimes[i]);
        }
        if (toBeRemoved != null)
        {
            creator.RemoveStationFromNewLine(toBeRemoved);
            toBeRemoved = null;
        }
    }

    void OnSceneGUI()
    {
        if (creating || editing)
        {
            var contorlId = GUIUtility.GetControlID(FocusType.Passive);
            Ray worldRay;
            RaycastHit hit;
            if (Event.current.button == 0)
            {
                switch (Event.current.GetTypeForControl(contorlId))
                {
                    case EventType.MouseDown:

                        worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        if (Physics.Raycast(worldRay, out hit))
                        {
                            var station = creator.GetClosestStationTo(hit);
                            //Debug.Log(creator.GetStationName(station));
                            creator.AddStationToNewLine(station);

                        }
                        Event.current.Use();
                        GUIUtility.hotControl = contorlId;
                        break;
                    case EventType.MouseUp:
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        break;
                }
            }
        }
    }



}



