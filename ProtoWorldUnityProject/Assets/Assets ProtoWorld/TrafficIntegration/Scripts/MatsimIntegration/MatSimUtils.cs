/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using MightyLittleGeodesy.Positions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MatSimUtils
{
    public static double[] GetLatLon(float x, float y)
    {
        var sweRef = new SWEREF99Position(y, x);
        var wgs84 = sweRef.ToWGS84();
        return new double[] { wgs84.Latitude, wgs84.Longitude };
    }

    public static float[] GetMinMaxXY<T>(List<T> list)
    {
        if (list != null && list.Count > 0)
        {
            if (list[0] is MatSimNode)
            {
                var nodes = list.Cast<MatSimNode>();
                return new float[] {
                        nodes.Min(n => n.x),
                        nodes.Min(n => n.y),
                        nodes.Max(n => n.x),
                        nodes.Max(n => n.y) };
            }
        }
        return null;
    }

    //Compute the dot product AB . AC
    public static double DotProduct(double[] pointA, double[] pointB, double[] pointC)
    {
        double[] AB = new double[2];
        double[] BC = new double[2];
        AB[0] = pointB[0] - pointA[0];
        AB[1] = pointB[1] - pointA[1];
        BC[0] = pointC[0] - pointB[0];
        BC[1] = pointC[1] - pointB[1];
        double dot = AB[0] * BC[0] + AB[1] * BC[1];

        return dot;
    }

    //Compute the cross product AB x AC
    public static double CrossProduct(double[] pointA, double[] pointB, double[] pointC)
    {
        double[] AB = new double[2];
        double[] AC = new double[2];
        AB[0] = pointB[0] - pointA[0];
        AB[1] = pointB[1] - pointA[1];
        AC[0] = pointC[0] - pointA[0];
        AC[1] = pointC[1] - pointA[1];
        double cross = AB[0] * AC[1] - AB[1] * AC[0];

        return cross;
    }

    //Compute the distance from A to B
    public static double Distance(double[] pointA, double[] pointB)
    {
        double d1 = pointA[0] - pointB[0];
        double d2 = pointA[1] - pointB[1];

        return Math.Sqrt(d1 * d1 + d2 * d2);
    }

    //Compute the distance from AB to C
    //if isSegment is true, AB is a segment, not a line.
    public static double LineToPointDistance2D(double[] pointA, double[] pointB, double[] pointC, bool isSegment = false)
    {
        double dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);
        if (isSegment)
        {
            double dot1 = DotProduct(pointA, pointB, pointC);
            if (dot1 > 0)
                return Distance(pointB, pointC);

            double dot2 = DotProduct(pointB, pointA, pointC);
            if (dot2 > 0)
                return Distance(pointA, pointC);
        }
        return Math.Abs(dist);
    }

    public static Mesh GenerateLineMesh(Vector3[] points, Vector3 normal, float width = 1)
    {
        if ((points.Length % 2) != 0)
        {
            Debug.LogWarning("Points not in pair");
            return null;
        }

        //Vector3 normal = new Vector3(0f, 0f, -1f);

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
            Vector3 crs = Vector3.Cross(normal, vec).normalized;
            Vector3 perpendicular = crs * width / 2;

            verts[idx] = points[i] - perpendicular;
            verts[idx + 1] = points[i] + perpendicular;
            verts[idx + 2] = points[i + 1] - perpendicular;
            verts[idx + 3] = points[i + 1] + perpendicular;

            norms[idx] = normal;
            norms[idx + 1] = normal;
            norms[idx + 2] = normal;
            norms[idx + 3] = normal;

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

}
