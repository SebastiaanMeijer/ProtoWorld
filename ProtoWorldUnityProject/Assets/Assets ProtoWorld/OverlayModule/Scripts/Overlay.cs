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
