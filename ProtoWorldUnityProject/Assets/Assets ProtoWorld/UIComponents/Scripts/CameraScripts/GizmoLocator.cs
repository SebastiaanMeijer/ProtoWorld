/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;

/// <summary>
/// A gizmo that highlights the object with a sphere and a vertical line.
/// </summary>
public class GizmoLocator : MonoBehaviour {

    /// <summary>
    /// Object to show gizmo on
    /// </summary>
	public Transform objectToGizmo;
    /// <summary>
    /// The size of the gizmo sphere
    /// </summary>
	public float GizmoSize=1f;
    /// <summary>
    /// The height of the vertical lines
    /// </summary>
	public float HeightOfLine=5;
	// Use this for initialization
	/// <summary>
	/// The gizmo logic
	/// </summary>
	void OnDrawGizmos()
	{
		Gizmos.color=Color.red;
		Gizmos.DrawSphere( transform.TransformPoint( objectToGizmo.localPosition),GizmoSize);
		Gizmos.color=Color.yellow;
		Gizmos.DrawLine(
		                transform.TransformPoint( objectToGizmo.localPosition), 
		                transform.TransformPoint( objectToGizmo.localPosition+new Vector3(0,HeightOfLine,0))
		               );
	}
	
}
