using UnityEngine;
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