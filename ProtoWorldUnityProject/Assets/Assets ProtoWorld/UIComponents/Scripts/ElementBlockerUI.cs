/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * ELEMENT UI
 * ElementBlockerUI.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Script to control the behaviour of blocking elements in the UI.
/// </summary>
[RequireComponent(typeof(FadingElementUI))]
public class ElementBlockerUI : MonoBehaviour
{
    public string key = "123";
    public UnityEngine.UI.InputField keyTextField;

    public bool showBlockingFromStart = false;

    public UnityEvent m_OnUnblock;

    private FadingElementUI fading;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        fading = this.GetComponent<FadingElementUI>();

        if (showBlockingFromStart)
            fading.fadeInCanvas();
    }

    /// <summary>
    /// Handler for continue button in the blocking elements. 
    /// </summary>
    public void Handler_ContinueButton()
    {
        if (keyTextField != null)
        {
            if (keyTextField.text.Equals(key))
            {
                keyTextField.text = "correct!";
                DeactivateBlocking();
            }
            else
            {
                keyTextField.text = "wrong key!";
            }
        }
    }

    /// <summary>
    /// Handler for cancel button in the blocking elements. 
    /// </summary>
    public void Handler_CancelButton()
    {
        fading.fadeOutCanvas();
    }

    /// <summary>
    /// Removes the blocking in the UI canvas.
    /// </summary>
    public void DeactivateBlocking()
    {
        fading.fadeOutCanvas();

        m_OnUnblock.Invoke();
    }
}
