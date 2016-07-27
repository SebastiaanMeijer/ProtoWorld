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
using System.Xml.XPath;


namespace Aram.OSMParser
{
	[Obsolete("Use OSMPostgresqlSource instead")]
	public class OSMSourceNonLinq
	{
		public XDocument OsmDocument { get; set; }
		public XmlDocument XmlDocument;
		public String filename;
		public XPathNavigator Navigator;
		public XPathDocument docNav;
		public XPathNodeIterator NodeIter;

		public OSMSourceNonLinq(String filename)
		{
			this.filename = filename;

			OsmDocument = XDocument.Load(filename);
			XmlDocument = new System.Xml.XmlDocument();
			XmlDocument.Load(filename);
			docNav = new XPathDocument(filename);
			Navigator = docNav.CreateNavigator();
		}

		public Bounds Bounds
		{
			get
			{
				var bounds = XmlDocument.SelectSingleNode("./osm/bounds");
				Bounds b = new OSMParser.Bounds(bounds);
				return b;
			}
		}
		public XmlNodeList Ways
		{
			get
			{
				var ways = XmlDocument.SelectNodes("./osm/way");
				return ways;
			}
		}
		public XmlNodeList Nodes
		{
			get
			{
				var nodes = XmlDocument.SelectNodes("./osm/node");
				return nodes;
			}
		}
		public double[] MinMaxLattitude
		{
			get
			{
				double max = double.Parse(Nodes[0].Attributes["lat"].Value, CultureInfo.InvariantCulture);
				double min = double.Parse(Nodes[0].Attributes["lat"].Value, CultureInfo.InvariantCulture);

				foreach (XmlNode node in Nodes)
				{
					var currentLat = Double.Parse(node.Attributes["lat"].Value, CultureInfo.InvariantCulture);
					if (currentLat < min)
						min = currentLat;
					if (currentLat > max)
						max = currentLat;
				}
				return new double[] { min, max };
			}
		}
		public double[] MinMaxLongtitude
		{
			get
			{
				double max = double.Parse(Nodes[0].Attributes["lon"].Value, CultureInfo.InvariantCulture);
				double min = double.Parse(Nodes[0].Attributes["lon"].Value, CultureInfo.InvariantCulture);

				foreach (XmlNode node in Nodes)
				{
					var currentLat = Double.Parse(node.Attributes["lon"].Value, CultureInfo.InvariantCulture);
					if (currentLat < min)
						min = currentLat;
					if (currentLat > max)
						max = currentLat;
				}
				return new double[] { min, max };
			}
		}
	}
}
