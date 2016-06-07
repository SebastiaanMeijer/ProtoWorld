//Furkan Sonmez

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickAndSpawn : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	//this is the gameobject that will be instantiated, assign this object in the inspector of the ClickAndDrop object
	public GameObject objectToInstantiate;

	// this happens as soon as you click on the image and start dragging. So Immediately after this the object that has been instantiated will be dragged.
	public void OnBeginDrag(PointerEventData eventData){
		//instantiate the gameobject to the hitlocation
		Instantiate (objectToInstantiate, rayHitPositionClass.hitLocation, Quaternion.identity);

	}
	
	public void OnDrag(PointerEventData eventData){

	}
	
	public void OnEndDrag(PointerEventData eventData){

	}
	
}