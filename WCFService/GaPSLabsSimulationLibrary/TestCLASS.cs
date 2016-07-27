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
    public class TestCLASS
    {
        public TimeStep t;
        public TestCLASS()
        {
            t = new TimeStep();
            t.Vehicles = new VehicleFCD [1];
            t.Vehicles[0] = new VehicleFCD();
            t.Vehicles[0].Id = "TEST";
            (t.Vehicles[0] as VehicleFCD).Speed = 5.1f;
            t.Vehicles[0].VehicleType = VehicleType.Simple;

        }
    }
}
