/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * TIME CONTROLLER
 * TimeController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the time of the simulation
/// </summary>
public class TimeController : MonoBehaviour
{
    /// <summary>
    /// This is the timer that orchrests the game and the integrated simulations.
    /// </summary>
    public float gameTime = 0f;

    private bool gamePaused = false;

    [Range(0, 10)]
    public int maxSpeedFactor = 3;

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    private LoggerAssembly loggerAssembly;

    [HideInInspector]
    [Range(1.0f, 10.0f)]
    public float timeVelocity = 1.0f;

    // Parameters related to the time controller
    public bool showClock = true;
    public bool showTimeSteps = true;
    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Text pauseButtonLabel;
    public UnityEngine.UI.Text scenarioTitle;
    public UnityEngine.UI.Button pauseButton;
    public UnityEngine.UI.Slider timeSlider;
    public GameObject loadingSplash;

    [HideInInspector]
    public SumoMainController smc;

    [HideInInspector]
    public TrafficIntegrationController tic;

    [HideInInspector]
    public EventButtonPanelController ebpc;

    [HideInInspector]
    public FlashPedestriansGlobalParameters fped;

    // Parameters for time slider and splash screen
    private TimeSliderController timeSliderContr;
    private bool isLoadingSplashOn = false;
    private bool isLoadingTime = false;
    private bool isMaxValueSetInSlider = false;

    // Parameters to handle the time slider
    private bool usingTimeSlider = false;
    private int totalNumberOfTimeSteps = 0;

    // Parameters to enable requested pauses/resumes
    private bool requestPause = false;
    private bool requestedPauseState = false;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        smc = FindObjectOfType<SumoMainController>();
        tic = FindObjectOfType<TrafficIntegrationController>();
        ebpc = FindObjectOfType<EventButtonPanelController>();
        fped = FindObjectOfType<FlashPedestriansGlobalParameters>();

        if (timeSlider != null)
            timeSliderContr = timeSlider.GetComponent<TimeSliderController>();

        if (loadingSplash != null)
            loadingSplash.SetActive(true);
    }

    /// <summary>
    /// Starts the script.
    /// </summary>
    void Start()
    {
        loggerAssembly = FindObjectOfType<LoggerAssembly>();

        if (loggerAssembly != null && loggerAssembly.logTimeController)
        {
            //LOG TIME CONTROLLER LOG INFO
            log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "User time velocity log"));

            //LOG STATION QUEUING CHART INFO
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 1, UIChartTypes.Line.ToString()));

            //LOG TIME CONTROLLER LOG INFO
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Time velocity changed"));
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 1, "Time velocity values"));
        }

        Time.timeScale = timeVelocity;
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        // Check if there is any pause request and handle it
        if (requestPause)
        {
            requestPause = false;
            PauseGame(requestedPauseState);
        }

        // Update time scale and game time
        Time.timeScale = timeVelocity;
        if (!gamePaused && (!usingTimeSlider || (gameTime < totalNumberOfTimeSteps && !Input.GetMouseButton(0))))
            gameTime += Time.deltaTime;

        int hours = Mathf.FloorToInt(gameTime / 3600F);
        int minutes = Mathf.FloorToInt(gameTime / 60F - hours * 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);

        // FIXME Consider using argument --step-length in the SUMO simulation
        if (smc != null)
            smc.timeStepVelocityInMs = (int)(1000.0f / timeVelocity);

        // Updates the time velocity in the traffic integration controller
        if (tic != null)
            tic.timeStepVelocityInMs = (int)(1000.0f / timeVelocity);

        if (Input.GetButtonDown("increaseVel"))
            IncreaseVel();

        if (Input.GetButtonDown("decreaseVel"))
            DecreaseVel();

        // Show the clock in the textbox
        timerText.text = "";
        if (showClock)
            timerText.text += string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        // Show the timesteps in the textbox
        if (showTimeSteps)
            timerText.text += " [" + Mathf.FloorToInt(gameTime) + "]";

        // Show the velocity factor in the textbox
        timerText.text += " (x" + timeVelocity.ToString() + ")";

        //text.text = "Time Velocity: x" + timeVelocity.ToString() + "\n Current Time: " + string.Format("{0:0000}", Time.time);

        // Show/hide the loading icon
        if (loadingSplash != null)
        {
            if (isLoadingTime && !isLoadingSplashOn)
            {
                // Fade in loading splash
                loadingSplash.GetComponent<FadingElementUI>().fadeInCanvas();

                // Prevent any click to pass through the loading screen
                loadingSplash.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;

                isLoadingSplashOn = true;
            }
            else if (!isLoadingTime && isLoadingSplashOn)
            {
                // Fade out loading splash
                loadingSplash.GetComponent<FadingElementUI>().fadeOutCanvas();

                // Allow clicks to pass through the loading screen
                loadingSplash.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;

                isLoadingSplashOn = false;
            }
        }

        // Updates the time slider
        if (usingTimeSlider && timeSlider != null)
        {
            if (!timeSlider.gameObject.activeSelf)
                timeSlider.gameObject.SetActive(true);

            if (!isMaxValueSetInSlider)
            {
                timeSliderContr.SetMaxValue(totalNumberOfTimeSteps - 1);
                isMaxValueSetInSlider = true;
            }

            timeSlider.value = gameTime;
        }
    }

    /// <summary>
    /// Increases the velocity of the game.
    /// </summary>
    public void IncreaseVel()
    {
        if (timeVelocity < maxSpeedFactor)
        {
            timeVelocity += 1.0f;

            Debug.Log("Velocity increased to " + timeVelocity);

            if (loggerAssembly != null && loggerAssembly.logTimeController)
            {
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "increased"));
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "float", 1, timeVelocity));
            }

            if (ebpc != null)
                ebpc.AddEvent(Time.time, "Velocity increased to " + timeVelocity, false);
        }
    }

    /// <summary>
    /// Decreases the velocity of the game.
    /// </summary>
    public void DecreaseVel()
    {
        if (timeVelocity > 1)
        {
            timeVelocity -= 1.0f;

            Debug.Log("Velocity decreased to " + timeVelocity);

            if (loggerAssembly != null && loggerAssembly.logTimeController)
            {
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "decreased"));
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "float", 1, timeVelocity));
            }

            if (ebpc != null)
                ebpc.AddEvent(Time.time, "Velocity decreased to " + timeVelocity, false);
        }
    }

    /// <summary>
    /// Requests a pause/unpause of the game.
    /// </summary>
    /// <param name="pause">True if the game should be paused.</param>
    public void PauseGame(bool pause)
    {
        if (tic != null)
            tic.simulationPaused = pause;

        if (fped != null)
            fped.flashPedestriansPaused = pause;

        gamePaused = pause;

        if (pause)
        {
            Debug.Log("Game paused");

            if (loggerAssembly != null && loggerAssembly.logTimeController)
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "game paused"));

            if (pauseButtonLabel != null)
                pauseButtonLabel.text = "Resume";
        }
        else
        {
            Debug.Log("Game resumed");

            if (loggerAssembly != null && loggerAssembly.logTimeController)
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "game resumed"));

            if (pauseButtonLabel != null)
                pauseButtonLabel.text = "Pause";
        }
    }

    /// <summary>
    /// Alternative method to trigger the pause/unpause of the game.
    /// </summary>
    public void PauseGameTrigger()
    {
        PauseGame(!gamePaused);
    }

    /// <summary>
    /// Alternative method to request to pause the game (needed for threading).
    /// </summary>
    public void RequestPauseGame()
    {
        requestPause = true;
        requestedPauseState = true;
    }

    /// <summary>
    /// Alternative method to request to resume the game (needed for threading).
    /// </summary>
    public void RequestResumeGame()
    {
        requestPause = true;
        requestedPauseState = false;
    }

    /// <summary>
    /// Tells if the game is paused.
    /// </summary>
    /// <returns>True if the game is paused.</returns>
    public bool IsPaused()
    {
        return gamePaused;
    }

    /// <summary>
    /// Tells if the time slider is active in the game. 
    /// </summary>
    /// <returns>True if it the time slider is active.</returns>
    public bool IsSliderTimeActive()
    {
        return usingTimeSlider;
    }

    /// <summary>
    /// Tells if the time slider is moving. 
    /// </summary>
    /// <returns>True if it the time slider is moving.</returns>
    public bool IsSliderMoving()
    {
        if (timeSliderContr != null)
            return timeSliderContr.IsSliderMoving();
        else
            return false;
    }

    /// <summary>
    /// Requests to show/hide the splash loading icon on the screen. 
    /// </summary>
    /// <param name="show">True if the splash icon should be shown.</param>
    public void ShowLoadingIcon(bool show)
    {
        isLoadingTime = show;
    }

    /// <summary>
    /// Sets and activates the time slider. 
    /// </summary>
    /// <param name="numberOfTimeSteps">Total number of timesteps that the time slider will cover.</param>
    public void ActivateTimeSlider(int numberOfTimeSteps)
    {
        this.totalNumberOfTimeSteps = numberOfTimeSteps;
        usingTimeSlider = true;
        isMaxValueSetInSlider = false;
    }

    /// <summary>
    /// Changes the scenario title in the UI Interface.
    /// </summary>
    /// <param name="newtitle">Title of the scenario.</param>
    public void ChangeScenarioTitle(string newtitle)
    {
        if (scenarioTitle != null)
            scenarioTitle.text = newtitle;
    }

    /// <summary>
    /// Blocks/unblocks the pause button in the UI.
    /// </summary>
    /// <param name="block">True if the pause button should be blocked.</param>
    public void BlockPauseButton(bool block)
    {
        if (pauseButton != null)
            pauseButton.enabled = !block;
    }

    /// <summary>
    /// Actions to perform on destroy.
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("Restoring time scale...");
        Time.timeScale = 1.0f;
    }
}

