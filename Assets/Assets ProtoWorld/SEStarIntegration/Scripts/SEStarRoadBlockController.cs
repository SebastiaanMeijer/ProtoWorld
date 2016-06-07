using UnityEngine;
using System.Collections;

public class SEStarRoadBlockController : SEStarChangeStateController
{
    public void Block()
    {
        ChangeState("Blocked");
    }
    public bool block= false;
    public bool unblock= false;
    public void Update()
    {
        if (block)
        {
            Block();
            block = false;
        }
        else if (unblock)
        {
            Unblock();
            unblock = false;
        }
    }
    public void Unblock()
    {
        ChangeState("Unblocked");
    }
    public override void OnStateChange(string newState)
    {
        state = newState;
        Debug.Log("State was changed to: " + state);
    }
}
