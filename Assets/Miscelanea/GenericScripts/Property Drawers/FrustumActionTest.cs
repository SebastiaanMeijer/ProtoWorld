using UnityEngine;
using System.Collections;

public class FrustumActionTest : MonoBehaviour
{

    private CameraBasedAction aicontroller;
    public void Start()
    {
        aicontroller = GetComponent<CameraBasedAction>();
        aicontroller.VicinityChanged += aicontroller_VicinityChanged;
    }

    void aicontroller_VicinityChanged(CameraBasedAction.CameraVisibilityChangeEventArgs e)
    {
        switch(e.Frustum)
        {
            case CameraBasedAction.CameraVisibilityChangeEventArgs.FrustumState.Enter:
                {
                    if (e.ObjectInVicinity)
                        GetComponent<Renderer>().enabled = true;
                    else 
                        GetComponent<Renderer>().enabled = false;
                    break;
                }
            case CameraBasedAction.CameraVisibilityChangeEventArgs.FrustumState.StayInside:
                {
                    if (e.ObjectInVicinity)
                        GetComponent<Renderer>().enabled = true;
                    else
                        GetComponent<Renderer>().enabled = false;
                    break;
                }
            case CameraBasedAction.CameraVisibilityChangeEventArgs.FrustumState.Leave:
                {
                    GetComponent<Renderer>().enabled = false;
                    break;
                }
        }
    }


}
