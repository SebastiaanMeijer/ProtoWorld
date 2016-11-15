using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class IssueScript : MonoBehaviour{


	//these floats are needed to determine whether the player is just clicking the object or if he's gonna drag it
	public float StartMouseX;
	public float StartMouseY;

	public bool clicked = false;
	public bool dragging = false;
	public float originalY;
	public int issuesId;
	public static int issuesIdStatic;


	public Text issueTypeText;
	public Text detailsText;
	public RectTransform issuesTransform;

	public string issueTypeString;
	public string detailsString;

	// Use this for initialization
	void Awake () {
		issueTypeText = GameObject.Find("IssueType").GetComponent<Text>();
		detailsText = GameObject.Find("DetailsText").GetComponent<Text>();
		issuesTransform = GameObject.Find ("IssuesUI").GetComponent<RectTransform> ();
	}


	// Use this for initialization
	void Start () {
		originalY = issuesTransform.localPosition.y;
		issuesTransform.localPosition = new Vector3 (issuesTransform.localPosition.x, originalY + 1000, issuesTransform.localPosition.z);
	}

	// Update is called once per frame
	void Update () {
		if (IssuesUIScript.issueButtonClicked == true) {
			if (IssuesUIScript.solveAnIssue == true) {
				if (issuesId == issuesIdStatic) {
					IssuesUIScript.issueButtonClicked = false;
					IssuesUIScript.solveAnIssue = false;
					Destroy (this.gameObject.GetComponentInParent<Transform> ().gameObject);
				}
			}
			if (IssuesUIScript.goToIssue == true) {
				if (issuesId == issuesIdStatic) {
					IssuesUIScript.issueButtonClicked = false;
					IssuesUIScript.goToIssue = false;
					Camera.main.gameObject.GetComponent<CameraControl> ().targetCameraPosition = this.transform.position + new Vector3 (Camera.main.gameObject.GetComponent<CameraControl> ().targetCameraPosition.x - this.transform.position.x, 0, Camera.main.gameObject.GetComponent<CameraControl> ().targetCameraPosition.z - this.transform.position.z).normalized * 30;
					Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y + 180, Camera.main.transform.rotation.eulerAngles.z);

				}
			}
		}
	}

	void OnMouseDown()
	{
		clicked = true;
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;
	}
	void OnMouseUp()
	{
		if (clicked == true)
		if (StartMouseX == Input.mousePosition.x && StartMouseY == Input.mousePosition.y) {
			
			issueTypeText.text = issueTypeString;
			detailsText.text = detailsString;
			issuesTransform.localPosition = new Vector3 (issuesTransform.localPosition.x, originalY, issuesTransform.localPosition.z);
			issuesTransform.gameObject.GetComponent<IssuesUIScript> ().showing = true;
			issuesIdStatic = issuesId;
		}
		clicked = false;
	}
		


}
