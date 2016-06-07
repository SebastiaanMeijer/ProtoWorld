using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// TimedDataSeries holds a series of data
/// </summary>
public class TimedDataSeries
{
    /// <summary>
    /// The queue that stores the time-data.
    /// </summary>
    Queue<TimedData> DataSeries { get; set; }

    /// <summary>
    /// Set the capacity of the queue.
    /// </summary>
    [XmlIgnoreAttribute]
    public int Capacity { get; set; }

    /// <summary>
    /// Return the dataQueue as a List.
    /// </summary>
    [XmlArray("DataEntries")]
    public List<TimedData> TimedDataList { get { return DataSeries.ToList<TimedData>(); } }

    /// <summary>
    /// A legend to this data queue.
    /// </summary>
    [XmlAttribute]
    public string Name { get; set; }

    /// <summary>
    /// The Chart Type if defined.
    /// </summary>
    [XmlAttribute]
    public UIChartTypes ChartType { get; set; }

    /// <summary>
    /// The latest added time-data.
    /// </summary>
    [XmlIgnoreAttribute]
    public TimedData LastTimedData { get; set; }

    /// <summary>
    /// Get the time in the latest added time-data.
    /// </summary>
    [XmlIgnoreAttribute]
    public float LastDataTime { get { return LastTimedData.time; } }

    /// <summary>
    /// Constructor with an initial capacity of 2000.
    /// </summary>
    public TimedDataSeries() : this(2000)
    {
    }

    /// <summary>
    /// Constructor with the given capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public TimedDataSeries(int capacity)
    {
        DataSeries = new Queue<TimedData>();
        Capacity = capacity;
        Name = "";

        // Set the first point/ last point as (0,0)
        Add(0, 0);
        //LastTimedData = new TimedValue
        //{
        //    time = 0,
        //    value = 0
        //};
    }

    /// <summary>
    /// Queue the time-data value pair.
    /// If the queue is longer than the defined capacity,
    /// it will start to dequeue.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="value"></param>
    public void Add(float time, float value)
    {
        TimedValue t = new TimedValue();
        t.value = value;
        t.time = time;

        DataSeries.Enqueue(t);
        LastTimedData = t;
        while (DataSeries.Count > Capacity)
        {
            DataSeries.Dequeue();
        }
    }

    /// <summary>
    /// Queue the time-action value pair.
    /// If the queue is longer than the defined capacity,
    /// it will start to dequeue.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="action"></param>
    public void Add(float time, string action)
    {
        TimedAction t = new TimedAction();
        t.time = time;
        t.action = action;

        DataSeries.Enqueue(t);
        LastTimedData = t;
        while (DataSeries.Count > Capacity)
        {
            DataSeries.Dequeue();
        }
    }

    /// <summary>
    /// Copy the values from the time-data list to the queue.
    /// This also set the capacity of the queue based on the list count.
    /// </summary>
    /// <param name="list"></param>
    public void Set(List<TimedData> list)
    {
        DataSeries = new Queue<TimedData>(list);
        Capacity = DataSeries.Count;
        LastTimedData = list[Capacity - 1];
    }

    /// <summary>
    /// Get the maximum time value (X-axis) in the series.
    /// </summary>
    /// <returns></returns>
    public TimedData GetFirstTimedData()
    {
        return DataSeries.Peek();
    }

    /// <summary>
    /// Get the minimum time value (X-axis) in the series.
    /// </summary>
    /// <returns></returns>
    public float GetFirstDataTime()
    {
        return GetFirstTimedData().time;
    }

    /// <summary>
    /// Get the minimum data value (Y-axis) in the series.
    /// </summary>
    /// <returns></returns>
    public float GetMinValue()
    {
        if (DataSeries.Count > 0)
            return DataSeries.Min(x => x.GetData());
        else
            return float.NaN;
    }

    /// <summary>
    /// Get the maximum data value (Y-axis) in the series.
    /// </summary>
    /// <returns></returns>
    public float GetMaxValue()
    {
        if (DataSeries.Count > 0)
            return DataSeries.Max(x => x.GetData());
        else
            return float.NaN;
    }
}