/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿//Furkan Sonmez

using UnityEngine;
using System.Collections;

public class GlobalConditionsScript : MonoBehaviour {


	public static int weatherCondition = 0;
	public static float weatherConPercentage = 20;
	public static bool newConditions = false;

	
	// Update is called once per frame
	void Update () {
		//Whenever a button becomes active
		if(WeatherButtonsFunctions.WCPButtonActive1 == true && ObjectData.objectSelectedStatus == false){
			WeatherConditionFunction(1);
		}
		else if(WeatherButtonsFunctions.WCPButtonActive2 == true && ObjectData.objectSelectedStatus == false){
			WeatherConditionFunction(2);
		}
		else if(WeatherButtonsFunctions.WCPButtonActive3 == true && ObjectData.objectSelectedStatus == false){
			WeatherConditionFunction(3);
		}
	}

	/// <summary>
	/// Changes the weatherConditions depending on the button that gets clicked
	/// </summary>
	public void WeatherConditionFunction(int i){
		weatherCondition = i;
		if(weatherCondition == 1){
		Debug.Log ("Changed weather condition to sunny");
			newConditions = true;
			weatherConPercentage = 20;
		}
		else if(weatherCondition == 2){
		Debug.Log ("Changed weather condition to windy");
			newConditions = true;
			weatherConPercentage = -10;
		}
		else if(weatherCondition == 3){
		Debug.Log ("Changed weather condition to rainy");
			newConditions = true;
			weatherConPercentage = -30;
		}
	}

	public void newConditionsFunc(){
		Debug.Log ("Bikestation added");
		newConditions = true;
	}

	public IEnumerator resetNewConditions(){
		yield return new WaitForSeconds(0.3f);
		newConditions = false;
	}
	
}
