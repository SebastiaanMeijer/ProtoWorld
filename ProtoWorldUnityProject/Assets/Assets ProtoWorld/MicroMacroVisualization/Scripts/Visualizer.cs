/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Micro/Macro Visualization Module
 * 
 * Furkan Sonmez
 * Berend Wouda
 */

using UnityEngine;
using System.Collections;

public class Visualizer : MonoBehaviour {
	private CameraControl cameraObject;

	private bool activatedMicroMacro = true;

	private int previousLevel;

	public GameObject[] objectRenders;
	public int[] objectRendersLvl;
	public GameObject[] objectScales;
	public int[] objectScalesLvl;
	public GameObject[] objectSelf;
	public int[] objectSelfLvl;


	/// <summary>
	/// Awake method.
	/// </summary>
	void Awake() {
		cameraObject = FindObjectOfType<CameraControl>();
	}


	public void Update() {
		if(previousLevel != ZoomScrollbarMMV.level) {
			previousLevel = ZoomScrollbarMMV.level;
			for(int i = 0; objectRenders.Length > i; i++) {
				changeVisualizationRenderer(previousLevel, objectRendersLvl[i]);
				changeVisualizationActive(previousLevel, objectScalesLvl[i]);
				//changeVisualizationScale (previousLevel,objectSelfLvl[i]);
			}
		}
	}

	/// <summary>
	/// Activate or deactivate the visualizer when button is pressed
	/// </summary>
	public void activateDeactivateMMV() {
		if(activatedMicroMacro == true) {
			for(int i = 0; objectRenders.Length > i; i++) {
				changeVisualizationRenderer(-1, objectRendersLvl[i]);
				changeVisualizationActive(-1, objectScalesLvl[i]);
				//changeVisualizationScale (-1, objectSelfLvl [i]);

			}
			activatedMicroMacro = false;
		}
		else {
			activatedMicroMacro = true;

			for(int i = 0; objectRenders.Length > i; i++) {
				changeVisualizationRenderer(previousLevel, objectRendersLvl[i]);
				changeVisualizationActive(previousLevel, objectScalesLvl[i]);
				//changeVisualizationScale (previousLevel,objectSelfLvl[i]);
			}
		}
	}


	public void changeVisualizationRenderer(int levelNumber, int arrayNumber) {
		if(activatedMicroMacro) {
			for(int i = 0; i < objectRenders.Length; i++) {
				if(objectRenders[i] != null) {
					Renderer[] rs = objectRenders[i].GetComponentsInChildren<Renderer>();
					foreach(Renderer r in rs)
						r.enabled = true;
				}
			}

			cameraObject.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Pedestrian");

			for(int i = 0; i < objectRenders.Length; i++) {
				if(objectRenders[i] != null) {
					Renderer[] rs = objectRenders[i].GetComponentsInChildren<Renderer>();
					if(objectRendersLvl[i] > levelNumber && objectRendersLvl[i] >= 0) {
						var newMask = cameraObject.GetComponent<Camera>().cullingMask | (1 << objectRenders[i].gameObject.layer);
						cameraObject.GetComponent<Camera>().cullingMask = newMask;

						foreach(Renderer r in rs) {
							//r.enabled = true;
							//LayerMask.NameToLayer("Pedestrian");
						}
					}
					else if(objectRendersLvl[i] <= levelNumber && objectRendersLvl[i] >= 0) {
						var newMask = cameraObject.GetComponent<Camera>().cullingMask & ~(1 << objectRenders[i].gameObject.layer);
						cameraObject.GetComponent<Camera>().cullingMask = newMask;

						foreach(Renderer r in rs) {

						}
					}
					else if(objectRendersLvl[i] > -levelNumber && objectRendersLvl[i] < 0) {
						var newMask = cameraObject.GetComponent<Camera>().cullingMask | (1 << objectRenders[i].gameObject.layer);
						cameraObject.GetComponent<Camera>().cullingMask = newMask;

						foreach(Renderer r in rs) {
							//r.enabled = true;
						}
					}
					else if(objectRendersLvl[i] <= -levelNumber && objectRendersLvl[i] < 0) {
						var newMask = cameraObject.GetComponent<Camera>().cullingMask & ~(1 << objectRenders[i].gameObject.layer);
						cameraObject.GetComponent<Camera>().cullingMask = newMask;
						foreach(Renderer r in rs) {
							//r.enabled = false;
						}
					}
				}
			}

		}
	}

	public void changeVisualizationActive(int levelNumber, int arrayNumber) {
		if(activatedMicroMacro) {
			for(int i = 0; i < objectSelf.Length; i++) {
				GameObject rs1 = objectSelf[i];
				if(rs1 != null) {
					if(objectSelfLvl[i] > levelNumber && objectSelfLvl[i] >= 0) {
						rs1.gameObject.SetActive(true);
					}
					else if(objectSelfLvl[i] <= levelNumber && objectSelfLvl[i] >= 0) {
						rs1.gameObject.SetActive(false);
					}
					else if(objectSelfLvl[i] > -levelNumber && objectSelfLvl[i] < 0) {
						rs1.gameObject.SetActive(true);
					}
					else if(objectSelfLvl[i] <= -levelNumber && objectSelfLvl[i] < 0) {
						rs1.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public void changeVisualizationScale(int levelNumber, int arrayNumber) {
		if(activatedMicroMacro) {
			for(int i = 0; i < objectScales.Length; i++) {
				if(objectSelf[i] != null) {
					GameObject rs1 = objectSelf[i];

					if(objectScalesLvl[i] > levelNumber && objectScalesLvl[i] >= 0) {
						objectScales[i].transform.localScale = new Vector3(1, 1, 1);
					}
					else if(objectScalesLvl[i] < levelNumber && objectScalesLvl[i] >= 0) {
						objectScales[i].transform.localScale = new Vector3(1, 0, 1);
					}
					else if(objectScalesLvl[i] > -levelNumber && objectScalesLvl[i] < 0) {
						objectScales[i].transform.localScale = new Vector3(1, 1, 1);
					}
					else if(objectScalesLvl[i] < -levelNumber && objectScalesLvl[i] < 0) {
						objectScales[i].transform.localScale = new Vector3(1, 0, 1);
					}
				}
			}
		}
	}
}
