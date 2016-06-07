/*
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

/// <summary>
/// This script controls the visualization of the VVIS data into the Unity scene. 
/// </summary>
public class VVisVisualizer : MonoBehaviour
{
    public Material highlightMaterial;

    public Color highlightGradientUp;
    public Color highlightGradientDown;

    private VVisDataTable vVisData;
    private VVisDataSQLite vVisSQLite;
    private VVisUIController vVisUICtrl;

    private Dictionary<string, GameObject> roadDictionary;
    private List<GameObject> currentRoadSelection;
    private GameObject[] lines;

    private Thread thread;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        vVisData = FindObjectOfType<VVisDataTable>();

        vVisSQLite = FindObjectOfType<VVisDataSQLite>();
        vVisUICtrl = FindObjectOfType<VVisUIController>();

        lines = GameObject.FindGameObjectsWithTag("Line");
        roadDictionary = new Dictionary<string, GameObject>();
        currentRoadSelection = new List<GameObject>();
    }

    /// <summary>
    /// Start method. 
    /// </summary>
    void Start()
    {
        CreateRoadDictionary();
        PrepareRoadsForVisualization();
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
            CleanRoadSelection();
            vVisSQLite.RequestQuery(this.gameObject, queryObj);
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
            vVisUICtrl.WriteDebugText("Error in query: the query requested could not be executed.", true);
        }
        else
        {
            vVisUICtrl.WriteDebugText("Query executed successfully!");
            VisualizeRoads(queryResults.queryResults, queryResults.visualizeWithGradient, queryResults.color);
        }
    }

    /// <summary>
    /// Visualizes in the scene the selected roads. 
    /// </summary>
    /// <param name="selectedDataRows">List of rows containing the osm_id of the roads selected.</param>
    internal void VisualizeRoads(List<string> selectedDataRows, bool useGradient, Color color)
    {
        Debug.Log("Total roads passed: " + selectedDataRows.Count);

        float interpolationIncrement = 1 / (float) selectedDataRows.Count;
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

                MeshRenderer roadRenderer = roadDictionary[osmId].GetComponent<MeshRenderer>();

                roadRenderer.enabled = true;

                // Implement transparency (TODO)
                // if (roadRenderer.material.color == Color.red)
                //     roadRenderer.material.color = Color.magenta;
                // else
                //     roadRenderer.material.color = currentColor;

                roadRenderer.material.color = currentColor;

                // Add the road to the current road selection
                currentRoadSelection.Add(roadDictionary[osmId]);
            }
            else
            {
                Debug.Log("Road " + osmId + " was not found");
            }
        }

        Debug.Log("Total roads drawn: " + currentRoadSelection.Count);
    }

    /// <summary>
    /// Cleans from the visualization the current road selection.
    /// </summary>
    private void CleanRoadSelection()
    {
        // Disable roads from the old selection
        foreach (GameObject G in currentRoadSelection)
            G.GetComponent<MeshRenderer>().enabled = false;

        // Clean old selection list
        currentRoadSelection.Clear();
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
                CleanRoadSelection();
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
            CleanRoadSelection();
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
                currentRoadSelection.Add(roadDictionary[osmId]);
            }
        }
    }
}
