/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿	using UnityEngine;
	using System.Collections;
	using UnityEngine.EventSystems;
	using UnityEngine.Events;
	
	public class RotateObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	//this is the event that is called when the user rotates the object
	public UnityEvent m_Rotated;

	// this is the object that will be rotated, this will ofcourse be the parent object, the object can be assigned from the inspector
	public Transform objectToRotate;

	//this is the first position of the mouse that is stored as soon as the user starts dragging on the arrow image
	public float firstMouseXPosition;

	//this is the rotation that the object is originally in
	public float originalRotationY;
	//this float holds the rotation of the object in the Y
	public float rotationY;
	//this float is to change the smoothness at which the rotation happens
	public float smooth = 10.0F;
	//this is the amount at which the object rotates
	public float rotationSpeed = 60.0F;

		
		


		// this happens as soon as you click on the image and start dragging. So Immediately after this the object that has been instantiated will be dragged.
		public void OnBeginDrag(PointerEventData eventData){
		//at the start of the rotation the rotation at that instant will be stored
		originalRotationY= objectToRotate.transform.rotation.y;
		//at the start of the drag, the mouseposition will be stored
		firstMouseXPosition = Input.mousePosition.x;


		}
		
		public void OnDrag(PointerEventData eventData){
			//while dragging, the new rotation is continuously being stored
			rotationY = originalRotationY + (firstMouseXPosition - Input.mousePosition.x);
		}
		
		public void OnEndDrag(PointerEventData eventData){
			// at the end of the rotation, the rotated event will be invoked, ofcourse this could also be put at the begin of the drag. This is optional
			m_Rotated.Invoke ();
		}


		void Awake(){
			//the objectToRotate gets assigned at the awake function. The parent of the parent of its parent will be assigned as the Transform. 
			//Ofcourse the RotateToCamera should nog be the child of another object than the object itself
			objectToRotate = GetComponentInParent<Transform>().parent.parent.parent;
		
		}


		public void Update(){
			// the objectRotation will continiously be changed to the new rotation
			Quaternion target = Quaternion.Euler(0, rotationY, 0);
			objectToRotate.transform.rotation = Quaternion.Slerp(objectToRotate.transform.rotation, target,  smooth);
			
		}
		
	}