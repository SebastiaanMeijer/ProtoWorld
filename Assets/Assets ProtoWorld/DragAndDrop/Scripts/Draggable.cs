using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public GameObject theOriginalParent;
	public GameObject objectToInstantiate;
	
	public Transform parentToReturnTo = null;

	public bool draggedOut = false;


	public void Start(){
		this.transform.parent = GameObject.Find ("Panel2").transform;
		}



	public void OnBeginDrag(PointerEventData eventData){
		if(draggedOut == false)
		Instantiate (objectToInstantiate);

		Debug.Log ("OnBeginDrag");

		parentToReturnTo = this.transform.parent;
		this.transform.SetParent (this.transform.parent.parent);

		GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("OnDrag");
		this.transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("OnEndDrag");

		if (parentToReturnTo != null) {
						this.transform.SetParent (parentToReturnTo);
		} else {
						Destroy (gameObject.transform);
		}
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}



}
