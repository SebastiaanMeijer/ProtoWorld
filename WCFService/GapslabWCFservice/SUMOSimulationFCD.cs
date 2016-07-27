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
    [DataContract]
    public class SUMOSimulationFCD
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public TimeStep[] TimeStep { get; set; }
        [DataMember]
        public String FCDElementId = "fcd-export";
        public enum VehicleType
        {
            Default,
            Simple
        };
    }
    [DataContract]
    public class TimeStep
    {
        [DataMember]
        public float time;
        [DataMember]
        public VehicleFCD[] Vehicles;
        [DataMember]
        public DateTime iMobilityTime;
        [DataMember]
        public int index;
    }
    [DataContract]
    public class SimpleTimeStep
    {
        [DataMember]
        public float time;
        [DataMember]
        public string[] VehicleIds;
        [DataMember]
        public float[] VehicleLats;
        [DataMember]
        public float[] VehicleLongs;
        [DataMember]
        public DateTime iMobilityTime;
        [DataMember]
        public int index;
    }
    [DataContract]
    public class VehicleFCD
    {
        [DataMember]
        public SUMOSimulationFCD.VehicleType VehicleType { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public float Latitude { get; set; }
        [DataMember]
        public float Longitude { get; set; }
        [DataMember]
        public float Angle { get; set; }
        [DataMember]
        public float Speed { get; set; }
        [DataMember]
        public float Pos { get; set; }
        [DataMember]
        public string Lane { get; set; }
        [DataMember]
        public float Slope { get; set; }

    }
    
}