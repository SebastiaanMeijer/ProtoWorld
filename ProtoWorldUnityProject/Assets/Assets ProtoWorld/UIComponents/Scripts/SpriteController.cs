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

public class SpriteController : MonoBehaviour {
	public float scale = 1.0f;
	public float hideDistance = 10.0f;


	public void Update () {
		transform.rotation = Camera.main.transform.rotation;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		Sprite sprite = spriteRenderer.sprite;

		Vector3 center = sprite.bounds.center;
		Vector3 minimum = sprite.bounds.min;

		Vector3 centerPosition = transform.TransformPoint(center);
		Vector3 minimumPosition = transform.TransformPoint(minimum);

		Vector3 screenCenterPosition = Camera.main.WorldToScreenPoint(centerPosition);
		Vector3 screenMinimumPosition = Camera.main.WorldToScreenPoint(minimumPosition);

		screenCenterPosition.z = 0.0f;
		screenMinimumPosition.z = 0.0f;

		float pixelDistance = Mathf.Sqrt(sprite.texture.height * sprite.texture.height / 4.0f + sprite.texture.width * sprite.texture.width / 4.0f);
		float screenDistance = Vector3.Distance(screenCenterPosition, screenMinimumPosition);

		float localScale = pixelDistance * scale / screenDistance;

		transform.localScale = new Vector3(localScale, localScale, localScale);

		spriteRenderer.enabled = Vector3.Distance(Camera.main.transform.position, transform.position) > hideDistance;
	}
}
