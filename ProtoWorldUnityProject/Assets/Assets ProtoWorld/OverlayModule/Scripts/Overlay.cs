/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Overlay Module
 * 
 * Berend Wouda
 */
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour {
	public GameObject[] gameObjects;

	public bool active;

	public string enableTitle;
	public string disableTitle;

	public Text text;


	public void Start() {
		updateButton();
		updateOverlay();
	}


	public void Toggle() {
		active = !active;

		updateButton();
		updateOverlay();
	}


	private void updateButton() {
		if(active) {
			text.text = disableTitle;
		}
		else {
			text.text = enableTitle;
		}
	}
	
	private void updateOverlay() {
		foreach(GameObject gameObject in gameObjects) {
			if(gameObject != null) {
				gameObject.SetActive(active);
			}
		}
	}
}
