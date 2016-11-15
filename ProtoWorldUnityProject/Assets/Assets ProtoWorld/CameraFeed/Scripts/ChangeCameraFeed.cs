using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeCameraFeed : MonoBehaviour {
	public RenderTexture cameraRender1;
	public RenderTexture cameraRender2;
	public RenderTexture cameraRender3;

	[HideInInspector]
	public GameObject feedCamerasObject;

	[HideInInspector]
	public Text cameraFeed1Text;

	[HideInInspector]
	public Text cameraFeed2Text;

	[HideInInspector]
	public Text cameraFeed3Text;

	[HideInInspector]
	public int i = 0;
	
	void Awake() {
		cameraFeed1Text = GameObject.Find("CameraFeed1Text").GetComponent<Text>();
		cameraFeed2Text = GameObject.Find("CameraFeed2Text").GetComponent<Text>();
		cameraFeed3Text = GameObject.Find("CameraFeed3Text").GetComponent<Text>();
		feedCamerasObject = GameObject.Find("FeedCameras");
	}

	void Start() {
		// Enable these.
		feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().enabled = true;
		feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().targetTexture = cameraRender1;
		feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().enabled = true;
		feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().targetTexture = cameraRender2;
		feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().enabled = true;
		feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().targetTexture = cameraRender3;

		// Disable these.
		feedCamerasObject.transform.GetChild(i + 3).GetComponent<Camera>().enabled = false;
		feedCamerasObject.transform.GetChild(i + 3).GetComponent<Camera>().targetTexture = null;
		feedCamerasObject.transform.GetChild(i + 4).GetComponent<Camera>().enabled = false;
		feedCamerasObject.transform.GetChild(i + 4).GetComponent<Camera>().targetTexture = null;
		feedCamerasObject.transform.GetChild(i + 5).GetComponent<Camera>().enabled = false;
		feedCamerasObject.transform.GetChild(i + 5).GetComponent<Camera>().targetTexture = null;

		// Rename these.
		cameraFeed1Text.text = feedCamerasObject.transform.GetChild(i).gameObject.GetComponent<FeedCamera>().name;
		cameraFeed2Text.text = feedCamerasObject.transform.GetChild(i + 1).gameObject.GetComponent<FeedCamera>().name;
		cameraFeed3Text.text = feedCamerasObject.transform.GetChild(i + 2).gameObject.GetComponent<FeedCamera>().name;
	}
	
	public void nextCameraFeed() {
		if(i + 3 < feedCamerasObject.transform.childCount) {
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().targetTexture = null;
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().enabled = false;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().targetTexture = null;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().enabled = false;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().targetTexture = null;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().enabled = false;
			i = i + 1;
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().enabled = true;
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().targetTexture = cameraRender1;
			cameraFeed1Text.text = feedCamerasObject.transform.GetChild(i).gameObject.GetComponent<FeedCamera>().name;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().enabled = true;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().targetTexture = cameraRender2;
			cameraFeed2Text.text = feedCamerasObject.transform.GetChild(i + 1).gameObject.GetComponent<FeedCamera>().name;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().enabled = true;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().targetTexture = cameraRender3;
			cameraFeed3Text.text = feedCamerasObject.transform.GetChild(i + 2).gameObject.GetComponent<FeedCamera>().name;
		}
	}

	public void previousCameraFeed() {
		if(i > 0) {
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().targetTexture = null;
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().enabled = false;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().targetTexture = null;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().enabled = false;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().targetTexture = null;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().enabled = false;
			i = i - 1;
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().enabled = true;
			feedCamerasObject.transform.GetChild(i).GetComponent<Camera>().targetTexture = cameraRender1;
			cameraFeed1Text.text = feedCamerasObject.transform.GetChild(i).gameObject.GetComponent<FeedCamera>().name;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().enabled = true;
			feedCamerasObject.transform.GetChild(i + 1).GetComponent<Camera>().targetTexture = cameraRender2;
			cameraFeed2Text.text = feedCamerasObject.transform.GetChild(i + 1).gameObject.GetComponent<FeedCamera>().name;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().enabled = true;
			feedCamerasObject.transform.GetChild(i + 2).GetComponent<Camera>().targetTexture = cameraRender3;
			cameraFeed3Text.text = feedCamerasObject.transform.GetChild(i + 2).gameObject.GetComponent<FeedCamera>().name;
		}
	}
}


