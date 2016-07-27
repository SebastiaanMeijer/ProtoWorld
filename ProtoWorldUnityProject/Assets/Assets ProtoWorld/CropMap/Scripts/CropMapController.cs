/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * CROP MAP TOOL
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CropMapController : MonoBehaviour
{
    public double minLat = -1;
    public double maxLat = -1;
    public double minLon = -1;
    public double maxLon = -1;

    [HideInInspector]
    public List<Vector3> polygon;

    [HideInInspector]
    public Vector3[] vertices;

    [Range(1, 50)]
    public float gizmoRadius = 30;

    private bool verbose = false;

    public void CalculateLatLon(MapBoundaries mb)
    {
        var latlon = new Vector3[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            var res = CoordinateConvertor.Vector3ToLatLon(vertices[i], mb);
            latlon[i] = new Vector3(res[0], 0, res[1]);
        }
        var bounds = GetBounds(latlon);
        minLat = bounds[0];
        maxLat = bounds[1];
        minLon = bounds[2];
        maxLon = bounds[3];

        if (verbose)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var convVert = CoordinateConvertor.LatLonToVector3(latlon[i].x, latlon[i].z, latlon[i].y, mb);
                Debug.Log("vertice: " + vertices[i] + " converted back: " + convVert);
            }
        }
    }

    public void InitVertices()
    {
        vertices = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
    }

    public void SetFirstVertex(Vector3 v0)
    {
        vertices[0] = v0;
        SetOppositeVertex(v0);
    }

    public void SetOppositeVertex(Vector3 v2)
    {
        var v0 = vertices[0];
        vertices[1] = new Vector3(v0.x, v0.y, v2.z);
        vertices[2] = v2;
        vertices[3] = new Vector3(v2.x, v0.y, v0.z);
    }

    public void SetVertex(int index, Vector3 v)
    {
        switch (index)
        {
            case 0:
                vertices[0] = v;
                vertices[1] = new Vector3(vertices[0].x, vertices[0].y, vertices[1].z);
                vertices[3] = new Vector3(vertices[3].x, vertices[0].y, vertices[0].z);
                break;
            case 1:
                vertices[1] = v;
                vertices[0] = new Vector3(vertices[1].x, vertices[0].y, vertices[0].z);
                vertices[2] = new Vector3(vertices[2].x, vertices[0].y, vertices[1].z);
                break;
            case 2:
                vertices[2] = v;
                vertices[1] = new Vector3(vertices[1].x, vertices[0].y, vertices[2].z);
                vertices[3] = new Vector3(vertices[2].x, vertices[0].y, vertices[3].z);
                break;
            case 3:
                vertices[3] = v;
                vertices[0] = new Vector3(vertices[0].x, vertices[0].y, vertices[3].z);
                vertices[2] = new Vector3(vertices[3].x, vertices[0].y, vertices[2].z);
                break;
        }
    }

    public float[] GetBounds()
    {
        return GetBounds(vertices);
    }

    public float[] GetBounds(Vector3[] vertices)
    {
        float minX = vertices[0].x;
        float maxX = vertices[0].x;
        float minZ = vertices[0].z;
        float maxZ = vertices[0].z;
        for (int i = 1; i < vertices.Length; i++)
        {
            var q = vertices[i];
            minX = Math.Min(q.x, minX);
            maxX = Math.Max(q.x, maxX);
            minZ = Math.Min(q.z, minZ);
            maxZ = Math.Max(q.z, maxZ);
        }
        return new float[] { minX, maxX, minZ, maxZ };
    }

    public List<Vector3> GetVertices()
    {
        return new List<Vector3>(vertices);
    }

    public int GetVerticeIndex(Vector3 testPoint)
    {
        if (vertices == null)
            return -1;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Vector3.Distance(testPoint, vertices[i]) <= gizmoRadius * 1.5f)
                return i;
        }
        return -1;
    }

    public bool CropAreaDefined()
    {
        if (vertices == null || vertices.Length == 0)
        {
            return false;
        }

        if (vertices[0].Equals(Vector3.zero) && vertices[0].Equals(vertices[2]))
        {
            return false;
        }

        return true;
    }

    public void OnDrawGizmos()
    {
        if (!CropAreaDefined())
            return;

        int j = vertices.Length - 1;
        for (int i = 0; i < vertices.Length; j = i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(vertices[i], gizmoRadius);
            Gizmos.DrawLine(vertices[i], vertices[j]);
        }
    }

    [Obsolete("Code for cropping with polygon, archived for reference.")]
    public void OnDrawGizmos_Polygon()
    {
        if (polygon != null && polygon.Count > 0)
        {
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; j = i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(polygon[i], gizmoRadius);
                Gizmos.DrawLine(polygon[i], polygon[j]);
            }
        }
    }

    public void ResetPolygon()
    {
        polygon = new List<Vector3>(4);
    }

    public void AddPoint(Vector3 point)
    {
        polygon.Add(point);
    }

    public int GetClosestPolygonPointIndex(Vector3 testPoint)
    {
        if (polygon == null)
            return -1;
        for (int i = 0; i < polygon.Count; i++)
        {
            if (Vector3.Distance(testPoint, polygon[i]) <= gizmoRadius * 1.5f)
                return i;
        }
        return -1;
    }

}
