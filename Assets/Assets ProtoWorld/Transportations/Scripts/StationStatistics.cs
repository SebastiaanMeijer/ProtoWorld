using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StationStatistics : MonoBehaviour {

    [System.Serializable]
    public class StationStats
    {
        [HideInInspector]
        public string name;
        public int headcount;
        public StationController controller;
    }

	public int[] QuePerStation;
	public int stationNR = 0;

    public int totalQueuing;
    public List<StationStats> queues;

    void Awake()
    {
        QuePerStation = new int[this.gameObject.transform.childCount];
    }

    // Use this for initialization
    void Start () {
        var stations = GetComponentsInChildren<StationController>();
        queues = new List<StationStats>();
        foreach (var station in stations)
        {
            queues.Add(new StationStats
            {
                controller = station,
                name = station.GetIdAndName(),
            });
        }
    }
	
	// Update is called once per frame
	void Update () {
        totalQueuing = 0;
        foreach (var station in queues)
        {
			
            station.headcount = station.controller.GetHeadCount();
			QuePerStation[stationNR] = station.headcount;
			stationNR = stationNR +1;
            totalQueuing += station.headcount;
        }
		stationNR = 0;
	}
}
