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
    private LineStatistics lineStatistics;
    private FlashPedestriansGlobalParameters pedestrianGlobals;

    public int busQueue = 0;
    public int trainQueue = 0;
    public int tramQueue = 0;
    public int metroQueue = 0;
    public int totalQueue = 0;

    // Use this for initialization
    void Start()
    {
        lineStatistics = GetComponentInChildren<LineStatistics>();
        pedestrianGlobals = GameObject.Find("FlashPedestriansModule").GetComponent<FlashPedestriansGlobalParameters>();
    }

    // Update is called once per frame
    void Update()
    {
        //pause the gathering of data when a log is being loaded
        if (KPIParameters.pauseKPIS)
            return;

        busQueue = pedestrianGlobals.numberOfPedestriansPerAgent * lineStatistics.busQueuing;
        trainQueue = pedestrianGlobals.numberOfPedestriansPerAgent * lineStatistics.trainQueuing;
        tramQueue = pedestrianGlobals.numberOfPedestriansPerAgent * lineStatistics.tramQueuing;
        metroQueue = pedestrianGlobals.numberOfPedestriansPerAgent * lineStatistics.metroQueuing;

        totalQueue = busQueue + trainQueue + tramQueue + metroQueue;
    }
}