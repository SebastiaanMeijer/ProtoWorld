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
 * VVisUIController.cs
 * Miguel Ramos Carretero
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the VVIS User Interface.
/// </summary>
public class VVisUIController : MonoBehaviour
{
    public bool loadQueriesFromXML = false;

    public string xmlFilePathFromStreamingAssets = "";

    public bool showDebugText = false;

    public Text infoTag;
    public InputField inputField;
    public Dropdown queryDropdown;
    public Button selectButton;
    public Button cleanQueryButton;
    public Button cleanSelectionButton;
    public Toggle drawToggle;
    public Text debugText;
    public GameObject captionBox;

    private string fullxmlFilePath = "";
    private VVisQueryList queryListObj;
    private VVisVisualizer vVisVisualizer;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        vVisVisualizer = FindObjectOfType<VVisVisualizer>();
        queryListObj = new VVisQueryList();

        if (loadQueriesFromXML)
        {
            Debug.Log("Loading queries from XML File");

            fullxmlFilePath = Application.streamingAssetsPath + xmlFilePathFromStreamingAssets;

            if (File.Exists(fullxmlFilePath))
            {
                //Read the query list deserializing from the XML file
                using (var stream = new FileStream(fullxmlFilePath, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(VVisQueryList));
                    queryListObj = serializer.Deserialize(stream) as VVisQueryList;
                }
            }
            else
            {
                Debug.LogError("The XML file to load the queries could not be found, loading default queries...");
                FillDefaultQueryList();
            }
        }
        else
        {
            FillDefaultQueryList();
        }

        FillDropDownList();
        UpdateCaptionBox();

        // Set event for caption box
        queryDropdown.onValueChanged.AddListener(delegate { UpdateCaptionBox(); });
    }

    /// <summary>
    /// Runs the query selected in the dropdown list. 
    /// </summary>
    public void SelectButtonClicked_RunQuerySelected()
    {
        vVisVisualizer.VisualizeQuery(queryListObj.queries[queryDropdown.value]);
    }

    /// <summary>
    /// Cleans the current query selection on the map.
    /// </summary>
    public void CleanQueryButtonClicked()
    {
        vVisVisualizer.CleanQueryRoadSelection();
        vVisVisualizer.HideFixedPoints();
    }

    /// <summary>
    /// Shows the fixed points loaded on the map.
    /// </summary>
    public void PointButtonClicked()
    {
        vVisVisualizer.ShowFixedPoints();
    }

    /// <summary>
    /// Cleans the current player selection on the map.
    /// </summary>
    public void CleanPlayerSelectionButtonClicked()
    {
        vVisVisualizer.CleanPlayerRoadSelection();
    }

    /// <summary>
    /// Enables/disables drawing (player selection) on the map. 
    /// </summary>
    /// <param name="value">True if drawing should be allowed.</param>
    public void PlayerSelectionToggleClicked(bool value)
    {
        vVisVisualizer.AllowPlayerSelection(value);
    }

    /// <summary>
    /// Fills the query list to be used in the user interface (for testing purposes).
    /// </summary>
    private void FillDefaultQueryList()
    {
        queryListObj.queries.Add(new VVisQueryRequest("QUERY 1", "select count(distinct vid) as uniqueVcount, osm_id from trajectory_paths group by osm_id order by uniqueVcount desc limit 100", "Default query info 1"));
        queryListObj.queries.Add(new VVisQueryRequest("QUERY 2", "select count(*) as count, osm_id from trajectory_paths group by osm_id  order by count desc limit 100", "Default query info 2"));
        queryListObj.queries.Add(new VVisQueryRequest("QUERY ERROR", "select count as count, osm_id from trajery_paths group by os_id  order b", "Default query info ERROR"));
    }

    /// <summary>
    /// Fills the dropdown list of the UI with the queries defined in the queryList property. 
    /// </summary>
    private void FillDropDownList()
    {
        if (queryDropdown != null)
        {
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();

            foreach (VVisQueryRequest Q in queryListObj.queries)
            {
                optionList.Add(new Dropdown.OptionData(Q.name));
            }

            queryDropdown.AddOptions(optionList);
        }
    }

    /// <summary>
    /// Updates the captions for the dropdown menu with the text infor from the queries.
    /// </summary>
    private void UpdateCaptionBox()
    {
        if (captionBox != null && captionBox.activeSelf)
        {
            captionBox.GetComponent<FadingElementUI>().fadeInCanvas();
            captionBox.GetComponentInChildren<Text>().text = queryListObj.queries[queryDropdown.value].info;
        }
    }

    /// <summary>
    /// Changes the mode of the VVIS UI interface between query mode and calculating mode.
    /// </summary>
    /// <param name="isCalculatingMode">If true, the UI will change to calculating mode. 
    /// Otherwise, it will change to query mode.</param>
    internal void UICalculatingMode(bool isCalculatingMode)
    {
        if (isCalculatingMode)
        {
           WriteDebugText("Running query...", Color.white, 0f);

            if (selectButton != null)
                selectButton.enabled = false;

            if (cleanQueryButton != null)
                cleanQueryButton.enabled = false;

            if (drawToggle != null)
                drawToggle.enabled = false;

            if (cleanSelectionButton != null)
                cleanSelectionButton.enabled = false;
        }
        else
        {
            if (selectButton != null)
                selectButton.enabled = true;

            if (cleanQueryButton != null)
                cleanQueryButton.enabled = true;

            if (drawToggle != null)
                drawToggle.enabled = true;

            if (cleanSelectionButton != null)
                cleanSelectionButton.enabled = true;
        }
    }

    /// <summary>
    /// Writes a new message in the debug textbox. 
    /// </summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="color">Color of the message.</param>
    /// <param name="seconds">Seconds before fade out. If 0, it will stay until 
    /// a new message is written.</param>
    internal void WriteDebugText(string message, Color color, float seconds)
    {
        CancelInvoke("CleanDebugText");

        if (debugText != null && showDebugText)
        {
            debugText.text = message;
            debugText.color = color;
        }

        if (seconds > 0f)
            Invoke("CleanDebugText", seconds);
    }

    /// <summary>
    /// Wrapping method for WriteDebugText. 
    /// Writes an informative text in the debug panel for 5 seconds.
    /// </summary>
    /// <param name="message">Text to appear in the debug panel.</param>
    public void WriteSimpleMessage(string message)
    {
        WriteDebugText(message, Color.green, 5.0f);
    }

    /// <summary>
    /// Cleans the debug textbox.
    /// </summary>
    internal void CleanDebugText()
    {
        if (debugText != null && showDebugText)
            debugText.text = "";
    }

    /// <summary>
    /// Handles clicks on the select button, considering the field input as the id of a single trajectory.
    /// </summary>
    [Obsolete("Not used anymore in the newer version of VVIS (0.3 and higher).")]
    public void SelectButtonClicked_SelectSingleTrajectory()
    {
        if (vVisVisualizer != null && inputField != null)
            vVisVisualizer.VisualizeTrajectory(inputField.text);
    }

    /// <summary>
    /// Handles clicks on the select button, considering the field input as a general selection expression.
    /// </summary>
    [Obsolete("Not used anymore in the newer version of VVIS (0.3 and higher).")]
    public void SelectButtonClicked_GeneralSelectionQuery()
    {
        if (vVisVisualizer != null && inputField != null)
            vVisVisualizer.VisualizeSelection(inputField.text);
    }
}
