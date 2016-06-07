/*
 * 
 * SESTAR INTEGRATION
 * SEStarSmartObject.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class identifies the generic parameters and methods of a SmartObject placed in the Unity scene. Every SmartObject in the Unity scene should have an instance of this class as a component. 
/// </summary>
public class SEStarSmartObject : MonoBehaviour
{    
    /// <summary>
    /// Name of the object.
    /// </summary>
    public string objectName { get; set; }

    /// <summary>
    /// Id of the object.
    /// </summary>
    public uint objectId { get; set; }

    /// <summary>
    /// Type of the object.
    /// </summary>
    public uint objectType { get; set; }

    /// <summary>
    /// Constructor of a Smart Object. 
    /// </summary>
    /// <param name="objectId">Id of the smartObject.</param>
    SEStarSmartObject(uint objectId)
    {
        objectId = this.objectId;
    }
}
