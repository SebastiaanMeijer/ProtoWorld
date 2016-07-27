/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashDestinationsUI.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

public class FlashDestinationsUI : MonoBehaviour
{
    // This is a hack (a quick solution for late hours at night)
    public bool useHackForDriebergen = false;

    RectTransform destCanvas;
    FlashPedestriansDestination[] destinations;
    public GameObject destinationSlider;

    void Awake()
    {
        destCanvas = this.GetComponent<RectTransform>();
        destinations = FindObjectsOfType<FlashPedestriansDestination>();

        // Adapt the size of the panel
        if (useHackForDriebergen)
            destCanvas.sizeDelta = new Vector2(destCanvas.sizeDelta.x, destCanvas.sizeDelta.y + destCanvas.sizeDelta.y * 3);
        else
            destCanvas.sizeDelta = new Vector2(destCanvas.sizeDelta.x, destCanvas.sizeDelta.y + destCanvas.sizeDelta.y * destinations.Length);

        int elemCounter = 0;

        // Create a slider for each destination
        for (int i = 0; i < destinations.Length; i++)
        {
            if (!destinations[i].hideInUI)
            {
                // Instantiate a new slider
                GameObject destinationPrioritySlider = Instantiate(destinationSlider) as GameObject;

                // Get the needed components of the slider object
                RectTransform canvasBox = destinationPrioritySlider.GetComponent<RectTransform>();
                UnityEngine.UI.Text destinationText = destinationPrioritySlider.transform.FindChild("DestinationText").GetComponent<UnityEngine.UI.Text>();
                UnityEngine.UI.Slider slider = destinationPrioritySlider.transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>();
                FlashDestinationSliderController sliderscript = destinationPrioritySlider.transform.FindChild("Slider").GetComponent<FlashDestinationSliderController>();

                // Position the slider according to its parent object
                destinationPrioritySlider.transform.SetParent(this.transform, false);
                canvasBox.localPosition = new Vector3(0, -canvasBox.sizeDelta.y * (elemCounter + 1), 0);

                // Change the destination text
                destinationText.text = destinations[i].destinationName;

                // Add the destination to the slider script
                sliderscript.destination = destinations[i];

                // Update the value of the slider according to the destination
                slider.value = destinations[i].destinationPriority;

                //Set the slider active
                destinationPrioritySlider.SetActive(true);

                elemCounter++;
            }
        }
    }
}
