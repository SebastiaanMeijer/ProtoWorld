/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Camera Feed Module
 * 
 * Furkan Sonmez
 * Berend Wouda
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoToFeedCamera : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	// These floats are needed to determine whether the player is just clicking the object or if he's gonna drag it.
	private float StartMouseX;
	private float StartMouseY;

	private bool clicked = false;
	private bool clickedCameraIcon = false;
	private bool dragging = false;

	private GameObject feedCamerasObject;
	private CameraControl cameraControlScript;

	private bool proximitySensor;
	private bool iAmASign = false;

	
	void Awake() {
		feedCamerasObject = GameObject.Find("FeedCameras");
		cameraControlScript = Camera.main.GetComponent<CameraControl>();
		if (this.transform.name == "CameraSign") {
			iAmASign = true;
		}
	}

	
	void LateUpdate() {
		if(clickedCameraIcon) {
			Camera.main.GetComponent<CameraControl>().targetCameraPosition = this.transform.parent.position;
			Camera.main.transform.rotation = this.transform.parent.rotation;
			clickedCameraIcon = false;
		}
		if(iAmASign)
		if (Vector3.Distance (this.transform.position, cameraControlScript.transform.position) < 80 && proximitySensor == true) {
			proximitySensor = false;
			this.GetComponent<SpriteRenderer> ().enabled = false;
		} else if(Vector3.Distance(this.transform.position, cameraControlScript.transform.position) > 80 && proximitySensor == false) {
			proximitySensor = true;
			this.GetComponent<SpriteRenderer> ().enabled = true;
		}
	}


	// This happens as soon as you click on the image and start dragging. So immediately after this the object that has been instantiated will be dragged.
	public void OnBeginDrag(PointerEventData eventData) {
		dragging = true;
		// Store the first x and y position of the mouse, this is to check if the user will drag the object or just click on it.
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;
	}

	public void OnClick() {
		if(dragging != true) {
			GameObject feedCamera = getFeedCameraForName(this.GetComponentInChildren<Text>().text);

			if(feedCamera != null) {
				cameraControlScript.targetCameraPosition = feedCamera.transform.position;
				Camera.main.transform.rotation = feedCamera.transform.rotation;
			}
		}
	}

	public void OnDrag(PointerEventData eventData) {

	}

	public void OnEndDrag(PointerEventData eventData) {
		if(clicked == true)
			if(StartMouseX == Input.mousePosition.x || StartMouseY == Input.mousePosition.y) {
				clicked = false;

			}

		dragging = false;
	}

	void OnMouseDown() {
		clickedCameraIcon = true;
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;
	}

	void OnMouseUp() {
		clickedCameraIcon = true;
		if(clickedCameraIcon == true)
			if(StartMouseX == Input.mousePosition.x && StartMouseY == Input.mousePosition.y) {
				//cameraControlScript.targetCameraPosition = GetComponentInParent<Transform>().transform.position;
				//Camera.main.transform.rotation = GetComponentInParent <Transform>().transform.rotation;

				//Camera.main.GetComponent<CameraControl>().targetCameraPosition = feedCamerasObject.transform.FindChild (this.GetComponentInParent<Camera> ().gameObject.name).transform.position;
				//cameraControlScript.targetCameraPosition = new Vector3(1000,200,1000);

				//Camera.main.transform.rotation = feedCamerasObject.transform.FindChild (this.GetComponentInParent<Camera> ().gameObject.name).transform.rotation;
			}
	}


	private GameObject getFeedCameraForName(string name) {
		for(int index = 0; index < feedCamerasObject.transform.childCount; index++) {
			GameObject child = feedCamerasObject.transform.GetChild(index).gameObject;
			if(child.GetComponent<FeedCamera>().name == name) {
				return child;
			}
		}

		return null;
	}
}
