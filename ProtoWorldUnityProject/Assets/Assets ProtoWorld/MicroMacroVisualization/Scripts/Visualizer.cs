using UnityEngine;
using System.Collections;

public class Visualizer : MonoBehaviour {

	public CameraControl cameraObject;

	public static bool microZoomStatus;
	public int zoomBorder = 150;

	public GameObject[] objectRenders;

	// Use this for initialization
	void Awake () {
		cameraObject = FindObjectOfType<CameraControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (cameraObject.targetCameraPosition.y >= zoomBorder && microZoomStatus == true) {
			microZoomStatus = false;
			changeVisualization (false);
		} 
		else if (cameraObject.targetCameraPosition.y < zoomBorder && microZoomStatus == false) {
			microZoomStatus = true;
			changeVisualization (true);
		}

	}

	public void changeVisualization(bool microStatus){
		
		//activate or deactivate render when zoomed in/out
		if (microStatus == true) {
			for (int i = 0; i < objectRenders.Length; i++) {
				Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rs)
					r.enabled = true;
			}
		}
		else if(microStatus == false){
			for (int i = 0; i < objectRenders.Length; i++) {
				Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rs)
					r.enabled = false;
			}
		}

		/*
		//change opacity of render when zoomed in/out
		if (microStatus == true) {
			for (int i = 0; i < objectRenders.Length; i++) {
				Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rs)
					r.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
		}
		else if(microStatus == false){
			for (int i = 0; i < objectRenders.Length; i++) {
				Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rs)
					r.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
			}
		}
		*/
	}

}
