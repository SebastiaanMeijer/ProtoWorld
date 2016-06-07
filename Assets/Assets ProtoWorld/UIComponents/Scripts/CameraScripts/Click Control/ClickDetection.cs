using UnityEngine;
using System.Collections;

public class ClickDetection : MonoBehaviour {
	

	//as soon as the object RotateToCamera gets clicked the function dontDeselect in the parent object gets called 
	void OnMouseDown()
	{
		this.GetComponentInParent<DragTransform>().dontDeselect();
		
	}
	
	//as soon as the user stops clicking the object RotateToCamera the fuction goDeselect in the parent object gets called 
	void OnMouseUp()
	{
		this.GetComponentInParent<DragTransform>().goDeselect();

	}

	
}
