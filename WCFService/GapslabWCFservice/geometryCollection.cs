/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GapslabWCFservice
{
	[DataContract]
	public class geometry_collection
	{
		public geometry_collection() { }
		[DataMember]
		public long id { get; set; }
		[DataMember]
		public string name { get; set; }
		[DataMember]
		public string version_title { get; set; }
		[DataMember]
		public int major { get; set; }
		[DataMember]
		public int minor { get; set; }
		[DataMember]
		public string format { get; set; }
		[DataMember]
		public long large_object_reference { get; set; }
		[DataMember]
		public int latitude { get; set; }
		[DataMember]
		public int longitude { get; set; }
		[DataMember]
		public float pivotx { get; set; }
		[DataMember]
		public float pivoty { get; set; }
		[DataMember]
		public float pivotz { get; set; }
		[DataMember]
		public int gis_id { get; set; }
		[DataMember]
		public string gis_type { get; set; }
		[DataMember]
		public DateTime last_update { get; set; }
	}


}