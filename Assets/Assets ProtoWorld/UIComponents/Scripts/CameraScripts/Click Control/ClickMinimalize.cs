//Furkan Sonmez

using UnityEngine;
using System.Collections;

public class ClickMinimalize : MonoBehaviour {

	//these are the transforms that will be activated and deactivated when clicking on the title area
	public Transform DragAndDropList;
	public Transform ScrollBarDragAndDropList;

	//these are the static values of the RectTransform
	static public float sizeWidth;
	static public float sizeHeight;
	static public float xPosition;
	static public float yPosition;

	void Start () {
		//here the values are stored into the static floats
		RectTransform rt = GetComponent<RectTransform>();
		sizeWidth = rt.rect.width;
		sizeHeight = rt.rect.height;
		xPosition = rt.rect.x;
		yPosition = rt.rect.y;

	}

	public void ActivateDragAndDrop()
	{
		//when this function is called and the gameObject was true, now make it false and vice versa
		if(DragAndDropList.gameObject.activeSelf == true){
			DragAndDropList.gameObject.SetActive (false);
			ScrollBarDragAndDropList.gameObject.SetActive (false);

		}
		else if(DragAndDropList.gameObject.activeSelf == false){
			DragAndDropList.gameObject.SetActive (true);
			ScrollBarDragAndDropList.gameObject.SetActive (true);
		}
	}
	
}
