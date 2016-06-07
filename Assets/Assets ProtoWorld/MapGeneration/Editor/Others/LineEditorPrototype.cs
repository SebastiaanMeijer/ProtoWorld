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

public class LineEditorPrototype : Editor
{
    //[MenuItem("Gapslabs GIS Package/Line TEST")]
    static void Create()
	{
		GameObject gameObject = new GameObject("Line");

		Polygon p = gameObject.AddComponent<Polygon>();

		if (gameObject.GetComponent<MeshFilter>() == null)
		{
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = new Mesh();
		}
		else
			gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
		//////////////////////////////////////////////////////////////////////////
		// Example vertices
		Vector3[] nodes= new Vector3[]{
			new Vector3(0,0,0),
			new Vector3(1,0,0),
			new Vector3(2,0,1),
			new Vector3(3,0,2)};
		Vector3 AnewVertex=new Vector3();
		
		float distanceFromLine=1f;
		// if y1 == y2 ==> Slope becomes zero, and the normal slope is infinity. Therefore, 
		// The new vertex on the normal is (x1, y1 + distanceFromLine)
		
		if (nodes[0].z==nodes[1].z)
			AnewVertex=new Vector3(nodes[0].x,0,nodes[0].z + distanceFromLine);
		// if x1 == x2 ==> Slope becomes infinity, and the normal slope is zero. Therefore,
		// The new vertex on the normal is (x1 + distanceFromLine, y1)
		else
		if (nodes[0].x==nodes[0].x)
			AnewVertex=new Vector3(nodes[0].x+distanceFromLine,0,nodes[0].z);
		// Calculate slope, offset and the resulting point.
		else		
		{
			float slope=-1f/CalculateSlope(nodes[0],nodes[1]);
			float b=CalculateB(slope,nodes[0]);
			AnewVertex=new Vector3(nodes[0].x+distanceFromLine,0, CalculateY(slope,b,nodes[0].x+distanceFromLine));
		}
		
		Debug.Log(AnewVertex);
		Vector3[] verts = new Vector3[] 
		{ 
			nodes[0],nodes[1],AnewVertex
		};
		int[] Tris = new int[]{
								0,1,2,
								};

		Vector2 uv0 = new Vector2(0, 0);
		Vector2 uv1 = new Vector2(1, 0);
		Vector2 uv2 = new Vector2(0.5f, 1);
		Vector2[] UVs = new Vector2[]{
									uv0,uv1,uv2,
		};
		//////////////////////////////////////////////////////////////////////////

		p.Rebuild(verts, Tris, UVs);
		p.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));
	}
    // Note that we don't invole y component into calculations as our roads are in 2D.
    public static float CalculateSlope(Vector3 p1, Vector3 p2)
    {
        return (p2.z - p1.z) / (p2.x - p1.x);
    }
    // Note that we don't invole y component into calculations as our roads are in 2D.
    public static float CalculateB(float Slope, Vector3 p)
    {
        // y= a*x + b
        // b= y-a*x
        // a= Slope
        return p.z - p.x * Slope;
    }
    public static float CalculateY(float Slope,float b,float X)
	{
			return Slope*X+b;
		}
}