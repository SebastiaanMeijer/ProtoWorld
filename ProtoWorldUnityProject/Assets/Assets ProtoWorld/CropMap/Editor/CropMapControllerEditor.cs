/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * CROP MAP TOOL
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(CropMapController))]
public class CropMapControllerEditor : Editor
{
    // TODO: Better interaction with only left button.

    private static bool editing = false;
    private static bool gizmoEditing = false;
    private static int pointIndex;
    private static List<string> meshTags = new List<string> { "Line", "Building", "Area", "Water" };

    private CropMapController cmc;
    private MapBoundaries mb;

    void OnEnable()
    {
        mb = FindObjectOfType<MapBoundaries>();
    }

    public override void OnInspectorGUI()
    {

        cmc = target as CropMapController;
        if (!editing && cmc.CropAreaDefined())
            GUI.color = Color.green;
        else
            GUI.color = Color.gray;
        DrawDefaultInspector();

        GUILayout.Space(10);
        if (editing)
        {
            if (cmc.CropAreaDefined())
            {
                GUI.color = Color.green;
                GUILayout.Label("Press this button to Crop");
                if (GUILayout.Button("Crop Map"))
                {
                    if (mb != null)
                        cmc.CalculateLatLon(mb);

                    CropGameObjects();
                    editing = false;
                }
                GUILayout.Space(10);
                GUI.color = Color.white;
                GUILayout.Label("You can edit the point by dragging it");
            }
            else
            {
                GUI.color = Color.yellow;
                GUILayout.Label("Click on the scene to define crop area");
            }

            GUILayout.FlexibleSpace();
            GUI.color = Color.cyan;
            GUILayout.Label("Press this button to abort");
            if (GUILayout.Button("Abort"))
            {
                cmc.InitVertices();
                editing = false;
            }
        }
        else
        {
            GUI.color = Color.green;
            GUILayout.Label("Press this button to start");
            GUI.color = Color.white;
            if (GUILayout.Button("Start"))
            {
                ResetMap();
                cmc.InitVertices();
                editing = true;
            }
        }

        GUILayout.FlexibleSpace();
        GUI.color = Color.white;
        GUILayout.Label("Press this button to un-crop everything");
        GUI.color = Color.red;
        if (GUILayout.Button("Reset Map"))
        {
            cmc.InitVertices();
            ResetMap();
            editing = false;
        }
        GUI.color = Color.white;
        GUILayout.Label("Press this button to exit 'Crop Map'");
        GUI.color = Color.yellow;
        if (GUILayout.Button("Exit"))
        {
            editing = false;
            gizmoEditing = false;
            DestroyImmediate(cmc.gameObject);
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
                        worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        if (Physics.Raycast(worldRay, out hit))
                        {
                            pointIndex = cmc.GetVerticeIndex(hit.point);
                            if (pointIndex < 0)
                            {
                                cmc.SetFirstVertex(hit.point);
                            }
                            else
                            {
                                gizmoEditing = true;
                            }
                        }
                        GUIUtility.hotControl = contorlId;
                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        gizmoEditing = false;
                        pointIndex = -1;
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        break;
                    case EventType.MouseDrag:

                        worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        if (Physics.Raycast(worldRay, out hit))
                        {
                            if (gizmoEditing)
                            {
                                cmc.SetVertex(pointIndex, hit.point);
                            }
                            else
                            {
                                cmc.SetOppositeVertex(hit.point);
                            }
                        }

                        GUIUtility.hotControl = contorlId;
                        Event.current.Use();

                        break;
                }
            }
        }
    }

    [Obsolete("Code for cropping with polygon, archived for reference.")]
    void OnSceneGUI_Polygon()
    {
        if (editing)
        {
            var contorlId = GUIUtility.GetControlID(FocusType.Passive);
            Ray worldRay;
            RaycastHit hit;
            switch (Event.current.GetTypeForControl(contorlId))
            {
                case EventType.MouseDown:
                    gizmoEditing = true;
                    worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (Physics.Raycast(worldRay, out hit))
                    {
                        pointIndex = cmc.GetClosestPolygonPointIndex(hit.point);
                        if (pointIndex < 0)
                        {
                            pointIndex = cmc.polygon.Count;
                            cmc.AddPoint(hit.point);
                        }
                    }
                    GUIUtility.hotControl = contorlId;
                    Event.current.Use();
                    break;
                case EventType.MouseUp:
                    gizmoEditing = false;
                    pointIndex = -1;
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    break;
                case EventType.MouseDrag:
                    if (gizmoEditing)
                    {
                        worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        if (Physics.Raycast(worldRay, out hit))
                        {
                            cmc.polygon[pointIndex] = hit.point;
                        }

                        GUIUtility.hotControl = contorlId;
                        Event.current.Use();
                    }
                    break;
            }
        }
    }

    void ResetMap()
    {
        var filters = GetMeshFilters();

        foreach (var filterList in filters.Values)
        {
            bool foundcombinedMesh = false;
            foreach (var filter in filterList)
            {
                if (!meshTags.Contains(filter.tag))
                {
                    foundcombinedMesh = true;
                    break;
                }
            }
            if (foundcombinedMesh)
            {
                foreach (var filter in filterList)
                {
                    if (meshTags.Contains(filter.tag))
                    {
                        filter.gameObject.SetActive(false);
                    }
                    else
                    {
                        filter.transform.parent.gameObject.SetActive(true);
                        filter.gameObject.SetActive(true);
                        filter.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
            else
            {
                foreach (var filter in filterList)
                {
                    filter.transform.parent.gameObject.SetActive(true);
                    filter.gameObject.SetActive(true);
                    filter.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }

    void CropGameObjects()
    {
        var filters = GetMeshFilters();

        bool isInside = false;
        foreach (var key in filters.Keys)
        {
            var filterList = filters[key];
            float totalCount = filterList.Count;
            //Debug.Log(key + " : " + totalCount);
            float progress = 0f;
            var infoStr = "Cropping " + key + " objects...";
            foreach (var filter in filterList)
            {
                progress += 1f;
                if (EditorUtility.DisplayCancelableProgressBar(infoStr, "" , progress / totalCount))
                {
                    ResetMap();
                    EditorUtility.ClearProgressBar();
                    return;
                }

                if (meshTags.Contains(filter.tag))
                {
                    filter.transform.parent.gameObject.SetActive(true);
                    filter.gameObject.SetActive(true);
                    filter.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    filter.gameObject.SetActive(false);
                    continue;
                }

                var mesh = filter.sharedMesh;
                if (mesh == null)
                {
                    continue;
                }

                var pos = filter.transform.position;

                isInside = false;
                foreach (var v in mesh.vertices)
                {
                    if (ContainsPoint(cmc.GetVertices(), cmc.GetBounds(), v + pos))
                    {
                        isInside = true;
                        break;
                    }
                }
                filter.gameObject.SetActive(isInside);
            }
            EditorUtility.ClearProgressBar();
        }
    }

    Dictionary<string, List<MeshFilter>> GetMeshFilters(bool includeLines = true, bool includeBuildings = true, bool includeAreas = true, bool includeWaterAreas = true)
    {
        Dictionary<string, List<MeshFilter>> meshDictionary = new Dictionary<string, List<MeshFilter>>();

        if (includeLines)
        {
            var go = GameObject.Find("Lines");
            if (go != null)
                meshDictionary.Add("Line", new List<MeshFilter>(go.GetComponentsInChildren<MeshFilter>(true)));
        }
        if (includeBuildings)
        {
            var go = GameObject.Find("Buildings");
            if (go != null)
                meshDictionary.Add("Building", new List<MeshFilter>(go.GetComponentsInChildren<MeshFilter>(true)));
        }
        if (includeAreas)
        {
            var go = GameObject.Find("Areas");
            if (go != null)
                meshDictionary.Add("Area", new List<MeshFilter>(go.GetComponentsInChildren<MeshFilter>(true)));
        }
        if (includeAreas)
        {
            var go = GameObject.Find("WaterAreas");
            if (go != null)
                meshDictionary.Add("Water", new List<MeshFilter>(go.GetComponentsInChildren<MeshFilter>(true)));
        }
        return meshDictionary;
    }

    float[] GetBounds(List<Vector3> polygon)
    {
        float minX = polygon[0].x;
        float maxX = polygon[0].x;
        float minZ = polygon[0].z;
        float maxZ = polygon[0].z;
        for (int i = 1; i < polygon.Count; i++)
        {
            var q = polygon[i];
            minX = Math.Min(q.x, minX);
            maxX = Math.Max(q.x, maxX);
            minZ = Math.Min(q.z, minZ);
            maxZ = Math.Max(q.z, maxZ);
        }
        return new float[] { minX, maxX, minZ, maxZ };
    }

    bool ContainsPoint(List<Vector3> polygon, float[] bounds, Vector3 point)
    {
        if (point.x < bounds[0] || point.x > bounds[1] || point.z < bounds[2] || point.z > bounds[3])
        {
            return false;
        }

        int j = polygon.Count - 1;
        bool isInside = false;
        for (int i = 0; i < polygon.Count; j = i++)
        {
            //if ((polygon[i].z <= point.z && point.z < polygon[j].z) || (polygon[j].z <= point.z && point.z < polygon[i].z) &&
            //        (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
            if ((polygon[i].z > point.z) != (point.z < polygon[j].z) &&
                point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x)
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }


}
