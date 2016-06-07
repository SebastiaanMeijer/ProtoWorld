/*
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
