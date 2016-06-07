//FURKAN SONMEZ

using UnityEngine;
using System.Collections;

public class waitingTime : MonoBehaviour {

	//NOT FINISHED
	//

	/*
	public Animator animator;
	public TimeController timeController;


	public static float avgWaitingTime = 0f;
	public static float totWaitingTime = 0f;
	public static int pedestriansWaiting = 0;
	public bool waiting = false;
	public float waitingStartTime = 0f;
	public float waitingTimeNow = 0f;
	public float randomNumber = 0;


	void Awake (){
		randomNumber = Random.value;
		if(randomNumber < 0.9){
			Destroy(this);

		}


		timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
		animator = gameObject.GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(waitingOrNot());
	}

	// Update is called once per frame
	void Update () {
		if(waiting == false){
			if(animator.GetBool("OnStation")){
				Debug.Log("WAITING");
				if(waiting == false){
					waitingStartTime = timeController.gameTime;
				}
				waiting = true;
			}
		}
		else{
			waitingTimeNow = timeController.gameTime - waitingStartTime;
			if(animator.GetBool(

		}

	}

	public IEnumerator waitingOrNot(){
		yield return new WaitForSeconds(3);
		if(animator.GetBool("OnStation")){
			Debug.Log("WAITING");
			if(waiting == false){
				waitingStartTime = timeController.gameTime;
			}
			waiting = true;
			StartCoroutine(waitingTimer());
		}
		else{
			Debug.Log("I AM DONE WAITING");
			if(waiting == true){
				StartCoroutine(doneWaiting());
			}
			waiting = false;

		}

	}

	public IEnumerator waitingTimer(){
		yield return new WaitForSeconds(1);
		waitingTimeNow = timeController.gameTime - waitingStartTime;
		StartCoroutine(waitingTimer());
	}

	public IEnumerator doneWaiting(){
		Debug.Log("DONEWAITING ENUMERATOR");
		pedestriansWaiting = pedestriansWaiting + 1;
		totWaitingTime = totWaitingTime + waitingTimeNow;
		avgWaitingTime = totWaitingTime / pedestriansWaiting;
		Debug.Log(avgWaitingTime);
		StopCoroutine(waitingTimer());
		yield return new WaitForSeconds(1);
	}

	*/
}
