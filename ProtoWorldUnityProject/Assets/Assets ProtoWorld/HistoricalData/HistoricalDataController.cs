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

public class HistoricalDataController : MonoBehaviour
{
    public int logInterval = 3;

    private DirectoryInfo logDirectory;
    private ScrollRect FileScrollView;
    private ScrollRect TimestampScrollView;

    public GameObject logFileButtonPrefab;
    public GameObject loadFileBrowser;

    private string savedLogFilePath;
    private string loadedLogFilePath;
    private string logFileName;

    private int priorityLevels = 3;

    private XDocument logFile;
    private XElement timeStamp;
    private XElement logFileRootElement;

	private Dictionary<string,XElement> historicalTimeStamps;

    private TimeController timeController;

	GameObject flashPedestriansModule;
	private FlashPedestriansGlobalParameters globalParameters;

    private List<float> gametimeEntries;

    private CameraControl camera;

    // Use this for initialization
    void Start()
    {
        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        flashPedestriansModule = GameObject.Find("FlashPedestriansModule");
        globalParameters = flashPedestriansModule.GetComponent<FlashPedestriansGlobalParameters>();
		gametimeEntries = new List<float>();
        logFile = new XDocument();
        logFileRootElement = new XElement("LogData");
        logFile.Add(logFileRootElement);

        StartCoroutine(processLogData());

        logDirectory = new DirectoryInfo(Application.dataPath + "/log/");
        if (!logDirectory.Exists) logDirectory.Create();

        FileScrollView = GameObject.Find("FileScrollView").GetComponent<ScrollRect>();
        loadFileBrowser = GameObject.Find("LoadFileBrowser");
        TimestampScrollView = GameObject.Find("TimestampScrollView").GetComponent<ScrollRect>();

        loadFileBrowser.SetActive(false);

        camera = GameObject.Find("Main Camera").GetComponent<CameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        //Lock the camera on mouseover!
        if (!EventSystem.current.IsPointerOverGameObject()) camera.enabled = true;
        else camera.enabled = false;

    }
		
	public void activateAllObjects()
	{

	}

	public IEnumerator processLogData()
	{
		while (!globalParameters.flashPedestriansPaused)
		{
			timeStamp = new XElement("TimeStamp");
			timeStamp.Add(new XAttribute("timestamp", timeController.gameTime));
			foreach (Loggable loggable in LoggableManager.getCurrentSubscribedLoggables())
			{
				timeStamp.Add(processObjectLogData (loggable.getLogData()));
			}
			logFileRootElement.Add(timeStamp);
			yield return new WaitForSeconds(logInterval);
		}
	}

	XElement processObjectLogData(NTree<KeyValuePair<string,string>> logData){
		XElement element = new XElement (logData.data.Key, logData.data.Value);
		foreach (NTree<KeyValuePair<string,string>> child in logData.getChildren()) {
			element.Add(processObjectLogData (child));
		}
		return element;
	}

	public void SaveHistoricalData()
	{
		timeController.PauseGame(true);
		logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
        savedLogFilePath = logDirectory + "/" + logFileName + ".xml";
		logFile.Save(savedLogFilePath);
		timeController.PauseGame(false);
	}

    public void loadLogDirectory()
    {

        loadFileBrowser.SetActive(true);
        timeController.PauseGame(true);

        foreach (GameObject item in FileScrollView.content)
        {
            GameObject.Destroy(item);
        }

        FileInfo[] filesInfo = logDirectory.GetFiles();
        foreach(FileInfo fileInfo in filesInfo)
        {
            if (fileInfo.Name.EndsWith(".xml"))
            {
                //Add file to the fileScrollView
                GameObject fileButton = Instantiate(logFileButtonPrefab) as GameObject;
                fileButtonController btn = fileButton.GetComponent<fileButtonController>();
                btn.path = fileInfo.FullName;
                fileButton.transform.SetParent(FileScrollView.content.transform);

                Text text = fileButton.GetComponentInChildren<Text>();
                text.text = fileInfo.Name;
                //Debug.Log("File found: " + fileInfo);
            }
        }

    }

	public void createTimeStampList(){
		List<XElement> timeStampElements =
			(from timeStampElement in logFileRootElement.Descendants("TimeStamp")
				select timeStampElement).ToList();
        historicalTimeStamps = new Dictionary<string, XElement>();
        foreach (XElement timeStampElement in timeStampElements) {
            gametimeEntries.Add(float.Parse(timeStampElement.Attribute("timestamp").Value.ToString()));
			historicalTimeStamps.Add (timeStampElement.Attribute ("timestamp").Value.ToString(), timeStampElement);
		}
		print(historicalTimeStamps.Count);
	}

    public void recreateLog(string file, string timestamp)
    {
        logFile = XDocument.Load(file);
        logFileRootElement = logFile.Root;
        createTimeStampList();

        recreateLogDataFromTimeStamp(timestamp);

        loadFileBrowser.SetActive(false);
        timeController.PauseGame(false);
    }

	public void recreateLogDataFromTimeStamp(string timeStamp){
		removeActiveData ();

		IList<Loggable> loggables = InterfaceHelper.FindObjects<Loggable> ();
		List<string> recreatedTags = new List<string> ();

		for (int priority = 1; priority <= priorityLevels; priority++) {
			foreach (Loggable loggable in loggables) {
				string currentObjectName = ((MonoBehaviour)loggable).gameObject.tag;
				if (!recreatedTags.Contains (currentObjectName) && loggable.getPriorityLevel() == priority) {
					recreatedTags.Add(currentObjectName);
					foreach (XElement loggedObject in historicalTimeStamps[timeStamp].Descendants(currentObjectName)) {
						loggable.rebuildFromLog(rebuildObjectLogData(loggedObject));
					}
				}
			}
		}
	}

	NTree<KeyValuePair<string,string>> rebuildObjectLogData(XElement element){
		KeyValuePair<string,string> data = new KeyValuePair<string, string> (element.Name.ToString (), element.Value);
		NTree< KeyValuePair<string,string> > logData = new NTree< KeyValuePair<string,string> > (data);
		foreach (XElement child in element.Descendants()) {
			logData.AddChild(rebuildObjectLogData(child));
		}
		return logData;
	}

	public void removeActiveData(){
		foreach (Loggable loggable in LoggableManager.getCurrentSubscribedLoggables())
		{
			LoggableManager.unsubscribe (loggable);
			Destroy (((MonoBehaviour)loggable).gameObject);
		}
	}
}
