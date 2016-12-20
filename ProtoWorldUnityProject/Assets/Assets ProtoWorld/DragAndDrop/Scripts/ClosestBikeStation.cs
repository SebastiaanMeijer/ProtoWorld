/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DRAG AND DROP SYSTEM 
 * Furkan Sonmez
 * 
 */


using UnityEngine;
using System.Collections;

/// <summary>
/// Makes the pedestrians find the closest bikestations and make actions based on it.
/// </summary>
public class ClosestBikeStation : MonoBehaviour {

	//the navAgent mesh
	[HideInInspector]
	public NavMeshAgent navAgent;

	//The different positions that are used
	public Vector3 closestBikeStationPos;
	public Vector3 currentBikeStationPos;
	private Vector3 nulVector = Vector3.zero;

	//Some used variables
	public bool bikeStationReached = false;
	public bool bikeAcquired = false;
	public bool recalculated = false;
	public bool retrying = false;
	public bool endReached = false;
	public int originalMode;

	//This will be a random number for determining wether to go to bikestation or not
	private float randomNumber1;

	//This is the animator
	public Animator animator;

	//This is the walkingpoint where the pedestrians walk to when in state walk
	public Transform WalkingPoint5;

	//This is the transform where the closest bikestations are stored
	public Transform closestBikeStationTransform;


	/// <summary>
	/// Awakes the script.
	/// </summary>
	void Awake()
	{
		navAgent = gameObject.GetComponent<NavMeshAgent>();
	}

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();

	}


	/// <summary>
	/// Checks all the bikestations, finds the closest one. 
	/// If the bikestation is closer than the final destination point, it will go to the bikestation by chance.
	/// </summary>
	public void bikeStationDecision(){

		//when this function is called
		//first it will store the originalmode
		originalMode = this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode;


		//then for each bikestation
		for(int i = 0; i < BikeStationScript.bikeStationAmt; i++){
			//it will check the distance of the current bikestation its checking and the closest one
			currentBikeStationPos = BikeStationScript.bikeStationPosition[i];
			float distance1 = Vector3.Distance (transform.position, currentBikeStationPos);
			float distance2 = Vector3.Distance (transform.position, closestBikeStationPos);
			//if the current one is closer it will go on
			if(distance1 < distance2 || closestBikeStationPos == nulVector){
				//the current bikestation will become the closest bikestation
				closestBikeStationPos = BikeStationScript.bikeStationPosition[i];
				closestBikeStationTransform = BikeStationScript.bikeStationTransform[i];
				//the closest bikestation will be stored in the walkingDestination6. The reason for this is to change as least as possible in the PedestrianKTHSteering method
				this.gameObject.GetComponent<PedestrianKTHSteering>().walkingDestinations[6] = BikeStationScript.bikeStationTransform[i];
			}

		}

		//now it will store the distances to the finalDestinationpoint and closestBikestationpoint
		float distance3 = Vector3.Distance (transform.position, this.gameObject.GetComponent<PedestrianKTHController>().finalDestination);
		float distance4 = Vector3.Distance (transform.position, closestBikeStationPos);

		//define a random nummer between 0 and 100
		randomNumber1 = Random.value * 100;
		
		//now if this distance is indeed closer and the randomnumber is less than the slider and weatherpercentages
		if(distance3 > distance4 && randomNumber1 < (WSliderToText.WsliderValue + GlobalConditionsScript.weatherConPercentage) && BikeStationScript.bikeStationAmt != 0){ 
			//Debug.Log (randomNumber1 + " is less than sliderValue " + WSliderToText.WsliderValue + " plus weatherConPer " + GlobalConditionsScript.weatherConPercentage);
			//set the mode to -3 in the PedestrianKTHKnowledge method and the state machine
			this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode = -3;
			animator.SetInteger("mode", -3);
			//set the destination to ToBikeStation
			this.gameObject.GetComponent<PedestrianKTHSteering>().SetNewDestination("ToBikeStation");
			

		}
		else{
			//else if distance is still closer
			if(distance3 > distance4){
				//Do nothing
				//Debug.Log (randomNumber1 + " is more than sliderValue " + WSliderToText.WsliderValue + " plus weatherConPer " + GlobalConditionsScript.weatherConPercentage);
			}
			//set bikestationreached to true(pretend it has reached the bikestation)
			bikeStationReached = true;
			animator.SetBool("bikeStationReached", true);
			//set integer mode in state machine to the original
			animator.SetInteger("mode", this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode);
			
		}


	}


	
	//FixedUpdate is called 50 times per second
	/// <summary>
	/// Checks if the pedestrian has reached the bikestation.
	/// And if it reaches it it will activate the bicycle.
	/// </summary>
	void FixedUpdate () {
		//This will check if the activateBikeStations is true or false, if it is false dont do anything with the bikestations
		if(PedestrianKTHSpawner.activateBikeStations){


			//if bikestation has been reached and had not been reached before
			if(Vector3.Distance (transform.position, closestBikeStationPos) < 3 && bikeStationReached == false){
					//check if the closestBikeStation is still there
				if(closestBikeStationTransform == null){
					bikeStationReached = true;
					return;
				}
				//if there are enough bikes left
				if(closestBikeStationTransform.gameObject.GetComponent<BikeStationScript>().capacityNumber > 0){
					//activate bike
					closestBikeStationTransform.gameObject.GetComponent<BikeStationScript>().capacityNumber = closestBikeStationTransform.gameObject.GetComponent<BikeStationScript>().capacityNumber - 1;
					//change the bikestation description etc.
					if(bikeStationAddBikes.currentBikeStation != null)
					if(bikeStationAddBikes.currentBikeStation.gameObject == closestBikeStationTransform.gameObject){

					closestBikeStationTransform.gameObject.GetComponent<ObjectData>().ObjectDescription = "Number of bikes left: " + closestBikeStationTransform.gameObject.GetComponent<BikeStationScript>().capacityNumber;
					closestBikeStationTransform.gameObject.GetComponent<ObjectData>().ObjectDescriptionText.text = closestBikeStationTransform.gameObject.GetComponent<ObjectData>().ObjectDescription;
					}
				
					//bike has been acquired
					bikeAcquired = true;
					animator.SetBool("bikeAcquired", true);
				}
				//change the mode back to originalmode
				this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode = originalMode;

				this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode = originalMode;

				//also in the animator
				animator.SetInteger("mode", originalMode);
				//set bikestationreached to true
				bikeStationReached = true;
				animator.SetBool("bikeStationReached", true);
				if(bikeAcquired == true){
				//if bike has been aqcuired activate it
				this.transform.FindChild("bike").gameObject.SetActive(true);
				this.gameObject.GetComponent<PedestrianKTHSteering>().navAgent.speed = this.gameObject.GetComponent<PedestrianKTHSteering>().speedInBike + Random.Range(-0.5f, this.gameObject.GetComponent<PedestrianKTHSteering>().speedVariation);
				}
				//set mode to originalmode
				animator.SetInteger("mode", originalMode);
				this.gameObject.GetComponent<PedestrianKTHSteering>().SetNewDestination (this.gameObject.GetComponent<PedestrianKTHKnowledge>().animationStateName);
			}
			//if(bikeStationReached == true){

			//check if bikestation has been reached and the animationstatename is still not changed. If it has not change it 
			//FIXME Does this condition statement work well in all the situations?
			if(bikeStationReached == true && this.gameObject.GetComponent<PedestrianKTHKnowledge>().animationStateName != this.gameObject.GetComponent<PedestrianKTHSteering>().goingTo && this.gameObject.GetComponent<PedestrianKTHSteering>().goingTo != "Library"){
				this.gameObject.GetComponent<PedestrianKTHSteering>().SetNewDestination (this.gameObject.GetComponent<PedestrianKTHKnowledge>().animationStateName);
			}

			//if mode is still -3 while bikestation is reached, change it to original 
			if(this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode == -3 && bikeStationReached == true){
				animator.SetInteger("mode", originalMode);
				this.gameObject.GetComponent<PedestrianKTHKnowledge>().mode = originalMode;
				animator.SetInteger("mode", originalMode);
				this.gameObject.GetComponent<PedestrianKTHSteering>().SetNewDestination (this.gameObject.GetComponent<PedestrianKTHKnowledge>().animationStateName);
			}

		}
	}


		

	//Update is called once per frame
	/// <summary>
	/// Checks if there is a new condition.
	/// For example, if the weatherconditions have changed or if there is a new bikestation.
	/// </summary>
	public void Update(){
		//This will check if the activateBikeStations is true or false, if it is false dont do anything with the bikestations
		if(PedestrianKTHSpawner.activateBikeStations){
			//check if there is a new condition unless the pedestrian already has a bike or if it has reached the end
			if(GlobalConditionsScript.newConditions == true && recalculated == false && bikeAcquired == false && endReached == false){
				//check if the closestbikestation is still there
				if(closestBikeStationTransform != null){
					closestBikeStationPos = closestBikeStationTransform.position;
				}


				//reset the status
				bikeStationReached = false;

				recalculated = true;
				animator.SetBool("callBack", true);
				animator.SetBool("bikeStationReached", false);

				resetDestination ();
				StartCoroutine (deltaReset ());
			}
		}
	}

	/// <summary>
	/// A function for resetting the closestBikestation.
	/// </summary>
	public void resetDestination(){
		//for every bikestation there is check which one is closest
		for(int i = 0; i < BikeStationScript.bikeStationAmt; i++){
			
			currentBikeStationPos = BikeStationScript.bikeStationPosition[i];
			float distance1 = Vector3.Distance (transform.position, currentBikeStationPos);
			float distance2 = Vector3.Distance (transform.position, closestBikeStationPos);
			
			if(distance1 < distance2 || closestBikeStationPos == nulVector){
				closestBikeStationPos = BikeStationScript.bikeStationPosition[i];
				closestBikeStationTransform = BikeStationScript.bikeStationTransform[i];
				this.gameObject.GetComponent<PedestrianKTHSteering>().walkingDestinations[6] = BikeStationScript.bikeStationTransform[i];
				
			}
			
		}
		//Set the destination back to ToBikeStation
		this.gameObject.GetComponent<PedestrianKTHSteering>().SetNewDestination("ToBikeStation");

	}

	/// <summary>
	/// A function for resetting newConditions etc. back to false
	/// </summary>
	public IEnumerator deltaReset(){
		yield return new WaitForSeconds(0.3f);
		animator.SetBool("callBack", false);
		yield return new WaitForSeconds(0.3f);
		GlobalConditionsScript.newConditions = false;
		recalculated = false;
	}

	/// <summary>
	/// A function for resetting the closestBikestation.
	/// </summary>
	public IEnumerator newDestinationRetry(){
		Debug.LogError("HELLO");
	yield return new WaitForSeconds(0.3f);
	this.gameObject.GetComponent<PedestrianKTHSteering>().SetNewDestination (this.gameObject.GetComponent<PedestrianKTHKnowledge>().animationStateName);
	yield return new WaitForSeconds(1.3f);
	if(this.gameObject.GetComponent<PedestrianKTHKnowledge>().animationStateName != this.gameObject.GetComponent<PedestrianKTHSteering>().goingTo && bikeStationReached == true)
		StartCoroutine (newDestinationRetry ());
	else
		retrying = false;
	}


}
