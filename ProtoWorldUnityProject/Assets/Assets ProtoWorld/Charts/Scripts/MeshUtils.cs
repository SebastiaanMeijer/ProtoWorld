/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * Utilities to create Mesh for Charts
 * MeshUtils.cs
 * Johnson Ho
 * 
 * 
 */

using UnityEngine;
using System;

public class MeshUtils : MonoBehaviour
{

    // Use this for initialization
    static public Vector3 mNormal = new Vector3(0f, 0f, -1f);

    // Call this will take care of removing the temperary mesh.
    public static void UpdateLineMesh(Mesh mesh, Vector3[] points, float width)
    {
        Mesh tempMesh = GenerateLineMesh(points, width);
        mesh.vertices = tempMesh.vertices;
        mesh.normals = tempMesh.normals;
        mesh.uv = tempMesh.uv;
        mesh.triangles = tempMesh.triangles;
        Destroy(tempMesh);
    }

    // Call this and you have to take care of removing the old mesh.
    public static Mesh GenerateLineMesh(Vector3[] points, float width)
    {
        if ((points.Length % 2) != 0)
        {
            Debug.LogWarning("Points not in pair");
            return null;
        }

        Mesh mesh = new Mesh();

        int length = points.Length * 2;

        Vector3[] verts = new Vector3[length];
        Vector3[] norms = new Vector3[length];
        Vector2[] uvs = new Vector2[length];
        int[] trias = new int[points.Length * 3];

        int idx = 0;
        int triIdx = 0;

        for (int i = 0; i < points.Length; i += 2)
        {
            Vector3 vec = (points[i] - points[i + 1]);
            Vector3 crs = Vector3.Cross(mNormal, vec).normalized;
            Vector3 perpendicular = crs * width / 2;
            //Debug.Log(perpendicular);

            verts[idx] = points[i] - perpendicular;
            verts[idx + 1] = points[i] + perpendicular;
            verts[idx + 2] = points[i + 1] - perpendicular;
            verts[idx + 3] = points[i + 1] + perpendicular;

            //Debug.Log(verts[idx + 0]);
            //Debug.Log(verts[idx + 1]);
            //Debug.Log(verts[idx + 2]);
            //Debug.Log(verts[idx + 3]);

            norms[idx] = mNormal;
            norms[idx + 1] = mNormal;
            norms[idx + 2] = mNormal;
            norms[idx + 3] = mNormal;

            uvs[idx] = new Vector2(verts[0].x, verts[0].y);
            uvs[idx + 1] = new Vector2(verts[1].x, verts[1].y);
            uvs[idx + 2] = new Vector2(verts[2].x, verts[2].y);
            uvs[idx + 3] = new Vector2(verts[3].x, verts[3].y);

            trias[triIdx++] = idx;
            trias[triIdx++] = idx + 1;
            trias[triIdx++] = idx + 2;
            trias[triIdx++] = idx + 1;
            trias[triIdx++] = idx + 3;
            trias[triIdx++] = idx + 2;

            idx += 4;
        }
        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.uv = uvs;
        mesh.triangles = trias;
        return mesh;
    }

    public static void UpdateRectMesh(Mesh mesh, Rect rect)
    {
        Mesh tempMesh = GenerateRectMesh(rect, mesh.name);
        mesh.vertices = tempMesh.vertices;
        mesh.normals = tempMesh.normals;
        mesh.uv = tempMesh.uv;
        mesh.triangles = tempMesh.triangles;
        Destroy(tempMesh);
    }

    public static Mesh GenerateRectMesh(Rect rect, string meshName = "Rect Mesh")
    {
        Mesh mesh = new Mesh();
        mesh.name = meshName;

        Vector3 normVec = MeshUtils.mNormal;

        if (rect.width > 0 && rect.height > 0)
        {
            Vector3[] verts = new Vector3[4];
            Vector3[] norms = new Vector3[] { normVec, normVec, normVec, normVec };
            Vector2[] uvs = new Vector2[4];
            int[] trias = new int[6];

            verts[0] = new Vector3(rect.xMin, rect.yMin);
            verts[1] = new Vector3(rect.xMin, rect.yMax);
            verts[2] = new Vector3(rect.xMax, rect.yMax);
            verts[3] = new Vector3(rect.xMax, rect.yMin);

            uvs[0] = new Vector2(verts[0].x, verts[0].y);
            uvs[1] = new Vector2(verts[1].x, verts[1].y);
            uvs[2] = new Vector2(verts[2].x, verts[2].y);
            uvs[3] = new Vector2(verts[3].x, verts[3].y);

            trias[0] = 0;
            trias[1] = 1;
            trias[2] = 2;
            trias[3] = 0;
            trias[4] = 2;
            trias[5] = 3;

            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.triangles = trias;
        }

        return mesh;

    }

    public static String NameGenerator(string str, int i)
    {
        return String.Format("{0} {1}", str, i);
    }
}
