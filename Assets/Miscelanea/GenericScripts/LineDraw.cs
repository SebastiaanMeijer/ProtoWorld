using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LineDraw : ScriptableObject
{
    public enum OSMType { Node, Line, Polygon, Relation };

    public void Draw(Vector3[] Points, Color StartingColor, Color EndingColor, float WidthStart, float WidthEnd, OSMType type, string Id, string other, string tag)
    {
        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        gameObject.tag = tag;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Particles/Zwrite")); //Particles/Additive
        lineRenderer.SetColors(StartingColor, EndingColor);
        lineRenderer.SetWidth(WidthStart, WidthEnd);
        lineRenderer.SetVertexCount(Points.Length);
        lineRenderer.useWorldSpace = false;
        for (int i = 0; i < Points.Length; i++)
            lineRenderer.SetPosition(i, Points[i]);
    }
    public void Draw(Vector3[] Points, Color StartingColor, Color EndingColor, float WidthStart, float WidthEnd, OSMType type, string Id, string other, string tag, string MaterialName)
    {

        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        gameObject.tag = tag;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = Resources.Load(MaterialName, typeof(Material)) as Material;
        lineRenderer.sharedMaterial.mainTextureScale = new Vector2(Points.Length, 1);
        lineRenderer.SetColors(StartingColor, EndingColor);
        lineRenderer.SetWidth(WidthStart, WidthEnd);
        lineRenderer.SetVertexCount(Points.Length);
        lineRenderer.useWorldSpace = false;
        for (int i = 0; i < Points.Length; i++)
            lineRenderer.SetPosition(i, Points[i]);
    }
    public void DrawUnscaled(Vector3[] Points, Color StartingColor, Color EndingColor, float WidthStart, float WidthEnd, OSMType type, string Id, string other, string tag, string MaterialName)
    {

        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        gameObject.tag = tag;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = Resources.Load(MaterialName, typeof(Material)) as Material;
        lineRenderer.sharedMaterial.mainTextureScale = new Vector2(Points.Length, 1);
        lineRenderer.SetColors(StartingColor, EndingColor);
        lineRenderer.SetWidth(WidthStart, WidthEnd);
        lineRenderer.SetVertexCount(Points.Length);
        lineRenderer.useWorldSpace = false;
        for (int i = 0; i < Points.Length; i++)
            lineRenderer.SetPosition(i, Points[i]);
    }
    public void Draw(Vector2[] Points, Color StartingColor, Color EndingColor, float WidthStart, float WidthEnd, OSMType type, string Id, string other, string tag)
    {
        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        gameObject.tag = tag;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Zwrite")); //Particles/Additive
        lineRenderer.SetColors(StartingColor, EndingColor);
        lineRenderer.SetWidth(WidthStart, WidthEnd);
        lineRenderer.SetVertexCount(Points.Length);
        lineRenderer.useWorldSpace = false;
        for (int i = 0; i < Points.Length; i++)
            lineRenderer.SetPosition(i, new Vector3(Points[i].x, 0, Points[i].y));
    }
    public void TestMeshGenerationSimple(Vector3[] Points, float Width, float Smoothness, OSMType type, string Id, string other, string tag, string MaterialName, float height = 0f)
    {
        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        gameObject.tag = tag;
        Polygon p = gameObject.AddComponent<Polygon>();
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
        }
        else
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh();


        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        Vector2 uvTopLeft = new Vector2(1, 0);// new Vector2(0,0);
        Vector2 uvTopRight = new Vector2(0, 1);//new Vector2(1,0);
        Vector2 uvBottomLeft = new Vector2(1, 1);//new Vector2(0,1);
        Vector2 uvBottomRight = new Vector2(0, 0);//new Vector2(1,1);

        int offset = 1;
        if (Points.Length > 2)
            offset = 2;
        Vector3 AverageScale = new Vector3(0.5f, 1, 0.5f);

        Vector3[][] AllQuads = new Vector3[Points.Length - 1][];
        for (int i = 0; i < Points.Length - 1; i++)
        {
            AllQuads[i] = Interpolations.MakeQuad(Points[i], Points[i + 1], Width, gameObject.transform);
        }
        for (int i = 0; i < Points.Length - offset; i++)
        {
            // Start 0 ,Start-L 1 ,End 2 , End-L 3
            if (Points.Length == 2)
            {
                vertices.AddRange(
                    new Vector3[] {
				                AllQuads[i][0],AllQuads[i][2],AllQuads[i][1],
			                  	AllQuads[i][1],AllQuads[i][2],AllQuads[i][3],
			                  	// Reverse
			                  	AllQuads[i][0],AllQuads[i][1],AllQuads[i][2],
			                  	AllQuads[i][1],AllQuads[i][3],AllQuads[i][2]
			    });
            }
            else
            {
                var AvgESIdx = Vector3.Scale(AllQuads[i][2] + AllQuads[i + 1][0], AverageScale);
                var AvgELSLIdx = Vector3.Scale(AllQuads[i][3] + AllQuads[i + 1][1], AverageScale);
                if (i == 0)
                    vertices.AddRange(
                    new Vector3[] {
				                // First Quad
				                AllQuads[i][0],AvgESIdx,AllQuads[i][1],
			                  	AllQuads[i][1],AvgESIdx,AvgELSLIdx,
			                  	// Reverse
			                  	AllQuads[i][0],AllQuads[i][1],AvgESIdx,
			                  	AllQuads[i][1],AvgELSLIdx,AvgESIdx,
			    });
                else
                {
                    var AvgESIdxP = Vector3.Scale(AllQuads[i - 1][2] + AllQuads[i][0], AverageScale);
                    var AvgELSLIdxP = Vector3.Scale(AllQuads[i - 1][3] + AllQuads[i][1], AverageScale);
                    vertices.AddRange(
                    new Vector3[] {
					                // First Quad
					                AvgESIdxP,AvgESIdx,AvgELSLIdxP,
				                  	AvgELSLIdxP,AvgESIdx,AvgELSLIdx,
				                  	// Reverse
				                  	AvgESIdxP,AvgELSLIdxP,AvgESIdx,
				                  	AvgELSLIdxP,AvgELSLIdx,AvgESIdx,
				    });
                }
            }
            UVs.AddRange(
                new Vector2[]{
								// First Quad
								uvTopLeft,uvTopRight,uvBottomLeft,
								uvBottomLeft,uvTopRight,uvBottomRight,
								//Reverse
								uvTopLeft,uvBottomLeft,uvTopRight,
								uvBottomLeft,uvBottomRight,uvTopRight,
				});

        }
        // Last quad
        if (Points.Length > 2)
        {
            var AvgESIdxlq = Vector3.Scale(AllQuads[AllQuads.Length - 2][2] + AllQuads[AllQuads.Length - 1][0], AverageScale);
            var AvgELSLIdxlq = Vector3.Scale(AllQuads[AllQuads.Length - 2][3] + AllQuads[AllQuads.Length - 1][1], AverageScale);
            vertices.AddRange(
                    new Vector3[] {
				                AvgESIdxlq,AllQuads[AllQuads.Length-1][2],AvgELSLIdxlq,
			                  	AvgELSLIdxlq,AllQuads[AllQuads.Length-1][2],AllQuads[AllQuads.Length-1][3],
			                  	// Reverse
			                  	AvgESIdxlq,AvgELSLIdxlq,AllQuads[AllQuads.Length-1][2],
			                  	AvgELSLIdxlq,AllQuads[AllQuads.Length-1][3],AllQuads[AllQuads.Length-1][2]
			    });
            UVs.AddRange(
                    new Vector2[]{

								uvTopLeft,uvTopRight,uvBottomLeft,
								uvBottomLeft,uvTopRight,uvBottomRight,
								//Reverse
								uvTopLeft,uvBottomLeft,uvTopRight,
								uvBottomLeft,uvBottomRight,uvTopRight,
				});
        }

        int[] tris = new int[vertices.Count];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;

        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, false);

    }
    public void TestMeshGenerationSimpleBackup(Vector3[] Points, float Width, float Smoothness, OSMType type, string Id, string other, string tag, string MaterialName, float height = 0f)
    {
        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        gameObject.tag = tag;
        Polygon p = gameObject.AddComponent<Polygon>();
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
        }
        else
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh();


        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        Vector2 uvTopLeft = new Vector2(1, 0);// new Vector2(0,0);
        Vector2 uvTopRight = new Vector2(0, 1);//new Vector2(1,0);
        Vector2 uvBottomLeft = new Vector2(1, 1);//new Vector2(0,1);
        Vector2 uvBottomRight = new Vector2(0, 0);//new Vector2(1,1);
        for (int i = 0; i < Points.Length - 1; i++)
        {
            // Start 0 ,Start-L 1 ,End 2 , End-L 3
            var quads = Interpolations.MakeQuad(Points[i], Points[i + 1], Width, gameObject.transform);
            // S,E,SL
            // vertices.AddRange(new Vector3[] { S, E, SL });
            // vertices.AddRange(new Vector3[] { SL, E, EL });
            // Reverse
            // vertices.AddRange(new Vector3[] { S, SL, E });
            // vertices.AddRange(new Vector3[] { SL, EL, E });

            vertices.AddRange(
                new Vector3[] {
			                  	quads[0],quads[2],quads[1],
			                  	quads[1],quads[2],quads[3],
			                  	// Reverse
			                  	quads[0],quads[1],quads[2],
			                  	quads[1],quads[3],quads[2]
			    });
            UVs.AddRange(
                new Vector2[]{
								uvTopLeft,uvTopRight,uvBottomLeft,
								uvBottomLeft,uvTopRight,uvBottomRight,
								//Reverse
								uvTopLeft,uvBottomLeft,uvTopRight,
								uvBottomLeft,uvBottomRight,uvTopRight
				});

        }


        int[] tris = new int[vertices.Count];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;

        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, false);
        //		if (GenerateColliders)
        //			gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

    }
    private static bool DebugMode = false;
    public static Vector3 FindMinimum(Vector3[] points)
    {
        var ret = points[0];
        foreach (var p in points)
            if (Vector3.Min(ret, p) == p)
                ret = p;
        return ret;
    }
    public static void MeshGenerationFilledCorners(Vector3[] Points, float Width, OSMType type, string Id, string other, string tag, string MaterialName, float height)
    {
        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        // TEST: Add the gameobject at the target position
        //
        // var PivotPoint = FindMinimum(Points);
        //var PivotPoint = Points[(Points.Length - 1) / 2];
        //gameObject.transform.position = PivotPoint;
        //for (int i = 0; i < Points.Length; i++)
        //{
        //    Points[i] -= PivotPoint;
        //}
        //
      

        if (!string.IsNullOrEmpty(tag))
            gameObject.tag = tag;
        Polygon p = gameObject.AddComponent<Polygon>();
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
        }
        else
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
        if (DebugMode)
        {
            if (Points.Length > 2)
                for (int i = 0; i < Points.Length - 2; i++)
                {
                    GameObject g = new GameObject();
                    g.transform.position = Points[i + 1];
                    g.transform.parent = gameObject.transform;
                    if (IsClockwise(Points[i], Points[i + 1], Points[i + 2]))
                    {
                        g.name = i + "Clockwise";
                    }
                    else
                        g.name = i + "Counterclockwise";

                    g.name += "-L1:" + (GetAngleOfLineBetweenTwoPoints(Points[i], Points[i + 1]) - GetAngleOfLineBetweenTwoPoints(Points[i + 1], Points[i + 2]));
                }
        }
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        Vector2 uvTopLeft = new Vector2(0, 0);// new Vector2(0,0);
        Vector2 uvTopRight = new Vector2(1, 0);//new Vector2(1,0);
        Vector2 uvBottomLeft = new Vector2(0, 1);//new Vector2(0,1);
        Vector2 uvBottomRight = new Vector2(1, 1);//new Vector2(1,1);
        Vector2 uvMiddleLeft = new Vector2(0, 0.5f);
        Vector2 uvMiddleRight = new Vector2(1, 0.5f);

        int offset = 1;
        //if (Points.Length > 2)
        //  offset = 2;
        Vector3 AverageScale = new Vector3(0.5f, 1, 0.5f);

        Vector3[][] AllQuads = new Vector3[Points.Length - 1][];
        for (int i = 0; i < Points.Length - 1; i++)
        {
            AllQuads[i] = Interpolations.MakeQuad(Points[i], Points[i + 1], Width, gameObject.transform);
            AllQuads[i][0].y = height;
            AllQuads[i][1].y = height;
            AllQuads[i][2].y = height;
            AllQuads[i][3].y = height;

            if (DebugMode)
            {
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
                s.name = "S " + i;
                s.transform.localScale = s.transform.localScale * 0.25f;
                s.transform.position = AllQuads[i][0];
                s.GetComponent<Renderer>().sharedMaterial = Resources.Load("S", typeof(Material)) as Material;

                GameObject sl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sl.name = "SL " + i;
                sl.transform.localScale = sl.transform.localScale * 0.25f;
                sl.transform.position = AllQuads[i][1];
                sl.GetComponent<Renderer>().sharedMaterial = Resources.Load("SL", typeof(Material)) as Material;

                GameObject e = GameObject.CreatePrimitive(PrimitiveType.Cube);
                e.name = "E " + i;
                e.transform.localScale = e.transform.localScale * 0.25f;
                e.transform.position = AllQuads[i][2];
                e.GetComponent<Renderer>().sharedMaterial = Resources.Load("E", typeof(Material)) as Material;

                GameObject el = GameObject.CreatePrimitive(PrimitiveType.Cube);
                el.name = "EL " + i;
                el.transform.localScale = el.transform.localScale * 0.25f;
                el.transform.position = AllQuads[i][3];
                el.GetComponent<Renderer>().sharedMaterial = Resources.Load("EL", typeof(Material)) as Material;

                s.transform.parent = gameObject.transform;
                sl.transform.parent = gameObject.transform;
                e.transform.parent = gameObject.transform;
                el.transform.parent = gameObject.transform;
            }
        }
        bool Clockwise = true;
        for (int i = 0; i < Points.Length - offset; i++)
        {
            Clockwise = true;
            // Start 0 ,Start-L 1 ,End 2 , End-L 3
            if (Points.Length == 2 || i == 0)
            {
                #region First segment
                vertices.AddRange(
                    new Vector3[] {
				                AllQuads[i][0],AllQuads[i][2],AllQuads[i][1],
			                  	AllQuads[i][1],AllQuads[i][2],AllQuads[i][3],
			                  	// Reverse
			                  	AllQuads[i][0],AllQuads[i][1],AllQuads[i][2],
			                  	AllQuads[i][1],AllQuads[i][3],AllQuads[i][2]
			    });
                #endregion
            }
            else if (i == Points.Length - 1)
            {
                #region Last segment
                Vector3 MatchClockwise;
                Vector3 OffsetClockwise = Vector3.zero;
                // CLOCKWISE
                if (IsClockwise(Points[i - 2], Points[i - 1], Points[i])) /*Sl Match to El*/
                {
                    Clockwise = true;
                    MatchClockwise = AllQuads[i - 2][2]; // El
                    OffsetClockwise = AllQuads[i - 2][2] - AllQuads[i - 1][0];
                    OffsetClockwise.y = 0;
                }
                else MatchClockwise = AllQuads[i - 1][0]; // S

                Vector3 MatchCounterClockwise;
                Vector3 OffsetCounterClockwise = Vector3.zero;
                // COUNTERCLOCKWISE
                if (!IsClockwise(Points[i - 2], Points[i - 1], Points[i])) /*Sl Match to El*/
                {
                    Clockwise = false;
                    MatchCounterClockwise = AllQuads[i - 2][3]; // El
                    OffsetCounterClockwise = AllQuads[i - 2][3] - AllQuads[i - 1][1];
                    OffsetCounterClockwise.y = 0;
                }
                else MatchCounterClockwise = AllQuads[i - 1][1]; // S  


                // Generate filled triangles or filled quads
                // Add UV before adding the vertices.
                #region Triangles
                if (Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 2][3];
                    Vector3 t2 = AllQuads[i - 1][1] + OffsetClockwise;
                    Vector3 t3 = AllQuads[i - 2][2];
                    vertices.AddRange(
                        new Vector3[] {
							t1,t2,t3,
							t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
							uvTopLeft,uvBottomLeft,uvMiddleRight,
							uvTopLeft,uvMiddleRight,uvBottomLeft
						});
                }
                else if (!Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 2][2];
                    Vector3 t2 = AllQuads[i - 2][3];
                    Vector3 t3 = AllQuads[i - 1][0] + OffsetCounterClockwise;
                    vertices.AddRange(
                        new Vector3[] {
						  t1,t2,t3,
						  t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
						  uvTopRight,uvMiddleLeft,uvBottomRight,
						  uvTopRight,uvBottomRight,uvMiddleLeft
						});
                }
                #endregion


                vertices.AddRange(
                new Vector3[] {
												OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,AllQuads[i][2],OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,
			                  	OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],AllQuads[i][3],
			                  	// Reverse
			                  	OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],
			                  	OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][3],AllQuads[i][2]
			    });
                #endregion
            }
            else
            {
                #region Middle segments
                Vector3 MatchClockwise;
                Vector3 OffsetClockwise = Vector3.zero;
                // CLOCKWISE
                if (IsClockwise(Points[i - 1], Points[i], Points[i + 1])) /*Sl Match to El*/
                {
                    Clockwise = true;
                    MatchClockwise = AllQuads[i - 1][2]; // El
                    OffsetClockwise = AllQuads[i - 1][2] - AllQuads[i][0];
                    OffsetClockwise.y = 0;
                }
                else MatchClockwise = AllQuads[i][0]; // S

                Vector3 MatchCounterClockwise;
                Vector3 OffsetCounterClockwise = Vector3.zero;
                // COUNTERCLOCKWISE
                if (!IsClockwise(Points[i - 1], Points[i], Points[i + 1])) /*Sl Match to El*/
                {
                    Clockwise = false;
                    MatchCounterClockwise = AllQuads[i - 1][3]; // El
                    OffsetCounterClockwise = AllQuads[i - 1][3] - AllQuads[i][1];
                    OffsetCounterClockwise.y = 0;
                }
                else MatchCounterClockwise = AllQuads[i][1]; // S  


                // Generate filled triangles or filled quads
                // Add UV before adding the vertices.
                #region Triangles
                if (Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 1][3];
                    Vector3 t2 = AllQuads[i][1] + OffsetClockwise;
                    Vector3 t3 = AllQuads[i - 1][2];
                    vertices.AddRange(
                        new Vector3[] {
							t1,t2,t3,
							t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
							uvTopLeft,uvBottomLeft,uvMiddleRight,
							uvTopLeft,uvMiddleRight,uvBottomLeft
						});
                }
                else if (!Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 1][2];
                    Vector3 t2 = AllQuads[i - 1][3];
                    Vector3 t3 = AllQuads[i][0] + OffsetCounterClockwise;
                    vertices.AddRange(
                        new Vector3[] {
						  t1,t2,t3,
						  t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
						  uvTopRight,uvMiddleLeft,uvBottomRight,
						  uvTopRight,uvBottomRight,uvMiddleLeft
						});
                }

                #endregion


                vertices.AddRange(
                new Vector3[] {
												OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,AllQuads[i][2],OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,
			                  	OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],AllQuads[i][3],
			                  	// Reverse
			                  	OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],
			                  	OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][3],AllQuads[i][2]
			    });



                #endregion
            }
            UVs.AddRange(
                new Vector2[]{
								// First Quad
                                uvTopLeft,uvBottomRight,uvTopRight,
                                uvTopLeft,uvBottomRight,uvBottomLeft,
                                //Reverse
                                uvTopRight,uvTopLeft,uvBottomRight,
                                uvTopLeft,uvBottomLeft,uvBottomRight
				});
        }


        int[] tris = new int[vertices.Count];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;


        // Pivot on center of bounding box.
        gameObject.transform.position = ChangePivot(ref vertices, PivotLocation.Center);
        ////

        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, true);
        GameObject.DestroyImmediate(p);
    }
    public enum PivotLocation {Center,MiddleVector};
    public static Vector3 ChangePivot(ref List<Vector3> vertices, PivotLocation pivotLocation)
    {
        var pivotxMin = vertices.Select(i => i.x).Min();
        var pivotzMin = vertices.Select(i => i.z).Min();
        var pivotyMin = vertices.Select(i => i.y).Min();
        var pivotxMax = vertices.Select(i => i.x).Max();
        var pivotzMax = vertices.Select(i => i.z).Max();
        var pivotyMax = vertices.Select(i => i.y).Max();
        var PivotPoint = new Vector3((pivotxMin + pivotxMax) / 2, (pivotyMin + pivotyMax) / 2, (pivotzMin + pivotzMax) / 2);
        
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] -= PivotPoint;
        }
        return PivotPoint;
    }
    public static void MeshGenerationFilledCornersORIGINAL(Vector3[] Points, float Width, OSMType type, string Id, string other, string tag, string MaterialName, float height)
    {
        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        if (!string.IsNullOrEmpty(tag))
            gameObject.tag = tag;
        Polygon p = gameObject.AddComponent<Polygon>();
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
        }
        else
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
        if (DebugMode)
        {
            if (Points.Length > 2)
                for (int i = 0; i < Points.Length - 2; i++)
                {
                    GameObject g = new GameObject();
                    g.transform.position = Points[i + 1];
                    g.transform.parent = gameObject.transform;
                    if (IsClockwise(Points[i], Points[i + 1], Points[i + 2]))
                    {
                        g.name = i + "Clockwise";
                    }
                    else
                        g.name = i + "Counterclockwise";

                    g.name += "-L1:" + (GetAngleOfLineBetweenTwoPoints(Points[i], Points[i + 1]) - GetAngleOfLineBetweenTwoPoints(Points[i + 1], Points[i + 2]));
                }
        }
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        Vector2 uvTopLeft = new Vector2(0, 0);// new Vector2(0,0);
        Vector2 uvTopRight = new Vector2(1, 0);//new Vector2(1,0);
        Vector2 uvBottomLeft = new Vector2(0, 1);//new Vector2(0,1);
        Vector2 uvBottomRight = new Vector2(1, 1);//new Vector2(1,1);
        Vector2 uvMiddleLeft = new Vector2(0, 0.5f);
        Vector2 uvMiddleRight = new Vector2(1, 0.5f);

        int offset = 1;
        //if (Points.Length > 2)
        //  offset = 2;
        Vector3 AverageScale = new Vector3(0.5f, 1, 0.5f);

        Vector3[][] AllQuads = new Vector3[Points.Length - 1][];
        for (int i = 0; i < Points.Length - 1; i++)
        {
            AllQuads[i] = Interpolations.MakeQuad(Points[i], Points[i + 1], Width, gameObject.transform);
            AllQuads[i][0].y = height;
            AllQuads[i][1].y = height;
            AllQuads[i][2].y = height;
            AllQuads[i][3].y = height;

            if (DebugMode)
            {
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
                s.name = "S " + i;
                s.transform.localScale = s.transform.localScale * 0.25f;
                s.transform.position = AllQuads[i][0];
                s.GetComponent<Renderer>().sharedMaterial = Resources.Load("S", typeof(Material)) as Material;

                GameObject sl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sl.name = "SL " + i;
                sl.transform.localScale = sl.transform.localScale * 0.25f;
                sl.transform.position = AllQuads[i][1];
                sl.GetComponent<Renderer>().sharedMaterial = Resources.Load("SL", typeof(Material)) as Material;

                GameObject e = GameObject.CreatePrimitive(PrimitiveType.Cube);
                e.name = "E " + i;
                e.transform.localScale = e.transform.localScale * 0.25f;
                e.transform.position = AllQuads[i][2];
                e.GetComponent<Renderer>().sharedMaterial = Resources.Load("E", typeof(Material)) as Material;

                GameObject el = GameObject.CreatePrimitive(PrimitiveType.Cube);
                el.name = "EL " + i;
                el.transform.localScale = el.transform.localScale * 0.25f;
                el.transform.position = AllQuads[i][3];
                el.GetComponent<Renderer>().sharedMaterial = Resources.Load("EL", typeof(Material)) as Material;

                s.transform.parent = gameObject.transform;
                sl.transform.parent = gameObject.transform;
                e.transform.parent = gameObject.transform;
                el.transform.parent = gameObject.transform;
            }
        }
        bool Clockwise = true;
        for (int i = 0; i < Points.Length - offset; i++)
        {
            Clockwise = true;
            // Start 0 ,Start-L 1 ,End 2 , End-L 3
            if (Points.Length == 2 || i == 0)
            {
                #region First segment
                vertices.AddRange(
                    new Vector3[] {
				                AllQuads[i][0],AllQuads[i][2],AllQuads[i][1],
			                  	AllQuads[i][1],AllQuads[i][2],AllQuads[i][3],
			                  	// Reverse
			                  	AllQuads[i][0],AllQuads[i][1],AllQuads[i][2],
			                  	AllQuads[i][1],AllQuads[i][3],AllQuads[i][2]
			    });
                #endregion
            }
            else if (i == Points.Length - 1)
            {
                #region Last segment
                Vector3 MatchClockwise;
                Vector3 OffsetClockwise = Vector3.zero;
                // CLOCKWISE
                if (IsClockwise(Points[i - 2], Points[i - 1], Points[i])) /*Sl Match to El*/
                {
                    Clockwise = true;
                    MatchClockwise = AllQuads[i - 2][2]; // El
                    OffsetClockwise = AllQuads[i - 2][2] - AllQuads[i - 1][0];
                    OffsetClockwise.y = 0;
                }
                else MatchClockwise = AllQuads[i - 1][0]; // S

                Vector3 MatchCounterClockwise;
                Vector3 OffsetCounterClockwise = Vector3.zero;
                // COUNTERCLOCKWISE
                if (!IsClockwise(Points[i - 2], Points[i - 1], Points[i])) /*Sl Match to El*/
                {
                    Clockwise = false;
                    MatchCounterClockwise = AllQuads[i - 2][3]; // El
                    OffsetCounterClockwise = AllQuads[i - 2][3] - AllQuads[i - 1][1];
                    OffsetCounterClockwise.y = 0;
                }
                else MatchCounterClockwise = AllQuads[i - 1][1]; // S  


                // Generate filled triangles or filled quads
                // Add UV before adding the vertices.
                #region Triangles
                if (Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 2][3];
                    Vector3 t2 = AllQuads[i - 1][1] + OffsetClockwise;
                    Vector3 t3 = AllQuads[i - 2][2];
                    vertices.AddRange(
                        new Vector3[] {
							t1,t2,t3,
							t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
							uvTopLeft,uvBottomLeft,uvMiddleRight,
							uvTopLeft,uvMiddleRight,uvBottomLeft
						});
                }
                else if (!Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 2][2];
                    Vector3 t2 = AllQuads[i - 2][3];
                    Vector3 t3 = AllQuads[i - 1][0] + OffsetCounterClockwise;
                    vertices.AddRange(
                        new Vector3[] {
						  t1,t2,t3,
						  t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
						  uvTopRight,uvMiddleLeft,uvBottomRight,
						  uvTopRight,uvBottomRight,uvMiddleLeft
						});
                }
                #endregion


                vertices.AddRange(
                new Vector3[] {
												OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,AllQuads[i][2],OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,
			                  	OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],AllQuads[i][3],
			                  	// Reverse
			                  	OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],
			                  	OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][3],AllQuads[i][2]
			    });
                #endregion
            }
            else
            {
                #region Middle segments
                Vector3 MatchClockwise;
                Vector3 OffsetClockwise = Vector3.zero;
                // CLOCKWISE
                if (IsClockwise(Points[i - 1], Points[i], Points[i + 1])) /*Sl Match to El*/
                {
                    Clockwise = true;
                    MatchClockwise = AllQuads[i - 1][2]; // El
                    OffsetClockwise = AllQuads[i - 1][2] - AllQuads[i][0];
                    OffsetClockwise.y = 0;
                }
                else MatchClockwise = AllQuads[i][0]; // S

                Vector3 MatchCounterClockwise;
                Vector3 OffsetCounterClockwise = Vector3.zero;
                // COUNTERCLOCKWISE
                if (!IsClockwise(Points[i - 1], Points[i], Points[i + 1])) /*Sl Match to El*/
                {
                    Clockwise = false;
                    MatchCounterClockwise = AllQuads[i - 1][3]; // El
                    OffsetCounterClockwise = AllQuads[i - 1][3] - AllQuads[i][1];
                    OffsetCounterClockwise.y = 0;
                }
                else MatchCounterClockwise = AllQuads[i][1]; // S  


                // Generate filled triangles or filled quads
                // Add UV before adding the vertices.
                #region Triangles
                if (Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 1][3];
                    Vector3 t2 = AllQuads[i][1] + OffsetClockwise;
                    Vector3 t3 = AllQuads[i - 1][2];
                    vertices.AddRange(
                        new Vector3[] {
							t1,t2,t3,
							t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
							uvTopLeft,uvBottomLeft,uvMiddleRight,
							uvTopLeft,uvMiddleRight,uvBottomLeft
						});
                }
                else if (!Clockwise)
                {
                    Vector3 t1 = AllQuads[i - 1][2];
                    Vector3 t2 = AllQuads[i - 1][3];
                    Vector3 t3 = AllQuads[i][0] + OffsetCounterClockwise;
                    vertices.AddRange(
                        new Vector3[] {
						  t1,t2,t3,
						  t1,t3,t2
						});
                    UVs.AddRange(
                        new Vector2[] {
						  uvTopRight,uvMiddleLeft,uvBottomRight,
						  uvTopRight,uvBottomRight,uvMiddleLeft
						});
                }

                #endregion


                vertices.AddRange(
                new Vector3[] {
												OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,AllQuads[i][2],OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,
			                  	OffsetClockwise+ MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],AllQuads[i][3],
			                  	// Reverse
			                  	OffsetCounterClockwise+MatchClockwise/*+AllQuads[i][0]*/,OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][2],
			                  	OffsetClockwise+MatchCounterClockwise/*AllQuads[i][1]*/,AllQuads[i][3],AllQuads[i][2]
			    });



                #endregion
            }
            UVs.AddRange(
                new Vector2[]{
								// First Quad
								uvTopLeft,uvBottomRight,uvTopLeft,
								uvTopLeft,uvBottomRight,uvBottomLeft,
								//Reverse
								uvTopRight,uvTopLeft,uvBottomRight,
								uvTopLeft,uvBottomLeft,uvBottomRight
				});
        }


        int[] tris = new int[vertices.Count];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;


      



        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, true);
        GameObject.DestroyImmediate(p);
    }

    public void GenerateRoad(Vector3[] Points, float Width, float Smoothness, OSMType type, string Id, string other, string tag, string MaterialName, float height = 0f)
    {

//        GameObject pathMesh = new GameObject();
//        pathMesh.name = type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : "");
//        pathMesh.tag = tag;
//        pathMesh.AddComponent(typeof(MeshFilter));
//        pathMesh.AddComponent(typeof(MeshRenderer));
//        pathMesh.renderer.sharedMaterial = Resources.Load(MaterialName, typeof(Material)) as Material;
//        pathMesh.AddComponent("AttachedPathScript");
//        AttachedPathScript pathScript = pathMesh.GetComponent<AttachedPathScript>();
//        pathScript.pathMesh = pathMesh;
//        pathScript.parentTerrain = GameObject.Find("Terrain"); //gameObject;
//        pathScript.NewPath();
//
//        pathScript.isRoad = true;
//        pathScript.pathSmooth = (int)Smoothness;
//        pathScript.pathWidth = Width;
//
//        Debug.Log(pathScript.pathWidth);
//        // List<TerrainPathCell> nodes = new List<TerrainPathCell>();
//        foreach (var p in Points)
//        {
//            TerrainPathCell temp = new TerrainPathCell();
//            temp.position = new Vector2(p.x, p.z);
//            temp.heightAtCell = height;
//            pathScript.CreatePathNode(temp, Width);
//        }
//
//        // EditorUtility.SetDirty(pathScript)s;
//        //pathScript.CreatePath(pathScript.pathSmooth, true, true);
//
//        if (pathScript.nodeObjects.Length > 1)
//        {
//            // define terrain cells
//            pathScript.terrainCells = new TerrainPathCell[pathScript.terData.heightmapResolution * pathScript.terData.heightmapResolution];
//
//            for (int x = 0; x < pathScript.terData.heightmapResolution; x++)
//            {
//                for (int y = 0; y < pathScript.terData.heightmapResolution; y++)
//                {
//                    pathScript.terrainCells[(y) + (x * pathScript.terData.heightmapResolution)].position.y = y;
//                    pathScript.terrainCells[(y) + (x * pathScript.terData.heightmapResolution)].position.x = x;
//                    pathScript.terrainCells[(y) + (x * pathScript.terData.heightmapResolution)].heightAtCell = pathScript.terrainHeights[y, x];
//                    pathScript.terrainCells[(y) + (x * pathScript.terData.heightmapResolution)].isAdded = false;
//                }
//            }
//
//            // finalize path
//            // Undo.RegisterUndo(pathScript.terData, "Undo finalize path");
//            bool success = pathScript.FinalizePath();
//
//            if (success)
//            {
//                if (pathScript.isRoad)
//                {
//                    pathScript.pathMesh.renderer.enabled = true;
//                }
//
//                else
//                {
//                    MeshFilter meshFilter = (MeshFilter)pathScript.pathMesh.GetComponent(typeof(MeshFilter));
//                    Mesh destroyMesh = meshFilter.sharedMesh;
//
//                    DestroyImmediate(destroyMesh);
//                    DestroyImmediate(meshFilter);
//                }
//            }
//        }
//
//        else
//            Debug.Log("Not enough nodes to finalize");

        //Object.DestroyImmediate(pathScript);
    }
    public static bool IsClockwise(Vector3 a, Vector3 b, Vector3 c)
    {
        float[] mat = new float[]
        {
            1,a.x,a.z,
            1,c.x,c.z,
            1,b.x,b.z
        };
        return mat.Det() > 0;
        //float sum = 0;
        //sum += (b.x - a.x) * (b.z + a.z);
        //sum += (c.x - b.x) * (c.z + b.z);
        //if (sum >= 0)
        //  return true;
        //else return false;
        
        //if (GetAngleOfLineBetweenTwoPoints(a, b) - GetAngleOfLineBetweenTwoPoints(b, c) >= 0)
        //    return true;
        //else return false;
    }
    public static float GetAngleOfLineBetweenTwoPoints(Vector3 p1, Vector3 p2)
    {
        float xDiff = p2.x - p1.x;
        float zDiff = p2.z - p1.z;
        return Mathf.Atan2(zDiff, xDiff) * (180 / Mathf.PI);
    }
    public static float AngleBetweenThreePoints(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return GetAngleOfLineBetweenTwoPoints(p1, p2) - GetAngleOfLineBetweenTwoPoints(p2, p3);
    }

    public static Vector3[] SegmentPoints(Vector3[] OriginalPoints, float precision)
    {
        List<Vector3> CurrentPoints = new List<Vector3>(OriginalPoints);
        List<Vector3> tPoints = new List<Vector3>();
        for (int i = 0; i < CurrentPoints.Count; i++)
        {
            tPoints.Add(CurrentPoints[i]);
            //Debug.Log("i "+i);
            if (i != CurrentPoints.Count - 1)
            {
                var diff = CurrentPoints[i + 1] - CurrentPoints[i];
                // Number of segments per unit distance
                var distance = Vector3.Distance(CurrentPoints[i], CurrentPoints[i + 1]);
                int tempNoOfSegments;
                if (precision == 0)
                    tempNoOfSegments = Mathf.FloorToInt(distance);
                else
                    tempNoOfSegments = Mathf.FloorToInt(distance / precision);
                Vector3 segment;
                if (distance >= precision)
                {
                    segment = diff / tempNoOfSegments;
                    for (int n = 0; n < tempNoOfSegments - 1; n++)
                        tPoints.Add(CurrentPoints[i] + segment * (n + 1));
                }
                else
                {
                    // TODO: Remove the current node and make the calculations for
                    //					segment=diff/1;
                    //					for (int n=0;n<NumberOfSegments-1;n++)
                    //						tPoints.Add(OriginalPoints[i]+segment*(n+1));
                }
            }
        }
        return tPoints.ToArray();
    }
}