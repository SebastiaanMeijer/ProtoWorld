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

    private bool fileBrowserOpened;
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
    

    // Use this for initialization
    void Start()
    {

        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        pedestrianInformer = GameObject.Find("FlashInformer").GetComponent<FlashPedestriansInformer>();
        loadLogWindow = GameObject.Find("LoadLogCanvas");
        timestampDropdown = GameObject.Find("LoadFileTimestampDropdown").GetComponent<Dropdown>();
        loadLogWindow.SetActive(false);
        fileBrowserOpened = false;

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
				if (key.Equals ("id")) {
					pedestrianElement.Add (new XAttribute ("id", singleValueLogData [key]));
				} else {
					pedestrianElement.Add (new XElement (key, singleValueLogData [key]));
				}
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

    List<FlashPedestriansController> getPedestrianData()
    {
        List<FlashPedestriansController> logData = new List<FlashPedestriansController>();
        foreach (KeyValuePair<int, FlashPedestriansController> pair in pedestrianInformer.activePedestrians)
        {
            logData.Add(pair.Value);
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
            List<XElement> pedestrians =
                (from pedestrian in historicalTimeStamps[timeStamp].Descendants("Pedestrian")
                 select pedestrian).ToList();
            foreach (XElement pedestrian in pedestrians)
            {
                FlashPedestriansProfile profile = recreatePedestrianProfile(pedestrian.Descendants("PedestrianProfile").Single());
                //print(profile.speed);
                //XElement pedestrianPosition = pedestrian.Descendants("PedestrianPosition").Single();
                /*
				pedestrianSpawner.SpawnPedestrian (
					new Vector3 (pedestrianPosition.Descendants ("PositionX").Single (), pedestrianPosition.Descendants ("PositionY").Single (), pedestrianPosition.Descendants ("PositionZ").Single ()),
					pedestrian.Descendants ("Profile").Single (),
					pedestrian.Descendants ("Destination").Single (),
					pedestrian.Descendants ("Itinerary").Single ());
					*/
            }
        }
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

    public void openLoadLogFileWindow()
    {
        fileBrowserOpened = true;
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
        removeActiveData();
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
        fileBrowserOpened = false;
        loadLogWindow.SetActive(false);
    }

    public void removeActiveData(){
		GameObject[] currentPedestrians = GameObject.FindGameObjectsWithTag ("Pedestrian");
		foreach (GameObject pedestrian in currentPedestrians) {
			Destroy (pedestrian);
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
