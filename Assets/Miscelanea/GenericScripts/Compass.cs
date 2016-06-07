using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {
	
	private Transform mainCamera;
	private float normRot;
	public Vector3 OffsetFromCamera=new Vector3(0.5f,0.34f,2f);
	// Use this for initialization
	void Start () {
		mainCamera=GameObject.FindWithTag("MainCamera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		normRot = Mathf.Abs(mainCamera.eulerAngles.y);
		if (normRot > 360) 
		{
			normRot = normRot % 360;
		}
		var temp=transform.eulerAngles;
		temp.y= normRot - 180; // because we look at the "back"
		transform.eulerAngles=temp;
		transform.rotation=mainCamera.rotation;
		transform.position= mainCamera.position+OffsetFromCamera;
	}
}
