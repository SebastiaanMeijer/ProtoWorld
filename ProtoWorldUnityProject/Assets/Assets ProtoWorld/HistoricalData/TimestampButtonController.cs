using UnityEngine;
using System.Collections;

public class TimestampButtonController : MonoBehaviour {

    FileBrowserController controller;

    public string timestamp;

	// Use this for initialization
	void Start () {
        controller = GameObject.Find("LoadFileBrowser").GetComponent<FileBrowserController>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnButtonClick()
    {
        controller.SelectTimestamp(timestamp);
    }
}
 