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
	public GameObject[] objectSelf;

	bool[] objectRendersA = new bool[100];
	bool[] objectScalesA = new bool[100];
	bool[] objectSelfA = new bool[100];
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

			for (int i = 0; i < objectScales.Length; i++) {
				objectScales [i].transform.localScale = new Vector3 (1, 1, 1);
			}

			foreach (GameObject r1 in objectSelf)
					r1.gameObject.SetActive (true);


		} else if (activatedMicroMacro == false) {
			activatedMicroMacro = true;

			//TODO: HERE MINIMALIZE THE MICRO MACRO VIS

			for (int i = 0; i < objectRenders.Length; i++) {
				Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
					foreach (Renderer r in rs) {
						r.enabled = objectRendersA [i];
					}
				}



			}


		}

	/*
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
*/
	/*

	/// <summary>
	/// Update method.
	/// </summary>
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

	*/

	/*
	public int id; public int typeOfDeactivate; public bool[] onOff;


	public void changeVisualizationById(int idButton){ id = idButton; } 

	public void changeVisualizationByType(int typeOfDeactivateButton){ typeOfDeactivate = typeOfDeactivateButton; }

	public void changeVisualizationBybutton(){


		//For renders
		//if (typeOfDeactivate == 1) {
			switch(typeOfDeactivate){
			case 1:
				Renderer[] rs = objectRenders [id].GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in rs)
				r.enabled = onOff[id];
				break;
			case 2:
				GameObject[] rs1 = objectSelf [id].GetComponentsInChildren<GameObject> ();
				foreach (GameObject r1 in rs1)
				r1.gameObject.SetActive(onOff[id]);
				break;
			case 3:
				for (int i = 0; i < objectScales.Length; i++) {
				if(onOff[id] == true)
						objectScales [i].transform.localScale = new Vector3 (1, 1, 1);
				else if(onOff[id] == false)
						objectScales [i].transform.localScale = new Vector3 (1, 0, 1);
				}	
				break;


			//}
		}

		if (onOff[id] == true) {
			onOff[id] = false;
		} else {
			onOff[id] = true;
		}

	}
	*/

	//public bool onOff = true;

	public void changeVisualizationRenderer(int arrayNumber){
		if (activatedMicroMacro) {

			Renderer[] rs = objectRenders [arrayNumber].GetComponentsInChildren<Renderer> ();
			if (rs [0].enabled == true) {
				foreach (Renderer r in rs) {
					r.enabled = false;
				}
				objectRendersA [arrayNumber] = false;
			} else {
				foreach (Renderer r in rs) {
					r.enabled = true;
				}
				objectRendersA [arrayNumber] = true;
			}

		}
	}

	public void changeVisualizationActive(int arrayNumber){
		if (activatedMicroMacro) {
			GameObject rs1 = objectSelf [arrayNumber];
			if (rs1.activeSelf == true) {
				rs1.gameObject.SetActive (false);
				objectSelfA [arrayNumber] = false;
			} else {
				rs1.gameObject.SetActive (true);
				objectSelfA [arrayNumber] = true;
			}
		}
	}
		/*
		foreach (GameObject r1 in rs1)
			if (rs1[1] == true) {
				r1.gameObject.SetActive (false);
			} else {
				r1.gameObject.SetActive (true);
			}
		*/


	public void changeVisualizationScale(int arrayNumber){
		if (activatedMicroMacro) {
			if (objectScales [arrayNumber].transform.localScale.y == 0) {
				objectScales [arrayNumber].transform.localScale = new Vector3 (1, 1, 1);
				objectScalesA [arrayNumber] = true;
			} else {
				objectScales [arrayNumber].transform.localScale = new Vector3 (1, 0, 1);
				objectScalesA [arrayNumber] = false;
			}	
		}

	}


			//}
		
		/*
		if (onOff[id] == true) {
			onOff[id] = false;
		} else {
			onOff[id] = true;
		}
*/



		/*
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
			*/

}
