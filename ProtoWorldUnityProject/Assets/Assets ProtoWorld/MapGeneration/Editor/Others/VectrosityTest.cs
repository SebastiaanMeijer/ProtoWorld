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

