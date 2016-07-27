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
 * PedestrianKTHController.cs
 * Miguel Ramos Carretero
 * Edited by Furkan Sonmez
 * 
 */

using UnityEngine;
using System.Collections;
using System.Linq;
using GaPSLabsUnity.StateMachine;

/// <summary>
/// Implements the behaviour that controls the decisions of a pedestrian.
/// </summary>
/// <remarks>This decision is control based on a finite state machine (Extension of the class StateMachineBase.cs).</remarks>
/// <see cref="StateMachineBase"/>
[RequireComponent(typeof(PedestrianKTHSteering))]
[RequireComponent(typeof(PedestrianKTHKnowledge))]
public class PedestrianKTHController : StateMachineBase
{
    [HideInInspector]
    public PedestrianKTHSteering steering;

    [HideInInspector]
    public PedestrianKTHKnowledge knowledge;

    [HideInInspector]
    public Transform floatingBalloon;

    //(Furkan)This is the destination the pedestrian wants to go to eventually(for example after getting a bike at the bikestation)
    public Vector3 finalDestination;

    public bool tellMeWhatYouAreDoing = false;
    public bool floatingBalloonsEnabled = true;

    private Queue pedestrianCache;

    /// <summary>
    /// Initializes the script. 
    /// </summary>
    /// 
    /// 

    public override void Init()
    {
        steering = gameObject.GetComponent<PedestrianKTHSteering>();

        knowledge = gameObject.GetComponent<PedestrianKTHKnowledge>();

        pedestrianCache = gameObject.transform.parent.GetComponent<PedestrianKTHSpawner>().pedestrianCache;

        floatingBalloon = transform.FindChild("ThinkingBalloon");

        onStateEnterEvent += onStateEnter;
        onStateExitEvent += onStateExit;
        onStateStayEvent += onStateStay;

        InvokeRepeating("CheckMyKnowledge", 1, 1);

    }

    /// <summary>
    /// Update method. 
    /// </summary>
    new void Update()
    {
        base.Update();

        knowledge.tellMeWhatYouAreDoing = tellMeWhatYouAreDoing;
        steering.tellMeWhatYouAreDoing = tellMeWhatYouAreDoing;
    }

    /// <summary>
    /// Implements the behaviour when the finite state machine stays in a certain state. 
    /// </summary>
    /// <param name="stateName">Name of the state in which the FSM stays.</param>
    void onStateStay(string stateName)
    {
        switch (stateName)
        {
            case "Classroom":
                {
                    //Check the current lecture
                    LectureInfo lecInfo = knowledge.CurrentLecture();

                    //Leave class if it's over or if the interest is not big enough
                    if (lecInfo == null)
                        animator.SetBool("leaveClass", true);
                    else
                    {
                        bool leaveClass = (lecInfo.GetTimeEnds() - Time.time) <= (1 - knowledge.studyInterest) * (lecInfo.GetTimeEnds() - lecInfo.GetTimeStarts());
                        animator.SetBool("leaveClass", leaveClass);
                    }
                    break;
                }

            case "Library":
                {
                    //Stay at the library until timeToGo
                    bool leaveLibrary = Time.time > knowledge.timeToGoToMode;
                    animator.SetBool("modeAvailableSoon", leaveLibrary);
                    break;
                }

            default:
                {
                    //Do nothing
                    break;
                }
        }
    }

    /// <summary>
    /// Implements the behaviour when the finite state machine enters to a certain state. 
    /// </summary>
    /// <param name="stateName">Name of the state that the FSM is entering.</param>
    public void onStateEnter(string stateName)
    {
        switch (stateName)
        {
            case "Classroom":
                {
                    //Disable mesh visualization
                    steering.isVisible = false;

                    gameObject.GetComponent<BoxCollider>().enabled = false;
                    gameObject.GetComponent<NavMeshAgent>().enabled = false;
                    break;
                }

            case "ModeChoice":
                {
                    if (PedestrianKTHSpawner.activateBikeStations)
                    {
                        //Modified by Furkan
                        //This part of the code is to store what the final destination of the pedestrian will be. 
                        //This position will be used later after the bike station has been reached
                        if (knowledge.mode == 0)
                            finalDestination = steering.walkingDestinations[4].position;
                        else if (knowledge.mode == 1)
                            finalDestination = steering.busStation.position;
                        else if (knowledge.mode == 2)
                            finalDestination = steering.metroStation.position;
                        else if (knowledge.mode == 3)
                            finalDestination = steering.walkingDestinations[4].position;



                        //Execute the function in ClosestBikeStation
                        ClosestBikeStation bs = this.gameObject.GetComponent<ClosestBikeStation>();

                        if (bs != null)
                            bs.bikeStationDecision();
                    }
                    else
                    {
                        //Bikestations system not active, thus go the regular way
                        animator.SetInteger("mode", knowledge.mode);
                    }
                    break;
                }

            case "Library":
                {
                    //Calculate timeToGo
                    CalculateWhenToGoToMode();

                    if (tellMeWhatYouAreDoing)
                        Debug.Log("I will stay at the library until: " + knowledge.timeToGoToMode);

                    gameObject.GetComponent<NavMeshAgent>().enabled = true;

                    //Set the new destination in path finding
                    steering.SetNewDestination(stateName);
                    break;
                }

            case "Bus":
            case "Metro":
                {
                    //Check if there is a mode available soon
                    animator.SetBool("modeAvailableSoon", IsModeAvailableSoon());

                    gameObject.GetComponent<NavMeshAgent>().enabled = true;

                    //Set the new destination in path finding
                    steering.SetNewDestination(stateName);
                    break;
                }

            default:
                {
                    gameObject.GetComponent<NavMeshAgent>().enabled = true;

                    //Set the new destination in path finding
                    steering.SetNewDestination(stateName);

                    break;
                }
        }

        // Visualization of floating balloons
        UpdateFloatingBalloon(stateName);

    }

    /// <summary>
    /// Implements the behaviour when the finite state machine exits a certain state. 
    /// </summary>
    /// <param name="stateName">Name of the state that the FSM is exiting.</param>
    public void onStateExit(string stateName)
    {
        switch (stateName)
        {
            case "Bus":
            case "Metro":
                {
                    if (CurrentState == "End")
                    {
                        //Reads ModeDB information at the station
                        knowledge.UpdateKnowledgeFromModeDB();

                        //Takes the next available mode
                        ModeInfo m = knowledge.NextAvailableMode();

                        // Decide if the pedestrian takes the next mode or leaves the station
                        if (m != null)
                        {
                            //Check if there is a mode available soon
                            bool modeAvailableSoon = IsModeAvailableSoon();
                            animator.SetBool("modeAvailableSoon", modeAvailableSoon);

                            if (tellMeWhatYouAreDoing)
                                Debug.Log("My decision to stay at the station: " + modeAvailableSoon);

                            //If there is no mode available soon, spread the rumour around
                            if (!modeAvailableSoon)
                            {
                                RumourInfo rInfo = new RumourInfo("rumour" + m.GetId(), m, 1.0f);
                                knowledge.rumour = new Tuple<RumourInfo, int>(rInfo, knowledge.mode);
                                knowledge.InvokeRepeating("SpreadRumour", 1, 1.0f + Random.Range(0.0f, 1.0f));
                            }
                            else
                            {
                                this.transform.FindChild("bike").gameObject.SetActive(false);

                                //Stay at the station, put in pedestrian cache for reuse
                                pedestrianCache.Enqueue(this.gameObject);

                                //Send the pedestrian model away from the end point
                                steering.SendToLimbo();
                            }
                        }
                        else
                        {
                            if (tellMeWhatYouAreDoing)
                                Debug.Log("I lost the last mode...");

                            //I lost the last mode! Go to the library then...
                            animator.SetBool("modeAvailableSoon", false);
                        }
                    }
                    break;
                }

            case "Walk":
            case "ToBikeStation": // added by furkan
            case "Bicycle":
                {
                    this.transform.FindChild("bike").gameObject.SetActive(false);

                    //Stay there, put in pedestrian cache for reuse
                    pedestrianCache.Enqueue(this.gameObject);

                    //Send the pedestrian model away from the end point
                    steering.SendToLimbo();

                    break;
                }

            case "End":
                {
                    //It is not the end of the trip for this pedestrian
                    animator.SetBool("endReached", false);
                    break;
                }

            default:
                {
                    //Do nothing
                    break;
                }
        }
    }

    /// <summary>
    /// Private auxiliar method. From the knowledge of the pedestrian, checks if there is a mode available soon.
    /// </summary>
    /// <returns>True if there is a mode available soon, False otherwise.</returns>
    private bool IsModeAvailableSoon()
    {
        float distanceToMode;
        ModeInfo nextMode = knowledge.NextAvailableMode();

        if (nextMode != null)
        {
            float timeOfArrival = nextMode.GetTimeOfArrival();

            //Calculates the distance to the mode station
            if (knowledge.mode == 1)
                distanceToMode = Vector3.Distance(this.transform.position,
                    steering.busStation.position);
            else if (knowledge.mode == 2)
                distanceToMode = Vector3.Distance(this.transform.position,
                    steering.metroStation.position);
            else
                return true;

            //Calculates the time remaining until the mode arrival
            float timeRemaining = timeOfArrival - (distanceToMode / steering.navAgent.speed) - Time.time;

            //Check if mode will be available soon depending on the pedestrian tolerance for waiting 
            bool modeAvailableSoon =
                (timeRemaining > 0) && (timeOfArrival < (Time.time + knowledge.delayTolerance * 3600.0f));

            //Mark mode as missed if the pedestrian cannot make it to the station in time
            if (!modeAvailableSoon)
            {
                knowledge.MarkModeAsMissed(nextMode.index);
            }

            if (tellMeWhatYouAreDoing)
            {
                Debug.Log("Time of arrival of next mode: " + timeOfArrival);
                Debug.Log("Time right now: " + Time.time);
                Debug.Log("My tolerance: " + knowledge.delayTolerance * 3600.0f);
                Debug.Log("Distance to mode: " + distanceToMode / steering.navAgent.speed);
                Debug.Log("Time I have to arrive there: " + timeRemaining);
            }

            return modeAvailableSoon;
        }
        else
            return false;
    }

    /// <summary>
    /// Private auxiliar method. Calculates the time to go to the mode station from the closest study point in order to arrive there in time.
    /// </summary>
    private void CalculateWhenToGoToMode()
    {
        ModeInfo m = knowledge.NextAvailableMode();

        if (m != null)
        {
            knowledge.timeToGoToMode = m.GetTimeOfArrival() - (Vector3.Distance(steering.ChooseClosestStudyPoint(), steering.metroStation.position) / steering.navAgent.speed);
            knowledge.timeToGoToMode -= 600; //leave 10 min before 
        }
        else
            //There are no more modes so far, I'll stay at the library! 
            knowledge.timeToGoToMode = float.MaxValue;
    }

    /// <summary>
    /// Periodic method. Check the knowledge of the pedestrian and update the behaviour parameters accordingly.  
    /// </summary>
    private void CheckMyKnowledge()
    {
        //Check if there is any change in the pedestrian knowledge
        if (knowledge.newKnowledgeToBeCheckedbyController)
        {
            if (tellMeWhatYouAreDoing)
                Debug.Log("Updating knowledge!\n");

            switch (CurrentState)
            {
                case "Bus":
                case "Metro":
                    animator.SetBool("modeAvailableSoon", IsModeAvailableSoon());
                    break;

                case "Library":
                    CalculateWhenToGoToMode();
                    break;

                default:
                    break;
            }

            knowledge.newKnowledgeToBeCheckedbyController = false;
        }
    }

    /// <summary>
    /// Updates the floating balloon of the pedestrian acording to its current state.
    /// <param name="stateName">Name of the current state of the FSM.</param>
    private void UpdateFloatingBalloon(string stateName)
    {
        //Deactivate old floating balloon
        floatingBalloon.gameObject.SetActive(false);

        switch (stateName)
        {
            case "Classroom":
                {
                    floatingBalloon = transform.FindChild("ClassroomBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            case "ModeChoice":
                {
                    floatingBalloon = transform.FindChild("ThinkingBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            case "Library":
                {
                    floatingBalloon = transform.FindChild("LibraryBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            case "Bus":
                {
                    floatingBalloon = transform.FindChild("BusBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            case "Metro":
                {
                    floatingBalloon = transform.FindChild("MetroBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            case "Walk":
                {
                    floatingBalloon = transform.FindChild("WalkBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            //Added by Furkan
            case "ToBikeStation":
                {
                    floatingBalloon = transform.FindChild("WalkBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            case "Bicycle":
                {
                    floatingBalloon = transform.FindChild("BicycleBalloon");
                    floatingBalloon.gameObject.SetActive(floatingBalloonsEnabled);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    /// <summary>
    /// Toogle the floating balloon depending on the given boolean.
    /// </summary>
    /// <param name="active">specify if the floating balloon should be active.</param>
    public void ToogleBalloon(bool active)
    {
        floatingBalloonsEnabled = active;

        if (floatingBalloon != null)
        {
            if (active)
            {
                if (!steering.PathCompleted())
                    floatingBalloon.gameObject.SetActive(active);
            }
            else
                floatingBalloon.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// Reset the state of the pedestrian to the initial values.
    /// </summary>
    public void Reset()
    {
        animator.SetBool("leaveClass", false);
        animator.SetBool("modeAvailableSoon", false);
        animator.SetInteger("mode", -1);
        animator.SetBool("endReached", false);
        animator.SetBool("generalTrigger", true);
        knowledge.Reset();
        animator.Play("Classroom");
        this.transform.FindChild("bike").gameObject.SetActive(false);
    }
}