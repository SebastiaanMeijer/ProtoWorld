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

namespace GaPSLabs.Geometry
{

    public class LineMath
    {
        public Vector3 p1;
        public Vector3 p2;

        public float m
        {
            get { return p1.Slope(p2); }
        }
        public float b
        {
            get { return p1.z - (m * p1.x); }
        }
        public LineMath() { }
        public LineMath(Vector3 p1, Vector3 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
        public LineMath PerpendicularAtAPointOnLine(Vector3 point)
        {
            LineMath lm = new LineMath();
            // TODO Check if it on the line
            lm.p1 = point;
            if (IsNaNorIsInfinity(m))
            {
                var x = point.x + 1;
                var z = point.z;
                lm.p2 = new Vector3(x, point.y, z);
                return lm;
            }
            else if (m != 0)
            {
                var perpM = -1 / m;
                var perpB = point.z - point.x * perpM;
                var x = point.x + 5;
                var z = perpM * x + perpB;
                lm.p2 = new Vector3(x, point.y, z);
                return lm;
            }
            else
            {
                var x = point.x;
                var z = point.z + 1;
                lm.p2 = new Vector3(x, point.y, z);
                return lm;
            }
        }
        public Vector3 Intersect(LineMath secondLine)
        {
            if (IsNaNorIsInfinity(m) && IsNaNorIsInfinity(secondLine.m)) // x=a1 and x=a2
            {
                // check if they are the same line, or parallel and have no intersection.
                if (p1.x == secondLine.p1.x)
                {
                    // Same line, return either end of first line, or start of the second line
                    return p2;
                }
                else
                    return new Vector3(float.NaN, float.NaN, float.NaN);
            }
            else if (IsNaNorIsInfinity(m))
            {
                return new Vector3(p1.x, p1.y, p1.x * secondLine.m + secondLine.b);
            }
            else if (IsNaNorIsInfinity(secondLine.m))
            {
                return new Vector3(secondLine.p1.x, secondLine.p1.y, secondLine.p1.x * m + b);
            }
            else if (m == secondLine.m)
            {
                if (b == secondLine.b)
                {
                    // Same line, return either end of first line, or start of the second line
                    return p2;
                }
                else
                    return new Vector3(float.NaN, float.NaN, float.NaN);
            }

            var intersectionX = (secondLine.b - b) / (m - secondLine.m);
            var intersectionZ = m * intersectionX + b;
            Vector3 intersection = new Vector3(intersectionX, p1.y, intersectionZ);
            return intersection;
        }
        public bool IsNaNorIsInfinity(float number)
        {
            return (float.IsNaN(number) || float.IsInfinity(number));
        }
        public Vector3[] FindPointsAt(float Distance, Vector3 FromPointOnLine)
        {
            // For the line y = ax + b
            // The vector that traverses parallel to the line is (1,a). 
            // Normalize this vector by dividing by sqrt(1+a^2) to get vector v in the same direction that has length 1. 
            // Then your desired point is w± (d⋅v) where w is the original point and d is the desired distance.
            if (IsNaNorIsInfinity(m) || float.IsInfinity(m))
            {
                //Interpolations.MyLog("b=" + b + " and point on line is " + FromPointOnLine);
                Vector3 result1 = new Vector3(FromPointOnLine.x, FromPointOnLine.y, FromPointOnLine.z + Distance);
                Vector3 result2 = new Vector3(FromPointOnLine.x, FromPointOnLine.y, FromPointOnLine.z - Distance);

                return new Vector3[] { result1, result2 };
            }
            else if (m == 0)
            {
                Vector3 result1 = new Vector3(FromPointOnLine.x + Distance, FromPointOnLine.y, FromPointOnLine.z);
                Vector3 result2 = new Vector3(FromPointOnLine.x - Distance, FromPointOnLine.y, FromPointOnLine.z);
                return new Vector3[] { result1, result2 };
            }
            else
            {
                Vector2 v = new Vector2(1, m);
                //Interpolations.MyLog("Mathf.Sqrt(1 + m * m) for m=" + m + " : " + Mathf.Sqrt(1 + m * m));
                v = v / (float)Math.Sqrt(1 + m * m);
                var dDotv = Distance * v;
                Vector3 result1 = new Vector3(FromPointOnLine.x + dDotv.x, FromPointOnLine.y, FromPointOnLine.z + dDotv.y);
                Vector3 result2 = new Vector3(FromPointOnLine.x - dDotv.x, FromPointOnLine.y, FromPointOnLine.z - dDotv.y);
                return new Vector3[] { result1, result2 };
            }
        }

      
        

    }
}
