/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Aram.OSMParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaPSLabs.Geometry
{
    public static class CoordinateConvertor
    {
        public enum OSMType { Node, Line, Polygon, Relation };
        public static float[] SimpleInterpolation(float PositionLat, float PositionLon, Bounds Boundings, float[] MinMaxX, float[] MinMaxY)
        {
            // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
            // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPi.xel)
            float Y = (float)((PositionLon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxY[1] - MinMaxY[0]));
            float X = (float)((PositionLat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxX[1] - MinMaxX[0]));
            return new float[] { X, Y };
        }
        public static GeometryUtility.CPoint2D[] ToCPoint2D(this Vector3[] points)
        {
            GeometryUtility.CPoint2D[] ret = new GeometryUtility.CPoint2D[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                ret[i] = new GeometryUtility.CPoint2D(points[i].x, points[i].z);
            }
            return ret;
        }
        public static float linear(float x, float x0, float x1, float y0, float y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
        public static Vector3[] ToVector3(this GeometryUtility.CPoint2D[] points, float yHeight)
        {
            Vector3[] ret = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                ret[i] = new Vector3((float)points[i].X, yHeight, (float)points[i].Y);
            }
            return ret;
        }
        public static Vector3 ToVector3(this GeometryUtility.CPoint2D point, float yHeight)
        {
            Vector3 ret = new Vector3((float)point.X, yHeight, (float)point.Y);
            return ret;
        }
        public static Vector3[] RemoveDuplicates(this Vector3[] array)
        {
            IEqualityComparer<Vector3> vector3Compare = new Vector3EqualityComparer();
            List<Vector3> result = new List<Vector3>();
            foreach (var a in array)
                if (!result.Contains(a, vector3Compare))
                    result.Add(a);
            return result.ToArray();
        }
        public static Vector3[] RemoveRedundantPointsInTheSameLines(this Vector3[] array, float Error = 0.001f)
        {
            List<Vector3> result = new List<Vector3>();
            if (array.Length < 3) return array;

            result.Add(array[0]);

            for (int i = 1; i < array.Length - 1; i++)
            {
                if (Math.Abs(array[i - 1].Slope(array[i]) - array[i].Slope(array[i + 1])) > Error)
                    result.Add(array[i]);
            }
            result.Add(array[array.Length - 1]);
            return result.ToArray();
        }
        public static bool CheckForDuplicates(this Vector3[] array)
        {
            IEqualityComparer<Vector3> vector3Compare = new Vector3EqualityComparer();
            return array.Count() != array.Distinct(vector3Compare).Count();
        }
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
                return value;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
                return value;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        public static Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var Vab = b - a;
            var Vbc = c - b;
            return Vector3.Cross(Vab, Vbc);
        }


        public static GameObject GenerateShapeUVedPlanar_Balanced(this PolygonCuttingEar.CPolygonShape shape, OSMType type, string Id, string other, string tag, string MaterialName = "Area", float Height = 0, bool GenerateColliders = true)
        {
            GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
            gameObject.tag = tag;

            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < shape.NumberOfPolygons; i++)
                vertices.AddRange(shape.Polygons(i).ToVector3(Height));

            // This is the starting index in the vertex list that belongs to the walls. 
            // We use this to distinguish between roof UVs and wall UVs.
            int WallStartIndex = vertices.Count;
            List<Vector2> UVs = new List<Vector2>();
            var minXRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.x).Min();
            var minZRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.z).Min();
            var maxXRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.x).Max();
            var maxZRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.z).Max();
            var average = new Vector3(
            vertices.GetRange(0, WallStartIndex).Select(s => s.x).Average(),
            vertices.GetRange(0, WallStartIndex).Select(s => s.y).Average(),
            vertices.GetRange(0, WallStartIndex).Select(s => s.z).Average());
            //var avgx = linear(average.x, minXRoof, maxXRoof, 0, 1);
            //var avgy = linear(average.z, minZRoof, maxZRoof, 0, 1);
            //Vector3[] OriginalVertices = shape.InputVertices.ToVector3(Height);
            for (int i = 0; i < WallStartIndex; i++)
            {
                // bool found = false;
                var x = linear(vertices[i].x, minXRoof, maxXRoof, 0, 1);
                var y = linear(vertices[i].z, minZRoof, maxZRoof, 0, 1);
                //for (int j = 0; j < OriginalVertices.Length; j++)
                //{
                //    if (Math.Abs(OriginalVertices[j].x - vertices[i].x) < 0.001f && Math.Abs(OriginalVertices[j].z - vertices[i].z) < 0.001f)
                //    {
                //        //if (j % 2 == 0)
                //        //    UVs.Add(new Vector2(0, 0));
                //        //else
                //        //    UVs.Add(new Vector2(1, 0));
                //        if (x > avgx && Math.Abs(x - avgx) < Math.Abs(y - avgy))
                //            x = 1;
                //        else if (x <= avgx && Math.Abs(avgx - x) < Math.Abs(y - avgy))
                //            x = 0;

                //        if (y > avgy && Math.Abs(y - avgy) < Math.Abs(x - avgx))
                //            y = 1;
                //        else if (y <= avgy && Math.Abs(avgy - y) < Math.Abs(x - avgx))
                //            y = 0;
                //        UVs.Add(new Vector2(x, y));
                //        found = true;
                //        break;
                //    }
                //}
                //if (!found)
                //{
                //    var x = linear(vertices[i].x, minXRoof, maxXRoof, 0, 1);
                //    var y = linear(vertices[i].z, minZRoof, maxZRoof, 0, 1);
                UVs.Add(new Vector2(x, y));
                //}
            }



            Vector2[] UVRef = new Vector2[]
			{
				new Vector2(0,0), // top left
				new Vector2(1,0), // top right
				new Vector2(0,1), // bottom left
				new Vector2(1,1) // bottom right
				// new Vector2(0,1), // bottom left
				// new Vector2(1,1) // bottom right
			};

            Vector3 HeightVector = new Vector3(0, 0, 0);

            int[] tris = new int[shape.NumberOfPolygons * 3];
            for (int i = 0; i < tris.Length; i++)
                tris[i] = i;

            gameObject.position = CoordinateConvertor.ChangePivot(ref vertices, CoordinateConvertor.PivotLocation.ZeroBottomCenter);
            gameObject.mesh = Rebuild(vertices.ToArray(), tris, UVs.ToArray());
            gameObject.material = new Material(MaterialName);

            return gameObject;
        }
        public static GameObject GenerateShapeUVedWithWalls_Balanced(this PolygonCuttingEar.CPolygonShape shape, OSMType type, string Id, string other, string tag, string MaterialName = "Building", float WallHeight = 5, float MaximumHeight = 12, bool GenerateColliders = true)
        {
            GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
            gameObject.tag = tag;

            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < shape.NumberOfPolygons; i++)
                vertices.AddRange(shape.Polygons(i).ToVector3(WallHeight));

            // This is the starting index in the vertex list that belongs to the walls. 
            // We use this to distinguish between roof UVs and wall UVs.
            int WallStartIndex = vertices.Count;
            List<Vector2> UVs = new List<Vector2>();
            var minXRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.x).Min();
            var minZRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.z).Min();
            var maxXRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.x).Max();
            var maxZRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.z).Max();
            // for (int i = 0; i < UVs.Length; i++)
            for (int i = 0; i < WallStartIndex; i++)
                UVs.Add(new Vector2(
                        linear(vertices[i].x, minXRoof, maxXRoof, 0, 1),
                        linear(vertices[i].z, minZRoof, maxZRoof, 0, 0.5f)
                ));


            Vector2[] UVRef = new Vector2[]
			{
				new Vector2(0,0.5f), // top left
				new Vector2(1,0.5f), // top right
				new Vector2(0,0.5f+0.5f*WallHeight/MaximumHeight), // bottom left
				new Vector2(1,0.5f+0.5f*WallHeight/MaximumHeight) // bottom right
				// new Vector2(0,1), // bottom left
				// new Vector2(1,1) // bottom right
			};
            Vector3[] OriginalVertices = shape.InputVertices.ToVector3(WallHeight);
            Vector3 HeightVector = new Vector3(0, WallHeight, 0);

            // Generating Walls - Total: (OriginalVertices.Length)*4*3
            for (int i = 0; i < OriginalVertices.Length - 1; i++)
            {
                Vector3 v1 = OriginalVertices[i];
                Vector3 v2 = OriginalVertices[i + 1];
                var dist = Vector3.Distance(v1, v2);

                //Debug.Log("Distance: "+dist);
                dist = dist.Clamp(0, 5);
                var UVratioTopRight = new Vector2(dist / 5, UVRef[1].y);
                var UVratioBottomRight = new Vector2(dist / 5, UVRef[3].y);

                Vector3 v3 = OriginalVertices[i] - HeightVector;
                Vector3 v4 = OriginalVertices[i + 1] - HeightVector;
                vertices.AddRange(new Vector3[] { v1, v2, v3 });
                UVs.Add(UVRef[0]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                UVs.Add(UVRef[2]);
                vertices.AddRange(new Vector3[] { v3, v2, v4 });
                UVs.Add(UVRef[2]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                UVs.Add(UVratioBottomRight); //UVs.Add(UVRef[3]);
                // Reverse
                vertices.AddRange(new Vector3[] { v1, v3, v2 });
                UVs.Add(UVRef[0]);
                UVs.Add(UVRef[2]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                vertices.AddRange(new Vector3[] { v3, v4, v2 });
                UVs.Add(UVRef[2]);
                UVs.Add(UVratioBottomRight); //UVs.Add(UVRef[3]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
            }

            // Last Wall - Total: 4*3
            Vector3 lv1 = OriginalVertices[OriginalVertices.Length - 1];
            Vector3 lv2 = OriginalVertices[0];
            var distlw = Vector3.Distance(lv1, lv2);
            //Debug.Log("Distance: "+distlw);
            distlw = distlw.Clamp(0, 5);
            var UVratioTopRightlw = new Vector2(distlw / 5, UVRef[1].y);
            var UVratioBottomRightlw = new Vector2(distlw / 5, UVRef[3].y);
            Vector3 lv3 = OriginalVertices[OriginalVertices.Length - 1] - HeightVector;
            Vector3 lv4 = OriginalVertices[0] - HeightVector;
            vertices.AddRange(new Vector3[] { lv1, lv2, lv3 });
            UVs.Add(UVRef[0]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);
            UVs.Add(UVRef[2]);
            vertices.AddRange(new Vector3[] { lv3, lv2, lv4 });
            UVs.Add(UVRef[2]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);
            UVs.Add(UVratioBottomRightlw); //UVs.Add(UVRef[3]);
            // Reverse
            vertices.AddRange(new Vector3[] { lv1, lv3, lv2 });
            UVs.Add(UVRef[0]);
            UVs.Add(UVRef[2]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);
            vertices.AddRange(new Vector3[] { lv3, lv4, lv2 });
            UVs.Add(UVRef[2]);
            UVs.Add(UVratioBottomRightlw); //UVs.Add(UVRef[3]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);



            int[] tris = new int[shape.NumberOfPolygons * 3 + (OriginalVertices.Length - 1) * 4 * 3 + 4 * 3];
            for (int i = 0; i < tris.Length; i++)
                tris[i] = i;

            gameObject.position = CoordinateConvertor.ChangePivot(ref vertices, CoordinateConvertor.PivotLocation.ZeroBottomCenter);
            gameObject.mesh = Rebuild(vertices.ToArray(), tris, UVs.ToArray());
            gameObject.material = new Material(MaterialName);

            return gameObject;
        }
        public static GameObject GenerateShapeUVedWithWalls_Balanced(this Poly2Tri.Polygon shape, OSMType type, string Id, string other, string tag, string MaterialName = "Building", float WallHeight = 5, float MaximumHeight = 12, bool GenerateColliders = true)
        {
            GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
            gameObject.tag = tag;
            if (shape.Triangles == null)
                return gameObject;
            if (shape.Triangles.Count == 0)
                return gameObject;
            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < shape.Triangles.Count; i++)
                vertices.AddRange(shape.Triangles[i].Points.Select(s => new Vector3((float)s.X, WallHeight, (float)s.Y)).Reverse());

            // This is the starting index in the vertex list that belongs to the walls. 
            // We use this to distinguish between roof UVs and wall UVs.
            int WallStartIndex = vertices.Count;
            List<Vector2> UVs = new List<Vector2>();
            var minXRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.x).Min();
            var minZRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.z).Min();
            var maxXRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.x).Max();
            var maxZRoof = vertices.GetRange(0, WallStartIndex).Select(i => i.z).Max();
            // for (int i = 0; i < UVs.Length; i++)
            for (int i = 0; i < WallStartIndex; i++)
                UVs.Add(new Vector2(
                        linear(vertices[i].x, minXRoof, maxXRoof, 0, 1),
                        linear(vertices[i].z, minZRoof, maxZRoof, 0, 0.5f)
                ));


            Vector2[] UVRef = new Vector2[]
			{
				new Vector2(0,0.5f), // top left
				new Vector2(1,0.5f), // top right
				new Vector2(0,0.5f+0.5f*WallHeight/MaximumHeight), // bottom left
				new Vector2(1,0.5f+0.5f*WallHeight/MaximumHeight) // bottom right
				// new Vector2(0,1), // bottom left
				// new Vector2(1,1) // bottom right
			};
            #region Outer wall
            Vector3 HeightVector = new Vector3(0, WallHeight, 0);
            Vector3[] OriginalVertices = shape.Points.Select(s => new Vector3((float)s.X, WallHeight, (float)s.Y)).ToArray();


            // Generating Walls - Total: (OriginalVertices.Length)*4*3
            for (int i = 0; i < OriginalVertices.Length - 1; i++)
            {
                Vector3 v1 = OriginalVertices[i];
                Vector3 v2 = OriginalVertices[i + 1];
                var dist = Vector3.Distance(v1, v2);

                //Debug.Log("Distance: "+dist);
                dist = dist.Clamp(0, 5);
                var UVratioTopRight = new Vector2(dist / 5, UVRef[1].y);
                var UVratioBottomRight = new Vector2(dist / 5, UVRef[3].y);

                Vector3 v3 = OriginalVertices[i] - HeightVector;
                Vector3 v4 = OriginalVertices[i + 1] - HeightVector;
                vertices.AddRange(new Vector3[] { v1, v2, v3 });
                UVs.Add(UVRef[0]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                UVs.Add(UVRef[2]);
                vertices.AddRange(new Vector3[] { v3, v2, v4 });
                UVs.Add(UVRef[2]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                UVs.Add(UVratioBottomRight); //UVs.Add(UVRef[3]);
                // Reverse
                vertices.AddRange(new Vector3[] { v1, v3, v2 });
                UVs.Add(UVRef[0]);
                UVs.Add(UVRef[2]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                vertices.AddRange(new Vector3[] { v3, v4, v2 });
                UVs.Add(UVRef[2]);
                UVs.Add(UVratioBottomRight); //UVs.Add(UVRef[3]);
                UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
            }

            // Last Wall - Total: 4*3
            Vector3 lv1 = OriginalVertices[OriginalVertices.Length - 1];
            Vector3 lv2 = OriginalVertices[0];
            var distlw = Vector3.Distance(lv1, lv2);
            //Debug.Log("Distance: "+distlw);
            distlw = distlw.Clamp(0, 5);
            var UVratioTopRightlw = new Vector2(distlw / 5, UVRef[1].y);
            var UVratioBottomRightlw = new Vector2(distlw / 5, UVRef[3].y);
            Vector3 lv3 = OriginalVertices[OriginalVertices.Length - 1] - HeightVector;
            Vector3 lv4 = OriginalVertices[0] - HeightVector;
            vertices.AddRange(new Vector3[] { lv1, lv2, lv3 });
            UVs.Add(UVRef[0]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);
            UVs.Add(UVRef[2]);
            vertices.AddRange(new Vector3[] { lv3, lv2, lv4 });
            UVs.Add(UVRef[2]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);
            UVs.Add(UVratioBottomRightlw); //UVs.Add(UVRef[3]);
            // Reverse
            vertices.AddRange(new Vector3[] { lv1, lv3, lv2 });
            UVs.Add(UVRef[0]);
            UVs.Add(UVRef[2]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);
            vertices.AddRange(new Vector3[] { lv3, lv4, lv2 });
            UVs.Add(UVRef[2]);
            UVs.Add(UVratioBottomRightlw); //UVs.Add(UVRef[3]);
            UVs.Add(UVratioTopRightlw); //UVs.Add(UVRef[1]);

            #endregion

            #region Inner walls
            List<Vector3[]> OriginalInnerVertices = new List<Vector3[]>();
            if (shape.Holes != null)
            {
                for (int i = 0; i < shape.Holes.Count; i++)
                {
                    OriginalInnerVertices.Add(shape.Holes[i].Points.Select(s => new Vector3((float)s.X, WallHeight, (float)s.Y)).ToArray());
                }

                for (int holeidx = 0; holeidx < shape.Holes.Count; holeidx++)
                {
                    // Generating Walls - Total: (OriginalInnerVertices.Length)*4*3
                    for (int i = 0; i < OriginalInnerVertices[holeidx].Length - 1; i++)
                    {
                        Vector3 v1 = OriginalInnerVertices[holeidx][i];
                        Vector3 v2 = OriginalInnerVertices[holeidx][i + 1];
                        var dist = Vector3.Distance(v1, v2);

                        //Debug.Log("Distance: "+dist);
                        dist = dist.Clamp(0, 5);
                        var UVratioTopRight = new Vector2(dist / 5, UVRef[1].y);
                        var UVratioBottomRight = new Vector2(dist / 5, UVRef[3].y);

                        Vector3 v3 = OriginalInnerVertices[holeidx][i] - HeightVector;
                        Vector3 v4 = OriginalInnerVertices[holeidx][i + 1] - HeightVector;
                        vertices.AddRange(new Vector3[] { v1, v2, v3 });
                        UVs.Add(UVRef[0]);
                        UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                        UVs.Add(UVRef[2]);
                        vertices.AddRange(new Vector3[] { v3, v2, v4 });
                        UVs.Add(UVRef[2]);
                        UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                        UVs.Add(UVratioBottomRight); //UVs.Add(UVRef[3]);
                        // Reverse
                        vertices.AddRange(new Vector3[] { v1, v3, v2 });
                        UVs.Add(UVRef[0]);
                        UVs.Add(UVRef[2]);
                        UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                        vertices.AddRange(new Vector3[] { v3, v4, v2 });
                        UVs.Add(UVRef[2]);
                        UVs.Add(UVratioBottomRight); //UVs.Add(UVRef[3]);
                        UVs.Add(UVratioTopRight); //UVs.Add(UVRef[1]);
                    }

                    // Last Wall - Total: 4*3
                    Vector3 lv1_hole = OriginalInnerVertices[holeidx][OriginalInnerVertices[holeidx].Length - 1];
                    Vector3 lv2_hole = OriginalInnerVertices[holeidx][0];
                    var distlw_hole = Vector3.Distance(lv1_hole, lv2_hole);
                    //Debug.Log("Distance: "+distlw_hole);
                    distlw_hole = distlw_hole.Clamp(0, 5);
                    var UVratioTopRightlw_hole = new Vector2(distlw_hole / 5, UVRef[1].y);
                    var UVratioBottomRightlw_hole = new Vector2(distlw_hole / 5, UVRef[3].y);
                    Vector3 lv3_hole = OriginalInnerVertices[holeidx][OriginalInnerVertices[holeidx].Length - 1] - HeightVector;
                    Vector3 lv4_hole = OriginalInnerVertices[holeidx][0] - HeightVector;
                    vertices.AddRange(new Vector3[] { lv1_hole, lv2_hole, lv3_hole });
                    UVs.Add(UVRef[0]);
                    UVs.Add(UVratioTopRightlw_hole); //UVs.Add(UVRef[1]);
                    UVs.Add(UVRef[2]);
                    vertices.AddRange(new Vector3[] { lv3_hole, lv2_hole, lv4_hole });
                    UVs.Add(UVRef[2]);
                    UVs.Add(UVratioTopRightlw_hole); //UVs.Add(UVRef[1]);
                    UVs.Add(UVratioBottomRightlw_hole); //UVs.Add(UVRef[3]);
                    // Reverse
                    vertices.AddRange(new Vector3[] { lv1_hole, lv3_hole, lv2_hole });
                    UVs.Add(UVRef[0]);
                    UVs.Add(UVRef[2]);
                    UVs.Add(UVratioTopRightlw_hole); //UVs.Add(UVRef[1]);
                    vertices.AddRange(new Vector3[] { lv3_hole, lv4_hole, lv2_hole });
                    UVs.Add(UVRef[2]);
                    UVs.Add(UVratioBottomRightlw_hole); //UVs.Add(UVRef[3]);
                    UVs.Add(UVratioTopRightlw_hole); //UVs.Add(UVRef[1]);
                }
            }
            #endregion

            // int[] tris = new int[shape.Triangles.Count * 3 + (OriginalVertices.Length - 1) * 4 * 3 + 4 * 3];
            int innerTriCount = 0;
            for (int i = 0; i < OriginalInnerVertices.Count; i++)
            {
                innerTriCount += (OriginalInnerVertices[i].Length - 1) * 4 * 3 + 4 * 3;
            }
            int[] tris = new int[shape.Triangles.Count * 3 + (OriginalVertices.Length - 1) * 4 * 3 + 4 * 3 + innerTriCount];
            for (int i = 0; i < tris.Length; i++)
                tris[i] = i;

            gameObject.position = CoordinateConvertor.ChangePivot(ref vertices, CoordinateConvertor.PivotLocation.ZeroBottomCenter);
            gameObject.mesh = Rebuild(vertices.ToArray(), tris, UVs.ToArray());
            gameObject.material = new Material(MaterialName);

            return gameObject;

        }
        public static GameObject CombineGameObjectsOfTheSameMaterial(this List<GameObject> GameObjects)
        {
            if (GameObjects.Count == 1)
                return GameObjects[0];

            List<Vector3> CombinedVertices = new List<Vector3>();
            List<int> CombinedTris = new List<int>();
            List<Vector2> CombinedUVs = new List<Vector2>();

            foreach (var go in GameObjects)
            {
                var vertLength = CombinedVertices.Count;
                CombinedVertices.AddRange(go.mesh.vertices.Select(v => v + go.position));
                CombinedTris.AddRange(go.mesh.triangles.Select(v => v + vertLength));
                CombinedUVs.AddRange(go.mesh.uv);
            }
            GameObject CombinedObject = new GameObject(GameObjects[0].Name);
            CombinedObject.material = GameObjects[0].material;
            CombinedObject.tag = GameObjects[0].tag;
            CombinedObject.type = GameObjects[0].type;

            CombinedObject.position = CoordinateConvertor.ChangePivot(ref CombinedVertices, CoordinateConvertor.PivotLocation.ZeroBottomCenter);
            CombinedObject.mesh = Rebuild(CombinedVertices.ToArray(), CombinedTris.ToArray(), CombinedUVs.ToArray());
            return CombinedObject;
        }
        public static Vector3[] ToSegmentedPoints(this Vector3[] OriginalPoints, float Precision)
        {
            return SegmentPoints(OriginalPoints, Precision);
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
                        tempNoOfSegments = (int)Math.Floor(distance);
                    else
                        tempNoOfSegments = (int)Math.Floor(distance / precision);
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
        public static Vector3[] MakeQuad(this Vector3 s, Vector3 e, float w)
        {
            var L = (float)Math.Sqrt((s.x - e.x) * (s.x - e.x) + (s.z - e.z) * (s.z - e.z));

            var offsetPixels = w;

            var x1p1 = s.x + offsetPixels * (e.z - s.z) / L;
            var x2p1 = e.x + offsetPixels * (e.z - s.z) / L;
            var y1p1 = s.z + offsetPixels * (s.x - e.x) / L;
            var y2p1 = e.z + offsetPixels * (s.x - e.x) / L;
            // This is the second line
            var x1p = s.x - offsetPixels * (e.z - s.z) / L;
            var x2p = e.x - offsetPixels * (e.z - s.z) / L;
            var y1p = s.z - offsetPixels * (s.x - e.x) / L;
            var y2p = e.z - offsetPixels * (s.x - e.x) / L;
            Vector3[] q = new Vector3[4];

            q[0] = new Vector3(x1p1, e.y, y1p1);
            q[1] = new Vector3(x1p, e.y, y1p);
            q[2] = new Vector3(x2p1, e.y, y2p1);
            q[3] = new Vector3(x2p, e.y, y2p);
            return q;
        }
        public static float Det(this float[] matrix3x3)
        {
            if (matrix3x3.Length != 9)
                throw new InvalidOperationException("Matrix should be of size 9");
            var a11 = matrix3x3[0];
            var a12 = matrix3x3[1];
            var a13 = matrix3x3[2];
            var a21 = matrix3x3[3];
            var a22 = matrix3x3[4];
            var a23 = matrix3x3[5];
            var a31 = matrix3x3[6];
            var a32 = matrix3x3[7];
            var a33 = matrix3x3[8];
            // |M| = a11*((a22 * a33) - (a23*a32)) - a12*((a21*a33) - (a23*a31)) + a13*((a21*a32) - (a22*a31))
            var result = a11 * ((a22 * a33) - (a23 * a32)) - a12 * ((a21 * a33) - (a23 * a31)) + a13 * ((a21 * a32) - (a22 * a31));
            return result;
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

        }
        public static GameObject MeshGenerationFilledCorners(Vector3[] Points, float Width, OSMType type, string Id, string other, string tag, string MaterialName, float height)
        {
            GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
            gameObject.material = new Material(MaterialName);

            if (!string.IsNullOrEmpty(tag))
                gameObject.tag = tag;


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
                AllQuads[i] = MakeQuad(Points[i], Points[i + 1], Width);
                AllQuads[i][0].y = height;
                AllQuads[i][1].y = height;
                AllQuads[i][2].y = height;
                AllQuads[i][3].y = height;
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
							// First Quad (BOTTOM)
                                uvTopLeft,uvTopRight,uvBottomRight,
                                uvTopLeft,uvBottomRight,uvBottomLeft,
                                //Reverse (TOP)
                                uvTopRight,uvTopLeft,uvBottomRight,
                                uvTopLeft,uvBottomLeft,uvBottomRight
                                
				});
            }


            int[] tris = new int[vertices.Count];
            for (int i = 0; i < tris.Length; i++)
                tris[i] = i;


            // Pivot on center of bounding box.
            gameObject.position = ChangePivot(ref vertices, PivotLocation.Center);
            ////

            gameObject.mesh = Rebuild(vertices.ToArray(), tris, UVs.ToArray());
            return gameObject;

        }
        public enum PivotLocation { Center, MiddleVector, ZeroBottomCenter };
        public static Vector3 ChangePivot(ref List<Vector3> vertices, PivotLocation pivotLocation)
        {
            var pivotxMin = vertices.Select(i => i.x).Min();
            var pivotzMin = vertices.Select(i => i.z).Min();
            var pivotyMin = vertices.Select(i => i.y).Min();
            var pivotxMax = vertices.Select(i => i.x).Max();
            var pivotzMax = vertices.Select(i => i.z).Max();
            var pivotyMax = vertices.Select(i => i.y).Max();
            var PivotPoint = new Vector3((pivotxMin + pivotxMax) / 2, (pivotyMin + pivotyMax) / 2, (pivotzMin + pivotzMax) / 2);
            if (pivotLocation == PivotLocation.ZeroBottomCenter)
                PivotPoint.y = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= PivotPoint;
            }
            return PivotPoint;
        }
        public static Mesh Rebuild(Vector3[] Vertices, int[] Triangles, Vector2[] UV, bool GenerateTangents = true)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = Vertices;
            mesh.triangles = Triangles;
            mesh.uv = UV;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            if (GenerateTangents)
                mesh.CalculateMeshTangents();
            return mesh;
        }

    }

    public class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {

        public bool Equals(Vector3 b1, Vector3 b2)
        {
            if (b1.x == b2.x && b1.y == b2.y && b1.z == b2.z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(Vector3 bx)
        {
            int hCode = bx.GetHashCode();
            return hCode.GetHashCode();
        }

    }
}
