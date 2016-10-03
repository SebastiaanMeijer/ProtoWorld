using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class goToFeedCamera: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{


	//these floats are needed to determine whether the player is just clicking the object or if he's gonna drag it
	public float StartMouseX;
	public float StartMouseY;

	public bool clicked = false;
	public bool dragging = false;

	public GameObject FeedCamerasObject;
	public CameraControl cameraControlScript;


	// Use this for initialization
	void Awake () {

		FeedCamerasObject = GameObject.Find ("FeedCameras");
		cameraControlScript = Camera.main.GetComponent<CameraControl>();
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		

	// this happens as soon as you click on the image and start dragging. So Immediately after this the object that has been instantiated will be dragged.
	public void OnBeginDrag(PointerEventData eventData)
	{
		dragging = true;
		Debug.LogError ("Dragging");
		//Store the first x and y position of the mouse, this is to check if the user will drag the object or just click on it
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;
	}
	public void OnClick(){
		if (dragging != true) {

			cameraControlScript.targetCameraPosition = FeedCamerasObject.transform.FindChild (this.GetComponentInChildren<Text> ().text).transform.position;
			Camera.main.transform.rotation = FeedCamerasObject.transform.FindChild (this.GetComponentInChildren<Text> ().text).transform.rotation;

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
			Debug.LogError ("CLICKED");

		}

		dragging = false;
	}


}
