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
using System.Xml.Linq;

namespace Aram.OSMParser
{
	/// <summary>
	/// The class that represents a Openstreetmap tag element.
	/// Please refer to <a href="http://www.openstreetmap.org/help">OSM Help</a> for more information.
	/// </summary>
	[Serializable]
	public class Tag
	{
		/// <summary>
		/// Gets or sets the OSM xml element of the tag.
		/// </summary>
		public XElement OsmElement { get; set; }
		/// <summary>
		/// The default constructor of the Tag
		/// </summary>
		public Tag() { }
		/// <summary>
		/// The constructor that sets the <see cref="OsmElement"/>.
		/// </summary>
		/// <param name="tag"></param>
		public Tag(XElement tag)
		{
			this.OsmElement = tag;
		}
		/// <summary>
		/// Gets or sets an array of key-value pair.
		/// The index 0 is the key, the index 1 is the value.
		/// </summary>
		public String[] KeyValueSQL { get; set; }
		/// <summary>
		/// Gets the key name of the tag from the OSM xml element.
		/// </summary>
		[Obsolete("This is only used when reading from an OSM file and deprecated.",true)]
		public String Key
		{
			get
			{
				return OsmElement.Attribute("k").Value;
			}
		}
		/// <summary>
		/// Gets the value of the tag from the OSM xml element.
		/// </summary>
		[Obsolete("This is only used when reading from an OSM file and deprecated.", true)]
		public String Value
		{
			get
			{
				return OsmElement.Attribute("v").Value;
			}
		}


	}


    public class Vector3EqualityComparer : IEqualityComparer<Tag>
    {

        public bool Equals(Tag b1, Tag b2)
        {
            if (b1.KeyValueSQL[0] == b2.KeyValueSQL[0] && b1.KeyValueSQL[1] == b2.KeyValueSQL[1] )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(Tag bx)
        {
            int hCode = bx.GetHashCode();
            return hCode.GetHashCode();
        }

    }
}
