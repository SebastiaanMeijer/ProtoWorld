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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;

namespace GapslabWCFservice
{
    [DataContract,StructLayout(LayoutKind.Sequential)]
    public class OsmNodeWCF
    {
        public OsmNodeWCF() { }
        public OsmNodeWCF(string id,int order,double lat,double lon) 
        {
            this.id = id;
            this.order = order;
            this.lat = lat;
            this.lon = lon;
        }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public int order { get; set; }
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lon { get; set; }
    }
}