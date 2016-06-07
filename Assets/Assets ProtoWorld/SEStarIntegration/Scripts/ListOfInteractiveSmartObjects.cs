/*
 * 
 * SESTar Integration
 * ListOfInteractiveSmartObjects.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to keep track of all the SEStar SmartObject that are currently interactive in the simulation. The interactive SmartObjects are responsible of registering themselves in this data structure. 
/// </summary>
public class ListOfInteractiveSmartObjects : MonoBehaviour
{
    public Dictionary<string, GameObject> interactiveSmartObjects = new Dictionary<string, GameObject>();
}
