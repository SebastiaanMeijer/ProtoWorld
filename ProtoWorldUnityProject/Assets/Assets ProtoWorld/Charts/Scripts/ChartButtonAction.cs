/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChartButtonAction : MonoBehaviour {

    struct TimeStampedValue
    {
        public float time;
        public float value;
    }

    string testPath = @"C:\Users\admgaming\Documents\GapslabsProject\bin";
    string[] lines = new string[0];
    
    public GameObject prefabEventButton;
    public GameObject lineChartPrefab;
    public GameObject stackedAreaChartPrefab;
    public GameObject[] chartRectObjects;
    
    float buttonPosition = -5;

    TimeChart[] timeCharts;

    Dictionary<string, List<string>> messageDictionary = new Dictionary<string, List<string>>();

    public void HandleSlider()
    {
        GameObject slider = GameObject.Find("Slider");
        MoveTimeIndicator(slider.GetComponent<Slider>().value);
    }

    private void MoveTimeIndicator(float value)
    {
        foreach (var chart in chartRectObjects)
        {
            TimeChart timeChart = chart.GetComponent<TimeChart>(); // Funkar det?
            if (timeChart!=null)
                timeChart.TimeSliderPosition = value;
        }
    }

    public void LoadIntroCanvas()
    {
        Application.LoadLevel(0);
    }

    void start()
    {
    }

    // Maybe pre-manufactured indicators and activates one of them to 
    // indicate when the event occured in relation to the chart.
    // Each button has an associated index.
    public void ClickToChangeTimeIndicator(int idx)
    {
        
    }

    // Maybe only one indicator that is moved to xPos of the chart.
    // Each button has a timestamp.
    // xPos = timeStamp/totalTime
    public void ClickToChangeTimeIndicator(float timeStamp)
    {

    }


    public void ClickLoadLogFile()
    {
#if UNITY_EDITOR

        string path = EditorUtility.OpenFilePanel("Open Log File", testPath, "log");
        if (path.Length > 0)
        {
            lines = File.ReadAllLines(path);
        }
                
        List<string> timeStrings = ExtractEventsAndMakeButtons(lines, "CheckKeyDown");
        SortDataFromLines(lines, "Update");
        MakeCharts(lines, "Start");
        PutDataInChart();
        MakeEventIndicators(timeStrings);

#endif
    }

    /// <summary>
    /// Put the data in dataDict to the corresponding chart
    /// </summary>
    private void PutDataInChart()
    {
        // 0 = chartIdx
        // 1 = seriesIdx
        // 2 = value
        //265   [1]  INFO LineChart UpdateData - 1:0:0 (Old)
        //637   [1]  INFO KPIManager Update - 0:0:0 

        foreach (var item in messageDictionary)
        {
            string[] key = item.Key.Split(':');
            TimeChart chart = timeCharts[int.Parse(key[0])];
            int chartIdx = int.Parse(key[1]);
            chart.SetDataSeriesCount(chartIdx + 1);
            float[] values = new float[item.Value.Count];
            int counter = 0;
            foreach (string str in item.Value)
            {
                float time = float.Parse(GetLogTime(str));
                values[counter++] = float.Parse(GetLogValue(str));
                chart.StartTime = time;
                chart.EndTime = time;
            }
            chart.SetData(chartIdx, values);
        }
    }

    /// <summary>
    /// Make indicators over the charts to indicate when an event has occured.
    /// </summary>
    /// <param name="timeStrings">A list of time stamps in string</param>
    private void MakeEventIndicators(List<string> timeStrings)
    {

    }

    /// <summary>
    /// Reconstruct the graphs from the log.
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="methodStr"></param>
    void MakeCharts(string[] lines, string methodStr = "InitChart")
    {
        // Make sure that the prefabs are there!

        timeCharts = new TimeChart[chartRectObjects.Length];

        foreach (var line in lines)
        {
            if (line.Contains(methodStr))
            {

                //600[1]  INFO KPIManager Start - 3:lineChart 0

                string str = GetLogMessage(line);
                string[] values = str.Split(new Char[] { ':', ' ' });
                if (line.Contains("lineChart"))
                {
                    CreateNewChart(ChartTypes.Line, int.Parse(values[2]));

                }
                else if (line.Contains("stackedAreaChart"))
                {
                    CreateNewChart(ChartTypes.StackedArea, int.Parse(values[2]));
                }
            }
        }
    }

    /// <summary>
    /// Gather messages generate from the same KPIManager/Chart for later parsing
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="methodStr"></param>
    private void SortDataFromLines(string[] lines, string methodStr = "UpdateData")
    {
        // 0 = chartIdx
        // 1 = seriesIdx
        // 2 = value
        //265   [1]  INFO LineChart UpdateData - 1:0:0 (Old)
        //637   [1]  INFO KPIManager Update - 0:0:0 
        foreach (var line in lines)
        {
            
            if (line.Contains(methodStr))
            {
                string str = GetLogMessage(line);
                string[] values = str.Split(':');
                string id = String.Concat(values[0], ":", values[1]);
                List<string> list;
                if (!messageDictionary.TryGetValue(id, out list))
                {
                    list = new List<string>();
                    messageDictionary.Add(id, list);
                }
                list.Add(line);


            }
        }
    }

    /// <summary>
    /// Creat a LineChart or a StackedAreaChart and put it in the index-th position.
    /// Make sure the Scene has enough RectTransforms as the number of charts logged.
    /// </summary>
    /// <param name="chartType"></param>
    /// <param name="index"></param>
    private void CreateNewChart(ChartTypes chartType, int index)
    {
        if (index > chartRectObjects.Length)
        {
            Debug.LogError("Too many charts for this scene!");
            return;
        }

        if (timeCharts[index] != null)
        {
            Debug.Log("Combine graphs detected, no need for a new chart...");
            return;
        }

        RectTransform transform = (RectTransform)chartRectObjects[index].transform;
        GameObject chart;
        switch (chartType)
        {
            case ChartTypes.Line:
                chart = Instantiate(lineChartPrefab, transform.localPosition, Quaternion.identity) as GameObject;
                LineChart lineChart = chart.GetComponent<LineChart>();
                lineChart.InitChart(); // Important to call InitChart!               
                timeCharts[index] = lineChart; // Maybe not needed!
                break;
            case ChartTypes.StackedArea:
                chart = Instantiate(stackedAreaChartPrefab, transform.localPosition, Quaternion.identity) as GameObject;
                StackedAreaChart areaChart = chart.GetComponent<StackedAreaChart>();
                areaChart.InitChart(); // Important to call InitChart!
                timeCharts[index] = areaChart; // Maybe not needed!
                break;
            default:
                Debug.LogWarning("Chart type not avaiable right now.");
                return;

        }
        RectTransform chartTransform = (RectTransform)chart.transform;
        chartTransform.sizeDelta = transform.sizeDelta;
        chartTransform.SetParent(transform.parent);
        chartTransform.pivot = transform.pivot;
        chartTransform.localPosition = transform.localPosition;

        chartRectObjects[index] = chart;

        Destroy(transform.gameObject);

    }
    
    /// <summary>
    /// Extract events from the loaded file, create buttons ..
    /// and return timestamps as a list of strings
    /// </summary>
    /// <param name="lines">Lines that made up the loaded file.</param>
    /// <param name="methodStr">Keyword that indicates an event.</param>
    List<string> ExtractEventsAndMakeButtons(string[] lines, string methodStr)
    {
        GameObject panel = GameObject.Find("EventButtonPanel");
        Vector3 startPosition = panel.transform.position;
        List<string> timeStrings = new List<string>();
        foreach (var line in lines)
        {
            if (line.Contains(methodStr))
            {
                GameObject button = Instantiate(prefabEventButton, startPosition, Quaternion.identity) as GameObject;
                button.transform.SetParent(panel.transform);
                button.transform.localPosition = new Vector3(0, buttonPosition);
                buttonPosition -= 35; // TO CHECK: Button get bounds ?

                UnityEngine.UI.Text buttonText = button.GetComponentInChildren<UnityEngine.UI.Text>();
                string timeString = GetLogTime(line);
                buttonText.text = timeString + ": " + GetLogMessage(line);
                timeStrings.Add(timeString);
                
            }
        }
        return timeStrings;
    }

    /// <summary>
    /// Change this will change the way to read the log message.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static string GetLogMessage(string line)
    {
        return line.Substring(line.LastIndexOf("-") + 2);
    }

    static string GetLogValue(string line)
    {
        return line.Substring(line.LastIndexOf(":") + 1);
    }

    /// <summary>
    /// Change this will change the way to read the timestamp.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static string GetLogTime(string line)
    {
        return line.Substring(0, line.IndexOf(" "));
    }

}
