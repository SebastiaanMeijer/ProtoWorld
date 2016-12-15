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

public class KPIDeltaDelaysPerType : MonoBehaviour
{
    public float deltaBusDelay = 0;
    public float deltaCarDelay = 0;
    public float deltaTramDelay = 0;
    public float deltaTrainDelay = 0;
    public float deltaMetroDelay = 0;

    private KPIDelays delays;
    private Transform transLines;

    private float oldBusDelay = 0;
    private float oldCarDelay = 0;
    private float oldTramDelay = 0;
    private float oldTrainDelay = 0;
    private float oldMetroDelay = 0;

    private float timeout = 0;

    // Use this for initialization
    void Start()
    {
        delays = transform.GetComponent<KPIDelays>();
        if (delays != null && delays.transLines != null)
            transLines = delays.transLines;
        else
            transLines = GameObject.Find("TransLines").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //pause the gathering of data when a log is being loaded
        if (KPIParameters.pauseKPIS)
            return;

        //run only once per second, otherwise the delays are not noticable
        timeout += Time.deltaTime;
        if (timeout >= 1)
        {
            getCarDeltaDelay();
            getPublicTransportDeltaDelay();
            timeout--;
        }
    }

    private void getCarDeltaDelay()
    {
        deltaCarDelay = delays.CarDelay - oldCarDelay;
        oldCarDelay = delays.CarDelay;
    }

    private void getPublicTransportDeltaDelay()
    {
        deltaBusDelay = delays.BusDelay - oldBusDelay;
        deltaTramDelay = delays.TramDelay - oldTramDelay;
        deltaTrainDelay = delays.TrainDelay - oldTrainDelay;
        deltaMetroDelay = delays.MetroDelay - oldMetroDelay;

        oldBusDelay = delays.BusDelay;
        oldTramDelay = delays.TramDelay;
        oldTrainDelay = delays.TrainDelay;
        oldMetroDelay = delays.MetroDelay;
    }
}