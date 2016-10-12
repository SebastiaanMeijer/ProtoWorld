using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

public class CheckForShittierLines : EditorWindow {

	[MenuItem("Window/CheckForShittierLines")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(CheckForShittierLines));
	}

	public void OnGUI() {
		if (GUILayout.Button("Check for shittier lines")) {
			CheckShitty();
		}
	}

	private static void CheckShitty() {
		Debug.Log("Checking for shitty lines...");

		GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

		Debug.Log("Number of game objects: " + gameObjects.Length);

		List<GameObject> list = new List<GameObject>();

		foreach(GameObject gameObject in gameObjects) {
			//Debug.Log("Now checking: " + gameObject.name);

			try {
				Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh; // Check for both nulls.
				Vector3 vertex = mesh.vertices[0]; // TODO Check all vertices.

				if(float.IsInfinity(vertex.x) || float.IsNaN(vertex.x) || float.IsInfinity(vertex.y) || float.IsNaN(vertex.y) || float.IsInfinity(vertex.z) || float.IsNaN(vertex.z)) {
					//Debug.Log("I FOUND IT I FOUND IT " + gameObject.name);
					//Debug.Log(vertex.ToString());

					list.Add(gameObject);
				}
			}
			catch(MissingComponentException exception) {
				//Debug.Log("No mesh filter on this bitch. " + gameObject.name);
			}
			catch(NullReferenceException exception) {
				//Debug.Log("No mesh on this bitch. " + gameObject.name);
			}
			catch(IndexOutOfRangeException exception) {
				//Debug.Log("No vertex on this bitch. " + gameObject.name);
			}
		}

		Debug.Log("Number of found game objects: " + list.Count);

		foreach(GameObject gameObject in list) {
			Debug.Log("Found: " + gameObject.name);

			Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			Vector3 vertex = mesh.vertices[0];

			Debug.Log("Vertex: " + vertex.ToString());
		}

		Debug.Log("Done!");

		while(list.Count > 0) {
			GameObject gameObject = list[0];

			list.RemoveAt(0);

			string name = gameObject.name;

			Debug.Log("Now fucking: " + name);

			DestroyImmediate(gameObject);
			gameObject = null;

			Debug.Log("Fucked.");
		}

		Debug.Log("Done again! " + list.Count);
	}
}