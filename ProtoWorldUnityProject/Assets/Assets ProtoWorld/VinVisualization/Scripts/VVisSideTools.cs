/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * VIN VISUALIZATION
 * VVisSideTools.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Side tools to facilitate the interaction with the VVIS components from external modules. 
/// </summary>
public class VVisSideTools : MonoBehaviour
{
    private static Toggle drawingToggle;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        if (drawingToggle == null)
        {
            GameObject obj = GameObject.Find("DrawingToggle");

            if (obj != null)
                drawingToggle = obj.GetComponent<Toggle>();
        }
    }

    /// <summary>
    /// Unchecks the drawing toggle.
    /// </summary>
    public void UncheckDrawingToggle()
    {
        if (drawingToggle != null)
            drawingToggle.isOn = false;
    }
}
