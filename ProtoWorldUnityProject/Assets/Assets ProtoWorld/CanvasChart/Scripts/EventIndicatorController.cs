/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * KPI MODULE
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventIndicatorController : MonoBehaviour {

    private ChartController controller;
    private Slider eventIndicator;
    private GameObject eventHandleArea;
    private Text eventButtonText;

    // Use this for initialization
    void Start () {
        controller = GetComponentInParent<ChartController>();
        eventIndicator = GetComponent<Slider>();
        eventHandleArea = transform.FindChild("Handle Slide Area").gameObject;
        eventButtonText = transform.FindChild("Handle Slide Area/Handle/EventButton/Text").GetComponent<Text>();
        SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetActive(bool active)
    {
        eventHandleArea.SetActive(active);
    }

    public void SetMinTime(float time)
    {
        eventIndicator.minValue = time;
    }

    public void SetMaxTime(float time)
    {
        eventIndicator.maxValue = time;
    }

    public void SetTime(float time)
    {
        //eventIndicator.value = time;
        //eventButtonText.text = time.ToString();
        SetActive(true);
        eventIndicator.value = Mathf.Clamp(time, eventIndicator.minValue, eventIndicator.maxValue);
        SetText(eventIndicator.value);
    }

    /// <summary>
    /// Used by the slider "On Value Changed"
    /// </summary>
    public void HandleValueChanged()
    {
        if (eventIndicator.value > 0)
        {
            SetText(eventIndicator.value);
        }
    }

    public void SetText(float time)
    {
        eventButtonText.text = ChartUtils.SecondsToTime(time);
    }

}
