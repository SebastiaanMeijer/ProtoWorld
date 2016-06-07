//Furkan Sonmez

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
