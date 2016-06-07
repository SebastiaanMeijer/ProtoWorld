//#define MYDEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GapslabWCFservice;
using GeometryUtility;
using System;

public static class Interpolations : object
{
    /// <summary>
    /// Calls Debug.Log() method internally, and can be disabled from Interpolations.cs by modifying MYDEBUG 
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    public static void MyLog(this object message)
    {
#if MYDEBUG
        Debug.Log(message);
#endif
    }
    /// <summary>
    /// Calls Debug.DrawRay() method internally, and can be disabled from Interpolations.cs by modifying MYDEBUG 
    /// </summary>
    /// <param name="start">Origin of the ray</param>
    /// <param name="end">Destination of the ray</param>
    /// <param name="color">Color of the ray</param>
    public static void MyDrawRay(Vector3 start, Vector3 end, Color color)
    {
#if MYDEBUG
        Debug.DrawRay(start, end, color);
#endif
    }
    /// <summary>
    /// Checks whether x,y,z component of the given vector is NaN (Not a Number)
    /// </summary>
    /// <param name="vector">The vector to be checked.</param>
    /// <returns>Return true if there is a Nan, false otherwise.</returns>
    public static bool CheckForNoNaN(this Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }
    /// <summary>
    /// Converts the distance between two points to meters.
    /// </summary>
    /// <param name="lat1">Latitude of the first point</param>
    /// <param name="lon1">Longitude of the first point</param>
    /// <param name="lat2">Latitude of the second point</param>
    /// <param name="lon2">Longitude of the second point</param>
    /// <returns>Returns the distance in meters.</returns>
    public static double GeoDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6378.137; // Radius of earth in KM
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c;
        return d * 1000; // meters
    }
    /// <summary>
    /// A generic extension of 'ForEach' to perform an action on all members of an enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration</typeparam>
    /// <param name="source">The source of enumeration</param>
    /// <param name="action">The action to be performed on the elements</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T element in source)
        {
            action(element);
        }
    }
    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
    /// <summary>
    /// A generic extension of 'For' to perform an action on all members of an array of type T.
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="value">The array</param>
    /// <param name="action">The action to be performed on the elements</param>
    public static void For<T>(this T[] value, Action<int, T> action)
    {
        for (int i = 0; i < value.Length; i++)
        {
            action(i, value[i]);
        }
    }
    /// <summary>
    /// Removes the all duplicate elements in the given array. 
    /// <para><strong>Note:</string>If you want only the consecutive duplicates to be removed, use <see cref="RemoveConsecutiveDuplicates"/> method.</para>
    /// </summary>
    /// <param name="array">The array to remove duplicates form</param>
    /// <returns>Returns a Vector3 array without duplicates.</returns>
    /// <seealso cref="RemoveConsecutiveDuplicates"/>
    public static Vector3[] RemoveDuplicates(this Vector3[] array)
    {
        List<Vector3> result = new List<Vector3>();
        foreach (var a in array)
            if (!result.Exists(r => r.EqualsManual(a)))
                result.Add(a);
        return result.ToArray();
    }
    /// <summary>
    /// Removes all the consecutive duplicate elements in the given array.
    /// </summary>
    /// <param name="array">The array to remove duplicates form</param>
    /// <param name="errorThreshold">The error threshold to tolerate when comparing values. 0 means only exact values are marked as duplicates.</param>
    /// <returns>Returns a Vector3 array without consecutive duplicates.</returns>
    public static Vector3[] RemoveConsecutiveDuplicates(this Vector3[] array, float errorThreshold = 0)
    {
        List<Vector3> result = new List<Vector3>();
        if (array.Length > 0)
        {
            result.Add(array[0]);
            if (errorThreshold == 0)
                for (int i = 1; i < array.Length; i++)
                {
                    if (!result[result.Count - 1].EqualsManual(array[i]))
                        result.Add(array[i]);
                }
            else
                for (int i = 1; i < array.Length; i++)
                {
                    if (!result[result.Count - 1].EqualsManual(array[i], true, errorThreshold))
                        result.Add(array[i]);
                }
            return result.ToArray();
        }
        else return array;
    }
    /// <summary>
    /// Checks whether two vectors have equal x,y,z components, with the option of tolerating small differences.
    /// </summary>
    /// <param name="a">The first vector</param>
    /// <param name="b">The second vector</param>
    /// <param name="tolerateError">Specifies whether the error toleration should be considered. The default value is false.</param>
    /// <param name="errorThreshold">The error threshold to tolerate when comparing values. The default value is 0.01f.</param>
    /// <returns>Returns true if equal, and false otherwise.</returns>
    public static bool EqualsManual(this Vector3 a, Vector3 b, bool tolerateError = false, float errorThreshold = 0.01f)
    {
        //return (Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z));
        if (tolerateError)
            return (Mathf.Abs(a.x - b.x) <= errorThreshold && Mathf.Abs(a.y - b.y) <= errorThreshold && Mathf.Abs(a.z - b.z) <= errorThreshold);
        else
            return a.x == b.x && a.y == b.y && a.z == b.z;
    }
    /// <summary>
    /// Checks if there are any duplicates in the array, using <see cref="Vector3EqualityComparer"/>.
    /// </summary>
    /// <param name="array">The array to check for duplicates</param>
    /// <returns>Returns true if any duplicates were found, and false otherwise.</returns>
    /// <seealso cref="Vector3EqualityComparer"/>
    /// <seealso cref="RemoveDuplicates"/>
    /// <seealso cref="RemoveConsecutiveDuplicates"/>
    public static bool CheckForDuplicates(this Vector3[] array)
    {
        IEqualityComparer<Vector3> vector3Compare = new Vector3EqualityComparer();
        return !(array.Count() == array.Distinct(vector3Compare).Count());
    }
    /// <summary>
    /// Calculates the distance between the nodes after converting them to Vector3 coordinates.
    /// </summary>
    /// <param name="node1">The first node</param>
    /// <param name="node2">The second node</param>
    /// <returns>Returns the distance in Unity coordinates.</returns>
    /// <seealso cref="Vector3.Distance"/>
    public static float Distance(this GapslabWCFservice.OsmNodeWCF node1, GapslabWCFservice.OsmNodeWCF node2)
    {
        return Vector3.Distance(
            CoordinateConvertor.LatLonToVector3(node1.lat, node1.lon)
            , CoordinateConvertor.LatLonToVector3(node1.lat, node1.lon));
    }
    /// <summary>
    /// Projects a point to a line that is formed by two points.
    /// </summary>
    /// <param name="toProject">The point to project</param>
    /// <param name="linepoint1">Starting point of the line</param>
    /// <param name="linepoint2">Ending point of the line</param>
    /// <returns>Returns the projected point.</returns>
    public static Vector3 ProjectToLine(this Vector3 toProject, Vector3 linepoint1, Vector3 linepoint2)
    {
        float m = (linepoint2.z - linepoint1.z) / (linepoint2.x - linepoint1.x);
        float b = linepoint1.z - (m * linepoint1.x);

        float x = (m * toProject.z + toProject.x - m * b) / (m * m + 1);
        float z = (m * m * toProject.z + m * toProject.x + b) / (m * m + 1);

        return new Vector3(x, linepoint1.y, z);
    }
    /// <summary>
    /// Checks whether a point is in the bounding box formed by two other points.
    /// <para>Notes that the y component is ignored.</para>
    /// </summary>
    /// <param name="Point">The point to be checked</param>
    /// <param name="bound1">Lowest point of the bounding box</param>
    /// <param name="bound2">Highest point of the bounding box</param>
    /// <returns>Returns true if it fall in the bounding box, and false otherwise.</returns>
    public static bool IsInRangeIgnoreY(this Vector3 Point, Vector3 bound1, Vector3 bound2)
    {
        float xMin = Mathf.Min(bound1.x, bound2.x);
        float zMin = Mathf.Min(bound1.z, bound2.z);
        float xMax = Mathf.Max(bound1.x, bound2.x);
        float zMax = Mathf.Max(bound1.z, bound2.z);
        //Interpolations.MyLog("x min max: " + Point.x + "->? " + xMin + " " + xMax + "\t" + "z min max: " + Point.z + "->? " + zMin + " " + zMax + "\t"
        //	+ (Point.x >= xMin && Point.x <= xMax && Point.z >= zMin && Point.z <= zMax));
        if (Point.x >= xMin && Point.x <= xMax && Point.z >= zMin && Point.z <= zMax)
            return true;
        else return false;
    }
    /// <summary>
    /// Vector3EqualityComparer class provides a comparison of Vector3's x,y,z components.
    /// </summary>
    /// <seealso cref="CheckForDuplicates"/>
    public class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {
        /// <summary>
        /// Check whether two Vector3's x,y,z components are equal.
        /// </summary>
        /// <param name="b1">The first vector</param>
        /// <param name="b2">The second vector</param>
        /// <returns>Returns true if equal, and false otherwise.</returns>
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

        /// <summary>
        /// Returns a unique hash for the given Vector3.
        /// </summary>
        /// <param name="bx">The vector to be hashed</param>
        /// <returns>Returns the hash of the vector.</returns>
        public int GetHashCode(Vector3 bx)
        {
            int hCode = bx.GetHashCode();
            return hCode.GetHashCode();
        }

    }
    /// <summary>
    /// Calculates the slope of a line that is formed by two points.
    /// </summary>
    /// <param name="linepoint1">The starting point</param>
    /// <param name="linepoint2">The ending point</param>
    /// <returns>Returns the slope.</returns>
    public static float Slope(this Vector3 linepoint1, Vector3 linepoint2)
    {
        return (linepoint2.z - linepoint1.z) / (linepoint2.x - linepoint1.x);
    }
    /// <summary>
    /// Calculates the determinant of a 3-by-3 matrix.
    /// </summary>
    /// <param name="matrix3x3">The matrix to be used</param>
    /// <returns>Returns the determinant value.</returns>
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
    /// <summary>
    /// Converts a Latitude/Longitude value to the Unity coordinates.
    /// </summary>
    /// <param name="PositionLat">The latitude value</param>
    /// <param name="PositionLon">The longitude value</param>
    /// <param name="Boundings">The range of the latitude/longitude values</param>
    /// <param name="MinMaxX">The range of X component in Unity coordinates</param>
    /// <param name="MinMaxY">The range of Y component in Unity coordinates</param>
    /// <returns>Returns an array of float containing the converted values. index 0 is X, index 1 is Y.</returns>
    public static float[] SimpleInterpolation(float PositionLat, float PositionLon, GapslabWCFservice.BoundsWCF Boundings, float[] MinMaxX, float[] MinMaxY)
    {
        // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
        // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPi.xel)
        float Y = (float)((PositionLon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxY[1] - MinMaxY[0]));
        float X = (float)((PositionLat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxX[1] - MinMaxX[0]));
        return new float[] { X, Y };
    }
    /// <summary>
    /// Converts a Latitude/Longitude value to the Unity coordinates.
    /// </summary>
    /// <param name="PositionLat">The latitude value</param>
    /// <param name="PositionLon">The longitude value</param>
    /// <param name="Boundings">The range of the latitude/longitude values, passed by reference.</param>
    /// <param name="MinMaxX">The range of X component in Unity coordinates</param>
    /// <param name="MinMaxY">The range of Y component in Unity coordinates</param>
    /// <returns>Returns an array of float containing the converted values. Index 0 is X, index 1 is Y.</returns>
    public static float[] SimpleInterpolation(float PositionLat, float PositionLon, ref GapslabWCFservice.BoundsWCF Boundings, float[] MinMaxX, float[] MinMaxY)
    {
        // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
        // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPi.xel)
        float Y = (float)((PositionLon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxY[1] - MinMaxY[0]));
        float X = (float)((PositionLat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxX[1] - MinMaxX[0]));
        return new float[] { X, Y };
    }
    //public static float[] SimpleInterpolation(float PositionLat, float PositionLon, Aram.OSMParser.Bounds Boundings, float[] MinMaxX, float[] MinMaxY)
    //{
    //    // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
    //    // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPixel)
    //    float Y = (float)((PositionLon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxY[1] - MinMaxY[0]));
    //    float X = (float)((PositionLat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxX[1] - MinMaxX[0]));
    //    return new float[] { X, Y };
    //}

    /// <summary>
    /// Converts a Latitude/Longitude array of OSM nodes to the Unity coordinates.
    /// </summary>
    /// <param name="sourceNodes">The source array</param>
    /// <param name="Source">0:minlat, 1:maxlat, 2:minlon, 3:maxlon</param>
    /// <param name="Target">0:minX, 1:maxX, 2:minY, 3:maxY</param>
    /// <returns>Returns a list of float array containing the converted values. Index 0 is X, index 1 is Y.</returns>
    public static List<float[]> ToInterpolatedPoints(this OsmNodeWCF[] sourceNodes, float[] Source, float[] Target)
    {
        List<float[]> ret = new List<float[]>();
        int count = sourceNodes.Length;
        for (int i = 0; i < count; i++)
        {
            ret.Add(new float[]{
                    linear((float)sourceNodes[i].lat,Source[0],Source[1],Target[0],Target[1]),
                    linear((float)sourceNodes[i].lon,Source[2],Source[3],Target[2],Target[3])
                });
            //ret[i] = SimpleInterpolation((float)sourceNodes[i].lat, (float)sourceNodes[i].lon, ref Source, ref Target);
        }
        return ret;
    }
    /// <summary>
    /// Converts a Latitude/Longitude array of OSM nodes to the Unity coordinates.
    /// </summary>
    /// <param name="sourceNodes">The source array</param>
    /// <param name="Source">0:minlat, 1:maxlat, 2:minlon, 3:maxlon</param>
    /// <param name="Target">0:minX, 1:maxX, 2:minY, 3:maxY</param>
    /// <returns>Returns a list of Vector2 elements containing the converted values.</returns>
    public static List<Vector2> ToInterpolatedPointsF(this  OsmNodeWCF[] sourceNodes, float[] Source, float[] Target)
    {
        List<Vector2> ret = new List<Vector2>();
        int count = sourceNodes.Length;
        for (int i = 0; i < count; i++)
        {
            ret.Add(new Vector2(
                    linear((float)sourceNodes[i].lat, Source[0], Source[1], Target[0], Target[1]),
                    linear((float)sourceNodes[i].lon, Source[2], Source[3], Target[2], Target[3])
                    ));

            //var temp=SimpleInterpolation((float)sourceNodes[i].lat, (float)sourceNodes[i].lon, ref Source, ref Target);
            //ret[i] = new PointF(temp[0], temp[1]);
        }
        return ret;
    }
    /// <summary>
    /// Converts an array of OSM nodes to a list of <see cref="GeometryUtility.CPoint2D"/> objects.
    /// </summary>
    /// <param name="sourceNodes">The source array</param>
    /// <param name="Source">0:minlat, 1:maxlat, 2:minlon, 3:maxlon</param>
    /// <param name="Target">0:minX, 1:maxX, 2:minY, 3:maxY</param>
    /// <returns>Returns a list of CPoint2D elements containing the converted values.</returns>
    public static List<GeometryUtility.CPoint2D> ToInterpolatedPointsCPoint2D(this OsmNodeWCF[] sourceNodes, float[] Source, float[] Target)
    {
        List<GeometryUtility.CPoint2D> ret = new List<GeometryUtility.CPoint2D>();
        int count = sourceNodes.Length;
        for (int i = 0; i < count; i++)
        {
            ret.Add(new GeometryUtility.CPoint2D(
                    linear((float)sourceNodes[i].lat, Source[0], Source[1], Target[0], Target[1]),
                    linear((float)sourceNodes[i].lon, Source[2], Source[3], Target[2], Target[3])
                    ));

            //var temp=SimpleInterpolation((float)sourceNodes[i].lat, (float)sourceNodes[i].lon, ref Source, ref Target);
            //ret[i] = new PointF(temp[0], temp[1]);
        }
        return ret;
    }
    [Obsolete("This method is obsolete and will be removed in the future. Use CoordinateConvertor class methods instead.")]
    public static List<GeometryUtility.CPoint2D> ToInterpolatedPointsCPoint2D(this OsmNodeWCF[] sourceNodes, float[] Source, float[] Target, Vector3 MinPointOnMap, Vector2 Scale)
    {
        throw new System.NotImplementedException("Replace this by the correct method from CoordinateConvertor class");
        List<GeometryUtility.CPoint2D> ret = new List<GeometryUtility.CPoint2D>();
        int count = sourceNodes.Length;
        for (int i = 0; i < count; i++)
        {
            float[] point = new float[2];
            point[0] = linear((float)sourceNodes[i].lat, Source[0], Source[1], Target[0], Target[1]);
            point[1] = linear((float)sourceNodes[i].lon, Source[2], Source[3], Target[2], Target[3]);

            point[0] -= MinPointOnMap.x;
            point[1] -= MinPointOnMap.z;
            point[0] *= Scale.x;
            point[1] *= Scale.y;
            ret.Add(new GeometryUtility.CPoint2D(point[0], point[1]));
        }
        return ret;
    }
    /// <summary>
    /// Converts an array of GeometryUtility.CPoint2D to an array of Vector2.
    /// </summary>
    /// <param name="points">The array points to be converted</param>
    /// <returns>Returns an array of Vector2 elements</returns>
    /// <seealso cref="ToCPoint2D"/>
    /// <seealso cref="ToVector3"/>
    public static Vector2[] ToVector2(this GeometryUtility.CPoint2D[] points)
    {
        Vector2[] ret = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            ret[i] = new Vector2((float)points[i].X, (float)points[i].Y);
        }
        return ret;
    }
    /// <summary>
    /// Converts an array of Vector3 to an array of GeometryUtility.CPoint2D.
    /// </summary>
    /// <param name="points">The array points to be converted</param>
    /// <returns>Returns an array of GeometryUtility.CPoint2D elements</returns>
    /// <seealso cref="ToVector2"/>
    /// <seealso cref="ToVector3"/>
    public static GeometryUtility.CPoint2D[] ToCPoint2D(this Vector3[] points)
    {
        GeometryUtility.CPoint2D[] ret = new GeometryUtility.CPoint2D[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            ret[i] = new GeometryUtility.CPoint2D(points[i].x, points[i].z);
        }
        return ret;
    }
    /// <summary>
    /// Checks if there are any duplicate points in the given array.
    /// </summary>
    /// <param name="points">The array to check for duplicates.</param>
    /// <returns>Returns true if a duplicate is found, and false otherwise.</returns>
    public static bool CheckForDuplicates(this GeometryUtility.CPoint2D[] points)
    {
        if (points.Length != points.Distinct().Count())
            return true;
        else return false;
    }
    /// <summary>
    /// Converts an array of Vector3 to an array of GeometryUtility.CPoint2D.
    /// </summary>
    /// <param name="points">The array points to be converted</param>
    /// <param name="yHeight">Sets the height of the returned elements.</param>
    /// <returns>Returns an array of GeometryUtility.CPoint2D elements</returns>
    /// <seealso cref="ToVector2"/>
    /// <seealso cref="ToCPoint2D"/>
    public static Vector3[] ToVector3(this GeometryUtility.CPoint2D[] points, float yHeight)
    {
        Vector3[] ret = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            ret[i] = new Vector3((float)points[i].X, yHeight, (float)points[i].Y);
        }
        return ret;
    }
    /// <summary>
    /// Converts a Vector3 to a GeometryUtility.CPoint2D.
    /// </summary>
    /// <param name="points">The point to be converted</param>
    /// <param name="yHeight">Sets the height of the returned Vector3.</param>
    /// <returns>Returns the converted Vector3.</returns>
    /// <seealso cref="ToVector2"/>
    /// <seealso cref="ToCPoint2D"/>
    public static Vector3 ToVector3(this GeometryUtility.CPoint2D point, float yHeight)
    {
        Vector3 ret = new Vector3((float)point.X, yHeight, (float)point.Y);
        return ret;
    }
    ///// <summary>
    ///// TODO
    ///// </summary>
    ///// <param name="PositionLat"></param>
    ///// <param name="PositionLon"></param>
    ///// <param name="Source">0:minlat, 1:maxlat, 2:minlon, 3:maxlon</param>
    ///// <param name="Target">0:minlat, 1:maxlat, 2:minlon, 3:maxlon</param>
    ///// <returns></returns>
    //public static float[] SimpleInterpolation(float PositionLat, float PositionLon, float[] Source, float[] Target)
    //{
    //    // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
    //    // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPi.xel)
    //    float X = (float)((PositionLat - Source[0]) / (Source[1] - Source[0]) * (Target[1] - Target[0]));
    //    float Y = (float)((PositionLon - Source[2]) / (Source[3] - Source[2]) * (Target[3] - Target[2]));
    //    return new float[] { X, Y };
    //}
    ///// <summary>
    ///// TODO
    ///// </summary>
    ///// <param name="PositionLat"></param>
    ///// <param name="PositionLon"></param>
    ///// <param name="Source"></param>
    ///// <param name="Target"></param>
    ///// <returns></returns>
    //public static float[] SimpleInterpolation(float PositionLat, float PositionLon, ref float[] Source, ref float[] Target)
    //{
    //    // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
    //    // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPi.xel)
    //    float X = (float)((PositionLat - Source[0]) / (Source[1] - Source[0]) * (Target[1] - Target[0]));
    //    float Y = (float)((PositionLon - Source[2]) / (Source[3] - Source[2]) * (Target[3] - Target[2]));
    //    return new float[] { X, Y };
    //}
    /// <summary>
    /// Returns the linear interpolation of a value.
    /// </summary>
    /// <param name="x">The value to be interpolated in the source system.</param>
    /// <param name="x0">Minimum value in the source system.</param>
    /// <param name="x1">Maximum value in the source system.</param>
    /// <param name="y0">Minimum value in the target system.</param>
    /// <param name="y1">Minimum value in the target system.</param>
    /// <returns>Returned the interpolated value.</returns>
    public static float linear(float x, float x0, float x1, float y0, float y1)
    {
        if ((x1 - x0) == 0)
        {
            return (y0 + y1) / 2;
        }
        return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
    }
    /// <summary>
    /// Generates mesh from the given shape.
    /// </summary>
    /// <param name="shape">The provided shape</param>
    /// <param name="type">OSM Type</param>
    /// <param name="Id">OSM id</param>
    /// <param name="other">Other information to included in the last part of object name.</param>
    /// <param name="tag">The tag of the gameobject</param>
    /// <param name="MaterialName">Material name for the object</param>
    /// <seealso cref="GenerateShapeUVedWithWalls_Balanced"/>
    /// <seealso cref="GenerateShapeFromServerObject"/>
    [Obsolete("This method is not used anymore. Use GenerateShapeUVedWithWalls_Balanced instead.", true)]
    public static void GenerateShape(this PolygonCuttingEar.CPolygonShape shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Building")
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

        int[] tris = new int[shape.NumberOfPolygons * 3];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < shape.NumberOfPolygons; i++)
            vertices.AddRange(shape.Polygons(i).ToVector3(0));
        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(1, 0);
        Vector2 uv2 = new Vector2(0.5f, 1);
        Vector2[] uvTemp = new Vector2[] { uv0, uv1, uv2 };
        Vector2[] UVs = new Vector2[vertices.Count];
        for (int i = 0; i < UVs.Length; i++)
            UVs[i] = uvTemp[i % 3];
        p.Rebuild(vertices.ToArray(), tris, UVs, MaterialName, false, false);

    }
    /// <summary>
    /// Generates mesh from the given shape.
    /// </summary>
    /// <param name="shape">The provided shape</param>
    /// <param name="type">OSM Type</param>
    /// <param name="Id">OSM id</param>
    /// <param name="other">Other information to included in the last part of object name.</param>
    /// <param name="tag">The tag of the gameobject</param>
    /// <param name="MaterialName">Material name for the object</param>
    /// <seealso cref="GenerateShapeUVedWithWalls_Balanced"/>
    /// <seealso cref="GenerateShapeFromServerObject"/>
    [Obsolete("This method is not used anymore. Use GenerateShapeUVedWithWalls_Balanced instead.", true)]
    public static void GenerateShapeUVed(this PolygonCuttingEar.CPolygonShape shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Building")
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

        int[] tris = new int[shape.NumberOfPolygons * 3];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < shape.NumberOfPolygons; i++)
            vertices.AddRange(shape.Polygons(i).ToVector3(0));

        var minX = vertices.Select(i => i.x).Min();
        var minZ = vertices.Select(i => i.z).Min();
        var maxX = vertices.Select(i => i.x).Max();
        var maxZ = vertices.Select(i => i.z).Max();
        float Margin = 0.1f;
        //Vector2 uv0 = new Vector2(0, 0);
        //Vector2 uv1 = new Vector2(1, 0);
        //Vector2 uv2 = new Vector2(0.5f, 1);
        //Vector2[] uvTemp = new Vector2[] {uv0,uv1,uv2};
        Vector2[] UVs = new Vector2[vertices.Count];
        for (int i = 0; i < UVs.Length; i++)
            UVs[i] = new Vector2(
                                                 linear(vertices[i].x, minX, maxX, 0 + Margin, 1 - Margin),
                                                 linear(vertices[i].z, minZ, maxZ, 0 + Margin, 1 - Margin));
        p.Rebuild(vertices.ToArray(), tris, UVs, MaterialName, false, false);

    }
    /// <summary>
    /// Generates mesh from the given shape.
    /// </summary>
    /// <param name="shape">The provided shape</param>
    /// <param name="type">OSM Type</param>
    /// <param name="Id">OSM id</param>
    /// <param name="other">Other information to included in the last part of object name.</param>
    /// <param name="tag">The tag of the gameobject</param>
    /// <param name="MaterialName">Material name for the object</param>
    /// <param name="WallHeight">The height variation for the building walls</param>
    /// <seealso cref="GenerateShapeUVedWithWalls_Balanced"/>
    /// <seealso cref="GenerateShapeFromServerObject"/>
    [Obsolete("This method is not used anymore. Use GenerateShapeUVedWithWalls_Balanced instead.", true)]
    public static void GenerateShapeUVedWithWallsBACKUP(this PolygonCuttingEar.CPolygonShape shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Building", float WallHeight = 5)
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
        for (int i = 0; i < shape.NumberOfPolygons; i++)
            vertices.AddRange(shape.Polygons(i).ToVector3(WallHeight));

        Vector3[] OriginalVertices = shape.InputVertices.ToVector3(WallHeight);
        Vector3 HeightVector = new Vector3(0, WallHeight, 0);
        // Generating Walls - Total: (OriginalVertices.Length)*4*3
        for (int i = 0; i < OriginalVertices.Length - 1; i++)
        {
            Vector3 v1 = OriginalVertices[i];
            Vector3 v2 = OriginalVertices[i + 1];
            Vector3 v3 = OriginalVertices[i] - HeightVector;
            Vector3 v4 = OriginalVertices[i + 1] - HeightVector;
            vertices.AddRange(new Vector3[] { v1, v2, v3 });
            vertices.AddRange(new Vector3[] { v3, v2, v4 });
            // Reverse
            vertices.AddRange(new Vector3[] { v1, v3, v2 });
            vertices.AddRange(new Vector3[] { v3, v4, v2 });
        }

        // Last Wall - Total: 4*3
        Vector3 lv1 = OriginalVertices[OriginalVertices.Length - 1];
        Vector3 lv2 = OriginalVertices[0];
        Vector3 lv3 = OriginalVertices[OriginalVertices.Length - 1] - HeightVector;
        Vector3 lv4 = OriginalVertices[0] - HeightVector;
        vertices.AddRange(new Vector3[] { lv1, lv2, lv3 });
        vertices.AddRange(new Vector3[] { lv3, lv2, lv4 });
        // Reverse
        vertices.AddRange(new Vector3[] { lv1, lv3, lv2 });
        vertices.AddRange(new Vector3[] { lv3, lv4, lv2 });


        int[] tris = new int[shape.NumberOfPolygons * 3 + (OriginalVertices.Length - 1) * 4 * 3 + 4 * 3];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;

        var minX = vertices.Select(i => i.x).Min();
        var minZ = vertices.Select(i => i.z).Min();
        var maxX = vertices.Select(i => i.x).Max();
        var maxZ = vertices.Select(i => i.z).Max();
        float Margin = 0;
        //Vector2 uv0 = new Vector2(0, 0);
        //Vector2 uv1 = new Vector2(1, 0);
        //Vector2 uv2 = new Vector2(0.5f, 1);
        //Vector2[] uvTemp = new Vector2[] {uv0,uv1,uv2};
        Vector2[] UVs = new Vector2[vertices.Count];
        for (int i = 0; i < UVs.Length; i++)
            UVs[i] = new Vector2(
                                                 linear(vertices[i].x, minX, maxX, 0 + Margin, 1 - Margin),
                                                 linear(vertices[i].z, minZ, maxZ, 0 + Margin, 1 - Margin));
        p.Rebuild(vertices.ToArray(), tris, UVs, MaterialName, false, false);

    }
    /// <summary>
    /// Generates mesh from the given shape.
    /// </summary>
    /// <param name="shape">The provided shape</param>
    /// <param name="type">OSM Type</param>
    /// <param name="Id">OSM id</param>
    /// <param name="other">Other information to included in the last part of object name.</param>
    /// <param name="tag">The tag of the gameobject</param>
    /// <param name="MaterialName">Material name for the object</param>
    /// <param name="WallHeight">The height variation for the building walls</param>
    /// <param name="MaximumHeight">Maximum wall height</param>
    /// <param name="GenerateColliders">Specifies whether the mesh colliders should be generated or not.</param>
    /// <seealso cref="GenerateShapeUVedWithWalls_Balanced"/>
    /// <seealso cref="GenerateShapeFromServerObject"/>
    [Obsolete("This method is not used anymore. Use GenerateShapeUVedWithWalls_Balanced instead.")]
    public static void GenerateShapeUVedBalanced(this Poly2Tri.Polygon shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Building", float WallHeight = 5, float MaximumHeight = 12, bool GenerateColliders = true)
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
        Vector3[] OriginalVertices = shape.Points.Select(s => new Vector3((float)s.X, WallHeight, (float)s.Y)).ToArray();
        Vector3 HeightVector = new Vector3(0, WallHeight, 0);


        // Generating Walls - Total: (OriginalVertices.Length)*4*3
        for (int i = 0; i < OriginalVertices.Length - 1; i++)
        {
            Vector3 v1 = OriginalVertices[i];
            Vector3 v2 = OriginalVertices[i + 1];
            var dist = Vector3.Distance(v1, v2);

            //Interpolations.MyLog("Distance: "+dist);
            dist = Mathf.Clamp(dist, 0, 5);
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
        //Interpolations.MyLog("Distance: "+distlw);
        distlw = Mathf.Clamp(distlw, 0, 5);
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


        int[] tris = new int[shape.Triangles.Count * 3 + (OriginalVertices.Length - 1) * 4 * 3 + 4 * 3];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;


        var minPoint = new Vector3(vertices.Min(m => m.x), vertices.Min(m => m.y), vertices.Min(m => m.z));
        vertices = vertices.Select(v => v - minPoint).ToList();

        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, false);
        if (GenerateColliders)
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        // Setting the pivot point.
        //var X = (vertices.Min(m => m.x) + vertices.Max(m => m.x)) / 2;
        //var Y = WallHeight / 2;
        //var Z = (vertices.Min(m => m.z) + vertices.Max(m => m.z)) / 2;
        gameObject.transform.position = minPoint;// new Vector3(X, Y, Z);

        GameObject.DestroyImmediate(p);

    }

    public static GameObject GenerateShapeUVedPlanar_Balanced(this Poly2Tri.Polygon shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Area", float Height = 0, bool GenerateColliders = true)
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
        for (int i = 0; i < shape.Triangles.Count; i++)
            vertices.AddRange(shape.Triangles[i].Points.Select(s => new Vector3((float)s.X, Height, (float)s.Y)).Reverse());

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

        int[] tris = new int[shape.Triangles.Count * 3];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;


        gameObject.transform.position = LineDraw.ChangePivot(ref vertices, LineDraw.PivotLocation.Center);

        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, true);
        
        if (GenerateColliders)
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        return gameObject;
    }

    /// <summary>
    /// Generates the mesh from the server-side calculated gameobject.
    /// </summary>
    /// <param name="serverObject">The object that is received from the server.</param>
    /// <param name="tag">The tag for the gameobject</param>
    /// <param name="GenerateColliders">Specifies whether the mesh colliders should be generated or not.</param>
    /// <seealso cref="GenerateShapeUVedWithWalls_Balanced"/>
    public static void GenerateShapeFromServerObject(this GaPSLabs.Geometry.GameObject serverObject, string tag, bool GenerateColliders = true)
    {
        GameObject gameObject = new GameObject(serverObject.Name);
        gameObject.tag = tag;
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
        }
        else
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh();

        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        gameObject.transform.position = serverObject.position.ToVector3();

        Polygon p = gameObject.AddComponent<Polygon>();

        p.Rebuild(serverObject.mesh.vertices.ToVector3(), serverObject.mesh.triangles, serverObject.mesh.uv.ToVector2(), serverObject.material.Name, true, false);
        if (GenerateColliders)
        {
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            //gameObject.GetComponent<MeshCollider>().isTrigger = true;
            gameObject.GetComponent<MeshCollider>().isTrigger = false; // Trigger in concave MeshColliders are not supported anymore in Unity 5
        }

        GameObject.DestroyImmediate(p);
    }
    /// <summary>
    /// Generates mesh from the given shape.
    /// </summary>
    /// <param name="shape">The provided shape</param>
    /// <param name="type">OSM Type</param>
    /// <param name="Id">OSM id</param>
    /// <param name="other">Other information to included in the last part of object name.</param>
    /// <param name="tag">The tag of the gameobject</param>
    /// <param name="MaterialName">Material name for the object</param>
    /// <param name="WallHeight">The height variation for the building walls</param>
    /// <param name="MaximumHeight">Maximum wall height</param>
    /// <param name="GenerateColliders">Specifies whether the mesh colliders should be generated or not.</param>
    /// <seealso cref="GenerateShapeFromServerObject"/>
    public static void GenerateShapeUVedWithWalls_Balanced(this PolygonCuttingEar.CPolygonShape shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Building", float WallHeight = 5, float MaximumHeight = 12, bool GenerateColliders = true)
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

            //Interpolations.MyLog("Distance: "+dist);
            dist = Mathf.Clamp(dist, 0, 5);
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
        //Interpolations.MyLog("Distance: "+distlw);
        distlw = Mathf.Clamp(distlw, 0, 5);
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

        gameObject.transform.position = LineDraw.ChangePivot(ref vertices, LineDraw.PivotLocation.Center);


        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, false);
        if (GenerateColliders)
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;



        GameObject.DestroyImmediate(p);

    }
    /// <summary>
    /// Generates mesh from the given shape.
    /// </summary>
    /// <param name="shape">The provided shape</param>
    /// <param name="type">OSM Type</param>
    /// <param name="Id">OSM id</param>
    /// <param name="other">Other information to included in the last part of object name.</param>
    /// <param name="tag">The tag of the gameobject</param>
    /// <param name="MaterialName">Material name for the object</param>
    /// <param name="WallHeight">The height variation for the building walls</param>
    /// <param name="MaximumHeight">Maximum wall height</param>
    /// <param name="GenerateColliders">Specifies whether the mesh colliders should be generated or not.</param>
    /// <returns>Returns the generated GameObject.</returns>
    /// <seealso cref="GenerateShapeFromServerObject"/>
    /// <seealso cref="GenerateShapeUVedWithWalls_Balanced"/>
    public static GameObject GenerateShapeUVedWithWalls_BalancedAndReturn(this PolygonCuttingEar.CPolygonShape shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Building", float WallHeight = 5, float MaximumHeight = 12, bool GenerateColliders = true)
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

            //Interpolations.MyLog("Distance: "+dist);
            dist = Mathf.Clamp(dist, 0, 5);
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
        //Interpolations.MyLog("Distance: "+distlw);
        distlw = Mathf.Clamp(distlw, 0, 5);
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

        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, false);
        if (GenerateColliders)
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        GameObject.DestroyImmediate(p);
        return gameObject;

    }
    /// <summary>
    /// Generates mesh from the given shape.
    /// </summary>
    /// <param name="shape">The provided shape</param>
    /// <param name="type">OSM Type</param>
    /// <param name="Id">OSM id</param>
    /// <param name="other">Other information to included in the last part of object name.</param>
    /// <param name="tag">The tag of the gameobject</param>
    /// <param name="MaterialName">Material name for the object</param>
    /// <param name="WallHeight">The height variation for the building walls</param>
    /// <param name="MaximumHeight">Maximum wall height</param>
    /// <param name="GenerateColliders">Specifies whether the mesh colliders should be generated or not.</param>
    /// <returns>Returns the generated GameObject.</returns>
    /// <seealso cref="GenerateShapeFromServerObject"/>
    /// <seealso cref="GenerateShapeUVedWithWalls_Balanced"/>
    [Obsolete("This method is not used anymore. Use GenerateShapeUVedWithWalls_Balanced instead.", true)]
    public static void GenerateShapeUVedWithWalls(this PolygonCuttingEar.CPolygonShape shape, LineDraw.OSMType type, string Id, string other, string tag, string MaterialName = "Building", float WallHeight = 5, float MaximumHeight = 12, bool GenerateColliders = true)
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
            Vector3 v3 = OriginalVertices[i] - HeightVector;
            Vector3 v4 = OriginalVertices[i + 1] - HeightVector;
            vertices.AddRange(new Vector3[] { v1, v2, v3 });
            UVs.Add(UVRef[0]);
            UVs.Add(UVRef[1]);
            UVs.Add(UVRef[2]);
            vertices.AddRange(new Vector3[] { v3, v2, v4 });
            UVs.Add(UVRef[2]);
            UVs.Add(UVRef[1]);
            UVs.Add(UVRef[3]);
            // Reverse
            vertices.AddRange(new Vector3[] { v1, v3, v2 });
            UVs.Add(UVRef[0]);
            UVs.Add(UVRef[2]);
            UVs.Add(UVRef[1]);
            vertices.AddRange(new Vector3[] { v3, v4, v2 });
            UVs.Add(UVRef[2]);
            UVs.Add(UVRef[3]);
            UVs.Add(UVRef[1]);
        }

        // Last Wall - Total: 4*3
        Vector3 lv1 = OriginalVertices[OriginalVertices.Length - 1];
        Vector3 lv2 = OriginalVertices[0];
        Vector3 lv3 = OriginalVertices[OriginalVertices.Length - 1] - HeightVector;
        Vector3 lv4 = OriginalVertices[0] - HeightVector;
        vertices.AddRange(new Vector3[] { lv1, lv2, lv3 });
        UVs.Add(UVRef[0]);
        UVs.Add(UVRef[1]);
        UVs.Add(UVRef[2]);
        vertices.AddRange(new Vector3[] { lv3, lv2, lv4 });
        UVs.Add(UVRef[2]);
        UVs.Add(UVRef[1]);
        UVs.Add(UVRef[3]);
        // Reverse
        vertices.AddRange(new Vector3[] { lv1, lv3, lv2 });
        UVs.Add(UVRef[0]);
        UVs.Add(UVRef[2]);
        UVs.Add(UVRef[1]);
        vertices.AddRange(new Vector3[] { lv3, lv4, lv2 });
        UVs.Add(UVRef[2]);
        UVs.Add(UVRef[3]);
        UVs.Add(UVRef[1]);

        int[] tris = new int[shape.NumberOfPolygons * 3 + (OriginalVertices.Length - 1) * 4 * 3 + 4 * 3];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;

        p.Rebuild(vertices.ToArray(), tris, UVs.ToArray(), MaterialName, true, false);
        if (GenerateColliders)
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

    }
    /// <summary>
    /// Generates a billboard for the OSM node at the specified position using default Unity quads.
    /// </summary>
    /// <param name="center">Center position of the billboard</param>
    /// <param name="Id">OSM Id</param>
    /// <param name="type">OSM Type</param>
    /// <param name="other">The other message is appended to the name of the generated gameobject.</param>
    /// <param name="tag">The gameobject tag</param>
    /// <param name="MaterialName">The material name</param>
    /// <param name="HeightFromGround">Base height of the billboard from the ground.</param>
    public static void GenerateBillboardPlaneUsingUnityQuad(this Vector3 center, string Id, LineDraw.OSMType type, string other, string tag, string MaterialName = "TrafficLight", float HeightFromGround = 5f)
    {
        GameObject gameObject = new GameObject();
        gameObject.name = type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : "");
        gameObject.tag = tag;
        gameObject.transform.position = center + new Vector3(0, HeightFromGround, 0);
        GameObject graphics = GameObject.CreatePrimitive(PrimitiveType.Quad);
        UnityEngine.Object.DestroyImmediate(graphics.GetComponent<MeshCollider>());
        gameObject.AddComponent<BoxCollider>().isTrigger = true;
        graphics.transform.position = gameObject.transform.position;
        graphics.transform.parent = gameObject.transform;

        if (!string.IsNullOrEmpty(MaterialName))
            graphics.GetComponent<Renderer>().sharedMaterial = Resources.Load(MaterialName, typeof(Material)) as Material;
    }
    /// <summary>
    /// Generates a billboard for the OSM node at the specified position.
    /// </summary>
    /// <param name="center">Center position of the billboard</param>
    /// <param name="Id">OSM Id</param>
    /// <param name="type">OSM Type</param>
    /// <param name="other">The other message is appended to the name of the generated gameobject.</param>
    /// <param name="tag">The gameobject tag</param>
    /// <param name="MaterialName">The material name</param>
    /// <param name="HeightFromGround">Base height of the billboard from the ground.</param>
    public static void GenerateBillboardPlane(this Vector3 center, string Id, LineDraw.OSMType type, string other, string tag, string MaterialName = "TrafficLight", float Width = 5f, float HeightFromGround = 5f)
    {
        GameObject gameObject = new GameObject(type + "|" + ((int)type) + "|" + Id + (!string.IsNullOrEmpty(other) ? "|" + other : ""));
        gameObject.tag = tag;
        gameObject.transform.position = center + new Vector3(0, HeightFromGround + Width / 2, 0);
        // gameObject.AddComponent<FaceCamera>();
        Polygon p = gameObject.AddComponent<Polygon>();
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
        }
        else
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();

        Vector3 lv1 = new Vector3(-Width / 2, 0, -Width / 2);
        Vector3 lv2 = new Vector3(Width / 2, 0, -Width / 2);
        Vector3 lv3 = new Vector3(Width / 2, 0, Width / 2);
        Vector3 lv4 = new Vector3(-Width / 2, 0, Width / 2);
        vertices.AddRange(new Vector3[] { lv1, lv2, lv3 });
        vertices.AddRange(new Vector3[] { lv3, lv4, lv1 });
        // Reverse
        vertices.AddRange(new Vector3[] { lv1, lv3, lv2 });
        vertices.AddRange(new Vector3[] { lv3, lv1, lv4 });

        int[] tris = new int[vertices.Count];
        for (int i = 0; i < tris.Length; i++)
            tris[i] = i;

        var minX = vertices.Select(i => i.x).Min();
        var minZ = vertices.Select(i => i.z).Min();
        var maxX = vertices.Select(i => i.x).Max();
        var maxZ = vertices.Select(i => i.z).Max();
        float Margin = 0f;

        Vector2[] UVs = new Vector2[vertices.Count];
        for (int i = 0; i < UVs.Length; i++)
            UVs[i] = new Vector2(
                                                 linear(vertices[i].x, minX, maxX, 0 + Margin, 1 - Margin),
                                                 linear(vertices[i].z, minZ, maxZ, 0 + Margin, 1 - Margin));
        p.Rebuild(vertices.ToArray(), tris, UVs, MaterialName, true, true);

    }
    /// <summary>
    /// Calculates the tangents for the given mesh.
    /// </summary>
    /// <param name="mesh">The mesh to use</param>
    public static void calculateMeshTangents(this Mesh mesh)
    {
        //speed up math by copying the mesh arrays
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;

        //variable definitions
        int triangleCount = triangles.Length;
        int vertexCount = vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

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
            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
    /// <summary>
    /// Creates a quad shape.
    /// </summary>
    /// <param name="s">Starting vector</param>
    /// <param name="e">Ending vector</param>
    /// <param name="w">The width of the quad</param>
    /// <returns>Returns an array of Vector3 elements containing the quad corners.</returns>
    public static Vector3[] MakeQuadManual(this Vector3 s, Vector3 e, float w)
    {
        var L = Mathf.Sqrt((s.x - e.x) * (s.x - e.x) + (s.z - e.z) * (s.z - e.z));

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
    /// <summary>
    /// Creates a quad shape.
    /// </summary>
    /// <param name="s">Starting vector</param>
    /// <param name="e">Ending vector</param>
    /// <param name="w">The width of the quad</param>
    /// <param name="transform">The transform to be used for inverse transform operations</param>
    /// <returns>Returns an array of Vector3 elements containing the quad corners.</returns>
    public static Vector3[] MakeQuad(this Vector3 s, Vector3 e, float w, Transform transform)
    {
        return MakeQuadManual(s, e, w);
        // s ... start point
        // e ... endpoint
        // w ... width of line segment
        w = w / 2;
        Vector3[] q = new Vector3[4];

        Vector3 n = Vector3.Cross(s, e);
        Vector3 l = Vector3.Cross(n, e - s);

        l.Normalize();

        q[0] = transform.InverseTransformPoint(s + l * w);
        q[1] = transform.InverseTransformPoint(s + l * -w);
        q[2] = transform.InverseTransformPoint(e + l * w);
        q[3] = transform.InverseTransformPoint(e + l * -w);

        return q;
    }
    /// <summary>
    /// Segments an path containing the array of points with the given precision.
    /// </summary>
    /// <param name="OriginalPoints">The source point array</param>
    /// <param name="Precision">The precision of the segmentations</param>
    /// <returns>Returns the segmented array of Vector3 elements.</returns>
    public static Vector3[] ToSegmentedPoints(this Vector3[] OriginalPoints, float Precision)
    {
        return LineDraw.SegmentPoints(OriginalPoints, Precision);
    }
    /// <summary>
    /// Converts a MapBoundaries object to the MapProperties
    /// </summary>
    /// <param name="up">The property object</param>
    /// <returns>Returns the MapProperties object.</returns>
    public static GaPSLabs.Geometry.MapProperties ToGapslabsMapProperties(this MapBoundaries up, float minimumHeight = 6, float maximumHeight = 30)
    {
        GaPSLabs.Geometry.MapProperties ret = new GaPSLabs.Geometry.MapProperties();
        ret.BuildingColor = up.BuildingColor.ToGapsLabsColor();
        ret.BuildingHeight = up.BuildingHeight;
        ret.BuildingHeightVariation = new GaPSLabs.Geometry.DiscreetCurve();
        List<float> heightVariations = new List<float>();
        float NumberOfSteps = up.BuildingHeightVariation.keys.Length * 2f;
        float maxHeight = up.BuildingHeightVariation.keys.Max(m => m.value);
        float minHeight = up.BuildingHeightVariation.keys.Min(m => m.value);
        for (int i = 0; i < NumberOfSteps; i++)
        {
            heightVariations.Add(
                Mathf.Max(
                minimumHeight,
                linear(up.BuildingHeightVariation.Evaluate(i / NumberOfSteps), minHeight, maxHeight, minHeight, maximumHeight)
                ));
        }
        ret.BuildingHeightVariation.data = heightVariations.ToArray();

        ret.BuildingLineThickness = up.BuildingLineThickness;
        ret.BuildingMaterial = new GaPSLabs.Geometry.Material() { Name = up.BuildingMaterial != null ? up.BuildingMaterial.name : "Building" };
        ret.CombinationOptimizationSize = up.CombinationOptimizationSize.ToGapsLabsVector2();
        // ret.CorrectAspectRatio =  up.CorrectAspectRatio;
        ret.CycleWayMaterial = new GaPSLabs.Geometry.Material() { Name = up.CycleWayMaterial.name };
        ret.CyclewayWidth = up.CyclewayWidth;
        ret.FootWayMaterial = new GaPSLabs.Geometry.Material() { Name = up.FootWayMaterial.name };
        ret.AreaMaterial = new GaPSLabs.Geometry.Material() { Name = up.AreaMaterial.name };
        ret.WaterMaterial = new GaPSLabs.Geometry.Material() { Name = up.WaterMaterial.name };
        ret.FootwayWidth = up.FootwayWidth;
        ret.LineColorEnd = up.LineColorEnd.ToGapsLabsColor();
        ret.LineColorStart = up.LineColorStart.ToGapsLabsColor();
        ret.maxLat = up.maxLat;
        ret.maxLon = up.maxLon;
        ret.minLat = up.minLat;
        ret.minLon = up.minLon;
        ret.minMaxX = up.minMaxX;
        ret.minMaxY = up.minMaxY;
        ret.MinPointOnMap = up.MinPointOnMap.ToGapsLabsVector3();
        ret.Name = up.Name;
        ret.OverrideDatabaseConnection = up.OverrideDatabaseConnection;
        ret.OverridenConnectionString = up.GetOverridenConnectionString();
        ret.RailWayMaterial = new GaPSLabs.Geometry.Material() { Name = up.RailWayMaterial != null ? up.RailWayMaterial.name : "" };
        ret.RoadLineThickness = up.RoadLineThickness;
        ret.RoadMaterial = new GaPSLabs.Geometry.Material() { Name = up.RoadMaterial.name };
        ret.ResidentialMaterial = new GaPSLabs.Geometry.Material() { Name = up.ResidentialMaterial.name };
        ret.ServiceMaterial = new GaPSLabs.Geometry.Material() { Name = up.ServiceMaterial.name };
        ret.RoadWidth = up.RoadWidth;
        ret.Scale = up.Scale.ToGapsLabsVector2();
        ret.StepsMaterial = new GaPSLabs.Geometry.Material() { Name = up.StepsMaterial.name };
        return ret;
    }
    /// <summary>
    /// Converts between the color systems
    /// </summary>
    /// <param name="color">The color to be converted</param>
    /// <returns>Returns the converted colors.</returns>
    public static GaPSLabs.Geometry.Color ToGapsLabsColor(this Color color)
    {
        return new GaPSLabs.Geometry.Color() { R = (int)(color.r * 255), G = (int)(color.g * 255), B = (int)(color.b * 255) };
    }
    /// <summary>
    /// Converts between the Vector3 classes.
    /// </summary>
    /// <param name="vector">The vector to be converted</param>
    /// <returns>Returns the converted Vector3</returns>
    /// <seealso cref="ToVector3"/>
    public static GaPSLabs.Geometry.Vector3 ToGapsLabsVector3(this Vector3 vector)
    {
        return new GaPSLabs.Geometry.Vector3() { x = vector.x, y = vector.y, z = vector.z };
    }
    /// <summary>
    /// Converts between the Vector3 classes.
    /// </summary>
    /// <param name="vector">The vector to be converted</param>
    /// <returns>Returns the converted Vector3</returns>
    /// <seealso cref="ToGapsLabsVector3"/>
    public static Vector3 ToVector3(this GaPSLabs.Geometry.Vector3 vector)
    {
        return new Vector3() { x = vector.x, y = vector.y, z = vector.z };
    }
    /// <summary>
    /// Converts between the arrays of Vector3 classes.
    /// </summary>
    /// <param name="vector">The vector array to be converted</param>
    /// <returns>Returns the converted Vector3 array</returns>
    /// <seealso cref="ToGapsLabsVector3"/>
    public static Vector3[] ToVector3(this GaPSLabs.Geometry.Vector3[] vector)
    {
        return vector.Select(v => v.ToVector3()).ToArray();
    }
    /// <summary>
    /// Converts between the arrays of Vector2 classes.
    /// </summary>
    /// <param name="vector">The vector array to be converted</param>
    /// <returns>Returns the converted Vector2 array</returns>
    /// <seealso cref="ToGapsLabsVector2"/>
    public static Vector2[] ToVector2(this GaPSLabs.Geometry.Vector2[] vector)
    {
        return vector.Select(v => v.ToVector2()).ToArray();
    }
    /// <summary>
    /// Converts between the Vector2 classes.
    /// </summary>
    /// <param name="vector">The vector to be converted</param>
    /// <returns>Returns the converted Vector2</returns>
    /// <seealso cref="ToVector2"/>
    public static GaPSLabs.Geometry.Vector2 ToGapsLabsVector2(this Vector2 vector)
    {
        return new GaPSLabs.Geometry.Vector2() { x = vector.x, y = vector.y };
    }
    /// <summary>
    /// Converts between the Vector2 classes.
    /// </summary>
    /// <param name="vector">The vector to be converted</param>
    /// <returns>Returns the converted Vector2</returns>
    /// <seealso cref="ToGapsLabsVector2"/>
    public static Vector2 ToVector2(this GaPSLabs.Geometry.Vector2 vector)
    {
        return new Vector2() { x = vector.x, y = vector.y };
    }
}
