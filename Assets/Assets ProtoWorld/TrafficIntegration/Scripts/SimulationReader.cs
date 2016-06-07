/*
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

public class SimulationReader : MonoBehaviour
{
    protected TrafficIntegrationController trafficController;

    protected TrafficIntegrationData trafficDB;

    protected TimeController timeController;

    protected bool useNetworkStream;

    protected int port = 3654;

    public string fileName;

    public SimulationIOBase simulationIO;

    private static string moduleName = "SimulationReader";

    void Awake()
    {
        timeController = FindObjectOfType<TimeController>();

        trafficDB = FindObjectOfType<TrafficIntegrationData>();

        trafficController = FindObjectOfType<TrafficIntegrationController>();
        if (trafficDB != null && trafficController != null)
            Debug.Log("TrafficIntegrationData & TrafficIntegrationController found. ");
        else
        {
            Debug.Log("TrafficIntegrationData & TrafficIntegrationController NOT found. ");
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
            case TrafficIntegrationController.TypeOfIntegration.MatsimXMLFile:
                SetUseNetworkStream(false);
                reader.SetFileName(trafficController.pathMatSimXMLFile);
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

    public void SetUseNetworkStream(bool useNetworkStream)
    {
        this.useNetworkStream = useNetworkStream;
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
            if (useNetworkStream)
            {
                return true;
            }
            else
            {
                if (File.Exists(fileName))
                {
                    return true;
                }
            }
        }

        return false;
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

            Thread thread = new Thread(simulationIO.Read);
            if (useNetworkStream)
            {
                thread.Start(port);
                simulationIO.status = "Listening port : " + port;
            }
            else
            {
                thread.Start(fileName);
                simulationIO.status = "Reading file: " + System.IO.Path.GetFileName(fileName);
            }
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

    void OnDestroy()
    {
        if (simulationIO != null)
        {
            simulationIO.RequestStop();
            //simulationIO.CloseWindow();
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



