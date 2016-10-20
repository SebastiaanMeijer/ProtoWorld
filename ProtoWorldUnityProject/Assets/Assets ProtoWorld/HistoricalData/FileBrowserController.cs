using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Linq;

public class FileBrowserController : MonoBehaviour {

    private ScrollRect FileScrollView;
    private ScrollRect TimestampScrollView;

    public GameObject timestampButtonPrefab;

    // Use this for initialization
    void Start () {
        FileScrollView = GameObject.Find("FileScrollView").GetComponent<ScrollRect>();
        TimestampScrollView = GameObject.Find("TimestampScrollView").GetComponent<ScrollRect>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void showFileInBrowser(string path)
    {
        Debug.Log("Show " + path);

        //Fetch all timestamps
        XDocument logFile = XDocument.Load(path);
        XElement logFileRootElement = logFile.Root;

        Dictionary<float, XElement> historicalTimeStamps = new Dictionary<float, XElement>();

        List<System.Xml.Linq.XElement> timeStampElements =
            (from timeStampElement in logFileRootElement.Descendants("TimeStamp")
             select timeStampElement).ToList();


        foreach (XElement timeStampElement in timeStampElements)
        {
            //gametimeEntries.Add(float.Parse(timeStampElement.Attribute("timestamp").Value.ToString()));
            historicalTimeStamps.Add(float.Parse(timeStampElement.Attribute("timestamp").Value.ToString()), timeStampElement);
        }

        foreach (GameObject item in TimestampScrollView.content)
        {
            GameObject.Destroy(item);
        }
        foreach (KeyValuePair<float, System.Xml.Linq.XElement> timeStamp in historicalTimeStamps)
        //Create timestamp button prefab
        {
            //Add file to the fileScrollView
            GameObject fileButton = Instantiate(timestampButtonPrefab) as GameObject;
            fileButton.transform.SetParent(TimestampScrollView.content.transform);

            Text btn = fileButton.GetComponentInChildren<Text>();
            btn.text = timeStamp.Key.ToString();
        }
    }
}
