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

public class KPIPassengersPerType : MonoBehaviour
{
    public int busPassengers = 0;
    public int trainPassengers = 0;
    public int tramPassengers = 0;
    public int metroPassengers = 0;
    public int carPassengers = 0;
    public int bicyclePassengers = 0;
    public int totalPublicTransportPassengers = 0;
    public int totalPassengers = 0;

    public int bicycleCounter = 0;

    private FlashPedestriansGlobalParameters pedestrianGlobals;
    private Transform spawnerPoints, transLines, destinationPoints, trafficIntegration;

    // Use this for initialization
    void Start()
    {
        //Fetch the gameobjects to gather this dataz
        spawnerPoints = GameObject.Find("SpawnerPoints").transform;
        transLines = GameObject.Find("TransLines").transform;
        destinationPoints = GameObject.Find("DestinationPoints").transform;
        trafficIntegration = GameObject.Find("TrafficIntegrationData").transform;

        pedestrianGlobals = GameObject.Find("FlashPedestriansModule").GetComponent<FlashPedestriansGlobalParameters>();
    }

    // Update is called once per frame
    void Update()
    {
        carPassengers = getCarPassengers();
        bicyclePassengers = pedestrianGlobals.numberOfPedestriansPerAgent * bicycleCounter;

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

        tramPassengers = pedestrianGlobals.numberOfPedestriansPerAgent * tmpTram;
        busPassengers = pedestrianGlobals.numberOfPedestriansPerAgent * tmpBus;
        trainPassengers = pedestrianGlobals.numberOfPedestriansPerAgent * tmpTrain;
        metroPassengers = pedestrianGlobals.numberOfPedestriansPerAgent * tmpMetro;

        totalPublicTransportPassengers = tramPassengers + busPassengers + trainPassengers + metroPassengers;
        totalPassengers = totalPublicTransportPassengers + carPassengers + bicyclePassengers;
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
        return pedestrianGlobals.numberOfPedestriansPerAgent * carcount;
    }

    /*
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
    */
}