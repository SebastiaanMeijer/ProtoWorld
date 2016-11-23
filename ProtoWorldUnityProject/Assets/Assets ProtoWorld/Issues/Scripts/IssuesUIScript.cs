/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Issues Module
 * 
 * Furkan Sonmez
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class IssuesUIScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private float myXPos;
	private float myYPos;

	private float myWidth;
	private float myHeight;

	public bool showing;

	public static bool issueButtonClicked = false;
	public static bool solveAnIssue = false;
	public static bool goToIssue = false;
	
	void Awake() {
		myYPos = GetComponent<RectTransform>().transform.position.y;
		myXPos = GetComponent<RectTransform>().transform.position.x;
		myWidth = GetComponent<RectTransform>().rect.width;
		myHeight = GetComponent<RectTransform>().rect.height;
	}
	
	void LateUpdate() {
		if(showing) {
			if(rayHitPositionClass.dragging) {
				if(Input.mousePosition.y > GetComponent<RectTransform>().transform.position.y + 0.5 * myHeight ||
					Input.mousePosition.y < GetComponent<RectTransform>().transform.position.y - 0.5 * myHeight ||
					Input.mousePosition.x > GetComponent<RectTransform>().transform.position.x + 0.5 * myWidth ||
					Input.mousePosition.x < GetComponent<RectTransform>().transform.position.x - 0.5 * myWidth) {

					this.transform.localPosition = new Vector3(this.transform.localPosition.x, myYPos + 1000, this.transform.localPosition.z);
				}
			}
		}
	}


	public void SolveIssue() {
		issueButtonClicked = true;
		solveAnIssue = true;

	}

	public void GoToIssue() {
		issueButtonClicked = true;
		goToIssue = true;
	}


	// This happens as soon as you click on the image and start dragging. So Immediately after this the object that has been instantiated will be dragged.
	public void OnBeginDrag(PointerEventData eventData) {
		//Debug.LogError("Dragging");

		//Store the first x and y position of the mouse, this is to check if the user will drag the object or just click on it
	}
	public void OnClick(PointerEventData eventData) {
		//Debug.LogError("CLICKING");
	}

	public void OnDrag(PointerEventData eventData) {
		//Debug.LogError("Dragging during");
	}

	public void OnEndDrag(PointerEventData eventData) {
		//Debug.LogError("DRAGGED");
	}
}
