using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineStatistics : MonoBehaviour {

    [System.Serializable]
    public class LineStats
    {
        [HideInInspector]
        public string name;
        public int queuing;
        public int traveling;
        public LineController controller;
    }

    public int totalQueuing;
    public int totalTraveling;

    public List<LineStats> lineStats;

    // Use this for initialization
    void Start () {
        var lines = GetComponentsInChildren<LineController>();
        lineStats = new List<LineStats>();
        foreach (var line in lines)
        {
            lineStats.Add(new LineStats
            {
                controller = line,
                name = line.gameObject.name,
            });
        }
    }
	
	// Update is called once per frame
	void Update () {
        totalQueuing = 0;
        totalTraveling = 0;
        foreach (var stats in lineStats)
        {
            stats.queuing = stats.controller.queuingHeadCount;
            stats.traveling = stats.controller.travelingHeadCount;
            totalQueuing += stats.queuing;
            totalTraveling += stats.traveling;
        }
    }
}
