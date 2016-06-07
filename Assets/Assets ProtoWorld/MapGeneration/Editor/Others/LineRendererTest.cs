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

