/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashCommutingBehaviour.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Actions to take while Flash Pedestrian is at state "Commuting".
/// </summary>
public class FlashCommutingBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Informs that the pedestrian has arrived at the station
        animator.GetComponent<FlashPedestriansController>().ArrivedWalkingAtStation();
    }
}
