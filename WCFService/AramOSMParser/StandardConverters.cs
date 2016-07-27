/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Aram.OSMParser
//{
//	/// <summary>
//	/// A converter class 
//	/// </summary>
//	public static class StandardConverters
//	{
//		public static float WGS84_a = 6378137;
//		public static float WGS84_1_over_f = 298.257224f;
//		public static float WGS84_f = 0.003352810659835f;
//		public static float Calculate_WGS84_C(GeoPosition Position)
//		{
//			float C = 1f / ((float)Math.Sqrt(
//					Math.Cos(Position.Lat) * Math.Cos(Position.Lat) + (1 - WGS84_f) * (1 - WGS84_f) * Math.Sin(Position.Lat) * Math.Sin(Position.Lat)
//					));
//			return C;
//		}
//		public static float Calculate_WGS84_S(float C)
//		{
//			float S = (1 - WGS84_f) * (1 - WGS84_f) * C;
//			return S;
//		}
//		public static float[] ConvertWGS84toECEF_NoAltitude(GeoPosition Position)
//		{
//			float C = Calculate_WGS84_C(Position);
//			float S = Calculate_WGS84_S(C);
//			var h = 0; // Zero altitude;
//			float x = (float)((WGS84_a * C + h) * Math.Cos(Position.Lat) * Math.Cos(Position.Lon));
//			float y = (float)((WGS84_a * C + h) * Math.Cos(Position.Lat) * Math.Sin(Position.Lon));
//			float z = (float)((WGS84_a * S + h) * Math.Sin(Position.Lat));
//			return new float[] { x, y, z };
//		}
//		public static float[] WGS84toXY_SimpleInterpolation(GeoPosition Position, Bounds Boundings, float[] MinMaxX, float[] MinMaxY)
//		{
//			// pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
//			// pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPixel)
//			float X = (float)((Position.Lon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxX[1] - MinMaxX[0]));
//			float Y = (float)((Position.Lat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxY[1] - MinMaxY[0]));
//			return new float[] { X, Y };
//		}
//	}
//}
