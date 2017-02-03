/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 *
 * Stockholm MatSIM integration.
 * 
 * Berend Wouda
 * 
 */

using UnityEngine;
using Npgsql;
using System;
using System.Collections.Generic;
using HeatmapLayer;
using System.Globalization;

public class VehicleEvents : MonoBehaviour {
	private struct VehicleInterval {
		public VehicleInterval(string id, int startTime, int endTime) {
			this.id = id;
			this.startTime = startTime;
			this.endTime = endTime;
		}

		public string id;
		public int startTime;
		public int endTime;
	}

	private struct VehiclePosition {
		public VehiclePosition(string id, Vector3 position) {
			this.id = id;
			this.position = position;
		}

		public string id;
		public Vector3 position;
	}
	
	private List<VehicleInterval> vehicleIntervals;
	private Dictionary<int, List<VehiclePosition>> vehiclePositions;
	private Dictionary<string, HeatmapController.VehicleContainer> vehicleContainers;

	private struct PersonTimes {
		public PersonTimes(string pid, int startTimeStep, int endTimeStep) {
			this.pid = pid;
			this.startTimeStep = startTimeStep;
			this.endTimeStep = endTimeStep;
		}

		public string pid;
		public int startTimeStep;
		public int endTimeStep;
	}

	private Dictionary<string, List<PersonTimes>> publicTransitPersonTimes;

	private Dictionary<string, HeatmapController.ScoreContainer> scoreContainers;
	private Dictionary<string, float> scores;

	private TimeController timeController;
	private HeatmapController heatmapController;


	public void Awake() {
		timeController = FindObjectOfType<TimeController>();
		heatmapController = FindObjectOfType<HeatmapController>();
		
		vehicleIntervals = new List<VehicleInterval>();
		vehiclePositions = new Dictionary<int, List<VehiclePosition>>();
		vehicleContainers = new Dictionary<string, HeatmapController.VehicleContainer>();

		publicTransitPersonTimes = new Dictionary<string, List<PersonTimes>>();

		scoreContainers = new Dictionary<string, HeatmapController.ScoreContainer>();
		scores = new Dictionary<string, float>();
	}

	public void Start() {
		if(!CoordinateConvertor.isInitialized) {
			CoordinateConvertor.Initialize();
		}

		try {
			NpgsqlConnection connection = new NpgsqlConnection(StockholmMatSIMParameters.Instance.ConnectionString);

			connection.Open();

			int hour = (int) StockholmMatSIMParameters.Instance.Hour;
			int startTimeStep = hour * 3600;
			int endTimeStep = (hour + 1) * 3600;

			// Can be up to 100000 rows.
			string intervalsQuery = string.Format("select veh_id, min(event_time), max(event_time) from vehicle_events where event_time >= {0} and event_time < {1} group by veh_id;", startTimeStep, endTimeStep);
			
			NpgsqlCommand intervalsCommand = new NpgsqlCommand(intervalsQuery, connection);

			NpgsqlDataReader intervalsDataReader = intervalsCommand.ExecuteReader();

			int intervalsCounter = 0;

			while(intervalsDataReader.Read()) {
				string id = intervalsDataReader.GetString(0);
				int startTime = (int) intervalsDataReader.GetFloat(1);
				int endTime = (int) intervalsDataReader.GetFloat(2);

				VehicleInterval vehicleInterval = new VehicleInterval(id, startTime, endTime);

				vehicleIntervals.Add(vehicleInterval);

				intervalsCounter++;
			}

			Debug.Log(string.Format("{0} vehicle intervals retrieved from the database.", intervalsCounter));

			if(!intervalsDataReader.IsClosed) {
				intervalsDataReader.Close();
			}

			// Can be up to 1000000 rows.
			string positionQuery = string.Format("select veh_id, event_time, longitude, latitude from vehicle_events where event_time >= {0} and event_time < {1};", startTimeStep, endTimeStep);

			NpgsqlCommand positionsCommand = new NpgsqlCommand(positionQuery, connection);

			NpgsqlDataReader positionsDataReader = positionsCommand.ExecuteReader();

			int positionsCounter = 0;

			while(positionsDataReader.Read()) {
				string id = positionsDataReader.GetString(0);
				int time = (int) positionsDataReader.GetFloat(1);
				double longitude = positionsDataReader.GetDouble(2);
				double latitude = positionsDataReader.GetDouble(3);

				Vector3 position = CoordinateConvertor.LatLonToVector3(latitude, longitude);

				if(!vehiclePositions.ContainsKey(time)) {
					vehiclePositions[time] = new List<VehiclePosition>();
				}

				VehiclePosition vehiclePosition = new VehiclePosition(id, position);
				vehiclePositions[time].Add(vehiclePosition);

				if(!vehicleContainers.ContainsKey(id)) {
					vehicleContainers[id] = new HeatmapController.VehicleContainer(id, Vector3.zero, false);
				}

				HeatmapController.VehicleContainer vehicleContainer = vehicleContainers[id];

				if(!scoreContainers.ContainsKey(id)) {
					scoreContainers[id] = new HeatmapController.ScoreContainer(0.0f);
				}
				
				HeatmapController.ScoreContainer scoreContainer = scoreContainers[id];

				heatmapController.trackVehicleScore(vehicleContainer, scoreContainer);

				positionsCounter++;
			}

			Debug.Log(string.Format("{0} vehicle positions retrieved from the database.", positionsCounter));

			if(!positionsDataReader.IsClosed) {
				positionsDataReader.Close();
			}

			// Roughly 100000 rows.
			string scoresQuery = "SELECT pid, score FROM scores ORDER BY pid;";

			NpgsqlCommand scoresCommand = new NpgsqlCommand(scoresQuery, connection);

			NpgsqlDataReader scoresDataReader = scoresCommand.ExecuteReader();

			int scoresCounter = 0;

			while(scoresDataReader.Read()) {
				// This table still has an int as person ID. This means some scores will be missing.
				string pid = scoresDataReader.GetInt32(0).ToString(CultureInfo.InvariantCulture);
				float score = scoresDataReader.GetFloat(1);

				scores[pid] = score;

				scoresCounter++;
			}

			Debug.Log(string.Format("{0} scores retrieved from the database.", scoresCounter));

			if(!scoresDataReader.IsClosed) {
				scoresDataReader.Close();
			}

			// Roughly 200000 rows.
			string personTimesQuery = "select veh_id, pid, min(event_time) as start, max(event_time) as end from events where veh_id like '%Veh%' and pid not like 'pt%' and pid not like '' group by veh_id, pid order by veh_id, pid;";

			NpgsqlCommand personTimesCommand = new NpgsqlCommand(personTimesQuery, connection);

			NpgsqlDataReader personTimesDataReader = personTimesCommand.ExecuteReader();

			int personTimesCounter = 0;

			while(personTimesDataReader.Read()) {
				string veh_id = personTimesDataReader.GetString(0);
				string pid = personTimesDataReader.GetString(1);
				float start = personTimesDataReader.GetFloat(2);
				float end = personTimesDataReader.GetFloat(3);
				
				PersonTimes personTimes = new PersonTimes(pid, (int) start, (int) end);

				if(!publicTransitPersonTimes.ContainsKey(veh_id)) {
					publicTransitPersonTimes[veh_id] = new List<PersonTimes>();
				}

				publicTransitPersonTimes[veh_id].Add(personTimes);

				personTimesCounter++;
			}

			Debug.Log(string.Format("{0} person times retrieved from the database.", personTimesCounter));

			if(!personTimesDataReader.IsClosed) {
				personTimesDataReader.Close();
			}

			connection.Close();
		}
		catch(Exception exception) {
			Debug.LogError(exception);
		}
	}

	public void Update() {
		int timeStep = 3600 * (int) StockholmMatSIMParameters.Instance.Hour + (int) timeController.gameTime;

		foreach(VehicleInterval vehicleInterval in vehicleIntervals) {
			HeatmapController.VehicleContainer vehicleContainer = vehicleContainers[vehicleInterval.id];

			if(timeStep >= vehicleInterval.startTime && timeStep <= vehicleInterval.endTime) {
				vehicleContainer.active = true;
			}
		}

		if(vehiclePositions.ContainsKey(timeStep)) {
			foreach(VehiclePosition vehiclePosition in vehiclePositions[timeStep]) {
				HeatmapController.VehicleContainer vehicleContainer = vehicleContainers[vehiclePosition.id];

				vehicleContainer.position = vehiclePosition.position;
			}
		}
		
		foreach(string id in vehicleContainers.Keys) {
			if(id.Contains("Veh") && publicTransitPersonTimes.ContainsKey(id)) {
				// Public transit. Public transit vehicles scores are the average of the contained person scores (since that is how the heatmap works).
				List<PersonTimes> vehiclePersonTimes = publicTransitPersonTimes[id];

				float score = 0.0f;
				int count = 0;

				foreach(PersonTimes personTimes in vehiclePersonTimes) {
					if(timeStep >= personTimes.startTimeStep && timeStep <= personTimes.endTimeStep) {
						// Some persons have a non-integer ID while the scores table has an integer ID, so skip these for now.
						if(scores.ContainsKey(personTimes.pid)) {
							score += scores[personTimes.pid];
							count++;
						}
					}
				}

				scoreContainers[id].score = score / count;
			}
			else if(scores.ContainsKey(id)) {
				// Personal transit. Person IDs are the same as vehicle IDs when they travel by car.
				float score = scores[id];

				scoreContainers[id].score = score;
			}
		}
	}
}
