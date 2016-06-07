using System.Collections;
using UnityEngine;
using UnityEngine.Events;

class DragTransformBackup2 : MonoBehaviour
{
	//This is the child of the object. This will show the arrow for the rotation.
	public Transform rotationCanvas;
	//This is the object control panel on screen space, this Transform will be assigned automatically at the awake function
	public Transform ObjectControlPanel;

	//These are events on which functions can be called.
	public UnityEvent m_Selected;
	public UnityEvent m_Deselected;
	public UnityEvent m_Dropped;
	public UnityEvent m_Moved;

	//These are booleans to check what the states of the object are. These bools are necessary for different if statements 
	public bool dropped = false;
	public bool dragging = false;
	public bool justDragged = true;
	public bool clicked = false;
	public bool selected;
	public bool ClickingOnChild;
	public bool justInstantiated = true;



	private float distance;
	public Vector3 hoveringover;

	private Vector3 nulVector;


	//these floats are needed to determine wether the player is just clicking the object or if he's gonna drag it
	public float StartMouseX;
	public float StartMouseY;
	// a bool to save wether its selected or not



	//this float holds the rotation of the object in the Y
	public float rotationY;
	//this float is to change the smoothness at which the rotation happens
	public float smooth = 10.0F;
	//this is the amount at which the object rotates
	public float rotationSpeed = 60.0F;

	float distanceClickAndSelf;
	public float distanceToDeselect = 10f;

	//These are the start and hightlightcolors of the object for when its selected or not. The highlightcolor can be set from the inspector
	private Color startcolor;
	public Color highLightColor;
	

	void Awake(){
		if(GetComponent<Renderer>().material.color != null)
		startcolor = GetComponent<Renderer>().material.color;
		// this code only applies to newly added objects. So objects that were added before the simulation will not be affected
		if(rayHitPositionClass.gameStartedBool == true && justInstantiated == true){
			//this will put the newly instantiated objects into the parent: InstantiatedObjects, to change the parent ,you have to change the string in the Find
			transform.parent = GameObject.Find("InstantiatedObjects").transform;

			dragging = true;
			GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color + highLightColor;
			StartCoroutine (timerToGoDeSelected ());
		}
		else
		{
			rotationCanvas.gameObject.SetActive (false);
		}

		if(GameObject.Find("ObjectControlPanel") != null){
		ObjectControlPanel = GameObject.Find("ObjectControlPanel").transform;
		}

	}
	

	public IEnumerator endTheJustInstantiated(){
		yield return new WaitForSeconds(0.5f);
		dragging = false;
		justInstantiated = false;
		clicked = false;
	}


	public IEnumerator timerToGoDeSelected(){
		yield return new WaitForSeconds(5f);
		Debug.LogError ("DESELECT NOW");
	}

	public void goDeselect(){
		ClickingOnChild = false;
	}

	public void dontDeselect(){
		ClickingOnChild = true;
	}


	//this code is only for after you have added an object, be it before the simulation or during the simulation
	void OnMouseDown()
	{
		StopCoroutine(timerToGoDeSelected ());
		clicked = true;
		//Store the first x and y position of the mouse
		StartMouseX = Input.mousePosition.x;
		StartMouseY = Input.mousePosition.y;

		distance = Vector3.Distance(transform.position,    Camera.main.transform.position);

		if(GameObject.Find("ObjectControlPanel").gameObject != null){
			RectTransform rtOCP = GameObject.Find("ObjectControlPanel").GetComponent<RectTransform>();
			Debug.LogError (rtOCP.position.y);
			if(Input.mousePosition.x > (rtOCP.position.x - (rtOCP.rect.width/2)) &&
			   Input.mousePosition.x < (rtOCP.position.x +(rtOCP.rect.width/2)) &&
			   Input.mousePosition.y < (rtOCP.position.y + (rtOCP.rect.height/2)) &&
			   Input.mousePosition.y > (rtOCP.position.y - (rtOCP.rect.height/2))){
				Debug.LogError ("SAY WHUUUT");

			}

		}


	}

	//this code is only for after you have added an object, be it before the simulation or during the simulation
	void OnMouseUp()
	{
		//if the mouse did not move while clicking, it wasnt a drag so the rotationCanvas will be either active of deactive
		if(dragging == false)
		if(rotationCanvas.gameObject.activeSelf == true){
			//Debug.LogError ("I should now deactivate the object");
			rotationCanvas.gameObject.SetActive (false);
			selected = false;
			if(GetComponent<Renderer>().material.color != null)
			GetComponent<Renderer>().material.color = startcolor;
			//The Deselected events invoker
			m_Deselected.Invoke ();
		}
		else if(rotationCanvas.gameObject.activeSelf == false){
			//Debug.LogError ("I should now activate the object");
			rotationCanvas.gameObject.SetActive (true);
			selected = true;
			if(GetComponent<Renderer>().material.color != null)
			GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color + highLightColor;
			//ClickingOnChild = true;
			//The selected events invoker
			m_Selected.Invoke ();
		}

		dragging = false;
		justInstantiated = false;
		clicked = false;


	}

	public IEnumerator justDraggedTimer(){
		yield return new WaitForSeconds(0.1f);
		justDragged = false;
		clicked = false;

	}



	void Update()
	{

		if(clicked == true && dragging == false){
			// check if the mouse has left either the first mouseposition x or y
			if((StartMouseX != Input.mousePosition.x || StartMouseY != Input.mousePosition.y) && clicked == true){
				dropped = false;
				dragging = true;
				justDragged = true;
				//Debug.LogError("Moved");
				//The Moved events invoker
				m_Moved.Invoke ();
			}
		}

		distanceClickAndSelf = Vector3.Distance (transform.position, rayHitPositionClass.hitLocation);
		//Debug.LogError (distanceClickAndSelf);
		//distanceClickAndSelf > distanceToDeselect &&
		if(rayHitPositionClass.dragging){
		if( dragging == false && selected && clicked != true){

				if(GameObject.Find("ObjectControlPanel").gameObject != null){
					RectTransform rtOCP = GameObject.Find("ObjectControlPanel").GetComponent<RectTransform>();

					if(Input.mousePosition.x > (rtOCP.position.x - (rtOCP.rect.width/2)) &&
					   Input.mousePosition.x < (rtOCP.position.x +(rtOCP.rect.width/2)) &&
					   Input.mousePosition.y < (rtOCP.position.y + (rtOCP.rect.height/2)) &&
					   Input.mousePosition.y > (rtOCP.position.y - (rtOCP.rect.height/2))){
						Debug.LogError ("SAY WHUUUT");

					}
					else{
						if(ClickingOnChild == false){
							selected = false;
							//The Deselected events invoker
							m_Deselected.Invoke ();
						}

					}

					
				}
		}
		}

		if(selected == false){

			rotationCanvas.gameObject.SetActive (false);
			selected = false;
			if(GetComponent<Renderer>().material.color != null)
				GetComponent<Renderer>().material.color = startcolor;

		}
		else if(selected == true){
			if(Input.GetMouseButtonDown (0)){
				Debug.LogError ("I clicked");
				RaycastHit hit;
				
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				// only if the ray hits an object(other than the objects with layer 13(the objects that are added)) the rest of the code will follow
				if (Physics.Raycast(ray, out hit, Mathf.Infinity)){
				if(hit.transform.name != transform.name){
					Debug.LogError ("I did not hit myself");
				}
				if(hit.transform.name == rotationCanvas.name){
					Debug.LogError ("I hit the rotationcanvas");
				}
				}
			}

		}


		// since OnMouseUp cannot be called when you add a new object, because the OnMouseDown has never happened for this object(since its a new object) this is used
		if(rayHitPositionClass.dragging != true){
			dragging = false;
			clicked = false;
			if(justDragged == true)
			StartCoroutine (justDraggedTimer ());
			//the code to be played when the object has been added can be put here
			if(justDragged == true){

				if(dropped == false){
				//The Dropped events invoker
				m_Dropped.Invoke ();
				dropped = true;
				}

				if(GameObject.Find("DragAndDropList") == null){
					Debug.LogError ("its inactive");
					if(Input.mousePosition.x > (SmallDeleteObjectClass.SxPosition - (SmallDeleteObjectClass.Swidth/2)) &&
					   Input.mousePosition.x < (SmallDeleteObjectClass.SxPosition +(SmallDeleteObjectClass.Swidth/2)) &&
					   Input.mousePosition.y < (SmallDeleteObjectClass.SyPosition + (SmallDeleteObjectClass.Sheight/2)) && Input.mousePosition.y > (SmallDeleteObjectClass.SyPosition - (SmallDeleteObjectClass.Sheight/2))){
						
						Destroy (gameObject);
					}
				}
				else{
					Debug.LogError ("its active");
					if(Input.mousePosition.x > (DeleteObjectClass.xPosition - (DeleteObjectClass.width/2)) &&
					   Input.mousePosition.x < (DeleteObjectClass.xPosition +(DeleteObjectClass.width/2)) &&
					   Input.mousePosition.y < (DeleteObjectClass.yPosition + (DeleteObjectClass.height/2)) && Input.mousePosition.y > (DeleteObjectClass.yPosition - (DeleteObjectClass.height/2))){

						Destroy (gameObject);
					}
				}

			}
		}
		//You have to be dragging this object to be able to rotate the object
		if(dragging){
			rotationY = rotationY + Input.GetAxis("Mouse ScrollWheel") * -rotationSpeed;
			Quaternion target = Quaternion.Euler(0, rotationY, 0);
		//	transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
		}
		//transform.rotation = rotationY;
		//the nulVector is used to prevent strange things from happening when there is no rayHit location yet.
		if (dragging == true && rayHitPositionClass.hitLocation != nulVector)
		{


			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 rayPoint = ray.GetPoint(distance);
			//the hoveringover Vector3 can be changed from the object Inspector for different objects. This is important so the object is not spawned under the ground.
			transform.position = rayHitPositionClass.hitLocation + hoveringover;
		}

	}


}