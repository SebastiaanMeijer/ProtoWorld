//Furkan Sonmez

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OCPButtonsFunctions : MonoBehaviour {
	
	//these are the static bools that will become true when a button is clicked 
	public static bool OCPButtonActive1 = false;
	public static bool OCPButtonActive2 = false;
	public static bool OCPButtonActive3 = false;
	public static bool OCPButtonActive4 = false;

	public static Slider OCPSlider;

	public static Button OCPButton1;
	public static Text OCPButton1Text;
	
	public static Button OCPButton2;
	public static Text OCPButton2Text;
	
	public static Button OCPButton3;
	public static Text OCPButton3Text;
	
	public static Button OCPButton4;
	public static Text OCPButton4Text;
	
	RectTransform rtOCP;
	RectTransform rtOCBP;
	RectTransform rtOCS;
	
	public Vector3 myPosition;


	/// <summary>
	/// Awakes the script.
	/// </summary>
	void Awake () {
		//Storing all objects etc. as public static
		rtOCP = GameObject.Find("ObjectControlPanel").GetComponent<RectTransform>();
		rtOCBP = GameObject.Find("ButtonPanel").GetComponent<RectTransform>();
		rtOCS = GameObject.Find ("SliderPanel").GetComponent<RectTransform>();

		OCPSlider = GameObject.Find ("OCPSlider").GetComponent<Slider>();
		OCPSlider.gameObject.SetActive (false);
		
		OCPButton1 = GameObject.Find("OCPButton1").GetComponent<Button>();
		OCPButton1Text = GameObject.Find("OCPButton1Text").GetComponent<Text>();
		OCPButton1.gameObject.SetActive (false);
		
		OCPButton2 = GameObject.Find("OCPButton2").GetComponent<Button>();
		OCPButton2Text = GameObject.Find("OCPButton2Text").GetComponent<Text>();
		OCPButton2.gameObject.SetActive (false);
		
		OCPButton3 = GameObject.Find("OCPButton3").GetComponent<Button>();
		OCPButton3Text = GameObject.Find("OCPButton3Text").GetComponent<Text>();
		OCPButton3.gameObject.SetActive (false);
		
		OCPButton4 = GameObject.Find("OCPButton4").GetComponent<Button>();
		OCPButton4Text = GameObject.Find("OCPButton4Text").GetComponent<Text>();
		OCPButton4.gameObject.SetActive (false);
		
	}
	
	//when one of these functions are called the specific bool will be true untill the LateUpdate
	public void OCPButton1Func(){
		OCPButtonActive1 = true;
	}
	
	public void OCPButton2Func(){
		OCPButtonActive2 = true;
	}
	
	public void OCPButton3Func(){
		OCPButtonActive3 = true;
	}
	
	public void OCPButton4Func(){
		OCPButtonActive4 = true;
	}
	
	
	//this Update is called after all other frames
	void LateUpdate () {
		//now all the buttons can be false again
		OCPButtonActive1 = false;
		OCPButtonActive2 = false;
		OCPButtonActive3 = false;
		OCPButtonActive4 = false;

		//This will position the position of the canvas automatically
		Vector3 myPosition = transform.position;
		myPosition.y = (rtOCP.position.y - (rtOCP.rect.height / 2) - (rtOCS.rect.height) - (rtOCBP.rect.height/2) +4);
		this.transform.position = myPosition;
	}
}
