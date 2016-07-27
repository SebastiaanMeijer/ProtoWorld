/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * UI COMPONENTS
 * PopUpInfoOnHover.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Pops up a given canvas when the mouse is over the holder gameobject. 
/// Note: The pop up canvas must have the script FadingElementUI attached.
/// </summary>
public class PopUpInfoOnHover : MonoBehaviour
{
    public RectTransform popUpCanvas;
    public UnityEvent eventOnMouseOver;
    public UnityEvent eventOnMouseExit;

    /// <summary>
    /// Show the popUpObject with the pivot centered in the mouse position.
    /// </summary>
    void OnMouseOver()
    {
        if (popUpCanvas != null)
        {
            var fading = popUpCanvas.GetComponent<FadingElementUI>();

            if (fading != null)
            {
                popUpCanvas.position = new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, Input.mousePosition.z);

                fading.fadeInCanvas();

                eventOnMouseOver.Invoke();
            }
        }
    }

    /// <summary>
    /// Hides the popUpObject.
    /// </summary>
    void OnMouseExit()
    {
        if (popUpCanvas != null)
        {
            var fading = popUpCanvas.GetComponent<FadingElementUI>();

            if (fading != null)
                fading.fadeOutCanvas();

            eventOnMouseExit.Invoke();
        }
    }
}
