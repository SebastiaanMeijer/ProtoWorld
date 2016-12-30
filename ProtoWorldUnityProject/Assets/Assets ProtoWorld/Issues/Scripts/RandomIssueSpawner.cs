/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Issues Module
 * 
 * Furkan Sonmez
 * Berend Wouda
 */

using UnityEngine;
using System.Collections;

public class RandomIssueSpawner : MonoBehaviour {
	public GameObject IssueObject;
	public Transform parentTransform;

	public float interval = 30.0f;

	private WaitForSeconds wait;


	void Start() {
		wait = new WaitForSeconds(interval);

		StartCoroutine(IssueSpawnerByChance());
	}


	public IEnumerator IssueSpawnerByChance() {
		yield return wait;
		CameraControl cameraControl = Camera.main.GetComponent<CameraControl>();
		Instantiate(IssueObject, new Vector3(Random.Range(0, cameraControl.maxUp), 2, Random.Range(0, cameraControl.maxRight)), Quaternion.identity, parentTransform);
		StartCoroutine(IssueSpawnerByChance());
	}
}

