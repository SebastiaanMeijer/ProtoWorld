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

    private GameObject loadLogWindow;
    private Dropdown timestampDropdown;

    // Use this for initialization
    void Start()
    {
        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        loadLogWindow = GameObject.Find("LoadLogCanvas");
        timestampDropdown = GameObject.Find("LoadFileTimestampDropdown").GetComponent<Dropdown>();
        loadLogWindow.SetActive(false);
        canLog = true;

        logFile = new XDocument();
        logFileRootElement = new XElement("LogData");
        logFile.Add(logFileRootElement);

        StartCoroutine(processLogData());
    }

    // Update is called once per frame
    void Update()
    {
    }

	public IEnumerator processLogData()
	{
		while (canLog)
		{
			timeStamp = new XElement("TimeStamp");
			timeStamp.Add(new XAttribute("time", timeController.timerText.text));
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
		canLog = false;
		logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
		savedLogFilePath = EditorUtility.SaveFilePanel("Save Data", "/Assets/SimulationLogs", logFileName, "xml");
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

	public void cancelLoadLogFile()
	{
		loadedLogFilePath = "";
		timestampDropdown.options.Clear();
		timestampDropdown.captionText.text = "";
		loadLogWindow.SetActive(false);
	}

	public void recreateLogDataFromTimeStamp(string timeStamp){
		foreach (LogObject logObject in InterfaceHelper.FindObjects<LogObject>()){
			foreach (XElement loggedObject in historicalTimeStamps[timeStamp].Descendants(((MonoBehaviour)logObject).gameObject.tag)) {
				Dictionary<string, Dictionary<string, string>> logData = new Dictionary<string,Dictionary<string,string>> ();
				logData.Add (((MonoBehaviour)logObject).gameObject.tag, new Dictionary<string,string> ());
				foreach (XElement dataItem in loggedObject.Descendants()) {
					if (dataItem.HasElements) {
						logData [((MonoBehaviour)logObject).gameObject.tag].Add(dataItem.Name.ToString(), dataItem.Value);
					} else {
						logData.Add (dataItem.Name.ToString(), new Dictionary<string,string> ());
						foreach (XElement dataItemChild in dataItem.Descendants()) {
							logData [dataItem.Name.ToString()].Add(dataItemChild.Name.ToString(), dataItemChild.Value);
						}
					}
				}
				logObject.rebuildFromLog (logData);
			}
		}
	}

	public void removeActiveData(){
		foreach (LogObject logObject in InterfaceHelper.FindObjects<LogObject>())
		{
			Destroy (((MonoBehaviour)logObject).gameObject);
		}
	}

	public void LoadLogdata()
	{
		if (loadedLogFilePath.Length <= 0) {
			throw new Exception ("no loaded logFile!");
		} else {
			removeActiveData ();
			recreateLogDataFromTimeStamp (timestampDropdown.options[timestampDropdown.value].text);
			timeController.gameTime = 0f;
			timeController.PauseGame(false);
		}
		canLog = true;
		loadLogWindow.SetActive(false);
	}
}
