using UnityEngine;
using System.Collections;
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


    public GameObject loadFileBrowser;
    public DirectoryInfo logDirectory;

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
			foreach (LogObject logObject in InterfaceHelper.FindObjects<LogObject>())
			{
				processObjectLogData (new XElement (logObject.getLogData().Keys.First()), logObject.getLogData());
			}
			logFileRootElement.Add(timeStamp);
			yield return new WaitForSeconds(logInterval);
		}
	}

	void processObjectLogData(XElement parent, Dictionary<string, Dictionary<string, string>> logData)
	{
		foreach (string key in logData.Keys)
		{
			if (key.Equals(parent.Name.ToString())) {
				foreach (string dataKey in logData[key].Keys) {
					parent.Add (new XElement (dataKey, logData [key] [dataKey]));
				}
			} else {
				XElement categoryElement = new XElement (key);
				parent.Add (categoryElement);
				foreach (string catagoryDataKey in logData[key].Keys) {
					categoryElement.Add (new XElement (catagoryDataKey, logData[key][catagoryDataKey]));
				}
			}
		}
		timeStamp.Add(parent);
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
        FileBrowserController controller = loadFileBrowser.GetComponent<FileBrowserController>();
        controller.loadLogDirectory();
        loadFileBrowser.SetActive(true);
        timeController.PauseGame(true);
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
		List<string> recreatedTags = new List<string> ();
        for (int i = 1; i <= priorityLevels; i++) {
            foreach (LogObject logObject in InterfaceHelper.FindObjects<LogObject>()) {
                if (logObject.getPriorityLevel() == i) {
                    if (!recreatedTags.Contains(((MonoBehaviour)logObject).gameObject.tag)) {
                        recreatedTags.Add(((MonoBehaviour)logObject).gameObject.tag);
                        foreach (XElement loggedObject in historicalTimeStamps[timeStamp].Descendants(((MonoBehaviour)logObject).gameObject.tag)) {
                            Dictionary<string, Dictionary<string, string>> logData = new Dictionary<string, Dictionary<string, string>>();
                            logData.Add(((MonoBehaviour)logObject).gameObject.tag, new Dictionary<string, string>());
                            print(((MonoBehaviour)logObject).gameObject.tag);
                            foreach (XElement dataItem in loggedObject.Descendants()) {
                                if (!dataItem.HasElements) {
                                    logData[((MonoBehaviour)logObject).gameObject.tag].Add(dataItem.Name.ToString(), dataItem.Value);
                                } else {
                                    logData.Add(dataItem.Name.ToString(), new Dictionary<string, string>());
                                    foreach (XElement dataItemChild in dataItem.Descendants()) {
                                        logData[dataItem.Name.ToString()].Add(dataItemChild.Name.ToString(), dataItemChild.Value);
                                    }
                                }
                            }
                            logObject.rebuildFromLog(logData);
                        }
                    }
                }
            }
        }
	}

	public void removeActiveData(){
		foreach (LogObject logObject in InterfaceHelper.FindObjects<LogObject>())
		{
			Destroy (((MonoBehaviour)logObject).gameObject);
		}
	}
}
