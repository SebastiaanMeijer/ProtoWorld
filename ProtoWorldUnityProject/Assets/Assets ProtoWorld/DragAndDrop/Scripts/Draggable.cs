/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
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
