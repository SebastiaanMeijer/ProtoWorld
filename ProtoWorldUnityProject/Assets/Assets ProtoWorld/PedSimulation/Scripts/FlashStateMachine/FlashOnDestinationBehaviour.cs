/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashOnDestinationBehaviour.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Actions to take while Flash Pedestrian is at state "Destination".
/// </summary>
public class FlashOnDestinationBehaviour : StateMachineBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    private bool loggerSetup = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!loggerSetup)
        {
            logSeriesId = LoggerAssembly.GetLogSeriesId();
            log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", " Destination log"));
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Destionation chosen"));
            loggerSetup = true;
        }

        // Recycle the pedestrian object
        var pedestrian = animator.GetComponent<FlashPedestriansController>();
        var destination = pedestrian.routing.destinationPoint.destinationName;
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "string", 0, destination));

        pedestrian.Recycle();
    }
}
