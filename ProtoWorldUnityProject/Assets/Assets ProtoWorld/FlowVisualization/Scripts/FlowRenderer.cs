/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Flow Visualisation Module
 * 
 * Antony Löbker
 * Berend Wouda
 */

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class FlowRenderer : MonoBehaviour {
	public enum CSVtype { Haifa, Venice };
	public CSVtype fileType;
	public TextAsset[] files;
	public MapBoundaries boundaries;

	private Material lineMaterial;
	private double realMinX, realMinY, realMaxX, realMaxY, gameMinX, gameMinY, gameMaxX, gameMaxY;

	private struct Path : IEquatable<Path> {
		public string from;
		public string to;

		public int timeStep;


		public Path(string from, string to, int timeStep) {
			this.from = from;
			this.to = to;
			this.timeStep = timeStep;
		}


		public bool Equals(Path other) {
			return other.from.Equals(from) && other.to.Equals(to) && other.timeStep.Equals(timeStep);
		}

		public override bool Equals(object other) {
			return other is Path && Equals((Path) other);
		}

		public override int GetHashCode() {
			// The quality of this hash function matters a lot in start-up time.
			return ((from.GetHashCode() ^ to.GetHashCode()) * 33) ^ timeStep.GetHashCode();
		}
	}

	private List<string> names;
	private Dictionary<string, Vector3> points;
	private Dictionary<Path, float> values;
	private Dictionary<Path, LineRenderer> lineRenderers;

	public Slider slider;
	
	private float minimumValue;
	private float maximumValue;

	private Dictionary<Path, float> minimumValues;
	private Dictionary<Path, float> maximumValues;

	public int sample = 10;

	public float lineWidth = 10.0f;
	public float laneWidth = 10.0f;
	

	void Start() {
		names = new List<string>();
		points = new Dictionary<string, Vector3>();
		values = new Dictionary<Path, float>();
		lineRenderers = new Dictionary<Path, LineRenderer>();
		minimumValues = new Dictionary<Path, float>();
		maximumValues = new Dictionary<Path, float>();

		lineMaterial = new Material(Shader.Find("SolidColor")); //Particles/Alpha Blended
		if(boundaries == null) {
			Transform scen = GameObject.Find("ScenarioModule").transform;
			Transform bound = scen.FindChild("AramGISBoundaries");
			boundaries = bound.GetComponent<MapBoundaries>();
		}

		//get latitude and longitude boundaries
		realMinX = boundaries.dbBoundMinLat;
		realMinY = boundaries.dbBoundMinLon;
		realMaxX = boundaries.dbBoundMaxLat;
		realMaxY = boundaries.dbBoundMaxLon;

		//get x and y boundaries
		gameMinX = boundaries.minMaxX[0] + boundaries.MinPointOnMap.x;
		gameMinY = boundaries.minMaxY[0] + boundaries.MinPointOnMap.z;
		gameMaxX = boundaries.minMaxX[1] + boundaries.MinPointOnMap.x;
		gameMaxY = boundaries.minMaxY[1] + boundaries.MinPointOnMap.z;

		minimumValue = float.MaxValue;
		maximumValue = float.MinValue;

		if(fileType == CSVtype.Haifa) {
			//loop over all files
			for(int i = 0; i < files.Length; i++) {
				TextAsset file = files[i];
				readCSVHaifa(i, file.name, file.text);
			}
		}
		else {
			if(files.Length < 2) {
				Debug.LogWarning("The Venice type requires two files");
				return;
			}
			TextAsset positions = files[0];
			TextAsset distances = files[1];
			readCSVVenice(positions.text, distances.text);
		}

		slid(0.0f);
	}


	private void readCSVHaifa(int index, string name, string text) {
		//GameObject parentObj = new GameObject(name);
		//parentObj.transform.parent = transform;

		string[] lines = text.Split("\n"[0]);
		foreach(string line in lines) {
			string[] parts = line.Split(',');
			// start_antenna, s_lat, s_long, end_antenna, e_lat, e_long, value
			if(parts.Length < 6 || parts[0] == "start_antenna")
				continue;

			//convert lat/long to ingame positions
			double x1 = double.Parse(parts[1]);
			double y1 = double.Parse(parts[2]);
			double x2 = double.Parse(parts[4]);
			double y2 = double.Parse(parts[5]);
			Vector3 pos1 = realToGameCoords(x1, y1);
			Vector3 pos2 = realToGameCoords(x2, y2);

			float value = float.Parse(parts[6]);
			if(value > 1.5f) {
				value = Mathf.Sqrt(value) / 100;
			}

			//create line
			//createLine(parentObj.transform, pos1, pos2, value);
			
			string from = parts[0];
			string to = parts[3];

			if(!names.Contains(from)) {
				names.Add(from);
				points[from] = pos1;
			}

			if(!names.Contains(to)) {
				names.Add(to);
				points[to] = pos2;
			}

			Path path = new Path(from, to, index);
			values[path] = value;

			Path fakePath = new Path(from, to, 0);
			
			//if(value < minimumValue) {
			//	minimumValue = value;
			//}

			minimumValue = 0.0f;
			
			if(value > maximumValue) {
				maximumValue = value;
			}
		}

		if(slider != null) {
			slider.maxValue = index;
		}
	}

	private void readCSVVenice(string text, string text2) {
		//flip files if they are in the wrong order
		if(text.Length > text2.Length) {
			string temp = text;
			text = text2;
			text2 = temp;
		}

		List<string> ids = new List<string>();
		Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
		string[] parts;

		string[] lines = text.Split("\n"[0]);
		foreach(string line in lines) {
			parts = line.Split(',');
			if(parts.Length < 4)
				continue;

			double x1 = double.Parse(parts[1]);
			double y1 = double.Parse(parts[2]);
			Vector3 pos = realToGameCoords(x1, y1);
			positions.Add(parts[0].Trim('"'), pos);
		}

		lines = text2.Split("\n"[0]);
		parts = lines[0].Split(',');
		foreach(string part in parts) {
			ids.Add(part.Trim('"'));
		}

		//GameObject parentObj = new GameObject("" + lines[1]);
		//parentObj.transform.parent = transform;
		int firstOffset = 1;
		int timeStep = 0;
		int timeSteps = 0;
		for(int i = 2; i < lines.Length; i++) {
			string line = lines[i];
			parts = line.Split(',');
			if(parts.Length == 1) {
				//if(i > 1000) //arbitrary stop to not have it calculate for like 10 minutes
				//	break;
				//parentObj = new GameObject("" + lines[i]);
				//parentObj.transform.parent = transform;
				string[] bits = line.Split(';');
				if(bits[0].Length > 0) {
					firstOffset = i;
					timeStep = int.Parse(bits[0]) / 15;
					timeSteps += 1;
				}
				continue;
			}

			for(int j = 0; j < parts.Length; j++) {
				float value = float.Parse(parts[j]);
				if(value > 0.1f) {
					Vector3 pos1 = positions[ids[i - firstOffset - 1]];
					Vector3 pos2 = positions[ids[j]];
					//createLine(parentObj.transform, pos1, pos2, value / 3000f);

					Path path = new Path(ids[i - firstOffset - 1], ids[j], timeStep);
					values[path] = value;

					Path fakePath = new Path(ids[i - firstOffset - 1], ids[j], 0);

					if(minimumValues.ContainsKey(fakePath)) {
						if(value < minimumValues[fakePath]) {
							minimumValues[fakePath] = value;
						}
					}
					else {
						minimumValues[fakePath] = value;
					}

					if(maximumValues.ContainsKey(fakePath)) {
						if(value > maximumValues[fakePath]) {
							maximumValues[fakePath] = value;
						}
					}
					else {
						maximumValues[fakePath] = value;
					}
				}
			}
		}

		foreach(string id in ids) {
			names.Add(id);
			points[id] = positions[id];
		}

		if(slider != null) {
			slider.maxValue = timeSteps;
		}

		for(int count = 0; count < points.Count - sample; count++) {
			int index = UnityEngine.Random.Range(0, names.Count);
			names.RemoveAt(index);
		}
	}

	private LineRenderer createLine(Transform parent, Vector3 pos1, Vector3 pos2, float value) {
		Vector3 direction = pos2 - pos1;
		direction.Normalize();
		Vector3 cross = Vector3.Cross(direction, Vector3.up);
		pos1 += cross * laneWidth;
		pos2 += cross * laneWidth;

		Vector3[] positions = { pos1, pos2 };
		
		GameObject lineObj = new GameObject("Line");
		lineObj.transform.parent = parent;
		lineObj.transform.position = Vector3.zero;

		LineRenderer lineRend = lineObj.AddComponent<LineRenderer>();
		lineRend.SetPositions(positions);
		lineRend.material = lineMaterial;
		lineRend.material.color = getLineColor(value); //colors[value % colors.Length];
		lineRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRend.receiveShadows = false;
		lineRend.SetWidth(lineWidth, lineWidth);

		return lineRend;
	}

	private Vector3 realToGameCoords(double posX, double posY) {
		double newX = (((posX - realMinX) / (realMaxX - realMinX)) * -(gameMaxX - gameMinX)) - gameMinX;
		double newY = (((posY - realMinY) / (realMaxY - realMinY)) * (gameMaxY - gameMinY)) - gameMinY;
		return new Vector3((float) newX, boundaries.BuildingMaximumHeight + 1, (float) newY);
	}

	private Color getLineColor(float val) {
		val = Mathf.Clamp(val * 3, 0, 3);
		if(val <= 1f) {
			return Color.Lerp(Color.blue, Color.green, val);
		}
		else if(val <= 2f) {
			return Color.Lerp(Color.green, Color.yellow, val - 1f);
		}
		else {
			return Color.Lerp(Color.yellow, Color.red, val - 2f);
		}
	}


	public void slid(float value) {
		int timeStep = Mathf.FloorToInt(value);

		foreach(string from in names) {
			foreach(string to in names) {
				if(from == to) {
					continue;
				}

				Path path = new Path(from, to, timeStep);

				if(values.ContainsKey(path)) {
					float leValue = values[path];

					Path fakePath = new Path(from, to, 0);

					float minimum = minimumValue;
					float maximum = maximumValue;

					if(minimumValues.Count > 0) {
						minimum = minimumValues[fakePath];
					}

					if(maximumValues.Count > 0) {
						maximum = maximumValues[fakePath];
					}
					
					leValue = (leValue - minimum) / (maximum - minimum);

					if(lineRenderers.ContainsKey(fakePath)) {
						lineRenderers[fakePath].material.color = getLineColor(leValue);
					}
					else {
						Vector3 fromPoint = points[from];
						Vector3 toPoint = points[to];

						lineRenderers[fakePath] = createLine(transform, fromPoint, toPoint, leValue);
					}
				}
			}
		}
	}
}
