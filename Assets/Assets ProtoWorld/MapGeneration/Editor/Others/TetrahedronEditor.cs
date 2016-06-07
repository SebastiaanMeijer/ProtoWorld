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