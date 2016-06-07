using UnityEngine;
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