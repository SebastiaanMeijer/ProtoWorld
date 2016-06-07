using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class KPIFeeder : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public UIChartTypes chartType = UIChartTypes.Bar;

    public List<GameObject> gameObjects = new List<GameObject>();
    [HideInInspector]
    public List<string> kpiStrings = new List<string>();
    [HideInInspector]
    public List<string> kpiNames = new List<string>();
    [HideInInspector]
    public List<Color> kpiColors = new List<Color>();
    //[HideInInspector]
    //public List<string> kpiTypes = new List<string>();
    [HideInInspector]
    public ChartController controller;

    void Start()
    {

        foreach (var s in kpiStrings)
        {
            log.Debug(s);
        }
        ApplySettings();
    }

    /// <summary>
    /// [0] = Gameobject name; [1] = Script name; [2] = Property/Field name; [3] = HashCode.
    /// </summary>
    /// <param name="kpiString"></param>
    /// <returns></returns>
    string[] SplitKPIString(string kpiString)
    {
        var split1 = kpiString.Split(' ');
        var objName = split1[0];
        var split2 = split1[1].Split(':');
        var scriptName = split2[0];
        scriptName = scriptName.Substring(1, scriptName.Length - 2);
        var split3 = split2[1].Split('.');
        var propName = split3[0];
        var hashCode = split3[1];

        return new string[] { objName, scriptName, propName, hashCode };
    }

    /// <summary>
    /// The KPIs are fed to the chart from here.
    /// </summary>
    void Update()
    {
        for (int i = 0; i < kpiStrings.Count; i++)
        {
            var split = SplitKPIString(kpiStrings[i]);
            var objName = split[0];
            var scriptName = split[1];
            var fieldName = split[2];
            GameObject go = null;
            for (int j = 0; j < gameObjects.Count; j++)
            {
                if (gameObjects[j] != null && gameObjects[j].name.Equals(objName))
                {
                    go = gameObjects[j];
                    break;
                }
            }
            if (go == null)
            {
                Debug.Log("Can't find KPI-gameObject?");
                continue;
            }
            var script = go.GetComponent(scriptName);
            //Debug.Log(script.GetType());

            var value = Convert.ToSingle(script.GetType().GetField(fieldName).GetValue(script));
            controller.AddTimedData(i, value);
        }
    }

    /// <summary>
    /// Update the chosen KPIs when a change is done to "gameObjects" in the inspector.
    /// </summary>
    void OnValidate()
    {
        // Check if there are duplicated gameObjects and set the later one to null.
        HashSet<int> codes = new HashSet<int>();
        for (int i = 0; i < gameObjects.Count; i++)
        {
            var go = gameObjects[i];
            if (go != null)
            {
                if (codes.Contains(go.GetHashCode()))
                {
                    Debug.Log(go.name + " already added.");
                    gameObjects[i] = null;
                }
                else
                {
                    codes.Add(go.GetHashCode());
                }
            }
        }
        // Remove chosen KPIs if its gameObject is removed from the list.
        List<string> kpiCleanse = new List<string>();
        for (int i = 0; i < kpiStrings.Count; i++)
        {
            var hashCode = int.Parse(SplitKPIString(kpiStrings[i])[3]);
            if (!codes.Contains(hashCode))
                kpiCleanse.Add(kpiStrings[i]);
        }
        foreach (var str in kpiCleanse)
        {
            RemoveKPI(str);
        }
    }

    public ChartController GetChartController()
    {
        controller = GetComponent<ChartController>();
        return controller;
    }

    /// <summary>
    /// Apply settings to chartController.
    /// </summary>
    public void ApplySettings()
    {
        // Get the chart this feeder is attached to.
        GetChartController();
        // Initialize DataContainer and init updateTimers-array.
        controller.SeriesCount = kpiStrings.Count;
        // Set series colors.
        SetSeriesColorsInChartController();
        // Set series names.
        SetSeriesNameInChartController();
        // Set streaming to true.
        controller.streaming = true;
        // Set desired chart type. No need to use SetChartType.
        controller.chartType = chartType;
    }

    void SetSeriesColorsInChartController()
    {
        for (int i = 0; i < kpiColors.Count; i++)
        {
            controller.SetSeriesColor(i, kpiColors[i]);
        }
    }

    void SetSeriesNameInChartController()
    {
        for (int i = 0; i < kpiNames.Count; i++)
        {
            controller.SetSeriesName(i, kpiNames[i]);
        }
    }


    public void AddKPI(string kpi, string legend, string type)
    {
        GetChartController();
        kpiStrings.Add(kpi);
        kpiNames.Add(legend);
        kpiColors.Add(controller.GetSeriesColor(kpiColors.Count));
        //kpiTypes.Add(type);
    }

    public string GetButtonString(int index)
    {
        var str = "";
        if (index >= 0 && index < kpiStrings.Count)
        {
            var splits = SplitKPIString(kpiStrings[index]);
            return splits[1] + ":" + splits[2];
        }
        return str;
    }

    public void RemoveKPI(string kpi)
    {
        var index = kpiStrings.IndexOf(kpi);
        kpiStrings.RemoveAt(index);
        kpiNames.RemoveAt(index);
        kpiColors.Remove(kpiColors.Last());
        //kpiTypes.RemoveAt(index);
    }

    public void RemoveAllKPIs()
    {
        kpiStrings.Clear();
        kpiNames.Clear();
        kpiColors.Clear();
        //kpiTypes.Clear();
    }
}

