/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * KPI MODULE
 * Johnson Ho
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct LinesForMesh
{
    public Vector3[] lines;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

}

public static class ChartUtils
{
    public static Vector3[] CreateLinesFromData(List<TimedData> list, RectTransform rectTransform, Rect bounds)
    {
        Vector3[] lines = new Vector3[list.Count * 2];

        float xScale = rectTransform.rect.width / (bounds.xMax - bounds.xMin);
        float yScale = rectTransform.rect.height / (bounds.yMax - bounds.yMin);

        int lineIdx = 0;
        for (int i = 0; i < list.Count - 1; i++)
        {
            lines[lineIdx++] = new Vector3((list[i].time - bounds.xMin) * xScale, (list[i].GetData() - bounds.yMin) * yScale);
            lines[lineIdx++] = new Vector3((list[i + 1].time - bounds.xMin) * xScale, (list[i + 1].GetData() - bounds.yMin) * yScale);
        }
        return lines;
    }

    public static Vector3[] CreateLineFromData(List<TimedData> list, RectTransform rectTransform, Rect bounds)
    {
        Vector3[] lines = new Vector3[list.Count];

        float xScale = rectTransform.rect.width / (bounds.xMax - bounds.xMin);
        float yScale = rectTransform.rect.height / (bounds.yMax - bounds.yMin);

        int lineIdx = 0;
        for (int i = 0; i < list.Count - 1; i++)
        {
            lines[lineIdx++] = new Vector3((list[i].time - bounds.xMin) * xScale, (list[i].GetData() - bounds.yMin) * yScale);
        }
        return lines;
    }

    public static LinesForMesh CreateLinesFromData(List<TimedData> list, RectTransform rectTransform)
    {
        Vector3[] lines = new Vector3[list.Count * 2];

        float startTime = list[0].time;
        float endTime = list[list.Count - 1].time;

        float minValue = list[0].GetData();
        float maxValue = list[0].GetData();
        float value;
        int lineIdx = 0;
        for (int i = 0; i < list.Count - 1; i++)
        {
            lines[lineIdx++] = new Vector3(list[i].time, list[i].GetData());
            value = list[i + 1].GetData();
            lines[lineIdx++] = new Vector3(list[i + 1].time, value);
            if (value < minValue)
                minValue = value;
            if (value > maxValue)
                maxValue = value;
        }

        float xScale = rectTransform.rect.width / (endTime - startTime);
        float yScale = rectTransform.rect.height / (maxValue - minValue);

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].x = (lines[i].x - startTime) * xScale;
            lines[i].y = (lines[i].y - minValue) * yScale;
        }

        LinesForMesh lfm = new LinesForMesh()
        {
            lines = lines,
            minX = startTime,
            maxX = endTime,
            minY = minValue,
            maxY = maxValue
        };

        return lfm;
    }

    public static Mesh GenerateLineMesh(Vector3[] points, float width = 1)
    {
        if ((points.Length % 2) != 0)
        {
            Debug.LogWarning("Points not in pair");
            return null;
        }

        Vector3 mNormal = new Vector3(0f, 0f, -1f);

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

            verts[idx] = points[i] - perpendicular;
            verts[idx + 1] = points[i] + perpendicular;
            verts[idx + 2] = points[i + 1] - perpendicular;
            verts[idx + 3] = points[i + 1] + perpendicular;

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

    public static Mesh GenerateStackedLineMesh(Vector3[] points, Vector3[] basis)
    {
        if ((points.Length % 2) != 0)
        {
            return null;
        }

        Vector2[] outlinePoints = new Vector2[points.Length + basis.Length];
        for (int i = 0; i < points.Length; i++)
        {
            outlinePoints[i] = new Vector2(points[i].x - points[0].x, points[i].y);
        }
        for (int i = 0; i < basis.Length; i++)
        {
            outlinePoints[points.Length + i] = basis[basis.Length - i - 1];
        }
        return CreateMeshFromPoints(outlinePoints);
    }

    public static String NameGenerator(string name, int index)
    {
        return String.Format("{0} {1}", name, index);
    }

    public static Material BlackMaterial 
    {
        get
        {
            Material m = new Material(Shader.Find("UI/Default"));
            m.color = Color.black;
            return m;
        }
    }

    public static string SecondsToTime(float timeInSeconds)
    {
        var time = TimeSpan.FromSeconds(timeInSeconds);
        string str = time.Seconds.ToString("00.") + "s";

        if (time.Hours > 0)
            str = time.Hours.ToString("00.") + "h:" + time.Minutes.ToString("00.") + "m:" + str;
        else if (time.Minutes > 0)
            str = time.Minutes.ToString("00.") + "m:" + str;

        return str;

        //return string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
        //        time.Hours,
        //        time.Minutes,
        //        time.Seconds);
    }

    public static Mesh CreatePieSectorMesh(Vector2 center, float radius, float startangle, float angle)
    {
        //1 per 0.1 radian
        int numPoints = Mathf.CeilToInt(angle * 10f);
        Vector2[] points = new Vector2[numPoints + 2];

        //determine point positions
        for (int i = 0; i <= numPoints; i++)
        {
            float ang = i * angle / numPoints;
            points[i] = center + new Vector2(radius * Mathf.Sin(startangle + ang), radius * Mathf.Cos(startangle + ang));
        }
        points[numPoints+1] = center;

        return CreateMeshFromPoints(points);
    }

    public static Mesh CreateMeshFromPoints(Vector2[] points)
    {
        int[] indices = Triangulate(new List<Vector2>(points));

        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, 0);
        }

        //create mesh
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private static int[] Triangulate(List<Vector2> m_points)
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area(m_points) > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2; )
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(m_points, u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private static float Area(List<Vector2> m_points)
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private static bool Snip(List<Vector2> m_points, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}
