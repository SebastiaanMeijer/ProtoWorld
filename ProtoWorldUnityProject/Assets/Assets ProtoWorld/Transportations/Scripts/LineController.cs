/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public enum LineDirection { InBound = -1, Undecided = 0, OutBound = 1 }
public enum LineCategory { Walk, Bus, Tram, Metro, Train }

public class LineController : MonoBehaviour
{
	public int id;

	[HideInInspector]
	public LineCategory category;

	[HideInInspector]
	public string lineName;

	public List<StationController> stations;

	[Range(0, 2000)]
	public float stationRadius = 50;

	public List<float> travelingTimes;

	public List<bool> allowTraveling;

	public bool useGlobalVehicleCapacity = true;

	public int localVehicleCapacity = 0;

	private int vehicleCapacity;

	public bool useDefaultVehicleSymbol = true;

	public HashSet<GameObject> vehicles;

	public int queuingHeadCount;

	public int travelingHeadCount;

	public bool showLineMesh = true;

	private LineRenderer lineMesh;

	public Color lineColor;

	protected RoutingController mainRouter;

	private Heatmap heatMap;

	public void Awake()
	{
		heatMap = FindObjectOfType<Heatmap>();
	}

	/// <summary>
	/// Method is called by SceneXMLReader to convert LineController to an xml-element.
	/// </summary>
	/// <param name="lc"></param>
	/// <returns></returns>
	public static BaseLine CreateBaseLine(LineController lc)
	{
		string sstr = "";
		foreach (var station in lc.stations)
		{
			sstr += station.gameObject.name + ",";
		}
		string tstr = "";
		foreach (var time in lc.travelingTimes)
		{
			tstr += time + ",";
			//tstr += time/60 + ",";
		}
		return new BaseLine
		{
			id = lc.id,
			category = lc.category.ToString(),
			name = lc.lineName,
			stationIds = sstr.Remove(sstr.Length - 1),
			travelingTimes = tstr.Remove(tstr.Length - 1),
		};
	}

	public static GameObject CreateGameObject(BaseLine line)
	{
		var lgo = new GameObject();
		lgo.tag = "TransLine";

		lgo.layer = LayerMask.NameToLayer("Stations");

		lgo.AddComponent<SpawnerLineController>();

		LineController ctrl = lgo.AddComponent<LineController>();
		ctrl.id = line.id;
		ctrl.SetLineCategory(line.GetCategory());
		ctrl.SetLineName(line.name);
		ctrl.travelingTimes = new List<float>();
		ctrl.allowTraveling = new List<bool>();
		ctrl.stations = new List<StationController>();

		var positions = new List<Vector3>();
		//Debug.Log("<" + line.stationIds + ">");
		string[] sids = line.stationIds.Split(RoutingController.DelimiterChars);
		foreach (var id in sids)
		{
			var go = GameObject.Find(id);
			positions.Add(go.transform.position);
			ctrl.stations.Add(go.GetComponent<StationController>());
		}

		string[] times = line.travelingTimes.Split(RoutingController.DelimiterChars);
		ctrl.travelingTimes = new List<float>();
		// time is in [min], need to convert to [s]
		float time;
		foreach (var t in times)
		{
			if (float.TryParse(t, out time))
			{
				//ctrl.travelingTimes.Add(time * 60);
				ctrl.travelingTimes.Add(time);
				ctrl.allowTraveling.Add(true);
			}
		}
		if (ctrl.stations.Count != ctrl.travelingTimes.Count + 1)
		{
			Debug.LogError("idList count != timelist.count + 1");
		}

		var renderer = lgo.AddComponent<LineRenderer>();
		//renderer.material = new Material(Shader.Find("GUI/Text Shader"));
		renderer.material = new Material(Shader.Find("Particles/Additive (Soft)"));
		renderer.SetColors(ctrl.lineColor, ctrl.lineColor);
		renderer.SetWidth(15, 15);
		renderer.SetVertexCount(sids.Length);
		renderer.SetPositions(positions.ToArray());
		renderer.enabled = false;

		return lgo;
	}

	public void SetLineName(string name)
	{
		lineName = name;
		UpdateGameObjectName();
	}

	public void SetLineCategory(LineCategory category)
	{
		this.category = category;
		var spawner = GetComponent<SpawnerLineController>();
		if (spawner == null)
			Debug.LogError("No spawner found!");
		switch (category)
		{
		case LineCategory.Bus:
			lineColor = Color.red;
			spawner.enabled = false;
			break;
		case LineCategory.Tram:
			lineColor = Color.blue;
			spawner.enabled = true;
			break;
		case LineCategory.Metro:
			lineColor = Color.cyan;
			spawner.enabled = true;
			break;
		case LineCategory.Train:
			lineColor = Color.magenta;
			spawner.enabled = true;
			break;
		default:
			lineColor = Color.white;
			spawner.enabled = false;
			break;
		}
		UpdateGameObjectName();
	}

	// Use this for initialization
	public virtual void Start()
	{
		mainRouter = GameObject.FindObjectOfType<RoutingController>();
		if (mainRouter == null)
			Debug.LogError("No routingController found!");
		vehicles = new HashSet<GameObject>();
		lineMesh = GetComponent<LineRenderer>();
	}

	public void Update()
	{
		if (useGlobalVehicleCapacity)
		{
			switch (category)
			{
			case LineCategory.Bus:
				vehicleCapacity = mainRouter.busCapacity;
				break;
			case LineCategory.Tram:
				vehicleCapacity = mainRouter.tramCapacity;
				break;
			case LineCategory.Metro:
				vehicleCapacity = mainRouter.metroCapacity;
				break;
			case LineCategory.Train:
				vehicleCapacity = mainRouter.trainCapacity;
				break;
			}
		}
		else
		{
			vehicleCapacity = localVehicleCapacity;
		}
		if (showLineMesh && lineMesh != null)
			UpdateLineMesh();
	}

	public int GetVehicleCapacity()
	{
		return vehicleCapacity;
	}

	/// <summary>
	/// Using LateUpdate due to spawn rate update from RoutingController.
	/// Important for the derived-class SpawnerLineController.
	/// </summary>
	public void LateUpdate()
	{
		queuingHeadCount = GetQueuingHeadCount();
		travelingHeadCount = GetTravelingHeadCount();
	}

	protected void UpdateLineMesh()
	{
		var positions = new Vector3[stations.Count];
		for (int i = 0; i < stations.Count; i++)
		{
			positions[i] = (stations[i].transform.position);
		}
		lineMesh.SetVertexCount(positions.Length);
		lineMesh.SetPositions(positions);
	}

	/// <summary>
	/// To keep track of the vehicles that this line has.
	/// </summary>
	/// <param name="vehicle"></param>
	public void AddVehicle(GameObject vehicle)
	{
        vehicle.tag = "TransVehicle";
        vehicles.Add(vehicle);
	}


	/// <summary>
	/// Based on the id of the station, return the before and after stations.
	/// Only return stations with travel time > 0 from this station.
	/// If the station is at the side, only one will return.
	/// </summary>
	/// <param name="stationId"></param>
	/// <returns>A list of station ids</returns>
	public List<StationController> GetAdjacentStations(StationController station)
	{
		List<StationController> adjacents = new List<StationController>();
		//int index = stations.FindIndex(x => x.Equals(station));
		int index = GetIndex(station);

		if (index > 0 && index < stations.Count)
		{
			StationController previous = stations[index - 1];
			if (allowTraveling[index - 1])
				adjacents.Add(previous);
		}
		if (index >= 0 && index < stations.Count - 1)
		{
			StationController next = stations[index + 1];
			try
			{
				if (allowTraveling[index])
					adjacents.Add(next);
			}
			catch (Exception)
			{
				Debug.LogError(station);
				Debug.LogError(index);
			}
		}
		return adjacents;
	}

	int GetIndex(StationController station)
	{
		return stations.FindIndex(x => x.Equals(station));
	}

	internal List<StationController> GetRemainingStations(StationController station, LineDirection direction)
	{
		int index = GetIndex(station);
		var remaining = stations.GetRange(index, stations.Count - index);

		return remaining;
	}

	/// <summary>
	/// Check if two stations are adjacent to each other.
	/// </summary>
	/// <param name="s1"></param>
	/// <param name="s2"></param>
	/// <returns></returns>
	public bool IsAdjacent(StationController s1, StationController s2)
	{
		int index1 = GetIndex(s1);
		if (index1 < 0)
			return false;
		int index2 = GetIndex(s2);
		if (index2 < 0)
			return false;
		if (Math.Abs(index2 - index1) == 1)
			return true;
		else
			return false;
	}

	/// <summary>
	/// Get the index of the leg between two adjacent stations.
	/// Return -1 if the stations are not adjacent to each other.
	/// </summary>
	/// <param name="stationId1"></param>
	/// <param name="stationId2"></param>
	/// <returns></returns>
	public int GetStageIndex(StationController s1, StationController s2)
	{
		if (IsAdjacent(s1, s2))
		{
			int index1 = GetIndex(s1);
			int index2 = GetIndex(s2);
			return Math.Min(index1, index2);
		}
		else
		{
			return -1;
		}
	}

	public float GetLegTravelTime(StationController s1, StationController s2)
	{
		int stageIndex = GetStageIndex(s1, s2);
		if (stageIndex >= 0)
		{
			if (allowTraveling[stageIndex])
				return travelingTimes[stageIndex];
			else
				return 0;
		}
		else
		{
			return -1;
		}
	}

	public StationController GetPreviousStop(StationController station, LineDirection direction)
	{
		int prevIndex = GetIndex(station) - (int)direction;
		if (prevIndex >= 0 && prevIndex < stations.Count)
			return stations[prevIndex];
		else
			return null;
	}

	public StationController GetNextStop(StationController station, LineDirection direction)
	{
		int nextIndex = GetIndex(station) + (int)direction;
		if (nextIndex >= 0 && nextIndex < stations.Count)
			return stations[nextIndex];
		else
			return null;
	}

	public StationController GetFirstStop(LineDirection direction)
	{
		if (direction == LineDirection.OutBound)
			return stations.First();
		else
			return stations.Last();
	}

	public StationController GetLastStop(LineDirection direction)
	{
		if (direction == LineDirection.OutBound)
			return stations.Last();
		else
			return stations.First();
	}

	public LineDirection GetDirection(StationController first, StationController second)
	{
		StationController next = GetNextStop(first, LineDirection.OutBound);
		if (next != null && next.Equals(second))
			return LineDirection.OutBound;
		next = GetNextStop(first, LineDirection.InBound);
		if (next != null && next.Equals(second))
			return LineDirection.InBound;
		return LineDirection.Undecided;
	}

	protected virtual int GetQueuingHeadCount()
	{
		int headCount = 0;
		foreach (var station in stations)
		{
			headCount += station.GetHeadCount();
		}
		return headCount;
	}

	protected virtual int GetTravelingHeadCount()
	{
		int headCount = 0;
		foreach (var vehicle in vehicles)
		{
			var controller = vehicle.GetComponent<VehicleController>();
			// Provide this from our cached version to improve performance, as this is called a lot during spawning events.
			controller.heatMap = heatMap;
			if (vehicle.activeInHierarchy && controller != null)
				headCount += controller.headCount;
		}
		return headCount;
	}

	/// <summary>
	/// Output containing id, name and station ids of this line.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		string str = "";
		foreach (var station in stations)
		{
			str += station.gameObject.name + ",";
		}
		return String.Format("LineId:{0}, Name:{1}, [{2}]",
			id, gameObject.name, str.Remove(str.Length - 1));
	}

	public void UpdateGameObjectName()
	{
		gameObject.name = category + "_" + lineName;
	}

	public static string MakeKeyString(int lineId, LineDirection direction)
	{
		string str;
		switch (direction)
		{
		case LineDirection.InBound:
			str = "-";
			break;
		case LineDirection.OutBound:
			str = "+";
			break;
		default:
			str = "?";
			break;
		}
		return str + lineId.ToString();
	}

	/// <summary>
	/// Get the first and last stations of this line.
	/// </summary>
	/// <returns></returns>
	public List<StationController> GetEndStations()
	{
		return new List<StationController>() { stations.First(), stations.Last() };
	}

	/// <summary>
	/// Check stations after currentStation for one that is in service.
	/// return null if there is none.
	/// </summary>
	/// <param name="currentStation"></param>
	/// <param name="direction"></param>
	/// <returns></returns>
	public StationController GetNextAvailableStation(StationController currentStation, LineDirection direction)
	{
		var nextStation = GetNextStop(currentStation, direction);
		while (nextStation != null && nextStation.outOfService)
		{
			nextStation = GetNextStop(nextStation, direction);
		}
		return nextStation;
	}

	/// <summary>
	/// Draw the link between stations with Gizmo
	/// </summary>
	/// <param name="radius">How thick is the link.</param>
	/// <param name="linkHeight">How much should it rise above the ground.</param>
	/// <param name="alpha">Use value 1.0 for highlighted and 0.5 for other.</param>
	public void DrawGizmoLine(float radius, float linkHeight, float alpha)
	{
		if (stations != null && stations.Count > 0)
		{
			if (lineColor == null)
				lineColor = Color.white;
			lineColor.a = alpha;
			for (int i = 1; i < stations.Count; i++)
			{
				GizmoDraw.TransLine(stations[i - 1].transform.position, stations[i].transform.position, lineColor, radius, linkHeight);
			}
		}
	}



}
