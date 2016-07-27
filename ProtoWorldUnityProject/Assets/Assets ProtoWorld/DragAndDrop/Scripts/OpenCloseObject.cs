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

public class OpenCloseObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenEvent(){


		if(OCPButtonsFunctions.OCPButton1Text.text == "Open"){

		Debug.Log ("The object is now open");
		this.GetComponent<ObjectData>().OCPButton1TextString = "Close";
			OCPButtonsFunctions.OCPButton1Text.text = "Close";

		}
		else if(OCPButtonsFunctions.OCPButton1Text.text == "Close"){

		Debug.Log ("The object is now open");
		this.GetComponent<ObjectData>().OCPButton1TextString = "Open";
			OCPButtonsFunctions.OCPButton1Text.text = "Open";

		}


	}

	public void JumpEvent(){
	


	}




}
