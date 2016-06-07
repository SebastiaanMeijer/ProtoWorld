using UnityEngine;
using System.Collections;

public class LookAtCameraYonly : MonoBehaviour {

	public Camera cameraToLookAt;

	public bool useThisMethod = true;


	// Use this for initialization
	void Start () {
		//Unless you set another camera to look at, it will look at the main camera
		if(cameraToLookAt == null){
			cameraToLookAt = Camera.main;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(useThisMethod){

		//This code will make the object face the camera position
		Vector3 v = cameraToLookAt.transform.position - transform.position;
		v.x = v.z = 0.0f;
		transform.LookAt( cameraToLookAt.transform.position - v ); 
		transform.Rotate(0,180,0);

		if(Camera.main.orthographic == true){
			transform.Rotate(0,-9999999,0);
		}
		}
	}
}
