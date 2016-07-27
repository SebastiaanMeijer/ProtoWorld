/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaPSLabs.Geometry
{
    public class DiscreetCurve
    {
        public List<float> data;
        public int Current = 0;
        private float min;
        private float max;
        public DiscreetCurve()
        {
            data = new List<float>();
        }
        public DiscreetCurve(DiscreetCurve curve)
        {
            data = curve.data;
            min = data.Min();
            max = data.Max();
        }
        public float this[int index]
        {
            get
            {
                if (index >= data.Count)
                    throw new IndexOutOfRangeException();
                else
                    return data[index];
            }
            set
            {
                if (index >= data.Count)
                    throw new IndexOutOfRangeException();
                else
                {
                    data[index] = value;
                    min = data.Min();
                    max = data.Max();
                }
            }
        }
        public float Min
        {
            get
            {
                UpdateMinMax();
                return min;
            }
        }
        public float Max
        {
            get
            {
                UpdateMinMax();
                return max;
            }
        }
        public float Average
        {
            get
            {
                UpdateMinMax();
                return (min + max) / 2f;
            }
        }

        public int Length
        {
            get { return data.Count; }
        }
        public float EvaluateCircular(int index)
        {
            return data[index % data.Count];
        }
        public float GetNextCircular()
        {
            return data[Current++ % data.Count];
        }
        public float GetPreviousCircular()
        {
            return data[Current-- % data.Count];
        }

        private void UpdateMinMax()
        {
            if (min == 0 || max == 0)
            {
                min = data.Min();
                max = data.Max();
            }
        }
    }
}
