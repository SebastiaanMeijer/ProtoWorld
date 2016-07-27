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

namespace GaPSlabsSimulationLibrary
{
    [Serializable]
    public class VehicleFCD : VehicleBase
    {
        public override VehicleType VehicleType { get; set; }
        public override string Id { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Angle { get; set; }
        public float Speed { get; set; }
        public float Pos { get; set; }
        public string Lane { get; set; }
        public float Slope { get; set; }
    }
}
