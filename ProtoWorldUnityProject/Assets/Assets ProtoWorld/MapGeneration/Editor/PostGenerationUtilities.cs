using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class Utilities : MonoBehaviour {
	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Remove Roads with Vertices with Non-finite Values")]
	private static void RemoveRoadsWithVerticesWithNonFiniteValues() {
		Debug.Log("Removing roads with vertices with non-finite values...");

		GameObject[] roads = GameObject.FindGameObjectsWithTag("Line");

		List<GameObject> roadsWithVerticesWithNonFiniteValues = new List<GameObject>();

		for(int roadIndex = 0; roadIndex < roads.Length; roadIndex++) {
			if(EditorUtility.DisplayCancelableProgressBar("Removing roads with vertices with non-finite values...", "", roadIndex / (float) roads.Length)) {
				return;
			}

			GameObject gameObject = roads[roadIndex];

			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			
			if(meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.vertices != null) {
				for(int vertexIndex = 0; vertexIndex < meshFilter.sharedMesh.vertexCount; vertexIndex++) {
					Vector3 vertex = meshFilter.sharedMesh.vertices[vertexIndex];
					
					if(float.IsInfinity(vertex.x) || float.IsNaN(vertex.x) || float.IsInfinity(vertex.y) || float.IsNaN(vertex.y) || float.IsInfinity(vertex.z) || float.IsNaN(vertex.z)) {
						roadsWithVerticesWithNonFiniteValues.Add(gameObject);

						// Only add each game object once.
						break;
					}
				}
			}
		}

		while(roadsWithVerticesWithNonFiniteValues.Count > 0) {
			GameObject gameObject = roadsWithVerticesWithNonFiniteValues[0];

			roadsWithVerticesWithNonFiniteValues.RemoveAt(0);

			Debug.Log("Removing road \"" + gameObject.name + "\"...");

			DestroyImmediate(gameObject);
			gameObject = null;
		}

		Debug.Log("Removed roads with vertices with non-finite values.");

		EditorUtility.ClearProgressBar();
	}
}
