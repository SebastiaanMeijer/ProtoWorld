/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DRAG AND DROP SYSTEM 
 * Furkan Sonmez
 * 
 */

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
