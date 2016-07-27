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

namespace Aram.OSMParser
{
    /// <summary>
    /// A class for extending functionality of other classes.
    /// </summary>
	public static class Helpy
	{
        /// <summary>
        /// Performing a member-wise comparison of two double arrays.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
		public static bool EqualsManual(this double[] array, double[] array2)
		{
			return array.SequenceEqual(array2);
		}
        /// <summary>
        /// Compares two OsmNode objects by PositionSQL property.
        /// </summary>
        /// <param name="node">The first node</param>
        /// <param name="node2">The seconds node</param>
        /// <returns></returns>
		public static bool EqualsByGeoPosition(this OsmNode node, OsmNode node2)
		{
			return node.PositionSQL.EqualsManual(node2.PositionSQL);
		}
        /// <summary>
        /// Compares two OsmNode objects by id property.
        /// </summary>
        /// <param name="node">The first node</param>
        /// <param name="node2">The seconds node</param>
        /// <returns></returns>
		public static bool EqualsById(this OsmNode node, OsmNode node2)
		{
			return node.id == node2.id;
		}
        /// <summary>
        /// Compares two GeoPosition objects by Lat / Lon properties.
        /// </summary>
        /// <param name="geo">The first geo position</param>
        /// <param name="geo2">The second geo position</param>
        /// <returns></returns>
		public static bool EqualsManual(this GeoPosition geo, GeoPosition geo2)
		{
			return geo.Lat == geo2.Lat && geo.Lon == geo2.Lon;
		}
        /// <summary>
        /// Checks whether <paramref name="nodes"/> contain <paramref name="targetNode"/>.
        /// </summary>
        /// <param name="nodes">The list of OsmNode elements</param>
        /// <param name="targetNode">The seeking OsmNode.</param>
        /// <returns></returns>
		public static bool ContainsOsmNodeById(this List<OsmNode> nodes, OsmNode targetNode)
		{
			foreach (var n in nodes)
				if (n.EqualsById(targetNode))
					return true;
			return false;
		}
	}
}
