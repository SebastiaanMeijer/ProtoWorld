/*
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

    [Range(0f, 60f)]
    public float debugMessageTime = 5.0f;

    public Text infoTag;
    public InputField inputField;
    public Dropdown queryDropdown;
    public Button selectButton;
    public Text debugText;

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
                FillQueryList();
            }
        }
        else
        {
            FillQueryList();
        }

        FillDropDownList();
    }

    /// <summary>
    /// Runs the query selected in the dropdown list. 
    /// </summary>
    public void SelectButtonClicked_RunQuerySelected()
    {
        vVisVisualizer.VisualizeQuery(queryListObj.queries[queryDropdown.value]);
    }

    /// <summary>
    /// Fills the query list to be used in the user interface.
    /// </summary>
    private void FillQueryList()
    {
        queryListObj.queries.Add(new VVisQueryRequest("QUERY 1", "select count(distinct vid) as uniqueVcount, osm_id from trajectory_paths group by osm_id order by uniqueVcount desc limit 100"));
        queryListObj.queries.Add(new VVisQueryRequest("QUERY 2", "select count(*) as count, osm_id from trajectory_paths group by osm_id  order by count desc limit 100"));
        queryListObj.queries.Add(new VVisQueryRequest("QUERY ERROR", "select count as count, osm_id from trajery_paths group by os_id  order b"));
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
    /// Changes the mode of the VVIS UI interface between query mode and calculating mode.
    /// </summary>
    /// <param name="isCalculatingMode">If true, the UI will change to calculating mode. 
    /// Otherwise, it will change to query mode.</param>
    internal void UICalculatingMode(bool isCalculatingMode)
    {
        if (infoTag != null && selectButton != null)
        {
            if (isCalculatingMode)
            {
                infoTag.GetComponent<FadingElementUI>().fadeInCanvas();
                selectButton.enabled = false;
            }
            else
            {
                infoTag.GetComponent<FadingElementUI>().fadeOutCanvas();
                selectButton.enabled = true;
            }
        }
    }

    /// <summary>
    /// Writes a new message in the debug textbox. 
    /// </summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="isError">True if the message is an error.</param>
    internal void WriteDebugText(string message, bool isError = false)
    {
        if (debugText != null && showDebugText)
        {
            debugText.text = message;

            if (isError)
                debugText.color = Color.red;
            else
                debugText.color = Color.green;
        }

        Invoke("CleanDebugText", debugMessageTime);
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
