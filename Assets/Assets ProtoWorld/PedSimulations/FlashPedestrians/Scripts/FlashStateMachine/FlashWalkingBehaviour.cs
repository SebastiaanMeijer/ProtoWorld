/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashWalkingBehaviour.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Actions to take while Flash Pedestrian is at state "Walking".
/// </summary>
public class FlashWalkingBehaviour : StateMachineBehaviour 
{
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        // Walk to the next point in the route of the pedestrian
        animator.GetComponent<FlashPedestriansController>().WalkToNextPoint();
	}
}
