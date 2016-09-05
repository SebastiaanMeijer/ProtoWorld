using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Xml;

public class HistoricalDataController : MonoBehaviour {

    public XmlDocument logFile;
    private bool fileBrowserOpen;
    private string savedLogFilePath;
    private string loadedLogFilePath;
    private string loadedLogfileContents;
    string logFileName;

    // Use this for initialization
    void Start () {
        //timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        logFile = new XmlDocument();
        fileBrowserOpen = false;

        XmlNode declaration = logFile.CreateXmlDeclaration("1.0", "UTF-8", null);
        logFile.AppendChild(declaration);
	}
	
	// Update is called once per frame
	void Update () {
        if (!fileBrowserOpen)
        {
            writeToLogFile(null);
        }
	}

    public void writeToLogFile(string content)
    {
        //XmlElement element = logFile.CreateElement(String.Empty,"test",String.Empty);
        //logFile.AppendChild(element);
        //logFile.create
        //XmlElement root = logFile.DocumentElement;
        ////XmlNode node = logFile.CreateNode("element", "test", "");
        ////node.InnerText = "test element";
        ////root.AppendChild(node);
        //print(root.ToString());
    }

    public void SaveHistoricalData()
    {
        fileBrowserOpen = true;
        logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
        savedLogFilePath = EditorUtility.SaveFilePanel("Save Data", "/Assets/SimulationLogs", logFileName, ".xml");
        logFile.Save(savedLogFilePath);
        fileBrowserOpen = false;
    }

    public void LoadHistoricalData()
    {
        fileBrowserOpen = true;
        loadedLogFilePath = EditorUtility.OpenFilePanel("Load Data", "/Assets/SimulationLogs", "xml");
        if (!loadedLogFilePath.Contains(".xml"))
        {
            logFile.Load(loadedLogFilePath);
            loadedLogfileContents = logFile.InnerXml;
        }
        fileBrowserOpen = false;
    }
}
