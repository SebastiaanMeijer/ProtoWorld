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

	public int previousLevel;

	public GameObject[] objectRenders;
	public int[] objectRendersLvl;
	public GameObject[] objectScales;
	public int[] objectScalesLvl;
	public GameObject[] objectSelf;
	public int[] objectSelfLvl;

	bool[] objectRendersA = new bool[100];
	bool[] objectScalesA = new bool[100];
	bool[] objectSelfA = new bool[100];
	/// <summary>
	/// Awake method.
	/// </summary>
	void Awake () {
		cameraObject = FindObjectOfType<CameraControl>();
	}


	public void Update(){

		if (previousLevel != ZoomScrollbarMMV.level) {
			previousLevel = ZoomScrollbarMMV.level;
			for (int i = 0; objectRenders.Length > i; i++) {
				
				changeVisualizationRenderer (previousLevel,objectRendersLvl[i]);
				changeVisualizationActive (previousLevel,objectScalesLvl[i]);
				changeVisualizationScale (previousLevel,objectSelfLvl[i]);
			}


		}

	}

	/// <summary>
	/// Activate or deactivate the visualizer when button is pressed
	/// </summary>
	public void activateDeactivateMMV(){
		if (activatedMicroMacro == true) {
			
			for (int i = 0; objectRenders.Length > i; i++) {
				changeVisualizationRenderer (-1, objectRendersLvl [i]);
				changeVisualizationActive (-1, objectScalesLvl [i]);
				changeVisualizationScale (-1, objectSelfLvl [i]);

			}
			activatedMicroMacro = false;

		} else {
			activatedMicroMacro = true;

			for (int i = 0; objectRenders.Length > i; i++) {

				changeVisualizationRenderer (previousLevel,objectRendersLvl[i]);
				changeVisualizationActive (previousLevel,objectScalesLvl[i]);
				changeVisualizationScale (previousLevel,objectSelfLvl[i]);
			}

		}


		}


public void changeVisualizationRenderer(int levelNumber, int arrayNumber){
		if (activatedMicroMacro) {

		for (int i = 0; i < objectRenders.Length; i++) {
				if (objectRenders [i] != null) {
					Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
					foreach (Renderer r in rs)
						r.enabled = true;
				}
		}



			for (int i = 0; i < objectRenders.Length; i++) {
				if (objectRenders [i] != null) {
					Renderer[] rs = objectRenders [i].GetComponentsInChildren<Renderer> ();
					if (objectRendersLvl [i] > levelNumber) {
						foreach (Renderer r in rs) {
							r.enabled = true;
						}
					} else if (objectRendersLvl [i] <= levelNumber) {
						foreach (Renderer r in rs) {
							r.enabled = false;
						}


					}
				}
			}
			
		}
	}

	public void changeVisualizationActive(int levelNumber, int arrayNumber){
		if (activatedMicroMacro) {


		for (int i = 0; i < objectSelf.Length; i++) {
			GameObject rs1 = objectSelf [i];
				if (rs1 != null) {
					if (objectSelfLvl [i] > levelNumber) {
						rs1.gameObject.SetActive (true);

					} else if (objectSelfLvl [i] <= levelNumber) {
						rs1.gameObject.SetActive (false);
					}
				}
		}

		}
	}


	public void changeVisualizationScale(int levelNumber, int arrayNumber){
		if (activatedMicroMacro) {
			for (int i = 0; i < objectScales.Length; i++) {
				GameObject rs1 = objectSelf [i];
				if (rs1 != null) {
					if (objectScalesLvl [i] > levelNumber) {
						objectScales [i].transform.localScale = new Vector3 (1, 1, 1);
						objectScalesA [i] = true;
					} else {

						objectScales [i].transform.localScale = new Vector3 (1, 0, 1);
						objectScalesA [i] = false;
					}
				}
			}
		}
	}
}
