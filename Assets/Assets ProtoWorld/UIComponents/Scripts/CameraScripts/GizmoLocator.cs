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
