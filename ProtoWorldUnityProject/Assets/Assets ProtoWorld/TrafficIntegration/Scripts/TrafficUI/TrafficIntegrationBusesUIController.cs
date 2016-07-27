/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * SUMO COMMUNICATION
 * TrafficIntegrationBusUIController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Controller for the slider of the UI canvas that allows the user to change the frequency of spawning buses.
/// </summary>
public class TrafficIntegrationBusesUIController : MonoBehaviour
{
    public StationController stationOption1;
    public StationController stationOption2;
    public SumoBusSpawner busSpawner;
    public UnityEngine.UI.Text sliderValueText;
    public EventButtonPanelController ebpc;

    public GameObject driebergenParking;

    private int currentOptionActive = 1;

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;

    void Awake()
    {
        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();
    }

    public void UpdateFrequency(float value)
    {
        if (busSpawner != null)
            busSpawner.ChangeSpawningFrequency(value);

        if (sliderValueText != null)
            sliderValueText.text = value.ToString();
    }

    public void AddNewBus()
    {
        busSpawner.SpawnNewBus();
    }

    public void ActivateOption1(bool active)
    {
        currentOptionActive = 1;
        ChangeStationState(stationOption1, active);

        if (driebergenParking != null)
            driebergenParking.SetActive(true);
    }

    public void ActivateOption2(bool active)
    {
        currentOptionActive = 2;
        ChangeStationState(stationOption2, active);

        if (driebergenParking != null)
            driebergenParking.SetActive(false);
    }

    public void ChangeStationState(StationController stationController, bool value)
    {
        if (stationController != null)
        {
            stationController.GetComponentInParent<RoutingController>().SetStationOutOfService(stationController, value);
            stationController.SendMessage("UpdateStationMaterial", value);
        }
    }

    public int GetCurrentOptionActive()
    {
        return currentOptionActive;
    }
}
