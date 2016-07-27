/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class ActivateRotating : MonoBehaviour {

	public Transform rotationCanvas;
	public Transform TheObject;

	// Use this for initialization
	void Start () {
	
	}

	void OnMouseDown()
	{
		//if(TheObject.DragTransform.clicked == true)
		//if(rotationCanvas.gameObject.activeSelf == true){
		//	Debug.LogError ("I should now deactivate the object");
		//	rotationCanvas.gameObject.SetActive (false);
		//}
		//else if(rotationCanvas.gameObject.activeSelf == false){
		//	Debug.LogError ("I should now activate the object");
		//	rotationCanvas.gameObject.SetActive (true);
		//}
	}

	void OnMouseUp()
	{

	}

	// Update is called once per frame
	void Update () {
	
	}
}
