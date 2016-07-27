/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// The ObjectLifeTime class is used along with simulation scripts to control the lifetime of dynamic objects such as cars.
/// <para>Generally, one would use this on simulations that do not specify when to remove the objects.</para>
/// </summary>
public class ObjectLifeTime : MonoBehaviour {

    private DateTime _LastUpdate;
    public DateTime LastUpdate
    {
        get { return _LastUpdate; }
        set 
        { 
            _LastUpdate = value;
            DateInString = _LastUpdate.ToString();
        }
    }
    public String DateInString;
    
}
