/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedestriansDestination.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Class that defines a destination entry for Flash Pedestrians (composed of a transform, 
/// a float priority and a array of colliders representing the stations nearby). 
/// </summary>
public class FlashPedestriansDestination : MonoBehaviour
{
    public string destinationName;

    public bool hideInUI = false;

    [HideInInspector]
    public Transform destinationTransform;

    [Range(0, 10)]
    public float destinationPriority;

    [HideInInspector]
    public Collider[] stationsNearThisDestination;

    [Range(0, 10000)]
    public float radiousToCheckStations = 100;

    void Awake()
    {
        destinationTransform = this.transform;

        FlashPedestriansGlobalParameters pedGlobalParameters = GetComponent<FlashPedestriansGlobalParameters>();

        // Get the stations near this destination point
        stationsNearThisDestination = Physics.OverlapSphere(destinationTransform.position, radiousToCheckStations, 1 << LayerMask.NameToLayer("Stations"));

        //Debug.Log(this.gameObject.name + " has found " + stationsNearThisDestination.Length 
            //+ " stations nearby");
    }
}
