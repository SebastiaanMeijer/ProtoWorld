/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;

/// <summary>
/// Quit key class, simply quits the current scene when the user presses escape button, or on Android when the user presses the back button.
/// </summary>
public class QuitKey : MonoBehaviour {

	/// <summary>
	/// Specifies whether the program should return to the level at index 0 or just quit completely.
	/// </summary>
	public static bool BackToMain=true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (BackToMain)
				Application.LoadLevel(0);
			else
				Application.Quit();
		}
	}
}
