//Furkan Sonmez

using UnityEngine;
using System.Collections;

public class rayHitPositionClass : MonoBehaviour {
	//This is the vector3 where the ray hit location will be stored into
	public static Vector3 hitLocation;

	//this is a static bool which states if the user is holding the mousebutton or not
	public static bool dragging = false;
		
	//this bool will be used to check if the game has started or not, this kicks in after 0.5 seconds after the game has been started
	public static bool gameStartedBool = false;


	void Start(){
		StartCoroutine (GameStarted());
	}

	// this method is used to not drag all the objects that had already been dropped into the simulation before the simulation had started
	public IEnumerator GameStarted(){
		yield return new WaitForSeconds(0.5f);
		gameStartedBool = true;

	}

	void Update(){
		// whenever you click anywhere this will happen and you will be in the state of "dragging"
		if (Input.GetMouseButtonDown(0)) {
			dragging = true;
		}

		// this will end the state of "dragging"
		if (Input.GetMouseButtonUp(0)) {
			dragging = false;
		}


		if(dragging == true){
			RaycastHit hit;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// only if the ray hits an object(other than the objects with layer 13(the objects that are added)) the rest of the code will follow
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 13))
				if(hit.collider != null){

				// this will store the point where anything is hit in the static Vector3 hitLocation. So use rayHitPositionClass.hitLocation to use this Vector3
				hitLocation = hit.point;
			}
			
		}
		}
}
