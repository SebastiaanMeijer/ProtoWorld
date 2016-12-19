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
using UnityEngine.UI;
using System.IO;

public class Heatmap : MonoBehaviour {
	private CameraControl cameraObject;

	private Vector4[] points;

	private Transform[] pedestrians;
	private Transform[] traffic;
	private Transform[] metro;

	public Material material;

	public int count = 1000;
	private int counted = 0;

	private int pedestriansCounted = 0;
	private int trafficCounted = 0;
	private int metroCounted = 0;

	public float refreshTime = 2;
	public int minCameraHeight = 100;
	public float heightHM;

	public float radiusMultiplier = 1.0f;

	private float radius;
	public float intensity = 10;

	private static float HMIntensity = 0.1f;
	private static float HMRadius = 0.1f;

	private const int gridSubdivisions = 7;

	private bool activeHeatMaps = true;
	private bool activatedHM = false;

	private bool zoomedIn;

	public int numberOfHeatmaps;

	public static int heatmapNumber = 1;
	public static int heatmapTypeNumber = 1;
	public static string typeString;

	public Text TypeText;
	
	private ComputeBuffer buffer;

	private WaitForSeconds wait;


	/// <summary>
	/// Awake method.
	/// </summary>
	void Awake() {
		TypeText = transform.parent.Find("HeatMapCanvas/HeatMapControlPanelUI/TypeText").GetComponent<Text>();
		cameraObject = FindObjectOfType<CameraControl>();
	}


	/// <summary>
	/// Start method.
	/// </summary>
	void Start() {
		points = new Vector4[count];

		pedestrians = new Transform[count];
		traffic = new Transform[count];
		metro = new Transform[count];
		
		buffer = new ComputeBuffer(count, 16);
		material.SetBuffer("points", buffer);

		resetPoints();

		wait = new WaitForSeconds(refreshTime);
		StartCoroutine(heatmapRefresh());
	}


	/// <summary>
	/// Put the following information about the pedestrian into the array of the heatmap method.
	/// </summary>
	public void putInArray(float posX, float posY, float posZ, Transform AnObject, int ObjectID) {
		switch(ObjectID) {
			case 1:
				if(pedestriansCounted > count - 1) {
					pedestriansCounted = 0;
				}
				pedestrians[pedestriansCounted] = AnObject;
				pedestriansCounted = pedestriansCounted + 1;
				break;
			case 2:
				if(trafficCounted > count - 1) {
					trafficCounted = 0;
				}
				traffic[trafficCounted] = AnObject;
				trafficCounted = trafficCounted + 1;
				break;
			case 3:
				if(metroCounted > count - 1) {
					metroCounted = 0;
				}
				metro[metroCounted] = AnObject;
				metroCounted = metroCounted + 1;
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
				resetPoints();
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
		heatmapTypeNumber = 1;

		if(heatmapNumber > numberOfHeatmaps) {
			heatmapNumber = 1;
		}

		resetPoints();
	}

	public void previousHeatmap() {
		heatmapNumber = heatmapNumber - 1;
		heatmapTypeNumber = 1;

		if(heatmapNumber < 1) {
			heatmapNumber = numberOfHeatmaps;
		}

		resetPoints();
	}

	public void nextType() {
		heatmapTypeNumber = heatmapTypeNumber + 1;

		if(heatmapTypeNumber > 3) {
			heatmapTypeNumber = 1;
		}

		resetPoints();
	}

	public void previousType() {
		heatmapTypeNumber = heatmapTypeNumber - 1;

		if(heatmapTypeNumber < 1) {
			heatmapTypeNumber = 3;
		}

		resetPoints();
	}

	/// <summary>
	/// Refresh the heatmap every refreshTime seconds.
	/// </summary>
	public IEnumerator heatmapRefresh() {
		yield return wait;

		// Convert the radius to a factor depending on the scale and the size of the grid:
		// - "radiusMultiplier" is a user set value to allow for adjusting the radius in advance without influencing the UI slider.
		// - "transform.localScale.x" is the scale of the grid. Make sure it is uniformly scaled!!!
		// - "0.5 * (0.5 ^ gridSubdivisions) / cos(30)" is the distance from the center point to a vertex of a triangle in the grid. This ensures that all data points influence at least one vertex by default.
		radius = HMRadius * radiusMultiplier * transform.localScale.x * 0.5f * Mathf.Pow(0.5f, gridSubdivisions) / Mathf.Cos(30.0f * Mathf.Deg2Rad);

		if(activeHeatMaps) {
			transform.position = new Vector3(transform.position.x, heightHM, transform.position.z);
			if(cameraObject.targetCameraPosition.y > minCameraHeight) {
				zoomedIn = false;

				switch(heatmapNumber) {
					case 1:
						updatePointsFromPedestrians(heatmapTypeNumber);
						break;
					case 2:
						updatePointsFromTraffic(heatmapTypeNumber);
						break;
					case 3:
						updatePointsFromTransport(heatmapTypeNumber);
						break;
					case 4:
						updatePointsFromMetro(heatmapTypeNumber);
						break;
					case 5:
						updatePointsFromTrain(heatmapTypeNumber);
						break;
				}
			}
			else if(zoomedIn == false) {
				resetPoints();
				zoomedIn = true;
			}
		}
		StartCoroutine(heatmapRefresh());
	}

	/// <summary>
	/// Reset the heatmap after the game ends.
	/// </summary>
	public void OnDestroy() {
		resetPoints();

		buffer.Release();
	}


	private void updatePointsFromPedestrians(int type) {
		for(int i = 0; i < pedestriansCounted; i++) {
			if(pedestrians[i].gameObject.activeSelf) {
				switch(type){
				case 1:
					typeString = "Locations";
					points [i] = new Vector4 (pedestrians [i].transform.position.x, heightHM, pedestrians [i].transform.position.z, intensity);
					break;
				default:
					typeString = "Locations";
					points [i] = new Vector4 (pedestrians [i].transform.position.x, heightHM, pedestrians [i].transform.position.z, intensity);
					break;
				}
			}
			else {
				points[i] = new Vector4(0, 0, 0, 0);
			}
		}
		counted = pedestriansCounted;
		updateShader();
	}

	private void updatePointsFromTraffic(int type) {
		for(int i = 0; i < trafficCounted; i++) {
			if(traffic[i].gameObject.activeSelf) {
				switch (type) {
				case 1:
					typeString = "Locations";
					points [i] = new Vector4 (traffic [i].transform.position.x, heightHM, traffic [i].transform.position.z, intensity);
					break;
				default:
					typeString = "Locations";
					points [i] = new Vector4 (traffic [i].transform.position.x, heightHM, traffic [i].transform.position.z, intensity);
					break;
				}
			}
			else {
				points[i] = new Vector4(0, 0, 0, 0);
			}
		}
		counted = trafficCounted;
		updateShader();
	}

	private void updatePointsFromTransport(int type) {
		for(int i = 0; i < metroCounted; i++) {
			if(metro[i].gameObject.activeSelf) {
				switch (type) {
				case 1:
					typeString = "Locations";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, intensity);
					break;
				case 2:
					typeString = "Delays";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, metro [i].GetComponent<VehicleController> ().delay);
					break;
				case 3:
					typeString = "Passengers";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, metro [i].GetComponent<VehicleController> ().headCount);
					break;
				default:
					typeString = "Locations";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, intensity);
					break;
				}
			}
			else {
				points[i] = new Vector4(0, 0, 0, 0);
			}
		}
		counted = metroCounted;
		updateShader();
	}

	private void updatePointsFromMetro(int type) {
		for(int i = 0; i < metroCounted; i++) {
			if(metro[i].gameObject.activeSelf) {
				if (metro [i].gameObject.GetComponent<VehicleController> ().line.category == LineCategory.Metro)
					switch (type) {
				case 1:
					typeString = "Locations";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, intensity);
					break;
				case 2:
					typeString = "Delays";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, metro [i].GetComponent<VehicleController> ().delay);
					break;
				case 3:
					typeString = "Passengers";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, metro [i].GetComponent<VehicleController> ().headCount);
					break;
				default:
					typeString = "Locations";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, intensity);
					break;
				}
			}
			else {
				points[i] = new Vector4(0, 0, 0, 0);
			}
		}
		counted = metroCounted;
		updateShader();
	}

	private void updatePointsFromTrain(int type) {
		for(int i = 0; i < metroCounted; i++) {
			if(metro[i].gameObject.activeSelf) {
				if (metro [i].gameObject.GetComponent<VehicleController> ().line.category == LineCategory.Train)
					switch (type) {
				case 1:
					typeString = "Locations";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, intensity);
					break;
				case 2:
					typeString = "Delays";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, metro [i].GetComponent<VehicleController> ().delay);
					break;
				case 3:
					typeString = "Passengers";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, metro [i].GetComponent<VehicleController> ().headCount);
					break;
				default:
					typeString = "Locations";
					points [i] = new Vector4 (metro [i].transform.position.x, heightHM, metro [i].transform.position.z, intensity);
					break;
				}
			}
			else {
				points[i] = new Vector4(0, 0, 0, 0);
			}
		}
		counted = metroCounted;
		updateShader();
	}


	private void resetPoints() {
		for(int i = 0; i < count; i++) {
			points[i] = new Vector4(0, 0, 0, 0);
		}
		counted = 0;
		updateShader();
	}


	private void updateShader() {
		TypeText.text = typeString;

		material.SetInt("count", counted);
		material.SetFloat("radius", radius);

        if(points != null)
		    buffer.SetData(points);
	}
}