/*
 * 
 * TIME CONTROLLER
 * TimeSliderController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.Slider))]
public class TimeSliderController : MonoBehaviour 
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private float logPeriod = 0.1f;
    private float nextLogTime = 0.0f;

    private TimeController timeController;
    public bool movingSlider = false;

    void Awake()
    {
        timeController = FindObjectOfType<TimeController>();
    }

    void Update()
    {
        if (!Input.GetMouseButton(0))
            movingSlider = false;
    }

    public void SetMaxValue(float value)
    {
        this.GetComponent<UnityEngine.UI.Slider>().maxValue = value;
    }

    public void UpdateGameTime()
    {
        if (timeController != null && !timeController.IsPaused() && Input.GetMouseButton(0))
        {
            movingSlider = true;
            timeController.gameTime = this.GetComponent<UnityEngine.UI.Slider>().value;

            if (Time.time > nextLogTime)
            {
                Debug.Log("Time slider moved to position " + this.GetComponent<UnityEngine.UI.Slider>().value);
                log.Info("Time slider moved to position " + this.GetComponent<UnityEngine.UI.Slider>().value);
                nextLogTime = Time.time + logPeriod; 
            }
        }
    }

    public bool IsSliderMoving()
    {
        return movingSlider;
    }
}