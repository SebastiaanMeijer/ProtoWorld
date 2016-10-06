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

    private string savedLogFilePath;
    private string loadedLogFilePath;
    private string logFileName;

    private XDocument logFile;
    private XElement timeStamp;
    private XElement logFileRootElement;

	private Dictionary<float,XElement> historicalTimeStamps;

    private TimeController timeController;
    private FlashPedestriansInformer pedestrianInformer;
    private FlashPedestriansGlobalParameters globalParameters;
	private FlashPedestriansSpawner pedestrianSpawner;

    private GameObject loadLogWindow;
    private Dropdown timestampDropdown;

    GameObject flashPedestriansModule;
    GameObject pedestrianDestionationPoints;
    GameObject pedestrianSpawnerPoints;

    List<GameObject> flashPedestrianSpawns;

    public GameObject pedestrianSpawnerPrefab;
    public GameObject pedestrianDestinationPrefab;
    public GameObject pedestrianPrefab;

    private List<float> gametimeEntries;


    // Use this for initialization
    void Start()
    {
        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        pedestrianInformer = GameObject.Find("FlashInformer").GetComponent<FlashPedestriansInformer>();
        loadLogWindow = GameObject.Find("LoadLogCanvas");
        //globalParameters = GameObject.Find("FlashPedestrianModule").GetComponent<FlashPedestriansGlobalParameters>();
        timestampDropdown = GameObject.Find("LoadFileTimestampDropdown").GetComponent<Dropdown>();
        loadLogWindow.SetActive(false);
        flashPedestriansModule = GameObject.Find("FlashPedestriansModule");
        globalParameters = flashPedestriansModule.GetComponent<FlashPedestriansGlobalParameters>();
        pedestrianDestionationPoints = GameObject.Find("DestinationPoints");
        pedestrianSpawnerPoints = GameObject.Find("SpawnerPoints");
        gametimeEntries = new List<float>();
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

        foreach (FlashPedestriansController data in getPedestrianData())
        {
                XElement pedestrianElement = new XElement("Pedestrian");
                Dictionary<string, string> singleValueLogData = data.getSingleValueLogData();
                Dictionary<string, Dictionary<string, string>> multipleValueLogData = data.getMultipleValueLogData();
                foreach (string key in singleValueLogData.Keys)
                {
                    pedestrianElement.Add(new XElement(key, singleValueLogData[key]));
                }
                foreach (string key in multipleValueLogData.Keys)
                {
                    Dictionary<string, string> catagoryLogData = multipleValueLogData[key];
                    XElement catagoryElement = new XElement(key);
                    pedestrianElement.Add(catagoryElement);
                    foreach (string catagoryDataKey in catagoryLogData.Keys)
                    {
                        catagoryElement.Add(new XElement(catagoryDataKey, catagoryLogData[catagoryDataKey]));
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

    public void processLoadedPedestrianLogData(float timeStamp)
    {
        if (loadedLogFilePath.Length <= 0)
        {
            throw new Exception("no loaded logFile!");
        }
        else
        {
            removeActiveData();
            List<XElement> destinations =
                (from destination in historicalTimeStamps[timeStamp].Descendants("PedestrianDestination")
                 select destination).ToList();
            flashPedestrianSpawns = new List<GameObject>();
            List<XElement> pedestrianSpawners =
                (from spawner in historicalTimeStamps[timeStamp].Descendants("PedestrianSpawner")
                 select spawner).ToList();
            List<XElement> pedestrians =
                (from spawner in historicalTimeStamps[timeStamp].Descendants("PedestrianSpawner")
                 select spawner).ToList();


            foreach (XElement destination in destinations)
            {
                recreatePedestrianDestination(destination);
            }
            foreach (XElement spawner in pedestrianSpawners)
            {
                flashPedestrianSpawns.Add(recreatePedestrianSpawner(spawner));
            }
            foreach (XElement pedestrian in pedestrians)
            {
                recreatePedestrian(pedestrian);
            }

            timeController.gameTime = timeStamp;
            timeController.PauseGame(false);
        }
    }

    public void activateAllObjects()
    {

    }

    public GameObject recreatePedestrian(XElement pedestrianData)
    {
        GameObject flashPedestrianObject = GameObject.Instantiate(pedestrianPrefab) as GameObject;
        FlashPedestriansController flashPedestrianScript = flashPedestrianObject.GetComponent<FlashPedestriansController>();
        Vector3 position = new Vector3();

        position.x = float.Parse(pedestrianData.Descendants("PositionX").Single().Value);
        position.y = float.Parse(pedestrianData.Descendants("PositionY").Single().Value);
        position.z = float.Parse(pedestrianData.Descendants("PositionZ").Single().Value);
        flashPedestrianObject.transform.position = position;
        flashPedestrianScript.transform.position = position;

        flashPedestrianScript.profile = recreatePedestrianProfile(pedestrianData.Descendants("Profile").Single());

        flashPedestrianObject.name = "FlashPedestrian";
        flashPedestrianScript.initializePedestrian();
        flashPedestrianScript.enabled = true;
        return flashPedestrianObject;
    }

    public GameObject recreatePedestrianSpawner(XElement spawnerdata)
    {
        GameObject flashSpawnerObject = GameObject.Instantiate(pedestrianSpawnerPrefab) as GameObject;
        FlashPedestriansSpawner flashSpawnerScript = flashSpawnerObject.GetComponent<FlashPedestriansSpawner>();
        Vector3 position = new Vector3();

        position.x = float.Parse(spawnerdata.Descendants("PositionX").Single().Value);
        position.y = float.Parse(spawnerdata.Descendants("PositionY").Single().Value);
        position.z = float.Parse(spawnerdata.Descendants("PositionZ").Single().Value);
        flashSpawnerObject.transform.position = position;
        flashSpawnerScript.transform.position = position;
        flashSpawnerScript.maxNumberOfPedestriansToSpawn = int.Parse(spawnerdata.Descendants("MaxNumberOfPedestriansToSpawn").SingleOrDefault().Value);
        flashSpawnerScript.spawnPedestriansInInfiniteLoop = bool.Parse(spawnerdata.Descendants("SpawnPedestriansInInfiteLoop").SingleOrDefault().Value);
        flashSpawnerScript.minPedestriansPerSpawningIteration = int.Parse(spawnerdata.Descendants("MinPedestriansPerSpawningIteration").SingleOrDefault().Value);
        flashSpawnerScript.maxPedestriansPerSpawningIteration = int.Parse(spawnerdata.Descendants("MaxPedestriansPerSpawningIteration").SingleOrDefault().Value);
        flashSpawnerScript.spawningFrequencyInSeconds = float.Parse(spawnerdata.Descendants("PedestrianSpawnFrequencyInSeconds").SingleOrDefault().Value);
        flashSpawnerScript.spawningArea = float.Parse(spawnerdata.Descendants("SpawningArea").SingleOrDefault().Value);
        flashSpawnerScript.radiousToCheckStations = float.Parse(spawnerdata.Descendants("RadiousToCheckStations").SingleOrDefault().Value);
        flashSpawnerScript.spawningDelayAtStart = float.Parse(spawnerdata.Descendants("SpawningDelayAtStart").SingleOrDefault().Value);
        flashSpawnerScript.initialNumberOfPedestriansInCache = int.Parse(spawnerdata.Descendants("InitialNumberOfPedestriansInCache").SingleOrDefault().Value);
        flashSpawnerScript.numberOfPedestriansGenerated = int.Parse(spawnerdata.Descendants("NumberOfPedestriansGenerated").SingleOrDefault().Value);
        flashSpawnerScript.numberOfPedestriansOnDestination = int.Parse(spawnerdata.Descendants("NumberOfPedestriansOnDestination").SingleOrDefault().Value);
        flashSpawnerObject.name = "FlashSpawner";
        flashSpawnerObject.transform.parent = pedestrianSpawnerPoints.transform;

        flashSpawnerScript.initializeSpawner();
        flashSpawnerScript.enabled = true;

        return flashSpawnerObject;
    }

    public GameObject recreatePedestrianDestination(XElement destinationData)
    {
        GameObject flashDestinationObject = GameObject.Instantiate(pedestrianDestinationPrefab) as GameObject;
        FlashPedestriansDestination flashDestinationScript = flashDestinationObject.GetComponent<FlashPedestriansDestination>();
        Vector3 position = new Vector3();

        position.x = float.Parse(destinationData.Descendants("PositionX").Single().Value);
        position.y = float.Parse(destinationData.Descendants("PositionY").Single().Value);
        position.z = float.Parse(destinationData.Descendants("PositionZ").Single().Value);
        flashDestinationScript.destinationName = destinationData.Descendants("Name").Single().Value;
        flashDestinationScript.radiousToCheckStations = float.Parse(destinationData.Descendants("CheckRadius").Single().Value);
        flashDestinationScript.destinationPriority = float.Parse(destinationData.Descendants("Priority").Single().Value);
        flashDestinationObject.transform.parent = pedestrianDestionationPoints.transform;
        flashDestinationObject.name = "FlashDestination";
        flashDestinationScript.destinationTransform.position = position;

        flashDestinationScript.initializeDestination();
        flashDestinationScript.enabled = true;

        return flashDestinationObject;
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
        while (!globalParameters.flashPedestriansPaused)
        {
            timeStamp = new XElement("TimeStamp");
            timeStamp.Add(new XAttribute("timestamp", timeController.gameTime));
            processPedestrianData();
            processPedestrianDestinationData();
            processPedestrianSpawnData();
            logFileRootElement.Add(timeStamp);

            yield return new WaitForSeconds(logInterval);
        }
    }

    public void SaveHistoricalData()
    {
        timeController.PauseGame(true);
        logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
        savedLogFilePath = EditorUtility.SaveFilePanel("Save Data", "/Assets/SimulationLogs", logFileName, ".xml");
        logFile.Save(savedLogFilePath);
        timeController.PauseGame(false);
    }

    public void openLoadLogFileWindow()
    {
        timeController.PauseGame(true);
        loadLogWindow.SetActive(true);
        timestampDropdown.options.Clear();
        timestampDropdown.captionText.text = "Please choose File";
    }

    public void loadLogfile()
    {
        loadedLogFilePath = EditorUtility.OpenFilePanel("Load Data", "/Assets/SimulationLogs", "xml");
        if (loadedLogFilePath.Contains(".xml"))
        {
            logFile = XDocument.Load(loadedLogFilePath);
            logFileRootElement = logFile.Root;
            createTimeStampList();
            timestampDropdown.options.Clear();
            Dropdown.OptionData option = new Dropdown.OptionData();
            foreach (KeyValuePair<float, XElement> timeStamp in historicalTimeStamps)
            {
                timestampDropdown.options.Add(new Dropdown.OptionData() { text = timeController.gameTimeToTimeStamp(timeStamp.Key)});
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
        timeController.PauseGame(false);
    }

    public void LoadLogdata()
    {
        processLoadedPedestrianLogData(gametimeEntries[timestampDropdown.value]);
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
        historicalTimeStamps = new Dictionary<float, XElement>();
        foreach (XElement timeStampElement in timeStampElements) {
            gametimeEntries.Add(float.Parse(timeStampElement.Attribute("timestamp").Value.ToString()));
			historicalTimeStamps.Add (float.Parse(timeStampElement.Attribute ("timestamp").Value.ToString()), timeStampElement);
		}
        print(historicalTimeStamps.Count);
	}
}
