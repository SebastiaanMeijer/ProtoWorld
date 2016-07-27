/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using UnityEngine;

public class StageInfo
{
    public StationController Start { get; set; }

    public StationController End { get; set; }

    public float TravelTime { get; set; }

    public LineController Line { get; set; }

    public LineCategory Category { get { return Line.category; } }

    public LineDirection Direction { get; set; }

    public bool Equals(StageInfo other)
    {
        if (other == null)
            return false;

        if (this.Start.Equals(other.Start) && this.End.Equals(other.End))
            return true;
        else
            return false;
    }

    public override string ToString()
    {
        return String.Format("{0}-{1}: {2}s {3}, {4}",
            Start.stationName, End.stationName, TravelTime, Category.ToString(), LineController.MakeKeyString(Line.id, Direction));
    }

}