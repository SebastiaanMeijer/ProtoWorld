/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Camera Feed Module
 * 
 * Furkan Sonmez
 * Berend Wouda
 */

using UnityEngine;

public class FeedCamera : MonoBehaviour {
	public string name;
	public static int numberOfFeedCameras;

    void Start()
    {
		numberOfFeedCameras = numberOfFeedCameras + 1;
		Debug.LogError (numberOfFeedCameras);
    }

    void Update()
    {

    }
}
