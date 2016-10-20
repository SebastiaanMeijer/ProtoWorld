using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class IssuesUIScript: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{

	public float myXPos;
	public float myYPos;

	public float myWidth;
	public float myHeight;

	public bool showing;

	// Use this for initialization
	void Awake () {
		
		myYPos = GetComponent<RectTransform> ().transform.position.y;
		myXPos = GetComponent<RectTransform> ().transform.position.x;
		myWidth = GetComponent<RectTransform> ().rect.width;
		myHeight = GetComponent<RectTransform> ().rect.height;

	}


	
	// Update is called once per frame
	void LateUpdate () {
		if(showing)
		if(rayHitPositionClass.dragging)
		if(Input.mousePosition.y > GetComponent<RectTransform> ().transform.position.y + 0.5*myHeight || 
			Input.mousePosition.y < GetComponent<RectTransform> ().transform.position.y - 0.5*myHeight || 
			Input.mousePosition.x > GetComponent<RectTransform> ().transform.position.x + 0.5*myWidth ||
			Input.mousePosition.x < GetComponent<RectTransform> ().transform.position.x - 0.5*myWidth){

			this.transform.localPosition = new Vector3 (this.transform.localPosition.x, myYPos + 1000, this.transform.localPosition.z);
		}
	}

	void OnMouseDown(){

		Debug.LogError ("HELLO");

	}

	// this happens as soon as you click on the image and start dragging. So Immediately after this the object that has been instantiated will be dragged.
	public void OnBeginDrag(PointerEventData eventData)
	{

		Debug.LogError ("Dragging");
		//Store the first x and y position of the mouse, this is to check if the user will drag the object or just click on it

	}
	public void OnClick(PointerEventData eventData){
		Debug.LogError ("CLICKING");
	}

	public void OnDrag(PointerEventData eventData)
	{
		Debug.LogError ("Dragging during");
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.LogError ("DRAGGED");
	}

}
