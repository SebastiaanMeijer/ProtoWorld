using UnityEngine;
using System.Collections;

public class ActivateRotating : MonoBehaviour {

	public Transform rotationCanvas;
	public Transform TheObject;

	// Use this for initialization
	void Start () {
	
	}

	void OnMouseDown()
	{
		//if(TheObject.DragTransform.clicked == true)
		//if(rotationCanvas.gameObject.activeSelf == true){
		//	Debug.LogError ("I should now deactivate the object");
		//	rotationCanvas.gameObject.SetActive (false);
		//}
		//else if(rotationCanvas.gameObject.activeSelf == false){
		//	Debug.LogError ("I should now activate the object");
		//	rotationCanvas.gameObject.SetActive (true);
		//}
	}

	void OnMouseUp()
	{

	}

	// Update is called once per frame
	void Update () {
	
	}
}
