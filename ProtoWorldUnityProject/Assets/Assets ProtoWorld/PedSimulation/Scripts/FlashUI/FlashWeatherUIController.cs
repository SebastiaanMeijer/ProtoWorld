/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashWeatherUIController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for the buttons of the UI canvas that allows the user to change the weather conditions in the scene.
/// </summary>
public class FlashWeatherUIController : MonoBehaviour
{

    //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    //private int logSeriesId;

    [HideInInspector]
    public FlashPedestriansGlobalParameters globalFlashParam;

    public UnityEngine.UI.Toggle sunnyToggle;
    public UnityEngine.UI.Toggle windyToggle;
    public UnityEngine.UI.Toggle rainyToggle;

    //public EventButtonPanelController ebpc;

    void Awake()
    {
        //TODO: Make log for the weather conditions events

        //logSeriesId = LoggerAssembly.GetLogSeriesId();

        //LOG SUBSCRIBER SLIDER LOG INFO
        //log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "Subscriber slider log"));

        //LOG SUBSCRIBER SLIDER VALUE CHART INFO
        //log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 1, UIChartTypes.Line.ToString()));

        //LOG SUBSCRIBER SLIDER LOG INFO
        //log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Subscriber slider value changed"));
        //log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 1, "Subscriber slider value"));

        globalFlashParam = FindObjectOfType<FlashPedestriansGlobalParameters>();

        // Check the toggle of the initial weather condition according to the global parameters
        switch (globalFlashParam.currentWeatherCondition)
        {
            case FlashPedestriansGlobalParameters.WeatherConditions.SunnyWeather:
                if (sunnyToggle != null) sunnyToggle.isOn = true;
                break;
            case FlashPedestriansGlobalParameters.WeatherConditions.WindyWeather:
                if (windyToggle != null) windyToggle.isOn = true;
                break;
            case FlashPedestriansGlobalParameters.WeatherConditions.RainyWeather:
                if (rainyToggle != null) rainyToggle.isOn = true;
                break;
        }

        //if (ebpc == null)
        //    ebpc = FindObjectOfType<EventButtonPanelController>();
    }

    /// <summary>
    /// Updates the current weather condition. 
    /// </summary>
    /// <param name="value">Integer value that will be casted to the enum WeatherConditions.</param>
    public void UpdateWeatherCondition(int value)
    {
        if (globalFlashParam != null)
            globalFlashParam.currentWeatherCondition = (FlashPedestriansGlobalParameters.WeatherConditions)value;
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    //LOG SUBSCRIBER SLIDER ACTION
    //    log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "value changed"));
    //    //LOG SUBSCRIBER SLIDER VALUE
    //    log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "float", 1, globalFlashParam.percOfPedSubscribed));

    //    if (ebpc != null)
    //        ebpc.AddEvent(Time.time, "% of subscribers set to " + globalFlashParam.percOfPedSubscribed * 100);

    //}
}
