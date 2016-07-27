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

public class CameraChangeScript : MonoBehaviour {
	
	public GameObject Camera1;
	public GameObject Camera2;
	
	
	public Vector3 perspectivePosition;
	public Vector3 perspectiveRotation;
	
	
	public Vector3 orthographicPosition;

    private float perspectiveSpeed;
    private float orthographicSpeed = 100;
	
	// Use this for initialization
	void Start () {
		perspectiveRotation = this.transform.rotation.eulerAngles;
		orthographicPosition = Camera2.transform.position;
		perspectivePosition = Camera1.transform.position;
        perspectiveSpeed = Camera.main.GetComponent<CameraControl>().moveSpeed;
	}

	/// <summary>
	/// Whenever the camerachangeButton or the B button is pressed, this function gets called
	/// </summary>
	public void changeCameraFunction(){
		//If orthographic it becomes perspective and vice versa
		if(this.GetComponent<Camera>().orthographic == false){
			perspectiveRotation = this.transform.rotation.eulerAngles;
			perspectivePosition = this.transform.position;
			
			Camera.main.GetComponent<CameraControl>().targetCameraPosition = orthographicPosition;
            Camera.main.GetComponent<CameraControl>().moveSpeed = orthographicSpeed;
			this.transform.rotation = Camera2.transform.rotation;
			this.GetComponent<Camera>().orthographic = true;
		}
		else if(this.GetComponent<Camera>().orthographic == true){
			
			this.transform.eulerAngles = perspectiveRotation;
			orthographicPosition = this.transform.position;
			
			
			Camera.main.GetComponent<CameraControl>().targetCameraPosition = perspectivePosition;
            Camera.main.GetComponent<CameraControl>().moveSpeed = perspectiveSpeed;
			//this.transform.rotation = Camera2.transform.rotation;
			this.GetComponent<Camera>().orthographic = false;
			//perspectivePosition = this.transform.position;
		}
	}
	
	
	
	// Update is called once per frame
	public void Update() {
		
		
		if (Input.GetKeyDown(KeyCode.B)) {
			if(this.GetComponent<Camera>().orthographic == false){
				perspectiveRotation = this.transform.rotation.eulerAngles;
				perspectivePosition = this.transform.position;
				
				Camera.main.GetComponent<CameraControl>().targetCameraPosition = orthographicPosition;
				this.transform.rotation = Camera2.transform.rotation;
				this.GetComponent<Camera>().orthographic = true;
			}
			else if(this.GetComponent<Camera>().orthographic == true){
				
				this.transform.eulerAngles = perspectiveRotation;
				orthographicPosition = this.transform.position;
				
				
				Camera.main.GetComponent<CameraControl>().targetCameraPosition = perspectivePosition;
				//this.transform.rotation = Camera2.transform.rotation;
				this.GetComponent<Camera>().orthographic = false;
				//perspectivePosition = this.transform.position;
			}
		}
		
	}
	
	
	
	
}