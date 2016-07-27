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
 * VVisFixPointInfo.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Stores and handles the information related to a certain VVIS fix point. 
/// </summary>
public class VVisFixPointInfo : MonoBehaviour 
{
    public List<string> pointInfo = new List<string>();
    
    /// <summary>
    /// Store the information of a certain point within the script. 
    /// </summary>
    /// <param name="info">String list with the information of the point.</param>
    public void StorePointInfo(List<string> info)
    {
        pointInfo.Clear();
        pointInfo.AddRange(info);
    }

    /// <summary>
    /// Fill the canvas with the information of the point store in this script. 
    /// </summary>
    public void FillCanvasWithPointInfo(RectTransform canvas)
    {
        Text text = canvas.GetComponentInChildren<Text>();

        if (text != null)
        {
            text.text = "";

            foreach (var S in pointInfo)
            {
                text.text += S + " "; 
            }
        }
    }
}
