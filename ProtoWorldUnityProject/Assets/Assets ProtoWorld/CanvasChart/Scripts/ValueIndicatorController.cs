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

public class ValueIndicatorController : MonoBehaviour
{
    private ChartController chartController { get; set; }
    private Slider valueIndicator;
    private Text valueButtonText;
    private GameObject valueHandleArea;

    public float Value
    {
        get { return valueIndicator.value; }
    }

    public float MinTime
    {
        get { return valueIndicator.minValue; }
        set { valueIndicator.minValue = value; }
    }

    public float MaxTime
    {
        get { return valueIndicator.maxValue; }
        set { valueIndicator.maxValue = value; }
    }

    // Use this for initialization
    void Start()
    {
        chartController = GetComponentInParent<ChartController>();
        valueIndicator = GetComponent<Slider>();
        valueHandleArea = transform.FindChild("Handle Slide Area").gameObject;
        valueButtonText = transform.FindChild("Handle Slide Area/Handle/ValueTimeButton/Text").GetComponent<Text>();
        SetActive(false);
    }

    void LateUpdate()
    {
        if (chartController.streaming)
        {
            SetTime(chartController.GetTotalMaxTime());
        }
        switch (chartController.chartType)
        {
            case UIChartTypes.Bar:
            case UIChartTypes.Pie:
                SetActive(false);
                break;
            case UIChartTypes.Line:
            case UIChartTypes.StackedArea:
                SetActive(true);
                break;
        }
    }

    public void SetActive(bool active)
    {
        valueHandleArea.SetActive(active);
    }

    public void SetTime(float time)
    {
        valueIndicator.value = time;
        SetText(time);
    }

    /// <summary>
    /// Used by the slider "On Value Changed"
    /// </summary>
    public void HandleValueChanged()
    {
        float time = valueIndicator.value;
        chartController.UpdateValues((time - MinTime) / (MaxTime - MinTime));
        SetText(valueIndicator.value);
        if (time > 0)
        {

        }
    }


    public void SetText(float time)
    {
        SetActive(true);
        //valueButtonText.text = time.ToString() + " s
        if (time > 0)
            valueButtonText.text = ChartUtils.SecondsToTime(time);
    }
}
