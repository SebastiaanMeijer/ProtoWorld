/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;

public class ArrowUtils
{
    [System.Obsolete("Use CheckTransportationModuleExist() to check if Transportation exist.")]
    public static void CheckParent()
    {
        var transport = GameObject.Find("Transportation");
        if (transport == null)
        {
            transport = new GameObject("Transportation");
        }
        var rc = transport.GetComponent<RoutingController>();
        if (rc == null)
            transport.AddComponent<RoutingController>();
        var stations = GameObject.Find("Stations");
        if (stations == null)
        {
            stations = new GameObject("Stations");
        }
        var ss = stations.GetComponent<StationStatistics>();
        if (ss == null)
            stations.AddComponent<StationStatistics>();
        stations.transform.SetParent(transport.transform);
        var lines = GameObject.Find("TransLines");
        if (lines == null)
        {
            lines = new GameObject("TransLines");
        }
        var ls = lines.GetComponent<LineStatistics>();
        if (ls == null)
            lines.AddComponent<LineStatistics>();
        lines.transform.SetParent(transport.transform);
        var travelers = GameObject.Find("Travelers");
        if (travelers == null)
        {
            travelers = new GameObject("Travelers");
        }
        var ts = travelers.GetComponent<TravelerStatistics>();
        if (ts == null)
            travelers.AddComponent<TravelerStatistics>();
        travelers.transform.SetParent(transport.transform);
    }

}
