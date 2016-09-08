﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderChangeValue : MonoBehaviour {

	public Slider slider;
	public Text text;
	public string unit;
	public byte decimals = 0;
	public int idNumber;

	public static float sliderValue;

	public static bool sliderChanged;

	public void Awake(){

		//slider = this.transform.GetComponent<Slider>();
		//text = this.GetComponentInChildren<Text>();
	}


	void OnEnable () 
	{
		if (idNumber == 1) {
			slider.onValueChanged.AddListener (ChangeValueIntensityHM);
			ChangeValueIntensityHM (slider.value);
		} else if (idNumber == 2) {
			slider.onValueChanged.AddListener (ChangeValueRadiusHM);
			ChangeValueRadiusHM (slider.value);
		}

	}
	void OnDisable()
	{
		slider.onValueChanged.RemoveAllListeners();
	}

	void ChangeValueIntensityHM(float value)
	{
		Heatmap.changeParameterIntensityHM (value);
		//text.text = value.ToString("n" + decimals) + " " + unit;
		//sliderValue = slider.value;
		//Debug.LogError (sliderValue);
		sliderChanged = true;
	}

	void ChangeValueRadiusHM(float value)
	{
		Heatmap.changeParameterRadiusHM (value);
		//text.text = value.ToString("n" + decimals) + " " + unit;
		//sliderValue = slider.value;
		//Debug.LogError (sliderValue);
		sliderChanged = true;
	}

	void LateUpdate () {

		//now  the bool can be false again
		sliderChanged = false;

		//BikeStationScript.newBikeStation = false;
	}


}