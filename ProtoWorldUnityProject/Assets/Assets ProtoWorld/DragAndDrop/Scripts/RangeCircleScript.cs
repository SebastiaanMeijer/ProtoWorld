/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * BIKES FOR DRAG AND DROP SYSTEM 
 * RangeCircleScript.cs
 * Furkan Sonmez
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class draws the circle around bike stations to showcast the radius of influence. 
/// </summary>
public class RangeCircleScript : MonoBehaviour 
{
    [HideInInspector]
	public Transform circleCanvas;

    [HideInInspector]
	public static bool circleBool;

    /// <summary>
    /// Use this for initialization
    /// </summary>
	void Start () 
    {
        circleCanvas = GetComponentInChildren<Transform>().FindChild("CircleCanvas");
		
        //make the circleBool true whenever a new bikestation is added
		if(rayHitPositionClass.gameStartedBool == true)
			circleBool = true;
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () 
    {
		//if the circleBool is true set the circleCanvas to true
		if(circleBool == true)
			circleCanvas.gameObject.SetActive (true);
		else
			circleCanvas.gameObject.SetActive (false);
	}

	/// <summary>
	/// Whenever a bikeStation is selected, this function is called
	/// </summary>
	public void SelectedCircle()
    {
		circleBool = true;
	}

	/// <summary>
	/// Whenever a bikeStation is deselected, this function is called
	/// </summary>
	public void DeselectedCircle()
    {
		circleBool = false;
	}
}
