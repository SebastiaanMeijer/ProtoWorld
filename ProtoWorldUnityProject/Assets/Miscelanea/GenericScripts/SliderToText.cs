/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderToText : MonoBehaviour {
	
	public Slider slider;
	public Text text;
	public string unit;
	public byte decimals = 0;

	public static float sliderValue;
	
	public static bool sliderChanged;

	public void Awake(){

		//slider = this.transform.GetComponent<Slider>();
		//text = this.GetComponentInChildren<Text>();
	}

	
	void OnEnable () 
	{
		slider.onValueChanged.AddListener(ChangeValue);
		ChangeValue(slider.value);
	}
	void OnDisable()
	{
		slider.onValueChanged.RemoveAllListeners();
	}
	
	void ChangeValue(float value)
	{
		text.text = value.ToString("n" + decimals) + " " + unit;
		sliderValue = slider.value;
		//Debug.LogError (sliderValue);
		sliderChanged = true;
	}

	void LateUpdate () {

		//now  the bool can be false again
		sliderChanged = false;

		//BikeStationScript.newBikeStation = false;
	}
	
	
}