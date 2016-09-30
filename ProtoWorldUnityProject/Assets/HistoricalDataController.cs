using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class HistoricalDataController : MonoBehaviour
{
	//public GameObject flashPedestrian;

    public int logInterval = 3;

    private bool canLog;
    private string savedLogFilePath;
    private string loadedLogFilePath;
    private string logFileName;

    private XDocument logFile;
    private XElement timeStamp;
    private XElement logFileRootElement;

	private Dictionary<string,XElement> historicalTimeStamps;

    private TimeController timeController;
    private FlashPedestriansInformer pedestrianInformer;

	private FlashPedestriansSpawner pedestrianSpawner;

    private GameObject loadLogWindow;
    private Dropdown timestampDropdown;

    GameObject flashPedestriansModule;
    GameObject pedestrianDestionationPoints;
    GameObject pedestrianSpawnerPoints;


    // Use this for initialization
    void Start()
    {

        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        pedestrianInformer = GameObject.Find("FlashInformer").GetComponent<FlashPedestriansInformer>();
        loadLogWindow = GameObject.Find("LoadLogCanvas");
        timestampDropdown = GameObject.Find("LoadFileTimestampDropdown").GetComponent<Dropdown>();
        loadLogWindow.SetActive(false);
        canLog = true;
        flashPedestriansModule = GameObject.Find("FlashPedestrianModule");
        pedestrianDestionationPoints = GameObject.Find("DestinationPoints");
        pedestrianSpawnerPoints = GameObject.Find("SpawnerPoints");

        logFile = new XDocument();
        logFileRootElement = new XElement("LogData");
        logFile.Add(logFileRootElement);
        //historicalTimeStamps = new Dictionary<string, XElement>();

        StartCoroutine(processLogData());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void processPedestrianData()
    {

        foreach (FlashPedestriansController data in getPedestrianData())
        {
			XElement pedestrianElement = new XElement("Pedestrian");
			Dictionary<string,string> singleValueLogData = data.getSingleValueLogData ();
			Dictionary<string,Dictionary<string,string>> multipleValueLogData = data.getMultipleValueLogData ();
			foreach (string key in singleValueLogData.Keys) {
					pedestrianElement.Add (new XElement (key, singleValueLogData [key]));
			}
			foreach (string key in multipleValueLogData.Keys) {
				Dictionary<string,string> catagoryLogData = multipleValueLogData [key];
				XElement catagoryElement = new XElement(key);
				pedestrianElement.Add (catagoryElement);
				foreach (string catagoryDataKey in catagoryLogData.Keys) {
					catagoryElement.Add (new XElement (catagoryDataKey, catagoryLogData[catagoryDataKey]));
				}
			}
			timeStamp.Add(pedestrianElement);
        }
    }

    void processPedestrianSpawnData()
    {

        foreach (FlashPedestriansSpawner data in getPedestrianSpawnerData())
        {
            XElement spawnerElement = new XElement("PedestrianSpawner");
            Dictionary<string, string> singleValueLogData = data.getSingleValueLogData();
            foreach (string key in singleValueLogData.Keys)
            {
                spawnerElement.Add(new XElement(key, singleValueLogData[key]));
            }
            timeStamp.Add(spawnerElement);
        }
    }

    void processPedestrianDestinationData()
    {

        foreach (FlashPedestriansDestination data in getPedestrianDestinationData())
        {
            XElement destinationElement = new XElement("PedestrianDestination");
            Dictionary<string, string> singleValueLogData = data.getSingleValueLogData();
            foreach (string key in singleValueLogData.Keys)
            {
                destinationElement.Add(new XElement(key, singleValueLogData[key]));
            }
            timeStamp.Add(destinationElement);
        }
    }

    List<FlashPedestriansController> getPedestrianData()
    {
        List<FlashPedestriansController> logData = new List<FlashPedestriansController>();
        foreach (KeyValuePair<int, FlashPedestriansController> pair in pedestrianInformer.activePedestrians)
        {
            logData.Add(pair.Value);
        }
        return logData;
    }

    List<FlashPedestriansDestination> getPedestrianDestinationData()
    {
        List<FlashPedestriansDestination> logData = new List<FlashPedestriansDestination>();
        foreach (GameObject destination in GameObject.FindGameObjectsWithTag("PedestrianDestination"))
        {
            logData.Add(destination.GetComponent<FlashPedestriansDestination>());
        }
        return logData;
    }

    List<FlashPedestriansSpawner> getPedestrianSpawnerData()
    {
        List<FlashPedestriansSpawner> logData = new List<FlashPedestriansSpawner>();
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("PedestrianSpawner"))
        {
            logData.Add(spawner.GetComponent<FlashPedestriansSpawner>());
        }
        return logData;
    }

    public void processLoadedLogData(string timeStamp)
    {
        if (loadedLogFilePath.Length <= 0)
        {
            throw new Exception("no loaded logFile!");
        }
        else
        {
            //TODO Initiate new pedestrians from a selected timestamp in stead of the first
            //TODO start timer at log-time
            //TODO break up this function into seperate readable ones
            removeActiveData();

            List<GameObject> destinationList = new List<GameObject>();
            List<XElement> destinations =
                (from destination in historicalTimeStamps[timeStamp].Descendants("Destination")
                 select destination).ToList();
            foreach (XElement destination in destinations)
            {
                destinationList.Add(recreatePedestrianDestination(destination.Descendants("DestinationData").SingleOrDefault()));
            }

            //List<XElement> pedestrians =
            //    (from pedestrian in historicalTimeStamps[timeStamp].Descendants("Pedestrian")
            //     select pedestrian).ToList();
            //foreach (XElement pedestrian in pedestrians)
            //{
            //    FlashPedestriansProfile profile = recreatePedestrianProfile(pedestrian.Descendants("PedestrianProfile").SingleOrDefault());
            //    Vector3 spawnPoint = recreatePedestrianSpawnPoint(pedestrian.Descendants("PedestrianPosition").SingleOrDefault());

            //    foreach(FlashPedestriansDestination destination in destinationList)
            //    {
            //        if (destination.name == pedestrian.Descendants("Destination").SingleOrDefault().Value.ToString());
            //        {
            //            pedestrianSpawner.SpawnPedestrianFromLog(spawnPoint, profile, destination);
            //        }
            //    }
            //    //pedestrianSpawner.SpawnPedestrianFromLog(spawnPoint, profile, destination);
            //}
            timeController.gameTime = 0f;
            timeController.PauseGame(false);
           
        }
    }

    public Vector3 recreatePedestrianSpawnPoint(XElement destinationData)
    {
        Vector3 spawnPoint = new Vector3();

        spawnPoint.x = float.Parse(destinationData.Descendants("PositionX").Single().Value.ToString());
        spawnPoint.y = float.Parse(destinationData.Descendants("PositionY").Single().Value.ToString());
        spawnPoint.z = float.Parse(destinationData.Descendants("PositionZ").Single().Value.ToString());

        return spawnPoint;
    }

    public GameObject recreatePedestrianDestination(XElement destinationData)
    {
        GameObject flashDestination = new GameObject();
        flashDestination.AddComponent<FlashPedestriansDestination>();
        FlashPedestriansDestination destination = flashDestination.GetComponent<FlashPedestriansDestination>();
        Vector3 position = new Vector3();

        position.x = float.Parse(destinationData.Descendants("PositionX").Single().Value.ToString());
        position.y = float.Parse(destinationData.Descendants("PositionY").Single().Value.ToString());
        position.z = float.Parse(destinationData.Descendants("PositionZ").Single().Value.ToString());
        destination.destinationName = destinationData.Descendants("Name").Single().Value.ToString();

        flashDestination.transform.parent = pedestrianDestionationPoints.transform;
        flashDestination.name = "FlashDestination";
        destination.destinationTransform.position = position;
        destination.transform.position = position;
        

        return flashDestination;
    }

    public FlashPedestriansProfile recreatePedestrianProfile(XElement profileData)
    {
        float speed = float.Parse(profileData.Descendants("speed").Single().Value.ToString());
        bool englishSpeaker = bool.Parse(profileData.Descendants("englishSpeaker").Single().Value.ToString());
        bool italianSpeaker = bool.Parse(profileData.Descendants("italianSpeaker").Single().Value.ToString());
        float chanceOfSubscription = float.Parse(profileData.Descendants("chanceOfSubscription").Single().Value.ToString());
        bool willingToChangeDestination = bool.Parse(profileData.Descendants("willingToChangeDestination").Single().Value.ToString());
        float chanceOfTakingABike = float.Parse(profileData.Descendants("chanceOfTakingABike").Single().Value.ToString());
        float weatherFactorOnTakingBikes = float.Parse(profileData.Descendants("weatherFactorOnTakingBikes").Single().Value.ToString());
        float chanceOfBelievingRumours = float.Parse(profileData.Descendants("chanceOfBelievingRumours").Single().Value.ToString());
        bool carAwareness = bool.Parse(profileData.Descendants("carAwareness").Single().Value.ToString());
        TravelPreference travelPreference = new TravelPreference();

        FlashPedestriansProfile profile = new FlashPedestriansProfile(speed, englishSpeaker, italianSpeaker, chanceOfSubscription, willingToChangeDestination, chanceOfTakingABike,
            chanceOfBelievingRumours, carAwareness, travelPreference);

        return profile;
    }

    public IEnumerator processLogData()
    {
        while (canLog)
        {
            timeStamp = new XElement("TimeStamp");
            timeStamp.Add(new XAttribute("time", timeController.timerText.text));
            processPedestrianData();
            processPedestrianDestinationData();
            processPedestrianSpawnData();
            logFileRootElement.Add(timeStamp);

            yield return new WaitForSeconds(logInterval);
        }
    }

    public void SaveHistoricalData()
    {
        canLog = false;
        logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
        savedLogFilePath = EditorUtility.SaveFilePanel("Save Data", "/Assets/SimulationLogs", logFileName, ".xml");
        logFile.Save(savedLogFilePath);
        canLog = true;
    }

    public void openLoadLogFileWindow()
    {
        //stop game time
        canLog = false;
        timeController.PauseGame(true);
        loadLogWindow.SetActive(true);
        timestampDropdown.options.Clear();
        timestampDropdown.captionText.text = "";
    }

    public void loadLogfile()
    {
        loadedLogFilePath = EditorUtility.OpenFilePanel("Load Data", "/Assets/SimulationLogs", "xml");
        if (loadedLogFilePath.Contains(".xml"))
        {
            logFile = XDocument.Load(loadedLogFilePath);
            logFileRootElement = logFile.Root;
            createTimeStampList();
            foreach (KeyValuePair<string, XElement> timeStamp in historicalTimeStamps)
            {
                timestampDropdown.options.Add(new Dropdown.OptionData() { text = timeStamp.Key });
            }
            timestampDropdown.value = 1;
            timestampDropdown.value = 0;
        }
    }

    public void cancelLoadLogFile()
    {
        loadedLogFilePath = "";
        timestampDropdown.options.Clear();
        timestampDropdown.captionText.text = "";
        loadLogWindow.SetActive(false);
    }

    public void LoadLogdata()
    {
        processLoadedLogData(timestampDropdown.options[timestampDropdown.value].text);
        canLog = true;
        loadLogWindow.SetActive(false);
    }

    public void removeActiveData(){
        GameObject[] currentPedestrianSpawnerPoints = GameObject.FindGameObjectsWithTag("PedestrianSpawner");
        GameObject[] currentPedestrianDestinationPoints = GameObject.FindGameObjectsWithTag("PedestrianDestination");
		GameObject[] currentPedestrians = GameObject.FindGameObjectsWithTag ("Pedestrian");
		foreach (GameObject pedestrian in currentPedestrians) {
			Destroy (pedestrian);
		}
        foreach (GameObject spawner in currentPedestrianSpawnerPoints)
        {
            Destroy(spawner);
        }
        foreach (GameObject destination in currentPedestrianDestinationPoints)
        {
            Destroy(destination);
        }
    }

	public void createTimeStampList(){
		List<XElement> timeStampElements =
			(from timeStampElement in logFileRootElement.Descendants("TimeStamp")
				select timeStampElement).ToList();
        historicalTimeStamps = new Dictionary<string, XElement>();
        foreach (XElement timeStampElement in timeStampElements) {
			historicalTimeStamps.Add (timeStampElement.Attribute ("time").Value.ToString(), timeStampElement);
		}
        print(historicalTimeStamps.Count);
	}
}
