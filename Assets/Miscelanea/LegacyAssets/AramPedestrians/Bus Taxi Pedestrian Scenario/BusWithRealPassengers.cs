using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BusWithRealPassengers : BusBaseClass
{
    public List<PedestrianBaseClass> Passengers;
    public float DistanceFromThePath = 0;
    private float DistanceFromThePathOldValue;
    private bool prepareForUpdate = false;
    private float LastModifiedValueTime;
    public Vector3[] BusNodesClosetsPointOnLine;
    public Vector3[] BusRoutePoints;
    public float ErrorThreshold = 0.01f;
    public bool Selected = false;
    RouteBaseClass thisRoute;
    // Use this for initialization
    void Start()
    {
        initBusBase();
        if (Passengers == null)
            Passengers = new List<PedestrianBaseClass>();
        BusRoutePoints = BusRoute.GetLane(DistanceFromThePath); // BusRoute.Points;
        BusRoutePoints = BusRoutePoints.RemoveConsecutiveDuplicates(ErrorThreshold);
        BusNodesClosetsPointOnLine = BusRoute.ProjectTheBusNodesOnTheLane(BusRoutePoints); // BusRoute.BusNodesClosetsPointOnLine;
        transform.position = BusRoutePoints[0]; //BusRoute.Points[0];
        BusAgent.CurrentDestination = transform.position;
        DistanceFromThePathOldValue = DistanceFromThePath;
        LastModifiedValueTime = Time.time;
    }

    int count = 0;
    int visitedBusStopIndex = 0;
    // Update is called once per frame
    void Update()
    {

        if (DistanceFromThePath != DistanceFromThePathOldValue && !prepareForUpdate)
        {
            LastModifiedValueTime = Time.time;
            prepareForUpdate = true;
        }
        if ((Time.time - LastModifiedValueTime) >= 1 && prepareForUpdate)
        {
            DistanceFromThePathOldValue = DistanceFromThePath;
            BusRoutePoints = BusRoute.GetLane(DistanceFromThePath); // BusRoute.Points;
            BusRoutePoints = BusRoutePoints.RemoveConsecutiveDuplicates(ErrorThreshold);
            BusNodesClosetsPointOnLine = BusRoute.ProjectTheBusNodesOnTheLane(BusRoutePoints); // BusRoute.BusNodesClosetsPointOnLine;
            prepareForUpdate = false;
        }
        //if (Selected)
        //    for (int i = 0; i < BusNodesClosetsPointOnLine.Length; i++)
        //    {
        //        Debug.DrawLine(BusRoute.BusNodes[i].transform.position, BusRoute.BusNodes[i].transform.position + Vector3.up, Color.red);
        //        Debug.DrawLine(BusNodesClosetsPointOnLine[i], BusNodesClosetsPointOnLine[i] + Vector3.up, Color.green);

        //    }
        if (Selected)
            for (int i = 0; i < BusRoutePoints.Length - 1; i++)
            {
                Debug.DrawLine(BusRoutePoints[i], BusRoutePoints[i] + Vector3.up, Color.magenta);
                Debug.DrawLine(BusRoutePoints[i], BusRoutePoints[i + 1], Color.cyan);
            }
        if (BusAgent.IsAtDestination(BusRoutePoints[count], GlobalSimulationPlannerBaseClass.AcceptedBusDistanceToBusStop))
        {
            // Debug.Log ("going toward the point #" + count);
            count++;
            BusAgent.CalculateSpeed();
            // If at the end of the line, start at the first point.
            if (count == BusRoutePoints.Length)
            {
                Interpolations.MyLog("Reached at the end of list.");
                count = 0;
                visitedBusStopIndex = 0;
                transform.position = BusRoutePoints[0];
                BusAgent.CurrentDestination = transform.position;
            }


        }
        if (Selected)
            if (visitedBusStopIndex < BusNodesClosetsPointOnLine.Length)
                Debug.DrawLine(BusNodesClosetsPointOnLine[visitedBusStopIndex], transform.position, Color.yellow);
        // Debug.Log (Vector3.Distance (BusNodesClosetsPointOnLine [visitedBusStopIndex], transform.position));
        // Check if we have reached at a bus stop
        if (visitedBusStopIndex < BusNodesClosetsPointOnLine.Length)
            if (Vector3.Distance(BusNodesClosetsPointOnLine[visitedBusStopIndex], transform.position) < GlobalSimulationPlannerBaseClass.AcceptedBusDistanceToBusStop)
            {
                Interpolations.MyLog("Bus stop " + BusRoute.BusNodes[visitedBusStopIndex].Name + " reached. Wait for passengers.");
                visitedBusStopIndex++;
                // It is a bus stop, so wait
                BusAgent.Wait(GlobalSimulationPlannerBaseClass.WaitTimeAtTheBusStopInSeconds);
            }


        // Set the next destination
        //BusAgent.MoveWithRelativeSpeed(BusRoutePoints[count]);
        BusAgent.Move(BusRoutePoints[count]);


    }

    public override int GetPassengerCount()
    {
        return Passengers.Count;
    }
}
