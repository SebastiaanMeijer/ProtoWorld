/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * PROTOWORLD MENU
 * ProtoWorldMenu.cs
 * Johnson Ho
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Defines the ProtoWorld Editor menu. 
/// </summary>
public class ProtoWorldMenu : MonoBehaviour
{
    static string navModuleName = "NavigationModule";
    static string scenModuleName = "ScenarioModule";
    static string pedModuleName = "FlashPedestriansModule";
    static string transModuleName = "TransportationModule";
    static string ddModuleName = "DragAndDropModule";
    static string kpiModuleName = "KPIModule";
    static string sumoComModuleName = "SumoComFullModule";
    static string traffIntegModuleName = "TrafficIntegrationModule";
    static string decisionTreeModuleName = "DecisionTreeModule";
    static string kpiChartName = "KPIChart";
    static string loggerAssemblyName = "LoggerAssembly";
    static string vvisName = "VVisDataSQLiteModule";

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Add Essentials", false, 0)]
    static void AddEssentialsModuleToScene()
    {
        DestroyImmediate(GameObject.Find("Main Camera"));
        DestroyImmediate(GameObject.Find("Directional Light"));

        DestroyImmediate(GameObject.Find(navModuleName));
        DestroyImmediate(GameObject.Find(scenModuleName));

        AddModuleIfNotExist(navModuleName);
        AddModuleIfNotExist(scenModuleName);
    }

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Generate Map", false, 1)]
    static void MapGenerationTool()
    {
        GenerateOsmMapWindow window = (GenerateOsmMapWindow)EditorWindow.GetWindow(typeof(GenerateOsmMapWindow), true, "Map Generation Tool");
        window.Show();
    }

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Crop Map", false, 2)]
    static void AddCropMapToScene()
    {
        string objName = "MapCropping";
        var go = GameObject.Find(objName);
        if (go == null)
        {
            go = new GameObject(objName);
            go.AddComponent<CropMapController>();
        }
        Selection.activeGameObject = go;
    }

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Remove Module", false, 3)]
    static void ClearEssentialsModule()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "Logger Assembly",
            "Do you want to remove " + navModuleName + " and " + scenModuleName,
            "Clear",
            "Do not clear",
            "Cancel");
        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find("Main Camera"));
                DestroyImmediate(GameObject.Find("Directional Light"));
                DestroyImmediate(GameObject.Find(navModuleName));
                DestroyImmediate(GameObject.Find(scenModuleName));
                Debug.Log("The modules are removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }

    }

    [MenuItem("ProtoWorld Editor/VVIS Visualization Module/Add VVIS Module", false, 3)]
    static void AddVVIS()
    {
        AddModuleIfNotExist(vvisName);

        var obj = GameObject.FindObjectOfType<TimeController>();
        if (obj != null)
        {
            var rect = obj.GetComponent<RectTransform>();
            if (rect != null)
                rect.position = new Vector2(-150, rect.position.y);
        }

        var obj2 = GameObject.Find("CameraChangeUI");
        if (obj2 != null)
        {
            var rect = obj2.GetComponent<RectTransform>();
            if (rect != null)
                rect.position = new Vector2(435, rect.position.y);
        }
    }

    [MenuItem("ProtoWorld Editor/VVIS Visualization Module/Remove Module", false, 4)]
    static void ClearVVIS()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "VVIS Visualization Module",
            "Do you want to remove " + vvisName,
            "Clear",
            "Do not clear",
            "Cancel");
        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(vvisName));
                Debug.Log("The module is removed.");
                var obj = GameObject.FindObjectOfType<TimeController>();
                if (obj != null)
                {
                    var rect = obj.GetComponent<RectTransform>();
                    if (rect != null)
                        rect.position = new Vector2(150, rect.position.y);
                }

                var obj2 = GameObject.Find("CameraChangeUI");
                if (obj2 != null)
                {
                    var rect = obj2.GetComponent<RectTransform>();
                    if (rect != null)
                        rect.position = new Vector2(335, rect.position.y);
                }
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    [MenuItem("ProtoWorld Editor/Logger Module/Add Logger Assembly", false, 3)]
    static void AddLogger()
    {
        string name = "LoggerAssembly";
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            go = new GameObject(name);
        }
        var creator = go.GetComponent<LoggerAssembly>();
        if (creator == null)
            go.AddComponent<LoggerAssembly>();

        Selection.activeGameObject = go;
    }

    [MenuItem("ProtoWorld Editor/Logger Module/Remove Module", false, 4)]
    static void ClearLogger()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "Logger Assembly",
            "Do you want to remove " + loggerAssemblyName,
            "Clear",
            "Do not clear",
            "Cancel");
        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(loggerAssemblyName));
                Debug.Log("The module is removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    [MenuItem("ProtoWorld Editor/Pedestrian Module/Add or Edit Pedestrian Points", false, 2)]
    static void EditFlash()
    {
        string name = "FlashCreator";
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            go = new GameObject(name);
        }
        var creator = go.GetComponent<FlashCreator>();
        if (creator == null)
            go.AddComponent<FlashCreator>();

        Selection.activeGameObject = go;
    }

    [MenuItem("ProtoWorld Editor/Pedestrian Module/Remove Module", false, 2)]
    static void ClearFlash()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "Spawners and Destinations",
            "Do you want to remove " + pedModuleName,
            "Clear",
            "Do not clear",
            "Cancel");
        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(pedModuleName));
                Debug.Log("The module is removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    [MenuItem("ProtoWorld Editor/Drag and Drop Module/Add Drag and Drop Module", false, 4)]
    static void AddDDModule()
    {
        AddModuleIfNotExist(ddModuleName);
    }

    [MenuItem("ProtoWorld Editor/Drag and Drop Module/Remove Module", false, 4)]
    static void ClearDragAndDrop()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "Drag and Drop",
            "Do you want to remove " + ddModuleName,
            "Clear",
            "Do not clear",
            "Cancel");

        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(ddModuleName));
                Debug.Log("The module is removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    [MenuItem("ProtoWorld Editor/KPI Module/Add Chart", false, 5)]
    static void AddKPIChart()
    {
        AddModuleIfNotExist(kpiModuleName);

        var parent = GameObject.Find("KPICanvas");
        if (parent != null)
        {
            var chart = AddPrefabToScene(kpiChartName);
            chart.transform.SetParent(parent.transform);
            var rectTrans = chart.transform as RectTransform;
            // Move the chart to position 0,0,0.
            rectTrans.localPosition = Vector3.zero;
            // If the module is newly added, the scale somehow became 0,0,0 (?!)
            rectTrans.localScale = Vector3.one;
            Selection.activeGameObject = chart;
        }
    }

    [MenuItem("ProtoWorld Editor/KPI Module/Remove Module", false, 5)]
    static void ClearKPIModule()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "Drag and Drop",
            "Do you want to remove " + kpiModuleName,
            "Clear",
            "Do not clear",
            "Cancel");

        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(kpiModuleName));
                Debug.Log("The module is removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Add Traffic Integration Module", false, 6)]
    public static GameObject AddTrafficIntegModule()
    {
        return AddModuleIfNotExist(traffIntegModuleName);
    }

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Remove Module", false, 6)]
    static void ClearTrafficInteg()
    {
        var option = EditorUtility.DisplayDialogComplex(
    "Traffic Integration Module",
    "Do you want to remove " + traffIntegModuleName,
    "Clear",
    "Do not clear",
    "Cancel");

        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(traffIntegModuleName));
                Debug.Log("The module is removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    [MenuItem("ProtoWorld Editor/Decision Tree Module/Add Decision Tree Module", false, 7)]
    static void AddDecisionTreeModule()
    {
        AddModuleIfNotExist(decisionTreeModuleName);
    }

    [MenuItem("ProtoWorld Editor/Decision Tree Module/Remove Module", false, 7)]
    static void ClearDecisionTreeModule()
    {
        var option = EditorUtility.DisplayDialogComplex(
    "Decision Tree Module",
    "Do you want to remove " + decisionTreeModuleName,
    "Clear",
    "Do not clear",
    "Cancel");

        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(decisionTreeModuleName));
                Debug.Log("The module is removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    [MenuItem("ProtoWorld Editor/Public Transport Module/Add or Edit Station", false, 1)]
    static void EditStation()
    {
        AddModuleIfNotExist(transModuleName);

        StationCreator creator = StationCreator.AttachToGameObject();

        Selection.activeGameObject = creator.gameObject;
    }

    [MenuItem("ProtoWorld Editor/Public Transport Module/Add or Edit Line", false, 1)]
    static void EditLine()
    {
        AddModuleIfNotExist(transModuleName);

        TransLineCreator creator = TransLineCreator.AttachToGameObject();

        Selection.activeGameObject = creator.gameObject;
    }

    [MenuItem("ProtoWorld Editor/Public Transport Module/Remove Module", false, 1)]
    static void ClearTransportation()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "Stations and Lines",
            "Do you want to remove " + transModuleName,
            "Clear",
            "Do not clear",
            "Cancel");

        switch (option)
        {
            case 0:
                DestroyImmediate(GameObject.Find(transModuleName));
                Debug.Log("The module is removed.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    static void LoadStationsAndLines()
    {
        AddModuleIfNotExist(transModuleName);

        var path = EditorUtility.OpenFilePanel("Load XML Data", "Assets/Transportations", "xml");

        if (path.Length == 0)
        {
            EditorUtility.DisplayDialog("Loading Cancelled", "No file was provided", "OK");
            return;
        }

        ClearAll();

        // Load stations and lines from xml.
        var container = TrafficContainer.Load(path);
        // Add stations from container to the scene.
        StationCreator stationCreator = StationCreator.AttachToGameObject();
        stationCreator.SetStations(FindObjectsOfType<StationController>());
        // the station might get a new id, therefor using a dictionary to 
        // find the right stationController when creating line.
        Dictionary<int, StationController> idLookUp = new Dictionary<int, StationController>();
        foreach (var s in container.stations)
        {
            var station = stationCreator.AddNewStation(s.GetPoint(), s.name);
            idLookUp.Add(s.id, station);
        }
        DestroyImmediate(stationCreator);

        // Add lines from container to the scene.
        TransLineCreator lineCreator = TransLineCreator.AttachToGameObject();
        lineCreator.SetLines(GameObject.FindGameObjectsWithTag("TransLine"));
        foreach (var line in container.lines)
        {
            lineCreator.ResetEditingInfo();
            lineCreator.editLineName = line.name;
            lineCreator.lineCategory = line.GetCategory();

            foreach (var id in line.GetStationIds())
            {
                var station = idLookUp[id];
                lineCreator.AddStationToNewLine(station);
            }
            lineCreator.CreateNewLine();
        }
        DestroyImmediate(lineCreator);

        string stationStats = string.Format("{0} stations loaded to the scene.", container.stations.Count);
        string lineStats = string.Format("{0} lines loaded to the scene.", container.lines.Count);
        EditorUtility.DisplayDialog("Loading Finished", stationStats + "\n" + lineStats, "OK");
    }

    static void LoadSumoData()
    {
        AddModuleIfNotExist(transModuleName);
        var path = EditorUtility.OpenFilePanel("Load Sumo Data", "Assets/Transportations", "");

        if (path.Length == 0)
        {
            EditorUtility.DisplayDialog("Loading Cancelled", "No file was provided", "OK");
            return;
        }
        //Remove ModuleAll();
        SumoContainer.LoadSumoCFG(path);

    }

    static void SaveStationsAndLines()
    {
        var sgos = GameObject.FindGameObjectsWithTag("TransStation");
        if (sgos.Length < 1)
        {
            Debug.Log("no stations to save.");
            return;
        }
        var lgos = GameObject.FindGameObjectsWithTag("TransLine");
        if (lgos.Length < 1)
        {
            Debug.Log("no lines to save.");
            return;
        }

        var path = EditorUtility.SaveFilePanel("Save XML Data", "Assets/Transportations", "", "xml");

        if (path.Length == 0)
        {
            EditorUtility.DisplayDialog("Saving Cancelled", "No file was provided", "OK");
            return;
        }

        List<BaseStation> stations = new List<BaseStation>();
        foreach (var go in sgos)
        {
            var sc = go.GetComponent<StationController>();
            var bs = new BaseStation
            {
                id = int.Parse(sc.name),
                x = go.transform.position.x,
                y = go.transform.position.y,
                z = go.transform.position.z,
                name = sc.stationName,
            };
            stations.Add(bs);
        }

        List<BaseLine> lines = new List<BaseLine>();
        foreach (var go in lgos)
        {
            var lc = go.GetComponent<LineController>();
            var bl = LineController.CreateBaseLine(lc);
            lines.Add(bl);
        }

        var container = new TrafficContainer();
        container.stations = stations;
        container.lines = lines;
        container.Save(path);
        string stationStats = string.Format("{0} stations saved.", container.stations.Count);
        string lineStats = string.Format("{0} lines saved.", container.lines.Count);
        EditorUtility.DisplayDialog("Saving Finished", stationStats + "\n" + lineStats + "\n to: " + path, "OK");
    }

    static void ClearAll()
    {
        var option = EditorUtility.DisplayDialogComplex(
            "Stations and Lines",
            "Do you want to clear existing stations and lines?",
            "Clear",
            "Do not clear",
            "Cancel");

        switch (option)
        {
            case 0:
                ClearStations();
                ClearLines();
                Debug.Log("Stations and lines are cleared.");
                return;
            default:
                Debug.Log("User cancelled the operation.");
                return;
        }
    }

    static void ClearLines()
    {
        var go = GameObject.FindGameObjectsWithTag("TransLine");
        for (int i = 0; i < go.Length; i++)
        {
            GameObject.DestroyImmediate(go[i]);
        }
    }

    static void ClearStations()
    {
        var go = GameObject.FindGameObjectsWithTag("TransStation");
        for (int i = 0; i < go.Length; i++)
        {
            GameObject.DestroyImmediate(go[i]);
        }
    }

    public static GameObject AddModuleIfNotExist(string moduleName)
    {
        var go = GameObject.Find(moduleName);
        if (go == null)
        {
            go = AddPrefabToScene(moduleName);
            if (go == null)
                Debug.Log("There is no " + moduleName);
        }
        PrefabUtility.DisconnectPrefabInstance(go);
        // NB: Do not set activeGameObject = go, otherwise all the CreatorEditors will not work.
        return go;
    }

    public static GameObject AddPrefabToScene(string prefabName)
    {
        GameObject instance = null;

        bool foundPrefab = false;
        var guids = AssetDatabase.FindAssets(prefabName);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains(".prefab"))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (fileName.Equals(prefabName))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    PrefabUtility.DisconnectPrefabInstance(instance);
                    Undo.RegisterCreatedObjectUndo(instance, "Prefab instantiated...");
                    Debug.Log(prefabName + " was added to the scene.");
                    foundPrefab = true;
                }
            }
        }
        if (!foundPrefab)
            Debug.Log("There is no prefab: " + prefabName);

        return instance;
    }
}
