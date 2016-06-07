using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log ("OnPointerEnter");
		}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log ("OnPointerExit");
	}

	public void OnDrop(PointerEventData eventData) {
		Debug.Log (eventData.pointerDrag.name + " was dropped onto " + gameObject.name);

		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		if (d != null) {
			d.draggedOut = true;
			d.parentToReturnTo = this.transform;
				}
	}
}
