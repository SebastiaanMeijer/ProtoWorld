/*
 * 
 * SESTAR INTEGRATION
 * SeStarTimeController.cs
 * Miguel Ramos Carretero
 * Aram Azhari
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the time of the SESTar simulation
/// </summary>
public class SEStarTimeController : MonoBehaviour
{
    private bool playingSimulation = true;
    private float speedPower = 0.0f;
    private float speedFactor = 1.0f;
    private float timer;
    private int hours;
    private int minutes;
    private int seconds;
    private UnityEngine.UI.Text timeText;
    private SEStar SESTarControl;
    private SumoMainController SumoControl;

    public KPIManager[] KPIsUsingTime;

    /// <summary>
    /// Initializes the script.
    /// </summary>
    void Start()
    {
        SESTarControl = FindObjectOfType<SEStar>();
        SumoControl = FindObjectOfType<SumoMainController>();
        timeText = this.transform.FindChild("Time").GetComponent<UnityEngine.UI.Text>();
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime * speedFactor;
        int hours = Mathf.FloorToInt(timer / 3600F);
        int minutes = Mathf.FloorToInt(timer / 60F - hours * 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        if (Input.GetButtonDown("increaseVel"))
            IncreaseVel();

        if (Input.GetButtonDown("decreaseVel"))
            DecreaseVel();

        timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + " (x" + speedFactor.ToString() + ")";

        if (KPIsUsingTime.Length > 0)
        {
            foreach (KPIManager kpi in KPIsUsingTime)
            {
                kpi.GetComponentInChildren<LineChart>().TimeString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }
        }
    }

    /// <summary>
    /// (In construction) Handler for pause and resume the integrated simulations.
    /// </summary>
    public void PauseAndPlay()
    {
        if (playingSimulation)
        {
            PauseSimulation();
            playingSimulation = false;
            //TODO pause simulation time
            //TODO pause SUMO
        }
        else
        {
            PlaySimulation();
            playingSimulation = true;
            //TODO resume simulation time
            //TODO resume SUMO
        }
    }

    /// <summary>
    /// Increases the velocity of the integrated simulations.
    /// </summary>
    public void IncreaseVel()
    {
        if (speedPower < 3.0f)
        {
            speedPower++;
            ChangeVel();
        }
    }

    /// <summary>
    /// Decreases the velocity of the integrated simulations.
    /// </summary>
    public void DecreaseVel()
    {
        if (speedPower > 0.0f)
        {
            speedPower--;
            ChangeVel();
        }
    }

    /// <summary>
    /// Auxiliar method. Changes the velocities of the integrated simulations.
    /// </summary>
    private void ChangeVel()
    {
        speedFactor = Mathf.Pow(2.0f, speedPower);
        SESTarControl.ChangeSpeedFactor(speedFactor);

        if (speedFactor != 0)
            SumoControl.timeStepVelocityInMs = (int)Mathf.Floor(1000.0f / speedFactor);

        if (KPIsUsingTime.Length > 0)
        {
            foreach (KPIManager kpi in KPIsUsingTime)
            {
                kpi.GetComponentInChildren<LineChart>().updateFrequency = 1.0F / speedFactor;
            }
        }
    }

    /// <summary>
    /// Changes the state of the SeStar simulation to "Playing". 
    /// </summary>
    public void PlaySimulation()
    {
        SESTarControl.ChangeSEStarSimulationState(SEStar.SimulationState.Playing);
    }

    /// <summary>
    /// Changes the state of the SeStar simulation to "Paused". 
    /// </summary>
    public void PauseSimulation()
    {
        SESTarControl.ChangeSEStarSimulationState(SEStar.SimulationState.Paused);
    }

    /// <summary>
    /// Changes the state of the SeStar simulation to "Stopped". 
    /// </summary>
    public void StopSimulation()
    {
        SESTarControl.ChangeSEStarSimulationState(SEStar.SimulationState.Stopped);
    }
}
