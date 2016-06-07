/*
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

    public bool logPedestrians;
    public bool logArrived;
    public bool logTraveling;
    public bool logQueuing;
    public bool logVehicles;


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
