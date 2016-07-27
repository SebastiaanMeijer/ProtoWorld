/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {
	
	private Transform mainCamera;
	private float normRot;
	public Vector3 OffsetFromCamera=new Vector3(0.5f,0.34f,2f);
	// Use this for initialization
	void Start () {
		mainCamera=GameObject.FindWithTag("MainCamera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		normRot = Mathf.Abs(mainCamera.eulerAngles.y);
		if (normRot > 360) 
		{
			normRot = normRot % 360;
		}
		var temp=transform.eulerAngles;
		temp.y= normRot - 180; // because we look at the "back"
		transform.eulerAngles=temp;
		transform.rotation=mainCamera.rotation;
		transform.position= mainCamera.position+OffsetFromCamera;
	}
}
