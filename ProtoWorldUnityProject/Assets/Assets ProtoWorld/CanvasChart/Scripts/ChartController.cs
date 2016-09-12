/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * KPI MODULE
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Main controller for the whole chart.
/// </summary>
/// <remarks>
/// Useful method: AddTimedData(int listIndex, float time, float value)
/// to add time-data to a certain series.
/// </remarks>
public class ChartController : MonoBehaviour
{
    /// <summary>
    /// To keep track of the number of charts in scene.
    /// </summary>
    private static int chartCounter = 0;

    /// <summary>
    /// The id of this chart.
    /// </summary>
    public int chartId;

    /// <summary>
    /// The name, appears in the toolbar.
    /// </summary>
    public string name;

    /// <summary>
    /// Whether this is a streaming chart or a static chart. 
    /// </summary>
    public bool streaming = false;

    /// <summary>
    /// It can be a bar, pie, line or stacked area chart.
    /// </summary>
    public UIChartTypes chartType;

    /// <summary>
    /// In streaming chart:
    /// Use internally for sampling update.
    /// </summary>
    private float timer;

    /// <summary>
    /// In streaming chart:
    /// The sampling rate in seconds.
    /// </summary>
    public float updateRate = 1;

    /// <summary>
    /// In streaming chart:
    /// How many data points to be visualized.
    /// </summary>
    [Range(2, 1000)] public int numberOfSamples = 100;

    /// <summary>
    /// How the Y-values are formated.
    /// </summary>
    public string specifier = "#,0.0";

    /// <summary>
    /// In streaming chart:
    /// Set the preferred maximum Y-value.
    /// This value will increase if maxYCanOnlyIncrease is set to true.
    /// </summary>
    public float preferredMaxYValue = 100;

    /// <summary>
    /// In streaming chart:
    ///  User can set whether MaxY can only Increase or AutoUpdate (increase or decrease)
    /// </summary>
    public bool maxYCanOnlyIncrease = false;

    /// <summary>
    /// Used internally to hold the current Max Y value of all series.
    /// </summary>
    private float currentMaxY;

    /// <summary>
    /// The data.
    /// </summary>
    public TimedDataSeriesContainer DataContainer = new TimedDataSeriesContainer();

    /// <summary>
    /// Colors for the different series.
    /// </summary>
    public Color32[] seriesColors = new Color32[] {Color.blue, Color.green, Color.red, Color.magenta, Color.black};

    /// <summary>
    /// Names for the different series.
    /// </summary>
    public string[] seriesNames = new string[5];

    /// <summary>
    /// Used by this and the axisController to set the colors of the series
    /// and the background of the legends.
    /// </summary>
    [HideInInspector] public Material[] materials = new Material[5];

    /// <summary>
    /// Used by the valueIndicator. 
    /// </summary>
    [HideInInspector] public float[] values;

    /// <summary>
    /// Every series has its own update timer.
    /// </summary>
    public float[] updateTimers;

    /// <summary>
    /// Used by the valueIndicator.
    /// </summary>
    [HideInInspector] public float valueTime;

    /// <summary>
    /// An indicator that shows when a logged event occured in relation to the time-data series.
    /// </summary>
    private EventIndicatorController eventIndicatorView;

    /// <summary>
    /// An indicator that shows the values at a certain time in the time-data series.
    /// </summary>
    private ValueIndicatorController valueIndicatorView;

    /// <summary>
    /// Get and set the number of series the chart will use.
    /// This will also update the size of the value array (used by valueIndicator).
    /// </summary>
    public int SeriesCount
    {
        get { return DataContainer.SeriesCount; }
        set
        {
            //seriesCount = value; // just for reference.
            DataContainer.SeriesCount = value;
            UpdateArraysLength(value);
            //Debug.Log("series count set!");
        }
    }

    //public int seriesCount; // just for reference.

    /// <summary>
    /// Whether we should generate random data for testing.
    /// </summary>
    public bool testing = false;

    /// <summary>
    /// Number of series used in testing.
    /// </summary>
    public int numberOfSeries = 0;

    /// <summary>
    /// Upper bound of the random value.
    /// </summary>
    public float randomSeedUpper = 20;

    /// <summary>
    /// Lower bound of the random value.
    /// </summary>
    public float randomSeedLower = -20;


    /// <summary>
    /// Minimize button
    /// </summary>
    public GameObject contentPanel;

    void Awake()
    {
        this.chartId = chartCounter++;
        //DataContainer = new TimedDataSeriesContainer();
    }

    /// <summary>
    /// Set the start time, init the TimedDataQueueList 
    /// and find the indicators.
    /// </summary>
    void Start()
    {
        timer = Time.time;

        //eventIndicatorView = transform.Find("ChartView/ChartHolder/EventIndicator").GetComponent<EventIndicatorController>();
        //valueIndicatorView = transform.Find("ChartView/ChartHolder/ValueIndicator").GetComponent<ValueIndicatorController>();

        eventIndicatorView = GetComponentInChildren<EventIndicatorController>();
        valueIndicatorView = GetComponentInChildren<ValueIndicatorController>();
    }

    /// <summary>
    /// If streaming chart, we update first the number of samples,
    /// else we update the min and max time in the indicators.
    /// For all cases, we will update the colors in the materials
    /// and the legend texts.
    /// </summary>
    void Update()
    {
        UpdateArraysLength(SeriesCount);
        UpdateMaterials();
        UpdateSeriesNameFromDataContainer();

        if (streaming)
        {
            UpdateNumberOfSamples();
            if (testing)
            {
                if (Time.time - timer > updateRate)
                {
                    GenerateStreamData();
                    timer = Time.time;
                }
            }
            for (int i = 0; i < values.Length; i++)
            {
                var data = DataContainer.GetLastTimedData(i);
                if (data != null)
                    values[i] = data.GetData();
            }
        }
        else
        {
            if (testing)
            {
                UseTestData();
                testing = false;
            }
            UpdateTimeInEventIndicator();
            UpdateTimeInValueIndicator();
        }
    }

    /// <summary>
    /// Update the array for values associated with valueIndicator.
    /// </summary>
    /// <param name="count"></param>
    void UpdateArraysLength(int count)
    {
        if (updateTimers.Length != count)
        {
            float[] newTimes = new float[count];
            for (int i = 0; i < count; i++)
            {
                if (i < updateTimers.Length)
                {
                    newTimes[i] = updateTimers[i];
                }
                else
                {
                    newTimes[i] = Time.time;
                }
            }
            updateTimers = newTimes;
        }

        if (values.Length != count)
        {
            float[] newValues = new float[count];
            for (int i = 0; i < count; i++)
            {
                if (i < values.Length)
                {
                    newValues[i] = values[i];
                }
                else
                {
                    newValues[i] = 0;
                }
            }
            values = newValues;
        }
    }

    /// <summary>
    /// Used by ChartKeeper to set the chart type.
    /// Does a check on whether the chart type is consistent for 2 series in the log.
    /// </summary>
    /// <param name="chartType"></param>
    public void SetChartType(UIChartTypes chartType)
    {
        if (this.chartType > 0 && this.chartType != chartType)
            Debug.LogError("Trying to put different ChartTypes into one chart!");
        else
            this.chartType = chartType;
    }

    /// <summary>
    /// Update the number of samples in every series.
    /// </summary>
    void UpdateNumberOfSamples()
    {
        if (chartType > UIChartTypes.Pie)
            DataContainer.SetCapacity(numberOfSamples);
        else
            DataContainer.SetCapacity(1);
    }

    public Color GetSeriesColor(int seriesIndex)
    {
        if (seriesIndex < seriesColors.Length)
            return seriesColors[seriesIndex];
        else
            return Color.gray;
    }

    public void SetSeriesColor(int seriesIndex, Color color)
    {
        if (seriesIndex >= seriesColors.Length)
        {
            var colors = new Color32[seriesIndex + 1];
            for (int i = 0; i < seriesColors.Length; i++)
            {
                colors[i] = seriesColors[i];
            }
            seriesColors = colors;
        }
        //Debug.Log(seriesIndex);
        seriesColors[seriesIndex] = color;
    }

    /// <summary>
    /// Update the colors of the materials.
    /// </summary>
    void UpdateMaterials()
    {
        if (materials.Length < seriesColors.Length)
            InitMaterials();
        for (int i = 0; i < seriesColors.Length; i++)
        {
            materials[i].color = seriesColors[i];
        }
    }

    /// <summary>
    /// Init the materials array with the same length as colors.
    /// </summary>
    void InitMaterials()
    {
        materials = new Material[seriesColors.Length];
        for (int i = 0; i < seriesColors.Length; i++)
        {
            materials[i] = new Material(Shader.Find("UI/Default"));
            materials[i].color = seriesColors[i];
        }
    }

    /// <summary>
    /// Generate test data for streaming view.
    /// </summary>
    void GenerateStreamData()
    {
        if (!streaming)
            return;
        SeriesCount = numberOfSeries;

        for (int i = 0; i < numberOfSeries; i++)
        {
            float rng = UnityEngine.Random.Range(randomSeedLower, randomSeedUpper);
            //Debug.Log("rng:" + rng);
            DataContainer.Add(i, Time.time, rng);
        }
    }

    /// <summary>
    /// Generate test data for static chart.
    /// </summary>
    void UseTestData()
    {
        SeriesCount = 2;
        for (int i = 0; i < numberOfSamples; i++)
        {
            DataContainer.Add(0, i, UnityEngine.Random.Range(randomSeedLower, randomSeedUpper));
        }
        SetChartType(UIChartTypes.Line);
        for (int i = 0; i < numberOfSamples; i++)
        {
            DataContainer.Add(1, i, UnityEngine.Random.Range(randomSeedLower, randomSeedUpper));
        }
    }

    public int RegisterNewKPI()
    {
        SeriesCount += 1;
        Debug.Log("Series count: " + SeriesCount);
        return SeriesCount - 1;
    }

    public void AddTimedData(int seriesIndex, float value)
    {
        AddTimedData(seriesIndex, Time.time, value);
    }

    /// <summary>
    /// Add time-data value pair to the i-th series.
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <param name="time"></param>
    /// <param name="value"></param>
    public void AddTimedData(int seriesIndex, float time, float value)
    {
        if (streaming)
        {
            //Debug.Log(seriesIndex);
            if (Time.time - updateTimers[seriesIndex] < updateRate)
            {
                return;
            }
            else
            {
                updateTimers[seriesIndex] = Time.time;
            }
        }

        DataContainer.Add(seriesIndex, time, value);
    }

    /// <summary>
    /// Used by ChartKeeper to set the series name from the log file.
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <param name="name"></param>
    public void SetSeriesName(int seriesIndex, string name)
    {
        if (seriesIndex >= seriesNames.Length)
        {
            var names = new string[seriesIndex + 1];
            for (int i = 0; i < seriesNames.Length; i++)
            {
                names[i] = seriesNames[i];
            }
            seriesNames = names;
        }
        DataContainer.SetSeriesName(seriesIndex, name);
    }

    /// <summary>
    /// Update the legends based on how many time-data series.
    /// </summary>
    void UpdateSeriesNameFromDataContainer()
    {
        for (int idx = 0; idx < DataContainer.SeriesCount; idx++)
        {
            var name = DataContainer.GetSeriesName(idx);
            //Debug.Log("name set: " + name);
            if (name.Length > 0)
                seriesNames[idx] = name;
        }
    }

    /// <summary>
    /// Initiate the eventIndicator by setting the mininum time and maximum time 
    /// of the chart.
    /// </summary>
    void UpdateTimeInEventIndicator()
    {
        eventIndicatorView.SetMinTime(DataContainer.GetFirstDataTime(0));
        eventIndicatorView.SetMaxTime(DataContainer.GetLastDataTime(0));
    }

    /// <summary>
    /// Method for static chart:
    /// Move the eventIndicator to the given time, used by ChartKeeper.
    /// </summary>
    /// <param name="time"></param>
    public void SetEventTime(float time)
    {
        eventIndicatorView.SetTime(time);
    }

    /// <summary>
    /// Initiate the valueIndicator by setting the mininum time and maximum time 
    /// of the chart.
    /// </summary>
    void UpdateTimeInValueIndicator()
    {
        valueIndicatorView.MinTime = DataContainer.GetFirstDataTime(0);
        valueIndicatorView.MaxTime = DataContainer.GetLastDataTime(0);
    }

    /// <summary>
    /// Method for static chart:
    /// While dragging the valueIndicator, the values shown next to the indicator
    /// will be updated from the loaded time-data series.
    /// </summary>
    /// <param name="relativePosition"></param>
    public void UpdateValues(float relativePosition)
    {
        for (int i = 0; i < SeriesCount; i++)
        {
            var dataCollection = DataContainer.GetTimedDataCollection(i);
            if (dataCollection.Count < 1)
                continue;

            int idx = Mathf.RoundToInt((dataCollection.Count - 1)*relativePosition);

            //Debug.Log("i: " + i + " count: " + dataCollection.Count);
            //Debug.Log("rel: " + relativePosition + " idx: " + idx);

            values[i] = dataCollection[idx].GetData();
        }
    }

    /// <summary>
    /// Method for static chart:
    /// Update the valueIndicator when the user pressed inside the chart area,
    /// the indicator will move to the cursor, time and values will be updated. 
    /// </summary>
    /// <param name="relativePosition"></param>
    public void UpdateValuesAndIndicator(float relativePosition)
    {
        if (chartType == UIChartTypes.Bar || chartType == UIChartTypes.Pie)
            return;
        if (relativePosition < 0 || relativePosition > 1)
            return;

        for (int i = 0; i < SeriesCount; i++)
        {
            var dataCollection = DataContainer.GetTimedDataCollection(i);
            int idx = Mathf.RoundToInt((dataCollection.Count - 1)*relativePosition);
            //Debug.Log("list count: " + list.Count + " idx: " + idx);
            values[i] = dataCollection[idx].GetData();
            valueTime = dataCollection[idx].time;
            valueIndicatorView.SetTime(valueTime);
        }
    }

    /// <summary>
    /// MinMaxOfAll from DataModel will be adjusted if maxYCanOnlyIncrease is set to true.
    /// If yMax is 0, it will set as the preferredMaxYValue.
    /// Important for ChartViewController and AxisController.
    /// </summary>
    /// <returns> Rect that store xMin, xMax, yMin, yMax </returns>
    public Rect GetMinMaxOfAll()
    {
        //for (int idx = 0; idx < SeriesCount; idx++)
        //{
        //    var dataCollection = DataContainer.GetTimedDataCollection(idx);
        //    if (dataCollection.Count < 2)
        //    {
        //        return new Rect { xMin = 0, xMax = 0, yMin = 0, yMax = preferredMaxYValue };
        //    }
        //}

        Rect minmax = DataContainer.MinMaxOfAll;
        if (chartType == UIChartTypes.StackedArea)
            minmax = DataContainer.MinMaxStacked;

        if (maxYCanOnlyIncrease)
            currentMaxY = (minmax.yMax > currentMaxY) ? minmax.yMax : currentMaxY;
        else
        {
            if (streaming)
                currentMaxY = (minmax.yMax < preferredMaxYValue) ? preferredMaxYValue : minmax.yMax;
            else
                currentMaxY = minmax.yMax;
        }
        minmax.yMax = currentMaxY;
        minmax.yMin = (minmax.yMin < 0) ? minmax.yMin : 0;
        return minmax;
    }

    /// <summary>
    /// Get the maximum value (Y-axis) in all series.
    /// </summary>
    /// <returns></returns>
    public float GetTotalMaxValue()
    {
        float yMax = DataContainer.GetTotalMaxValue();
        if (maxYCanOnlyIncrease)
            currentMaxY = (yMax > currentMaxY) ? yMax : currentMaxY;
        else
        {
            if (streaming)
                currentMaxY = (yMax < preferredMaxYValue) ? preferredMaxYValue : yMax;
            else
                currentMaxY = yMax;
        }
        return currentMaxY;
    }

    /// <summary>
    /// Get the minimmum value (Y-axis) in all series.
    /// </summary>
    /// <returns></returns>
    public float GetTotalMinValue()
    {
        return DataContainer.GetTotalMinValue();
    }

    /// <summary>
    /// Get the maximum time value (X-axis) in all series.
    /// </summary>
    /// <returns></returns>
    public float GetTotalMaxTime()
    {
        return DataContainer.GetTotalMaxTime();
    }

    /// <summary>
    /// Get the minimmum time value (X-axis) in all series.
    /// </summary>
    /// <returns></returns>
    public float GetTotalMinTime()
    {
        return DataContainer.GetTotalMinTime();
    }

    public void ToggleVisibility()
    {
        contentPanel.SetActive(!contentPanel.activeSelf);

        //Clear the CanvasRenderer, else the axis/lines remain visible.
        Transform content = this.transform.Find("Content");
        Transform chartView = content.Find("ChartView");
        Transform chartHolder = chartView.transform.Find("ChartHolder");

        CanvasRenderer[] renderers = chartHolder.gameObject.GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer canvasRenderer in renderers)
            canvasRenderer.Clear();
        
        Transform axisHolder = chartView.transform.Find("AxisHolder");
        CanvasRenderer renderer = axisHolder.gameObject.GetComponent<CanvasRenderer>();
        renderer.Clear();
    }




}