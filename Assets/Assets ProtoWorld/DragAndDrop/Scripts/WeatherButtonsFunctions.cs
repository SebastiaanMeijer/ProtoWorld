//Furkan Sonmez

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeatherButtonsFunctions : MonoBehaviour {

	public static bool WCPButtonActive1 = false;
	public static bool WCPButtonActive2 = false;
	public static bool WCPButtonActive3 = false;
	public static bool WCPButtonActive4 = false;

	
	public static Button WCPButton1;
	public static Text WCPButton1Text;
	
	public static Button WCPButton2;
	public static Text WCPButton2Text;
	
	public static Button WCPButton3;
	public static Text WCPButton3Text;
	
	public static Button WCPButton4;
	public static Text WCPButton4Text;
	
	RectTransform rtWCP;
	RectTransform rtWCBP;
	RectTransform rtWCS;
	
	public Vector3 myWPosition;

	/// <summary>
	/// Awakes the script.
	/// </summary>
	void Awake () {
		
		rtWCP = GameObject.Find("WeatherControlPanel").GetComponent<RectTransform>();
		rtWCBP = GameObject.Find("WeatherButtonPanel").GetComponent<RectTransform>();
		rtWCS = GameObject.Find ("ChanceSliderPanel").GetComponent<RectTransform>();
		
		WCPButton1 = GameObject.Find("WCPButton1").GetComponent<Button>();
		WCPButton1Text = GameObject.Find("WCPButton1Text").GetComponent<Text>();
		//OCPButton1.gameObject.SetActive (false);
		
		WCPButton2 = GameObject.Find("WCPButton2").GetComponent<Button>();
		WCPButton2Text = GameObject.Find("WCPButton2Text").GetComponent<Text>();
		//OCPButton2.gameObject.SetActive (false);
		
		WCPButton3 = GameObject.Find("WCPButton3").GetComponent<Button>();
		WCPButton3Text = GameObject.Find("WCPButton3Text").GetComponent<Text>();
		//OCPButton3.gameObject.SetActive (false);
		
		WCPButton4 = GameObject.Find("WCPButton4").GetComponent<Button>();
		WCPButton4Text = GameObject.Find("WCPButton4Text").GetComponent<Text>();
		WCPButton4.gameObject.SetActive (false);
		
		WCPButton1Text.text = "Sunny";
		
		
		WCPButton2Text.text = "Windy";
		
		
		WCPButton3Text.text = "Rainy";
		
		
	}

	//when one of these functions are called the specific bool will be true untill the LateUpdate
	public void WCPButton1Func(){
		WCPButtonActive1 = true;
	}
	
	public void WCPButton2Func(){
		WCPButtonActive2 = true;
	}
	
	public void WCPButton3Func(){
		WCPButtonActive3 = true;
	}
	
	public void WCPButton4Func(){
		WCPButtonActive4 = true;
	}


	void LateUpdate () {
		//now all the buttons can be false again
		WCPButtonActive1 = false;
		WCPButtonActive2 = false;
		WCPButtonActive3 = false;
		WCPButtonActive4 = false;
	}

}
