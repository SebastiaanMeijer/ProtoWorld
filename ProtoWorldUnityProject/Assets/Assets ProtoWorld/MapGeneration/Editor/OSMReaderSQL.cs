/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * GAPSLABS EXTENDED EDITOR
 * Aram Azhari
 * 
 * Reviewed by Miguel Ramos Carretero
 * 
 */

#define SHOW_STUFF

using System.Collections;
using System.Collections.Generic;
//using System.Data.OleDb;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Threading;
using Aram.OSMParser;
using GapslabWCFservice;
using UnityEditor;
using UnityEngine;

public class OSMReaderSQL : Editor
{
    /// -----------------------------------------------------------------------
    /// MENU ITEMS FOR GENERATING ELEMENTS 
    /// -----------------------------------------------------------------------

    //[MenuItem("ProtoWorld Editor/Scenario/Generate Roads and Buildings", false, 1)]
    static public void Create2ServerSide()
    {
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        CalculateBoundaries(mapproperties);
        ImportOSMDataSQL_ServerSide(true, true, true);
        Vector3 scale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        GroupObjectsWithTags("Lines", new string[] { "Line" }, scale, false, true);
        GroupObjectsWithTags("Buildings", new string[] { "Building" }, scale, false, true);
        GroupObjectsWithTags("Areas", new string[] { "Area" }, scale, false, true);
        //GroupLines(false);
        AssignStaticLayers();
        //BakeNavigation();
    }

    //[MenuItem("ProtoWorld Editor/Scenario/Generate Buildings only", false, 1)]
    static public void Create2_2()
    {
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        CalculateBoundaries(mapproperties);
        ImportOSMDataSQL_ServerSide(true, false, true);
        Vector3 scale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        GroupObjectsWithTags("Buildings", new string[] { "Building" }, scale, false);
        //GroupBuildings();
        AssignStaticLayers();
        //BakeNavigation();
    }

    //[MenuItem("ProtoWorld Editor/Scenario/Generate Roads only", false, 1)]
    static public void Create2_3()
    {
        PrepareForCombining.TurnRoadCollidersOff();
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        CalculateBoundaries(mapproperties);
        ImportOSMDataSQL_ServerSide(true, true, false);
        Vector3 scale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        GroupObjectsWithTags("Lines", new string[] { "Line" }, scale, false, true);
        //GroupRoads();
        AssignStaticLayers();
        //BakeNavigation();
    }

    //[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advance/Generate water areas")]
    static public void CreateWaterAreas()
    {
        var go = GameObject.Find("AramGISBoundaries");
        var properties = go.GetComponent<MapBoundaries>();
        string[][] tags = new string[1][];
        tags[0] = new string[] { "natural", "water" };
        ImportOSMDataSQL_ArbitraryShapeFromWay(tags, "Water", "Water", properties.WaterMaterial.name, 0f);
        GroupObjectsWithTags("Water Areas", new string[] { "Water" }, Vector3.zero, false, true);
        //AddBusStopLogic();
    }

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Clean map", false, 3)]
    static void CleanScene()
    {
        var gos = GameObject.FindGameObjectsWithTag("Line");
        var gos2 = GameObject.FindGameObjectsWithTag("Building");
        var gos3 = GameObject.FindGameObjectsWithTag("Area");
        var gos4 = GameObject.FindGameObjectsWithTag("Water");
        var gos5 = GameObject.FindGameObjectsWithTag("BusStop");
        var gos6 = GameObject.FindGameObjectsWithTag("TrafficLight");
        if (EditorUtility.DisplayDialog("Deleting roads", "There are a total of " + (gos.Length + gos2.Length + gos3.Length + gos4.Length + gos5.Length + gos6.Length) + " elements.\nAre you sure you want to delete them?", "Yes", "No"))
        {
            foreach (var g in gos)
                GameObject.DestroyImmediate(g);
            foreach (var g in gos2)
                GameObject.DestroyImmediate(g);
            foreach (var g in gos3)
                GameObject.DestroyImmediate(g);
            foreach (var g in gos4)
                GameObject.DestroyImmediate(g);
            foreach (var g in gos5)
                GameObject.DestroyImmediate(g);
            foreach (var g in gos6)
                GameObject.DestroyImmediate(g);

            GameObject.DestroyImmediate(GameObject.Find("Lines"));
            GameObject.DestroyImmediate(GameObject.Find("Buildings"));
            GameObject.DestroyImmediate(GameObject.Find("Areas"));
            GameObject.DestroyImmediate(GameObject.Find("Bus stops"));
            GameObject.DestroyImmediate(GameObject.Find("Traffic Lights"));
            GameObject.DestroyImmediate(GameObject.Find("Water Areas"));

        }
    }

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advance/Generate roads with filter")]
    static void Create2_4()
    {
        GenerateOSMRoadsWithFilterWindow window = (GenerateOSMRoadsWithFilterWindow)EditorWindow.GetWindow(typeof(GenerateOSMRoadsWithFilterWindow), true, "Road generation with filters");
        window.Show();
    }

    [MenuItem("Edit/Clear Progress Bar")]
    static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }

    /// -----------------------------------------------------------------------
    /// MENU ITEMS NOT EXPOSED
    /// -----------------------------------------------------------------------

    //[MenuItem("ProtoWorld Editor/Scenario/Get Traffic lights from the database")]
    static void CreateTrafficLight()
    {
        string[][] tags = new string[1][];
        tags[0] = new string[] { "highway", "traffic_signals" };
        ImportOSMDataSQL_Billboard(tags, "TrafficLight", "TrafficLight", "TrafficLight", 0.5f);
        GroupObjectsWithTags("Traffic Lights", new string[] { "TrafficLight" }, Vector3.zero, true, true);
    }

    //[MenuItem("ProtoWorld Editor/Scenario/Bus stops from the database")]
    static void CreateBusStops()
    {
        string[][] tags = new string[2][];
        tags[0] = new string[] { "highway", "bus_stop" };
        tags[1] = new string[] { "bus", "yes" };
        ImportOSMDataSQL_Billboard(tags, "BusStop", "BusStop", "BusStopMaterial", 0.5f);
        GroupObjectsWithTags("Bus stops", new string[] { "BusStop" }, Vector3.zero, true, true);
        //AddBusStopLogic();
    }

    //[MenuItem("ProtoWorld Editor/Scenario/Generate Roads + Building Shapes (Basic)", false, 1)] -- Process not optimized
    static void Create2()
    {
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        ImportOSMDataSQL(true, true, true);
        Vector3 scale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        GroupObjectsWithTags("Lines", new string[] { "Line" }, scale, false, true);
        GroupObjectsWithTags("Buildings", new string[] { "Building" }, scale, false, true);
        //GroupLines(false);
        AssignStaticLayers();
    }

    //[MenuItem("Gapslabs Extended Editor/Update/Road materials")]
    static void ReAssignRoadMaterials()
    {
        var go = GameObject.Find("AramGISBoundaries");
        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            var gos = GameObject.FindGameObjectsWithTag("Line");
            var layerResidential = "residential";
            var layerService = "service";
            var layerFootway = "footway";
            var layerCycleway = "cycleway";
            var layerSteps = "steps";
            var layerPedestrian = "pedestrian";
            try
            {
                for (int i = 0; i < gos.Length; i++)
                {
                    if (gos[i].name.ToLower().EndsWith(layerResidential))
                        gos[i].GetComponent<Renderer>().sharedMaterial = mapboundaries.ResidentialMaterial;
                    else if (gos[i].name.ToLower().EndsWith(layerService))
                        gos[i].GetComponent<Renderer>().sharedMaterial = mapboundaries.ServiceMaterial;
                    else if (gos[i].name.ToLower().EndsWith(layerFootway))
                        gos[i].GetComponent<Renderer>().sharedMaterial = mapboundaries.FootWayMaterial;
                    else if (gos[i].name.ToLower().EndsWith(layerSteps))
                        gos[i].GetComponent<Renderer>().sharedMaterial = mapboundaries.StepsMaterial;
                    else if (gos[i].name.ToLower().EndsWith(layerPedestrian))
                        gos[i].GetComponent<Renderer>().sharedMaterial = mapboundaries.FootWayMaterial;
                    else if (gos[i].name.ToLower().EndsWith(layerCycleway))
                        gos[i].GetComponent<Renderer>().sharedMaterial = mapboundaries.CycleWayMaterial;
                }
            }
            catch (System.Exception ex)
            { Debug.LogException(ex); }
        }
    }

    //[MenuItem("Gapslabs Extended Editor/Update/Building Materials")]
    static void SetBuildingMaterial()
    {
        var go = GameObject.Find("AramGISBoundaries");
        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            var gos = GameObject.FindGameObjectsWithTag("Building");
            for (int i = 0; i < gos.Length; i++)
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Setting line widths", "Buildings\t" + i + "/" + gos.Length, i / (float)gos.Length))
                {
                    gos[i].GetComponent<Renderer>().sharedMaterial = mapboundaries.BuildingMaterial;
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            EditorUtility.ClearProgressBar();
        }
    }

    //[MenuItem("Gapslabs Extended Editor/Update/Line-Building Widths")]
    static void SetWidthAndColor()
    {
        var go = GameObject.Find("AramGISBoundaries");
        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            var gos = GameObject.FindGameObjectsWithTag("Building");
            for (int i = 0; i < gos.Length; i++)
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Setting line widths", "Buildings\t" + i + "/" + gos.Length, i / (float)gos.Length))
                {
                    if (gos[i].GetComponent<LineRenderer>() != null)
                    {
                        gos[i].GetComponent<LineRenderer>().SetWidth(mapboundaries.BuildingLineThickness, mapboundaries.BuildingLineThickness);
                        gos[i].GetComponent<LineRenderer>().SetColors(mapboundaries.BuildingColor, mapboundaries.BuildingColor);
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }

            var gos2 = GameObject.FindGameObjectsWithTag("Line");
            for (int i = 0; i < gos2.Length; i++)
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Setting line widths", "Roads\t" + i + "/" + gos2.Length, i / (float)gos.Length))
                {
                    gos2[i].GetComponent<LineRenderer>().SetWidth(mapboundaries.RoadLineThickness, mapboundaries.RoadLineThickness);
                    gos[i].GetComponent<LineRenderer>().SetColors(mapboundaries.BuildingColor, mapboundaries.BuildingColor);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            EditorUtility.ClearProgressBar();


        }
    }

    //[MenuItem("R&D/Reflection")]
    static void RD_Reflection()
    {
        try
        {
            // Notice the type that is passed in this line is not necessarily the type we are looking for.
            // It is only important for us to get the assembly. This is useful when we are trying to access
            // an internal / private or a sealed class.
            System.Reflection.Assembly design = System.Reflection.Assembly.GetAssembly(typeof(UnityEngine.GUI));
            var assetstore = design.GetType("UnityEngine.GameObject");
            var methods = assetstore.GetMethods();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(assetstore.Name);
            foreach (var m in methods)
                sb.AppendLine((m.IsPublic ? "Public" : "Private") + " " + (m.IsStatic ? "static " : "") + m.ReturnType + " " + m.Name + "()");
            Debug.Log(sb.ToString());
            var CreatedInstance = System.Activator.CreateInstance(assetstore);
            var setName = methods.Where(i => i.Name == "set_name").FirstOrDefault();
            setName.Invoke(CreatedInstance, new object[] { "Empty gameobject, created by reflection" });

            var createPrimitiveMethod = methods.Where(i => i.Name == "CreatePrimitive").FirstOrDefault();
            GameObject res = (GameObject)createPrimitiveMethod.Invoke(null, new object[] { PrimitiveType.Cube });
            setName.Invoke(res, new object[] { "A cube, created by reflection" });

        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogWarning(e.StackTrace);
        }
    }

    //[MenuItem("Gapslabs Extended Editor/Delete/Buildings")]
    static void DeleteBuildings()
    {
        var gos = GameObject.FindGameObjectsWithTag("Building");
        if (EditorUtility.DisplayDialog("Deleting buildings", "There are a total of " + gos.Length + " buildings.\nAre you sure you want to delete them?", "Yes", "No"))
        {
            foreach (var g in gos)
                GameObject.DestroyImmediate(g);
        }
    }

    //[MenuItem("Gapslabs Extended Editor/Delete/Roads")]
    static void DeleteRoads()
    {
        var gos = GameObject.FindGameObjectsWithTag("Line");
        if (EditorUtility.DisplayDialog("Deleting roads", "There are a total of " + gos.Length + " lines.\nAre you sure you want to delete them?", "Yes", "No"))
        {
            foreach (var g in gos)
                GameObject.DestroyImmediate(g);
        }
    }

    //[MenuItem("Gapslabs Extended Editor/GenerateBillboard")]
    static void DebugLineRenderer()
    {
        new Vector3(10, 0, 10).GenerateBillboardPlane("TESTID", LineDraw.OSMType.Node, "", "TrafficLight", "TrafficLight", 5f);
    }

    //[MenuItem("Gapslabs Extended Editor/Group all by tags")]
    static void GroupLines(bool RoadsOnly)
    {
        PrepareForCombining.TurnRoadCollidersOff();
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        if (!RoadsOnly)
        {
            GameObject go = new GameObject("Buildings");
            var gos = GameObject.FindGameObjectsWithTag("Building");
            for (int i = 0; i < gos.Length; i++)
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Grouping in progress", "Grouping all the buildings\t" + i + "/" + gos.Length, i / (float)gos.Length))
                {
                    gos[i].transform.parent = go.transform;
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            go.transform.localScale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        }
        GameObject go2 = new GameObject("Lines");
        var gos2 = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < gos2.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Grouping in progress", "Grouping all the lines\t" + i + "/" + gos2.Length, i / (float)gos2.Length))
            {
                gos2[i].transform.parent = go2.transform;
            }
            else
            {
                EditorUtility.ClearProgressBar();
                break;
            }
        }
        go2.transform.localScale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);

        PrepareForCombining.TurnRoadCollidersOn();
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("Gapslabs Extended Editor/Group roads")]
    static void GroupRoads()
    {
        PrepareForCombining.TurnRoadCollidersOff();
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        GameObject go2 = new GameObject("Lines");
        var gos2 = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < gos2.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Grouping in progress", "Grouping all the lines\t" + i + "/" + gos2.Length, i / (float)gos2.Length))
            {
                gos2[i].transform.parent = go2.transform;
            }
            else
            {
                EditorUtility.ClearProgressBar();
                break;
            }
        }
        go2.transform.localScale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);

        PrepareForCombining.TurnRoadCollidersOn();
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/From the database", false, 1)]
    static void Create()
    {

        //ServiceGapslabsClient client =ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        //string[] wayid= client.GetWayIdsWithIdCriteria(ServicePropertiesClass.ConnectionDatabase,"id<1260");
        //foreach( var w in wayid)
        //	Debug.Log (w);
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        ImportOSMDataSQL(false, true, true);

        GroupLines(false);
    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/Poly2Tri", false, 1)]
    static void CreatePoly2Tri()
    {
        try
        {
            ImportOSMDataSQLUsingPoly2Tri(true, true, true);
            var globalO = GameObject.Find("AramGISBoundaries");
            var mapproperties = globalO.GetComponent<MapBoundaries>();
            Vector3 scale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
            GroupObjectsWithTags("Lines", new string[] { "Line" }, scale, false, true);
            GroupObjectsWithTags("Buildings", new string[] { "Building" }, scale, false, true);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/TEST LIST", false, 1)]
    static void CreateFROMTESTLIST()
    {
        ChooseConnection();
        // 2370570
        // 1246645
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        var members = client.GetRelationMembers(wcfCon, "1246645");
        var ways = members.Where(m => m.Type == 1).OrderBy(m => m.order).Select(m => m.ReferenceId).ToArray();
        ImportOSMDataSQLOFTHELIST(ways, "MockData", true, true, true);
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        Vector3 scale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        GroupObjectsWithTags("MockDataGroup", new string[] { "MockData" }, scale, false);
    }

    //[MenuItem("Gapslabs Extended Editor/Generate and save as Obj/From the database(roads+Building Shapes)", false, 1)]
    static void GenerateAndSaveAsOBJ()
    {
        var result = EditorUtility.SaveFolderPanel("Select the target folder", "", "");
        if (!string.IsNullOrEmpty(result))
        {
            var globalO = GameObject.Find("AramGISBoundaries");
            var mapproperties = globalO.GetComponent<MapBoundaries>();
            ImportOSMDataSQLToOBJ(true, true, true, result);
            GroupLines(false);
        }
    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/Road Chunk 1", false, 1)]
    static void CreateRoadChunk1()
    {
        ImportOSMDataSQLByIndex(true, true, false, 0, 0.25f);
        //GroupRoads();
    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/Road Chunk 2", false, 1)]
    static void CreateRoadChunk2()
    {
        ImportOSMDataSQLByIndex(true, true, false, 0.25f, 0.5f);
        //GroupRoads();
    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/Road Chunk 3", false, 1)]
    static void CreateRoadChunk3()
    {
        ImportOSMDataSQLByIndex(true, true, false, 0.5f, 0.75f);
        //GroupRoads();
    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/Road Chunk 4", false, 1)]
    static void CreateRoadChunk4()
    {
        ImportOSMDataSQLByIndex(true, true, false, 0.75f, 1);
        //GroupRoads();
    }

    //[MenuItem("Gapslabs Extended Editor/Debug/Generate segmented network")]
    static void Create3()
    {

        //ServiceGapslabsClient client =ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        //string[] wayid= client.GetWayIdsWithIdCriteria(ServicePropertiesClass.ConnectionDatabase,"id<1260");
        //foreach( var w in wayid)
        //	Debug.Log (w);
        try
        {
            ImportOSMDataSQL_Ext(true, true, true);
            GroupLines(false);
        }
        catch (UnityException e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogException(e);
            return;
        }

    }

    //[MenuItem("Gapslabs Extended Editor/Load Load OSM Data/Roads from the database")]
    static void CreateRoads()
    {

        //ServiceGapslabsClient client =ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        //string[] wayid= client.GetWayIdsWithIdCriteria(ServicePropertiesClass.ConnectionDatabase,"id<1260");
        //foreach( var w in wayid)
        //	Debug.Log (w);
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        ImportOSMDataSQL(false, true, false);
        GroupLines(true);
    }

    /// -----------------------------------------------------------------------
    /// AUXILIAR METHODS AND ATTRIBUTES 
    /// -----------------------------------------------------------------------

    public static void Import(string[] tags)
    {
        PrepareForCombining.TurnRoadCollidersOff();
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();

        foreach (string T in tags)
        {
            string[] tag = {T};
            ImportOSMDataSQL_ServerSide_Extra(true, true, false, TagValues: tag);
        }

        //ImportOSMDataSQL_ServerSide_Extra(true, true, false, TagValues: tags);
        Vector3 scale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        GroupObjectsWithTags("Lines", new string[] { "Line" }, scale, false, true);
        AssignStaticLayers();
    }

    private static void AssignStaticLayers()
    {
        var navLayers = GameObjectUtility.GetNavMeshAreaNames();
        string layerError = "The navigation layer {0} does not exist. Please create the layer and retry.";
        bool proceed = true;
        var layerVehicle = "vehicle road";
        var layerResidential = "residential";
        var layerService = "service";
        var layerFootway = "footway";
        var layerCycleway = "cycleway";
        var layerSteps = "steps";
        var layerPedestrian = "pedestrian";
        var trafficRoads = "TrafficRoads";
        var trafficRoadsLayer = LayerMask.NameToLayer(trafficRoads);
        var walkableLayer = LayerMask.NameToLayer(layerFootway); //Added by Miguel R. C.

        if (!navLayers.Contains(layerVehicle))
        {
            Debug.LogError(string.Format(layerError, layerVehicle));
            proceed = false;
        }
        if (!navLayers.Contains(layerResidential))
        {
            Debug.LogError(string.Format(layerError, layerResidential));
            proceed = false;
        }
        if (!navLayers.Contains(layerService))
        {
            Debug.LogError(string.Format(layerError, layerService));
            proceed = false;
        }
        if (!navLayers.Contains(layerFootway))
        {
            Debug.LogError(string.Format(layerError, layerFootway));
            proceed = false;
        }
        if (!navLayers.Contains(layerCycleway))
        {
            Debug.LogError(string.Format(layerError, layerCycleway));
            proceed = false;
        }
        if (!navLayers.Contains(layerSteps))
        {
            Debug.LogError(string.Format(layerError, layerSteps));
            proceed = false;
        }
        if (!navLayers.Contains(layerPedestrian))
        {
            Debug.LogError(string.Format(layerError, layerPedestrian));
            proceed = false;
        }
        if (proceed)
        {
            var globalO = GameObject.Find("AramGISBoundaries");
            var mapproperties = globalO.GetComponent<MapBoundaries>();
            var visibleObjects = ((MeshRenderer[])GameObject.FindObjectsOfType(typeof(MeshRenderer))).Where(w => w.GetComponent<Renderer>().enabled).ToArray();
            Hashtable layers = new Hashtable();
            layers.Add(layerVehicle, GameObjectUtility.GetNavMeshAreaFromName(layerVehicle));
            layers.Add(layerResidential, GameObjectUtility.GetNavMeshAreaFromName(layerResidential));
            layers.Add(layerService, GameObjectUtility.GetNavMeshAreaFromName(layerService));
            layers.Add(layerFootway, GameObjectUtility.GetNavMeshAreaFromName(layerFootway));
            layers.Add(layerCycleway, GameObjectUtility.GetNavMeshAreaFromName(layerCycleway));
            layers.Add(layerSteps, GameObjectUtility.GetNavMeshAreaFromName(layerSteps));
            layers.Add(layerPedestrian, GameObjectUtility.GetNavMeshAreaFromName(layerPedestrian));



            try
            {
                for (int i = 0; i < visibleObjects.Length; i++)
                {
                    if (!EditorUtility.DisplayCancelableProgressBar("Assigning layers", "in progress", (float)i / visibleObjects.Length))
                    {
                        // Assign flags for oclussion culling
                        GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic);

                        try
                        {
                            if (visibleObjects[i].sharedMaterial != null)
                            {
                                var matname = visibleObjects[i].sharedMaterial.name;
                                if (matname == mapproperties.CycleWayMaterial.name)
                                {
                                    visibleObjects[i].gameObject.layer = walkableLayer; //Added by Miguel R. C.
                                    GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);
                                    GameObjectUtility.SetNavMeshArea(visibleObjects[i].gameObject, (int)layers[layerCycleway]);
                                }
                                else if (matname == mapproperties.FootWayMaterial.name || matname == mapproperties.AreaMaterial.name)
                                {
                                    visibleObjects[i].gameObject.layer = walkableLayer; //Added by Miguel R. C.
                                    GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);
                                    GameObjectUtility.SetNavMeshArea(visibleObjects[i].gameObject, (int)layers[layerFootway]);
                                }
                                else if (matname == mapproperties.StepsMaterial.name)
                                {
                                    visibleObjects[i].gameObject.layer = walkableLayer; //Added by Miguel R. C.
                                    GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);
                                    GameObjectUtility.SetNavMeshArea(visibleObjects[i].gameObject, (int)layers[layerSteps]);
                                }
                                else if (matname == mapproperties.FootWayMaterial.name)
                                {
                                    visibleObjects[i].gameObject.layer = walkableLayer; //Added by Miguel R. C.
                                    GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);
                                    GameObjectUtility.SetNavMeshArea(visibleObjects[i].gameObject, (int)layers[layerFootway]);
                                }
                                else if (matname == mapproperties.ResidentialMaterial.name || visibleObjects[i].name.ToLower().EndsWith(layerResidential))
                                {
                                    visibleObjects[i].gameObject.layer = trafficRoadsLayer;
                                    GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);
                                    GameObjectUtility.SetNavMeshArea(visibleObjects[i].gameObject, (int)layers[layerResidential]);
                                }
                                else if (matname == mapproperties.ServiceMaterial.name || visibleObjects[i].name.ToLower().EndsWith(layerService))
                                {
                                    visibleObjects[i].gameObject.layer = trafficRoadsLayer;
                                    GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);
                                    GameObjectUtility.SetNavMeshArea(visibleObjects[i].gameObject, (int)layers[layerService]);
                                }
                                else if (matname == mapproperties.RoadMaterial.name)
                                {
                                    visibleObjects[i].gameObject.layer = trafficRoadsLayer;
                                    GameObjectUtility.SetStaticEditorFlags(visibleObjects[i].gameObject, StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);
                                    GameObjectUtility.SetNavMeshArea(visibleObjects[i].gameObject, (int)layers[layerVehicle]);
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogException(ex, visibleObjects[i]);
                        }
                    }
                    else break;
                }
                EditorUtility.DisplayDialog("Assign Layers", "Navigation layers were assigned successfully. You can now re-bake the navigation.", "OK");
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            { EditorUtility.ClearProgressBar(); }
        }
    }

    private static void BakeNavigation()
    {
        NavMeshBuilder.ClearAllNavMeshes();
        NavMeshBuilder.BuildNavMeshAsync();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
    }

    static void GroupBuildings()
    {
        PrepareForCombining.TurnBuildingCollidersOff();
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        GameObject go = new GameObject("Buildings");
        var gos = GameObject.FindGameObjectsWithTag("Building");
        for (int i = 0; i < gos.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Grouping in progress", "Grouping all the buildings\t" + i + "/" + gos.Length, i / (float)gos.Length))
            {
                gos[i].transform.parent = go.transform;
            }
            else
            {
                EditorUtility.ClearProgressBar();
                break;
            }
        }
        go.transform.localScale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        PrepareForCombining.TurnRoadCollidersOn();
        EditorUtility.ClearProgressBar();
    }

    static void GroupObjectsWithTags(string GroupName, string[] Tags, Vector3 OverrideScale, bool addfaceCamera = true, bool applyScaleToTheObjects = false)
    {
        //NOTE: <param name="OverrideScale">Pass Vector3.zero to ignore, otherwise to override.</param>

        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        GameObject go = new GameObject(GroupName);
        List<GameObject> gos = new List<GameObject>();

        if (OverrideScale == Vector3.zero)
            OverrideScale = new Vector3(mapproperties.Scale.x, mapproperties.Scale.x, mapproperties.Scale.y);

        foreach (var tag in Tags)
        {
            gos.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
        for (int i = 0; i < gos.Count; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Grouping in progress", "Grouping all to " + GroupName + "\t" + i + "/" + gos.Count, i / (float)gos.Count))
            {
                gos[i].transform.parent = go.transform;
                if (applyScaleToTheObjects)
                {
                    gos[i].transform.position = Vector3.Scale(gos[i].transform.position, OverrideScale);
                    gos[i].transform.localScale = OverrideScale;
                }
                if (addfaceCamera)
                    gos[i].GetComponentInChildren<MeshFilter>().gameObject.AddComponent<FaceCamera>();
            }
            else
            {
                EditorUtility.ClearProgressBar();
                break;
            }
        }

        if (!applyScaleToTheObjects)
        {
            if (OverrideScale == Vector3.zero)
                go.transform.localScale = new Vector3(mapproperties.Scale.x, mapproperties.Scale.x, mapproperties.Scale.y);
            else
                go.transform.localScale = OverrideScale;
        }
        EditorUtility.ClearProgressBar();
    }

    public static string ChooseConnection()
    {
        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;
        return wcfCon;
    }

    private static string trafficSignalTag = "traffic_signals";

    private static string busStopsTag = "highway=bus_stop|bus=yes";

    static void ImportOSMDataSQL_Billboard(string[][] WithTags, string Other, string Tag, string MaterialName, float OffsetFromTheGround = 0)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        var go = GameObject.Find("AramGISBoundaries");
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 

        var boundsTemp = client.GetBounds(wcfCon);
        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon = 32;


        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;


        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            //if (mapboundaries.CorrectAspectRatio)
            //{
            //    var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
            //    minmaxY[1] = (float)(minmaxX[1] / aspectRatio);
            //}
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            SelectedArea.minlat = 59.3675700;
            SelectedArea.minlon = 18.0044500;
            SelectedArea.maxlat = 59.3757400;
            SelectedArea.maxlon = 18.0175600;
            minmaxX = new float[] { 0, 40000f };
            minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting nodes...");

        //string[] ways= client.GetWayIdsInBound(wcfCon,SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        string[] trafficNodes = client.GetNodeIdsInBoundWithKeyValueTag(wcfCon, SelectedArea, WithTags);
        Debug.Log("Nodes were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);

        Debug.Log("Number of traffic nodes:" + trafficNodes.Length);


        int totalNodes = trafficNodes.Length;
        float progress = 0f;


        // var mapProperties = go.GetComponent<MapBoundaries>();
        // the constant in scaleSize is simply the size of the quad polygon.
        // var scaleSize = 1;// / ((mapProperties.Scale.x + mapProperties.Scale.y) / 2);

        foreach (var CurrentNode in trafficNodes)
        {
            try
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating " + Tag + "\t" + progress + "/" + totalNodes, progress++ / totalNodes))
                {
                    //var WayNodes =client.GetWayNodes(CurrentNode,wcfCon); //new Way(CurrentNode);
                    //var WayTags = client.GetWayTags(CurrentNode,wcfCon);
                    var node = client.GetNodeInfo(CurrentNode, wcfCon);
                    //if (WayTags.Where(i=> i.KeyValue[0].ToLower()== "landuse"||i.KeyValue[0].ToLower()== "building"||i.KeyValue[0].ToLower()== "highway").Count()!=0)

                    Vector3 tempPoint = new Vector3();
                    int counter = 0;

                    var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                    tempPoint = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;

                    // TODO: Replace with the new Quad primitive of unity.
                    tempPoint.GenerateBillboardPlaneUsingUnityQuad(CurrentNode, LineDraw.OSMType.Node, Other, Tag, MaterialName, OffsetFromTheGround);

                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }
        }

        EditorUtility.ClearProgressBar();
    }

    static void ImportOSMDataSQL_ArbitraryShapeFromWay(string[][] WithTags, string Other, string Tag, string MaterialName, float OffsetFromTheGround = 0)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        var go = GameObject.Find("AramGISBoundaries");
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 

        var boundsTemp = client.GetBounds(wcfCon);
        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon = 32;


        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;


        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            //if (mapboundaries.CorrectAspectRatio)
            //{
            //    var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
            //    minmaxY[1] = (float)(minmaxX[1] / aspectRatio);
            //}
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            SelectedArea.minlat = 59.3675700;
            SelectedArea.minlon = 18.0044500;
            SelectedArea.maxlat = 59.3757400;
            SelectedArea.maxlon = 18.0175600;
            minmaxX = new float[] { 0, 40000f };
            minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting nodes...");

        //string[] ways= client.GetWayIdsInBound(wcfCon,SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        string[] WayAreas = client.GetWayIdsWithTags(wcfCon, SelectedArea, WithTags);
        Debug.Log("Nodes were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);

        if (WayAreas == null || WayAreas.Length == 0)
        {
            Debug.LogWarning("There were no water areas defined.");
            return;
        }
        else
            Debug.Log("Number of areas:" + WayAreas.Length);

        int totalAreas = WayAreas.Length;
        float progress = 0f;

        // var mapProperties = go.GetComponent<MapBoundaries>();
        // the constant in scaleSize is simply the size of the quad polygon.
        // var scaleSize = 1;// / ((mapProperties.Scale.x + mapProperties.Scale.y) / 2);

        foreach (var CurrentArea in WayAreas)
        {
            try
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating " + Tag + "\t" + progress + "/" + totalAreas, progress++ / totalAreas))
                {
                    //var WayNodes =client.GetWayNodes(CurrentNode,wcfCon); //new Way(CurrentNode);
                    //var WayTags = client.GetWayTags(CurrentNode,wcfCon);
                    var area = client.GetWayNodes(CurrentArea, wcfCon);
                    //if (WayTags.Where(i=> i.KeyValue[0].ToLower()== "landuse"||i.KeyValue[0].ToLower()== "building"||i.KeyValue[0].ToLower()== "highway").Count()!=0)

                    List<Vector3> tempPoints = new List<Vector3>();
                    int counter = 0;

                    foreach (var node in area)
                    {
                        var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                        var tempPoint = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                        tempPoints.Add(tempPoint);
                    }

                    tempPoints = Interpolations.RemoveDuplicates(tempPoints.ToArray()).ToList();

                    if (tempPoints.Count <= 2)
                    {
                        // Buildings that are too small to show such as 76844368
                        // http://www.openstreetmap.org/browse/way/76844368
                        Debug.Log(
                                        string.Format(
                                                "A weird area were found and ignored.\nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                , CurrentArea)
                                        );
                        continue;
                    }
                    var polypoints = tempPoints.Select(s => new Poly2Tri.PolygonPoint(s.x, s.z));
                    Poly2Tri.Polygon poly = new Poly2Tri.Polygon(polypoints);
                    Poly2Tri.P2T.Triangulate(poly);

                    // TODO: Replace with the new Quad primitive of unity.
                    poly.GenerateShapeUVedPlanar_Balanced(LineDraw.OSMType.Line, CurrentArea, Other, Tag, MaterialName, OffsetFromTheGround);

                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }
        }

        EditorUtility.ClearProgressBar();
    }

    static void ImportOSMDataSQL_TrafficLights()
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        var go = GameObject.Find("AramGISBoundaries");
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 

        var boundsTemp = client.GetBounds(wcfCon);
        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        boundsTemp.maxlon = 32;


        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;


        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            SelectedArea.minlat = 59.3675700;
            SelectedArea.minlon = 18.0044500;
            SelectedArea.maxlat = 59.3757400;
            SelectedArea.maxlon = 18.0175600;
            minmaxX = new float[] { 0, 40000f };
            minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting nodes...");

        //string[] ways= client.GetWayIdsInBound(wcfCon,SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        string[] trafficNodes = client.GetNodeIdsInBoundWithInfo(wcfCon, SelectedArea, trafficSignalTag);

        Debug.Log("Nodes were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);
        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);

        Debug.Log("Number of traffic lights:" + trafficNodes.Length);


        int totalNodes = trafficNodes.Length;
        float progress = 0f;


        var mapProperties = go.GetComponent<MapBoundaries>();
        // the constant in scaleSize is simple the size of the quad polygon.
        var scaleSize = 1 / ((mapProperties.Scale.x + mapProperties.Scale.y) / 2);

        foreach (var CurrentNode in trafficNodes)
        {

            if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating traffic lights\t" + progress + "/" + totalNodes, progress++ / totalNodes))
            {
                //var WayNodes =client.GetWayNodes(CurrentNode,wcfCon); //new Way(CurrentNode);
                //var WayTags = client.GetWayTags(CurrentNode,wcfCon);
                var node = client.GetNodeInfo(CurrentNode, wcfCon);
                //if (WayTags.Where(i=> i.KeyValue[0].ToLower()== "landuse"||i.KeyValue[0].ToLower()== "building"||i.KeyValue[0].ToLower()== "highway").Count()!=0)

                Vector3 tempPoint = new Vector3();
                int counter = 0;

                var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                tempPoint = new Vector3((float)result[0], 0, (float)result[1]) - MinPointOnMap;


                tempPoint.GenerateBillboardPlane(CurrentNode, LineDraw.OSMType.Node, "TrafficLight", "TrafficLight", "TrafficLight", 1 * scaleSize, 5 * scaleSize);

            }
            else
            {
                EditorUtility.ClearProgressBar();
                break;
            }
        }
        EditorUtility.ClearProgressBar();
    }

    //static string conn = "Server=localhost\\SQLEXPRESS;Database=GIStest;User Id=gapslabuser;Password=test;";

    //static string connOleDb = "Driver=SQLOLEDB;Server=localhost\\SQLEXPRESS;Database=GIStest;User Id=gapslabuser;Password=test;";

    static string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase; // .ConnectionDatabase;

    //static string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabaseTestForDistribution; // .ConnectionDatabase;

    static void ImportOSMDataSQL_Ext(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;
        float height = 5f;
        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 

        var boundsTemp = client.GetBounds(wcfCon);
        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;


        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;


        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            SelectedArea.minlat = 59.3675700;
            SelectedArea.minlon = 18.0044500;
            SelectedArea.maxlat = 59.3757400;
            SelectedArea.maxlon = 18.0175600;
            minmaxX = new float[] { 0, 40000f };
            minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");

        // index 0 of 2nd dimension is the line ID, 
        // index 1 of 2nd dimension is the original line ID
        string[][] ways = client.GetWayExtIdsInBound(wcfCon, SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        Debug.Log("Way were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);
        Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0], 0, MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);

        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();
        foreach (var FirstWay in ways)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
            {
                WayNodes = client.GetWayExtNodes(FirstWay[0], wcfCon); //new Way(FirstWay);
                WayTags = client.GetWayTags(FirstWay[1], wcfCon);

                if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
                {
                    tempPoints = new Vector3[WayNodes.Length];
                    int counter = 0;

                    foreach (var node in WayNodes)
                    {
                        var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                        tempPoints[counter] = new Vector3((float)result[0], 0, (float)result[1]) - MinPointOnMap;
                        counter++;
                    }
                    WayNodes = null;
                    var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
                    var highwayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "highway");
                    WayTags = null;
                    if (building.Count() != 0)
                    {
                        if (GenerateBuildings)
                        {
                            //Debug.Log("Current building: "+FirstWay);
                            if (GenerateBuildingShapes)
                            {
                                // Check if it has overlapping start and ending points.
                                // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                // Checking for duplicates:
                                List<Vector3> optimizedList = new List<Vector3>();
                                for (int i = 0; i < tempPoints.Length; i++)
                                {
                                    if (!optimizedList.Contains(tempPoints[i]))
                                        optimizedList.Add(tempPoints[i]);
                                }
                                tempPoints = optimizedList.ToArray();


                                //Debug.Log("Initializing the cutting");
                                var p2d = tempPoints.ToCPoint2D();

                                shp = new PolygonCuttingEar.CPolygonShape(p2d);
                                p2d = null;
                                System.GC.Collect();
                                //Debug.Log("Cutting...");
                                shp.CutEar();


                                shp.GenerateShapeUVedWithWalls_Balanced(
                                        LineDraw.OSMType.Polygon, FirstWay[0],
                                        "Building", "Building", "Building",
                                        height + Random.Range(-3f, +7f), height + 7, true);
                                //Debug.Log("The crap's been generated.");
                            }
                            else
                                draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay[0], "Building", "Building");
                        }
                    }
                    else
                    {
                        if (highwayType.Count() != 0)
                        {
                            if (GenerateRoads)
                            {
                                var hwtype = highwayType.First();
                                switch (hwtype.KeyValue[1])
                                {
                                    case "cycleway":
                                        {
                                            //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.8f,0.8f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.CycleWayMaterial.name);
                                            //draw.TestMeshGenerationSimple(tempPoints,0.05f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.CycleWayMaterial.name);
                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay[1], hwtype.KeyValue[1], "Line", mapProperty.CycleWayMaterial.name, -0.01f);
                                            break;
                                        }
                                    case "footway":
                                    case "path":
                                    case "pedestrian":
                                        {
                                            //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.4f,0.4f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.FootWayMaterial.name);
                                            //draw.TestMeshGenerationSimple(tempPoints, 0.025f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.FootWayMaterial.name);
                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.FootwayWidth, LineDraw.OSMType.Line, FirstWay[1], hwtype.KeyValue[1], "Line", mapProperty.FootWayMaterial.name, -0.01f);
                                            break;
                                        }
                                    case "steps":
                                        {
                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay[1], hwtype.KeyValue[1], "Line", mapProperty.StepsMaterial.name, 0.01f);
                                            break;
                                        }
                                    case "motorway":
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,1.6f,1.6f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.RoadMaterial.name);
                                            //draw.TestMeshGenerationSimple(tempPoints, 0.1f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.RoadMaterial.name);
                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapProperty.RoadWidth, LineDraw.OSMType.Line, FirstWay[1], hwtype.KeyValue[1], "Line", mapProperty.RoadMaterial.name, 0f);
                                            break;
                                        }
                                }
                            }

                        }
                        else if (GenerateRoads)
                            draw.Draw(tempPoints, lineColor, lineColor2, LineWidth, LineWidth, LineDraw.OSMType.Line, FirstWay[0], null, "Line");
                    }
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
                break;
            }

        }

        EditorUtility.ClearProgressBar();
    }

    static void ImportOSMDataSQLByIndex(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings, float from, float to, bool CorrectAspectRatio = false)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;
        float height = 5f;
        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 

        var boundsTemp = client.GetBounds(wcfCon);
        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;


        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;


        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            if (CorrectAspectRatio)
            {
                var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
                minmaxY[1] = (float)(minmaxX[1] / aspectRatio);
            }

            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            SelectedArea.minlat = 59.3675700;
            SelectedArea.minlon = 18.0044500;
            SelectedArea.maxlat = 59.3757400;
            SelectedArea.maxlon = 18.0175600;
            minmaxX = new float[] { 0, 40000f };
            minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");

        string[] ways = client.GetWayIdsInBound(wcfCon, SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        Debug.Log("Total number of Ways:" + ways.Length);
        int startIndex = Mathf.FloorToInt(Interpolations.linear(from, 0, 1, 0, ways.Length));
        int endIndex = Mathf.FloorToInt(Interpolations.linear(to, 0, 1, 0, ways.Length));
        ways = ways.Skip(startIndex).Take(endIndex - startIndex).ToArray();

        Debug.Log("Length:" + ways.Length + "   " + startIndex + "   " + endIndex);

        Debug.Log("Way were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);
        Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0], 0, MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);

        Debug.Log("Number of Ways used:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();
        foreach (var FirstWay in ways)
        {

            try
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                {
                    WayNodes = client.GetWayNodes(FirstWay, wcfCon); //new Way(FirstWay);
                    WayTags = client.GetWayTags(FirstWay, wcfCon);
                    if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
                    {
                        tempPoints = new Vector3[WayNodes.Length];
                        int counter = 0;

                        foreach (var node in WayNodes)
                        {
                            var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                            tempPoints[counter] = new Vector3((float)result[0], 0, (float)result[1]) - MinPointOnMap;
                            counter++;
                        }
                        WayNodes = null;
                        var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
                        var highwayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "highway");
                        WayTags = null;
                        if (building.Count() != 0)
                        {
                            if (GenerateBuildings)
                            {
                                //Debug.Log("Current building: "+FirstWay);
                                if (GenerateBuildingShapes)
                                {
                                    // Check if it has overlapping start and ending points.
                                    // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                    // Checking for duplicates:
                                    List<Vector3> optimizedList = new List<Vector3>();
                                    for (int i = 0; i < tempPoints.Length; i++)
                                    {
                                        if (!optimizedList.Contains(tempPoints[i]))
                                            optimizedList.Add(tempPoints[i]);
                                    }
                                    if (optimizedList.Count <= 2)
                                    {
                                        // Buildings that are too small to show such as 76844368
                                        // http://www.openstreetmap.org/browse/way/76844368
                                        Debug.Log(
                                                        string.Format(
                                                                "A weird building were found and ignored.\nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                                , FirstWay)
                                                        );
                                        continue;
                                    }
                                    tempPoints = optimizedList.ToArray();
                                    //Debug.Log("# of points:"+tempPoints.Length);
                                    //Debug.Log("Current Building: "+FirstWay);

                                    //Debug.Log("Initializing the cutting");
                                    var p2d = tempPoints.ToCPoint2D();
                                    //Debug.Log("# of points 2D:"+p2d.Length);
                                    //							foreach(var ptemp in p2d)
                                    //								Debug.Log(ptemp.X +", "+ ptemp.Y);

                                    shp = new PolygonCuttingEar.CPolygonShape(p2d);

                                    //Debug.Log("Cutting...");
                                    //try 
                                    {
                                        shp.CutEar();
                                    }
                                    //catch (System.Exception shpExp)
                                    {

                                    }
                                    p2d = null;
                                    System.GC.Collect();

                                    var randHeight = Random.Range(-3f, +7f);
                                    var randMaterial = randHeight > 5 ? "BuildingTall" : randHeight < 0 ? "Building2" : "Building";
                                    shp.GenerateShapeUVedWithWalls_Balanced(
                                            LineDraw.OSMType.Polygon, FirstWay,
                                            "Building", "Building", randMaterial,
                                            height + randHeight, height + 7, true);
                                    //Debug.Log("The crap's been generated.");
                                }
                                else
                                    draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay, "Building", "Building");
                            }
                        }
                        else
                        {
                            if (highwayType.Count() != 0)
                            {
                                if (GenerateRoads)
                                {
                                    var hwtype = highwayType.First();
                                    switch (hwtype.KeyValue[1])
                                    {
                                        case "cycleway":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.8f,0.8f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.CycleWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints,0.05f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.CycleWayMaterial.name);
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.CycleWayMaterial.name, -0.01f);
                                                break;
                                            }
                                        case "footway":
                                        case "path":
                                        case "pedestrian":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.4f,0.4f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.FootWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.025f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.FootWayMaterial.name);
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.FootwayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.FootWayMaterial.name, -0.01f);
                                                break;
                                            }
                                        case "steps":
                                            {
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.StepsMaterial.name, 0.01f);
                                                break;
                                            }
                                        case "motorway":
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,1.6f,1.6f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.RoadMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.1f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.RoadMaterial.name);
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapProperty.RoadWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.RoadMaterial.name, 0f);
                                                break;
                                            }
                                    }
                                }

                            }
                            else if (GenerateRoads)
                                draw.Draw(tempPoints, lineColor, lineColor2, LineWidth, LineWidth, LineDraw.OSMType.Line, FirstWay, null, "Line");
                        }
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }

        }

        EditorUtility.ClearProgressBar();

    }

    static void ImportOSMDataSQL_PARALLEL(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;
        float height = 5f;
        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");

        var boundsTemp = client.GetBounds(wcfCon);


        Debug.Log("Setting interpolation Boundary");


        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;


        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            SelectedArea.minlat = 59.3675700;
            SelectedArea.minlon = 18.0044500;
            SelectedArea.maxlat = 59.3757400;
            SelectedArea.maxlon = 18.0175600;
            minmaxX = new float[] { 0, 40000f };
            minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");

        string[] ways = client.GetWayIdsInBound(wcfCon, SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        Debug.Log("Way were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        // Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);


        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();
        foreach (var FirstWay in ways)
        {
            try
            {
                WayNodes = client.GetWayNodes(FirstWay, wcfCon); //new Way(FirstWay);
                WayTags = client.GetWayTags(FirstWay, wcfCon);
                if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
                {
                    tempPoints = new Vector3[WayNodes.Length];
                    int counter = 0;

                    foreach (var node in WayNodes)
                    {
                        var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                        // Testing the correct direction
                        tempPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
#if SHOW_STUFF
                        Debug.Log(tempPoints[counter].x + ", " + tempPoints[counter].z);
#endif
                        // tempPoints[counter]=new Vector3((float)result[0],0,(float)result[1])-MinPointOnMap;
                        counter++;
                    }
                    WayNodes = null;
                    var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
                    var highwayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "highway");
                    WayTags = null;
                    if (building.Count() != 0)
                    {
                        if (GenerateBuildings)
                        {
                            //Debug.Log("Current building: "+FirstWay);
                            if (GenerateBuildingShapes)
                            {
                                // Check if it has overlapping start and ending points.
                                // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                // Checking for duplicates:
                                List<Vector3> optimizedList = new List<Vector3>();
                                for (int i = 0; i < tempPoints.Length; i++)
                                {
                                    if (!optimizedList.Contains(tempPoints[i]))
                                        optimizedList.Add(tempPoints[i]);
                                }
                                if (optimizedList.Count <= 2)
                                {
                                    // Buildings that are too small to show such as 76844368
                                    // http://www.openstreetmap.org/browse/way/76844368
                                    Debug.Log(
                                                    string.Format(
                                                            "A weird building were found and ignored.\nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                            , FirstWay)
                                                    );
                                    continue;
                                }
                                tempPoints = optimizedList.ToArray();
                                //Debug.Log("# of points:"+tempPoints.Length);
                                //Debug.Log("Current Building: "+FirstWay);

                                //Debug.Log("Initializing the cutting");
                                var p2d = tempPoints.ToCPoint2D();
                                //Debug.Log("# of points 2D:"+p2d.Length);
                                //							foreach(var ptemp in p2d)
                                //								Debug.Log(ptemp.X +", "+ ptemp.Y);

                                shp = new PolygonCuttingEar.CPolygonShape(p2d);

                                //Debug.Log("Cutting...");
                                //try 
                                {
                                    shp.CutEar();
                                }
                                //catch (System.Exception shpExp)
                                {

                                }
                                p2d = null;
                                System.GC.Collect();

                                var randHeight = Random.Range(-3f, +40f);
                                var randMaterial = randHeight > 20 ? "BuildingTall" : randHeight < 10 ? "Building2" : "Building";
                                shp.GenerateShapeUVedWithWalls_Balanced(
                                        LineDraw.OSMType.Polygon, FirstWay,
                                        "Building", "Building", randMaterial,
                                        height + randHeight, height + 7, true);
                                //Debug.Log("The crap's been generated.");
                            }
                            else
                                draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay, "Building", "Building");
                        }
                    }
                    else
                    {
                        if (highwayType.Count() != 0)
                        {
                            if (GenerateRoads)
                            {
                                var hwtype = highwayType.First();
                                switch (hwtype.KeyValue[1])
                                {
                                    case "cycleway":
                                        {
                                            //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.8f,0.8f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.CycleWayMaterial.name);
                                            //draw.TestMeshGenerationSimple(tempPoints,0.05f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.CycleWayMaterial.name);

                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.CycleWayMaterial.name, -0.01f);

                                            break;
                                        }
                                    case "footway":
                                    case "path":
                                    case "pedestrian":
                                        {
                                            //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.4f,0.4f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.FootWayMaterial.name);
                                            //draw.TestMeshGenerationSimple(tempPoints, 0.025f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.FootWayMaterial.name);

                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.FootwayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.FootWayMaterial.name, -0.01f);

                                            break;
                                        }
                                    case "steps":
                                        {

                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.StepsMaterial.name, -0.01f);

                                            break;
                                        }
                                    case "motorway":
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,1.6f,1.6f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.RoadMaterial.name);
                                            //draw.TestMeshGenerationSimple(tempPoints, 0.1f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.RoadMaterial.name);

                                            LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapProperty.RoadWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.RoadMaterial.name, 0f);

                                            break;
                                        }
                                }
                            }

                        }
                        //else if (GenerateRoads)
                        //    draw.Draw(tempPoints, lineColor, lineColor2, LineWidth, LineWidth, LineDraw.OSMType.Line, FirstWay, null, "Line");
                    }
                }

            }
            catch (System.Exception e)
            {

                throw e;
            }

        }

        EditorUtility.ClearProgressBar();


    }

    static void ImportOSMDataSQLOFTHELIST(string[] ways, string tag, bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;

        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 
        Debug.Log("Check if there are errors");

        var boundsTemp = client.GetBounds(wcfCon);
        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;
        Debug.Log("Is there an error?");

        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;

        float height; //5f;
        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            height = mapboundaries.BuildingHeight;
            if (height < 4)
                height = 4;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            //SelectedArea.minlat = 59.3675700;
            //SelectedArea.minlon = 18.0044500;
            //SelectedArea.maxlat = 59.3757400;
            //SelectedArea.maxlon = 18.0175600;
            //minmaxX = new float[] { 0, 40000f };
            //minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Ways were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        // Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);


        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();
        foreach (var FirstWay in ways)
        {

            try
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                {
                    WayNodes = client.GetWayNodes(FirstWay, wcfCon); //new Way(FirstWay);
                    WayTags = client.GetWayTags(FirstWay, wcfCon);
                    if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
                    {
                        tempPoints = new Vector3[WayNodes.Length];
                        int counter = 0;

                        foreach (var node in WayNodes)
                        {
                            var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                            // Testing the correct direction
                            tempPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
#if SHOW_STUFF
                            Debug.Log(tempPoints[counter].x + ", " + tempPoints[counter].z);
#endif
                            // tempPoints[counter]=new Vector3((float)result[0],0,(float)result[1])-MinPointOnMap;
                            counter++;
                        }
                        WayNodes = null;
                        var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
                        var highwayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "highway");
                        WayTags = null;
                        if (building.Count() != 0)
                        {
                            if (GenerateBuildings)
                            {
                                //Debug.Log("Current building: "+FirstWay);
                                if (GenerateBuildingShapes)
                                {
                                    // Check if it has overlapping start and ending points.
                                    // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                    // Checking for duplicates:
                                    List<Vector3> optimizedList = new List<Vector3>();
                                    for (int i = 0; i < tempPoints.Length; i++)
                                    {
                                        if (!optimizedList.Contains(tempPoints[i]))
                                            optimizedList.Add(tempPoints[i]);
                                    }
                                    if (optimizedList.Count <= 2)
                                    {
                                        // Buildings that are too small to show such as 76844368
                                        // http://www.openstreetmap.org/browse/way/76844368
                                        Debug.Log(
                                                        string.Format(
                                                                "A weird building were found and ignored.\nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                                , FirstWay)
                                                        );
                                        continue;
                                    }
                                    tempPoints = optimizedList.ToArray();
                                    //Debug.Log("# of points:"+tempPoints.Length);
                                    //Debug.Log("Current Building: "+FirstWay);

                                    //Debug.Log("Initializing the cutting");
                                    var p2d = tempPoints.ToCPoint2D();
                                    //Debug.Log("# of points 2D:"+p2d.Length);
                                    //							foreach(var ptemp in p2d)
                                    //								Debug.Log(ptemp.X +", "+ ptemp.Y);

                                    shp = new PolygonCuttingEar.CPolygonShape(p2d);

                                    //Debug.Log("Cutting...");
                                    //try 
                                    {
                                        shp.CutEar();
                                    }
                                    //catch (System.Exception shpExp)
                                    {

                                    }
                                    p2d = null;
                                    System.GC.Collect();

                                    var randHeight = Random.Range(-3f, height);
                                    var randMaterial = (randHeight > height / 2f) ? "BuildingTall" : randHeight < height / 2f ? "Building2" : "Building";
                                    shp.GenerateShapeUVedWithWalls_Balanced(
                                            LineDraw.OSMType.Polygon, FirstWay,
                                            "Building", tag, randMaterial,
                                            height + randHeight, height + 7, true);
                                    //Debug.Log("The crap's been generated.");
                                }
                                else
                                    draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay, "Building", tag);
                            }
                        }
                        else
                        {
                            if (highwayType.Count() != 0)
                            {
                                if (GenerateRoads)
                                {
                                    var hwtype = highwayType.First();
                                    switch (hwtype.KeyValue[1])
                                    {
                                        case "cycleway":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.8f,0.8f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.CycleWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints,0.05f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.CycleWayMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], tag, mapProperty.CycleWayMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "footway":
                                        case "path":
                                        case "pedestrian":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.4f,0.4f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.FootWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.025f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.FootWayMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.FootwayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], tag, mapProperty.FootWayMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "steps":
                                            {

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], tag, mapProperty.StepsMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "motorway":
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,1.6f,1.6f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.RoadMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.1f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.RoadMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapProperty.RoadWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], tag, mapProperty.RoadMaterial.name, 0f);

                                                break;
                                            }
                                    }
                                }

                            }
                            //else if (GenerateRoads)
                            //    draw.Draw(tempPoints, lineColor, lineColor2, LineWidth, LineWidth, LineDraw.OSMType.Line, FirstWay, null, "Line");
                        }
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }

        }

        EditorUtility.ClearProgressBar();


    }

    static void ImportOSMDataSQL_ServerSide_Extra(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings, bool CorrectAspectRatio = false, string[] TagValues = null)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;

        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        var Con = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : wcfCon;
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 
        Debug.Log("Check if there are errors");
        BoundsWCF boundsTemp = null;

        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            boundsTemp = client.GetBounds(Con);
        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally { EditorUtility.ClearProgressBar(); }

        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;
        Debug.Log("Is there an error?");
        if (boundsTemp == null)
        {
            Debug.LogWarning("Could not communicate to receive the database bounds.");
            return;
        }
        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};



        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;
        MapBoundaries mapboundaries;
        float height; //5f;
        if (go != null)
        {
            mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            Debug.Log(minmaxX + "\t" + minmaxY);
            //if (CorrectAspectRatio)
            //{
            //    //var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
            //    //minmaxY[1] = (float)(minmaxX[1] * aspectRatio);
            //    var latDist = CoordinateConvertor.GeoDistance(SelectedArea.minlat, SelectedArea.minlon, SelectedArea.maxlat, SelectedArea.minlon) * 1000;
            //    var lonDist = CoordinateConvertor.GeoDistance(SelectedArea.minlat, SelectedArea.minlon, SelectedArea.minlat, SelectedArea.maxlon) * 1000;
            //    Debug.Log("Latitude in kilometers: " + latDist + " ,in float: " + (float)latDist);
            //    Debug.Log("Longitude in kilometers: " + lonDist + " ,in float: " + (float)lonDist);
            //    minmaxX[1] = (float)latDist;
            //    minmaxY[1] = (float)lonDist;
            //    mapboundaries.minMaxX = minmaxX;
            //    mapboundaries.minMaxY = minmaxY;
            //}

            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            height = mapboundaries.BuildingHeight;
            if (height < 4)
                height = 4;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            //SelectedArea.minlat = 59.3675700;
            //SelectedArea.minlon = 18.0044500;
            //SelectedArea.maxlat = 59.3757400;
            //SelectedArea.maxlon = 18.0175600;
            //minmaxX = new float[] { 0, 40000f };
            //minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");


        string[] ways = null;
        string[] buildingsInRelationTable = null;

        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            if (!GenerateRoads)
            {
                string[][] buildingtag = new string[1][];
                buildingtag[0] = new string[] { "building", "" }; // NOTE: building tag in OSM is in lower case.

                ways = client.GetWayIdsWithTags(Con, SelectedArea, buildingtag);
                buildingsInRelationTable = client.GetRelationBuildingsInBound(Con, SelectedArea);
            }
            else
                if (!GenerateBuildings)
                {
                    string[][] roadtag = new string[1][];
                    if (TagValues == null)
                    {
                        roadtag[0] = new string[] { "highway", "" }; // NOTE: tag in OSM is in lower case.
                    }
                    else
                    {
                        roadtag = new string[TagValues.Length][];
                        for (int i = 0; i < TagValues.Length; i++)
                        {
                            roadtag[i] = new string[] { "highway", TagValues[i] };
                        }
                    }

                    ways = client.GetWayIdsWithTags(Con, SelectedArea, roadtag);
                }
                else
                {
                    ways = client.GetWayIdsInBound(Con, SelectedArea); //client.GetWayIdsWithIdCriteria(Con,"id>0 and id<101000");
                    buildingsInRelationTable = client.GetRelationBuildingsInBound(Con, SelectedArea);
                }


        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally { EditorUtility.ClearProgressBar(); }



        Debug.Log("Ways were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        GaPSLabs.Geometry.Vector3 MinPointOnMap = new GaPSLabs.Geometry.Vector3();
        MinPointOnMap.x = direction * MinPointOnArea[0];
        MinPointOnMap.y = 0;
        MinPointOnMap.z = MinPointOnArea[1];
        // Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        EditorUtility.SetDirty(go);


        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        if (buildingsInRelationTable != null)
            totalWays += buildingsInRelationTable.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();

        client.GetOSMmeshFromOsmId(null, MinPointOnMap, boundsTemp, mapboundaries.ToGapslabsMapProperties(mapboundaries.BuildingMinimumHeight, mapboundaries.BuildingMaximumHeight), Con, "Way"
            , mapboundaries.RemoveRedundantPointsOnTheSameLine, mapboundaries.RedundantPointErrorThreshold, mapboundaries.SegmentLines, true);
        try
        {
            foreach (var FirstWay in ways)
            {
                //#if SHOW_STUFF
                //                if (FirstWay != "109728659")
                //                    continue;
                //#endif
                try
                {
                    if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                    {
                        var serverObject = client.GetOSMmeshFromOsmId(FirstWay, MinPointOnMap, boundsTemp, null, Con, "Way"
                            , mapboundaries.RemoveRedundantPointsOnTheSameLine, mapboundaries.RedundantPointErrorThreshold, mapboundaries.SegmentLines, true);
                        //var serverObjectBinary = client.GetOSMmeshFromOsmId2(FirstWay, MinPointOnMap, boundsTemp, null, Con);
                        //ProtoBuf.Serializer.Deserialize<GaPSLabs.Geometry.GameObject>()
                        if (serverObject != null)
                        {
                            var tag = serverObject.Name.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
                            var osmtype = serverObject.Name.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[3].ToLower();
                            if (tag == "1")
                                tag = "Line";
                            else if (tag == "2" && !osmtype.EndsWith("area"))
                                tag = "Building";
                            else if (tag == "2" && osmtype.EndsWith("area"))
                                tag = "Area";
                            serverObject.GenerateShapeFromServerObject(tag, true);
                        }
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                        break;
                    }
                }
                catch (System.Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    throw e;
                }

            }
            if (buildingsInRelationTable != null)
            {
                foreach (var FirstWay in buildingsInRelationTable)
                {
                    //#if SHOW_STUFF
                    //                if (FirstWay != "109728659")
                    //                    continue;
                    //#endif
                    try
                    {
                        if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                        {
                            //Debug.Log("Doing " + FirstWay);
                            var serverObject = client.GetOSMmeshFromOsmId(FirstWay, MinPointOnMap, boundsTemp, null, Con, "Relation"
                                , mapboundaries.RemoveRedundantPointsOnTheSameLine, mapboundaries.RedundantPointErrorThreshold, mapboundaries.SegmentLines, true);
                            //Debug.Log("Triangles:" + serverObject.mesh.triangles.Length);
                            //var serverObjectBinary = client.GetOSMmeshFromOsmId2(FirstWay, MinPointOnMap, boundsTemp, null, Con);
                            //ProtoBuf.Serializer.Deserialize<GaPSLabs.Geometry.GameObject>()
                            if (serverObject != null)
                            {
                                var tag = serverObject.Name.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
                                if (tag == "1")
                                    tag = "Line";
                                else if (tag == "2")
                                    tag = "Building";
                                else if (tag == "3") // A building that were generated out of a relation
                                    tag = "Building";
                                serverObject.GenerateShapeFromServerObject(tag, true);
                            }
                        }
                        else
                        {
                            EditorUtility.ClearProgressBar();
                            break;
                        }
                    }
                    catch (System.Exception e)
                    {
                        EditorUtility.ClearProgressBar();
                        Debug.LogException(e);
                    }

                }
            }
        }
        finally
        {
            client.Close();
        }
        EditorUtility.ClearProgressBar();


    }


    static void ImportOSMDataSQL_ServerSide(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings, bool CorrectAspectRatio = false)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;

        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        var Con = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : wcfCon;
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 
        Debug.Log("Check if there are errors");
        BoundsWCF boundsTemp = null;

        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            boundsTemp = client.GetBounds(Con);
        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally { EditorUtility.ClearProgressBar(); }

        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;
        Debug.Log("Is there an error?");
        if (boundsTemp == null)
        {
            Debug.LogWarning("Could not communicate to receive the database bounds.");
            return;
        }
        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};



        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;
        MapBoundaries mapboundaries;
        float height; //5f;
        if (go != null)
        {
            mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            Debug.Log(minmaxX + "\t" + minmaxY);
            //if (CorrectAspectRatio)
            //{
            //    //var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
            //    //minmaxY[1] = (float)(minmaxX[1] * aspectRatio);
            //    var latDist = CoordinateConvertor.GeoDistance(SelectedArea.minlat, SelectedArea.minlon, SelectedArea.maxlat, SelectedArea.minlon) * 1000;
            //    var lonDist = CoordinateConvertor.GeoDistance(SelectedArea.minlat, SelectedArea.minlon, SelectedArea.minlat, SelectedArea.maxlon) * 1000;
            //    Debug.Log("Latitude in kilometers: " + latDist + " ,in float: " + (float)latDist);
            //    Debug.Log("Longitude in kilometers: " + lonDist + " ,in float: " + (float)lonDist);
            //    minmaxX[1] = (float)latDist;
            //    minmaxY[1] = (float)lonDist;
            //    mapboundaries.minMaxX = minmaxX;
            //    mapboundaries.minMaxY = minmaxY;
            //}

            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            height = mapboundaries.BuildingHeight;
            if (height < 4)
                height = 4;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            //SelectedArea.minlat = 59.3675700;
            //SelectedArea.minlon = 18.0044500;
            //SelectedArea.maxlat = 59.3757400;
            //SelectedArea.maxlon = 18.0175600;
            //minmaxX = new float[] { 0, 40000f };
            //minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");


        string[] ways = null;
        string[] buildingsInRelationTable = null;

        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            if (!GenerateRoads)
            {
                string[][] buildingtag = new string[1][];
                buildingtag[0] = new string[] { "building", "" }; // NOTE: building tag in OSM is in lower case.

                ways = client.GetWayIdsWithTags(Con, SelectedArea, buildingtag);
                buildingsInRelationTable = client.GetRelationBuildingsInBound(Con, SelectedArea);
            }
            else if (!GenerateBuildings)
            {
                string[][] roadtag = new string[1][];
                roadtag[0] = new string[] { "highway", "" }; // NOTE: highway tag in OSM is in lower case.
                ways = client.GetWayIdsWithTags(Con, SelectedArea, roadtag);
            }
            else
            {
                ways = client.GetWayIdsInBound(Con, SelectedArea); //client.GetWayIdsWithIdCriteria(Con,"id>0 and id<101000");
                buildingsInRelationTable = client.GetRelationBuildingsInBound(Con, SelectedArea);
            }


        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally
        {
            EditorUtility.ClearProgressBar();
        }



        Debug.Log("Ways were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        GaPSLabs.Geometry.Vector3 MinPointOnMap = new GaPSLabs.Geometry.Vector3();
        MinPointOnMap.x = direction * MinPointOnArea[0];
        MinPointOnMap.y = 0;
        MinPointOnMap.z = MinPointOnArea[1];
        // Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        EditorUtility.SetDirty(go);


        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        if (buildingsInRelationTable != null)
            totalWays += buildingsInRelationTable.Length;
        int progress = 0;

        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();

        client.GetOSMmeshFromOsmId(null, MinPointOnMap, boundsTemp, mapboundaries.ToGapslabsMapProperties(mapboundaries.BuildingMinimumHeight, mapboundaries.BuildingMaximumHeight), Con, "Way"
            , mapboundaries.RemoveRedundantPointsOnTheSameLine, mapboundaries.RedundantPointErrorThreshold, mapboundaries.SegmentLines);
        try
        {
            foreach (var FirstWay in ways)
            {
                try
                {
                    if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                    {
                        var serverObject = client.GetOSMmeshFromOsmId(FirstWay, MinPointOnMap, boundsTemp, null, Con, "Way"
                            , mapboundaries.RemoveRedundantPointsOnTheSameLine, mapboundaries.RedundantPointErrorThreshold, mapboundaries.SegmentLines);
                        //var serverObjectBinary = client.GetOSMmeshFromOsmId2(FirstWay, MinPointOnMap, boundsTemp, null, Con);
                        //ProtoBuf.Serializer.Deserialize<GaPSLabs.Geometry.GameObject>()
                        if (serverObject != null)
                        {
                            var tag = serverObject.Name.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
                            var osmtype = serverObject.Name.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[3].ToLower();
                            if (tag == "1")
                                tag = "Line";
                            else if (tag == "2" && !osmtype.EndsWith("area"))
                                tag = "Building";
                            else if (tag == "2" && osmtype.EndsWith("area"))
                                tag = "Area";
                            serverObject.GenerateShapeFromServerObject(tag, true);
                        }
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                        break;
                    }
                }
                catch (System.Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    throw e;
                }

            }
            if (buildingsInRelationTable != null)
            {
                foreach (var FirstWay in buildingsInRelationTable)
                {
                    //#if SHOW_STUFF
                    //                if (FirstWay != "109728659")
                    //                    continue;
                    //#endif
                    try
                    {
                        if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                        {
                            //Debug.Log("Doing " + FirstWay);
                            var serverObject = client.GetOSMmeshFromOsmId(FirstWay, MinPointOnMap, boundsTemp, null, Con, "Relation"
                                , mapboundaries.RemoveRedundantPointsOnTheSameLine, mapboundaries.RedundantPointErrorThreshold, mapboundaries.SegmentLines);
                            //Debug.Log("Triangles:" + serverObject.mesh.triangles.Length);
                            //var serverObjectBinary = client.GetOSMmeshFromOsmId2(FirstWay, MinPointOnMap, boundsTemp, null, Con);
                            //ProtoBuf.Serializer.Deserialize<GaPSLabs.Geometry.GameObject>()
                            if (serverObject != null)
                            {
                                var tag = serverObject.Name.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
                                if (tag == "1")
                                    tag = "Line";
                                else if (tag == "2")
                                    tag = "Building";
                                else if (tag == "3") // A building that were generated out of a relation
                                    tag = "Building";
                                serverObject.GenerateShapeFromServerObject(tag, true);
                            }
                        }
                        else
                        {
                            EditorUtility.ClearProgressBar();
                            break;
                        }
                    }
                    catch (System.Exception e)
                    {
                        EditorUtility.ClearProgressBar();
                        Debug.LogException(e);
                    }

                }
            }
        }
        finally
        {
            client.Close();
        }
        EditorUtility.ClearProgressBar();


    }

    static void ImportOSMDataSQL(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings, bool CorrectAspectRatio = false)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;

        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        var Con = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : wcfCon;

        Debug.Log(Con);

        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 
        Debug.Log("Check if there are errors");
        BoundsWCF boundsTemp = null;
        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            boundsTemp = client.GetBounds(Con);
            Debug.Log("These are the bounds: " + boundsTemp.minlat + ", " + boundsTemp.minlon + ", " + boundsTemp.maxlat + ", " + boundsTemp.maxlon + ", ");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;
        //Debug.Log("Is there an error?");

        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;

        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;

        float height; //5f;
        if (go != null)
        {
            Debug.Log("going");
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;

            //Debug.Log("SelectedArea bounds: maxlon " + SelectedArea.maxlon + ", maxlat " + SelectedArea.maxlat + ", minlon " + SelectedArea.minlon + ", minlat " + SelectedArea.minlat);

            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;

            //Debug.Log("minmaxX " + minmaxX[0] + "," + minmaxX[1] + "minmaxY " + minmaxY[0] + "," + minmaxY[1]);

            if (CorrectAspectRatio)
            {
                var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
                minmaxY[1] = (float)(minmaxX[1] * aspectRatio);
            }

            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            height = mapboundaries.BuildingHeight;
            if (height < 4)
                height = 4;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            //SelectedArea.minlat = 59.3675700;
            //SelectedArea.minlon = 18.0044500;
            //SelectedArea.maxlat = 59.3757400;
            //SelectedArea.maxlon = 18.0175600;
            //minmaxX = new float[] { 0, 40000f };
            //minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");

        string[] ways = null;
        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            if (!GenerateRoads)
            {
                Debug.Log("Generating only buildings...");
                string[][] buildingtag = new string[1][];
                buildingtag[0] = new string[] { "building", "" }; // NOTE: building tag in OSM is in lower case.
                ways = client.GetWayIdsWithTags(Con, SelectedArea, buildingtag);
            }
            else
                if (!GenerateBuildings)
                {
                    Debug.Log("Generating only roads...");
                    string[][] roadtag = new string[1][];
                    roadtag[0] = new string[] { "highway", "" }; // NOTE: building tag in OSM is in lower case.
                    ways = client.GetWayIdsWithTags(Con, SelectedArea, roadtag);
                }
                else
                {
                    Debug.Log("Generating buildings and roads...");
                    Debug.Log(Con);
                    Debug.Log("SelectedArea bounds: maxlon " + SelectedArea.maxlon + ", maxlat " + SelectedArea.maxlat + ", minlon " + SelectedArea.minlon + ", minlat " + SelectedArea.minlat);

                    ways = client.GetWayIdsInBound(Con, SelectedArea); //client.GetWayIdsWithIdCriteria(Con,"id>0 and id<101000");

                    if (ways == null)
                        Debug.Log("Didn't received any ways");
                    else
                        Debug.Log(ways.Length);
                }


        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log("Ways were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        // Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);


        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();
        foreach (var FirstWay in ways)
        {

            try
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                {
                    WayNodes = client.GetWayNodes(FirstWay, Con); //new Way(FirstWay);
                    WayTags = client.GetWayTags(FirstWay, Con);
                    if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
                    {
                        tempPoints = new Vector3[WayNodes.Length];
                        int counter = 0;

                        foreach (var node in WayNodes)
                        {
                            var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                            // Testing the correct direction
                            tempPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
#if SHOW_STUFF
                            Debug.Log(tempPoints[counter].x + ", " + tempPoints[counter].z);
#endif
                            // tempPoints[counter]=new Vector3((float)result[0],0,(float)result[1])-MinPointOnMap;
                            counter++;
                        }
                        WayNodes = null;
                        var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
                        var highwayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "highway");
                        var onewayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "oneway").FirstOrDefault();
                        WayTags = null;
                        if (building.Count() != 0)
                        {
                            if (GenerateBuildings)
                            {
                                //Debug.Log("Current building: "+FirstWay);
                                if (GenerateBuildingShapes)
                                {
                                    // Check if it has overlapping start and ending points.
                                    // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                    // Checking for duplicates:
                                    List<Vector3> optimizedList = new List<Vector3>();
                                    for (int i = 0; i < tempPoints.Length; i++)
                                    {
                                        if (!optimizedList.Contains(tempPoints[i]))
                                            optimizedList.Add(tempPoints[i]);
                                    }
                                    if (optimizedList.Count <= 2)
                                    {
                                        // Buildings that are too small to show such as 76844368
                                        // http://www.openstreetmap.org/browse/way/76844368
                                        Debug.Log(
                                                        string.Format(
                                                                "A weird building were found and ignored.\nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                                , FirstWay)
                                                        );
                                        continue;
                                    }
                                    tempPoints = optimizedList.ToArray();
                                    //Debug.Log("# of points:"+tempPoints.Length);
                                    //Debug.Log("Current Building: "+FirstWay);

                                    //Debug.Log("Initializing the cutting");
                                    var p2d = tempPoints.ToCPoint2D();
                                    //Debug.Log("# of points 2D:"+p2d.Length);
                                    //							foreach(var ptemp in p2d)
                                    //								Debug.Log(ptemp.X +", "+ ptemp.Y);

                                    shp = new PolygonCuttingEar.CPolygonShape(p2d);

                                    //Debug.Log("Cutting...");
                                    //try 
                                    {
                                        shp.CutEar();
                                    }
                                    //catch (System.Exception shpExp)
                                    {

                                    }
                                    p2d = null;
                                    System.GC.Collect();

                                    var randHeight = Random.Range(-3f, height);
                                    var randMaterial = (randHeight > height / 2f) ? "BuildingTall" : randHeight < height / 2f ? "Building2" : "Building";
                                    shp.GenerateShapeUVedWithWalls_Balanced(
                                            LineDraw.OSMType.Polygon, FirstWay,
                                            "Building", "Building", randMaterial,
                                            height + randHeight, height + 7, true);

                                }
                                else
                                    draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay, "Building", "Building");
                            }
                        }
                        else
                        {
                            if (highwayType.Count() != 0)
                            {
                                if (GenerateRoads)
                                {
                                    var hwtype = highwayType.First();
                                    switch (hwtype.KeyValue[1])
                                    {
                                        case "cycleway":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.8f,0.8f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.CycleWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints,0.05f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.CycleWayMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.CycleWayMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "footway":
                                        case "path":
                                        case "pedestrian":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.4f,0.4f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.FootWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.025f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.FootWayMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.FootwayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.FootWayMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "steps":
                                            {

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.StepsMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "motorway":
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,1.6f,1.6f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.RoadMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.1f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.RoadMaterial.name);

                                                var calculatedRoadWidth = mapProperty.RoadWidth;
                                                if (onewayType == null)
                                                    calculatedRoadWidth = mapProperty.RoadWidth * 2;
                                                else if (onewayType.KeyValue[1].ToLower() == "0" | onewayType.KeyValue[1].ToLower() == "no")
                                                    calculatedRoadWidth = mapProperty.RoadWidth * 2;
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), calculatedRoadWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.RoadMaterial.name, 0f);

                                                break;
                                            }
                                    }
                                }

                            }
                            //else if (GenerateRoads)
                            //    draw.Draw(tempPoints, lineColor, lineColor2, LineWidth, LineWidth, LineDraw.OSMType.Line, FirstWay, null, "Line");
                        }
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }

        }

        EditorUtility.ClearProgressBar();


    }

    [System.Obsolete("This method is not used anymore. It is only being kept for education purposes.")]
    static void ImportOSMDataSQLUsingPoly2Tri(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings)
    {

        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;

        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        var Con = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : wcfCon;
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 
        Debug.Log("Check if there are errors");
        BoundsWCF boundsTemp = null;
        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            boundsTemp = client.GetBounds(Con);
        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally { EditorUtility.ClearProgressBar(); }

        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;
        Debug.Log("Is there an error?");

        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;

        float height; //5f;
        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            height = mapboundaries.BuildingHeight;
            if (height < 4)
                height = 4;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            //SelectedArea.minlat = 59.3675700;
            //SelectedArea.minlon = 18.0044500;
            //SelectedArea.maxlat = 59.3757400;
            //SelectedArea.maxlon = 18.0175600;
            //minmaxX = new float[] { 0, 40000f };
            //minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");


        string[] ways = null;
        try
        {
            EditorUtility.DisplayProgressBar("Getting database bounds...", "client.GetBounds", 0.5f);
            if (!GenerateRoads)
            {
                string[][] buildingtag = new string[1][];
                buildingtag[0] = new string[] { "building", "" }; // NOTE: building tag in OSM is in lower case.
                ways = client.GetWayIdsWithTags(Con, SelectedArea, buildingtag);
            }
            else
                if (!GenerateBuildings)
                {
                    string[][] roadtag = new string[1][];
                    roadtag[0] = new string[] { "highway", "" }; // NOTE: building tag in OSM is in lower case.
                    ways = client.GetWayIdsWithTags(Con, SelectedArea, roadtag);
                }
                else
                    ways = client.GetWayIdsInBound(Con, SelectedArea); //client.GetWayIdsWithIdCriteria(Con,"id>0 and id<101000");


        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally { EditorUtility.ClearProgressBar(); }



        Debug.Log("Ways were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        // Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);


        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();
        foreach (var FirstWay in ways)
        {

            try
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                {
                    WayNodes = client.GetWayNodes(FirstWay, Con); //new Way(FirstWay);
                    WayTags = client.GetWayTags(FirstWay, Con);
                    if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
                    {
                        tempPoints = new Vector3[WayNodes.Length];
                        int counter = 0;

                        foreach (var node in WayNodes)
                        {
                            var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                            // Testing the correct direction
                            tempPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
#if SHOW_STUFF
                            Debug.Log(tempPoints[counter].x + ", " + tempPoints[counter].z);
#endif
                            // tempPoints[counter]=new Vector3((float)result[0],0,(float)result[1])-MinPointOnMap;
                            counter++;
                        }
                        WayNodes = null;
                        var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
                        var highwayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "highway");
                        WayTags = null;
                        if (building.Count() != 0)
                        {
                            if (GenerateBuildings)
                            {
                                //Debug.Log("Current building: "+FirstWay);
                                if (GenerateBuildingShapes)
                                {
                                    // Check if it has overlapping start and ending points.
                                    // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                    // Checking for duplicates:
                                    List<Vector3> optimizedList = new List<Vector3>();
                                    for (int i = 0; i < tempPoints.Length; i++)
                                    {
                                        if (!optimizedList.Contains(tempPoints[i]))
                                            optimizedList.Add(tempPoints[i]);
                                    }
                                    if (optimizedList.Count <= 2)
                                    {
                                        // Buildings that are too small to show such as 76844368
                                        // http://www.openstreetmap.org/browse/way/76844368
                                        Debug.Log(
                                                        string.Format(
                                                                "A weird building were found and ignored.\nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                                , FirstWay)
                                                        );
                                        continue;
                                    }
                                    tempPoints = optimizedList.ToArray();
                                    //Debug.Log("# of points:"+tempPoints.Length);
                                    //Debug.Log("Current Building: "+FirstWay);
                                    var polypoints = tempPoints.Select(s => new Poly2Tri.PolygonPoint(s.x, s.z));
                                    Poly2Tri.Polygon poly = new Poly2Tri.Polygon(polypoints);
                                    Poly2Tri.P2T.Triangulate(poly);

                                    //Debug.Log("Cutting...");
                                    //try 
                                    {
                                        //    Poly2Tri.P2T.Triangulate(poly);
                                    }
                                    //catch (System.Exception shpExp)
                                    {

                                    }

                                    System.GC.Collect();

                                    var randHeight = Random.Range(-3f, height);
                                    var randMaterial = (randHeight > height / 2f) ? "BuildingTall" : randHeight < height / 2f ? "Building2" : "Building";
                                    poly.GenerateShapeUVedBalanced(
                                            LineDraw.OSMType.Polygon, FirstWay,
                                            "Building", "Building", randMaterial,
                                            height + randHeight, height + 7, true);
                                    //Debug.Log("The crap's been generated.");
                                }
                                else
                                    draw.Draw(tempPoints, buildingColor, buildingColor, BuildingWidth, BuildingWidth, LineDraw.OSMType.Line, FirstWay, "Building", "Building");
                            }
                        }
                        else
                        {
                            if (highwayType.Count() != 0)
                            {
                                if (GenerateRoads)
                                {
                                    var hwtype = highwayType.First();
                                    switch (hwtype.KeyValue[1])
                                    {
                                        case "cycleway":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.8f,0.8f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.CycleWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints,0.05f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.CycleWayMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.CycleWayMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "footway":
                                        case "path":
                                        case "pedestrian":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.4f,0.4f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.FootWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.025f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.FootWayMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.FootwayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.FootWayMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "steps":
                                            {

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.StepsMaterial.name, -0.01f);

                                                break;
                                            }
                                        case "motorway":
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,1.6f,1.6f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.RoadMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.1f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.RoadMaterial.name);

                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapProperty.RoadWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.RoadMaterial.name, 0f);

                                                break;
                                            }
                                    }
                                }

                            }
                            //else if (GenerateRoads)
                            //    draw.Draw(tempPoints, lineColor, lineColor2, LineWidth, LineWidth, LineDraw.OSMType.Line, FirstWay, null, "Line");
                        }
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }

        }

        EditorUtility.ClearProgressBar();


    }

    static void CalculateBoundaries(MapBoundaries t)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        Debug.Log(wcfCon);
        var b = client.GetBounds(wcfCon);
        Debug.Log("bound maxlon " + b.maxlon);
        var latDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.maxlat, b.minlon) * 1000;
        var lonDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.minlat, b.maxlon) * 1000;
        //var latDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.maxLat, t.minLon) * 1000;
        //var lonDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.minLat, t.maxLon) * 1000;
        Debug.Log("Latitude in kilometers: " + latDist + " ,in float: " + (float)latDist);
        Debug.Log("Longitude in kilometers: " + lonDist + " ,in float: " + (float)lonDist);
        var maxX = (float)latDist;
        var maxY = (float)lonDist;
        t.minMaxX[1] = maxX;
        t.minMaxY[1] = maxY;

        //Get the DB boundaries, so they can be used locally in play mode (without the need of WCF) -- Miguel R. C.
        var databaseBounds = client.GetBounds(wcfCon);
        t.dbBoundMinLat = databaseBounds.minlat;
        t.dbBoundMaxLat = databaseBounds.maxlat;
        t.dbBoundMinLon = databaseBounds.minlon;
        t.dbBoundMaxLon = databaseBounds.maxlon;
    }

    static void ImportOSMDataSQLToOBJ(bool GenerateBuildingShapes, bool GenerateRoads, bool GenerateBuildings, string filePath, bool CorrectAspectRatio = false)
    {
        ChooseConnection();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);


        // int StressTest=0;
        float height = 5f;
        Color buildingColor = new Color(0, 0, 255, 255);
        Color lineColor = Color.red;
        Color lineColor2 = Color.yellow;

        var go = GameObject.Find("AramGISBoundaries");
        // Aram.OSMParser.Bounds  bounds=new Aram.OSMParser.Bounds();
        // Note: These are the min max of values in the database. 

        Debug.Log(wcfCon);

        var boundsTemp = client.GetBounds(wcfCon);
        // Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
        //boundsTemp.maxlon=32;


        Debug.Log("Setting interpolation Boundary");

        // However, the real ranges for lat and lon are:
        // Latitude measurements range from 0° to (+/–)90°.
        // Longitude measurements range from 0° to (+/–)180°.
        // bounds.minlat=-90;
        // bounds.maxlat=90;
        // bounds.minlon=-180;
        // bounds.maxlon=180;



        //double[] minmaxX=new double[] {-60000f,120000f};
        //double[] minmaxY=new double[] {-60000f,120000f};

        float[] minmaxX; //=new double[] {0,20000f};
        float[] minmaxY; //=new double[] {0f,40000f};

        BoundsWCF SelectedArea = new BoundsWCF();
        float LineWidth = 0.4f;
        float BuildingWidth = 0.6f;


        if (go != null)
        {
            var mapboundaries = go.GetComponent<MapBoundaries>();
            SelectedArea.minlat = mapboundaries.minLat;
            SelectedArea.maxlat = mapboundaries.maxLat;
            SelectedArea.minlon = mapboundaries.minLon;
            SelectedArea.maxlon = mapboundaries.maxLon;
            minmaxX = mapboundaries.minMaxX;
            minmaxY = mapboundaries.minMaxY;
            if (CorrectAspectRatio)
            {
                var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
                minmaxY[1] = (float)(minmaxX[1] / aspectRatio);
            }
            LineWidth = mapboundaries.RoadLineThickness;
            BuildingWidth = mapboundaries.BuildingLineThickness;
            //buildingColor=mapboundaries.BuildingColor;
            //lineColor=mapboundaries.LineColorStart;
            //lineColor2=mapboundaries.LineColorEnd;
        }
        else
        {
            SelectedArea.minlat = 59.3675700;
            SelectedArea.minlon = 18.0044500;
            SelectedArea.maxlat = 59.3757400;
            SelectedArea.maxlon = 18.0175600;
            minmaxX = new float[] { 0, 40000f };
            minmaxY = new float[] { 0f, 40000f };
            throw new MissingComponentException("The AramGISBoundaries object is not found.");
        }

        Debug.Log("Getting ways...");

        string[] ways = client.GetWayIdsInBound(wcfCon, SelectedArea); //client.GetWayIdsWithIdCriteria(wcfCon,"id>0 and id<101000");
        Debug.Log("Way were received.");
        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, boundsTemp, minmaxX, minmaxY);

        // Testing the correct direction
        int direction = -1;
        Vector3 MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        // Vector3 MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
        go.GetComponent<MapBoundaries>().MinPointOnMap = MinPointOnMap;
        EditorUtility.SetDirty(go);

        Debug.Log("Number of Ways:" + ways.Length);


        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        int totalWays = ways.Length;
        int progress = 0;



        OsmNodeWCF[] WayNodes;
        TagWCF[] WayTags;
        Vector3[] tempPoints;
        PolygonCuttingEar.CPolygonShape shp;
        var mapProperty = go.GetComponent<MapBoundaries>();
        foreach (var FirstWay in ways)
        {

            try
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Importing data", "Generating OSM elements\t" + progress + "/" + totalWays, progress++ / (float)totalWays))
                {
                    WayNodes = client.GetWayNodes(FirstWay, wcfCon); //new Way(FirstWay);
                    WayTags = client.GetWayTags(FirstWay, wcfCon);
                    if (WayTags.Where(i => i.KeyValue[0].ToLower() == "landuse" || i.KeyValue[0].ToLower() == "building" || i.KeyValue[0].ToLower() == "highway").Count() != 0)
                    {
                        tempPoints = new Vector3[WayNodes.Length];
                        int counter = 0;

                        foreach (var node in WayNodes)
                        {
                            var result = SimpleInterpolation((float)node.lat, (float)node.lon, boundsTemp/*SelectedArea*/, minmaxX, minmaxY);
                            // Testing the correct direction
                            tempPoints[counter] = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
                            // tempPoints[counter]=new Vector3((float)result[0],0,(float)result[1])-MinPointOnMap;
                            counter++;
                        }
                        WayNodes = null;
                        var building = WayTags.Where(i => i.KeyValue[0].ToLower() == "building");
                        var highwayType = WayTags.Where(i => i.KeyValue[0].ToLower() == "highway");
                        WayTags = null;
                        if (building.Count() != 0)
                        {
                            if (GenerateBuildings)
                            {
                                //Debug.Log("Current building: "+FirstWay);
                                if (GenerateBuildingShapes)
                                {
                                    // Check if it has overlapping start and ending points.
                                    // NOTE: Replaced with the code to remove all the duplicates, not only the endpoints.							
                                    // Checking for duplicates:
                                    List<Vector3> optimizedList = new List<Vector3>();
                                    for (int i = 0; i < tempPoints.Length; i++)
                                    {
                                        if (!optimizedList.Contains(tempPoints[i]))
                                            optimizedList.Add(tempPoints[i]);
                                    }
                                    if (optimizedList.Count <= 2)
                                    {
                                        // Buildings that are too small to show such as 76844368
                                        // http://www.openstreetmap.org/browse/way/76844368
                                        Debug.Log(
                                                        string.Format(
                                                                "A weird building were found and ignored.\nRelated url: http://www.openstreetmap.org/browse/way/{0}"
                                                                , FirstWay)
                                                        );
                                        continue;
                                    }
                                    tempPoints = optimizedList.ToArray();
                                    //Debug.Log("# of points:"+tempPoints.Length);
                                    //Debug.Log("Current Building: "+FirstWay);

                                    //Debug.Log("Initializing the cutting");
                                    var p2d = tempPoints.ToCPoint2D();
                                    //Debug.Log("# of points 2D:"+p2d.Length);
                                    //							foreach(var ptemp in p2d)
                                    //								Debug.Log(ptemp.X +", "+ ptemp.Y);

                                    shp = new PolygonCuttingEar.CPolygonShape(p2d);

                                    //Debug.Log("Cutting...");
                                    //try 
                                    {
                                        shp.CutEar();
                                    }
                                    //catch (System.Exception shpExp)
                                    {

                                    }
                                    p2d = null;
                                    System.GC.Collect();

                                    var randHeight = Random.Range(-3f, +7f);
                                    var randMaterial = randHeight > 5 ? "BuildingTall" : randHeight < 0 ? "Building2" : "Building";
                                    var GeneratedGameObject = shp.GenerateShapeUVedWithWalls_BalancedAndReturn(
                                            LineDraw.OSMType.Polygon, FirstWay,
                                            "Building", "Building", randMaterial,
                                            height + randHeight, height + 7, true);
                                    var FileName = filePath + "\\" + GeneratedGameObject.name.Replace("|", "_");
                                    SaveToDisk.SaveObjToDisk(GeneratedGameObject.GetComponent<MeshFilter>(), FileName + ".obj");
                                    SaveToDisk.SaveMetaData(GeneratedGameObject.GetComponent<Renderer>().sharedMaterial.name, FileName + ".material");
                                    Debug.Log(FileName);
                                    GameObject.DestroyImmediate(GeneratedGameObject);
                                    //Debug.Log("The crap's been generated.");
                                }
                                //else
                                //draw.Draw(tempPoints, buildingColor,buildingColor,BuildingWidth,BuildingWidth,LineDraw.OSMType.Line,FirstWay,"Building","Building");
                            }
                        }
                        else
                        {
                            if (highwayType.Count() != 0)
                            {
                                if (GenerateRoads)
                                {
                                    var hwtype = highwayType.First();
                                    switch (hwtype.KeyValue[1])
                                    {
                                        case "cycleway":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.8f,0.8f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.CycleWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints,0.05f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.CycleWayMaterial.name);
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(2f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.CycleWayMaterial.name, -0.01f);
                                                break;
                                            }
                                        case "footway":
                                        case "path":
                                        case "pedestrian":
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,0.4f,0.4f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.FootWayMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.025f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.FootWayMaterial.name);
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.FootwayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.FootWayMaterial.name, -0.01f);
                                                break;
                                            }
                                        case "steps":
                                            {
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(4f), mapProperty.CyclewayWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.StepsMaterial.name, 0.01f);
                                                break;
                                            }
                                        case "motorway":
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                //draw.DrawUnscaled(tempPoints, lineColor,lineColor2,1.6f,1.6f,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"Line",mapProperty.RoadMaterial.name);
                                                //draw.TestMeshGenerationSimple(tempPoints, 0.1f,1,LineDraw.OSMType.Line,FirstWay,hwtype.KeyValue[1],"PATHTEST",mapProperty.RoadMaterial.name);
                                                LineDraw.MeshGenerationFilledCorners(tempPoints.ToSegmentedPoints(0.5f), mapProperty.RoadWidth, LineDraw.OSMType.Line, FirstWay, hwtype.KeyValue[1], "Line", mapProperty.RoadMaterial.name, 0f);
                                                break;
                                            }
                                    }
                                }

                            }
                            else if (GenerateRoads)
                                draw.Draw(tempPoints, lineColor, lineColor2, LineWidth, LineWidth, LineDraw.OSMType.Line, FirstWay, null, "Line");
                        }
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }

        }

        EditorUtility.ClearProgressBar();

    }

    public static float[] SimpleInterpolation(float PositionLat, float PositionLon, GapslabWCFservice.BoundsWCF Boundings, float[] MinMaxX, float[] MinMaxY)
    {
        return CoordinateConvertor.SimpleInterpolation(PositionLat, PositionLon, Boundings, MinMaxX, MinMaxY);
        //float Y = (float)((PositionLon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxY[1] - MinMaxY[0]));
        //float X = (float)((PositionLat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxX[1] - MinMaxX[0]));
        //return new float[] { X, Y };
    }

    public static float[] SimpleInterpolation(float PositionLat, float PositionLon, Aram.OSMParser.Bounds Boundings, float[] MinMaxX, float[] MinMaxY)
    {
        // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
        // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPixel)
        float Y = (float)((PositionLon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxY[1] - MinMaxY[0]));
        float X = (float)((PositionLat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxX[1] - MinMaxX[0]));
        return new float[] { X, Y };
    }

    [System.Obsolete("This method is deprecated. Use GroupObjectsWithTags() instead.")]
    static void GroupTrafficLights()
    {
        var globalO = GameObject.Find("AramGISBoundaries");
        var mapproperties = globalO.GetComponent<MapBoundaries>();
        GameObject go = new GameObject("TrafficLight");
        var gos = GameObject.FindGameObjectsWithTag("TrafficLight");
        for (int i = 0; i < gos.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Grouping in progress", "Grouping all the traffic lights\t" + i + "/" + gos.Length, i / (float)gos.Length))
            {
                gos[i].transform.parent = go.transform;
            }
            else
            {
                EditorUtility.ClearProgressBar();
                break;
            }
        }
        go.transform.localScale = new Vector3(mapproperties.Scale.x, 1, mapproperties.Scale.y);
        EditorUtility.ClearProgressBar();
    }
}