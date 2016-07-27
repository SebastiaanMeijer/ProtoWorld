/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class LookAtCameraYonly : MonoBehaviour {

	public Camera cameraToLookAt;

	public bool useThisMethod = true;


	// Use this for initialization
	void Start () {
		//Unless you set another camera to look at, it will look at the main camera
		if(cameraToLookAt == null){
			cameraToLookAt = Camera.main;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(useThisMethod){

		//This code will make the object face the camera position
		Vector3 v = cameraToLookAt.transform.position - transform.position;
		v.x = v.z = 0.0f;
		transform.LookAt( cameraToLookAt.transform.position - v ); 
		transform.Rotate(0,180,0);

		if(Camera.main.orthographic == true){
			transform.Rotate(0,-9999999,0);
		}
		}
	}
}
