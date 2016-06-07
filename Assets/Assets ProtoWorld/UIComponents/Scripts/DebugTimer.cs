/*
 * 
 * SUMO COMMUNICATION
 * DebugTimer.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Diagnostics;

/// <summary>
/// Class used for debugging. Defines a common debug timer for all the resources 
/// of the simulation.
/// </summary>
public class DebugTimer : MonoBehaviour 
{
    public Stopwatch debugTimer = new Stopwatch();

    /// <summary>
    /// Initialization of the timer. 
    /// </summary>
	void Awake () 
    {
        debugTimer.Start();
	}
}
