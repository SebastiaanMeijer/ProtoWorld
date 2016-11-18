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
	public int i;
	
	void Awake() {
		cameraFeed1Text = GameObject.Find("CameraFeed1Text").GetComponent<Text>();
		cameraFeed2Text = GameObject.Find("CameraFeed2Text").GetComponent<Text>();
		cameraFeed3Text = GameObject.Find("CameraFeed3Text").GetComponent<Text>();
		feedCamerasObject = GameObject.Find("FeedCameras");
	}

	void Start() {
		i = 0;

		setFeedCamerasEnabled(i);
	}
	

	public void nextCameraFeed() {
		if(i + 3 < getFeedCameraCount()) {
			i += 1;

			setFeedCamerasEnabled(i);
		}
	}

	public void previousCameraFeed() {
		if(i > 0) {
			i = i - 1;

			setFeedCamerasEnabled(i);
		}
	}


	private int getFeedCameraCount() {
		int cameraCount = 0;

		for(int index = 0; index < feedCamerasObject.transform.childCount; index++) {
			Camera feedCamera = feedCamerasObject.transform.GetChild(index).gameObject.GetComponent<Camera>();

			if(feedCamera != null) {
				cameraCount += 1;
			}
		}

		return cameraCount;
	}

	private void setFeedCamerasEnabled(int startIndex) {
		// Enable the first three feed cameras (from the starting position) and disable the rest.
		int cameraIndex = -startIndex;
		
		for(int index = 0; index < feedCamerasObject.transform.childCount; index++) {
			Camera feedCamera = feedCamerasObject.transform.GetChild(index).gameObject.GetComponent<Camera>();

			if(feedCamera != null) {
				if(cameraIndex == 0) {
					feedCamera.enabled = true;
					feedCamera.targetTexture = cameraRender1;
					cameraFeed1Text.text = feedCamera.gameObject.GetComponent<FeedCamera>().name;
				}
				else if(cameraIndex == 1) {
					feedCamera.enabled = true;
					feedCamera.targetTexture = cameraRender2;
					cameraFeed2Text.text = feedCamera.gameObject.GetComponent<FeedCamera>().name;
				}
				else if(cameraIndex == 2) {
					feedCamera.enabled = true;
					feedCamera.targetTexture = cameraRender3;
					cameraFeed3Text.text = feedCamera.gameObject.GetComponent<FeedCamera>().name;
				}
				else {
					feedCamera.enabled = false;
					feedCamera.targetTexture = null;
				}

				cameraIndex += 1;
			}
		}
	}
}


