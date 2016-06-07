using UnityEngine;
using System.Collections;

/// <summary>
/// This controller is only suitable for buses from Sumo-simulations.
/// </summary>
public class SumoBusController : VehicleController
{

    /// <summary>
    /// Calling the base class to initate [disembarkersAtStation]
    /// Should not be needed to call.
    /// </summary>
    //void start()
    //{
    //    base.Start();
    //}

    /// <summary>
    /// To reuse the vehicle.
    /// Override so that the gameobject position is not set to the currentStation at reset.
    /// </summary>
    /// <param name="direction"></param>
    public override void ResetVehicle(LineDirection direction)
    {
        this.direction = direction;
        currentStation = line.GetFirstStop(direction);
        nextStation = currentStation;
        capacity = line.GetVehicleCapacity();
        InitLists();
        ResetTimer();
    }

    /// <summary>
    /// Override VehicleController so that the position update is done in Sumo.
    /// </summary>
	public override void Update()
    {
        if (nextStation == null)
        {
            headCount = GetHeadCount();
            foreach (var list in disembarkersAtStation.Values)
            {
                foreach (var traveler in list)
                {
                    traveler.ArrivedAt(currentStation);
                }
                list.Clear();
            }
            return;
        }

        float distToNext = Vector3.Distance(transform.position, nextStation.transform.position);
        if (distToNext < line.stationRadius)
        {
            //Debug.Log(name + ": close to a station!");
            ArrivedAtNextStation();
        }
    }

    // Obsolete: should be taken care of by Update().
    //public override void ArrivedAtNextStation()
    //{
    //    base.ArrivedAtNextStation();
    //    //Debug.LogFormat("bus {0} arrived to {1}, heading {2}", this, currentStation, nextStation);


    //    //foreach (var list in disembarkersAtStation.Values)
    //    //{
    //    //    foreach (var traveler in list)
    //    //    {
    //    //        traveler.ArrivedAt(nextStation);
    //    //    }
    //    //    list.Clear();
    //    //}
    //}

    // Obsolete: should be taken care of in Update()
    //void OnDisable()
    //{
    //    ArrivedAtNextStation();
    //}
}
