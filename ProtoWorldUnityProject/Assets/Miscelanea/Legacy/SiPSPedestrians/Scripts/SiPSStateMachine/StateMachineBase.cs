/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * GENERIC CLASS
 * StateMachineBase.cs
 * Aram Azhari
 * Michael van den Berg
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Linq;
using GaPSLabsUnity.StateMachine;

/// <summary>
/// Creates a generic implementation for behaviour control using a finite state machine. 
/// </summary>
public class StateMachineBase : MonoBehaviour
{
    public delegate void StateEventHandler(string stateName);
    public delegate void TransitionEventHandler(int hashedName);

    public event StateEventHandler onStateEnterEvent;
    public event StateEventHandler onStateStayEvent;
    public event StateEventHandler onStateExitEvent;
    public event TransitionEventHandler onTransitionEvent;

    public StateInfoClass StateInfoObject;

    [HideInInspector]
    public string CurrentState;
    private string PreviousState;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public AnimatorStateInfo info;

    /// <summary>
    /// Starts the script. 
    /// </summary>
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("Why is this null?");
        PreviousState = "noState";
        Init();
    }

    /// <summary>
    /// Interface for Init
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// Update method, runs handlers according to the behaviour of the finite state machine. 
    /// </summary>
    public void Update()
    {
        info = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.IsInTransition(0))
        {
            if (StateInfoObject.StateHash.TryGetValue(info.nameHash, out CurrentState))
            {
                if (CurrentState != PreviousState)
                {
                    if (onStateExitEvent != null)
                        onStateExitEvent(PreviousState);

                    if (onStateEnterEvent != null)
                        onStateEnterEvent(CurrentState);

                    PreviousState = CurrentState;
                }
                else
                {
                    if (onStateStayEvent != null)
                        onStateStayEvent(CurrentState);
                }
            }
        }
        else // I'm in the transition.
        {
            if (onTransitionEvent != null)
                onTransitionEvent(animator.GetAnimatorTransitionInfo(0).nameHash);
        }
    }
}
