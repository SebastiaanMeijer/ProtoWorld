using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdditionalKPIData : MonoBehaviour {
    public ChartController chart1, chart2, chart3, chart4;
    public static int arrived = 0;

    private float dataTimer = 1f;
    private List<int> walkers, cyclists, busPassengers, trainPassengers, drivers, others, arrivals;
    private Transform spawnerPoints, transLines, destinationPoints;

    private List<int> pieCyc = new List<int>(), pieBus = new List<int>(), pieArriv = new List<int>();

    private Dictionary<string, int> destinationCount;

	// Use this for initialization
	void Start () {
        spawnerPoints = GameObject.Find("SpawnerPoints").transform;
        transLines = GameObject.Find("TransLines").transform;
        destinationPoints = GameObject.Find("DestinationPoints").transform;

        walkers = new List<int>();
        cyclists = new List<int>();
        busPassengers = new List<int>();
        trainPassengers = new List<int>();
        drivers = new List<int>();
        others = new List<int>();
        arrivals = new List<int>();

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
        chart2.RegisterNewKPI();
        chart2.SetSeriesName(0, "Ped To Cyc");
        chart2.SetSeriesName(1, "Cyc To Ped");
        chart2.SetSeriesName(2, "Ped To Pass");
        chart2.SetSeriesName(3, "Pass To Ped");
        chart2.SetSeriesName(4, "Arrivals");

        if (chart3 != null)
        {
            chart3.RegisterNewKPI();
            chart3.RegisterNewKPI();
            chart3.RegisterNewKPI();
            chart3.SetSeriesName(0, "To Cyclists");
            chart3.SetSeriesName(1, "To Passengers");
            chart3.SetSeriesName(2, "To Arrivals");
        }

        destinationCount = new Dictionary<string, int>();
        foreach (Transform destinationPoint in destinationPoints) {
            FlashPedestriansDestination fpd = destinationPoint.GetComponent<FlashPedestriansDestination>();
            chart4.RegisterNewKPI();
            chart4.SetSeriesName(destinationCount.Count, fpd.destinationName);
            destinationCount.Add(fpd.destinationName,0);
        }
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
            getArrived();

            getDestinations();

            chart1.AddTimedData(0, walkers.Count, walkers[walkers.Count - 1]);
            chart1.AddTimedData(1, cyclists.Count, cyclists[cyclists.Count - 1]);
            chart1.AddTimedData(2, busPassengers.Count, busPassengers[busPassengers.Count - 1]);

            if (cyclists.Count >= 2)
            {
                chart2.AddTimedData(0, cyclists.Count, Mathf.Max(cyclists[cyclists.Count - 1] - cyclists[cyclists.Count - 2], 0));
                chart2.AddTimedData(1, cyclists.Count, Mathf.Max(cyclists[cyclists.Count - 2] - cyclists[cyclists.Count - 1], 0));
                chart2.AddTimedData(2, busPassengers.Count, Mathf.Max(busPassengers[busPassengers.Count - 1] - busPassengers[busPassengers.Count - 2], 0));
                chart2.AddTimedData(3, busPassengers.Count, Mathf.Max(busPassengers[busPassengers.Count - 2] - busPassengers[busPassengers.Count - 1], 0));
                //arrived
                chart2.AddTimedData(4, arrivals.Count, arrivals[arrivals.Count - 1]);

                if (chart3 != null)
                {
                    //pie chart
                    if (pieCyc.Count >= 50)
                        pieCyc.RemoveAt(0);
                    if (pieBus.Count >= 50)
                        pieBus.RemoveAt(0);
                    pieCyc.Add(Mathf.Max(cyclists[cyclists.Count - 1] - cyclists[cyclists.Count - 2], 0));
                    pieBus.Add(Mathf.Max(busPassengers[busPassengers.Count - 1] - busPassengers[busPassengers.Count - 2], 0));

                    int totalCyc = 0, totalBus = 0, totalArriv = 0;
                    int listsize = Mathf.Min(pieCyc.Count, pieBus.Count, pieArriv.Count);
                    while (pieCyc.Count > listsize) { pieCyc.RemoveAt(0); }
                    while (pieBus.Count > listsize) { pieBus.RemoveAt(0); }
                    while (pieArriv.Count > listsize) { pieArriv.RemoveAt(0); }
                    for (int i = 0; i < listsize; i++)
                    {
                        totalCyc += pieCyc[i];
                        totalBus += pieBus[i];
                        totalArriv += pieArriv[i];
                    }
                    //int totalPie = totalCyc + totalBus + totalArriv;

                    chart3.AddTimedData(0, cyclists.Count, totalCyc);
                    chart3.AddTimedData(1, busPassengers.Count, totalBus);
                    chart3.AddTimedData(2, cyclists.Count, totalArriv);
                }
            }

            int j = 0;
            foreach (string key in destinationCount.Keys)
            {
                chart4.AddTimedData(j, 0, destinationCount[key]);
                j++;
            }
        }
        dataTimer += Time.deltaTime;
	}
    
    //Get number of pedestrians in simulation
    private void getPedestrians()
    {
        int walk = 0, cyc = 0, bus = 0, train = 0, car = 0;

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
        trainPassengers.Add(train);
        //car
        drivers.Add(car);
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

    private void getArrived()
    {
        arrivals.Add(arrived);

        //pie chart
        if (pieArriv.Count >= 50)
            pieArriv.RemoveAt(0);
        pieArriv.Add(arrived);

        arrived = 0;
    }

    private void getDestinations()
    {
        List<string> keys = new List<string>(destinationCount.Keys);
        foreach (string key in keys)
        {
            destinationCount[key] = 0;
        }

        foreach (Transform spawner in spawnerPoints)
        {
            if (spawner.gameObject.activeSelf)
            {
                foreach (Transform pedestrian in spawner)
                {
                    if (pedestrian.gameObject.activeSelf)
                    {
                        FlashPedestriansController fpc = pedestrian.GetComponent<FlashPedestriansController>();
                        string destName = fpc.routing.destinationPoint.destinationName;
                        if (!destinationCount.ContainsKey(destName))
                        {
                            destinationCount.Add(destName, 1);
                        }
                        else
                        {
                            destinationCount[destName]++;
                        }
                    }
                }
            }
        }
    }
}
