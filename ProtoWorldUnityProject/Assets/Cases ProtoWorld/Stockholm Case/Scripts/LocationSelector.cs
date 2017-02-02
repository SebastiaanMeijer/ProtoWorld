/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 *
 * Stockholm MatSIM integration.
 * 
 * Berend Wouda
 * 
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class LocationSelector : MonoBehaviour {
	public GameObject box;

	public float height = 110.0f;
	
	private Plane plane;
	
	private bool dragging;

	private Vector3 startPoint;
	private Vector3 endPoint;
	
	private GameObject boxInstance;
	
	private CameraControl cameraControl;


	public void Awake() {
		cameraControl = Camera.main.GetComponent<CameraControl>();
	}

	public void Start () {
		plane = new Plane(Vector3.up, -height);
	}


	public void Update () {
		if(Input.GetKeyUp(KeyCode.Mouse0) && dragging) {
			endPoint = getPoint(Input.mousePosition);

			Destroy(boxInstance);
			boxInstance = null;
			
			focus(startPoint, endPoint);

			dragging = false;
		}

		if(Input.GetKeyDown(KeyCode.Mouse0) && !dragging && !EventSystem.current.IsPointerOverGameObject()) {
			startPoint = getPoint(Input.mousePosition);
			
			if(!dragging) {
				dragging = true;

				boxInstance = Instantiate(box, startPoint, Quaternion.identity, transform) as GameObject;
			}
		}

		if(dragging) {
			endPoint = getPoint(Input.mousePosition);

			Vector3 size = endPoint - startPoint;

			boxInstance.transform.localScale = new Vector3(size.x, 1.0f, size.z);
		}
	}


	private Vector3 getPoint(Vector3 screenPosition) {
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);

		float distance;

		// This can never fail with the way we set up our camera.
		plane.Raycast(ray, out distance);

		return ray.GetPoint(distance);
	}

	
	private void focus(Vector3 start, Vector3 end) {
		Vector3 size = end - start;

		Vector3 center = start + 0.5f * size;

		// This is pure emperical conjecture.
		Vector3 position = center + Vector3.up * (0.5f * size.magnitude - height);
		
		cameraControl.targetCameraPosition = position;
	}
}
