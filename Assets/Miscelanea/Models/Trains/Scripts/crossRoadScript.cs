using UnityEngine;
using System.Collections;

public class crossRoadScript : MonoBehaviour {
	public TimeController timeController;


	public static float crossRoadClosingTimer = 0f;
	public float crossRoadOpeningTimer = 10f;

	// Use this for initialization
	void Start () {
		timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
		transform.position = new Vector3(transform.position.x,100,transform.position.z);
		transform.localScale = new Vector3(0,transform.localScale.y,transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
		if(TrainMovement.crossRoadClosed == 1){
			crossRoadClosingTimer = timeController.gameTime;
			TrainMovement.crossRoadClosed = 2;
			transform.position = new Vector3(transform.position.x,0,transform.position.z);
		}
			
		if(TrainMovement.crossRoadClosed == 2){
			if(transform.localScale.x < 29)
				transform.localScale = new Vector3(transform.localScale.x + 6 * Time.deltaTime,transform.localScale.y,transform.localScale.z);
			else
				transform.localScale = new Vector3(29,transform.localScale.y,transform.localScale.z);


			if(crossRoadOpeningTimer < timeController.gameTime - crossRoadClosingTimer){
				Debug.Log("CROSSROAD OPENED");
				TrainMovement.crossRoadClosed = 0;
				transform.position = new Vector3(transform.position.x,100,transform.position.z);
				transform.localScale = new Vector3(0,transform.localScale.y,transform.localScale.z);
			}
		}
			
	}
}
