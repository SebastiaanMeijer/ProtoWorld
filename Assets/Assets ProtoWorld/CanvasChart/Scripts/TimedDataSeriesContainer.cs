using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// TimedDataSeriesContainer is a collection of one or several dataseries.
/// </summary>
public class TimedDataSeriesContainer
{

    /// <summary>
    /// Title of the collection of the dataseries.
    /// </summary>
    [XmlAttribute]
    public string Title { get; set; }

    /// <summary>
    /// The collection of the dataseries as a list (convert from the main dictionary).
    /// </summary>
    [XmlArray("DataSeries")]
    public List<TimedDataSeries> seriesList { get { return container.Values.ToList(); } }

    /// <summary>
    /// The main collection of the dataseries as a dictionary.
    /// </summary>
    Dictionary<int, TimedDataSeries> container = new Dictionary<int, TimedDataSeries>();

    /// <summary>
    /// Set and Get number of series. It resets the container when setting a new value.
    /// </summary>
    public int SeriesCount
    {
        get { return container.Count; }
        set
        {
            container.Clear();
            for (int i = 0; i < value; i++)
            {
                container.Add(i, new TimedDataSeries());

            }
        }
    }

    /// <summary>
    /// Get the min max X- and Y-values of all series.
    /// </summary>
    public Rect MinMaxOfAll
    {
        get
        {
            return new Rect()
            {
                xMin = GetTotalMinTime(),
                xMax = GetTotalMaxTime(),
                yMin = GetTotalMinValue(),
                yMax = GetTotalMaxValue()
            };
        }
    }

    /// <summary>
    /// Set the i-th series name.
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <param name="name"></param>
    public void SetSeriesName(int seriesIndex, string name)
    {
        TimedDataSeries series;
        // Add a dictionary if dictionary not exists.
        if (!container.ContainsKey(seriesIndex))
        {
            container.Add(seriesIndex, new TimedDataSeries());
        }
        // Get the dictionary and add the timedAction to it.
        if (container.TryGetValue(seriesIndex, out series))
        {
            series.Name = name;
            //Debug.Log("name set: " + name);
        }

    }

    /// <summary>
    /// Get the i-th series name.
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <returns></returns>
    public string GetSeriesName(int seriesIndex)
    {
        TimedDataSeries series;
        if (container.TryGetValue(seriesIndex, out series))
        {
            return series.Name;
        }
        else
        {
            return seriesIndex.ToString();
        }
    }



    /// <summary>
    /// Set the number of samples for all series.
    /// </summary>
    /// <param name="capacity"></param>
    public void SetCapacity(int capacity)
    {
        for (int i = 0; i < container.Count; i++)
        {
            SetCapacity(i, capacity);
        }
    }

    /// <summary>
    /// Set the number of samples for i-th series.
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <param name="capacity"></param>
    public void SetCapacity(int seriesIndex, int capacity)
    {
        if (seriesIndex < container.Count)
        {
            container[seriesIndex].Capacity = capacity;
        }
    }

    /// <summary>
    /// Get the number of samples for i-th series.
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <returns></returns>
    public int GetCapacity(int seriesIndex)
    {
        if (seriesIndex < container.Count)
        {
            return container[seriesIndex].Capacity;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Add the time-data value-pair to the end of the array (queue)
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <param name="time"></param>
    /// <param name="value"></param>
    public void Add(int seriesIndex, float time, float value)
    {
        TimedDataSeries series;
        // Add a dictionary if dictionary not exists.
        if (!container.ContainsKey(seriesIndex))
        {
            container.Add(seriesIndex, new TimedDataSeries());
        }
        // Get the dictionary and add the timedAction to it.
        if (container.TryGetValue(seriesIndex, out series))
        {
            series.Add(time, value);
        }

    }

    /// <summary>
    /// Add the time-action value-pair to the end of the array (queue)
    /// </summary>
    /// <param name="seriesIndex"></param>
    /// <param name="time"></param>
    /// <param name="action"></param>
    public void Add(int seriesIndex, float time, string action)
    {
        TimedDataSeries series;
        // Add a dictionary if dictionary not exists.
        if (!container.ContainsKey(seriesIndex))
        {
            container.Add(seriesIndex, new TimedDataSeries());
        }
        // Get the dictionary and add the timedAction to it.
        if (container.TryGetValue(seriesIndex, out series))
        {
            series.Add(time, action);
        }

    }

    public void SetTimedDataCollection(int seriesIndex, List<TimedData> list)
    {
        if (seriesIndex < container.Count)
        {
            container[seriesIndex].Set(list);
        }
    }

    public List<TimedData> GetTimedDataCollection(int seriesIndex)
    {
        TimedDataSeries series;
        if (container.TryGetValue(seriesIndex, out series))
        {
            return series.TimedDataList;
        }

        return null;
    }

    public TimedData GetFirstTimedData(int seriesIndex)
    {
        TimedDataSeries series;
        if (container.TryGetValue(seriesIndex, out series))
        {
            return series.GetFirstTimedData();
        }
        else
        {
            return null;
            //return new TimedData() { time = -1, value = -1 };
        }
    }

    public float GetFirstDataTime(int seriesIndex)
    {
        TimedDataSeries series;
        if (container.TryGetValue(seriesIndex, out series))
        {
            return series.GetFirstDataTime();
        }
        else
        {
            return -1;
        }
    }

    public TimedData GetLastTimedData(int seriesIndex)
    {
        TimedDataSeries series;
        if (container.TryGetValue(seriesIndex, out series))
        {
            return series.LastTimedData;
        }
        else
        {
            return null;
            //return new TimedData() { time = -1, value = -1 };
        }
    }

    public float GetLastDataTime(int seriesIndex)
    {
        TimedDataSeries series;
        if (container.TryGetValue(seriesIndex, out series))
        {
            return series.LastDataTime;
        }
        else
        {
            return -1;
        }
    }

    public float GetTotalMinValue()
    {
        float min = float.MaxValue;
        foreach (var item in container.Values)
        {
            float value = item.GetMinValue();
            if (!float.IsNaN(value) && value < min)
                min = value;
        }
        return min;
    }

    public float GetTotalMaxValue()
    {
        float max = float.MinValue;
        foreach (var item in container.Values)
        {
            float value = item.GetMaxValue();
            if (!float.IsNaN(value) && value > max)
                max = value;
        }
        return max;
    }

    public float GetTotalMinTime()
    {
        float min = float.MaxValue;
        foreach (var series in container.Values)
        {
            float time = series.GetFirstDataTime();
            if (time < min)
                min = time;
        }
        return min;
    }

    public float GetTotalMaxTime()
    {
        float max = float.MinValue;
        foreach (var series in container.Values)
        {
            float time = series.LastDataTime;
            if (time > max)
                max = time;
        }
        return max;
    }

    public void SetChartType(int seriesIndex, UIChartTypes chartType)
    {
        TimedDataSeries series;
        // Add a dictionary if dictionary not exists.
        if (!container.ContainsKey(seriesIndex))
        {
            container.Add(seriesIndex, new TimedDataSeries());
        }
        if (container.TryGetValue(seriesIndex, out series))
        {
            series.ChartType = chartType;
        }
    }

    public void SetLegend(int seriesIndex, string legend)
    {
        TimedDataSeries series;
        // Add a dictionary if dictionary not exists.
        if (!container.ContainsKey(seriesIndex))
        {
            container.Add(seriesIndex, new TimedDataSeries());
        }
        if (container.TryGetValue(seriesIndex, out series))
        {
            series.Name = legend;

        }
    }

}