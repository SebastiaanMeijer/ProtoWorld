using UnityEngine;
using System.Collections;

public class MultitouchController : MonoBehaviour
{
    [Range(0.01f, 1.0f)]
    public float touchSensivity = 0.01f;

    public bool movingDragAndDropObject = false;

    private CameraControl cameraControl;
    private Vector2 worldStartPoint;

    // Use this for initialization
    void Start()
    {
        cameraControl = FindObjectOfType<CameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraControl != null && !movingDragAndDropObject)
        {
            Touch[] touches = Input.touches;

            switch (Input.touchCount)
            {
                // One touch
                case 1:

                    Debug.Log("Single touching");

                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                        worldStartPoint = GetWorldPoint(Input.GetTouch(0).position);

                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        Vector2 worldDelta = GetWorldPoint(Input.GetTouch(0).position) - worldStartPoint;
                        
                        cameraControl.UpdateCameraXZPosition(-worldDelta.x, -worldDelta.y);
                    }

                    //if (touches[0].tapCount == 2)
                    //{
                    //    // Zoom in in the position raycasted on the map
                    //    Debug.Log("Doubletap: Focusing on position touched");

                    //    Ray touchRay = GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
                    //    RaycastHit rayHit = new RaycastHit();
                    //    Physics.Raycast(touchRay, out rayHit, 10000f, 1 << 9); // 9 is the ground layer

                    //    cameraControl.FocusOnHotPoint(rayHit.transform);
                    //}

                    break;

                // Two touches
                case 2:

                    Debug.Log("Multiple touching");

                    Vector2 firstTouchPreviousPos = touches[0].position
                        - touches[0].deltaPosition;

                    Vector2 secondTouchPreviousPos = touches[1].position
                        - touches[1].deltaPosition;

                    float deltaDistance =
                        Vector2.Distance(touches[0].position, touches[1].position) -
                        Vector2.Distance(firstTouchPreviousPos, secondTouchPreviousPos);

                    if (deltaDistance > 0)
                    {
                        // The distance is increasing: apply zoom out
                        Debug.Log("Multitouch: Zooming out");
                        cameraControl.UpdateCameraHeight(deltaDistance * touchSensivity);
                    }
                    else if (deltaDistance < 0)
                    {
                        // The distance is decreasing: apply zoom in
                        Debug.Log("Multitouch: Zooming in");
                        cameraControl.UpdateCameraHeight(deltaDistance * touchSensivity);
                    }

                    break;
                
                // Default behaviour
                default:

                    // Do nothing
                    break;
            }
        }
    }

    // convert screen point to world point
    private Vector2 GetWorldPoint(Vector2 screenPoint)
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hit);
        return hit.point;
    }
}
