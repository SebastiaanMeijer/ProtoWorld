using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class FocusOnClickController : MonoBehaviour
{
    public float zoomFactor = 5.0f;
    private CameraControl cameraControl;

    void Awake()
    {
        cameraControl = FindObjectOfType<CameraControl>();
    }

    /// <summary>
    /// Handler for when the mouse is over the object. 
    /// </summary>
    void OnMouseOver()
    {
        // Left button updates the UI
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject() /*This condition avoids clicks passing through the GUI elements*/)
        {
            if (cameraControl != null)
                cameraControl.FocusOnHotPoint(this.transform, zoomFactor);
        }
    }
}
