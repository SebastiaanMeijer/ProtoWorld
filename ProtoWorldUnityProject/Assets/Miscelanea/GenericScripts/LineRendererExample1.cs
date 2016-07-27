/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;

public class LineRendererExample1 : MonoBehaviour {
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 6;
    void Start() {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(0.2F, 0.2F);
        
		Vector3[] nodes= new Vector3[]{
			new Vector3(0,0,0),
			new Vector3(1,0,0),
			new Vector3(2,0,1),
			new Vector3(2.5f,0,2),
			new Vector3(3,0,2),
			new Vector3(3,0,3),
			new Vector3(4,0,4),
			new Vector3(5,0,5),
			new Vector3(-2,0,4),
			new Vector3(-3,0,3),
		};
		lengthOfLineRenderer= nodes.Length;
		lineRenderer.SetVertexCount(nodes.Length);
        int i = 0;
        while (i < lengthOfLineRenderer) {
            
            lineRenderer.SetPosition(i, nodes[i%nodes.Length]);
            i++;
        }
    }
    void Update() {
	
    }
}