/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Camera Feed Module
 * 
 * Furkan Sonmez
 * Berend Wouda
 */

using UnityEngine;
using UnityEngine.UI;

public class ChangeCameraFeed : MonoBehaviour {
	public RenderTexture cameraRender1;
	public RenderTexture cameraRender2;
	public RenderTexture cameraRender3;

	private GameObject feedCamerasObject;

	private Text cameraFeed1Text;
	private Text cameraFeed2Text;
	private Text cameraFeed3Text;

	private int i;

	void Awake() {
		cameraFeed1Text = GameObject.Find("CameraFeed1Text").GetComponent<Text>();
		cameraFeed2Text = GameObject.Find("CameraFeed2Text").GetComponent<Text>();
		cameraFeed3Text = GameObject.Find("CameraFeed3Text").GetComponent<Text>();
		feedCamerasObject = GameObject.Find("FeedCameras");
	}

	void Start() {
		i = 0;

		setFeedCamerasEnabled(i);
	}


	public void nextCameraFeed() {
		if(i + 3 < getFeedCameraCount()) {
			i += 1;

			setFeedCamerasEnabled(i);
		}
	}

	public void previousCameraFeed() {
		if(i > 0) {
			i = i - 1;

			setFeedCamerasEnabled(i);
		}
	}


	private int getFeedCameraCount() {
		int cameraCount = 0;

		for(int index = 0; index < feedCamerasObject.transform.childCount; index++) {
			Camera feedCamera = feedCamerasObject.transform.GetChild(index).gameObject.GetComponent<Camera>();

			if(feedCamera != null) {
				cameraCount += 1;
			}
		}

		return cameraCount;
	}

	private void setFeedCamerasEnabled(int startIndex) {
		// Enable the first three feed cameras (from the starting position) and disable the rest.
		int cameraIndex = -startIndex;

		for(int index = 0; index < feedCamerasObject.transform.childCount; index++) {
			Camera feedCamera = feedCamerasObject.transform.GetChild(index).gameObject.GetComponent<Camera>();

			if(feedCamera != null) {
				if(cameraIndex == 0) {
					feedCamera.enabled = true;
					feedCamera.targetTexture = cameraRender1;
					cameraFeed1Text.text = feedCamera.gameObject.GetComponent<FeedCamera>().name;
				}
				else if(cameraIndex == 1) {
					feedCamera.enabled = true;
					feedCamera.targetTexture = cameraRender2;
					cameraFeed2Text.text = feedCamera.gameObject.GetComponent<FeedCamera>().name;
				}
				else if(cameraIndex == 2) {
					feedCamera.enabled = true;
					feedCamera.targetTexture = cameraRender3;
					cameraFeed3Text.text = feedCamera.gameObject.GetComponent<FeedCamera>().name;
				}
				else {
					feedCamera.enabled = false;
					feedCamera.targetTexture = null;
				}

				cameraIndex += 1;
			}
		}
	}
}


