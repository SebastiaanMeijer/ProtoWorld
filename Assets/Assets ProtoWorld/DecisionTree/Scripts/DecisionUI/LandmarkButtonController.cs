using UnityEngine;
using System.Collections;

public class LandmarkButtonController : MonoBehaviour 
{
    public float zoomFactor = 0.5f;
    public GameObject landmarkObject;
    private CameraControl cameraControl;

    void Awake()
    {
        cameraControl = FindObjectOfType<CameraControl>();
    }

    public void FocusOnLandmark()
    {
        if (cameraControl != null && landmarkObject != null)
            cameraControl.FocusOnHotPoint(landmarkObject.transform, zoomFactor);
    }
}
