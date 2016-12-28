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

    public GameObject modelToSpawn;

    /// <summary>
    /// Calling base start to make sure that the vehicles-list is initiated.
    /// Initiate vehiclesOut
    /// </summary>
    public void Start()
    {
        timeController = GameObject.FindObjectOfType<TimeController>();
		if(timeController == null)
			Debug.LogError("No timeController found!");
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

    void SpawnVehicleFromLog(GameObject logVehicle)
    {
        GameObject vehicle = VehicleController.CreateGameObject(transline, LineDirection.OutBound);
        // Provide this from our cached version to improve performance, as this is called a lot during spawning events.
        vehicle.GetComponent<VehicleController>().timeController = timeController;



        transline.AddVehicle(vehicle);
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
			// Provide this from our cached version to improve performance, as this is called a lot during spawning events.
			vehicle.GetComponent<VehicleController>().timeController = timeController;
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
			// Provide this from our cached version to improve performance, as this is called a lot during spawning events.
			vehicle.GetComponent<VehicleController>().timeController = timeController;
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
