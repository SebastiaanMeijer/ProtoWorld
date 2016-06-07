/*
 * 
 * SUMO COMMUNICATION
 * SumoTrafficDB.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Implements the traffic DB generated from SUMO.
/// The traffic DB is composed of a list of <see cref=" TimeStepTDB"/>, each of them containing a list of <see cref="VehicleTDB"/>.
/// </summary>
/// <remarks>It inheritates <see cref="TrafficIntegrationData"/> to keep backward compatibility with the old SUMO module.</remarks>
public class SumoTrafficDB : TrafficIntegrationData { }