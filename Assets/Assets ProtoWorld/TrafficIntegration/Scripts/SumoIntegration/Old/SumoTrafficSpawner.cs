/*
 * 
 * SUMO COMMUNICATION
 * SumoTrafficSpawner.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Generates the graphic vehicles in the Unity scene from the data of Sumo Traffic DB.
/// </summary>
/// <remarks>
/// This script reads the <see cref="SumoTrafficDB"/> and updates the scene for each 
/// timestep of the SUMO simulation. The speed at which the Unity scene is updated is 
/// directly affected by <see cref="SumoMainController.delay"/>.
/// </remarks> 
public class SumoTrafficSpawner : MonoBehaviour
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

    private bool brakingActive = true;
    private int timeToBrakeInSeconds = 1;
    private float driversPatienceInSeconds = 3.0f;
    private float angleOfView = 90.0f;
    private SumoTrafficDB tdb;
    private Dictionary<string, SumoVehicleController> vehControllers;
    public int timeStepIndex;
    private int simulationTime;
    private CameraFrustumScript cameraFrustum;
    private SumoMainController mc;
    private SumoConfig conf;

    private float period = 1.0f;
    private float nextActionTime = 0.0f;
    private float nextUpdate = 0.0f;

    private RoutingController routingController;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        conf = this.GetComponentInParent<SumoConfig>();
        UseFrustumForUpdate = conf.useFrustumForUpdate;
        UseCoordinateConversion = conf.useCoordinateConversion;
        brakingActive = conf.vehicleBrakingActive;
        timeToBrakeInSeconds = conf.timeToBrakeInSeconds;
        driversPatienceInSeconds = conf.driversPatientInSeconds;
        angleOfView = conf.driversAngleOfView;
    }

    /// <summary>
    /// Initializes the fields when the script starts. 
    /// </summary>
    void Start()
    {
        if (conf.integrateSumo)
        {
            mc = FindObjectOfType<SumoMainController>();
            tdb = (SumoTrafficDB)this.GetComponent("SumoTrafficDB");
            vehControllers = new Dictionary<string, SumoVehicleController>();
            timeStepIndex = 0;
            cameraFrustum = Camera.main.GetComponent<CameraFrustumScript>();
            routingController = FindObjectOfType<RoutingController>();
        }
    }

    /// <summary>
    /// Updates the Unity scene if there are timesteps to be read in the DB. 
    /// </summary>
    /// <seealso cref="UpdateVehiclesInScene"/>
    /// <seealso cref="CleanScene"/>
    void Update()
    {
        if (conf.integrateSumo)
        {
            if (nextUpdate < Time.time)
            { 
                //Update the scene for each TimeStepTDB in SumoTrafficDB
                if (timeStepIndex < tdb.getNumberOfTimeSteps() - 1)
                {
                    UpdateVehiclesInScene();
                    CleanScene();
                    timeStepIndex++;
                    nextUpdate += mc.timeStepVelocityInMs / 1000f;
                }
            }
        }
    }

    /// <summary>
    /// Auxiliar private method. Creates and updates the graphic vehicles in the Unity scene 
    /// for the current timestep of the simulation. 
    /// </summary>
    private void UpdateVehiclesInScene()
    {
        int currentNumberOfVehicles = tdb.GetNumberOfVehiclesInTimeStep(timeStepIndex);

        // Log the number of vehicles once per second
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            log.Info("Current number of vehicles in SUMO: " + currentNumberOfVehicles);
        }

        //Check for each vehicle in the SumoTrafficDB at the current timestep
        for (int i = 0; i < currentNumberOfVehicles; i++)
        {
            VehicleTDB v = tdb.GetVehicleAt(timeStepIndex, i);

            Vector3 v3;
            if (UseCoordinateConversion)
                v3 = CoordinateConvertor.LatLonToVector3(v.latitude, v.longitude);
            else
                v3 = new Vector3(v.latitude, 0.0f, v.longitude);

            SumoVehicleController vController;

            if (vehControllers.TryGetValue(v.id, out vController))
            {
                vController.isUpdated = true;
                vController.UpdateVehiclePosition(v3.x, v3.z, v.angle);
                var meshRenderer = vController.gameObject.GetComponent<MeshRenderer>();
                var lod = vController.gameObject.GetComponent<LODGroup>();

                //Render an existing vehicle if it is inside the frustrum
                if (!UseFrustumForUpdate || UnityEngine.GeometryUtility.TestPlanesAABB(cameraFrustum.Frustum, new Bounds(v3, 2 * Vector3.one)))
                {
                    if (meshRenderer != null)
                        meshRenderer.enabled = true;
                    //vController.gameObject.SetActive(true);
                    if (lod != null)
                        lod.enabled = true;
                }
                else
                {
                    if (meshRenderer != null)
                        meshRenderer.enabled = false;
                    //vController.gameObject.SetActive(false);
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
                    vehObject = (GameObject)Instantiate(graphicBus, vehPosition, Quaternion.identity);
                    if (routingController != null)
                        routingController.HandleBusObjectsFromSumo(vehObject, v.id);
                }
                else
                {
                    vehObject = (GameObject)Instantiate(graphicCar[Random.Range(0, graphicCar.Length)], vehPosition, Quaternion.identity);
                }
                vehObject.transform.parent = this.transform;
                vehObject.name = v.id;
                SumoVehicleController vc = vehObject.GetComponent<SumoVehicleController>();
                vc.smooth = smoothPaths;
                vc.brakingActive = brakingActive;
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
        foreach (SumoVehicleController v in vehControllers.Values)
        {
            if (!v.isUpdated)
            {
                v.gameObject.SetActive(false);
            }

            v.isUpdated = false;
        }
    }

    /// <summary>
    /// [OBSOLETE] 
    /// Check for synchronization between SUMO simulator and the Unity scene.
    /// </summary>
    /// <param name="maxTimestepsDelay">Max number of timesteps that the visual 
    /// scene can be behind the SUMO simulation.</param>
    [System.Obsolete]
    private void CheckSynchronization(int maxTimestepsDelay)
    {
        List<int> syncValues = new List<int>();

        foreach (SumoVehicleController v in vehControllers.Values)
        {
            syncValues.Add(v.GetNoPathsToReachLastPoint());
        }

        if (syncValues.Average() > maxTimestepsDelay)
        {
            mc.PauseSimulation();
        }
        else
        {
            mc.RunSimulation();
            timeStepIndex++;
        }
    }
}