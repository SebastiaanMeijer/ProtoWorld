/*
 * 
 * TRAFFIC INTEGRATION MODULE
 * SimulationWriter.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.IO;

public class SimulationWriter : MonoBehaviour
{
    protected TrafficIntegrationData trafficDB;
    public string fileName;
    public SimulationIOBase simulationIO;
    private static string moduleName = "SimulationWriter";

    void Awake()
    {

    }


    public void SetFileName(string fileName)
    {
        this.fileName = fileName;
    }

    public void SetIO(SimulationIOBase simulationIO)
    {
        this.simulationIO = simulationIO;
    }

    public void Init()
    {
        trafficDB = FindObjectOfType<TrafficIntegrationData>();
        var writer = TryAddWriterToScene();
    }

    public void WorkerUpdate()
    {
        if (Initialized())
        {
            simulationIO.SetTrafficDB(trafficDB);
            simulationIO.Write(fileName);
        }
    }

    public bool Initialized()
    {
        if (simulationIO != null && trafficDB != null)
        {
            if (File.Exists(fileName))
                return true;
        }
        return false;
    }

    public static SimulationWriter TryAddWriterToScene()
    {
        var reader = FindObjectOfType<SimulationWriter>();
        if (reader == null)
        {
            var controller = FindObjectOfType<TrafficIntegrationController>();
            var go = new GameObject(moduleName);
            go.transform.SetParent(controller.transform);
            reader = go.AddComponent<SimulationWriter>();
        }
        return reader;
    }
}
