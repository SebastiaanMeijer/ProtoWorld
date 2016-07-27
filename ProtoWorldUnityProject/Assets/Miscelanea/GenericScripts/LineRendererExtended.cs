/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererExtended : MonoBehaviour
{
	public Color c1 = Color.yellow;
	public Color c2 = Color.red;
	public float WidthStart = 0.5f;
	public float WidthEnd = 0.5f;
	public Vector3[] Points;
	public Vector3 DebugScale = Vector3.one;
	[HideInInspector]
	public LineRenderer line;
	public bool Show = true;
	void Start()
	{
		line = GetComponent<LineRenderer>();
		line.material = new Material(Shader.Find("Particles/Additive"));
		line.SetColors(c1, c2);
		line.SetWidth(WidthStart, WidthEnd);
		line.SetVertexCount(Points.Length);

	}
	void Update()
	{
		if (Show)
		{
			for (int i = 0; i < Points.Length; i++)
			{
				line.SetPosition(i, Vector3.Scale(Points[i], DebugScale));
			}
            //for (int i = 0; i < Points.Length - 1; i++)
            //{
            //    Debug.DrawLine(Vector3.Scale(Points[i], DebugScale), Vector3.Scale(Points[i + 1], DebugScale), Color.blue);
            //    Debug.DrawLine(Points[i], new Vector3(Points[i].x, 3, Points[i].z), Color.green);
            //}
		}
	}

    

}