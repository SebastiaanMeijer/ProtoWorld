/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GaPSLabsUnity.StateMachine
{
    public class StateInfoClass : MonoBehaviour
    {
        public static string BaseLayerName = "Base Layer.";
        public string[] States;

        [HideInInspector]
        public Dictionary<int, string> StateHash;

        public string[] StateParameters;

        [HideInInspector]
        private Dictionary<int, string> StateParametersHash;

        void Start()
        {
            StateHash = new Dictionary<int, string>();

            for (int i = 0; i < States.Length; i++)
            {
                StateHash.Add(Animator.StringToHash(BaseLayerName + States[i]), States[i]);
            }

            StateParametersHash = new Dictionary<int, string>();

            for (int i = 0; i < StateParameters.Length; i++)
            {
                StateParametersHash.Add(Animator.StringToHash(BaseLayerName + StateParameters[i]), StateParameters[i]);
            }

            //Debug.Log("StateInfoClass is initialized " + StateHash.Count);
        }
    }
}