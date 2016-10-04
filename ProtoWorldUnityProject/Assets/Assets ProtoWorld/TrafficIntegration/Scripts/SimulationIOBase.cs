/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * TRAFFIC INTEGRATION MODULE
 * SimulationIOBase.cs
 * Johnson Ho
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
//using UnityEditor;

/// <summary>
/// Abstract class for the different simulation output format.
/// Formats include: Sumo, Vissim, Matsim & ProtoWorldSim.
/// </summary>
public abstract class SimulationIOBase
{
    protected HashSet<float> timeSteps;

    protected TrafficIntegrationData trafficDB;

    protected TimeController timeController;

    protected TrafficIntegrationController trafficController;

    protected volatile bool shouldStop;

    public string status;

    protected BackgroundWorker worker;

    protected float startTimeStep, progress, totalSteps;

    protected int currentReadingStep, readingChunkForMatsim = 200;

    /// <summary>
    /// Show status of reading simulation, only called from menu item.
    /// </summary>
    public void ShowWindow()
    {
        //NOTE: THIS GIVES PROBLEMS BUILDING THE RELEASE OF THE GAME
        //var window = UnityEditor.EditorWindow.GetWindow(typeof(SimulationIOWindow)) as SimulationIOWindow;
        //window.SetSimulationIO(this);
        //window.Show();
    }

    /// <summary>
    /// Initiate write with backgroundWorker, method Write has to be implemented in a sub-class.
    /// </summary>
    /// <param name="fileName"></param>
    public void Write(string fileName)
    {
        worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        worker.DoWork += Write; // Write = method to be overriden in a sub-class.
        worker.ProgressChanged += WorkerProgress;
        worker.RunWorkerCompleted += WorkerCompleted;
        worker.RunWorkerAsync(fileName);
    }


    /// <summary>
    /// Call this to start a thread to read a simulation output-file.
    /// If param is string, the IO expects a file name.
    /// If param is int, the IO expects a port number.
    /// </summary>
    /// <param name="param"></param>
    public void Read(object param)
    {
        if (!Initialized())
            return;

        // Pause the game here and show the loading splash
        if (trafficController != null &&
            trafficController.typeOfIntegration != TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration &&
            trafficController.typeOfIntegration != TrafficIntegrationController.TypeOfIntegration.NoTrafficIntegration &&
            timeController != null)
        {
            timeController.RequestPauseGame();
            timeController.ShowLoadingIcon(true);
        }

        // Wait for traffDB reset in Unity-thread.
        Debug.Log("Waiting for trafficDB.");
        trafficDB.timeStepResetEvent.WaitOne(2000);
        Debug.Log("Waiting is over, trafficDB is ready.");

        timeSteps = new HashSet<float>();
        timeSteps.Clear();

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        if (param is string)
        {
            shouldStop = false;
            Read((string)param);
        }
        if (param is int)
        {
            shouldStop = false;
            Read((int)param);
        }

        stopwatch.Stop();
        Debug.Log("Read time elapsed: " + stopwatch.Elapsed);

        // Resume the game here and hide the loading splash
        if (trafficController != null &&
            trafficController.typeOfIntegration != TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration &&
            trafficController.typeOfIntegration != TrafficIntegrationController.TypeOfIntegration.NoTrafficIntegration &&
            timeController != null)
        {
            timeController.RequestResumeGame();
            timeController.ShowLoadingIcon(false);
            timeController.ActivateTimeSlider(trafficDB.getNumberOfTimeSteps());
        }

        //Keeps reading concurrently for MatSim integration
        if (trafficController != null &&
            trafficController.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.MatsimDatabase &&
            timeController != null)
        {
            int completedIterations = 1;

            Debug.Log(trafficDB.getNumberOfTimeSteps() + ", " + readingChunkForMatsim * completedIterations);

            while(trafficDB.getNumberOfTimeSteps() == readingChunkForMatsim * completedIterations)
            {
                shouldStop = false;
                Read((string)param);
                timeController.ActivateTimeSlider(trafficDB.getNumberOfTimeSteps());
                completedIterations++;
            }
        }
    }

    /// <summary>
    /// Return true if traffic database is set.
    /// </summary>
    /// <returns></returns>
    public bool Initialized()
    {
        if (trafficDB != null)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Set the traffic database that store the output of the simulation.
    /// </summary>
    /// <param name="trafficDB"></param>
    public void SetTrafficDB(TrafficIntegrationData trafficDB)
    {
        this.trafficDB = trafficDB;
    }

    /// <summary>
    /// Set the time controller to allow pause/resume of the game.
    /// </summary>
    /// <param name="timeController"></param>
    public void SetTimeController(TimeController timeController)
    {
        this.timeController = timeController;
    }

    /// <summary>
    /// Set the traffic controller.
    /// </summary>
    /// <param name="trafficController"></param>
    public void SetTrafficIntegrationController(TrafficIntegrationController trafficController)
    {
        this.trafficController = trafficController;
    }

    /// <summary>
    /// Return whether the reading has/should stop, called by SimulationIOWindow.
    /// </summary>
    /// <returns></returns>
    public bool IsStopped()
    {
        return shouldStop;
    }

    /// <summary>
    /// Request stop in reading-threads.
    /// </summary>
    public virtual void RequestStop()
    {
        //Debug.Log("Request stop...");
        shouldStop = true;
    }

    /// <summary>
    /// Return the reading progress, called by SimulationIOWindow.
    /// </summary>
    /// <returns></returns>
    public virtual string GetStatus()
    {
        return GetTimeStepCount() + " time steps read.";
    }

    /// <summary>
    /// Return the number of time steps read.
    /// </summary>
    /// <returns></returns>
    protected int GetTimeStepCount()
    {
        if (timeSteps == null)
            return -1;
        else
            return timeSteps.Count;
    }

    /// <summary>
    /// Add a time step with a corresponding time in simulation to the traffic database.
    /// </summary>
    /// <param name="time"></param>
    protected virtual void TryAddTimeStep(float time)
    {
        // True if added and False if already contains time.
        if (timeSteps.Add(time))
        {
            trafficDB.InsertNewTimeStep(time);
                            currentReadingStep++;

        }
        //Debug.Log(trafficDB.getNumberOfTimeSteps());
    }

    /// <summary>
    /// Standard way for the sub-classes to add a vehicle to the traffic database.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
    /// <param name="angle"></param>
    protected virtual void InsertVehicle(string id, string x, string y, string type, string angle)
    {
        trafficDB.InsertVehicle(id, x, y, type, angle);
        //Debug.LogFormat("t: {0} id: {1} x: {2} y: {3} ang: {4}", "", id, x, y, angle);
    }

    /// <summary>
    /// Overriden by streaming-capable IO
    /// </summary>
    /// <param name="port"></param>
    public virtual void Read(int port)
    {

    }

    public abstract void Read(string fileName);
    public abstract bool CanRead(string fileName);
    public abstract void Write(object sender, DoWorkEventArgs e);

    public void WorkerProgress(object sender, ProgressChangedEventArgs e)
    {
        progress = (e.ProgressPercentage - startTimeStep) / totalSteps * 100f;
    }
    public void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Cancelled)
            Debug.Log("You canceled!");
        else if (e.Error != null)
            Debug.Log("Worker exception: " + e.Error.ToString());
        else
            Debug.Log("Complete: " + e.Result);
    }

    public abstract bool CanWrite(string fileName);
}
