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
