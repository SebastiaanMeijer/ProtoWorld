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

public class KPIDelays : MonoBehaviour
{
    public float TramDelay;
    public float BusDelay;
    public float TrainDelay;
    public float MetroDelay;
    public float CarDelay = 0;

    public Transform transLines;

    // Use this for initialization
    void Start()
    {
        transLines = GameObject.Find("TransLines").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //pause the gathering of data when a log is being loaded
        if (KPIParameters.pauseKPIS)
            return;

        float tmpTramDelay = 0;
        float tmpTrainDelay = 0;
        float tmpBusDelay = 0;
        float tmpMetroDelay = 0;

        foreach (Transform transLine in transLines)
        {
            foreach (Transform item in transLine)
            {
                VehicleController vc = item.GetComponent<VehicleController>();
                if (transLine.name.StartsWith("Tram_"))
                    tmpTramDelay += vc.delay;

                if (transLine.name.StartsWith("Train_"))
                    tmpTrainDelay += vc.delay;

                if (transLine.name.StartsWith("Bus_"))
                    tmpBusDelay += vc.delay;

                if (transLine.name.StartsWith("Metro_"))
                    tmpMetroDelay += vc.delay;
            }
        }

        TramDelay = tmpTramDelay;
        BusDelay = tmpBusDelay;
        TrainDelay = tmpTrainDelay;
        MetroDelay = tmpMetroDelay;
    }
}
