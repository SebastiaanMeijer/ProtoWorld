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
 * KPIManager.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script that manages the information that is feed to plot the data onto a certain KPI chart.
/// </summary>
public class KPIManager : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public string kpiName = "?";
    public bool tellMeWhatYouAreDoing = true;
    public bool isAverage = false;

    private int nextFreeKey = 0;
    private Dictionary<int, float> kpiValues = new Dictionary<int, float>();
    private float period = 1.0f;
    private float nextActionTime = 0.0f;

    private static int id = 0;
    private string chartId;
    public string chartType;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        chartId = id.ToString();
        id++;
    }

    /// <summary>
    /// Initializes the script.
    /// </summary>
    void Start()
    {
        //Nothing to do here for now
        log.Info(string.Concat(chartId, ":", chartType));
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;

            float sumKPI = 0;
            foreach (KeyValuePair<int, float> pair in kpiValues)
            {
                sumKPI += pair.Value;
            }

            int idx = 0;

            // Send the KPI to the plot
            if (isAverage && kpiValues.Count > 0)
            {
                this.transform.GetComponentInChildren<DataHolder>().mData[idx] = sumKPI / (float)kpiValues.Count;
                log.Info(string.Concat(chartId, ":", idx, ":", sumKPI / (float)kpiValues.Count));

                if (tellMeWhatYouAreDoing)
                    Debug.Log("Plot " + sumKPI / (float)kpiValues.Count + " for KPI " + kpiName);
            }
            else
            {
                this.transform.GetComponentInChildren<DataHolder>().mData[idx] = sumKPI;
                log.Info(string.Concat(chartId, ":", idx, ":", sumKPI ));
                if (tellMeWhatYouAreDoing)
                    Debug.Log("Plot " + sumKPI + " for KPI " + kpiName);
            }
        }
    }

    /// <summary>
    /// Registers a new informer of the KPI. 
    /// </summary>
    /// <returns>Key that identifies the informer.</returns>
    public int RegisterNewInformer()
    {
        int key = nextFreeKey;
        nextFreeKey++;
        kpiValues.Add(key, 0.0f);
        return key;
    }

    /// <summary>
    /// Informs the KPI about the change on a value on one of the informers. 
    /// </summary>
    /// <param name="key">Key that identifies the informer.</param>
    /// <param name="value">New value.</param>
    public void InformKPI(int key, float value)
    {
        kpiValues[key] = value;
    }
}
