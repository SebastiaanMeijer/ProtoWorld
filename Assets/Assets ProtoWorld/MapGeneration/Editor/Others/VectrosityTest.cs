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

public class VectrosityTest : Editor
{
	public static Color c1 = Color.yellow;
	public static Color c2 = Color.red;
	public static int lengthOfLineRenderer = 20;
	
	//[MenuItem("Gapslabs GIS Package/Vector Test")]
	static void Create()
	{
		Vector3[] nodes=new Vector3[lengthOfLineRenderer];
		
		//GameObject gameObject = new GameObject("RenderedLine");
		int i = 0;
		while (i < lengthOfLineRenderer)
		{
			nodes[i] = new Vector3(i * 0.2F, Mathf.Sin(i + Time.time),0);
			i++;
		}
		Material lineMaterial = new Material(Shader.Find("Unlit"));
		var myLine = new VectorLine("MyLine", nodes, lineMaterial, 10.0f,LineType.Continuous,Joins.Fill);
		
		myLine.mesh.RecalculateNormals();
		Vector.DrawLine3D(myLine);
	}
}

