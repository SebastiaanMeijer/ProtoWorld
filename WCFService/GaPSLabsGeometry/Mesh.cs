/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaPSLabs.Geometry
{
    public class Mesh
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uv;
        public Vector3[] normals;
        public Vector4[] tangents;
        public void RecalculateNormals()
        {
            normals = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length - 2; i += 3)
            {
                var temp =
                    CoordinateConvertor.CalculateNormal(
                    vertices[triangles[i]], vertices[triangles[i  + 1]], vertices[triangles[i  + 2]]
                    );
                normals[i ] = temp;
                normals[i  + 1] = temp;
                normals[i  + 2] = temp;
            }
        }
        public void RecalculateBounds() { }
        public void CalculateMeshTangents()
        {
          
            //speed up math by copying the mesh arrays
            //int[] triangles = mesh.triangles;
            //Vector3[] vertices = mesh.vertices;
            //Vector2[] uv = mesh.uv;
            //Vector3[] normals = mesh.normals;

            //variable definitions
            int triangleCount = triangles.Length;
            int vertexCount = vertices.Length;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            tan1 = Enumerable.Repeat<Vector3>(Vector3.zero, vertexCount).ToArray();
            tan2 = Enumerable.Repeat<Vector3>(Vector3.zero, vertexCount).ToArray();

            Vector4[] tangents = new Vector4[vertexCount];
            tangents = Enumerable.Repeat<Vector4>(Vector4.zero, vertexCount).ToArray();

            for (long a = 0; a < triangleCount; a += 3)
            {
                long i1 = triangles[a + 0];
                long i2 = triangles[a + 1];
                long i3 = triangles[a + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }


            for (long a = 0; a < vertexCount; ++a)
            {
                Vector3 n = normals[a];
                Vector3 t = tan1[a];

                //Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
                //tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);

                //TODO
                //Vector3.OrthoNormalize(ref n, ref t);
                tangents[a]= Vector3.Normalize(t - n * Vector3.Dot(n, t));

                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }

        }
    }
}
