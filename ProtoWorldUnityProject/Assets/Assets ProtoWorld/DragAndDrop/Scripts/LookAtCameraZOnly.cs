/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class LookAtCameraZOnly : MonoBehaviour {
	
	public Camera cameraToLookAt;
	public float x2;
	public float y2;
	public float z2;
	public float x1;
	public float y1;
	public float z1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = cameraToLookAt.transform.position - transform.position;

		v.x = 90; 
		//v.y = y2; 
		v.z = 0;

		transform.LookAt( cameraToLookAt.transform.position); 
		transform.Rotate(x1,y1,z1);
	}
}
