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
	/// Defines an OpenstreetMaps Relation member. Refer to OpenstreetMaps wiki for more.
	/// </summary>
	[Serializable]
	public class Member
	{
        public static string RoleInner = "inner";
        public static string RoleOuter = "outer";
		// public enum MemberType { Way, Node };
		/// <summary>
		/// Gets or sets the relation id.
		/// </summary>
		public String RelationId { get; set; }
		/// <summary>
		/// Gets or sets the referenced element id.
		/// </summary>
		public String ReferenceId { get; set; }
		/// <summary>
		/// 0 for Node, 1 for Way, 2 for Relation
		/// </summary>
		public int Type { get; set; }
		/// <summary>
		/// Gets or sets the role of this member in the relation.
		/// </summary>
		public String Role { get; set; }
		/// <summary>
		/// Gets or sets the order of the member in the relation.
		/// </summary>
		public int order { get; set; }
	}
}
