/*
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

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Add or Edit/Add Essentials", false, 0)]
    static void AddEssentialsModuleToScene()
    {
        ClearEssentialsModule();

        AddModuleIfNotExist(navModuleName);
        AddModuleIfNotExist(scenModuleName);
    }

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Clear", false, 0)]
    static void ClearEssentialsModule()
    {
        DestroyImmediate(GameObject.Find("Main Camera"));
        DestroyImmediate(GameObject.Find("Directional Light"));

        DestroyImmediate(GameObject.Find(navModuleName));
        DestroyImmediate(GameObject.Find(scenModuleName));
    }

    [MenuItem("ProtoWorld Editor/Crop Map", false, 5)]
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

    [MenuItem("ProtoWorld Editor/Pedestrian Module/Add or Edit/Add or Edit Pedestrian Points", false, 2)]
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

    [MenuItem("ProtoWorld Editor/Pedestrian Module/Clear", false, 2)]
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

    [MenuItem("ProtoWorld Editor/Drag and Drop Module/Add or Edit/Add Module", false, 4)]
    static void AddDDModule()
    {
        AddModuleIfNotExist(ddModuleName);
    }

    [MenuItem("ProtoWorld Editor/Drag and Drop Module/Clear", false, 4)]
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

    [MenuItem("ProtoWorld Editor/KPI Module/Add or Edit/Add Chart", false, 5)]
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

    [MenuItem("ProtoWorld Editor/KPI Module/Clear", false, 5)]
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

    //[MenuItem("ProtoWorld Editor/Sumo Communication Module/Add or Edit/Add Module", false, 6)]
    //static void AddSumoComModule()
    //{
    //    AddModuleIfNotExist(sumoComModuleName);
    //}

    //[MenuItem("ProtoWorld Editor/Sumo Communication Module/Clear", false, 6)]
    //static void ClearSumoCom()
    //{
    //    var option = EditorUtility.DisplayDialogComplex(
    //        "Drag and Drop",
    //        "Do you want to remove " + sumoComModuleName,
    //        "Clear",
    //        "Do not clear",
    //        "Cancel");

    //    switch (option)
    //    {
    //        case 0:
    //            DestroyImmediate(GameObject.Find(sumoComModuleName));
    //            Debug.Log("The module is removed.");
    //            return;
    //        default:
    //            Debug.Log("User cancelled the operation.");
    //            return;
    //    }
    //}

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Add or Edit/Add Module", false, 6)]
    static void AddTrafficIntegModule()
    {
        AddModuleIfNotExist(traffIntegModuleName);
    }

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Clear", false, 6)]
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

    [MenuItem("ProtoWorld Editor/Decision Tree Module/Add or Edit/Add Module", false, 7)]
    static void AddDecisionTreeModule()
    {
        AddModuleIfNotExist(decisionTreeModuleName);
    }

    [MenuItem("ProtoWorld Editor/Decision Tree Module/Clear", false, 7)]
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

    [MenuItem("ProtoWorld Editor/Transportation Module/Add or Edit/Add or Edit Station", false, 1)]
    static void EditStation()
    {
        AddModuleIfNotExist(transModuleName);

        StationCreator creator = StationCreator.AttachToGameObject();

        Selection.activeGameObject = creator.gameObject;
    }

    [MenuItem("ProtoWorld Editor/Transportation Module/Add or Edit/Add or Edit Line", false, 1)]
    static void EditLine()
    {
        AddModuleIfNotExist(transModuleName);

        TransLineCreator creator = TransLineCreator.AttachToGameObject();

        Selection.activeGameObject = creator.gameObject;
    }

    [MenuItem("ProtoWorld Editor/Transportation Module/Clear", false, 1)]
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


    //[MenuItem("ProtoWorld Editor/Transportation Module/Load or Save/Load Stations And Lines XML", false, 1)]
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

    //[MenuItem("ProtoWorld Editor/Transportation Module/Load or Save/Load Sumo Bus Lines", false, 1)]
    static void LoadSumoData()
    {
        AddModuleIfNotExist(transModuleName);
        var path = EditorUtility.OpenFilePanel("Load Sumo Data", "Assets/Transportations", "");

        if (path.Length == 0)
        {
            EditorUtility.DisplayDialog("Loading Cancelled", "No file was provided", "OK");
            return;
        }
        //ClearAll();
        SumoContainer.LoadSumoCFG(path);

    }

    //[MenuItem("ProtoWorld Editor/Transportation Module/Load or Save/Save Stations and Lines XML", false, 1)]
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

    //[MenuItem("ProtoWorld Editor/Pedestrian Module/", false, 2)]
    //[MenuItem("ProtoWorld Editor/Transportation Module/", false, 3)]
    //[MenuItem("ProtoWorld Editor/Drag and Drop Module/", false, 4)]
    //[MenuItem("ProtoWorld Editor/KPI Module/", false, 5)]
    //[MenuItem("ProtoWorld Editor/Sumo Communication Module/", false, 6)]

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
            Debug.Log(path);
            if (path.Contains(".prefab"))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                Debug.Log(fileName);
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
