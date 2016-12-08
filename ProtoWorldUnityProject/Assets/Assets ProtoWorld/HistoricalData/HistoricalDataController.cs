using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

class DistinctLoggableComparer : IEqualityComparer<Loggable> {

	public bool Equals(Loggable x, Loggable y) {
		return ((MonoBehaviour)x).tag.Equals(((MonoBehaviour)y).tag);
	}

	public int GetHashCode(Loggable obj) {
		return ((MonoBehaviour)obj).tag.GetHashCode ();
	}
}

public class HistoricalDataController : MonoBehaviour
{
    public int logIntervalSeconds = 3;

    public DirectoryInfo logDirectory;
    private ScrollRect FileScrollView;
    private ScrollRect TimestampScrollView;

    public GameObject logFileButtonPrefab;
    public GameObject loadFileBrowser;

    private string logFileName;

    private XDocument logFile;

    private TimeController timeController;

	private FlashPedestriansGlobalParameters globalParameters;

    private CameraControl camera;

	//TODO: logfile reset after loading of snapshot

    // Use this for initialization
    void Start()
    {
        getLoggables();
        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
		GameObject flashPedestriansModule = GameObject.Find("FlashPedestriansModule");
        globalParameters = flashPedestriansModule.GetComponent<FlashPedestriansGlobalParameters>();

		initiateLogFile ();
		createLogDestination ();
		initiateLoggingInterface ();

		StartCoroutine(logToXML());
    }

    //TODO: Optimize! THIS IS A HORSETHING!!!//
    List<Loggable> getLoggables()
    {
        List<GameObject> loggableObjects = new List<GameObject>();
        loggableObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g => g.tag == "Loggable").ToList();
        List<Loggable> loggables = new List<Loggable>();
        foreach (GameObject loggable in  loggableObjects)
        {
            print(loggable.ToString());
            loggables.Add(loggable.GetComponent<Loggable>());
        }
        //print(loggables.Count);
        return loggables;
    }

    void initiateLogFile(){
		//Create XML document for logging data with root element "LogData" 
		logFile = new XDocument();
		logFile.Add(new XElement("LogData"));
	}

	void createLogDestination(){
		//Define path to the log folder and create it if it doesn't exist
		logDirectory = new DirectoryInfo(Application.dataPath + "/log/");
		if (!logDirectory.Exists) logDirectory.Create();
	}

	void initiateLoggingInterface(){
		FileScrollView = GameObject.Find("FileScrollView").GetComponent<ScrollRect>();
		loadFileBrowser = GameObject.Find("LoadFileBrowser");
		TimestampScrollView = GameObject.Find("TimestampScrollView").GetComponent<ScrollRect>();
		loadFileBrowser.SetActive(false);
		camera = GameObject.Find("Main Camera").GetComponent<CameraControl>();
	}

	public IEnumerator logToXML()
	{
		//As long as the simulation is not paused log the data of each subscribed loggable object
		while (!globalParameters.flashPedestriansPaused)
		{
			XElement timeStamp = new XElement("TimeStamp");
			timeStamp.Add(new XAttribute("timestamp", timeController.gameTime));
			logFile.Root.Add(timeStamp);
			foreach (Loggable loggable in LoggableManager.getCurrentSubscribedLoggables())
			{
				timeStamp.Add(convertDataToXML (loggable.getLogData()));
			}
			yield return new WaitForSeconds(logIntervalSeconds);
		}
	}

	//Recursive call for logging a tree to XML
	XElement convertDataToXML(LogDataTree logData){
		XElement element = new XElement (logData.Key, logData.Value);
		foreach (LogDataTree child in logData.getChildren()) {
			element.Add(convertDataToXML (child));
		}
		return element;
	}

	public void SaveHistoricalData()
	{
		timeController.PauseGame(true);
		logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
		logFile.Save(logDirectory + "/" + logFileName + ".xml");
		timeController.PauseGame(false);
	}

    public void loadLogDirectory()
    {
        FileBrowserController controller = loadFileBrowser.GetComponent<FileBrowserController>();
        controller.loadLogDirectory();
        loadFileBrowser.SetActive(true);
        timeController.PauseGame(true);
    }

	public void recreateLog(string file, string timestamp)
	{
        //print((from element in XDocument.Load(file).Root.Descendants("TimeStamp") where element.FirstAttribute.Value.Equals(timestamp) select element).FirstOrDefault());
        XElement timeStampElement =
            (from element in XDocument.Load(file).Root.Descendants("TimeStamp")
             where element.FirstAttribute.Value.Equals(timestamp)
             select element).FirstOrDefault();

		// TODO Use the interface to allow objects to destroy (or recycle, etc.) themselves.
        removeActiveData ();
        recreateObjects(timeStampElement);

        loadFileBrowser.SetActive(false);
		timeController.PauseGame(false);
	}

	//Recreates the logged loggables in order of priority
	private void recreateObjects(XElement timestampElement){
        //Get all objects which have the loggable interface with a unique tag
        List<Loggable> loggables = InterfaceHelper.FindObjectsInResources<Loggable>().Distinct(new DistinctLoggableComparer()).ToList();
        //List<Loggable> loggables = getLoggables();
        foreach (LogPriorities priority in Enum.GetValues(typeof(LogPriorities))) {
            foreach (Loggable loggable in loggables.FindAll(item => item.getPriorityLevel() == priority))
            {
                foreach (XElement loggedObject in timestampElement.Descendants(((MonoBehaviour)loggable).tag))
                {
					// TODO Instead of recreating spawners and destinations we can just update the properties that
					// change over time. This temporary if statement shows that it works without recreating them.
					if(!(loggable is FlashPedestriansSpawner) && !(loggable is FlashPedestriansDestination)) {
						loggable.rebuildFromLog(rebuildObjectLogData(loggedObject));
					}
                }
            }
        }
	}

	//Recursively transforms the XML data of a loggable to a LogDataTree
	private LogDataTree rebuildObjectLogData(XElement element){
		LogDataTree logData = new LogDataTree(element.Name.ToString (), element.Value);
		foreach (XElement child in element.Descendants()) {
			logData.AddChild(rebuildObjectLogData(child));
		}
		return logData;
	}
	
	private void removeActiveData(){
        FlashPedestriansSpawner.nextIdForPedestrian = 0;
        FlashPedestriansInformer flashInformer = FindObjectOfType<FlashPedestriansInformer>();
        flashInformer.accumPedestrians = 0;
        flashInformer.activePedestrians = new Dictionary<int, FlashPedestriansController>();
        foreach (Loggable loggable in LoggableManager.getCurrentSubscribedLoggables())
		{
			// TODO Instead of destroying spawners and destinations we can just update the properties that
			// change over time. This temporary if statement shows that it works without destroying them.
			if(!(loggable is FlashPedestriansSpawner) && !(loggable is FlashPedestriansDestination)) {
				LoggableManager.unsubscribe (loggable);
				Destroy (((MonoBehaviour)loggable).gameObject);
			}
		}
	}
}
