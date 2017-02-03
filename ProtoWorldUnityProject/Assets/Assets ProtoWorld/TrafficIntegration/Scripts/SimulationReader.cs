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
 * SimulationReader.cs
 * Johnson Ho
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.IO;
using System.Threading;
using Npgsql;

public class SimulationReader : MonoBehaviour
{
    protected TrafficIntegrationController trafficController;

    protected TrafficIntegrationData trafficDB;

    protected TimeController timeController;

    protected MapBoundaries aramGisBoundaries;

    protected bool useNetworkStream;

    protected int port = 3654;

    protected bool useDataBase;

    protected string connectionString;
    
    [HideInInspector]
    public string fileName;

    public SimulationIOBase simulationIO;

    private static string moduleName = "SimulationReader";

    Thread thread;

    void Awake()
    {
        timeController = FindObjectOfType<TimeController>();

        trafficDB = FindObjectOfType<TrafficIntegrationData>();
        if (trafficDB == null)
        {
            Debug.Log("TrafficIntegrationData NOT found. ");
            return;
        }
        
        trafficController = FindObjectOfType<TrafficIntegrationController>();
        if (trafficController == null)
        {
            Debug.Log("TrafficIntegrationController NOT found. ");
            return;
        }

        aramGisBoundaries = FindObjectOfType<MapBoundaries>();
        if (aramGisBoundaries == null)
        {
            Debug.Log("MapBoundaries NOT found. ");
            return;
        }

        var reader = FindSimulationReader();
        reader.SetTrafficDB(trafficDB);

        switch (trafficController.typeOfIntegration)
        {
            case TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration:
                SetUseNetworkStream(true);
                reader.SetIO(new SumoIO());
                break;
            case TrafficIntegrationController.TypeOfIntegration.SumoFCDFile:
                SetUseNetworkStream(false);
                reader.SetFileName(trafficController.pathSumoFCDFile);
                reader.SetIO(new SumoIO());
                break;
            case TrafficIntegrationController.TypeOfIntegration.VissimFZPFile:
                SetUseNetworkStream(false);
                reader.SetFileName(trafficController.pathVissimFZPFile);
                reader.SetIO(new VissimIO());
                break;
            case TrafficIntegrationController.TypeOfIntegration.MatsimDatabase:
                reader.SetConnectionString(aramGisBoundaries.GetOverridenConnectionString());
                reader.SetIO(new MatsimIO());
                break;
            case TrafficIntegrationController.TypeOfIntegration.PWSimPWSFile:
                SetUseNetworkStream(false);
                reader.SetFileName(trafficController.pathPWSimMetaFile);
                reader.SetIO(new ProtoMetaIO());
                break;
            case TrafficIntegrationController.TypeOfIntegration.DecisionTreeIntegration:
                return;
            case TrafficIntegrationController.TypeOfIntegration.NoTrafficIntegration:
                return;
        }

        reader.ThreadUpdate();
    }

    public void SetUseDataBase(bool useDataBase)
    {
        this.useDataBase = useDataBase;
    }

    public void SetUseNetworkStream(bool useNetworkStream)
    {
        this.useNetworkStream = useNetworkStream;
    }

    public void SetConnectionString(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void SetFileName(string fileName)
    {
        this.fileName = fileName;
    }

    public void SetIO(SimulationIOBase simulationIO)
    {
        this.simulationIO = simulationIO;
    }

    public bool Initialized()
    {
        if (simulationIO != null && trafficDB != null)
        {
            trafficController = FindObjectOfType<TrafficIntegrationController>();
            if (trafficController != null)
            {
                switch (trafficController.typeOfIntegration)
                {
                    case TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration:
                        return true;
                    case TrafficIntegrationController.TypeOfIntegration.SumoFCDFile:
                    case TrafficIntegrationController.TypeOfIntegration.VissimFZPFile:
                    case TrafficIntegrationController.TypeOfIntegration.PWSimPWSFile:
                        // Check file exists.
                        return File.Exists(fileName);
                    case TrafficIntegrationController.TypeOfIntegration.MatsimDatabase:
                        // Check connection open.
                        Debug.Log(connectionString);
                        return IsConnectionOpen(connectionString);
                        //case TrafficIntegrationController.TypeOfIntegration.DecisionTreeIntegration:
                        //    return;
                        //case TrafficIntegrationController.TypeOfIntegration.NoTrafficIntegration:
                        //    return;
                }
            }
            //if (useNetworkStream)
            //{
            //    return true;
            //}
            //else
            //{
            //    if (File.Exists(fileName))
            //    {
            //        return true;
            //    }
            //}
        }

        return false;
    }

    public bool IsConnectionOpen(string connectionString)
    {
        try
        {
            NpgsqlConnection dbConn = new NpgsqlConnection(connectionString);
            dbConn.Open();
            Debug.Log("Connection is open...");
            dbConn.Close();
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex + " " + connectionString);
            return false;
        }
    }

    public void TryShowWindow()
    {
        //simulationIO.ShowWindow();
    }

    public void ThreadUpdate()
    {
        if (Initialized())
        {
            if (timeController != null)
                simulationIO.SetTimeController(timeController);

            if (trafficController != null)
                simulationIO.SetTrafficIntegrationController(trafficController);

            if (trafficDB != null)
                simulationIO.SetTrafficDB(trafficDB);

            thread = new Thread(simulationIO.Read);
            switch (trafficController.typeOfIntegration)
            {
                case TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration:
                    thread.Start(port);
                    simulationIO.status = "Listening port : " + port;
                    break;
                case TrafficIntegrationController.TypeOfIntegration.SumoFCDFile:
                case TrafficIntegrationController.TypeOfIntegration.VissimFZPFile:
                case TrafficIntegrationController.TypeOfIntegration.PWSimPWSFile:
                    thread.Start(fileName);
                    simulationIO.status = "Reading file: " + System.IO.Path.GetFileName(fileName);
                    break;
                case TrafficIntegrationController.TypeOfIntegration.MatsimDatabase:
                    thread.Start(connectionString);
                    simulationIO.status = "Reading Database: " + connectionString;
                    break;
            }
            //if (useNetworkStream)
            //{
            //    thread.Start(port);
            //    simulationIO.status = "Listening port : " + port;
            //}
            //else
            //{
            //    thread.Start(fileName);
            //    simulationIO.status = "Reading file: " + System.IO.Path.GetFileName(fileName);
            //}
        }
        else
        {
            simulationIO.status = "Not initialized yet.";
        }

    }

    public void SetTrafficDB(TrafficIntegrationData trafficDB)
    {
        this.trafficDB = trafficDB;
    }

    public TrafficIntegrationData GetTrafficDB()
    {
        return trafficDB;
    }

    /// <summary>
    /// Releases the resources when the object containing the script is being destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (simulationIO != null)
        {
            simulationIO.RequestStop();
            //simulationIO.CloseWindow();
        }

        if (thread != null)
        {
            thread.Abort();
        }
    }

    /// <summary>
    /// When application quits, aborts the thread for listening. 
    /// </summary>
    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Application quitting! Abort Simulation Reader");
        if (simulationIO != null)
        {
            simulationIO.RequestStop();

            if (thread != null)
                thread.Abort();
            return;

            //simulationIO.CloseWindow();
        }
    }

    /// <summary>
    /// Return the SimulationReader in the scene or add one automatically if not exists.
    /// </summary>
    /// <returns></returns>
    public static SimulationReader FindSimulationReader()
    {
        var reader = FindObjectOfType<SimulationReader>();
        if (reader == null)
        {
            var controller = FindObjectOfType<TrafficIntegrationController>();
            var go = new GameObject(moduleName);
            go.transform.SetParent(controller.transform);
            reader = go.AddComponent<SimulationReader>();
        }
        return reader;
    }

}



