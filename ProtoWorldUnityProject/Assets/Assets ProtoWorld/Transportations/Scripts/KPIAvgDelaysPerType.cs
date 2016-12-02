/* 
This file is part of ProtoWorld. 

ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 
*/

/*
 * KPI data for displaying in charts
 * 
 * Nathan van Ofwegen
 * Antony Löbker
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KPIAvgDelaysPerType : MonoBehaviour
{
    public float avgBusDelay = 0;
    public float avgCarDelay = 0;
    public float avgTrainDelay = 0;
    public float avgTramDelay = 0;

    private KPIDelays delays;

    //Alternative is an array with a finger
    private Dictionary<int, float> TramDelaysDict;
    private Dictionary<int, float> BusDelaysDict;
    private Dictionary<int, float> TrainDelaysDict;

    //Amount of seconds to take the average of.
    [SerializeField]
    private int timeunit;

    // Use this for initialization
    void Start()
    {
        delays = transform.GetComponent<KPIDelays>();
        timeunit = 30;

        TramDelaysDict = new Dictionary<int, float>(timeunit);
        BusDelaysDict = new Dictionary<int, float>(timeunit);
        TrainDelaysDict = new Dictionary<int, float>(timeunit);
    }

    // Update is called once per frame
    void Update()
    {
        avgTramDelay = Update(TramDelaysDict, delays.TramDelay);
        avgBusDelay = Update(BusDelaysDict, delays.BusDelay);
        avgTrainDelay = Update(TrainDelaysDict, delays.TrainDelay);
    }

    private float Update(Dictionary<int, float> dict, float value)
    {
        int time = (int)Time.time;

        //Add the average value if it isn't stored yet (for this second)
        if (!dict.ContainsKey(time))
            dict.Add(time, value);
        
        //Remove all old (time - timeunit) values
        RemoveOld(dict);
        
        //Calculate the sum
        return CalculateSum(dict);
    }

    private float CalculateSum(Dictionary<int, float> dict)
    {
        float sum = 0;
        int count = 0;
        foreach(KeyValuePair<int, float> entry in dict)
        {
            count++;
            sum += entry.Value;
        }
        return sum / count;
    }

    private void RemoveOld(Dictionary<int, float> dict)
    {
        int time = (int)Time.time;
        List<int> old_keys = new List<int>();
        foreach (KeyValuePair<int, float> entry in dict)
        {
            if (entry.Key <= (time - timeunit))
                old_keys.Add(entry.Key);
        }
        foreach (int key in old_keys)
        {
            dict.Remove(key);
        }
    }
}