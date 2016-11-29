/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* Heatmap Module
* 
* Furkan Sonmez
* Berend Wouda
* 
* Contains elements of a tutorial by Alan Zucconi (www.alanzucconi.com).
*/

using UnityEngine;
using System.Collections;

public class Heatmap : MonoBehaviour {
	private CameraControl cameraObject;

	private Vector4[] positions;
	private Vector4[] properties;
	private float[] radiuses;
	private float[] intensities;
	private Transform[] pedestrians;
	private Transform[] traffic;
	public Transform[] metro;

	public Material material;

	public int count = 5000;
	private int counted = 0;
	private int counted1 = 0;
	private int counted2 = 0;
	private static float HMIntensity = 0.1f;
	private static float HMRadius = 0.1f;
	public float maxRadius = 20f;
	public float refreshTime = 2f;
	public int minCameraHeight = 100;
	public float heightHM;

	private static bool activeHeatMaps = true;
	private static bool activatedHM = false;
	private bool zoomedIn;

	public static int heatmapNumber = 1;

	private IEnumerator refreshCoroutine;


	/// <summary>
	/// Awake method.
	/// </summary>
	void Awake() {
		cameraObject = FindObjectOfType<CameraControl>();
	}


	/// <summary>
	/// Start method.
	/// </summary>
	void Start() {
		positions = new Vector4[count];
		properties = new Vector4[count];
		pedestrians = new Transform[count];
		material.SetInt("_Points_Length", count);
		for(int i = 0; i < count; i++) {
			positions[i] = new Vector3(0, -1000, 0);
			properties[i] = new Vector2(HMRadius, HMIntensity);
		}
		material.SetVectorArray("_Points", positions);
		material.SetVectorArray("_Properties", properties);
		refreshCoroutine = heatmapRefresh();
		StartCoroutine(refreshCoroutine);
	}


	/// <summary>
	/// Put the following information about the pedestrian into the array of the heatmap method.
	/// </summary>
	public void putInArray(float posX, float posY, float posZ, Transform AnObject, int ObjectID) {
		switch(ObjectID) {
		case 1:
			if(counted > count - 1) {
				counted = 0;
			}
			pedestrians[counted] = AnObject;
			counted = counted + 1;
			break;
		case 2:
			if(counted1 > count - 1) {
				counted1 = 0;
			}
			traffic[counted1] = AnObject;
			counted1 = counted1 + 1;
			break;
		case 3:
			if(counted2 > count - 1) {
				counted2 = 0;
			}
			metro[counted2] = AnObject;
			counted2 = counted2 + 1;
			break;
		}
	}


	/// <summary>
	/// Change intensity when slider is moved.
	/// </summary>
	public static void changeParameterIntensityHM(float intensity) {
		Heatmap.HMIntensity = intensity / 80;
	}

	/// <summary>
	/// Change radius when slider is moved.
	/// </summary>
	public static void changeParameterRadiusHM(float radius) {
		Heatmap.HMRadius = radius;
	}


	/// <summary>
	/// Update method.
	/// </summary>
	public void Update() {
		if(activatedHM == true) {
			if(activeHeatMaps == false) {
				material.SetInt("_Points_Length", count);
				for(int i = 0; i < count; i++) {
					positions[i] = new Vector3(0, -1000, 0);
					properties[i] = new Vector2(HMRadius, HMIntensity);
				}
				material.SetVectorArray("_Points", positions);
				material.SetVectorArray("_Properties", properties);
			}
			activatedHM = false;
		}

		if(Input.GetKeyUp(KeyCode.H)) {
			activateDeactivateHM();
		}
	}


	/// <summary>
	/// Activate or deactivate the heatmap when button is pressed.
	/// </summary>
	public void activateDeactivateHM() {
		activatedHM = true;

		activeHeatMaps = !activeHeatMaps;
	}

	public void nextHeatmap() {
		heatmapNumber = heatmapNumber + 1;
		if(heatmapNumber > 3)
			heatmapNumber = 1;
		Debug.Log(heatmapNumber);

		for(int i = 0; i < count; i++) {
			positions[i] = new Vector3(0, -1000, 0);
			properties[i] = new Vector2(HMRadius, HMIntensity);
		}
	}

	/// <summary>
	/// Refresh the heatmap every refreshTime seconds.
	/// </summary>
	public IEnumerator heatmapRefresh() {
		yield return new WaitForSeconds(refreshTime);
		if(activeHeatMaps) {
			transform.position = new Vector3(transform.position.x, heightHM, transform.position.z);
			if(cameraObject.targetCameraPosition.y > minCameraHeight) {
				zoomedIn = false;

				switch(heatmapNumber) {
				case 1:
					for(int i = 0; i < counted; i++) {
						positions[i] = new Vector3(pedestrians[i].transform.position.x, heightHM, pedestrians[i].transform.position.z);
						if(pedestrians[i].gameObject.activeSelf == false) {
							positions[i] = new Vector3(0, -1000, 0);
						}
						properties[i] = new Vector2(HMRadius, HMIntensity);

					}
					break;
				case 2:
					for(int i = 0; i < counted1; i++) {
						positions[i] = new Vector3(traffic[i].transform.position.x, heightHM, traffic[i].transform.position.z);
						if(traffic[i].gameObject.activeSelf == false) {
							positions[i] = new Vector3(0, -1000, 0);
						}
						properties[i] = new Vector2(HMRadius, HMIntensity);
					}
					break;
				case 3:
					for(int i = 0; i < counted2; i++) {
						positions[i] = new Vector3(metro[i].transform.position.x, heightHM, metro[i].transform.position.z);
						if(metro[i].gameObject.activeSelf == false) {
							positions[i] = new Vector3(0, -1000, 0);
						}
						properties[i] = new Vector2(HMRadius, HMIntensity * (metro [i].GetComponent<VehicleController> ().delay));
					}
					break;
				}

				/*
				switch (heatmapNumber) {
				case 1:
					properties [i] = new Vector2 (HMRadius, HMIntensity);
					break;
				case 2:
					properties [i] = new Vector2 (HMRadius, HMIntensity * pedestrians [i].GetComponent<FlashPedestriansController> ().speed);
					break;
				case 3:
					properties [i] = new Vector2 (HMRadius, HMIntensity * metro [i].GetComponent<FlashPedestriansController> ().speed);
					break;
				}
				*/

				material.SetVectorArray("_Points", positions);
				material.SetVectorArray("_Properties", properties);
			}
			else if(zoomedIn == false) {
				for(int i = 0; i < count; i++) {
					positions[i] = new Vector3(0, -1000, 0);
					properties[i] = new Vector2(HMRadius, HMIntensity);
				}
				material.SetVectorArray("_Points", positions);
				material.SetVectorArray("_Properties", properties);
				zoomedIn = true;
			}

		}
		StartCoroutine(heatmapRefresh());
	}

	/// <summary>
	/// Reset the heatmap after the game ends.
	/// </summary>
	public void OnDestroy() {
		for(int i = 0; i < count; i++) {
			positions[i] = new Vector3(0, -1000, 0);
			properties[i] = new Vector2(HMRadius, HMIntensity);
		}
		material.SetVectorArray("_Points", positions);
		material.SetVectorArray("_Properties", properties);
	}
}