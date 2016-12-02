/* 
This file is part of ProtoWorld. 

ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 
*/

/*
 * KPI data for displaying in charts
 * 
 * Nathan van Ofwegen
 * Antony Löbker
 */

using UnityEngine;
using System.Collections;

public class KPIQueuePerType : MonoBehaviour
{
    public int busQueue = 0;

    private StationStatistics stationStatistics;
    public int trainQueue = 0;

    private LineStatistics lineStatistics;
    public int tramQueue = 0;

    // Use this for initialization
    void Start()
    {
        stationStatistics = GetComponentInChildren<StationStatistics>();
        lineStatistics = GetComponentInChildren<LineStatistics>();
    }

    // Update is called once per frame
    void Update()
    {
        busQueue = getBusQueue();
        trainQueue = stationStatistics.totalQueuing;
        tramQueue = lineStatistics.totalQueuing;
    }

    private int getBusQueue()
    {
        return 0;
    }
}