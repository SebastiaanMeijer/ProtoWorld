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

using UnityEngine;
using UnityEditor;

public class ReplaceMaterialsWindow : EditorWindow {
	private string tag;
	private Material material;

	[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Materials/Replace Materials")]
	public static void ReplaceMaterials() {
		EditorWindow.GetWindow(typeof(ReplaceMaterialsWindow), true, "Replace Materials").Show();
	}

	public void Awake() {
		Vector2 screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		Vector2 windowSize = new Vector2(400, 70);

		Vector2 windowPosition = (screenSize - windowSize) / 2;

		position = new Rect(windowPosition, windowSize);
		
		minSize = windowSize;
		maxSize = windowSize;
	}

	private void OnGUI() {
		tag = EditorGUILayout.TagField("Tag:", tag);
		material = EditorGUILayout.ObjectField("Material:", material, typeof(Material), false) as Material;

		GUILayout.Space(10f);

		GUILayout.BeginHorizontal();

		if(GUILayout.Button("Replace Materials")) {
			Close();

			ReplaceGameObjectMaterials(tag, material);
		}

		if(GUILayout.Button("Restore Materials")) {
			Close();

			RestoreGameObjectMaterials(tag);
		}

		GUILayout.EndHorizontal();
	}

	private void ReplaceGameObjectMaterials(string tag, Material material) {
		Debug.Log("Replacing materials...");

		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);

		for(int gameObjectIndex = 0; gameObjectIndex < gameObjects.Length; gameObjectIndex++) {
			if(EditorUtility.DisplayCancelableProgressBar("Replacing materials...", "", gameObjectIndex / (float) gameObjects.Length)) {
				return;
			}

			GameObject gameObject = gameObjects[gameObjectIndex];

			MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

			if(meshRenderer != null) {
				OriginalMaterials originalMaterials = gameObject.GetComponent<OriginalMaterials>();

				if(originalMaterials == null) {
					originalMaterials = gameObject.AddComponent<OriginalMaterials>();

					originalMaterials.originalMaterials = meshRenderer.sharedMaterials;
				}

				Debug.Log("Replacing materials of \"" + gameObject.name + "\"...");
				
				meshRenderer.sharedMaterials = new Material[] { material };
			}
		}

		EditorUtility.ClearProgressBar();

		Debug.Log("Replaced materials.");
	}

	private void RestoreGameObjectMaterials(string tag) {
		Debug.Log("Restoring materials...");

		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);

		for(int gameObjectIndex = 0; gameObjectIndex < gameObjects.Length; gameObjectIndex++) {
			if(EditorUtility.DisplayCancelableProgressBar("Restoring materials...", "", gameObjectIndex / (float) gameObjects.Length)) {
				return;
			}

			GameObject gameObject = gameObjects[gameObjectIndex];

			MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
			OriginalMaterials originalMaterials = gameObject.GetComponent<OriginalMaterials>();

			if(meshRenderer != null && originalMaterials != null) {
				Debug.Log("Restoring materials of \"" + gameObject.name + "\"...");

				meshRenderer.sharedMaterials = originalMaterials.originalMaterials;

				GameObject.DestroyImmediate(originalMaterials);
			}
		}

		EditorUtility.ClearProgressBar();

		Debug.Log("Restored materials.");
	}
}
