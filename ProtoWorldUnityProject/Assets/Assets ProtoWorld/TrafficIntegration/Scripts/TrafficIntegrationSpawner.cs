/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* 
* TRAFFIC INTEGRATION
* TrafficIntegrationSpawner.cs
* Miguel Ramos Carretero
* Johnson Ho
* 
*/

using UnityEngine;
using System.Collections.Generic;
using Npgsql;

/// <summary>
/// Generates the graphic vehicles in the Unity scene from the data of Traffic Integration Data.
/// </summary>
/// <remarks>
/// This script reads the <see cref="TrafficIntegrationData"/> and updates the scene for each 
/// timestep of the simulation. The speed at which the vehicles are updated is 
/// directly affected by <see cref="TrafficIntegrationController.timeStepVelocityInMs"/>.
/// </remarks> 
public class TrafficIntegrationSpawner : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    [HideInInspector]
    public bool UseFrustumForUpdate = true;

    [HideInInspector]
    public bool UseCoordinateConversion = true;

    [HideInInspector]
    public bool smoothPaths = false;

    public bool visualizeTrafficHeatmap = false;

    public GameObject[] graphicCar;
    public GameObject graphicBus;
    public GameObject graphicAltBus;
	public GameObject ferryModel;
	public GameObject tramModel;
	public GameObject trainModel;
	public GameObject pendeltagModel;

	private bool brakingActive = true;
    private int timeToBrakeInSeconds = 1;
    private float driversPatienceInSeconds = 3.0f;
    private float angleOfView = 90.0f;

    private TrafficIntegrationData trafficData;

    private Dictionary<string, TrafficIntegrationVehicle> vehControllers;
    public int timeStepIndex;
    private int lastTimeStepExecuted;
    private int simulationTime;
    private CameraFrustumScript cameraFrustum;

    private TrafficIntegrationController trafficContr;
    private bool simulationIntegrated = false;

    private float period = 1.0f;
    private float nextActionTime = 0.0f;
    private float nextUpdate = 0.0f;

    private RoutingController routingContr;

    private TimeController timeContr;

    private HeatmapLayer.HeatmapController heatmapCtrl;

	private struct PersonTimes {
		public PersonTimes(int pid, int startTimeStep, int endTimeStep) {
			this.pid = pid;
			this.startTimeStep = startTimeStep;
			this.endTimeStep = endTimeStep;
		}

		public int pid;
		public int startTimeStep;
		public int endTimeStep;
	}

	private Dictionary<string, List<PersonTimes>> publicTransitPersonTimes;
	private Dictionary<string, HeatmapLayer.HeatmapController.ScoreContainer> scoreContainers;
	private Dictionary<int, float> scores;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        heatmapCtrl = FindObjectOfType<HeatmapLayer.HeatmapController>();

        trafficContr = FindObjectOfType<TrafficIntegrationController>();
        timeContr = FindObjectOfType<TimeController>();

        if (trafficContr != null)
        {
            simulationIntegrated = (trafficContr.typeOfIntegration != TrafficIntegrationController.TypeOfIntegration.NoTrafficIntegration);
            UseFrustumForUpdate = trafficContr.useFrustumForUpdate;
            UseCoordinateConversion = trafficContr.useCoordinateConversion;
            brakingActive = trafficContr.vehicleBrakingActive;
            timeToBrakeInSeconds = trafficContr.timeToBrakeInSeconds;
            driversPatienceInSeconds = trafficContr.driversPatientInSeconds;
            angleOfView = trafficContr.driversAngleOfView;
            visualizeTrafficHeatmap = trafficContr.visualizeTrafficInHeatmap;
        }
		
		if(trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.MatsimDatabase) {
			try
			{
				// HACK: For the Stockholm case. This is not meant to be universal code.
				NpgsqlConnection connection = new NpgsqlConnection(StockholmMatSIMParameters.Instance.ConnectionString);

				connection.Open();
				
				scores = new Dictionary<int, float>();

				// Roughly 100000 rows.
				string scoresQuery = "SELECT pid, score FROM scores ORDER BY pid;";

				NpgsqlCommand scoresCommand = new NpgsqlCommand(scoresQuery, connection);

				NpgsqlDataReader scoresDataReader = scoresCommand.ExecuteReader();
				
				int scoresCounter = 0;

				while (scoresDataReader.Read())
				{
					int pid = scoresDataReader.GetInt32(0);
					float score = scoresDataReader.GetFloat(1);

					scores[pid] = score;

					scoresCounter++;
				}

				Debug.Log(string.Format("{0} scores retrieved from the database.", scoresCounter));

				if (!scoresDataReader.IsClosed)
				{
					scoresDataReader.Close();
				}

				publicTransitPersonTimes = new Dictionary<string, List<PersonTimes>>();
				scoreContainers = new Dictionary<string, HeatmapLayer.HeatmapController.ScoreContainer>();

				// Roughly 200000 rows.
				string personTimesQuery = "select veh_id, pid, min(event_time) as start, max(event_time) as end from events where veh_id like 'Veh%' and pid not like 'pt%' and pid not like '' group by veh_id, pid order by veh_id, pid;";

				NpgsqlCommand personTimesCommand = new NpgsqlCommand(personTimesQuery, connection);

				NpgsqlDataReader personTimesDataReader = personTimesCommand.ExecuteReader();
				
				int personTimesCounter = 0;

				while (personTimesDataReader.Read())
				{
					string veh_id = personTimesDataReader.GetString(0);
					string pid = personTimesDataReader.GetString(1);
					float start = personTimesDataReader.GetFloat(2);
					float end = personTimesDataReader.GetFloat(3);

					// All person IDs should now be just integers in string form, so no guards.
					PersonTimes personTimes = new PersonTimes(int.Parse(pid), (int) start, (int) end);
					
					if(!publicTransitPersonTimes.ContainsKey(veh_id)) {
						publicTransitPersonTimes[veh_id] = new List<PersonTimes>();
					}

					publicTransitPersonTimes[veh_id].Add(personTimes);

					personTimesCounter++;
				}

				Debug.Log(string.Format("{0} person times retrieved from the database.", personTimesCounter));

				if (!personTimesDataReader.IsClosed)
				{
					personTimesDataReader.Close();
				}

				connection.Close();
			}
			catch (System.Exception ex)
			{
				Debug.LogError(ex);
			}
		}
    }

    /// <summary>
    /// Initializes the fields when the script starts. 
    /// </summary>
    void Start()
    {
        if (simulationIntegrated)
        {
            trafficData = (TrafficIntegrationData)this.GetComponent("TrafficIntegrationData");
            vehControllers = new Dictionary<string, TrafficIntegrationVehicle>();
            timeStepIndex = 0;
            lastTimeStepExecuted = -1;
            cameraFrustum = Camera.main.GetComponent<CameraFrustumScript>();
            routingContr = FindObjectOfType<RoutingController>();
        }
    }

    /// <summary>
    /// Updates the Unity scene if there are timesteps to be read in the Traffic Integration Data. 
    /// </summary>
    /// <seealso cref="UpdateVehiclesInScene"/>
    /// <seealso cref="CleanScene"/>
    void Update()
    {
        if (simulationIntegrated && !trafficContr.simulationPaused)
        {
            if (trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration)
            {
                //if using live integration from SUMO, update as before:

                if (nextUpdate < timeContr.gameTime)
                {
                    if (timeStepIndex < trafficData.getNumberOfTimeSteps() - 1)
                    {
                        UpdateVehiclesInScene();
                        CleanScene();
                        timeStepIndex++;
                        nextUpdate += trafficContr.timeStepVelocityInMs / 1000f;
                    }
                }
            }
            else
            {
                //else, update the traffic data allowing playback:

                timeStepIndex = (int)timeContr.gameTime;

                if ((nextUpdate > trafficContr.timeStepVelocityInMs || timeContr.IsSliderMoving())
                    && timeStepIndex < trafficData.getNumberOfTimeSteps() - 1)
                {
                    UpdateVehiclesInScene();
                    CleanScene();
                    nextUpdate = 0.0f;
                }

                nextUpdate += Time.deltaTime * 1000;
            }
        }
    }

    /// <summary>
    /// Auxiliar private method. Creates and updates the graphic vehicles in the Unity scene 
    /// for the current timestep of the simulation. 
    /// </summary>
    private void UpdateVehiclesInScene()
    {
        int currentNumberOfVehicles = trafficData.GetNumberOfVehiclesInTimeStep(timeStepIndex);

        // Log the number of vehicles once per second (independently of game time)
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            log.Info("Current number of vehicles in the simulation: " + currentNumberOfVehicles);
        }

        //Check for each vehicle in the Traffic Integration Data at the current timestep
        for (int i = 0; i < currentNumberOfVehicles; i++)
        {
            VehicleTDB v = trafficData.GetVehicleAt(timeStepIndex, i);

            Vector3 v3;
            if (UseCoordinateConversion)
                v3 = CoordinateConvertor.LatLonToVector3(v.latitude, v.longitude);
            else
                v3 = new Vector3(v.latitude, 0.0f, v.longitude);

            TrafficIntegrationVehicle vController;

            if (vehControllers.TryGetValue(v.id, out vController))
            {
                if (!vController.gameObject.activeSelf)
                    vController.gameObject.SetActive(true);

                vController.isUpdated = true;

                var meshRenderer = vController.gameObject.GetComponent<MeshRenderer>();
                var lod = vController.gameObject.GetComponent<LODGroup>();

                //Render an existing vehicle if it is inside the frustrum
                if (!UseFrustumForUpdate || UnityEngine.GeometryUtility.TestPlanesAABB(cameraFrustum.Frustum, new Bounds(v3, 2 * Vector3.one)) || v.type.ToUpperInvariant().Contains("BUS"))
                {
                    vController.UpdateVehiclePosition(v3.x, v3.z, v.angle);

                    if (meshRenderer != null)
                        meshRenderer.enabled = true;
                    if (lod != null)
                        lod.enabled = true;
                }
                else
                {
                    if (meshRenderer != null)
                        meshRenderer.enabled = false;
                    if (lod != null)
                        lod.enabled = false;
                }
				
				// HACK: For the Stockholm case.
				if (visualizeTrafficHeatmap && heatmapCtrl != null) {
					if(v.id.StartsWith("Veh")) {
						// Public transit. Public transit vehicles scores are the sum of the contained person scores (since that is how the heatmap works).
						List<PersonTimes> vehiclePersonTimes = publicTransitPersonTimes[v.id];

						float score = 0.0f;

						foreach(PersonTimes personTimes in vehiclePersonTimes) {
							if(timeStepIndex >= personTimes.startTimeStep && timeStepIndex <= personTimes.endTimeStep) {
								score += scores[personTimes.pid];
							}
						}

						scoreContainers[v.id].score = score;
					}
				}
            }
            else
            {
                //Create a new vehicle in the scene
                Vector3 vehPosition = new Vector3(v3.x, 0.0f, v3.z);

                GameObject vehObject;

                var vehType = v.type.ToUpperInvariant();

				if (vehType.Contains("BUS") && graphicBus != null)
                {
                    if (v.id.Contains("dz_repl")) // for Driebergen-Zeist case
                        vehObject = (GameObject)Instantiate(graphicAltBus, vehPosition, Quaternion.identity);
                    else
                        vehObject = (GameObject)Instantiate(graphicBus, vehPosition, Quaternion.identity);

                    if (routingContr != null)
                        routingContr.HandleBusObjectsFromSumo(vehObject, v.id);
                }
				else if (vehType.Contains("FERRY") && ferryModel != null)
					vehObject = (GameObject)Instantiate(ferryModel, vehPosition, Quaternion.identity);
				else if (vehType.Contains("TRAM") && tramModel != null)
					vehObject = (GameObject)Instantiate(tramModel, vehPosition, Quaternion.identity);
				else if (vehType.Contains("TRAIN") && trainModel != null)
					vehObject = (GameObject)Instantiate(trainModel, vehPosition, Quaternion.identity);
				else if (vehType.Contains("PENDELTAG") && pendeltagModel != null)
					vehObject = (GameObject)Instantiate(pendeltagModel, vehPosition, Quaternion.identity);
				else
                    vehObject = (GameObject)Instantiate(graphicCar[Random.Range(0, graphicCar.Length)], vehPosition, Quaternion.identity);

                vehObject.transform.parent = this.transform;
                vehObject.name = v.id;
                TrafficIntegrationVehicle vc = vehObject.GetComponent<TrafficIntegrationVehicle>();
                vc.smooth = smoothPaths;
                vc.brakingActive = brakingActive && (trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration);
                vc.timeToBrakeInSeconds = timeToBrakeInSeconds;
                vc.driversPatienceInSeconds = driversPatienceInSeconds;
                vc.angleOfView = angleOfView;
                vehControllers.Add(v.id, vc);

                // Track the vehicle in the heatmap
                if (visualizeTrafficHeatmap && heatmapCtrl != null) {
					//heatmapCtrl.TrackNewElement(vehObject);

					// HACK: For the Stockholm case.
					if(v.id.StartsWith("Veh")) {
						// Public transit. Public transit vehicles start out without a score.
						HeatmapLayer.HeatmapController.ScoreContainer scoreContainer = new HeatmapLayer.HeatmapController.ScoreContainer(0.0f);

						scoreContainers[v.id] = scoreContainer;

						//heatmapCtrl.TrackNewElement(vehObject, scoreContainer);
					}
					else {
						// Personal transit. Person IDs are the same as vehicle IDs when they travel by car.
						int pid;
					
						if(int.TryParse(v.id, out pid)) {
							float score = scores[pid];

							HeatmapLayer.HeatmapController.ScoreContainer scoreContainer = new HeatmapLayer.HeatmapController.ScoreContainer(score);

							//heatmapCtrl.TrackNewElement(vehObject, scoreContainer);
						}
					}
				}
            }
        }
    }

    /// <summary>
    /// Removes the vehicles that have finished their trip from the scene.
    /// </summary>
    private void CleanScene()
    {
        foreach (TrafficIntegrationVehicle v in vehControllers.Values)
        {
            if (!v.isUpdated)
            {
                v.gameObject.SetActive(false);
            }

            v.isUpdated = false;
        }
    }
}