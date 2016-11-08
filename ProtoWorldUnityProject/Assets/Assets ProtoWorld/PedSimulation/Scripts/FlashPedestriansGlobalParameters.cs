/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

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
    /// Percentage of pedestrians willing to take transport. 
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float percOfPedWillingToTakeTransport = 0.5f;

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
    public GameObject[] pedestrianObject;

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

    /// <summary>
    /// Tune the navigation Mesh at start.
    /// </summary>
    void Start()
    {
        NavMesh.avoidancePredictionTime = 0.01f;
        NavMesh.pathfindingIterationsPerFrame = 5000;
    }
}
