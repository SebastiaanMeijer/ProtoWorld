using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KPITransfersOfTransport : MonoBehaviour
{
    private KPIPassengersPerType passenger_data;

    //Tram
    private int current_tram = 0;
    public int deltaTram = 0;

    //Train
    private int current_train = 0;
    public int deltaTrain = 0;

    //Bus
    private int current_bus = 0;
    public int deltaBus = 0;

    //Car
    private int current_car = 0;
    public int deltaCar = 0;

    //Metro
    private int current_metro = 0;
    public int deltaMetro = 0;


    // Use this for initialization
    void Start()
    {
        GameObject obj = GameObject.Find("TransportationModule");
        passenger_data = obj.GetComponent<KPIPassengersPerType>();
    }

    //Calculates the difference in passengers per type
    void Update()
    {
        //Tram
        if (passenger_data.tramPassengers != current_tram)
        {
            deltaTram = passenger_data.tramPassengers - current_tram;
            current_tram = passenger_data.tramPassengers;
        }

        //Train
        if(passenger_data.trainPassengers != current_train)
        {
            deltaTrain = passenger_data.trainPassengers - current_train;
            current_train = passenger_data.trainPassengers;
        }

        //Bus
        if (passenger_data.busPassengers != current_bus)
        {
            deltaBus = passenger_data.busPassengers - current_bus;
            current_bus = passenger_data.busPassengers;
        }

        //Car
        if (passenger_data.carPassengers != current_car)
        {
            deltaCar = passenger_data.carPassengers - current_car;
            current_car = passenger_data.carPassengers;
        }

        //Metro
        if (passenger_data.metroPassengers != current_metro)
        {
            deltaMetro = passenger_data.metroPassengers - current_metro;
            current_metro = passenger_data.metroPassengers;
        }

    }
}