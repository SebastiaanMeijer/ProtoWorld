using UnityEngine;
using System.Collections;

public class OpenCloseObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenEvent(){


		if(OCPButtonsFunctions.OCPButton1Text.text == "Open"){

		Debug.Log ("The object is now open");
		this.GetComponent<ObjectData>().OCPButton1TextString = "Close";
			OCPButtonsFunctions.OCPButton1Text.text = "Close";

		}
		else if(OCPButtonsFunctions.OCPButton1Text.text == "Close"){

		Debug.Log ("The object is now open");
		this.GetComponent<ObjectData>().OCPButton1TextString = "Open";
			OCPButtonsFunctions.OCPButton1Text.text = "Open";

		}


	}

	public void JumpEvent(){
	


	}




}
