/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿
/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedTakingBikesSliderController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Controller for the slider of the UI canvas that allows the user to change the percentage of pedestrians taking bikes.
/// </summary>
public class FlashPedTakingBikesSliderController : MonoBehaviour 
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;

    [HideInInspector]
    public FlashPedestriansGlobalParameters globalFlashParam;

    public EventButtonPanelController ebpc;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        //LOG SUBSCRIBER SLIDER LOG INFO
        log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "Bikers slider log"));

        //LOG SUBSCRIBER SLIDER VALUE CHART INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 1, UIChartTypes.Line.ToString()));

        //LOG SUBSCRIBER SLIDER LOG INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Bikers slider value changed"));
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 1, "Bikers slider value"));

        globalFlashParam = FindObjectOfType<FlashPedestriansGlobalParameters>();

        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();
    }

    public void UpdatePercentageOfBikers(float value)
    {
        if (globalFlashParam != null)
            globalFlashParam.percOfPedTakingBikes = value / 100.0f;

        this.transform.FindChild("ValueText").GetComponent<UnityEngine.UI.Text>().text = value.ToString();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //LOG SUBSCRIBER SLIDER ACTION
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "value changed"));
        //LOG SUBSCRIBER SLIDER VALUE
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "float", 1, globalFlashParam.percOfPedTakingBikes));

        if (ebpc != null)
            ebpc.AddEvent(Time.time, "% of bikers set to " + globalFlashParam.percOfPedTakingBikes * 100);
    }
}
