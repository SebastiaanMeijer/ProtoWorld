using UnityEngine;
using System.Collections;

public class fileButtonController : MonoBehaviour {

    public string path;

    private FileBrowserController browser;

	// Use this for initialization
	void Start () {
        browser = GameObject.Find("LoadFileBrowser").GetComponent<FileBrowserController>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnSelect()
    {
        browser.showFileInBrowser(path);
    }
}
