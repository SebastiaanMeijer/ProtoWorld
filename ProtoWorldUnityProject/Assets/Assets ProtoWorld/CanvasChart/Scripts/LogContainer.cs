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
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.Linq;
using System.Text;

/// <summary>
/// title, chart, legend, action, intData, floatData, boolData, stringData
/// </summary>
public enum Keywords { title, chart, legend, action, intData, floatData, boolData, stringData }

[XmlRoot("LogContainer")]
public class LogContainer
{
    /// <summary>
    /// The main dictionary that holds all log-collections.
    /// </summary>
    Dictionary<int, TimedDataSeriesContainer> containers = new Dictionary<int, TimedDataSeriesContainer>();

    /// <summary>
    /// The path to the log file.
    /// </summary>
    [XmlAttribute]
    public string LogFilePath { get; set; }

    /// <summary>
    /// A list of all log-collections, converted from the main dictionary.
    /// </summary>
    [XmlArray("LogEntries")]
    public List<TimedDataSeriesContainer> seriesList { get { return containers.Values.ToList(); } }

    /// <summary>
    /// Check if logContainer is reading a log-file with the correct format.
    /// </summary>
    /// <param name="logLines"></param>
    /// <returns></returns>
    bool IsCorrectVersion(string[] logLines)
    {
        foreach (var line in logLines)
        {
            string message = GetLogMessage(line);
            if (message.Contains("Logger initiated"))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Read the log-file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool ReadLog(string path)
    {
        string[] logLines = File.ReadAllLines(path);

        if (!IsCorrectVersion(logLines))
        {
            return false;
        }

        LogFilePath = path;

        foreach (var line in logLines)
        {
            float time = GetLogTime(line);
            string message = GetLogMessage(line);
            int containerId = GetLogCollectionId(message);
            // There could be other logged messages, not in timedData-format.
            if (containerId < 0)
                continue;
            int logSeriesIndex = GetLogSeriesIndex(message);
            string logData = GetLogData(message);
            switch (GetKeyWord(message))
            {
                case Keywords.title:
                    if (!containers.ContainsKey(containerId))
                    {
                        //int idx = message.LastIndexOf(":");
                        //string data = message.Substring(idx);

                        var container = new TimedDataSeriesContainer()
                        {
                            Title = logData
                        };
                        containers.Add(containerId, container);
                        //Debug.Log("ContainerId: " + containerId);
                    }
                    break;
                case Keywords.chart:
                    if (logSeriesIndex > -1)
                    {
                        containers[containerId].SetChartType(
                            logSeriesIndex, (UIChartTypes)Enum.Parse(typeof(UIChartTypes), logData));
                    }
                    break;
                case Keywords.legend:
                    if (logSeriesIndex > -1)
                    {
                        containers[containerId].SetLegend(logSeriesIndex, logData);
                        //Debug.Log("cid: " + containerId + " sid: " + logSeriesIndex + " leg: " + logData);
                    }
                    break;
                case Keywords.action:
                case Keywords.stringData:
                    containers[containerId].Add(logSeriesIndex, time, logData);
                    break;
                case Keywords.intData:
                case Keywords.floatData:
                    float logValue;
                    if (float.TryParse(logData, out logValue))
                    {
                        containers[containerId].Add(logSeriesIndex, time, logValue);
                    }
                    break;
                case Keywords.boolData:
                    // Not implemented yet.
                    break;
            }
        }
        Debug.Log("ReadLog finished: There are " + containers.Count + " log collections.");
        return true;
    }

    /// <summary>
    /// Get the id of the dataseries in a collection.
    /// </summary>
    /// <param name="logMessage"></param>
    /// <returns></returns>
    static int GetLogSeriesIndex(string logMessage)
    {
        //Debug.Log(logMessage);

        var result = logMessage.Substring(0, logMessage.LastIndexOf(":"));
        result = result.Substring(result.LastIndexOf(":") + 1);
        int i;
        if (int.TryParse(result, out i))
        {
            return i;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Read the message that is logged.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static string GetLogMessage(string line)
    {
        return line.Substring(line.IndexOf("-") + 2);
    }

    /// <summary>
    /// Get the time stamp in milliseconds.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static float GetLogTime(string line)
    {
        float time;
        if (float.TryParse(line.Substring(0, line.IndexOf(" ")), out time))
        {
            return time;
        }
        return -1;
    }

    /// <summary>
    /// Get the collection id.
    /// </summary>
    /// <param name="logMessage"></param>
    /// <returns></returns>
    static int GetLogCollectionId(string logMessage)
    {
        //Debug.Log(logMessage);
        int index = logMessage.IndexOf(":");
        if (index > 0)
        {
            var result = logMessage.Substring(0, index);
            int i;
            if (int.TryParse(result, out i))
            {
                return i;
            }
            else
            {
                return -1;
            }
        }
        else
            return -1;
    }

    /// <summary>
    /// Get the keyword in the logged message.
    /// </summary>
    /// <param name="logMessage"></param>
    /// <returns></returns>
    static Keywords GetKeyWord(string logMessage)
    {
        //Debug.Log(logMessage);

        Dictionary<string, Keywords> exceptStrings = new Dictionary<string, Keywords>
        {
            { "int", Keywords.intData },
            { "float", Keywords.floatData },
            { "bool", Keywords.boolData },
            { "string", Keywords.stringData }
        };

        var splits = logMessage.Split(':');
        var keyString = splits[1].Split(' ')[0];

        //Debug.Log( keyString);

        Keywords keyword;

        if (exceptStrings.TryGetValue(keyString, out keyword))
            return keyword;
        else
        {
            return (Keywords)Enum.Parse(typeof(Keywords), keyString);
        }
    }

    /// <summary>
    /// Get the logged message.
    /// </summary>
    /// <param name="logMessage"></param>
    /// <returns></returns>
    static string GetLogData(string logMessage)
    {
        return logMessage.Substring(logMessage.LastIndexOf(":") + 1);
    }


    /// <summary>
    /// Serialize the logContainer into xml.
    /// </summary>
    /// <param name="path"></param>
    public void SaveAsXML(string path)
    {
        Type[] extraTypes = { typeof(TimedValue), typeof(TimedAction) };
        //List<TimedDataSeriesContainer> entries = containers.Values.ToList();
        //Debug.Log("Serializing: " + entries.Count + " entries");
        XmlSerializer serializer = new XmlSerializer(typeof(LogContainer), extraTypes);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            TextWriter writer = new StreamWriter(stream, new UTF8Encoding());
            serializer.Serialize(writer, this);
        }
        Debug.Log("Finished serializing: " + containers.Count + " entries");

    }
}
