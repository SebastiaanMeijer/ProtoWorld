using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeCameraFeed : MonoBehaviour {


	public RenderTexture CameraRender1;
	public RenderTexture CameraRender2;
	public RenderTexture CameraRender3;

	public GameObject FeedCamerasObject;

	public Text cameraFeed1Text;
	public Text cameraFeed2Text;
	public Text cameraFeed3Text;

	public int i = 0;
	// Use this for initialization
	void Awake () {
		cameraFeed1Text = GameObject.Find("CameraFeed1Text").GetComponent<Text> ();
		cameraFeed2Text = GameObject.Find("CameraFeed2Text").GetComponent<Text> ();
		cameraFeed3Text = GameObject.Find("CameraFeed3Text").GetComponent<Text> ();
		FeedCamerasObject = GameObject.Find ("FeedCameras");
	}

	void Start(){
		FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().enabled = true;
		FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().targetTexture = CameraRender1;
		cameraFeed1Text.text = FeedCamerasObject.transform.GetChild (i).name;
		FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().enabled = true;
		FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().targetTexture = CameraRender2;
		cameraFeed2Text.text = FeedCamerasObject.transform.GetChild (i+1).name;
		FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().enabled = true;
		FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().targetTexture = CameraRender3;
		cameraFeed3Text.text = FeedCamerasObject.transform.GetChild (i+2).name;

	}
	
	// Update is called once per frame
	void Update () {
	



	}

	public void nextCameraFeed(){

		if (i +3 < FeedCamerasObject.transform.childCount) {
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().enabled = false;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().enabled = false;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().enabled = false;
			i = i + 1;
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().enabled = true;
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().targetTexture = CameraRender1;
			cameraFeed1Text.text = FeedCamerasObject.transform.GetChild (i).name;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().enabled = true;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().targetTexture = CameraRender2;
			cameraFeed2Text.text = FeedCamerasObject.transform.GetChild (i+1).name;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().enabled = true;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().targetTexture = CameraRender3;
			cameraFeed3Text.text = FeedCamerasObject.transform.GetChild (i+2).name;
		}
	}

	public void previousCameraFeed(){

		if (i > 0) {
			/*
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().targetTexture = null;
			i = i - 1;
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().targetTexture = CameraRender1;
			cameraFeed1Text.text = FeedCamerasObject.transform.GetChild (i).name;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().targetTexture = CameraRender2;
			cameraFeed2Text.text = FeedCamerasObject.transform.GetChild (i+1).name;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().targetTexture = CameraRender3;
			cameraFeed3Text.text = FeedCamerasObject.transform.GetChild (i+2).name;
			*/
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().enabled = false;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().enabled = false;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().targetTexture = null;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().enabled = false;
			i = i - 1;
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().enabled = true;
			FeedCamerasObject.transform.GetChild (i).GetComponent<Camera> ().targetTexture = CameraRender1;
			cameraFeed1Text.text = FeedCamerasObject.transform.GetChild (i).name;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().enabled = true;
			FeedCamerasObject.transform.GetChild (i + 1).GetComponent<Camera> ().targetTexture = CameraRender2;
			cameraFeed2Text.text = FeedCamerasObject.transform.GetChild (i+1).name;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().enabled = true;
			FeedCamerasObject.transform.GetChild (i + 2).GetComponent<Camera> ().targetTexture = CameraRender3;
			cameraFeed3Text.text = FeedCamerasObject.transform.GetChild (i+2).name;
		}
	}




}


