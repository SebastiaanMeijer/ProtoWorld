/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * PEDESTRIANS KTH
 * PedestrianKTHSteering.cs
 * Miguel Ramos Carretero
 * Edited by Furkan Sonmez
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Implements the steering behaviour of a pedestrian.
/// </summary>
/// <remarks>The steering behaviour is based on a navigation mesh.</remarks>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class PedestrianKTHSteering : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent navAgent;

    [HideInInspector]
    public bool tellMeWhatYouAreDoing = false;

    [Range(3.0f, 10.0f)]
    public float speedInBike = 3.0f;

    [Range(0.0f, 1.0f)]
    public float speedVariation;

    [Range(0.1f, 5.0f)]
    public float timeBetweenCollisions = 0.5f;

    [Range(0.1f, 5.0f)]
    public float accelerationInRoad = 2.0f;

    public Transform busStation;
    public Transform metroStation;
    public Transform[] studyPoints;
    public Transform[] classrooms;
    public Transform[] walkingDestinations;
    public bool isVisible = false;
    public float collisionTimer = 0.0f;
    public bool collided = false;

    private bool onRoad = false;
    private bool agentWalking = false;
    private bool goingToEndPoint = false;
    public AIControllerWithLOD aiController;
    public string goingTo;


    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        navAgent.speed = 1.5f; // +Random.Range(0.0f, speedVariation * 4.0f);
        aiController = gameObject.GetComponent<AIControllerWithLOD>();
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        if (agentWalking)
        {
            if (onRoad)
            {
                if (aiController != null) aiController.SetAnimationParameter("Speed", 2.0f);

            }
            else
            {
                if (aiController != null) aiController.SetAnimationParameter("Speed", 0.5f);
            }

            if (PathCompleted() && this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode != -3 && navAgent.destination.x != this.gameObject.GetComponent<ClosestBikeStation>().closestBikeStationPos.x)
            {
                if (aiController != null) aiController.SetAnimationParameter("Speed", 0.0f);

                if (goingToEndPoint)
                {
                    //Debug.LogError (navAgent.destination + " may be same as " + this.gameObject.GetComponent<ClosestBikeStation>().closestBikeStationPos);
					//Added by Furkan, this will make the endReached bool true in ClosestBikeStation, so the other scripts know that the end has been reached
                    this.gameObject.GetComponent<ClosestBikeStation>().endReached = true;
                    gameObject.GetComponent<Animator>().SetBool("endReached", true);
                }

                agentWalking = false;
                isVisible = false;
                Hide();

                gameObject.GetComponent<NavMeshAgent>().enabled = false;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<PedestrianKTHController>().floatingBalloon.gameObject.SetActive(false);
            }
        }

        if (collisionTimer > 0.0f)
            collisionTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Checks if the path has been completed. 
    /// </summary>
    /// <returns>True if the path is completed, False otherwise.</returns>
    public bool PathCompleted()
    {
        if (Vector3.Distance(navAgent.destination, this.transform.position)
            <= navAgent.stoppingDistance)
        {
            if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Sets a new destination for the pedestrian.
    /// </summary>
    /// <param name="destination">String with the name of the new destination.</param>
    internal void SetNewDestination(string destination)
    {
        try
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = true;

            switch (destination)
            {
                case "Bus":
                    navAgent.ResetPath();
                    agentWalking = true;
                    if (tellMeWhatYouAreDoing) Debug.Log("Going to bus: " + busStation.position.ToString());
                    navAgent.SetDestination(busStation.position);
                    isVisible = true;
                    Show();
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    goingToEndPoint = true;
                    goingTo = "Bus";
                    break;

                case "Metro":
                    navAgent.ResetPath();
                    agentWalking = true;
                    if (tellMeWhatYouAreDoing) Debug.Log("Going to metro: " + metroStation.position.ToString());
                    navAgent.SetDestination(metroStation.position);
                    isVisible = true;
                    Show();
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    goingToEndPoint = true;
                    goingTo = "Metro";
                    break;

                case "Bicycle":
                    navAgent.ResetPath();
                    agentWalking = true;
                    Vector3 w1 = walkingDestinations[Random.Range(0, walkingDestinations.Length)].position;

					if(w1 == walkingDestinations[6].position){
						
						w1 = walkingDestinations[4].position;
					}
				
                    if (tellMeWhatYouAreDoing) Debug.Log("Going cycling: " + w1.ToString());
                    navAgent.speed = speedInBike + Random.Range(-0.5f, speedVariation);
                    navAgent.SetDestination(w1);
                    isVisible = true;
                    Show();
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    goingToEndPoint = true;
                    goingTo = "Bicycle";
                    break;

                case "Walk":
                    navAgent.ResetPath();
                    agentWalking = true;
                    Vector3 w2 = walkingDestinations[Random.Range(0, walkingDestinations.Length)].position;

					if(w2 == walkingDestinations[6].position){
						
						w2 = walkingDestinations[4].position;
					}


                    if (tellMeWhatYouAreDoing) Debug.Log("Going walking: " + w2.ToString());
                    navAgent.SetDestination(w2);
                    isVisible = true;
                    Show();
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    goingToEndPoint = true;
                    goingTo = "Walk";
                    break;

                case "Library":
                    navAgent.ResetPath();
                    agentWalking = true;
                    Vector3 w3 = ChooseClosestStudyPoint();
                    if (tellMeWhatYouAreDoing) Debug.Log("Going to study: " + w3.ToString());
                    navAgent.SetDestination(w3);
                    isVisible = true;
                    Show();
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    goingToEndPoint = false;
                    goingTo = "Library";
                    break;

                //Added by Furkan
                //It makes the pedestrian walk to the closest bikestation first
                case "ToBikeStation":
                    navAgent.ResetPath();
                    agentWalking = true;
				//it sets the bikestationposition to the position of the walkingDestination[6] because the position is stored there
                    Vector3 w4 = walkingDestinations[6].position;

                    if (tellMeWhatYouAreDoing) Debug.Log("Going walking: " + w4.ToString());
                    navAgent.SetDestination(w4);
                    isVisible = true;
                    Show();
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    goingToEndPoint = true;
                    goingTo = "ToBikeStation";
                    break;

                default:
                    //Destination not set: do nothing.
                    break;
            }
        }
        catch
        {
            UnityEngine.Debug.Log("Error with pedestrian: removing pedestrian from the system!");
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Chooses the closest study point from the current position of the pedestrian.
    /// </summary>
    /// <returns>Vector3 with the position of the closest study point.</returns>
    public Vector3 ChooseClosestStudyPoint()
    {
        float dist = float.MaxValue;
        float auxDist;
        int indexClosestPoint = 0;

        for (int i = 0; i < studyPoints.Length; i++)
        {
            auxDist = Vector3.Distance(navAgent.transform.position, studyPoints[i].position);

            if (auxDist < dist)
            {
                dist = auxDist;
                indexClosestPoint = i;
            }
        }

        return studyPoints[indexClosestPoint].position;
    }

    /// <summary>
    /// Handles the collision when a pedestrian enters in a traffic road. 
    /// </summary>
    /// <param name="col">Collider with the information of the collision</param>
    private void OnTriggerEnter(Collider col)
    {
        onRoad = true;
        navAgent.speed += accelerationInRoad;
    }

    /// <summary>
    /// Handles the collision when a pedestrian enters in a traffic road. This method checks for vehicles
    /// around and informs them about the position of the pedestrian. 
    /// </summary>
    /// <param name="col">Collider with the information of the collision</param>
    private void OnTriggerStay(Collider col)
    {
        if (collisionTimer <= 0.0f)
        {
            if (tellMeWhatYouAreDoing)
                Debug.Log(this.name + ": I got a collision with " + col.name);

            collisionTimer = timeBetweenCollisions;

            Collider[] vehiclesAround = Physics.OverlapSphere(this.transform.position, 20.0f, LayerMask.GetMask("Vehicle"));

            if (tellMeWhatYouAreDoing)
                Debug.Log(this.name + ": There are " + vehiclesAround.Length + " vehicles inside the sphere");

            collided = (vehiclesAround.Length != 0);

            foreach (var v in vehiclesAround)
            {
                if (tellMeWhatYouAreDoing)
                    Debug.Log("Sending message to vehicle " + v.name);

                v.SendMessage("PedestrianCrossingRoad", this.transform.position);
            }
        }
    }

    /// <summary>
    /// Handles the collision when a pedestrian exits a traffic road. 
    /// </summary>
    /// <param name="col">Collider with the information of the collision</param>
    private void OnTriggerExit(Collider col)
    {
        onRoad = false;
        navAgent.speed -= accelerationInRoad;
    }

    /// <summary>
    /// Draw a gizmo in the pedestrian informing for cars around it. 
    /// </summary>
    void OnDrawGizmos()
    {
        if (onRoad && collided)
            Gizmos.DrawWireSphere(this.transform.position, 20.0f);
    }

    /// <summary>
    /// Makes the game object visible.
    /// </summary>
    public void Show()
    {
        if (isVisible)
        {
            if (tellMeWhatYouAreDoing) Debug.Log("Showing! " + this.name);

            if (aiController != null)
                aiController.Visible = true;
            else
                GetComponent<Renderer>().enabled = true;
        }
    }

    /// <summary>
    /// Makes the game object not visible.
    /// </summary>
    public void Hide()
    {
        if (tellMeWhatYouAreDoing) Debug.Log("Hiding! " + this.name);

        if (aiController != null)
            aiController.Visible = false;
        else
            GetComponent<Renderer>().enabled = false;
    }

    /// <summary>
    /// Sends the pedestrian model away from the scene, but it does not destroy the object.
    /// </summary>
    public void SendToLimbo()
    {
        this.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }
}