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