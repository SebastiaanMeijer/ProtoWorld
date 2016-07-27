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

public class RoadCreatorEditor : Editor
{
	//[MenuItem("Gapslabs GIS Package/Create Road From OpenStreetMap")]
	static void Create()
	{
		string filename = EditorUtility.OpenFilePanel("Select the Open Street Map (OSM) file", "", "osm");
		EditorUtility.DisplayDialog("The file", "The file was " + filename, "OK");
		GameObject gameObject = new GameObject("Tetrahedron");

		Polygon p = gameObject.AddComponent<Polygon>();

		if (gameObject.GetComponent<MeshFilter>() == null)
		{
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = new Mesh();
		}
		else
			gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
		//////////////////////////////////////////////////////////////////////////
		// Example vertices
		Vector3 p0 = new Vector3(0, 0, 0);
		Vector3 p1 = new Vector3(1, 0, 0);
		Vector3 p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
		Vector3 p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);
		Vector3[] verts = new Vector3[] 
		{ 
			p0, p1, p2, 
			p0,p2,p3,
			p2,p1,p3,
			p0,p3,p1
		};
		int[] Tris = new int[]{
																0,1,2,
																3,4,5,
																6,7,8,
																9,10,11
																};

		Vector2 uv0 = new Vector2(0, 0);
		Vector2 uv1 = new Vector2(1, 0);
		Vector2 uv2 = new Vector2(0.5f, 1);
		Vector2[] UVs = new Vector2[]{
																uv0,uv1,uv2,
																uv0,uv1,uv2,
																uv0,uv1,uv2,
																uv0,uv1,uv2
		};
		//////////////////////////////////////////////////////////////////////////

		p.Rebuild(verts, Tris, UVs);
		p.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));
	}
}