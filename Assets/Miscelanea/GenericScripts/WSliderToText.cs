//Furkan Sonmez

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WSliderToText : MonoBehaviour {
		
		public Slider Wslider;
		public Text text;
		public string unit;
		public byte decimals = 0;
		
		public static float WsliderValue;
		
		public static bool WsliderChanged;

		//Whenever the slider is moved
		void OnEnable () 
		{
			Wslider.onValueChanged.AddListener(ChangeValue);
			ChangeValue(Wslider.value);
		}
		void OnDisable()
		{
			Wslider.onValueChanged.RemoveAllListeners();
		}
		
		//Whenever a value is changed
		void ChangeValue(float value)
		{
			text.text = value.ToString("n" + decimals) + " " + unit;
			WsliderValue = Wslider.value;
			WsliderChanged = true;
		}
		
		void LateUpdate () {
			
			//now  the bool can be false again
			WsliderChanged = false;
		}
		
		
	}