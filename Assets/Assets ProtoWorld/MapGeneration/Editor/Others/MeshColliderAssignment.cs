using UnityEngine;
using System.Collections;
using UnityEditor;

public class MeshColliderAssignment : Editor {

	//[MenuItem("Gapslabs GIS Package/Assign colliders to buildings")]
	static void AssignColliders()
	{
		var go = GameObject.FindGameObjectsWithTag("Building");
		int count = 1;
		foreach (var g in go)
		{
			if (!EditorUtility.DisplayCancelableProgressBar("Assigning", count + " of " + go.Length, count++ / (float)go.Length))
				g.GetComponent<MeshCollider>().sharedMesh = g.GetComponent<MeshFilter>().sharedMesh;
			else
				break;
		}
		EditorUtility.ClearProgressBar();
	}
}
