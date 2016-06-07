//Furkan Sonmez

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectData))]
public class bikeStationAddBikes : MonoBehaviour 
{
	public static GameObject currentBikeStation;
	public GameObject currentBikeStation2;

	/// <summary>
	/// Makes the displayCanvas display this bikeStation and its information
	/// </summary>
	public void bikesDisplay(){
		this.gameObject.GetComponent<ObjectData>().ObjectDescription = "Number of bikes left: " + this.gameObject.GetComponent<BikeStationScript>().capacityNumber;
		currentBikeStation = this.gameObject;
		currentBikeStation2 = currentBikeStation;
	}

	/// <summary>
	/// Whenever an addition button is pressed this function is called
	/// </summary>
	public void addBikes(int i)
    {
		this.gameObject.GetComponent<BikeStationScript>().capacityNumber = this.gameObject.GetComponent<BikeStationScript>().capacityNumber + i;
		this.gameObject.GetComponent<ObjectData>().ObjectDescription = "Number of bikes left: " + this.gameObject.GetComponent<BikeStationScript>().capacityNumber;
		this.gameObject.GetComponent<ObjectData>().ObjectDescriptionText.text = this.gameObject.GetComponent<ObjectData>().ObjectDescription;
	}

	/// <summary>
	/// Whenever an substraction button is pressed this function is called
	/// </summary>
	public void removeBikes(int i){

		this.gameObject.GetComponent<BikeStationScript>().capacityNumber = this.gameObject.GetComponent<BikeStationScript>().capacityNumber - i;

		if(this.gameObject.GetComponent<BikeStationScript>().capacityNumber <0)
			this.gameObject.GetComponent<BikeStationScript>().capacityNumber = 0;

		this.gameObject.GetComponent<ObjectData>().ObjectDescription = "Number of bikes left: " + this.gameObject.GetComponent<BikeStationScript>().capacityNumber;
		this.gameObject.GetComponent<ObjectData>().ObjectDescriptionText.text = this.gameObject.GetComponent<ObjectData>().ObjectDescription;
	}
}
