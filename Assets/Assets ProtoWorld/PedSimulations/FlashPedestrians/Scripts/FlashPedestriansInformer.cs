/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedestriansInformer.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This informer provides methods to allow Flash Pedestrians handle and get updates related to the routing information. 
/// </summary>
public class FlashPedestriansInformer : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;

    [HideInInspector]
    public FlashPedestriansGlobalParameters globalParam;

    RoutingController routingController;
    System.Collections.Generic.Dictionary<int, FlashPedestriansController> activePedestrians;

    private float secondsPerMeter;

    [Range(0, 10)]
    public float UpdateRateInSeconds = 1.0f;
    private float nextUpdateTime = 0;
    public GameObject chartControllerObject;
    private ChartController chartController;
    public int accumPedestrians = 0;
    public int accumSubscribers = 0;
    public int accumRouteChanges = 0;

    private Dictionary<int, BikeStationScript> bikeStations;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        globalParam = FindObjectOfType<FlashPedestriansGlobalParameters>();
        routingController = FindObjectOfType<RoutingController>();
        activePedestrians = new Dictionary<int, FlashPedestriansController>();
        secondsPerMeter = 1 / globalParam.averageSpeed;

        logSeriesId = LoggerAssembly.GetLogSeriesId();

        bikeStations = new Dictionary<int, BikeStationScript>();
    }

    /// <summary>
    /// Starts method.
    /// </summary>
    void Start()
    {
        nextUpdateTime = Time.time + UpdateRateInSeconds;
        //LOG PEDESTRIANS LOG INFO
        log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "Pedestrian subscriptions log"));

        if (chartControllerObject != null)
        {
            chartController = chartControllerObject.GetComponent<ChartController>();
            chartController.SeriesCount = 3;
            chartController.SetSeriesName(0, "#active");
            chartController.SetSeriesName(1, "#subs");
            chartController.SetSeriesName(2, "#follows");


            //LOG PEDESTRIANS LOG CHART INFO
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 0, chartController.chartType.ToString()));
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 1, chartController.chartType.ToString()));
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 2, chartController.chartType.ToString()));
        }

        //LOG PEDESTRIANS LOG INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Number of active pedestrian"));
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 1, "Number of subscribers"));
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 2, "Number of subscribers following the messages"));
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        int diffPedestrians = activePedestrians.Count * globalParam.numberOfPedestriansPerAgent - accumPedestrians;
        accumPedestrians += (diffPedestrians > 0) ? diffPedestrians : 0;
        accumSubscribers += (diffPedestrians > 0) ? Mathf.RoundToInt(diffPedestrians * globalParam.percOfPedSubscribed) : 0;

        if (nextUpdateTime < Time.time)
        {
            // SET NEXT UPDATE TIME
            nextUpdateTime += UpdateRateInSeconds;
            
            // LOG NUMBER OF PEDESTRIANS
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "int", 0, accumPedestrians));
            // LOG NUMBER OF SUBSCRIBERS
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "int", 1, accumSubscribers));
            // LOG NUMBER OF ROUTE CHANGES
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "int", 2, accumRouteChanges));

            // SEND INFO TO CHART
            if (chartController != null)
            {
                
                chartController.AddTimedData(0, nextUpdateTime, accumPedestrians);
                chartController.AddTimedData(1, nextUpdateTime, accumSubscribers);
                chartController.AddTimedData(2, nextUpdateTime, accumRouteChanges);
            }

        }

    }

    /// <summary>
    /// Bubble algorithm to find the best itinerary between the stations given and the stations close to the destination point. 
    /// </summary>
    /// <param name="startingPoint">Point in the map where the pedestrian starts.</param>
    /// <param name="destination">Final destination of the pedestrian.</param>
    /// <param name="stationsNearby">Stations nearby considered by the pedestrian.</param>
    /// <param name="travelPreference">Travel preferences of the pedestrian.</param>
    /// <returns>An Itinerary object with the best itinerary for the pedestrian (empty itinerary if anything better than walking has been found).</returns>
    public Itinerary FindBestItinerary(Vector3 startingPoint, FlashPedestriansDestination destination, StationController[] stationsNearby, TravelPreference travelPreference)
    {
        Itinerary bestItineraryFound = new Itinerary(new List<StationController>());

        // The default travel time taken into account is the possibility of walking all the way
        float bestTravelTime = Vector3.Distance(startingPoint, destination.transform.position) * secondsPerMeter;

        for (int i = 0; i < stationsNearby.Length; i++)
        {
            for (int j = 0; j < destination.stationsNearThisDestination.Length; j++)
            {
                Itinerary nextItinerary = routingController.GetItinerary(
                     stationsNearby[i],
                     destination.stationsNearThisDestination[j].GetComponent<StationController>(),
                     travelPreference);

                if (nextItinerary != null)
                {
                    // Check the total travel time considering walking times form the starting point to the first station and from th last station to the destination
                    float timeWalkingToFirstStation = Vector3.Distance(startingPoint, nextItinerary.FirstStop.transform.position) * secondsPerMeter;
                    float timeWalkingFromLastStation = Vector3.Distance(nextItinerary.LastStop.transform.position, destination.transform.position) * secondsPerMeter;
                    float travelTime = nextItinerary.GetTotalTravelTime() + timeWalkingToFirstStation + timeWalkingFromLastStation;

                    if (travelTime < bestTravelTime)
                    {
                        bestTravelTime = travelTime;
                        bestItineraryFound = nextItinerary;
                    }
                }
            }
        }

        if (bestItineraryFound == null) Debug.Log("No itinerary found");

        return bestItineraryFound;
    }

    /// <summary>
    /// Subscribes a pedestrian into the informer system.
    /// </summary>
    /// <param name="ped">Pedestrian to subscribe.</param>
    public void SubscribePedestrian(FlashPedestriansController ped)
    {
        activePedestrians.Add(ped.uniqueId, ped);
    }

    /// <summary>
    /// Unsubscribes a pedestrian from the informer system.
    /// </summary>
    /// <param name="ped">Pedestrian to unsubscribe.</param>
    public void UnsuscribePedestrian(FlashPedestriansController ped)
    {
        if (activePedestrians.ContainsKey(ped.uniqueId))
            activePedestrians.Remove(ped.uniqueId);
    }

    /// <summary>
    /// Informs the pedestrians subscribed about the current situation of the routing system on the scene.
    /// </summary>
    public void InformPedestriansSubscribed()
    {
        int itineraryChangedCount = 0;

        foreach (KeyValuePair<int, FlashPedestriansController> P in activePedestrians)
        {
            if (globalParam.percOfPedSubscribed + activePedestrians[P.Key].profile.chanceOfSubscription >= 1.0f)
                if (activePedestrians[P.Key].CheckNewRoutingInfo())
                    itineraryChangedCount += 1;
        }

        accumRouteChanges += itineraryChangedCount * globalParam.numberOfPedestriansPerAgent;
        Debug.Log("Total routes changed> " + accumRouteChanges.ToString());
    }

    /// <summary>
    /// Registers a bike station in the informer. 
    /// </summary>
    /// <param name="bikeStation">Transform of the bike station.</param>
    internal void RegisterBikeStationPosition(BikeStationScript bikeStation)
    {
        if (bikeStations.ContainsKey(bikeStation.GetHashCode()))
            bikeStations[bikeStation.GetHashCode()] = bikeStation;
        else
            bikeStations.Add(bikeStation.GetHashCode(), bikeStation);

    }

    /// <summary>
    /// Attempts to find the closest bike station within a certain radious of a given point. 
    /// </summary>
    /// <param name="point">Point of interest.</param>
    /// <param name="radius">Radious around the point to check bike stations.</param>
    /// <returns>The transform of the closest bike station or null if any was found.</returns>
    internal BikeStationScript FindBikeStationNearby(Vector3 point)
    {
        BikeStationScript closestBikeStation = null;
        float currentDistance = float.MaxValue;

        foreach (KeyValuePair<int, BikeStationScript> K in bikeStations)
        {
            float newDistance = Vector3.Distance(K.Value.transform.position, point);
            if (newDistance < currentDistance && Vector3.Distance(K.Value.transform.position, point) < K.Value.bikeRadius && K.Value.capacityNumber > 0)
            {
                closestBikeStation = K.Value;
                currentDistance = newDistance;
            }
        }

        return closestBikeStation;
    }
}
