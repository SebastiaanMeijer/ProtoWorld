/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿//Furkan Sonmez

using UnityEngine;
using System.Collections;

public class ClickMinimalize : MonoBehaviour {

	//these are the transforms that will be activated and deactivated when clicking on the title area
	public Transform DragAndDropList;
	public Transform ScrollBarDragAndDropList;

	//these are the static values of the RectTransform
	static public float sizeWidth;
	static public float sizeHeight;
	static public float xPosition;
	static public float yPosition;

	void Start () {
		//here the values are stored into the static floats
		RectTransform rt = GetComponent<RectTransform>();
		sizeWidth = rt.rect.width;
		sizeHeight = rt.rect.height;
		xPosition = rt.rect.x;
		yPosition = rt.rect.y;

	}

	public void ActivateDragAndDrop()
	{
		//when this function is called and the gameObject was true, now make it false and vice versa
		if(DragAndDropList.gameObject.activeSelf == true){
			DragAndDropList.gameObject.SetActive (false);
			ScrollBarDragAndDropList.gameObject.SetActive (false);

		}
		else if(DragAndDropList.gameObject.activeSelf == false){
			DragAndDropList.gameObject.SetActive (true);
			ScrollBarDragAndDropList.gameObject.SetActive (true);
		}
	}
	
}
