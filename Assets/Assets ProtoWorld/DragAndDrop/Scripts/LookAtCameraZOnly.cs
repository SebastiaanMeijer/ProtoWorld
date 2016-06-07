using UnityEngine;
using System.Collections;

public class LookAtCameraZOnly : MonoBehaviour {
	
	public Camera cameraToLookAt;
	public float x2;
	public float y2;
	public float z2;
	public float x1;
	public float y1;
	public float z1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = cameraToLookAt.transform.position - transform.position;

		v.x = 90; 
		//v.y = y2; 
		v.z = 0;

		transform.LookAt( cameraToLookAt.transform.position); 
		transform.Rotate(x1,y1,z1);
	}
}
