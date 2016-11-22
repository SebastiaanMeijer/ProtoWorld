using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class GoToFeedCamera: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
	//these floats are needed to determine whether the player is just clicking the object or if he's gonna drag it
	public float StartMouseX;
	public float StartMouseY;

	public bool clicked = false;
	public bool clickedCameraIcon = false;
	public bool dragging = false;

	public GameObject feedCamerasObject;
	public CameraControl cameraControlScript;


	// Use this for initialization
	void Awake () {
		feedCamerasObject = GameObject.Find ("FeedCameras");
		cameraControlScript = Camera.main.GetComponent<CameraControl>();
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (clickedCameraIcon) {
			Camera.main.GetComponent<CameraControl> ().targetCameraPosition = this.transform.parent.position;
			Camera.main.transform.rotation = this.transform.parent.rotation;
			clickedCameraIcon = false;
		}
	}
		

	// this happens as soon as you click on the image and start dragging. So Immediately after this the object that has been instantiated will be dragged.
	public void OnBeginDrag(PointerEventData eventData)
	{
		dragging = true;
		//Store the first x and y position of the mouse, this is to check if the user will drag the object or just click on it
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;
	}

	public void OnClick(){
		if (dragging != true) {
			GameObject feedCamera = getFeedCameraForName(this.GetComponentInChildren<Text>().text);

			if(feedCamera != null) {
				cameraControlScript.targetCameraPosition = feedCamera.transform.position;
				Camera.main.transform.rotation = feedCamera.transform.rotation;
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{

	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (clicked == true)
		if (StartMouseX == Input.mousePosition.x || StartMouseY == Input.mousePosition.y) {
			clicked = false;

		}

		dragging = false;
	}

	void OnMouseDown()
	{
		clickedCameraIcon = true;
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;
	}

	void OnMouseUp()
	{
		clickedCameraIcon = true;
		if (clickedCameraIcon == true)
		if (StartMouseX == Input.mousePosition.x && StartMouseY == Input.mousePosition.y) {
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
