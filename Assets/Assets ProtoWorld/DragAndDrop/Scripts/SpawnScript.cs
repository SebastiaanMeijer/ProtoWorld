using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SpawnScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public GameObject objectToInstantiate;

	public void Start(){
		Instantiate (objectToInstantiate);
		//this.transform.parent = GameObject.Find ("Panel").transform;
	}
	
	
	
	public void OnBeginDrag(PointerEventData eventData){

		Instantiate (objectToInstantiate);
		
		Debug.Log ("OnBeginDrag");

		
		//GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}
	
	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("OnDrag");
		//this.transform.position = eventData.position;
	}
	
	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("OnEndDrag");

		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

}
