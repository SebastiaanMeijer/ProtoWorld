/*
 * 
 * TRAFFIC INTEGRATION
 * TrafficIntegrationData.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;

/// <summary>
/// Class that defines a vehicle in the Traffic Integration Data. 
/// A vehicle is defined by its id and its position (latitude, longitude).
/// </summary>
/// <seealso cref="TrafficIntegrationData"/>
public class VehicleTDB
{
    public string id { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
    public string type { get; set; }
    public float angle { get; set; }

    /// <summary>
    /// Constructor of the class.
    /// </summary>
    /// <param name="id">Id of the vehicle.</param>
    /// <param name="lan">Latitude position.</param>
    /// <param name="lon">Longitude position.</param>
    /// <param name="type">Type of the vehicle.</param>
    /// <param name="angle">Angle of the vehicle</param>
    public VehicleTDB(string id, string lat, string lon, string type, string angle)
    {
        this.id = id;
        this.latitude = float.Parse(lat);
        this.longitude = float.Parse(lon);
        this.type = type;
        this.angle = float.Parse(angle);
    }

    /// <summary>
    /// Prints the information of the vehicle. 
    /// </summary>
    /// <returns>String with the information.</returns>
    public override string ToString()
    {
        return ("Id: " + this.id + "\n" +
            "Latitude: " + this.latitude + "\n" +
            "Longitude: " + this.longitude + "\n" +
            "Type: " + this.type + "\n" +
            "Angle: " + this.angle + "\n");
    }
}

/// <summary>
/// Class that defines a timestep in the Traffic Integration Data.
/// A timestep is defined by a time and the list of vehicles of that time. 
/// An additional field stores the index of the timestep in the general Traffic Integration Data.
/// </summary>
/// <seealso cref="TrafficIntegrationData"/>
public class TimeStepTDB
{
    public float time;
    public int index;
    public List<VehicleTDB> vehicles;

    /// <summary>
    /// Constructor of the class.
    /// </summary>
    /// <param name="time">Time of the simulation.</param>
    /// <param name="index">Index position in the Traffic Integration Data.</param>
    public TimeStepTDB(float time, int index)
    {
        this.time = time;
        this.index = index;
        vehicles = new List<VehicleTDB>();
    }

    /// <summary>
    /// Adds a vehicle to this timestep.
    /// </summary>
    /// <param name="v">VehicleTDB object to add.</param>
    /// <seealso cref="VehicleTDB"/>
    public void AddVehicle(VehicleTDB v)
    {
        vehicles.Add(v);
    }
}

/// <summary>
/// Implements the traffic data generated from the traffic simulation. 
/// The traffic data is composed of a list of <see cref=" TimeStepTDB"/>, each of them containing
/// a list of <see cref="VehicleTDB"/>.
/// </summary>
public class TrafficIntegrationData : MonoBehaviour
{
    public List<TimeStepTDB> timeStep { get; set; }
    public volatile int currentTimeStepIndex;
    public AutoResetEvent timeStepResetEvent = new AutoResetEvent(false);

    /// <summary>
    /// Initializes the fields when the script awakes.
    /// </summary>
    /// <remarks> 
    /// The field <see cref="TrafficIntegrationData.currentTimeStepIndex"/> is set to -1 by default 
    /// for functionality purposes related to <see cref="TrafficIntegrationSpawner"/>.
    /// </remarks>
    void Awake()
    {
        ResetTrafficDB();
    }

    /// <summary>
    /// Call this when using thread.
    /// </summary>
    public void ResetTrafficDB()
    {
        this.currentTimeStepIndex = -1;
        timeStep = new List<TimeStepTDB>();
        timeStepResetEvent.Set();
        Debug.Log("trafficDB is ready...");
    }

    /// <summary>
    /// Creates a new timestep. 
    /// </summary>
    /// <param name="time">Time of the simulation.</param>
    public void InsertNewTimeStep(float time)
    {
        timeStep.Add(new TimeStepTDB(time, ++currentTimeStepIndex));
        //this.currentTimeStepIndex++;
        //Debug.Log("new time step: " + time + " curr: " + currentTimeStepIndex);
    }

    internal void InsertVehicle(int timeStepIndex, string id, string lon, string lat, string type, string angle)
    {
        try
        {
            VehicleTDB v = new VehicleTDB(id, lat, lon, type, angle);
            timeStep[timeStepIndex].AddVehicle(v);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    /// <summary>
    /// Inserts a new vehicle in the current timestep.
    /// </summary>
    /// <param name="id">Id of the vehicle.</param>
    /// <param name="lon">Longitude position.</param>
    /// <param name="lat">Latitude position.</param>
    /// <param name="type">Type of the vehicle.</param>
    /// <param name="angle">Angle of the vehicle.</param>
    internal void InsertVehicle(string id, string lon, string lat, string type, string angle)
    {
        InsertVehicle(currentTimeStepIndex, id, lon, lat, type, angle);
        //try
        //{
        //    VehicleTDB v = new VehicleTDB(id, lat, lon, type, angle);
        //    timeStep[currentTimeStepIndex].AddVehicle(v);
        //}
        //catch
        //{
        //    Debug.Log("id: " + id + " curr: " + currentTimeStepIndex);
        //    Debug.Log("Out of range when InsertVehicle");
        //}
    }

    /// <summary>
    /// Gets the total number of timesteps.
    /// </summary>
    /// <returns>Integer with the number of timesteps.</returns>
    internal int getNumberOfTimeSteps()
    {
        return timeStep.Count;
    }


    internal TimeStepTDB GetTimeStep(int index)
    {
        if (index > 0 && index < timeStep.Count)
            return timeStep[index];
        else
            return null;
    }

    /// <summary>
    /// Gets the total number of vehicles in a certain timestep.
    /// </summary>
    /// <param name="index">Index of the timestep.</param>
    /// <returns>
    /// Returns an integer with the number of vehicles in the timestep requested,
    /// or -1 if there are no vehicles in that timestep.
    /// </returns>
    internal int GetNumberOfVehiclesInTimeStep(int index)
    {
        try
        {
            return timeStep[index].vehicles.Count;
        }
        catch
        {
            //No vehicles
            return -1;
        }
    }

    /// <summary>
    /// Gets a <see cref="VehicleTDB"/> given the timestep and the vehicle indices.
    /// </summary>
    /// <param name="timeStepIndex">Index of the timestep.</param>
    /// <param name="vehicleIndex">Index of the vehicle.</param>
    /// <returns></returns>
    internal VehicleTDB GetVehicleAt(int timeStepIndex, int vehicleIndex)
    {
        try
        {
            return timeStep[timeStepIndex].vehicles[vehicleIndex];
        }
        catch
        {
            Debug.Log("Out of range in GetVehicleAt");
            return null;
        }
    }

    void OnApplicationQuit()
    {
        timeStepResetEvent.Reset();
        Debug.LogWarning("ResetEvent reset (blocking simRead thread): ");
    }

    void OnDestroy()
    {
        timeStepResetEvent.Reset();
        Debug.LogWarning("ResetEvent reset (blocking simRead thread): ");
    }

}

