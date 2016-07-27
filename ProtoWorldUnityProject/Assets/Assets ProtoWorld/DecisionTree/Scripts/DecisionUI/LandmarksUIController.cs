/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DECISION TREE MODULE
 * LandmarksUIController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

public class LandmarksUIController : MonoBehaviour 
{
    public float zoomFactor = 0.5f;
    private LandmarkController[] landmarks;
    private RectTransform destCanvas;
    public GameObject templateButton;

	void Awake () 
    {
        destCanvas = this.GetComponent<RectTransform>();
        landmarks = FindObjectsOfType<LandmarkController>();
        
        // Adapt the size of the panel
        destCanvas.sizeDelta = new Vector2(destCanvas.sizeDelta.x, destCanvas.sizeDelta.y + destCanvas.sizeDelta.y * landmarks.Length + 10f);

        // Create a button for each landmark
        for (int i = 0; i < landmarks.Length; i++)
        {
            // Instantiate a new button
            GameObject landmarkButton = Instantiate(templateButton) as GameObject;

            // Get the needed components of the button object
            RectTransform canvasBox = landmarkButton.GetComponent<RectTransform>();
            UnityEngine.UI.Button button = landmarkButton.transform.FindChild("Button").GetComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.Text buttonText = button.transform.FindChild("Text").GetComponent<UnityEngine.UI.Text>();
            LandmarkButtonController buttonController = landmarkButton.GetComponent<LandmarkButtonController>();

            // Position the button according to its parent object
            landmarkButton.transform.SetParent(this.transform, false);
            canvasBox.localPosition = new Vector3(0, -canvasBox.sizeDelta.y * (i + 1), 0);

            // Change the destination text
            buttonText.text = landmarks[i].landmarkTag;

            // Set the gameobject reference and the zoomfactor
            buttonController.landmarkObject = landmarks[i].gameObject;
            buttonController.zoomFactor = zoomFactor;

            //Set the slider active
            landmarkButton.SetActive(true);
        }
	}
}
