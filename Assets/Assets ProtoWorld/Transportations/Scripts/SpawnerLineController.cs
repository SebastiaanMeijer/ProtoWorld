using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This LineController will spawn vehicles in Unity.
/// TODO: seperate this from LineController, should only handle spawning and reusing vehicles.
/// </summary>
public class SpawnerLineController : MonoBehaviour
{
    protected HashSet<GameObject> vehiclesOutOfService;

    public int numberVehiclesOutOfService;

    public bool useGlobalSpawnRate = true;

    [Range(1, 65535)]
    public float localSpawnRate = 5;

    protected float globalSpawnRate = 5;

    public bool spawnFromStart = false;

    [Range(0, 65535)]
    public float spawnFromStartDelay = 0;

    protected float startTime;

    protected LineController transline;

    protected RoutingController mainRouter;

    protected TimeController timeController;

    /// <summary>
    /// Calling base start to make sure that the vehicles-list is initiated.
    /// Initiate vehiclesOut
    /// </summary>
    public void Start()
    {
        timeController = GameObject.FindObjectOfType<TimeController>();
        mainRouter = GameObject.FindObjectOfType<RoutingController>();
        if (mainRouter == null)
            Debug.LogError("No routingController found!");
        transline = GetComponent<LineController>();
        if (transline == null)
            Debug.LogError("No lineController attached to this gameobject: " + gameObject.name);
        vehiclesOutOfService = new HashSet<GameObject>();
        if (spawnFromStart)
            startTime = (useGlobalSpawnRate) ? -globalSpawnRate + spawnFromStartDelay : -localSpawnRate + spawnFromStartDelay;
        //SpawnAndResetTimer();
    }

    void Update()
    {
        switch (transline.category)
        {
            case LineCategory.Bus:
                SetSpawnRate(mainRouter.busSpawnFrequency);
                break;
            case LineCategory.Tram:
                SetSpawnRate(mainRouter.tramSpawnFrequency);
                break;
            case LineCategory.Metro:
                SetSpawnRate(mainRouter.metroSpawnFrequency);
                break;
            case LineCategory.Train:
                SetSpawnRate(mainRouter.trainSpawnFrequency);
                break;
        }
    }

    /// <summary>
    /// Using LateUpdate due to spawn rate update from RoutingController.
    /// </summary>
    public void LateUpdate()
    {
        if (timeController == null)
            return;

        if (timeController.gameTime - startTime > ((useGlobalSpawnRate) ? globalSpawnRate : localSpawnRate))
        {
            //if (category == LineCategory.Train || category == LineCategory.Metro || category == LineCategory.Tram)
            SpawnAndResetTimer();
        }
        foreach (var vehicle in transline.vehicles)
        {
            if (vehicle.gameObject.activeInHierarchy == false)
            {
                vehiclesOutOfService.Add(vehicle);
            }
        }
        numberVehiclesOutOfService = vehiclesOutOfService.Count;
    }

    /// <summary>
    /// This will spawn 2 vehicles at each end of this transline.
    /// Will reuse deactivated vehicles if exist or it will create a new vehicle.
    /// </summary>
    protected void SpawnAndResetTimer()
    {
        if (timeController == null)
            return;
        startTime = timeController.gameTime;

        GameObject vehicle;
        if (vehiclesOutOfService.Count > 0)
        {
            vehicle = vehiclesOutOfService.First();
            vehiclesOutOfService.Remove(vehicle);
            vehicle.SetActive(true);
            vehicle.GetComponent<VehicleController>().ResetVehicle(LineDirection.OutBound);
        }
        else
        {
            vehicle = VehicleController.CreateGameObject(transline, LineDirection.OutBound);
            vehicle.transform.SetParent(transform);
            transline.AddVehicle(vehicle);
        }
        if (vehiclesOutOfService.Count > 0)
        {
            vehicle = vehiclesOutOfService.First();
            vehiclesOutOfService.Remove(vehicle);
            vehicle.SetActive(true);
            vehicle.GetComponent<VehicleController>().ResetVehicle(LineDirection.InBound);
        }
        else
        {
            vehicle = VehicleController.CreateGameObject(transline, LineDirection.InBound);
            vehicle.transform.SetParent(transform);
            transline.AddVehicle(vehicle);
        }
    }

    /// <summary>
    /// Called by RoutingController to be set with the globalSpawningRate.
    /// </summary>
    /// <param name="externalSpawnRate"></param>
    public void SetSpawnRate(float externalSpawnRate)
    {
        globalSpawnRate = externalSpawnRate;
    }
}
