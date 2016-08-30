/*
* 
* Visualizer for levels of details
* Furkan Sonmez
* 
*/

using UnityEngine;
using System.Collections;

public class Visualizer : MonoBehaviour {

	public CameraControl cameraObject;

	public static bool microZoomStatus = true;
	public int zoomBorder = 150;
	public bool activatedMicroMacro = true;


	public GameObject[] objectRenders;
	public GameObject[] objectScales;

	/// <summary>
	/// Awake method.
	/// </summary>
	void Awake () {
		cameraObject = FindObjectOfType<CameraControl>();
	}


	/// <summary>
	/// Activate or deactivate the visualizer when button is pressed
	/// </summary>
	public void activateDeactivateMMV(){
		if (activatedMicroMacro == true) {
			activatedMicroMacro = false;
			for (int i = 0; i < objectRenders.Length; i++) {
				Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rs)
					r.enabled = true;
			}


		} else if (activatedMicroMacro == false) {
			activatedMicroMacro = true;
			for (int i = 0; i < objectRenders.Length; i++) {
				Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rs)
					r.enabled = false;
			}
		}

		//change the scale 1 to 0 to make objects look flat
		if (activatedMicroMacro == true) {
			for (int i = 0; i < objectScales.Length; i++) {
				objectScales [i].transform.localScale = new Vector3 (1, 1, 1);
			}
		}
		else if(activatedMicroMacro == false){
			for (int i = 0; i < objectScales.Length; i++) {
				objectScales [i].transform.localScale = new Vector3 (1, 0, 1);
			}
		}


	}


	// Update is called once per frame
	void Update () {
		if (activatedMicroMacro == true) {
			if (cameraObject.targetCameraPosition.y >= zoomBorder && microZoomStatus == true) {
				microZoomStatus = false;
				changeVisualization (false);
			} else if (cameraObject.targetCameraPosition.y < zoomBorder && microZoomStatus == false) {
				microZoomStatus = true;
				changeVisualization (true);
			}

		}
	}

	public void changeVisualization(bool microStatus){
		Debug.Log (microStatus);
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


		/// <summary>
		/// Update method.
		/// </summary>
		if (microStatus == true) {
			for (int i = 0; i < objectScales.Length; i++) {
				objectScales [i].transform.localScale = new Vector3 (1, 1, 1);
			}
		}
		else if(microStatus == false){
			for (int i = 0; i < objectScales.Length; i++) {
				objectScales [i].transform.localScale = new Vector3 (1, 0, 1);
			}
		}

	}

}
