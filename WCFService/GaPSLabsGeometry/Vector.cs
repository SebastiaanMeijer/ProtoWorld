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
    public class Vector2
    {
        public float x;
        public float y;
        public Vector2() { }
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 Scale(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public void Scale(Vector2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        public void Normalize()
        {
            float magnitude = this.magnitude;
            if (magnitude > 1E-05f)
            {
                this.x = (this.x / magnitude);
                this.y = (this.y / magnitude);
            }
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public Vector2 normalized
        {
            get
            {
                Vector2 vector = new Vector2(this.x, this.y);
                vector.Normalize();
                return vector;
            }
        }

        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y };
            return String.Format("({0:F1}, {1:F1})", args);
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format) };
            return String.Format("({0}, {1})", args);
        }

        public override int GetHashCode()
        {
            return (this.x.GetHashCode() ^ (this.y.GetHashCode() << 2));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2))
            {
                return false;
            }
            Vector2 vector = (Vector2)other;
            return (this.x.Equals(vector.x) && this.y.Equals(vector.y));
        }

        public static float Dot(Vector2 lhs, Vector2 rhs)
        {
            return ((lhs.x * rhs.x) + (lhs.y * rhs.y));
        }

        [Newtonsoft.Json.JsonIgnore]
        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt((this.x * this.x) + (this.y * this.y));
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public float sqrMagnitude
        {
            get
            {
                return ((this.x * this.x) + (this.y * this.y));
            }
        }
        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float)(Math.Acos(CoordinateConvertor.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            Vector2 vector = a - b;
            return vector.magnitude;
        }

        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        {
            if (vector.sqrMagnitude > (maxLength * maxLength))
            {
                return (Vector2)(vector.normalized * maxLength);
            }
            return vector;
        }

        public static float SqrMagnitude(Vector2 a)
        {
            return ((a.x * a.x) + (a.y * a.y));
        }

        public float SqrMagnitude()
        {
            return ((this.x * this.x) + (this.y * this.y));
        }

        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        [Newtonsoft.Json.JsonIgnore]
        public static Vector2 zero
        {
            get
            {
                return new Vector2(0f, 0f);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public static Vector2 one
        {
            get
            {
                return new Vector2(1f, 1f);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public static Vector2 up
        {
            get
            {
                return new Vector2(0f, 1f);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public static Vector2 right
        {
            get
            {
                return new Vector2(1f, 0f);
            }
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.x, -a.y);
        }

        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator *(float d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator /(Vector2 a, float d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);
        }

        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }
    }

    public class Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3() { }
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        [Newtonsoft.Json.JsonIgnore]
        public static Vector3 zero
        {
            get
            {
                return new Vector3(0f, 0f, 0f);
            }
        }
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }


        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Vector3 result = new Vector3();
            result.x = a.x - b.x;
            result.y = a.y - b.y;
            result.z = a.z - b.z;
            return result;
        }

        public static float Magnitude(Vector3 a)
        {
            return (float)Math.Sqrt(((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));
        }


        public static Vector3 Normalize(Vector3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return (Vector3)(value / num);
            }
            return zero;
        }
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static implicit operator Aram.OSMParser.Vector3GaPS(Vector3 v)
        {
            return new Aram.OSMParser.Vector3GaPS(v.x, v.y, v.z);
        }
        public static bool EqualsManual(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        //public static bool operator ==(Vector3 lhs, Vector3 rhs)
        //{
        //    return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
        //}

        //public static bool operator !=(Vector3 lhs, Vector3 rhs)
        //{
        //    return (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);
        //}

        public static float SqrMagnitude(Vector3 a)
        {
            return (((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));
        }
        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return (float)Math.Sqrt(((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
        }

        public float Slope(Vector3 linepoint2)
        {
            return (linepoint2.z - this.z) / (linepoint2.x - this.x);
        }
        public static float Slope(Vector3 linepoint1, Vector3 linepoint2)
        {
            return (linepoint2.z - linepoint1.z) / (linepoint2.x - linepoint1.x);
        }
    }


    public class Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public Vector4() { }
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        [Newtonsoft.Json.JsonIgnore]
        public static Vector4 zero
        {
            get
            {
                return new Vector4(0f, 0f, 0f, 0f);
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public static Vector4 one
        {
            get
            {
                return new Vector4(1f, 1f, 1f, 1f);
            }
        }
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vector4 operator -(Vector4 a)
        {
            return new Vector4(-a.x, -a.y, -a.z, -a.w);
        }

        public static Vector4 operator *(Vector4 a, float d)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator *(float d, Vector4 a)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator /(Vector4 a, float d)
        {
            return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
        }

        public static bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            return (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);
        }

        public static implicit operator Vector4(Vector3 v)
        {
            return new Vector4(v.x, v.y, v.z, 0f);
        }

        public static implicit operator Vector3(Vector4 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector4(Vector2 v)
        {
            return new Vector4(v.x, v.y, 0f, 0f);
        }

        public static implicit operator Vector2(Vector4 v)
        {
            return new Vector2(v.x, v.y);
        }
        public static float SqrMagnitude(Vector4 a)
        {
            return Dot(a, a);
        }
        public static float Dot(Vector4 a, Vector4 b)
        {
            return ((((a.x * b.x) + (a.y * b.y)) + (a.z * b.z)) + (a.w * b.w));
        }

        public override int GetHashCode()
        {
            return (((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2)) ^ (this.w.GetHashCode() >> 1));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector4))
            {
                return false;
            }
            Vector4 vector = (Vector4)other;
            return (((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z)) && this.w.Equals(vector.w));
        }

    }
}
