/*
 * 
 * SESTAR INTEGRATION
 * SEStarSyntheticEntity.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class identifies the generic parameters and methods of a SyntheticEntity placed in the Unity scene. Every SyntheticEntity in the Unity scene should have an instance of this class as a component. 
/// </summary>
public class SEStarSyntheticEntity : MonoBehaviour
{
    /// <summary>
    /// Type of the pedestrian.
    /// </summary>
    public string pedestrianType;

    /// <summary>
    /// Constructor of a Synthetic Entity. 
    /// </summary>
    /// <param name="pedestrianType">Type of the pedestrian.</param>
    SEStarSyntheticEntity(string pedestrianType)
    {
        pedestrianType = this.pedestrianType;
    }
}
