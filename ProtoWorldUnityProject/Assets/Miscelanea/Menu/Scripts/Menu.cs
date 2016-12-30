/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Menu
 * 
 * Berend Wouda
 */
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public string title;
	public string version;

	[Serializable]
	public struct MenuItem {
		public Texture image;
		public string text;
		public string path;
	}

	public MenuItem[] menuItems;

	public Text titleText;
	public GameObject slotParent;

	public GameObject slot;
	public GameObject missingImageOverlay;

	public TransitionController transitionController;


	public void Start() {
		titleText.text = title + " " + version;

		float x = 0.0f;
		float y = 0.0f;
		float z = 0.0f;

		for(int index = 0; index < menuItems.Length; index++) {
			MenuItem menuItem = menuItems[index];

			GameObject slotInstance = Instantiate(slot, slotParent.transform) as GameObject;

			slotInstance.name = "Slot (" + menuItem.text + ")";

			if(index % 2 == 0) {
				x = -153.0f;

				if(y < 0.0f) {
					y -= 208.0f;
				}
				else {
					y -= 8.0f;
				}
			}
			else {
				x = 153.0f;
			}

			slotInstance.GetComponent<RectTransform>().localPosition = new Vector3(x, y, z);

			slotInstance.GetComponentInChildren<RawImage>().texture = menuItem.image;
			slotInstance.GetComponentInChildren<Text>().text = menuItem.text;

			if(menuItem.image == null) {
				GameObject missingImageOverlayInstance = Instantiate(missingImageOverlay, slotInstance.transform) as GameObject;

				missingImageOverlayInstance.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			}

			slotInstance.GetComponentInChildren<Button>().onClick.AddListener(() => loadScene(menuItem));
		}

		float width = 0.0f;
		float height = -y + 208.0f;

		// Work around the issue where the Unity editor likes to move the content pane while you're not looking.
		slotParent.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		slotParent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
	}


	public void loadScene(MenuItem menuItem) {
		if(transitionController != null) {
			transitionController.transitionOut(() => load(menuItem.path));
		}
		else {
			load(menuItem.path);
		}
	}

	public void exitScene() {
		if(transitionController != null) {
			transitionController.transitionOut(exit);
		}
		else {
			exit();
		}
	}


	private void load(string path) {
		SceneManager.LoadSceneAsync(path, LoadSceneMode.Single);
	}

	private void exit() {
		Application.Quit();
	}
}
