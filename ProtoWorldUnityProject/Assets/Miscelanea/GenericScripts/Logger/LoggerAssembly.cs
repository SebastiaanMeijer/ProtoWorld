/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * SESTAR INTEGRATION
 * LoggerAsembly.cs
 * Johnson Ho
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

// Asembly line to define the config file path for the logger (log4net). 
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "UnityLog4Net.config", Watch = true)]

/// <summary>
/// This script must be attached to a gameobject in order to run the assembly line of the logger.
/// </summary>
public class LoggerAssembly : Singleton<LoggerAssembly>
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;

    private static int logSeriesCounter = 0;

    public bool logTimeController = true;
    public bool logCameraChanges = true;
    public bool logPedestrians = true;
    public bool logArrived = true;
    public bool logTraveling = true;
    public bool logQueuing = true;
    public bool logVehicles = true;
    public bool logVVis = true;
    public bool logDragAndDrop = true;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();
        log.Info("Logger initiated.");
    }

    void OnLevelWasLoaded(int level)
    {
        string str = level.ToString();
        //Debug.Log("(LoggerAssembly) Level loaded: " + str);
        log.Info("Level: " + level.ToString() + " loaded.");
    }

    void OnApplicationQuit()
    {
        log.Info("Logger ends.");
    }

    public static int GetLogSeriesId()
    {
        return logSeriesCounter++;
    }

    public static T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}
