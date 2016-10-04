using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
        if (!dict.ContainsKey(time)) dict.Add(time, value);
        
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
            if (entry.Key <= (time - timeunit)) old_keys.Add(entry.Key);
        }
        foreach (int key in old_keys) dict.Remove(key);
    }
}