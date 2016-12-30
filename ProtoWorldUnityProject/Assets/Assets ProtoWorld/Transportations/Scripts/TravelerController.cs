/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A simple implementation of a traveler traveling in the transportation-system
/// and "walk" to the next destionation if required.
/// </summary>
public class TravelerController : MonoBehaviour
{
    private static long counter = 0;
	public long travelerId;

    public Transform destination;
    public Itinerary itinerary;
    public int wayPointIndex;
    
    public float speed = 10;
    private float startTime;
    private Vector3 startPosition;
    private float journeyLength;
    private MeshRenderer meshRenderer;

    public List<StationController> stations;

    /// <summary>
    /// To create a gameobject with this script attached to it.
    /// </summary>
    /// <param name="itinerary">The itinerary that this traveler will follow</param>
    /// <param name="destination">The final destination</param>
    /// <returns></returns>
    public static GameObject CreateGameObject(Itinerary itinerary, Transform destination = null)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Passenger " + counter++;
        obj.transform.localScale *= 0.4f;
        var controller = obj.AddComponent<TravelerController>();
        controller.travelerId = FlashPedestriansGlobalParameters.travelerId;
        if (destination == null)
            controller.destination = itinerary.LastStop.transform;
        else
            controller.destination = destination;
        controller.itinerary = itinerary;
        controller.wayPointIndex = 0;

        obj.GetComponent<Collider>().enabled = false;

        return obj;
    }

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.red;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRenderer.useLightProbes = false;

        Reset();
        stations = itinerary.WayPoints;
    }

    /// <summary>
    /// Traveler will move towards the next waypoint,
    /// If it has reached the current destination, ArrivedAt(station) will be called to decide what to do next.
    /// If the current destination is the final destination, the gameobject will be deactivated.
    /// </summary>
    void Update()
    {
        if (!meshRenderer.enabled)
            return;

        if (Vector3.Distance(transform.position, CurrentDestination()) < 1)
        {
            if (CurrentDestination() == destination.transform.position)
            {
                SetAnimation(false);
                gameObject.SetActive(false);
                return;
            }

            StationController station = CurrentStop();
            if (station != null)
            {
                SetAnimation(false);
                ArrivedAt(station);
                return;
            }
        }

        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPosition, CurrentDestination(), fracJourney);
    }

    /// <summary>
    /// Return the keyString to which queue the traveler should go at the station.
    /// </summary>
    /// <returns></returns>
    public virtual string GetNextLineQueueId()
    {
        var info = itinerary.StageInfos[wayPointIndex];
        return LineController.MakeKeyString(info.Line.id, info.Direction);
    }

    /// <summary>
    /// Get the stations when the traveler will transit.
    /// </summary>
    /// <returns></returns>
    public virtual List<StationController> GetStagesTransits()
    {
        return itinerary.Transits;
    }

    /// <summary>
    /// When the traveler arrived to the station, 
    /// it has to decide how to get to the next waypoint or its final destination.
    /// </summary>
    /// <param name="station"></param>
    public virtual void ArrivedAt(StationController station)
    {
        wayPointIndex = itinerary.FindStopIndex(station);
        SetAnimation(false);

        startPosition = station.transform.position;

        CheckInfo();

        if (wayPointIndex < itinerary.StageInfos.Count)
        {
            var info = itinerary.StageInfos[wayPointIndex];
            //Debug.Log(info);
            if (info.Category == LineCategory.Walk)
            {
                ++wayPointIndex;
                startTime = Time.time;
                journeyLength = Vector3.Distance(startPosition, CurrentDestination());
                SetAnimation(true);
                return;
            }
        }
        else
        {
            // The last part of walking!
            SetAnimation(true);
            return;
        }

        CurrentStop().QueueTraveler(this);
    }

    /// <summary>
    /// Return the last station of the itinerary.
    /// </summary>
    /// <returns></returns>
    public StationController LastStop()
    {
        return itinerary.LastStop;
    }

    /// <summary>
    /// Get the next Station, return null if there is no next stop.
    /// </summary>
    /// <returns></returns>
    public virtual StationController CurrentStop()
    {
        return itinerary.GetStop(wayPointIndex);
    }

    /// <summary>
    /// Return the position of the next Station,
    /// if the passenger has finished the route,
    /// the destionationPoint is returned.
    /// </summary>
    /// <returns></returns>
    public Vector3 CurrentDestination()
    {
        StationController s = CurrentStop();
        if (s != null)
            return CurrentStop().transform.position;
        return destination.position;
    }

    public override string ToString()
    {
        return gameObject.name + ", itinerary: " + itinerary;
    }

    /// <summary>
    /// Will set the meshRenderer to the boolean-value.
    /// </summary>
    /// <param name="turnOn"></param>
    public void SetAnimation(bool turnOn)
    {
        meshRenderer.enabled = turnOn;
    }

    internal virtual void Embark()
    {
        // Implemented in subclass FlashPedestrianController
    }

    private void CheckInfo()
    {
        //throw new NotImplementedException();
    }

	public long getTravelerId(){
		return travelerId;
	}

    /// <summary>
    /// Reset the parameters to animate the walk to the next destination.
    /// </summary>
    public virtual void Reset()
    {
        startTime = Time.time;
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, CurrentDestination());
    }
}
