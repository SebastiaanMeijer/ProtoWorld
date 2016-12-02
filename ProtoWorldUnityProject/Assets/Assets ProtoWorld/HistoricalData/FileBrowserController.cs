using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Linq;
using System.IO;

public class FileBrowserController : MonoBehaviour {

    public HistoricalDataController controller;
    private TimeController timecontroller;
    public GameObject logFileButtonPrefab;

    public ScrollRect FileScrollView;
    public ScrollRect TimestampScrollView;

    public GameObject timestampButtonPrefab;

    public Button loadButton;

    private string selected_file = "";
    private string selected_timestamp;

    // Use this for initialization
    void Start () {
		controller = GameObject.Find("HistoricalDataModule").GetComponent<HistoricalDataController>();
		timecontroller = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
    }
	
	// Update is called once per frame
	void Update () {
		if (selected_timestamp == null) loadButton.interactable = false;
		else loadButton.interactable = true;


		if (controller.loadFileBrowser.activeSelf)
		{
			//Highlight the selected file
			foreach (Transform button in FileScrollView.content.transform)
			{
				Image btn_img = button.GetComponent<Image>();
				Button btn = button.GetComponent<Button>();
				fileButtonController btn_ctl = button.GetComponent<fileButtonController>();

				btn_img.color = btn.colors.normalColor;
				if (selected_file.Equals(btn_ctl.path))
				{
					btn_img.color = btn.colors.highlightedColor;
				}
			}


			//Highlight the selected timestamp
			if(selected_timestamp != null)
			{
				foreach (Transform button in TimestampScrollView.content.transform)
				{
					Image btn_img = button.GetComponent<Image>();
					Button btn = button.GetComponent<Button>();
					TimestampButtonController ctlr = button.GetComponent<TimestampButtonController>();

					btn_img.color = btn.colors.normalColor;
					if (selected_timestamp.Equals(ctlr.timestamp))
					{
						btn_img.color = btn.colors.highlightedColor;
					}
				}
			}

		}

	}

	public void loadLogDirectory()
	{
		List<GameObject> gameObjects = new List<GameObject>();

		foreach (Transform transform in FileScrollView.content)
		{
			gameObjects.Add(transform.gameObject);
		}

		while(gameObjects.Count > 0) {
			GameObject gameObject = gameObjects[0];
			gameObjects.RemoveAt(0);
			Destroy(gameObject);
			gameObject = null;
		}

		FileInfo[] filesInfo = controller.logDirectory.GetFiles();
		foreach (FileInfo fileInfo in filesInfo)
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
			}
		}
	}

	internal void showFileInBrowser(string path)
	{

		selected_file = path;
		selected_timestamp = null;

		//Fetch all timestamps
		XDocument logFile = XDocument.Load(path);
		XElement logFileRootElement = logFile.Root;

		Dictionary<string, XElement> historicalTimeStamps = new Dictionary<string, XElement>();

		List<System.Xml.Linq.XElement> timeStampElements =
			(from timeStampElement in logFileRootElement.Descendants("TimeStamp")
				select timeStampElement).ToList();


		foreach (XElement timeStampElement in timeStampElements)
		{
			historicalTimeStamps.Add(timeStampElement.Attribute("timestamp").Value.ToString(), timeStampElement);
		}

		foreach (Transform item in TimestampScrollView.content)
		{
			GameObject.Destroy(item.gameObject);
		}

		foreach (KeyValuePair<string, System.Xml.Linq.XElement> timeStamp in historicalTimeStamps)
			//Create timestamp button prefab
		{
			//Add file to the fileScrollView
			GameObject fileButton = Instantiate(timestampButtonPrefab) as GameObject;
			fileButton.transform.SetParent(TimestampScrollView.content.transform);

			Text btn = fileButton.GetComponentInChildren<Text>();
			btn.text = timeStamp.Key.ToString();
			TimestampButtonController ctl = fileButton.GetComponent<TimestampButtonController>();
			ctl.timestamp = timeStamp.Key;
		}
	}

    public void TogglePanel()
    {
        gameObject.SetActive(false);
        timecontroller.PauseGame(false);
    }


    public void SelectTimestamp(string timestamp)
    {
        selected_timestamp = timestamp;
    }

    public void LoadLog()
    {
        Debug.Log("Loading timestamp" + selected_timestamp + " from " + selected_file);
        controller.recreateLog(selected_file, selected_timestamp);
    }

    public void SaveLog()
    {
        controller.SaveHistoricalData();
    }
}
