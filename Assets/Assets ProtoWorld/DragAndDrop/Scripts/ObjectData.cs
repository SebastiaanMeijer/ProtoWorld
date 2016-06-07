/*
 * 
 * DRAG AND DROP SYSTEM 
 * DragTransform.cs
 * Furkan Sonmez
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class ObjectData : MonoBehaviour {
	
	//these are the events that will be triggered when the specific buttons on the Object control panel are clicked
	public UnityEvent m_Slider;
	public UnityEvent m_Button1;
	public UnityEvent m_Button2;
	public UnityEvent m_Button3;
	public UnityEvent m_Button4;
	
	//these are the objects of the Object control panel that will be linked at the awake function
	public Text NameText;
	public Text ObjectDescriptionText;
	
	public GameObject OCPSlider;
	
	public Button OCPButton1;
	public Text OCPButton1Text;
	public Button OCPButton2;
	public Text OCPButton2Text;
	public Button OCPButton3;
	public Text OCPButton3Text;
	public Button OCPButton4;
	public Text OCPButton4Text;
	
	
	//these are public strings for the user to fill in in the inspector of the object. These strings will be shown in the Object control panel
	public string ObjectName;
	public string ObjectDescription;
	public string OCPButton1TextString;
	public string OCPButton2TextString;
	public string OCPButton3TextString;
	public string OCPButton4TextString;
	
	
	//this bool is to check whether the user has selected this object
	public bool selectedStatus = false;
	
	public static bool objectSelectedStatus = false;

	/// <summary>
	/// Starts the script and stores the buttons etc.
	/// </summary>
	void Start () {
		// if a objectName string is empty in the inspector, the name of the transform will be used
		if(ObjectName == "")
			ObjectName = name;
		
		//OCPSlider = GameObject.Find ("OCPSlider");
		
		//this is when the objects will be assigned
		NameText = GameObject.Find("ObjectName").GetComponent<Text>();
		ObjectDescriptionText = GameObject.Find("ObjectDescriptionText").GetComponent<Text>();
		
		//OCPButton1 = GameObject.Find("OCPButton1").GetComponent<Button>();
		OCPButton1 = OCPButtonsFunctions.OCPButton1;
		//OCPButton1 = transform.Find ("OCPButton1").GetComponent<Button>();
		//if(GameObject.Find("OCPButton1Text").gameObject != null)
		OCPButton1Text = OCPButtonsFunctions.OCPButton1Text;
		
		OCPButton2 = OCPButtonsFunctions.OCPButton2;
		OCPButton2Text = OCPButtonsFunctions.OCPButton2Text;
		
		OCPButton3 = OCPButtonsFunctions.OCPButton3;
		OCPButton3Text = OCPButtonsFunctions.OCPButton3Text;
		
		OCPButton4 = OCPButtonsFunctions.OCPButton4;
		OCPButton4Text = OCPButtonsFunctions.OCPButton4Text;
	}

	/// <summary>
	/// When an object is selected this function gets called.
	/// </summary>
	public void Selected(){

		//when the Selected function is called the selectedStatus will become true
		selectedStatus = true;
		objectSelectedStatus = true;
		
		//since the object has been selected, the paneltexts will be changed to the strings that are set in the inspector
		NameText.text = ObjectName;
		ObjectDescriptionText.text = ObjectDescription;
		OCPButton1.gameObject.SetActive(true);


		//if the button has nog been given a name the button will be set to inactive
		if(OCPButton1TextString != ""){
			OCPButton1.gameObject.SetActive(true);
			OCPButton1Text.text = OCPButton1TextString;
		}
		else{
			OCPButton1.gameObject.SetActive(false);
		}
		
		if(OCPButton2TextString != ""){
			OCPButton2.gameObject.SetActive(true);
			OCPButton2Text.text = OCPButton2TextString;
		}
		else{
			OCPButton2.gameObject.SetActive(false);
		}
		
		if(OCPButton3TextString != ""){
			OCPButton3.gameObject.SetActive(true);
			OCPButton3Text.text = OCPButton3TextString;
		}
		else{
			OCPButton3.gameObject.SetActive(false);
		}
		
		if(OCPButton4TextString != ""){
			OCPButton4.gameObject.SetActive(true);
			OCPButton4Text.text = OCPButton4TextString;
		}
		else{
			OCPButton4.gameObject.SetActive(false);
		}
		

	}

	//The update function gets called once per frame
	void Update () {
		//if one of the buttons has been clicked while this object was selected then the specific events will be invoked
		if(selectedStatus == true && SliderToText.sliderChanged == true){
			m_Slider.Invoke ();
		}
		
		if(selectedStatus == true && OCPButtonsFunctions.OCPButtonActive1 == true){
			m_Button1.Invoke ();
		}
		if(selectedStatus == true && OCPButtonsFunctions.OCPButtonActive2 == true){
			m_Button2.Invoke ();
		}
		if(selectedStatus == true && OCPButtonsFunctions.OCPButtonActive3 == true){
			m_Button3.Invoke ();
		}
		if(selectedStatus == true && OCPButtonsFunctions.OCPButtonActive4 == true){
			m_Button4.Invoke ();
		}
		
	}
	
	
	public void Deselected(){
		//when the object is deselected again the selectedStatus will be set to false again and the buttons will be set to inactive
		selectedStatus = false;
		objectSelectedStatus = false;

		OCPButton1.gameObject.SetActive (false);

		OCPButton2.gameObject.SetActive (false);

		OCPButton3.gameObject.SetActive (false);
		
		OCPButton4Text.text = "";
		OCPButton4.gameObject.SetActive (false);
		
		
	}
	
}
