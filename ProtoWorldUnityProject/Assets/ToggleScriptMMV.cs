using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ToggleScriptMMV : MonoBehaviour {

	public UnityEvent toggleClick;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void toggleClicked(){
		toggleClick.Invoke ();
	}

}
