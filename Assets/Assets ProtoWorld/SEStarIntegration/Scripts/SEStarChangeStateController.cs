using UnityEngine;
using System.Collections;

public class SEStarChangeStateController : MonoBehaviour
{

    public uint Id;
    public string state;
    public SEStar sestar;
    
    public virtual void OnStateChange(string newState)
    { }
    public void ChangeState(string newState)
    { 
        state = newState;
        if (sestar != null)
            sestar.ChangeSEStarObjectState((uint)Id, state);
    }
}
