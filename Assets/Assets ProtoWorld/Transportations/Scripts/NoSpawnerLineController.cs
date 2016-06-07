using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controller to accomodate Sumo vehicles that needs to carry traveler in Unity-scene.
/// </summary>
public class NoSpawnerLineController : LineController
{
    //List<GameObject> toBeRemoved = new List<GameObject>();

    /// <summary>
    /// Assuming that the vechicle only has 2 stops.
    /// SumoTrafficSpawner will set gameobject to false once it reached its destination,
    /// destination is the end-station so ArrivedAtNextStation() should be called.
    /// </summary>
    //public override void LateUpdate()
    //{
        
    //    foreach (var child in vehicles)
    //    {
    //        if (child.gameObject.activeInHierarchy == false)
    //        {
    //            child.gameObject.SetActive(true);
    //            child.GetComponent<VehicleController>().ArrivedAtNextStation();
    //            child.gameObject.SetActive(false);
    //            toBeRemoved.Add(child);
    //        }
    //    }
    //    foreach (var go in toBeRemoved)
    //    {
    //        vehicles.Remove(go);
    //    }
    //    toBeRemoved.Clear();
    //}
}
