/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Linq;

namespace GaPSLabsUnity.StateMachine
{
    [RequireComponent(typeof(ClassSession))]
    public class StateMachineExperiment : StateMachineBase
    {
        [HideInInspector]
        public ClassSession Session;
        [Range (0.0f,1.0f)]
        public float CurrentInterest;

        /// <summary>
        /// NOTE: This needs to be more than the travel time to the library (temporary activity while waiting for the mode) and back.
        /// </summary>
        public float TooLongTheshold;

        public float ClassToLibraryDistance = 10;
        public float LibraryToModeDistance = 5;

        /// <summary>
        /// NOTE: This is a time of the day.
        /// </summary>
        public float NextAvailableModeForMe;
        public float WalkingTimeToMode;

        [HideInInspector]
        public int mode;
        public NavMeshAgent navAgent;

        public override void Init()
        {
            mode = Random.Range(0, 3);

            navAgent = gameObject.GetComponent<NavMeshAgent>();
            navAgent.SetDestination(transform.Find("Destiny").GetComponent<Transform>().position);

            onStateEnterEvent += onStateEnter;
            onStateStayEvent += onStateStay;
            onTransitionEvent += onTransition;
        }

        void onStateStay(string stateName)
        {
            Debug.LogWarning("Staying at state=" + stateName);
            switch (stateName)
            {
                case "atTheClass":
                    {
                        bool leaveClass = (Session.Finish - Time.time) <= (1 - CurrentInterest) * (Session.Finish - Session.Start);
                        animator.SetBool("leaveClass", leaveClass);
                        break;
                    }
                case "modeChoice":
                    {
                        animator.SetInteger("mode", mode);
                        break;
                    }
                case "Walk":
                case "Bicycle":
                    {
                        // Do nothing
                        break;
                    }
                case "Bus":
                case "Metro":
                    {
                        TooLongTheshold = (ClassToLibraryDistance + LibraryToModeDistance) * 2;
                        bool modeAvailableSoon = (NextAvailableModeForMe - Time.time) - WalkingTimeToMode < TooLongTheshold;
                        animator.SetBool("modeAvailableSoon", modeAvailableSoon);

                        break;
                    }
                case "atLibrary":
                    {
                        float timeToKill = NextAvailableModeForMe - (ClassToLibraryDistance + LibraryToModeDistance);
                        // WaitAtLibraryFor timeToKill minutes.
                        // Stay at the library for the duration of timeToKill
                        bool leaveLibrary = // We leave the library a minute earlier just to be on the safe side.(and float rounding errors)
                            (NextAvailableModeForMe - Time.time) < LibraryToModeDistance + 1;

                        animator.SetBool("modeAvailableSoon", leaveLibrary);
                        break;
                    }
                case "End":
                    {
                        Debug.Log("I'm done");
                        break;
                    }
                default:
                    {
                        Debug.LogWarning("TODO: State=" + stateName);
                        break;
                    }
            }
        }

        public void onTransition(int hashedName)
        {
            Debug.Log("Transitioning..");
        }

        public void onStateEnter(string stateName)
        {
           
        }

        public void Trigger()
        { animator.SetTrigger("generalTrigger"); }

    }
}