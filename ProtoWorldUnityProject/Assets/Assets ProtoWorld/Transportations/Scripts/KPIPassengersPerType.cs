using UnityEngine;
using System.Collections;

public class KPIPassengersPerType : MonoBehaviour
{
    public int busPassengers = 0;
    public int trainPassengers = 0;
    public int tramPassengers = 0;
    public int metroPassengers = 0;
    public int carPassengers = 0;
    public int bicyclePassengers = 0;
    public int bicyclePassengersDecentralized = 0;

    private LineStatistics lineStatistics;
    private Transform spawnerPoints, transLines, destinationPoints, trafficIntegration;

    // Use this for initialization
    void Start()
    {
        //Fetch the gameobjects to gather this dataz
        spawnerPoints = GameObject.Find("SpawnerPoints").transform;
        transLines = GameObject.Find("TransLines").transform;
        destinationPoints = GameObject.Find("DestinationPoints").transform;
        trafficIntegration = GameObject.Find("TrafficIntegrationData").transform;

        lineStatistics = GetComponentInChildren<LineStatistics>();
    }

    // Update is called once per frame
    void Update()
    {
        //busPassengers = getBusPassengers();
        //tramPassengers = lineStatistics.totalTraveling;
        //trainPassengers = getTrainPassengers();
        //metroPassengers = getMetroPassengers();
        carPassengers = getCarPassengers();
        bicyclePassengers = getBicyclePassengers();

        int tmpTram = 0;
        int tmpTrain = 0;
        int tmpBus = 0;
        int tmpMetro = 0;

        foreach (Transform transLine in transLines)
        {
            if (transLine.gameObject.activeSelf)
            {
                foreach (Transform item in transLine)
                {
                    if (item.gameObject.activeSelf)
                    {
                        VehicleController vc = item.GetComponent<VehicleController>();
                        if (transLine.name.StartsWith("Tram_"))
                            tmpTram += vc.headCount;
                        else if (transLine.name.StartsWith("Train_"))
                            tmpTrain += vc.headCount;
                        else if (transLine.name.StartsWith("Bus_"))
                            tmpBus += vc.headCount;
                        else if (transLine.name.StartsWith("Metro_"))
                            tmpMetro += vc.headCount;
                    }
                }
            }
        }

        tramPassengers = tmpTram;
        busPassengers = tmpBus;
        trainPassengers = tmpTrain;
        metroPassengers = tmpMetro;
    }

    private int getBusPassengers()
    {
        int buscount = 0;
        foreach (Transform transType in transLines)
        {
            if (transType.gameObject.activeSelf)
            {
                foreach (Transform carriage in transType)
                {
                    if (carriage.gameObject.activeSelf)
                    {
                        VehicleController vc = carriage.GetComponent<VehicleController>();
                        if (vc != null)
                            buscount += vc.headCount;
                    }
                }
            }
        }
        return buscount;
    }

    private int getCarPassengers()
    {
        int carcount = 0;
        foreach (Transform vehicle in trafficIntegration)
        {
            if (vehicle.gameObject.activeSelf)
            {
                if (vehicle.name.Substring(0, 3) == "veh")
                    carcount++;
            }
        }
        return carcount;
    }

    private int getMetroPassengers()
    {
        return 0;
    }

    private int getTrainPassengers()
    {
        return 0;
    }

    private int getBicyclePassengers()
    {
        int cyclists = 0;
        foreach (Transform spawner in spawnerPoints)
        {
            if (spawner.gameObject.activeSelf)
            {
                foreach (Transform pedestrian in spawner)
                {
                    if (pedestrian.gameObject.activeSelf)
                    {
                        foreach (Transform child in pedestrian)
                        {
                            if (child.name == "bike" && child.gameObject.activeSelf)
                            {
                                cyclists++;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return cyclists;
    }
}