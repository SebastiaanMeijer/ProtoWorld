using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {
	
	float speed=1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	transform.Rotate(Vector3.up,speed* Time.deltaTime);
	transform.Rotate(Vector3.left,speed* Time.deltaTime);
	if (Input.GetKey(KeyCode.UpArrow))
			speed-=1f;
	if (Input.GetKey(KeyCode.DownArrow))
			speed+=1f;
	}	
	
	}
