/*
 * 
 * PEDESTRIANS KTH
 * PedestriansKTHConfig.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Sets the configuration parameters for the pedestrian simulation. 
/// </summary> 
public class PedestriansKTHConfig : MonoBehaviour 
{
    [Range(0, 1000)]
    public int numberOfPedestrians = 100;
    
    [Range(0.1f, 1.0f)]
    public float speedVariability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float studyInterestVariability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float subscriptionProbability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float rumourSusceptibilityVariability = 0.6f;
    
    [Range(0.1f, 1.0f)]
    public float delayToleranceVariability = 0.6f;
    
    [Range(0.1f,1.0f)]
    public float spawningFrequency = 0.5f;

    public bool useCoordinateConversion = true;
    public bool showFloatingBalloons = true;
    public bool useDatabaseForSpawning = true;
    public bool spawnOnlyDefaultModel = true;
    public bool sendInfoToPedestriansSuscribed = true;
}
