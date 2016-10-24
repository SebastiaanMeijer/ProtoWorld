using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Linq;

public class FileBrowserController : MonoBehaviour {

    private HistoricalDataController controller;
    private TimeController timecontroller;

    private ScrollRect FileScrollView;
    private ScrollRect TimestampScrollView;

    public GameObject timestampButtonPrefab;

    public Button loadButton;

    private string selected_file = "";
    private string selected_timestamp;

    // Use this for initialization
    void Start () {
        FileScrollView = GameObject.Find("FileScrollView").GetComponent<ScrollRect>();
        TimestampScrollView = GameObject.Find("TimestampScrollView").GetComponent<ScrollRect>();
        controller = GameObject.Find("HistoricalDataModule").GetComponent<HistoricalDataController>();
        timecontroller = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        //        inputField = GameObject.Find("SaveInputField");
        loadButton = GameObject.Find("LoadButton").GetComponent<Button>();
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
            //gametimeEntries.Add(float.Parse(timeStampElement.Attribute("timestamp").Value.ToString()));
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
        //controller.recreateLogDataFromTimeStamp(selected_timestamp);
    }

    public void SaveLog()
    {
        controller.SaveHistoricalData();
    }
}
