/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedestriansGlobalParameters.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Definition of the global parameters for the Flash Pedestrian Simulator. 
/// </summary>
public class FlashPedestriansGlobalParameters : MonoBehaviour
{
    /// <summary>
    /// Percentage of Italian speakers. 
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float percOfItalianSpeakers = 0.5f;

    /// <summary>
    /// Percentage of English speakers. 
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float percOfEnglishSpeakers = 0.5f;

    /// <summary>
    /// Percentage of pedestrians subscribed to the itinerary informer. 
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float percOfPedSubscribed = 0.5f;

    /// <summary>
    /// Percentage of pedestrians willing to change destination. 
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float percOfPedWillingToChangeDestination = 0.5f;

    /// <summary>
    /// Average speed for pedestrians. 
    /// </summary>
    [Range(0.0f, 20.0f)]
    public float averageSpeed = 1.5f;

    /// <summary>
    /// Number of pedestrians represented by a single agent.
    /// </summary>
    [Range(1, 100)]
    public int numberOfPedestriansPerAgent = 1;

    /// <summary>
    /// Bool that enables awareness between pedestrians and SUMO cars. 
    /// </summary>
    public bool sumoCarAwarenessEnabled = true;

    /// <summary>
    /// Bool that enables rumours between pedestrians. 
    /// </summary>
    public bool rumoursEnabled = true;

    /// <summary>
    /// Percentage of pedestrians susceptible to rumours. 
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float percOfPedSusceptibleToRumours = 0.5f;

    /// <summary>
    /// Radious within the rumours will be spread. 
    /// </summary>
    [Range(0.0f, 100.0f)]
    public float radiusOfSpreadingRumours = 50.0f;

    /// <summary>
    /// Bool that enables bike choice for pedestrians. 
    /// </summary>
    public bool bikesEnabled = true;

    /// <summary>
    /// Percentage of pedestrians willing to take a bike. 
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float percOfPedTakingBikes = 0.5f;

    /// <summary>
    /// Enumerates the possible weather conditions on the scene. 
    /// </summary>
    public enum WeatherConditions
    {
        DefaultWeather,
        SunnyWeather,
        WindyWeather,
        RainyWeather
    }

    /// <summary>
    /// Current weather condition.
    /// </summary>
    public WeatherConditions currentWeatherCondition = WeatherConditions.SunnyWeather;

    /// <summary>
    /// The game object that defines a Flash Pedestrian agent. 
    /// </summary>
    public GameObject pedestrianObject;

    /// <summary>
    /// Tune the navigation Mesh at start.
    /// </summary>
    void Start()
    {
        NavMesh.avoidancePredictionTime = 0.1f;
        NavMesh.pathfindingIterationsPerFrame = 1000;
    }

    /// <summary>
    /// Property for playing/pausing the pedestrian simulation in game
    /// </summary>
    public bool flashPedestriansPaused = false;

    /// <summary>
    /// Property for KPIs: Shows the number of pedestrians currently running on the scenario.
    /// </summary>
    public int numberOfPedestriansOnScenario = 0;

    /// <summary>
    /// Property for KPIs: Shows the number of pedestrians that have reached their destination.
    /// </summary>
    public int numberOfPedestrianReachingDestination = 0;
}
