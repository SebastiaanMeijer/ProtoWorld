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
using System.Xml;
using System.Xml.Linq;

namespace Aram.OSMParser
{
	/// <summary>
	/// The boundary class that defines minimum and maximum latitude/longitude for a rectangular area.
	/// </summary>
	[Serializable]
	public class Bounds
	{
		/// <summary>
		/// The default constructor of Bounds class.
		/// </summary>
		public Bounds() { }
		/// <summary>
		/// Initializes the Bounds from the values retrieved from the bound element of an XML OSM format.
		/// </summary>
		/// <param name="boundBoxOsmElement">The bound element in the OSM XML.</param>
		public Bounds(XElement boundBoxOsmElement)
		{
			this.minlat = Double.Parse(boundBoxOsmElement.Attribute("minlat").Value, CultureInfo.InvariantCulture);
			this.minlon = Double.Parse(boundBoxOsmElement.Attribute("minlon").Value, CultureInfo.InvariantCulture);
			this.maxlat = Double.Parse(boundBoxOsmElement.Attribute("maxlat").Value, CultureInfo.InvariantCulture);
			this.maxlon = Double.Parse(boundBoxOsmElement.Attribute("maxlon").Value, CultureInfo.InvariantCulture);
		}
		/// <summary>
		/// Initializes the Bounds from the values retrieved from the bound element of an XML OSM format.
		/// </summary>
		/// <param name="boundBoxOsmElement">The bound element in the OSM XML.</param>
		public Bounds(XmlNode boundBoxOsmElement)
		{
			this.minlat = Double.Parse(boundBoxOsmElement.Attributes["minlat"].Value, CultureInfo.InvariantCulture);
			this.minlon = Double.Parse(boundBoxOsmElement.Attributes["minlon"].Value, CultureInfo.InvariantCulture);
			this.maxlat = Double.Parse(boundBoxOsmElement.Attributes["maxlat"].Value, CultureInfo.InvariantCulture);
			this.maxlon = Double.Parse(boundBoxOsmElement.Attributes["maxlon"].Value, CultureInfo.InvariantCulture);
		}
		/// <summary>
		/// Gets or sets the minimum latitude.
		/// </summary>
		public double minlat { get; set; }
		/// <summary>
		/// Gets or sets the minimum longitude.
		/// </summary>
		public double minlon { get; set; }
		/// <summary>
		/// Gets or sets the maximum latitude.
		/// </summary>
		public double maxlat { get; set; }
		/// <summary>
		/// Gets or sets the maximum longitude.
		/// </summary>
		public double maxlon { get; set; }
		
	}
}
