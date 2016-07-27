/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class GizmoDraw : MonoBehaviour
{

    public static void TransLine(Vector3 start, Vector3 end, Color solidColor, float radius, float linkHeight)
    {
        Gizmos.color = solidColor;
        float dist = Vector3.Distance(start, end);
        int limit = Mathf.RoundToInt(dist / radius);
        for (int i = 0; i < limit; i += 2)
        {
            var t = (float)i / (float)limit;
            var center = Vector3.Lerp(start, end, t);
            center.y = (-4 * Mathf.Pow((t - .5f), 2) + 1) * linkHeight;
            Gizmos.DrawSphere(center, radius);
        }
    }

    public static void WireTransLine(Vector3 start, Vector3 end, Color wireColor, float radius, float linkHeight)
    {
        Gizmos.color = wireColor;
        float dist = Vector3.Distance(start, end);
        int limit = Mathf.RoundToInt(dist / radius);
        for (int i = 0; i < limit; i += 2)
        {
            var t = (float)i / (float)limit;
            var center = Vector3.Lerp(start, end, t);
            center.y = (-4 * Mathf.Pow((t - .5f), 2) + 1) * linkHeight;
            Gizmos.DrawWireSphere(center, radius);
        }
    }

}
