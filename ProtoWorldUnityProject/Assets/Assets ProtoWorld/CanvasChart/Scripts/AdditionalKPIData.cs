using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdditionalKPIData : MonoBehaviour {
    public ChartController chartTravel, chartTransfer, chartTransferPedestrian, chartDestination, chartDelays, chartQueues;
    public static int arrived = 0;

    private float dataTimer = 1f;
    private List<int> walkers, cyclists, busPassengers, trainPassengers, drivers, others, arrivals, queuing;
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
        queuing = new List<int>();

        if (chartTravel != null)
        {
            chartTravel.RegisterNewKPI("Pedestrians");
            chartTravel.RegisterNewKPI("Cyclists");
            chartTravel.RegisterNewKPI("Passengers");
        }

        if (chartTransfer != null)
        {
            chartTransfer.RegisterNewKPI("Ped To Cyc");
            chartTransfer.RegisterNewKPI("Cyc To Ped");
            chartTransfer.RegisterNewKPI("Ped To Pass");
            chartTransfer.RegisterNewKPI("Pass To Ped");
            chartTransfer.RegisterNewKPI("Arrivals");
        }

        if (chartTransferPedestrian != null)
        {
            chartTransferPedestrian.RegisterNewKPI("To Cyclists");
            chartTransferPedestrian.RegisterNewKPI("To Passengers");
            chartTransferPedestrian.RegisterNewKPI("To Arrivals");
        }

        destinationCount = new Dictionary<string, int>();
        if (chartDestination != null)
        {
            foreach (Transform destinationPoint in destinationPoints)
            {
                FlashPedestriansDestination fpd = destinationPoint.GetComponent<FlashPedestriansDestination>();
                chartDestination.RegisterNewKPI();
                chartDestination.SetSeriesName(destinationCount.Count, fpd.destinationName);
                destinationCount.Add(fpd.destinationName, 0);
            }
        }

        if (chartDelays != null)
        {

        }

        if (chartQueues != null)
        {
            chartQueues.RegisterNewKPI("Queues");
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
            getDelays();
            getQueues();

            if (chartTravel != null)
            {
                chartTravel.AddTimedData(0, walkers.Count, walkers[walkers.Count - 1]);
                chartTravel.AddTimedData(1, cyclists.Count, cyclists[cyclists.Count - 1]);
                chartTravel.AddTimedData(2, busPassengers.Count, busPassengers[busPassengers.Count - 1]);
            }

            if (cyclists.Count >= 2)
            {
                if (chartTransfer != null)
                {
                    chartTransfer.AddTimedData(0, cyclists.Count, Mathf.Max(cyclists[cyclists.Count - 1] - cyclists[cyclists.Count - 2], 0));
                    chartTransfer.AddTimedData(1, cyclists.Count, Mathf.Max(cyclists[cyclists.Count - 2] - cyclists[cyclists.Count - 1], 0));
                    chartTransfer.AddTimedData(2, busPassengers.Count, Mathf.Max(busPassengers[busPassengers.Count - 1] - busPassengers[busPassengers.Count - 2], 0));
                    chartTransfer.AddTimedData(3, busPassengers.Count, Mathf.Max(busPassengers[busPassengers.Count - 2] - busPassengers[busPassengers.Count - 1], 0));
                    //arrived
                    chartTransfer.AddTimedData(4, arrivals.Count, arrivals[arrivals.Count - 1]);
                }

                if (chartTransferPedestrian != null)
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

                    chartTransferPedestrian.AddTimedData(0, cyclists.Count, totalCyc);
                    chartTransferPedestrian.AddTimedData(1, busPassengers.Count, totalBus);
                    chartTransferPedestrian.AddTimedData(2, cyclists.Count, totalArriv);
                }
            }

            if (chartDestination != null)
            {
                int j = 0;
                foreach (string key in destinationCount.Keys)
                {
                    chartDestination.AddTimedData(j, 0, destinationCount[key]);
                    j++;
                }
            }

            if (chartDelays != null)
            {

            }

            if (chartQueues != null)
            {
                chartQueues.AddTimedData(0, queuing.Count, queuing[queuing.Count - 1]);
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

    private void getDelays()
    {
        //get delays for car transport and public transport

        //and delta delays
    }

    private void getQueues()
    {
        //get waiting lines at public transport stations
        int queues = 0;

        Transform stations = GameObject.Find("Stations").transform;
        foreach (Transform station in stations)
        {
            StationController sc = station.GetComponent<StationController>();
            queues += sc.queuing;
        }

        queuing.Add(queues);

        //number
        //time
    }

    private void getAccidents()
    {
        //google maps marker overlay
    }

    private void getBlockedRoads()
    {
        //google maps marker overlay
    }

    private void getStaticPublicTransport()
    {
        //all busses and trams that are not moving
    }
}
