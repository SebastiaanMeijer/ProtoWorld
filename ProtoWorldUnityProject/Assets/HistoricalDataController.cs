using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

public class HistoricalDataController : MonoBehaviour
{

    public int logInterval = 3;

    private bool fileBrowserOpened;
    private string savedLogFilePath;
    private string loadedLogFilePath;
    private string logFileName;

    private XDocument logFile;
    private XElement timeStamp;
    private XElement logFileRootElement;

    private TimeController timeController;
    private FlashPedestriansInformer pedestrianInformer;

    // Use this for initialization
    void Start()
    {

        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        pedestrianInformer = GameObject.Find("FlashInformer").GetComponent<FlashPedestriansInformer>();
        fileBrowserOpened = false;

        logFile = new XDocument();
        logFileRootElement = new XElement("LogData");
        logFile.Add(logFileRootElement);

        StartCoroutine(processLogData());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void processPedestrianData()
    {
        foreach (FlashPedestriansController.LogData data in getPedestrianData())
        {
            XElement pedestrianElement = new XElement("Pedestrian");
            pedestrianElement.Add(new XAttribute("id", data.id));
            pedestrianElement.Add(new XElement("PedestrianPosition",
                new XElement("PositionX", data.posX),
                new XElement("PositionY", data.posY),
                new XElement("PositionZ", data.posZ)),
                new XElement("Destination", data.destination));
            timeStamp.Add(pedestrianElement);
        }
    }

    List<FlashPedestriansController.LogData> getPedestrianData()
    {
        List<FlashPedestriansController.LogData> logData = new List<FlashPedestriansController.LogData>();
        foreach (KeyValuePair<int, FlashPedestriansController> pair in pedestrianInformer.activePedestrians)
        {
            logData.Add(pair.Value.getLogData());
        }
        return logData;
    }

    public void processLoadedLogData(string timeStamp)
    {
        if (loadedLogFilePath.Length <= 0)
        {
            throw new Exception("no loaded logFile!");
        }

    }

    public IEnumerator processLogData()
    {
        while (!fileBrowserOpened)
        {
            timeStamp = new XElement("TimeStamp");
            timeStamp.Add(new XAttribute("time", timeController.timerText.text));
            processPedestrianData();
            logFileRootElement.Add(timeStamp);

            yield return new WaitForSeconds(logInterval);
        }
    }

    public void SaveHistoricalData()
    {
        fileBrowserOpened = true;
        logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
        savedLogFilePath = EditorUtility.SaveFilePanel("Save Data", "/Assets/SimulationLogs", logFileName, ".xml");
        logFile.Save(savedLogFilePath);
        fileBrowserOpened = false;
    }

    public void LoadHistoricalData()
    {
        fileBrowserOpened = true;
        loadedLogFilePath = EditorUtility.OpenFilePanel("Load Data", "/Assets/SimulationLogs", "xml");
        if (loadedLogFilePath.Contains(".xml"))
        {
            logFile = XDocument.Load(loadedLogFilePath);
        }
        processLoadedLogData(null);
        fileBrowserOpened = false;
    }
}
