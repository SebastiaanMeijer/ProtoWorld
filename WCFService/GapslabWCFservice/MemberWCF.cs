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
using System.Runtime.Serialization;
using System.Web;

namespace GapslabWCFservice
{

    /// <summary>
    /// Defines an OpenstreetMaps Relation member. Refer to OpenstreetMaps wiki for more.
    /// </summary>
    [DataContract]
    public class MemberWCF
    {
        public enum RouteCorrection { RemoveDuplicates, FixReversedLine, RemoveDeadends }
        // public enum MemberType { Way, Node };
        /// <summary>
        /// Gets or sets the relation id.
        /// </summary>
        [DataMember]
        public String RelationId { get; set; }
        /// <summary>
        /// Gets or sets the referenced element id.
        /// </summary>
        [DataMember]
        public String ReferenceId { get; set; }
        /// <summary>
        /// 0 for Node, 1 for Way, 2 for Relation
        /// </summary>
        [DataMember]
        public int Type { get; set; }
        /// <summary>
        /// Gets or sets the role of this member in the relation.
        /// </summary>
        [DataMember]
        public String Role { get; set; }
        /// <summary>
        /// Gets or sets the order of the member in the relation.
        /// </summary>
        [DataMember]
        public int order { get; set; }
    }
}