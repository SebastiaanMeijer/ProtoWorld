using UnityEngine;
using System.Collections;

public class callBackFunctionTest : MonoBehaviour {

    void testCallbackRotating()
    {
        UnityEngine.Debug.Log("Calling rotating callback function");
    }

    void testCallbackSelected()
    {
        UnityEngine.Debug.Log("Calling selected callback function");
    }

    void testCallbackDeselected()
    {
        UnityEngine.Debug.Log("Calling deselected callback function");
    }

    void testCallbackMoving()
    {
        UnityEngine.Debug.Log("Calling moving callback function");
    }

    void testCallbackDropping()
    {
        UnityEngine.Debug.Log("Calling dropping callback function");
    }
}
