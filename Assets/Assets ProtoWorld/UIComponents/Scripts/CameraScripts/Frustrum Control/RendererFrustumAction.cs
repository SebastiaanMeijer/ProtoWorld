using UnityEngine;
using System.Collections;

public class RendererFrustumAction : MonoBehaviour
{

    public void HideRenderer()
    {
        GetComponent<Renderer>().enabled = false;
    }
    public void ShowRenderer()
    { GetComponent<Renderer>().enabled = true; }
}
