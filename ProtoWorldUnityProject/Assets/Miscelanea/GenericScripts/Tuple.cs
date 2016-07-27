/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Class that defines a Tuple.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class Tuple<T1, T2>
{
    private T1 item1 { get; set; }
    private T2 item2 { get; set; }

    /// <summary>
    /// Constructor of the class.
    /// </summary>
    /// <param name="item1">Value of field item1.</param>
    /// <param name="item2">Value of field item2.</param>
    public Tuple(T1 item1, T2 item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }

    /// <summary>
    /// Get field item1.
    /// </summary>
    public T1 Item1
    {
        get { return item1; }
    }

    /// <summary>
    /// Get field item2.
    /// </summary>
    public T2 Item2
    {
        get { return item2; }
    }
}
