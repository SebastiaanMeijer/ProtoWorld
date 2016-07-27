/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * GAPSLABS EXTENDED EDITOR
 * Aram Azhari
 * 
 * Reviewed by Miguel Ramos Carretero
 * Note: This is not used anymore. Look at OSMReaderSQL.cs instead.
 * 
 */

using UnityEngine;
using UnityEditor;

public class TetrahedronEditor : Editor
{
	//[MenuItem("GameObject/Create Other/Tetrahedron")]
	static void Create()
	{
		GameObject gameObject = new GameObject("Tetrahedron");
		Tetrahedron s = gameObject.AddComponent<Tetrahedron>();
		if (gameObject.GetComponent<MeshFilter>() == null)
		{
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = new Mesh();
		}
		else
			gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
		s.Rebuild();
	}
}