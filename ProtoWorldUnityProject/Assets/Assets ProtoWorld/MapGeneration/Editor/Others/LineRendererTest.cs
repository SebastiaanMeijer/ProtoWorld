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

public class LineRendererTest : Editor
{
	public static Color c1 = Color.yellow;
	public static Color c2 = Color.red;
	public static int lengthOfLineRenderer = 20;

	//[MenuItem("Gapslabs GIS Package/Line Renderer Test")]
	static void Create()
	{
		GameObject gameObject = new GameObject("RenderedLine");
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(c1, c2);
		lineRenderer.SetWidth(0.2F, 0.2F);
		lineRenderer.SetVertexCount(lengthOfLineRenderer);
		lineRenderer.useWorldSpace=false;
		int i = 0;
		while (i < lengthOfLineRenderer)
		{
			Vector3 pos = new Vector3(i * 0.5F, Mathf.Sin(i + Time.time), 0);
			lineRenderer.SetPosition(i, pos);
			i++;
		}
	}
}

