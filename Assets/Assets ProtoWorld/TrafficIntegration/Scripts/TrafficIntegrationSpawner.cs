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
using System.Diagnostics;
using System.Linq;

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

    public GameObject[] graphicCar;
    public GameObject graphicBus;
    public GameObject graphicAltBus;

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

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
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
            }
            else
            {
                //Create a new vehicle in the scene
                Vector3 vehPosition = new Vector3(v3.x, 0.0f, v3.z);

                GameObject vehObject;
                if (v.type.ToUpperInvariant().Contains("BUS"))
                {
                    if (v.id.Contains("dz_repl")) // for Driebergen-Zeist case
                        vehObject = (GameObject)Instantiate(graphicAltBus, vehPosition, Quaternion.identity);
                    else
                        vehObject = (GameObject)Instantiate(graphicBus, vehPosition, Quaternion.identity);

                    if (routingContr != null)
                        routingContr.HandleBusObjectsFromSumo(vehObject, v.id);
                }
                else
                {
                    vehObject = (GameObject)Instantiate(graphicCar[Random.Range(0, graphicCar.Length)], vehPosition, Quaternion.identity);
                }

                vehObject.transform.parent = this.transform;
                vehObject.name = v.id;
                TrafficIntegrationVehicle vc = vehObject.GetComponent<TrafficIntegrationVehicle>();
                vc.smooth = smoothPaths;
                vc.brakingActive = brakingActive && (trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration);
                vc.timeToBrakeInSeconds = timeToBrakeInSeconds;
                vc.driversPatienceInSeconds = driversPatienceInSeconds;
                vc.angleOfView = angleOfView;
                vehControllers.Add(v.id, vc);
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