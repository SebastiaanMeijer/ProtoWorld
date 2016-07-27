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

namespace GaPSLabs.Geometry
{
    public class Color
    {
        public Color() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="R">A number between 0 and 255</param>
        /// <param name="G">A number between 0 and 255</param>
        /// <param name="B">A number between 0 and 255</param>
        public Color(int R,int G,int B)
        { 
            this.R = R;
            this.G = G;
            this.B = B; 
        }
        /// <summary>
        /// A number between 0 and 255
        /// </summary>
        public int R;
        /// <summary>
        /// A number between 0 and 255
        /// </summary>
        public int G;
        /// <summary>
        /// A number between 0 and 255
        /// </summary>
        public int B;
    }
}
