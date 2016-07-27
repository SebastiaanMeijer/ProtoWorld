/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Aram.OSMParser
{
	/// <summary>
	/// A GeoPosition class that defines a point in latitude/longitude format.
	/// </summary>
	/// <seealso cref="Bounds"/>
	[Serializable]
	public class GeoPosition
	{
		/// <summary>
		/// The default constructor of <see cref="GeoPosition"/>.
		/// </summary>
		public GeoPosition() { }
		/// <summary>
		/// Initializes the class with latitude and longitude values that are parsed from strings.
		/// </summary>
		/// <param name="Lat">The latitude value</param>
		/// <param name="Lon">The longitude value</param>
		public GeoPosition(String Lat, String Lon)
		{
			this.Lat = double.Parse(Lat, CultureInfo.InvariantCulture);
			this.Lon = double.Parse(Lon, CultureInfo.InvariantCulture);
		}
		/// <summary>
		/// Initializes the class with latitude and longitude values.
		/// </summary>
		/// <param name="Lat">The latitude value</param>
		/// <param name="Lon">The longitude value</param>
		public GeoPosition(double Lat, double Lon)
		{
			this.Lat = Lat;
			this.Lon = Lon;
		}
		/// <summary>
		/// Gets or set the latitude.
		/// </summary>
		public double Lat { get; set; }
		/// <summary>
		/// Gets or set the longitude.
		/// </summary>
		public double Lon { get; set; }
		/// <summary>
		/// Gets or set the altitude. Note: pending development.
		/// </summary>
		public double Alt { get; set; }
	}
}
