using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdditionalKPIData : MonoBehaviour {
    public ChartController chart1, chart2;

    private float dataTimer = 1f;
    private List<int> walkers, cyclists, busPassengers, trainPassengers, drivers, others;
    private Transform spawnerPoints, transLines;

	// Use this for initialization
	void Start () {
        spawnerPoints = GameObject.Find("SpawnerPoints").transform;
        transLines = GameObject.Find("TransLines").transform;

        walkers = new List<int>();
        cyclists = new List<int>();
        busPassengers = new List<int>();
        trainPassengers = new List<int>();
        drivers = new List<int>();
        others = new List<int>();

        chart1.RegisterNewKPI();
        chart1.RegisterNewKPI();
        chart1.RegisterNewKPI();
        chart1.SetSeriesName(0, "Pedestrians");
        chart1.SetSeriesName(1, "Cyclists");
        chart1.SetSeriesName(2, "Passengers");

        chart2.RegisterNewKPI();
        chart2.RegisterNewKPI();
        chart2.RegisterNewKPI();
        chart2.RegisterNewKPI();
        chart2.SetSeriesName(0, "Ped To Cyc");
        chart2.SetSeriesName(1, "Cyc To Ped");
        chart2.SetSeriesName(2, "Ped To Pass");
        chart2.SetSeriesName(3, "Pass To Ped");
    }
	
	// Update is called once per frame
	void Update () {
        if (dataTimer >= 1)
        {
            dataTimer -= 1f;
            getPedestrians();
            getTrains();
            getBuses();
            getCars();
        }
        dataTimer += Time.deltaTime;

        chart1.AddTimedData(0, walkers.Count, walkers[walkers.Count - 1]);
        chart1.AddTimedData(1, cyclists.Count, cyclists[cyclists.Count - 1]);
        chart1.AddTimedData(2, busPassengers.Count, busPassengers[busPassengers.Count - 1]);

        chart2.AddTimedData(0, cyclists.Count, Mathf.Max(cyclists[cyclists.Count - 1] - cyclists[cyclists.Count - 2], 0));
        chart2.AddTimedData(1, cyclists.Count, Mathf.Max(cyclists[cyclists.Count - 2] - cyclists[cyclists.Count - 1], 0));
        chart2.AddTimedData(2, busPassengers.Count, Mathf.Max(busPassengers[busPassengers.Count - 1] - busPassengers[busPassengers.Count - 2], 0));
        chart2.AddTimedData(3, busPassengers.Count, Mathf.Max(busPassengers[busPassengers.Count - 2] - busPassengers[busPassengers.Count - 1], 0));
	}
    
    //Get number of pedestrians in simulation
    private void getPedestrians()
    {
        int walk = 0, cyc = 0, bus = 0;

        foreach (Transform spawner in spawnerPoints)
        {
            if (spawner.gameObject.activeSelf)
            {
                foreach (Transform pedestrian in spawner)
                {
                    if (pedestrian.gameObject.activeSelf)
                    {
                        bool isCyclist = false;
                        foreach (Transform child in pedestrian)
                        {
                            if (child.name == "bike" && child.gameObject.activeSelf)
                            {
                                isCyclist = true;
                                break;
                            }
                        }
                        if (isCyclist)
                            cyc++;
                        else
                            walk++;
                    }
                }
            }
        }
        foreach (Transform transType in transLines)
        {
            if (transType.gameObject.activeSelf)
            {
                foreach (Transform carriage in transType)
                {
                    if (carriage.gameObject.activeSelf)
                    {
                        VehicleController vc = carriage.GetComponent<VehicleController>();
                        if (vc == null)
                            continue;
                        else
                        {
                            //Debug.Log(carriage.name+" "+vc.headCount);
                            bus += vc.headCount;
                        }
                    }
                }
            }
        }

        //walking
        walkers.Add(walk);
        //cycling
        cyclists.Add(cyc);
        //bus
        busPassengers.Add(bus);
        //train
        //car
        //other
    }
    
    private void getTrains()
    {

    }

    private void getBuses()
    {

    }

    private void getCars()
    {

    }
}
