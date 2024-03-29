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
 * FlashBikingBehaviour.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Actions to take while Flash Pedestrian is at state "Biking".
/// </summary>
public class FlashBikingBehaviour : StateMachineBehaviour 
{
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        FlashPedestriansController fpc = animator.GetComponent<FlashPedestriansController>();

        if (fpc.targetedBikeStation.capacityNumber > 0)
        {
            fpc.targetedBikeStation.PickBike();
            fpc.GoBikingToDestination();
        }
        else
        {
            fpc.BikeStationIsEmpty();
        }
	}
}
