/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Post Generation Utilities
 * 
 * Berend Wouda
 */

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class Utilities : Editor {
	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Remove Buildings with Vertices with Non-finite Values")]
	public static void RemoveBuildingsWithVerticesWithNonFiniteValues() {
		RemoveGameObjectsWithVerticesWithNonFiniteValues("building", "buildings", "Building");
	}

	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Remove Roads with Vertices with Non-finite Values")]
	public static void RemoveRoadsWithVerticesWithNonFiniteValues() {
		RemoveGameObjectsWithVerticesWithNonFiniteValues("road", "roads", "Line");
	}

	private static void RemoveGameObjectsWithVerticesWithNonFiniteValues(string singularName, string pluralName, string tag) {
		Debug.Log("Removing " + pluralName + " with vertices with non-finite values...");

		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);

		List<GameObject> gameObjectsWithVerticesWithNonFiniteValues = new List<GameObject>();

		for(int gameObjectIndex = 0; gameObjectIndex < gameObjects.Length; gameObjectIndex++) {
			if(EditorUtility.DisplayCancelableProgressBar("Retrieving " + pluralName + " with vertices with non-finite values...", "", gameObjectIndex / (float) gameObjects.Length)) {
				return;
			}

			GameObject gameObject = gameObjects[gameObjectIndex];

			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

			if(meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.vertices != null) {
				for(int vertexIndex = 0; vertexIndex < meshFilter.sharedMesh.vertexCount; vertexIndex++) {
					Vector3 vertex = meshFilter.sharedMesh.vertices[vertexIndex];

					if(float.IsInfinity(vertex.x) || float.IsNaN(vertex.x) || float.IsInfinity(vertex.y) || float.IsNaN(vertex.y) || float.IsInfinity(vertex.z) || float.IsNaN(vertex.z)) {
						gameObjectsWithVerticesWithNonFiniteValues.Add(gameObject);

						// Only add each game object once.
						break;
					}
				}
			}
		}

		EditorUtility.ClearProgressBar();

		float gameObjectsWithVerticesWithNonFiniteValuesCount = gameObjectsWithVerticesWithNonFiniteValues.Count;

		while(gameObjectsWithVerticesWithNonFiniteValues.Count > 0) {
			if(EditorUtility.DisplayCancelableProgressBar("Removing " + pluralName + " with vertices with non-finite values...", "", (gameObjectsWithVerticesWithNonFiniteValuesCount - gameObjectsWithVerticesWithNonFiniteValues.Count) / gameObjectsWithVerticesWithNonFiniteValuesCount)) {
				return;
			}

			GameObject gameObject = gameObjectsWithVerticesWithNonFiniteValues[0];

			gameObjectsWithVerticesWithNonFiniteValues.RemoveAt(0);

			Debug.Log("Removing " + singularName + " \"" + gameObject.name + "\"...");

			DestroyImmediate(gameObject);
			gameObject = null;
		}

		EditorUtility.ClearProgressBar();

		Debug.Log("Removed " + pluralName + " with vertices with non-finite values.");
	}


	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Assign Missing Layers to Buildings")]
	public static void AssignMissingLayersToBuildings() {
		AssignMissingLayersToGameObjects("building", "buildings", "Buildings", "Building", "Buildings");
	}

	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Assign Missing Layers to Roads")]
	public static void AssignMissingLayersToRoads() {
		AssignMissingLayersToGameObjects("road", "roads", "Lines", "Line", "Lines");
	}

	private static void AssignMissingLayersToGameObjects(string singularName, string pluralName, string name, string tag, string layer) {
		Debug.Log("Assigning missing layers to " + pluralName + "...");

		GameObject group = GameObject.Find(name);

		if(group.layer == LayerMask.NameToLayer("Default")) {
			Debug.Log("Assigning missing layer \"" + layer + "\" to group \"" + group.name + "\"...");

			group.layer = LayerMask.NameToLayer(layer);
		}

		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);

		for(int index = 0; index < gameObjects.Length; index++) {
			if(EditorUtility.DisplayCancelableProgressBar("Assigning missing layers to " + pluralName + "...", "", index / (float) gameObjects.Length)) {
				return;
			}

			GameObject gameObject = gameObjects[index];

			if(gameObject.layer == LayerMask.NameToLayer("Default")) {
				Debug.Log("Assigning missing layer \"" + layer + "\" to " + singularName + " \"" + gameObject.name + "\"...");

				gameObject.layer = LayerMask.NameToLayer(layer);
			}
		}

		EditorUtility.ClearProgressBar();

		Debug.Log("Assigned missing layers to " + pluralName + ".");
	}


	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Assign Missing Layers to Grouped Buildings")]
	public static void AssignMissingLayersToGroupedBuildings() {
		AssignMissingLayersToGroupedGameObjects("building", "buildings", "CombinedBuildings");
	}

	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Assign Missing Layers to Grouped Roads")]
	public static void AssignMissingLayersToGroupedRoads() {
		AssignMissingLayersToGroupedGameObjects("road", "roads", "CombinedLines");
	}

	private static void AssignMissingLayersToGroupedGameObjects(string singularName, string pluralName, string tag) {
		Debug.Log("Assigning missing layers to grouped " + pluralName + "...");

		GameObject[] combinations = GameObject.FindGameObjectsWithTag(tag);

		for(int combinationsIndex = 0; combinationsIndex < combinations.Length; combinationsIndex++) {
			if(EditorUtility.DisplayCancelableProgressBar("Assigning missing layers to grouped " + pluralName + "...", "", combinationsIndex / (float) combinations.Length)) {
				return;
			}

			GameObject combination = combinations[combinationsIndex];

			// Set the layer to the layer the first matching child is in, which is likely
			// to be the layer all the children are in, since their materials are the same.
			if(combination.layer == LayerMask.NameToLayer("Default")) {
				for(int childIndex = 0; childIndex < combination.transform.parent.childCount; childIndex++) {
					GameObject child = combination.transform.parent.GetChild(childIndex).gameObject;

					if(child.tag != tag) {
						if(child.GetComponent<Renderer>().sharedMaterial.name == combination.GetComponent<Renderer>().sharedMaterial.name) {
							Debug.Log("Assigning missing layer \"" + LayerMask.LayerToName(child.layer) + "\" to grouped " + singularName + " \"" + combination.name + "\"...");

							combination.layer = child.layer;

							break;
						}
					}
				}
			}
		}

		EditorUtility.ClearProgressBar();

		Debug.Log("Assigned missing layers to grouped " + pluralName + ".");
	}


	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Remove Empty Buildings")]
	public static void RemoveEmptyBuildings() {
		RemoveEmptyGameObjects("building", "buildings", "Building");
	}

	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Utilities/Remove Empty Roads")]
	public static void RemoveEmptyRoads() {
		RemoveEmptyGameObjects("road", "roads", "Line");
	}

	private static void RemoveEmptyGameObjects(string singularName, string pluralName, string tag) {
		Debug.Log("Removing empty " + pluralName + "...");

		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);

		List<GameObject> emptyGameObjects = new List<GameObject>();

		for(int gameObjectIndex = 0; gameObjectIndex < gameObjects.Length; gameObjectIndex++) {
			if(EditorUtility.DisplayCancelableProgressBar("Retrieving empty " + pluralName + "...", "", gameObjectIndex / (float) gameObjects.Length)) {
				return;
			}

			GameObject gameObject = gameObjects[gameObjectIndex];

			MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
			MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

			if(!meshRenderer.enabled && meshCollider == null) {
				emptyGameObjects.Add(gameObject);
			}
		}

		EditorUtility.ClearProgressBar();

		float emptyGameObjectsCount = emptyGameObjects.Count;

		while(emptyGameObjects.Count > 0) {
			if(EditorUtility.DisplayCancelableProgressBar("Removing empty " + pluralName + "...", "", (emptyGameObjectsCount - emptyGameObjects.Count) / emptyGameObjectsCount)) {
				return;
			}

			GameObject gameObject = emptyGameObjects[0];

			emptyGameObjects.RemoveAt(0);

			Debug.Log("Removing " + singularName + " \"" + gameObject.name + "\"...");

			DestroyImmediate(gameObject);
			gameObject = null;
		}

		EditorUtility.ClearProgressBar();

		Debug.Log("Removed empty " + pluralName + ".");
	}
}
