using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class PedestrianBaseClass : MonoBehaviour {

	public static int globalIdIndex = 0;
	public string id;
	private NavMeshAgent NavMeshAgent;
	// Use this for initialization
	void Start () {
		NavMeshAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * Draft Pedestrian
	 * 1- Choose a destination from the GlobalSimulationPlannerBaseClass.
	 * 2- Choose an origin from the GlobalSimulationPlannerBaseClass.
	 * 3- Initialize valid position close to origin for the current pedestrian.
	 * 4- Retrieve trip of size N from SL Service for (currentPosition, Destination).
	 * 5- Initialize mode of transport (MOT) either randomly or consider the trip[N] scenario to make a decision.
	 * 6- IF (MOT=="Walking")
	 *				THEN ActivateWalkToDestination(); // The pedestrian will automatically stop at the destination.
	 *		ELSE {
	 *					LET idx = 0; // Index of the trip[N]
	 *					WHILE ( ! IsAtDestination() )
	 *								{
	 *									IF (trip[idx].mode == "Walking") // Meaning that the current part of the trip is to walk (probably to the Bus Stop)
	 *											THEN WalkToTripPoint(Trip[idx].Dest);
	 *									ELSE IF (trip[idx].mode == "Bus")
	 *														THEN {
	 *																	WHILE (! trip[idx].vehicle.IsAvailable){
	 *																		Wait(); }
	 *																	GetOnTheBus( trip[idx].vehicle );
	 *																	WHILE ( trip[idx].vehicle.IsMoving && ! trip[idx].vehicle.IsAtDest( trip[idx].Dest ) ){
	 *																		Wait(); }
	 *																	GetOffTheBus();
	 *																	}
	 *									idx++; // Progress to the next part of the trip
	 *									}
	 *						}
	 *					
	 */
}
