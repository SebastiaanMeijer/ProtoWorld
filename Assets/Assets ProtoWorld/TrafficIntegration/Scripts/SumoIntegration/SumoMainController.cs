/*
 * 
 * SUMO COMMUNICATION
 * SumoMainController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Threading;
using System;

/// <summary>
/// Implements an interface for communication with SUMO.
/// </summary>
public class SumoMainController : MonoBehaviour
{
    [HideInInspector]
    [Range(0.1f, 1000)]
    public int timeStepVelocityInMs = 1000;

    [HideInInspector]
    public bool runSumoWhenGameModeStarts = true;

    Thread simulationThread;
    private SumoTCPCommunication tcpComScript;
    private bool isSimulationStarted = false;
    private static bool isPause = false;

    private TrafficIntegrationController trafficContr;

    /// <summary>
    /// This is kept for backward compatibility with the old module.
    /// </summary>
    private SumoConfig oldConf;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        trafficContr = FindObjectOfType<TrafficIntegrationController>();

        if (trafficContr != null)
        {
            runSumoWhenGameModeStarts = (trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration);
            timeStepVelocityInMs = trafficContr.timeStepVelocityInMs;
        }
        else
        {
            // running SUMO with old module:
            oldConf = this.GetComponentInParent<SumoConfig>();
            if (oldConf != null)
                runSumoWhenGameModeStarts = oldConf.integrateSumo && !oldConf.simulateFromFCDFile;
        }
    }

    /// <summary>
    /// Starts the script.
    /// </summary>
    void Start()
    {
        tcpComScript = (SumoTCPCommunication)this.GetComponent(typeof(SumoTCPCommunication));

        if (runSumoWhenGameModeStarts)
            RunSimulation();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.playmodeStateChanged += HandleOnPlayModeChanged;
#endif
    }

    /// <summary>
    /// Updates method.
    /// </summary>
    void Update()
    {
        // Update the pause state from the traffic controller
        if (trafficContr != null &&  trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration)
        {
            if (trafficContr.simulationPaused && !isPause)
                PauseSimulation();
            else if (!trafficContr.simulationPaused && isPause)
                RunSimulation();
        }
    }

    /// <summary>
    /// Starts SUMO simulation running an infinite loop of timesteps. 
    /// If simulation is in pause, resumes the simulation.
    /// </summary>
    /// <remarks>
    /// While running the simulation, the time between timesteps is set 
    /// by the public field <see cref="MainController.timeStepVelocityInMs"/> (in ms). 
    /// A thread is created to run the simulation in loops. For each timestep, 
    /// the thread request a new simulation timestep to <see cref="SumoTCPCommunication"/>.
    /// </remarks>
    /// <seealso cref="RunSimThread"/>
    public void RunSimulation()
    {
        if (!isSimulationStarted)
        {
            //Create a thread for the loop
            ThreadStart ts = new ThreadStart(RunSimThread);
            simulationThread = new Thread(ts);
            simulationThread.Start();
            Debug.Log("Thread for running simulation created");
            isSimulationStarted = true;
        }
        if (isPause)
        {
            isPause = false;
            simulationThread.Interrupt();
        }
    }

    /// <summary>
    /// If SUMO simulation is running, pauses the simulation. 
    /// </summary>
    public void PauseSimulation()
    {
        if (isSimulationStarted)
        {
            isPause = true;
        }
    }

    /// <summary>
    /// Runs a single timestep in SUMO. 
    /// This method only works if the simulation is in pause. 
    /// </summary>
    public void RunSingleStep()
    {
        if (isSimulationStarted && isPause)
        {
            tcpComScript.RunSingleSimulationStep();
        }
    }

    /// <summary>
    /// Requests SUMO to end the current simulation.
    /// </summary>
    public void EndSimulation()
    {
        if (isSimulationStarted)
        {
            tcpComScript.EndSimulation();
        }
    }

    /// <summary>
    /// Requests SUMO to change the speed of a certain vehicle in the current simulation. Doing
    /// this, the vehicle will keep the same speed until its car-following behaviour is resumed.
    /// </summary>
    /// <param name="vehId">String containing the vehicle id.</param>
    /// <param name="speed">New speed for the vehicle.</param>
    /// <param name="ms">Milliseconds to reach the new speed.</param>
    /// <seealso cref="ResumeVehicleBehaviour"/>
    public void ChangeVehicleSpeed(string vehId, double speed, int ms)
    {
        if (isSimulationStarted && !isPause)
        {
            tcpComScript.ChangeVehicleSpeed(vehId, speed, ms);
        }
    }

    /// <summary>
    /// Requests SUMO to resume the car-following behaviour of a certain vehicle in the current
    /// simulation.
    /// </summary>
    /// <param name="vehId">String containing the vehicle id.</param>
    /// <seealso cref="ChangeVehicleSpeed"/>
    public void ResumeVehicleBehaviour(string vehId)
    {
        if (isSimulationStarted && !isPause)
        {
            tcpComScript.ResumeVehicleBehaviour(vehId);
        }
    }

    /// <summary>
    /// Requests SUMO to add a new vehicle in the simulation.
    /// </summary>
    /// <param name="vehId">String containing the vehicle id.</param>
    /// <param name="type">String containing the type of the vehicle.</param>
    /// <param name="routeId">String containing the route id of the vehicle.</param>
    /// <param name="departTime">Specify at which timestep of the simulation the vehicle should depart.</param>
    /// <param name="departPosition">Specify the position from which the vehicle should depart.</param>
    /// <param name="departSpeed">Specify the initial speed of the vehicle.</param>
    /// <param name="departLane">Specify the lane where the vehicle should start.</param>
    public void AddNewVehicle(string vehId, string type, string routeId, int departTime, double departPosition, double departSpeed, byte departLane)
    {
        if (isSimulationStarted && !isPause)
        {
            tcpComScript.AddNewVehicle(vehId, type, routeId, departTime, departPosition, departSpeed, departLane);
        }
    }

    /// <summary>
    /// Requests SUMO to add a stop in an existing vehicle of the simulation.
    /// </summary>
    /// <param name="vehId">String containing the vehicle id.</param>
    /// <param name="edgeId">String containing the edge where the vehicle should stop.</param>
    /// <param name="position">Position in the edge where the vehicle should stop.</param>
    /// <param name="laneIndex">Index of the lane.</param>
    /// <param name="durationInMs">Number of milliseconds the vehicle will stop before continuing its trip.</param>
    public void AddStopInVehicle(string vehId, string edgeId, double position, byte laneIndex, int durationInMs)
    {
        if (isSimulationStarted && !isPause)
        {
            tcpComScript.AddStopInVehicle(vehId, edgeId, position, laneIndex, durationInMs);
        }
    }

    /// <summary>
    /// Gets the current timestep of the SUMO simulation.
    /// </summary>
    /// <returns>Return the current timestep or -1 if the simulation is not running.</returns>
    public int getCurrentTimeStep()
    {
        return tcpComScript.stepNumber;
    }

    /// <summary>
    /// Auxiliar method for threading. 
    /// Runs an infinite loop of simulation timesteps in SUMO. 
    /// </summary>
    /// <remarks>
    /// While running the simulation, the time between timesteps is set 
    /// by the public field <see cref="timeStepVelocityInMs"/> (in ms). 
    /// </remarks>
    private void RunSimThread()
    {
        while (true)
        {
            try
            {
                if (!isPause)
                {
                    tcpComScript.RunSingleSimulationStep();
                    Thread.Sleep(timeStepVelocityInMs);
                }
                else
                    Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadAbortException)
            {
                UnityEngine.Debug.Log("Thread for loop simulation aborted");
                return;
            }
            catch (ThreadInterruptedException)
            {
                //Do nothing
            }
        }
    }

    /// <summary>
    /// When application quits, ends SUMO simulation. 
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("Application quitting! Finish TCP Communication");
        tcpComScript.EndSimulation();
    }

    /// <summary>
    /// Handles the mode changes while the application is running (used to handle the editor pause).
    /// </summary>
    void HandleOnPlayModeChanged()
    {
#if UNITY_EDITOR
        isPause = UnityEditor.EditorApplication.isPaused;
#endif

        if (isSimulationStarted && !isPause)
        {
            simulationThread.Interrupt();
        }
    }
}
