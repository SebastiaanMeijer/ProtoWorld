/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class DoSomething : MonoBehaviour {

	// Use this for initialization
	public void  myFirstEvent() {
		Debug.Log("THIS IS MY FIRST EVENT!!");
	}

	public void  mySelected() {
		Debug.Log("Selected");
	}

	public void  myDeselected() {
		Debug.Log("Deselected");
	}

	public void  myMoved() {
		Debug.Log("Moved");
	}

	public void  myDropped() {
		Debug.Log("Dropped");
	}
	public void  myRotated() {
		Debug.Log("Rotated");
	}


	// Update is called once per frame
	void Update () {
	
	}
}
