using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class Utilities : MonoBehaviour {
	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Remove Game Objects with Vertices with Non-finite Values")]
	private static void RemoveGameObjectsWithVerticesWithNonFiniteValues() {
		Debug.Log("Removing game objects with vertices with non-finite values...");

		GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

		List<GameObject> gameObjectsWithVerticesWithInifiniteValues = new List<GameObject>();

		foreach(GameObject gameObject in gameObjects) {
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

			if(meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.vertices != null) {
				for(int index = 0; index < meshFilter.sharedMesh.vertexCount; index++) {
					Vector3 vertex = meshFilter.sharedMesh.vertices[index];
					
					if(float.IsInfinity(vertex.x) || float.IsNaN(vertex.x) || float.IsInfinity(vertex.y) || float.IsNaN(vertex.y) || float.IsInfinity(vertex.z) || float.IsNaN(vertex.z)) {
						gameObjectsWithVerticesWithInifiniteValues.Add(gameObject);

						// Only add each game object once.
						break;
					}
				}
			}
		}

		while(gameObjectsWithVerticesWithInifiniteValues.Count > 0) {
			GameObject gameObject = gameObjectsWithVerticesWithInifiniteValues[0];

			gameObjectsWithVerticesWithInifiniteValues.RemoveAt(0);

			Debug.Log("Removing " + gameObject.name + "...");

			DestroyImmediate(gameObject);
			gameObject = null;
		}

		Debug.Log("Removed game objects with vertices with non-finite values.");
	}
}
