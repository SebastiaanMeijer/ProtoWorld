/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * VIN VISUALIZATION
 * VVisVisualizer.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System;
using System.Collections;

/// <summary>
/// This script controls the visualization of the VVIS data into the Unity scene. 
/// </summary>
public class VVisVisualizer : MonoBehaviour
{
    public LayerMask drawingMask;
    public Material highlightMaterial;
    public Color drawingColor;
    public Color highlightGradientUp;
    public Color highlightGradientDown;

    [Range(0f, 60f)]
    public float debugMessageTime = 5.0f;

    private bool playerSelectionActive = false;

    private VVisDataTable vVisData;
    private VVisDataSQLite vVisSQLite;
    private VVisFixPointsSQLite vVisFixPoints;
    private VVisUIController vVisUICtrl;

    private Dictionary<string, GameObject> roadDictionary;

    private List<GameObject> queryRoadSelection;

    private List<GameObject> playerRoadSelection;

    private GameObject[] lines;

    private Thread thread;

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private LoggerAssembly loggerAssembly;

    private MultitouchController multitouch;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        vVisData = FindObjectOfType<VVisDataTable>();

        vVisSQLite = FindObjectOfType<VVisDataSQLite>();
        vVisUICtrl = FindObjectOfType<VVisUIController>();
        vVisFixPoints = FindObjectOfType<VVisFixPointsSQLite>();

        multitouch = FindObjectOfType<MultitouchController>();

        loggerAssembly = FindObjectOfType<LoggerAssembly>();

        lines = GameObject.FindGameObjectsWithTag("Line");
        roadDictionary = new Dictionary<string, GameObject>();
        queryRoadSelection = new List<GameObject>();
        playerRoadSelection = new List<GameObject>();
    }

    /// <summary>
    /// Start method. 
    /// </summary>
    void Start()
    {
        CreateRoadDictionary();
        PrepareRoadsForVisualization();
        StartCoroutine(PlayerSelectionCoroutine());
    }

    /// <summary>
    /// Coroutine to control the player selection of roads.
    /// </summary>
    IEnumerator PlayerSelectionCoroutine()
    {
        while (true)
        {
            bool makeSelection = false;
            bool allowDeselecting = false;

            if (playerSelectionActive)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // Single click allows deselection
                    makeSelection = true;
                    allowDeselecting = true;
                }
                else if (Input.GetMouseButton(0))
                {
                    // Dragging do not allow deselection
                    makeSelection = true;
                    allowDeselecting = false;
                }

                if (makeSelection)
                {
                    RaycastHit hit;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, drawingMask))
                    {
                        string osmId = hit.transform.gameObject.name.Split('|')[2];
                        RoadClickedByPlayer(osmId, allowDeselecting);
                    }

                    if (allowDeselecting)
                        yield return new WaitForSeconds(0.2f); // This reduces sensivity of single clicks
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// Adds / removes the clicked road from the player road selection.
    /// </summary>
    /// <param name="road">OSM id of the road clicked by the player.</param>
    internal void RoadClickedByPlayer(string osmId, bool allowDeselecting)
    {
        Debug.Log("Road " + osmId + " clicked");

        GameObject road = roadDictionary[osmId];

        if (road != null)
        {
            if (playerRoadSelection.Contains(road))
            {
                if (allowDeselecting)
                {
                    // If the road is in the player selection: deselect
                    Debug.Log("Deselecting road");
                    DeselectRoad(osmId);
                    playerRoadSelection.Remove(road);

                    if (loggerAssembly != null && loggerAssembly.logVVis)
                        log.Info("Road " + osmId + " deselected");
                }
            }
            else
            {
                // Else: select
                Debug.Log("Selecting road");
                HighlightRoad(osmId, drawingColor);
                playerRoadSelection.Add(road);

                if (loggerAssembly != null && loggerAssembly.logVVis)
                    log.Info("Road " + osmId + " selected");
            }
        }
    }

    /// <summary>
    /// Allows / restricts player selection of roads in the map.
    /// </summary>
    /// <param name="value">True if player selection should be allowed.</param>
    internal void AllowPlayerSelection(bool value)
    {
        playerSelectionActive = value;

        // Restricts touch navigation while drawing
        multitouch.BlockNavigation(value);
    }

    /// <summary>
    /// Visualizes a certain query from the vVis Data.
    /// </summary>
    /// <param name="vVisQuery">Query object with a select query.</param>
    internal void VisualizeQuery(VVisQueryRequest queryObj)
    {
        if (vVisSQLite != null && vVisSQLite.IsDBReady())
        {
            vVisUICtrl.UICalculatingMode(true);
            CleanQueryRoadSelection();
            vVisSQLite.RequestQuery(this.gameObject, queryObj);

            //Log info
            if (loggerAssembly != null && loggerAssembly.logVVis)
                log.Info("Running query " + queryObj.name);
        }
    }

    /// <summary>
    /// Callback from the query request. This method is activated via a message from 
    /// vVisDataSQLite.
    /// </summary>
    /// <param name="queryResults">Object containing the results of the query and 
    /// how to visualize them</param>
    internal void RequestQuery_CallBack(VVISQueryResults queryResults)
    {
        vVisUICtrl.UICalculatingMode(false);

        if (queryResults.queryError)
        {
            vVisUICtrl.WriteDebugText("Error in query: the query requested could not be executed.", Color.red, debugMessageTime);

            //Log error
            if (loggerAssembly != null && loggerAssembly.logVVis)
                log.Error("Error in query: The query requested could not be executed");
        }
        else
        {
            vVisUICtrl.WriteDebugText("Query executed successfully!", Color.green, debugMessageTime);

            VisualizeRoads(queryResults.queryResults, queryResults.visualizeWithGradient, queryResults.color);

            //Log info
            if (loggerAssembly != null && loggerAssembly.logVVis)
                log.Info("Query executed successfully");
        }
    }

    /// <summary>
    /// Visualizes in the scene the selected roads. 
    /// </summary>
    /// <param name="selectedDataRows">List of rows containing the osm_id of the roads selected.</param>
    internal void VisualizeRoads(List<string> selectedDataRows, bool useGradient, Color color)
    {
        Debug.Log("Total roads passed: " + selectedDataRows.Count);

        float interpolationIncrement = 1 / (float)selectedDataRows.Count;
        Color currentColor = color;

        // Highlight new road selection
        for (int i = 0; i < selectedDataRows.Count; i++)
        {
            string osmId = selectedDataRows[i].ToString();

            if (roadDictionary.ContainsKey(osmId))
            {
                if (useGradient)
                {
                    float blueComponent = Mathf.Lerp(highlightGradientUp.b, highlightGradientDown.b, i * interpolationIncrement);
                    float redComponent = Mathf.Lerp(highlightGradientUp.r, highlightGradientDown.r, i * interpolationIncrement);
                    float greenComponent = Mathf.Lerp(highlightGradientUp.g, highlightGradientDown.g, i * interpolationIncrement);
                    float alphaComponent = Mathf.Lerp(highlightGradientUp.a, highlightGradientDown.a, i * interpolationIncrement);

                    currentColor = new Color(redComponent, greenComponent, blueComponent, alphaComponent);
                }

                HighlightRoad(osmId, currentColor);

                // Add the road to the current road selection
                queryRoadSelection.Add(roadDictionary[osmId]);
            }
            else
            {
                Debug.Log("Road " + osmId + " was not found");
            }
        }

        Debug.Log("Total roads drawn: " + queryRoadSelection.Count);
    }

    /// <summary>
    /// Highlights a given road with a color. 
    /// </summary>
    /// <param name="osmId">OSM id of the road.</param>
    /// <param name="color">Color for highlighting.</param>
    internal void HighlightRoad(string osmId, Color color)
    {
        MeshRenderer roadRenderer = roadDictionary[osmId].GetComponent<MeshRenderer>();
        roadRenderer.enabled = true;
        roadRenderer.material.color = color;
    }

    /// <summary>
    /// Deselects a given road. 
    /// </summary>
    /// <param name="osmId">OSM id of the road.</param>
    internal void DeselectRoad(string osmId)
    {
        roadDictionary[osmId].GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// Cleans from the visualization the current query road selection.
    /// </summary>
    internal void CleanQueryRoadSelection()
    {
        // Disable roads from the old selection
        foreach (GameObject G in queryRoadSelection)
            G.GetComponent<MeshRenderer>().enabled = false;

        // Clean old selection list
        queryRoadSelection.Clear();

        // Log info
        if (loggerAssembly != null && loggerAssembly.logVVis)
            log.Info("Cleaning query selection");
    }

    /// <summary>
    /// Cleans from the visualization the current player road selection.
    /// </summary>
    internal void CleanPlayerRoadSelection()
    {
        // Disable roads from the old selection
        foreach (GameObject G in playerRoadSelection)
            G.GetComponent<MeshRenderer>().enabled = false;

        // Clean old selection list
        playerRoadSelection.Clear();

        // Log info
        if (loggerAssembly != null && loggerAssembly.logVVis)
            log.Info("Cleaning drawing selection");
    }

    /// <summary>
    /// Hides the fixed points loaded from the SQLite DB.
    /// </summary>
    internal void HideFixedPoints()
    {
        if (vVisFixPoints != null)
            vVisFixPoints.HideLoadedPoints();
    }

    /// <summary>
    /// Shows the fixed points loaded from the SQLite DB.
    /// </summary>
    internal void ShowFixedPoints()
    {
        if (vVisFixPoints != null)
            vVisFixPoints.ShowLoadedPoints();
    }

    /// <summary>
    /// Creates a road dictionary based on the road objects in the scene.
    /// </summary>
    private void CreateRoadDictionary()
    {
        UnityEngine.Debug.Log("Creating road dictionary...");

        foreach (GameObject L in lines)
        {
            string osmId = L.name.Split('|')[2];
            roadDictionary.Add(osmId, L);
        }

        UnityEngine.Debug.Log("Road dictionary completed");
        UnityEngine.Debug.Log("Number of roads: " + roadDictionary.Count);
    }

    /// <summary>
    /// Prepare the roads of the map for the visualization of trajectories.
    /// </summary>
    private void PrepareRoadsForVisualization()
    {
        foreach (GameObject road in roadDictionary.Values)
        {
            // Move the road 5 units up (to avoid z buffer conflict with the base layer)
            road.transform.position = new Vector3(road.transform.position.x,
                road.transform.position.y + 5, road.transform.position.z);

            // Give the highlight material to the road
            if (highlightMaterial != null)
                road.GetComponent<MeshRenderer>().material = highlightMaterial;
        }
    }

    /// <summary>
    /// Visualizes a certain trajectory from the vVis data. 
    /// </summary>
    /// <param name="tidString">Id of the trajectory.</param>
    [Obsolete("Not used anymore in the newer version of VVIS (0.3 and higher).")]
    internal void VisualizeTrajectory(string tidString)
    {
        int tidInt;

        if (int.TryParse(tidString, out tidInt))
        {
            if (vVisData != null && vVisData.IsParsingCompleted())
            {
                vVisUICtrl.UICalculatingMode(true);
                vVisData.RequestDataSelection(this.gameObject, "tid = " + tidString);
                CleanQueryRoadSelection();
            }
        }
        else
            UnityEngine.Debug.LogWarning("The given tid is not valid, no selection could be made.");
    }

    /// <summary>
    /// Visualizes a selection of trajectories from the vVis data. 
    /// </summary>
    /// <param name="queryString">Query string defining the selection expression.</param>
    [Obsolete("Not used anymore in the newer version of VVIS (0.3 and higher).")]
    internal void VisualizeSelection(string queryString)
    {
        if (vVisData != null && vVisData.IsParsingCompleted())
        {
            vVisUICtrl.UICalculatingMode(true);
            CleanQueryRoadSelection();
            vVisData.RequestDataSelection(this.gameObject, queryString);
        }
    }

    /// <summary>
    /// Callback from the selection request. This method is activated via a message from vVisDataTable.
    /// </summary>
    [Obsolete("Not used anymore in the newer version of VVIS (0.3 and higher).")]
    internal void RequestDataSelection_CallBack(DataRow[] rows)
    {
        vVisUICtrl.UICalculatingMode(false);
        VisualizeRoads(rows);
    }

    /// <summary>
    /// Visualizes in the scene the selected roads. 
    /// </summary>
    /// <param name="selectedDataRows">List of rows containing the osm_id of the roads selected.</param>
    [Obsolete("Not used anymore in the newer version of VVIS (0.3 and higher).")]
    private void VisualizeRoads(DataRow[] selectedDataRows)
    {
        // Highlight new road selection
        for (int i = 0; i < selectedDataRows.Length; i++)
        {
            string osmId = selectedDataRows[i]["osm_id"].ToString();

            if (roadDictionary.ContainsKey(osmId))
            {
                MeshRenderer roadRenderer = roadDictionary[osmId].GetComponent<MeshRenderer>();
                roadRenderer.enabled = true;

                // Add the road to the current road selection
                queryRoadSelection.Add(roadDictionary[osmId]);
            }
        }
    }
}
