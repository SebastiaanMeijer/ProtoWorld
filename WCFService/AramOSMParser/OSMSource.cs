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

/*
 
Using System.Xml.Linq in Unity and on iOS
Posted: 05:40 PM 1 Week Ago

    I am so relieved that this works (at least what I've tested so far.)

    People should know, though, that in Unity 4.0 I didn't just have to add a reference to the System.Xml.Linq assembly in MonoDevelop - but I had to actually locate the assembly itself and drag it into my Assets folder.

    That took me a while to figure out because Mono would build just fine but I would get errors about missing namespace System.Xml.Linq when trying to run it in the Editor (much less iOS.)

    I did have a hiccup at one time where Unity reported that the AOT compile failed relating to link but I shut down Mono and Unity and did a complete rebuild and it worked just fine again.

    Linq is a great time saver Thanks for the great work Unity team!

    Hans

    P.S. I used the version 3.5 System.Xml.Linq assembly. 


 */
namespace Aram.OSMParser
{
  [Obsolete("Use OSMPostgresqlSource instead")]
	public class OSMSource
	{
		public String filename;
		public OSMSource(String filename)
		{
			this.filename = filename;
			OsmDocument = XDocument.Load(filename);
		}
		public XDocument OsmDocument { get; set; }
		public Bounds Bounds
		{
			get
			{
				var bounds = (from s in OsmDocument.Descendants("osm")
											select s.Element("bounds")).Single();
				Bounds b = new OSMParser.Bounds(bounds);
				return b;
			}
		}
		public List<XElement> Ways
		{
			get
			{
				var ways = (from s in OsmDocument.Descendants("osm")
										select s.Descendants("way")).Single();
				return ways.ToList();
			}
		}
		public List<XElement> Nodes
		{
			get
			{
				var nodes = OsmDocument.Descendants("osm").Select(i => i.Elements("node")).First().ToList();
				return nodes;
			}
		}
		public double[] MinMaxLattitude
		{
			get
			{
				double max = double.Parse(this.Nodes.First<XElement>().Attribute("lat").Value);
				double min = double.Parse(this.Nodes.First<XElement>().Attribute("lat").Value);
				foreach (XElement node in this.Nodes)
				{
					double currentLat = double.Parse(node.Attribute("lat").Value, CultureInfo.InvariantCulture);
					if (currentLat < min)
					{
						min = currentLat;
					}
					if (currentLat > max)
					{
						max = currentLat;
					}
				}
				return new double[]
				{
					min,
					max
				};
			}
		}
	}
}
