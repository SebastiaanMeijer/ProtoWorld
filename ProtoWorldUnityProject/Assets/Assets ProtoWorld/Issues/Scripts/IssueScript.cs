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
using UnityEngine.UI;

public class IssueScript : MonoBehaviour {
	// These floats are needed to determine whether the player is just clicking the object or if he's gonna drag it.
	private float StartMouseX;
	private float StartMouseY;

	private bool clicked = false;
	private bool dragging = false;
	public static float originalY;
	public int issuesId;
	private static int issuesIdStatic;
	
	public Text issueTypeText;
	public Text detailsText;
	public RectTransform issuesTransform;

	private string issueTypeString = "";
	private string detailsString = "";
	

	void Awake() {
		issueTypeText = GameObject.Find("IssueType").GetComponent<Text>();
		detailsText = GameObject.Find("DetailsText").GetComponent<Text>();
		issuesTransform = GameObject.Find("IssuesUI").GetComponent<RectTransform>();
	}
	
	void Start() {
		issuesId = issuesId + 1;
		//originalY = issuesTransform.localPosition.y - 1000;
		//issuesTransform.localPosition = new Vector3(issuesTransform.localPosition.x, originalY + 1000, issuesTransform.localPosition.z);
	}
	
	void LateUpdate() {
		if(IssuesUIScript.issueButtonClicked == true) {
			if(IssuesUIScript.solveAnIssue == true) {
				if(issuesId == issuesIdStatic) {
					issuesIdStatic = -1;
					IssuesUIScript.issueButtonClicked = false;
					IssuesUIScript.solveAnIssue = false;
					issuesTransform.localPosition = new Vector3(issuesTransform.localPosition.x, originalY + 1000, issuesTransform.localPosition.z);
					Destroy(this.gameObject.GetComponentInParent<Transform>().gameObject);
				}
			}
			if(IssuesUIScript.goToIssue == true) {
				if(issuesId == issuesIdStatic) {
					IssuesUIScript.issueButtonClicked = false;
					IssuesUIScript.goToIssue = false;
					issuesTransform.localPosition = new Vector3(issuesTransform.localPosition.x, originalY + 1000, issuesTransform.localPosition.z);
					Camera.main.gameObject.GetComponent<CameraControl>().targetCameraPosition = this.transform.position + new Vector3(Camera.main.gameObject.GetComponent<CameraControl>().targetCameraPosition.x - this.transform.position.x, 0, Camera.main.gameObject.GetComponent<CameraControl>().targetCameraPosition.z - this.transform.position.z).normalized * 30;
					Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y + 180, Camera.main.transform.rotation.eulerAngles.z);

				}
			}
		}
	}

	void OnMouseDown() {
		clicked = true;
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;

	}

	void OnMouseUp() {
		if(clicked == true) {
			if(StartMouseX == Input.mousePosition.x && StartMouseY == Input.mousePosition.y) {
				issueTypeText.text = issueTypeString;
				detailsText.text = detailsString;
				issuesTransform.localPosition = new Vector3(issuesTransform.localPosition.x, originalY, issuesTransform.localPosition.z);
				issuesTransform.gameObject.GetComponent<IssuesUIScript>().showing = true;
				issuesId = Random.Range (0, 10000);
				issuesIdStatic = issuesId;
			}
		}
		clicked = false;
	}
}
