/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * TRAFFIC INTEGRATION
 * TrafficIntegrationVehicle.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Script to control the individual behaviour of an integrated vehicle in the Unity scene. 
/// </summary>
public class TrafficIntegrationVehicle : MonoBehaviour
{
    [HideInInspector]
    public bool isUpdated = true;

    [HideInInspector]
    public bool smooth = false;

    [HideInInspector]
    public float driverVisionLength = 20.0f;

    public SumoMainController mc;

    public bool brakingActive = false;
    public int timeToBrakeInSeconds = 1;
    public float driversPatienceInSeconds = 3.0f;
    public float angleOfView = 90.0f;
    public bool tellMeWhatYouAreDoing = false;

    private List<Vector3> route;
    private int numberOfPathsRouted;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float fractionCovered;
    private float currentPathLength;
    private float startTime;
    private float speed;
    private bool braked = false;
    private float brakingCountdown = 0.0f;
    private float currentAngle = float.MaxValue;

    /// <summary>
    /// Initializes the fields when the script starts. 
    /// </summary>
    void Start()
    {
        // If braking active, the simulation is live with SUMO
        if (brakingActive && mc == null)
            mc = FindObjectOfType<SumoMainController>();

        route = new List<Vector3>();
        route.Add(transform.position);
        numberOfPathsRouted = 0;
        fractionCovered = 1.0f;
        startTime = -1.0f;
        speed = 1.5f;
    }

    /// <summary>
    /// Updates smoothly the position of the vehicle and controls the braking in case there 
    /// is any pedestrian in the way.
    /// </summary>
    /// <seealso cref="GetNextPath"/>
    /// <seealso cref="Vector3.Lerp"/>
    /// <seealso cref="Brake"/>
    void Update()
    {
        if (brakingActive)
        {
            if (brakingCountdown > 0.0f)
            {
                brakingCountdown -= Time.deltaTime;
            }
            else
            {
                if (braked)
                {
                    ResumeVehicleBehaviour();
                }
            }
        }

        // OBSOLETE: Raycasting algorithm for braking cars (Not used anymore)

        //if (brakingActive)
        //{
        //    Ray ray = new Ray(transform.position, transform.forward);
        //    UnityEngine.Debug.DrawRay(transform.position, transform.forward * driverVisionLength);

        //    RaycastHit hit;

        //    if (brakingCountdown > 0.0f)
        //    {
        //        brakingCountdown -= Time.deltaTime;
        //    }
        //    else
        //    {
        //        if (Physics.Raycast(ray, out hit, driverVisionLength))
        //        {
        //            if (hit.collider.tag == "Pedestrian")
        //            {
        //                //UnityEngine.Debug.Log("Vehicle " + this.gameObject.name + " is honking the horn!\n");

        //                if (!braked)
        //                {
        //                    Brake(timeToBrakeInMs);
        //                }
        //                brakingCountdown = driverPatienceInSeconds;
        //            }
        //        }
        //        if (braked)
        //        {
        //            ResumeVehicleBehaviour();
        //        }
        //    }
        //}

        // OBSOLETE: Algorithm for smoothing the paths of the vehicles (Not used anymore)

        //if (smooth)
        //{
        //    if ((Time.time - startTime) < 1.0f)
        //    {
        //        //fractionCovered = ((Time.time - startTime) * speed) / 1.0f;
        //        fractionCovered += Time.deltaTime;

        //        if (smooth)
        //            //transform.position = Vector3.Lerp(startPoint, endPoint, fractionCovered);
        //            transform.position = Vector3.Lerp(startPoint, endPoint, fractionCovered / 1.0f);
        //        else
        //            transform.position = endPoint;

        //        //if (this.gameObject.name == "5" && fractionCovered >= 1.0f)
        //        //{
        //        //    UnityEngine.Debug.Log("Smoothing completed for veh5 from timestep " + (numberOfPathsRouted - 1) + " to " + numberOfPathsRouted + " at " + debugTimer.ElapsedMilliseconds + "\n");
        //        //}
        //    }
        //    else
        //    {
        //        transform.position = endPoint;
        //        GetNextPath();
        //    }
        //}
    }

    /// <summary>
    /// Updates immediately the vehicle position.
    /// </summary>
    /// <param name="x">x position (Unity coordinates).</param>
    /// <param name="z">z position (Unity coordinates).</param>
    /// <param name="angle"> angle of the vehicle.</param>
    /// <remarks>The points are all set in the plane y=0.</remarks>
    internal void UpdateVehiclePosition(float x, float z, float angle)
    {
        //Move to the new position
        Vector3 newPosition = new Vector3(x, 0.0f, z);
        transform.position = newPosition;

        //Fix the rotation
        //Note: There are no conventions for angles between different 
        //simulations, so car rotations may appear wrong in the simulation. 
        //If that is the case, fix it in the following lines:
        transform.rotation = Quaternion.Euler(0, 90.0f, 0);
        //this.transform.Rotate(Vector3.up, angle + 180);
        this.transform.Rotate(Vector3.up, -angle);

        if (tellMeWhatYouAreDoing)
            UnityEngine.Debug.Log("Vehicle " + this.name + " is rotating: " + angle);
    }

    /// <summary>
    /// If moving, brakes the vehicle to speed 0 in the given milliseconds.
    /// </summary>
    /// <param name="milliseconds">Milliseconds given to stop the vehicle.</param>
    internal void Brake(int milliseconds)
    {
        if (!braked)
        {
            if (mc != null)
                mc.ChangeVehicleSpeed(this.name, 0.0d, milliseconds);
            braked = true;
        }
    }

    /// <summary>
    /// If braked, resumes the normal vehicle behaviour.
    /// </summary>
    internal void ResumeVehicleBehaviour()
    {
        if (braked)
        {
            if (mc != null)
                mc.ResumeVehicleBehaviour(this.name);
            braked = false;
        }
    }

    /// <summary>
    /// Handles the situations in which a pedestrian is crossing the road near the position of the car, 
    /// forcing it to stop in case the driver can see the pedestrian. 
    /// </summary>
    /// <param name="pedestrianPosition">Position of the pedestrian crossing the road.</param>
    public void PedestrianCrossingRoad(Vector3 pedestrianPosition)
    {
        if (brakingActive)
        {

            //Calculates the angle between the frontal vector of the car and the pedestrian
            float angle = Vector3.Angle(pedestrianPosition - this.transform.position, this.transform.forward);

            if (tellMeWhatYouAreDoing)
            {
                //UnityEngine.Debug.Log(this.name + ": I see a pedestrian!");
                //UnityEngine.Debug.Log("Angle: " + angle);
            }

            //Brakes if the driver can see the pedestrian
            if (angle <= angleOfView)
            {
                if (!braked)
                {
                    Brake(timeToBrakeInSeconds * 1000);
                }
                brakingCountdown = driversPatienceInSeconds;
            }
        }
    }

    /// <summary>
    /// [OBSOLETE]
    /// Add another point to the route of the vehicle.
    /// </summary>
    /// <param name="x">x position (Unity coordinates).</param>
    /// <param name="z">z position (Unity coordinates).</param>
    /// <remarks>The points are all set in the plane y=0.</remarks>
    [System.Obsolete]
    internal void AddRoutePoint(float x, float z)
    {
        route.Add(new Vector3(x, 0.0f, z));
    }

    /// <summary>
    /// [OBSOLETE]
    /// Gets the next destiny point of the route and sets the parameters needed for
    /// smoothing the path through linear interpolation.
    /// </summary>
    /// <seealso cref="Vector3.Lerp"/>
    [System.Obsolete]
    private void GetNextPath()
    {
        if (numberOfPathsRouted < route.Count - 1)
        {
            startPoint = transform.position;
            //endPoint = route[route.Count-1];
            endPoint = route[++numberOfPathsRouted];
            transform.LookAt(endPoint);

            startTime = Time.time;
            fractionCovered = 0.0f;
            //currentPathLength = Vector3.Distance(startPoint, endPoint);

            //fractionCovered = ((Time.time - startTime) * speed) / 1.0f;
            fractionCovered += Time.deltaTime;

            if (smooth)
                //transform.position = Vector3.Lerp(startPoint, endPoint, 0.0f);
                transform.position = Vector3.Lerp(startPoint, endPoint, fractionCovered / 1.0f);
            else
                transform.position = endPoint;
        }
    }

    /// <summary>
    /// [OBSOLETE]
    /// Gets the number of paths that the vehicle has to complete to reach the last point. 
    /// </summary>
    /// <returns>Integer with the number of paths yet to complete.</returns>
    [System.Obsolete]
    internal int GetNoPathsToReachLastPoint()
    {
        if (route == null)
            return 0;
        else
            return (route.Count - numberOfPathsRouted);
    }
}
