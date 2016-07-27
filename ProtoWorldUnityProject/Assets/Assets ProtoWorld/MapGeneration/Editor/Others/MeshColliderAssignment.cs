/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;
using UnityEditor;

public class MeshColliderAssignment : Editor {

	//[MenuItem("Gapslabs GIS Package/Assign colliders to buildings")]
	static void AssignColliders()
	{
		var go = GameObject.FindGameObjectsWithTag("Building");
		int count = 1;
		foreach (var g in go)
		{
			if (!EditorUtility.DisplayCancelableProgressBar("Assigning", count + " of " + go.Length, count++ / (float)go.Length))
				g.GetComponent<MeshCollider>().sharedMesh = g.GetComponent<MeshFilter>().sharedMesh;
			else
				break;
		}
		EditorUtility.ClearProgressBar();
	}
}
