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
	//public GameObject flashPedestrian;

    public int logInterval = 3;

    private bool fileBrowserOpened;
    private string savedLogFilePath;
    private string loadedLogFilePath;
    private string logFileName;

    private XDocument logFile;
    private XElement timeStamp;
    private XElement logFileRootElement;

	Dictionary<string,XElement> historicalTimeStamps;

    private TimeController timeController;
    private FlashPedestriansInformer pedestrianInformer;

	private FlashPedestriansSpawner pedestrianSpawner;

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
		if (loadedLogFilePath.Length <= 0) {
			throw new Exception ("no loaded logFile!");
		} else {
			//TODO Initiate new pedestrians from a selected timestamp in stead of the first
			//TODO start timer at log-time
			//TODO break up this function into seperate readable ones
			List<XElement> pedestrians = 
				from pedestrian in historicalTimeStamps [timeStamp].Descendants ("Pedestrian")
				select pedestrian;
			foreach (XElement pedestrian in pedestrians) {
				XElement pedestrianPosition = pedestrian.Descendants ("PedestrianPosition").Single ();
				/*
				pedestrianSpawner.SpawnPedestrian (
					new Vector3 (pedestrianPosition.Descendants ("PositionX").Single (), pedestrianPosition.Descendants ("PositionY").Single (), pedestrianPosition.Descendants ("PositionZ").Single ()),
					pedestrian.Descendants ("Profile").Single (),
					pedestrian.Descendants ("Destination").Single (),
					pedestrian.Descendants ("Itinerary").Single ());
					*/
				pedestrianSpawner.SpawnPedestrian (
					new Vector3 (pedestrianPosition.Descendants ("PositionX").Single (), pedestrianPosition.Descendants ("PositionY").Single (), pedestrianPosition.Descendants ("PositionZ").Single ()),
					new FlashPedestriansProfile(),
					new FlashPedestriansDestination(),
					new Itinerary());
			}
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
        //open dialog
        loadedLogFilePath = EditorUtility.OpenFilePanel("Load Data", "/Assets/SimulationLogs", "xml");
        //if xml
        if (loadedLogFilePath.Contains(".xml"))
        {
            //load data
            logFile = XDocument.Load(loadedLogFilePath);
			logFileRootElement = logFile.Root;
			createTimeStampList();
        }
		removeActiveData();
		processLoadedLogData(historicalTimeStamps.Keys.First); //TODO Make people select which timestamp to load
        fileBrowserOpened = false;
    }

	public void removeActiveData(){
		GameObject[] currentPedestrians = GameObject.FindGameObjectsWithTag ("Pedestrian");
		foreach (GameObject pedestrian in currentPedestrians) {
			Destroy (pedestrian);
		}
	}

	public void createTimeStampList(){
		List<XElement> timeStampElements =
			from timeStampElement in logFileRootElement.Descendants("TimeStamp")
			select timeStampElement;

		foreach (XElement timeStampElement in timeStampElements) {
			historicalTimeStamps.Add ((string)timeStampElement.Attribute ("time"), timeStampElement);
		}
	}
}
