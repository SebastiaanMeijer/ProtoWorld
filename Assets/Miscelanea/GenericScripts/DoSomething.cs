using UnityEngine;
using System.Collections;

public class DoSomething : MonoBehaviour {

	// Use this for initialization
	public void  myFirstEvent() {
		Debug.Log("THIS IS MY FIRST EVENT!!");
	}

	public void  mySelected() {
		Debug.Log("Selected");
	}

	public void  myDeselected() {
		Debug.Log("Deselected");
	}

	public void  myMoved() {
		Debug.Log("Moved");
	}

	public void  myDropped() {
		Debug.Log("Dropped");
	}
	public void  myRotated() {
		Debug.Log("Rotated");
	}


	// Update is called once per frame
	void Update () {
	
	}
}
