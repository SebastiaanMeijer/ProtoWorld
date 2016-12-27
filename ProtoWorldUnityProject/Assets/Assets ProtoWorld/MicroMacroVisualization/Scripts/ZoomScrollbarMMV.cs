/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Micro/Macro Visualization Module
 * 
 * Furkan Sonmez
 * Berend Wouda
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ZoomScrollbarMMV : MonoBehaviour {
	private Scrollbar scrollbar;
	public GameObject indicatorImage;
	private Text handleText;
	private RectTransform rt;

	private int height;
	private int lastHeight;
	private float maxHeight;
	private float value;

	public int[] stagesHeights;
	public string[] displayText;
	public static int level;

	private bool fading = false;

	private WaitForSeconds waitForShow = new WaitForSeconds(1.0f);
	private WaitForSeconds waitForChange = new WaitForSeconds(0.1f);

	void Awake() {
		scrollbar = this.gameObject.GetComponent<Scrollbar>();
		maxHeight = Camera.main.GetComponent<CameraControl>().maxHeight;
		handleText = GameObject.Find("HandleTextMMV").GetComponent<Text>();
		rt = this.GetComponent(typeof(RectTransform)) as RectTransform;
	}

	void Start() {
		for(int i = 0; i < stagesHeights.Length; i++) {
			GameObject instantiatedIndicator = Instantiate(indicatorImage, new Vector3(this.transform.position.x, (stagesHeights[i] / maxHeight) * rt.sizeDelta.y + transform.position.y - rt.sizeDelta.y / 2, this.transform.position.z), Quaternion.identity) as GameObject;
			instantiatedIndicator.transform.parent = this.transform;
		}
		StartCoroutine(changeTag());
	}

	public void sliderChanged() {
		value = scrollbar.value;
		CameraControl camera = Camera.main.GetComponent<CameraControl>();
		camera.targetCameraPosition = new Vector3(camera.targetCameraPosition.x, value * maxHeight, camera.targetCameraPosition.z);
	}

	public void zoomedInOut(float heightOfCamera) {
		height = (int) heightOfCamera;
		scrollbar.value = height / maxHeight;

		if(fading == false && lastHeight != height) {
			for(int i = 0; i < stagesHeights.Length; i++) {
				if(value * maxHeight > stagesHeights[i]) {
					level = i;

					handleText.text = "" + stagesHeights[i];
				}
			}

			fading = true;
			handleText.color = new Vector4(handleText.color.r, handleText.color.g, handleText.color.b, 1);

			StartCoroutine(showTag());
			//StartCoroutine (changeTag (0.1f));
		}
	}

	public IEnumerator showTag() {
		yield return waitForShow;
		handleText.color = new Vector4(handleText.color.r, handleText.color.g, handleText.color.b, 0);
		fading = false;
		lastHeight = height;
		yield return null;
	}

	public IEnumerator changeTag() {
		yield return waitForChange;
		if(lastHeight != height) {
			for(int i = 0; i < stagesHeights.Length; i++) {
				if(value * maxHeight > stagesHeights[i]) {
					level = i;

					handleText.text = "" + displayText[i];

				}
			}
			lastHeight = height;
		}
		StartCoroutine(changeTag());
	}
}