/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;

public class CameraControlWithAcceleration : MonoBehaviour {
	
	public GameObject backPlane = null;
	
	Vector3 mouseWorldPosition;
	
	Vector3 mouseScreenPosition;
	Ray mouseRay;
//	Vector3 dragPosOrigin;
	
	bool dragging = false;
	Vector3 initMouseWorldPosition;
	Ray initMouseRay;
	Vector3 initCameraPos;
	Plane cameraPlane;
	Vector3 straightDownFromInit;
	Vector3 asIfHitPoint;
	
	public Vector3 targetCameraPosition;
	
	public float moveSpeed = 15;
	
//	float desiredDistFromGround = 75;
	
	// Use this for initialization
	void Start () {
		targetCameraPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		
		Vector3 oldMouseAt = mouseWorldPosition;
		Vector3 oldMousePosition = mouseScreenPosition;
		//Ray oldMouseRay = mouseRay;
		setMousePositions();
		
		
		if (Input.GetMouseButton(0)){
			
			if (!dragging) {
				dragging = true;	
				initMouseRay = mouseRay;
				initMouseWorldPosition = mouseWorldPosition;
				initCameraPos = gameObject.transform.position;
				
				
				RaycastHit downRayHit = new RaycastHit();
				Physics.Raycast(new Ray(initCameraPos, Vector3.down), out downRayHit, 10000f, 1 << 9); // 9 is the ground layer
				straightDownFromInit = downRayHit.point;
				
				asIfHitPoint = straightDownFromInit + (initMouseWorldPosition - initCameraPos);
//				Debug.Log(initCameraPos+":"+straightDownFromInit+":"+asIfHitPoint);
			}
			
			//for camera fixed on a terrain trasposed to itself
//			Ray asIfCameraRay = new Ray(initCameraPos, mouseRay.direction);
//			RaycastHit rayHit = new RaycastHit();
//			Physics.Raycast(asIfCameraRay, out rayHit);
//			Vector3 asIfWorldPosition = rayHit.point;
//			
//			targetCameraPosition = initCameraPos + (initMouseWorldPosition - asIfWorldPosition);
			
			// for camera fixed on a plane
//			cameraPlane = new Plane (Vector3.up, camera.transform.position);
//			Ray rayFromMousePoint = new Ray(initMouseWorldPosition, mouseRay.direction);
//			float dist = 0;
//			cameraPlane.Raycast(rayFromMousePoint, out dist);
//			targetCameraPosition = rayFromMousePoint.GetPoint(dist);
				
			
			//for camera fixed on a terrain trasposed straight up
			
			
			RaycastHit rayHit = new RaycastHit();
			Debug.DrawRay(asIfHitPoint, -mouseRay.direction*100, Color.red);
			bool success = Physics.Raycast(new Ray(asIfHitPoint, -mouseRay.direction), out rayHit, 10000f, 1 << 9); // 9 is the ground layer
			
			if (success) {
				
				Vector3 movedGroundPoint = rayHit.point;
				targetCameraPosition = movedGroundPoint+(initCameraPos - straightDownFromInit);
//				Debug.Log(success+":"+movedGroundPoint+":"+(initCameraPos - straightDownFromInit));
			}
		}
		else {
			dragging = false;	
		}
		
		
		
		
		if (Input.GetMouseButton(1)){
			Vector3 posChange = (oldMousePosition - Input.mousePosition);
			Vector3 oldRot = gameObject.transform.rotation.eulerAngles;
			gameObject.transform.RotateAround(Vector3.up, posChange.x*-0.005f);
			gameObject.transform.RotateAround(gameObject.transform.right, posChange.y*0.005f);
		}
		
		targetCameraPosition+=Vector3.up*Input.GetAxis("Mouse ScrollWheel")*-30;
		setMousePositions();
		
		Vector3 keyBoardControlToMove = Vector3.zero;
		if(Input.GetKey(KeyCode.A)) {
			keyBoardControlToMove-=transform.right;
		}
		if(Input.GetKey(KeyCode.D)) {
			keyBoardControlToMove+=transform.right;
		}
		if(Input.GetKey(KeyCode.W)) {
			keyBoardControlToMove+=transform.forward;
		}
		if(Input.GetKey(KeyCode.S)) {
			keyBoardControlToMove-=transform.forward;
		}
		keyBoardControlToMove.Normalize();
		if (keyBoardControlToMove.sqrMagnitude > 0.1f){
			keyBoardControlToMove*=moveSpeed;
			RaycastHit targetRayHit = new RaycastHit();
			if (!Physics.Raycast(new Ray(targetCameraPosition, Vector3.down), out targetRayHit, 10000f, 1 << 9)) // 9 is the ground layer
				Physics.Raycast(new Ray(targetCameraPosition, Vector3.up), out targetRayHit, 10000f, 1 << 9);
			float height = targetCameraPosition.y-targetRayHit.point.y;
			targetCameraPosition+=keyBoardControlToMove;
			targetRayHit = new RaycastHit();
			if (!Physics.Raycast(new Ray(targetCameraPosition, Vector3.down), out targetRayHit, 10000f, 1 << 9)) // 9 is the ground layer
				Physics.Raycast(new Ray(targetCameraPosition, Vector3.up), out targetRayHit, 10000f, 1 << 9);
			targetCameraPosition.y = targetRayHit.point.y+height;
		}
		
		if(Input.GetKey(KeyCode.Space)) {
			targetCameraPosition+=Vector3.up*moveSpeed;
		}
		if(Input.GetKey(KeyCode.LeftShift)) {
			targetCameraPosition-=Vector3.up*moveSpeed;
		}
		RaycastHit upRayHit = new RaycastHit();
		if (!Physics.Raycast(new Ray(targetCameraPosition, Vector3.down), out upRayHit, 10000f, 1 << 9)) // 9 is the ground layer
			Physics.Raycast(new Ray(targetCameraPosition, Vector3.up), out upRayHit, 10000f, 1 << 9);
		if (targetCameraPosition.y < upRayHit.point.y+10) {
			targetCameraPosition.y = upRayHit.point.y+10;
		}
		//gameObject.transform.position = targetCameraPosition;
		gameObject.transform.position = gameObject.transform.position + (targetCameraPosition-gameObject.transform.position)*0.2f;
		
		
//		RaycastHit rayHit = new RaycastHit();
//		Physics.Raycast(new Ray(gameObject.transform.position, Vector3.down), out rayHit, 10000f, 1 << 9); // 9 is the ground layer
//		float distToGround = rayHit.distance;
//		
//		gameObject.transform.position+=Vector3.up*(desiredDistFromGround-distToGround);
		
	}
	
	private void setMousePositions() {
		
		mouseScreenPosition = Input.mousePosition;
		mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		RaycastHit rayHit = new RaycastHit();
		Physics.Raycast(mouseRay, out rayHit, 10000f, 1 << 9); // 9 is the ground layer
		mouseWorldPosition = rayHit.point;
		
	}
	
	public Vector3 getMouseColliderPos() {
		return mouseWorldPosition;	
	}
	
}








