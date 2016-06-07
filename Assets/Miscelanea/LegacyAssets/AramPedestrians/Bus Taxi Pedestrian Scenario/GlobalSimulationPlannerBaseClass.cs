using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalSimulationPlannerBaseClass : MonoBehaviour
{

    // TODO:= Implement a lat/lon to Vector3 method with global accesibility.
    public static List<BusStopClass> OriginDestinationSet;
    public static List<BusStopClass> AvailableOrigins;
    public static float AcceptedBusDistanceToDestination = 0.5f;
    public static float AcceptedBusDistanceToBusStop = 0.5f;
    public static float WaitTimeAtTheBusStopInSeconds = 5f;
    public static float DefaultBusRouteHeight = 0.5f;
    public static float SlowDownRatioForExtremeAngles = 70f;
    public static Vector3 PathSmoothingMaximumDistance = Vector3.one;
    public static int DefaultNormalBusCapacity = 30;
    public static float DefaultNormalBusDistance = 1;
    public static float BusFrontAwarenessDistance = 5;
    public static float BusFrontAwarnessWaitingTime = 0.25f;
    public static ServiceGapslabsClient client;
    public static string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase;

    public void Start()
    {
        if (client == null)
            client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        ChooseConnection();
    }
    public static void Init()
    {
        if (client == null)
            client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        ChooseConnection();
    }
    private static void ChooseConnection()
    {
        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;
    }
    void OnDestroy()
    {
        if (client != null)
            client.Close();
    }
}
