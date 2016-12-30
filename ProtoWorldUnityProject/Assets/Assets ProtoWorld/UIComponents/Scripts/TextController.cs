/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * UI Components
 * 
 * Berend Wouda
 */
using UnityEngine;

public class TextController : MonoBehaviour {
	public float scale = 1.0f;
	public float hideDistance = 10.0f;

	public float height = 0.1f;


	public void Update () {
		transform.rotation = Camera.main.transform.rotation;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		
		Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 minimum = new Vector3(-height / 2.0f, -height / 2.0f, 0.0f);

		Vector3 centerPosition = transform.TransformPoint(center);
		Vector3 minimumPosition = transform.TransformPoint(minimum);

		Vector3 screenCenterPosition = Camera.main.WorldToScreenPoint(centerPosition);
		Vector3 screenMinimumPosition = Camera.main.WorldToScreenPoint(minimumPosition);

		screenCenterPosition.z = 0.0f;
		screenMinimumPosition.z = 0.0f;
		
		float screenDistance = Vector3.Distance(screenCenterPosition, screenMinimumPosition);

		float localScale = scale / screenDistance;

		transform.localScale = new Vector3(localScale, localScale, localScale);

		meshRenderer.enabled = Vector3.Distance(Camera.main.transform.position, transform.position) > hideDistance;
	}
}
