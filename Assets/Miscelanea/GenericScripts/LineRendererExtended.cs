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