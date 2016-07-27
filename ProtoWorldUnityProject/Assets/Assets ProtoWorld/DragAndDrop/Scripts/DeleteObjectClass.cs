/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DRAG AND DROP SYSTEM 
 * Furkan Sonmez
 * 
 */

using UnityEngine;
using System.Collections;

public class DeleteObjectClass : MonoBehaviour{
	//this part is for the deletionsystem of panel that contains the objects list
	//these are the static floats that store the RectTransform values
	public static float xPosition;
	public static float width;
	public static float yPosition;
	public static float height;
	
	public void Start(){
		//the RectTransform values will be stored into the static floats at the start of the game
		RectTransform rt = GetComponent<RectTransform>();
		xPosition = transform.position.x;
		yPosition = transform.position.y;
		width = rt.rect.width;
		height = rt.rect.height;

	}
	
}