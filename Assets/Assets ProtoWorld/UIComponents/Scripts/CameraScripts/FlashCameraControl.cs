/*
 * 
 * CAMERA CONTROL
 * FlashCameraControl.cs
 * Miguel Ramos Carretero
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// A class for control the camera with an aerial view for the Flash Pedestrian simulation.
/// </summary>
public class FlashCameraControl : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// The minimum height of the camera. The camera won't go under this plane.
    /// </summary>
    public GameObject backPlane = null;

    /// <summary>
    /// Avoid rotation of the camera in other axis apart from the Y axis.
    /// </summary>
    public bool restrictMouseRotationToAxisY = true;

    public bool dragging = false;

    public Vector3 initMouseWorldPosition;

    public Ray initMouseRay;

    public Vector3 initCameraPos;

    public Plane cameraPlane;

    public Vector3 straightDownFromInit;

    public Vector3 asIfHitPoint;

    public Vector3 targetCameraPosition;

    public float moveSpeed = 15;

    public TimeController timeController;

    private Vector3 mouseWorldPosition;

    private Vector3 mouseScreenPosition;

    private Ray mouseRay;

    public GameObject targetPoint;

    private bool requestToTargetPoint = false;

    private bool interpolatingToTargetPoint = false;

    private float zoomFactor = 1.0f;

    private Transform oldPosition;

    private Transform newPosition;

    private float delta = 0.0f;

    private bool interpolatingToNewPosition = false;

    /// <summary>
    /// Initializes the camera control logic
    /// </summary>
    void Start()
    {
        targetCameraPosition = gameObject.transform.position;
    }

    /// <summary>
    /// Updates the camera position according to controls.
    /// </summary>
    void Update()
    {
        // If requested (by method FocusOnHotPoint()), set the camera coordinates to focus on the given target point
        if (requestToTargetPoint)
        {
            gameObject.transform.forward = -targetPoint.transform.forward;
            gameObject.transform.Rotate(45, 0, 0, Space.Self);

            // Get the world coordinates depending on the zoomFactor.
            Vector3 targetInWorld;
            if (zoomFactor != 0)
                targetInWorld = targetPoint.transform.TransformPoint(new Vector3(0, 250 / zoomFactor, 250 / zoomFactor));
            else
                targetInWorld = targetPoint.transform.TransformPoint(new Vector3(0, 0, 0));


            // Make the transition smoothly
            targetCameraPosition = targetInWorld;

            requestToTargetPoint = false;
        }

        Vector3 oldMouseAt = mouseWorldPosition;
        Vector3 oldMousePosition = mouseScreenPosition;
        setMousePositions();

        if (Input.GetMouseButton(0))
        {
            if (!dragging)
            {
                dragging = true;
                initMouseRay = mouseRay;
                initMouseWorldPosition = mouseWorldPosition;
                initCameraPos = gameObject.transform.position;

                RaycastHit downRayHit = new RaycastHit();
                Physics.Raycast(new Ray(initCameraPos, Vector3.down), out downRayHit, 10000f, 1 << 9); // 9 is the ground layer
                straightDownFromInit = downRayHit.point;

                asIfHitPoint = straightDownFromInit + (initMouseWorldPosition - initCameraPos);
                //Debug.Log(initCameraPos+":"+straightDownFromInit+":"+asIfHitPoint);
            }

            //for camera fixed on a terrain trasposed to itself
            //			Ray asIfCameraRay = new Ray(initCameraPos, mouseRay.direction);
            //			RaycastHit rayHit = new RaycastHit();
            //			Physics.Raycast(asIfCameraRay, out rayHit);
            //			Vector3 asIfWorldPosition = rayHit.point;
            //			
            //			targetCameraPosition = initCameraPos + (initMouseWorldPosition - asIfWorldPosition);

            // for camera fixed on a plane
            //			cameraPlane = new Plane (Vector3.up, camera.transform.position);
            //			Ray rayFromMousePoint = new Ray(initMouseWorldPosition, mouseRay.direction);
            //			float dist = 0;
            //			cameraPlane.Raycast(rayFromMousePoint, out dist);
            //			targetCameraPosition = rayFromMousePoint.GetPoint(dist);

            //for camera fixed on a terrain trasposed straight up
            RaycastHit rayHit = new RaycastHit();
            Debug.DrawRay(asIfHitPoint, -mouseRay.direction * 100, Color.red);
            bool success = Physics.Raycast(new Ray(asIfHitPoint, -mouseRay.direction), out rayHit, 10000f, 1 << 9); // 9 is the ground layer

            if (success)
            {

                Vector3 movedGroundPoint = rayHit.point;
                targetCameraPosition = movedGroundPoint + (initCameraPos - straightDownFromInit);
                //Debug.Log(success+":"+movedGroundPoint+":"+(initCameraPos - straightDownFromInit));
            }
        }
        else
        {
            dragging = false;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 posChange = (oldMousePosition - Input.mousePosition);
            Vector3 oldRot = gameObject.transform.rotation.eulerAngles;

            gameObject.transform.RotateAround(Vector3.up, posChange.x * -0.005f);

            if (!restrictMouseRotationToAxisY)
                gameObject.transform.RotateAround(gameObject.transform.right, posChange.y * 0.005f);
        }

        if (!dragging)
        {
            //Edited by Furkan Sonmez, if the camera is in perspective, then the usual code is called, but if it is not, the controls are a bit different
            if (this.GetComponent<Camera>().orthographic == false)
            {
                targetCameraPosition += Vector3.up * Input.GetAxis("Mouse ScrollWheel") * -30;
            }
            else
            {
                this.GetComponent<Camera>().orthographicSize = this.GetComponent<Camera>().orthographicSize + Input.GetAxis("Mouse ScrollWheel") * -30;
                if (this.GetComponent<Camera>().orthographicSize < 5)
                {
                    this.GetComponent<Camera>().orthographicSize = 5;
                }
            }
        }
        setMousePositions();

        Vector3 keyBoardControlToMove = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            keyBoardControlToMove -= transform.right * 10;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            keyBoardControlToMove += transform.right * 10;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //Edited by Furkan Sonmez, if the camera is in perspective, then the usual code is called, but if it is not, the controls are a bit different
            if (this.GetComponent<Camera>().orthographic == false)
            {
                keyBoardControlToMove += transform.forward * 10;
            }
            else
            {
                keyBoardControlToMove.z = keyBoardControlToMove.z + 10;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //Edited by Furkan Sonmez, if the camera is in perspective, then the usual code is called, but if it is not, the controls are a bit different
            if (this.GetComponent<Camera>().orthographic == false)
            {
                keyBoardControlToMove -= transform.forward * 10;
            }
            else
            {
                keyBoardControlToMove.z = keyBoardControlToMove.z - 10;
            }
        }

        keyBoardControlToMove.Normalize();

        if (keyBoardControlToMove.sqrMagnitude > 0.1f)
        {
            //FIXME: Make all scenes consider timeController eventually (so this "if" will not be needed).
            if (timeController != null)
                keyBoardControlToMove *= moveSpeed * Time.deltaTime / timeController.timeVelocity;
            else
                keyBoardControlToMove *= moveSpeed * Time.deltaTime;

            RaycastHit targetRayHit = new RaycastHit();
            if (!Physics.Raycast(new Ray(targetCameraPosition, Vector3.down), out targetRayHit, 10000f, 1 << 9)) // 9 is the ground layer
                Physics.Raycast(new Ray(targetCameraPosition, Vector3.up), out targetRayHit, 10000f, 1 << 9);
            float height = targetCameraPosition.y - targetRayHit.point.y;
            targetCameraPosition += keyBoardControlToMove;
            targetRayHit = new RaycastHit();
            if (!Physics.Raycast(new Ray(targetCameraPosition, Vector3.down), out targetRayHit, 10000f, 1 << 9)) // 9 is the ground layer
                Physics.Raycast(new Ray(targetCameraPosition, Vector3.up), out targetRayHit, 10000f, 1 << 9);
            targetCameraPosition.y = targetRayHit.point.y + height;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            //FIXME: Make all scenes consider timeController eventually (so this "if" will not be needed).
            if (timeController != null)
            {
                //Edited by Furkan Sonmez, if the camera is in perspective, then the usual code is called, but if it is not, the controls are a bit different
                if (this.GetComponent<Camera>().orthographic == false)
                {
                    targetCameraPosition += Vector3.up * moveSpeed * Time.deltaTime / timeController.timeVelocity;
                }
                else
                {
                    this.GetComponent<Camera>().orthographicSize = this.GetComponent<Camera>().orthographicSize + moveSpeed * Time.deltaTime / timeController.timeVelocity;
                    if (this.GetComponent<Camera>().orthographicSize < 5)
                    {
                        this.GetComponent<Camera>().orthographicSize = 5;
                    }
                }


                // targetCameraPosition += Vector3.up * moveSpeed * Time.deltaTime / timeController.timeVelocity;
            }
            else
            {

                //Edited by Furkan Sonmez, if the camera is in perspective, then the usual code is called, but if it is not, the controls are a bit different
                if (this.GetComponent<Camera>().orthographic == false)
                {
                    targetCameraPosition += Vector3.up * moveSpeed * Time.deltaTime;
                }
                else
                {
                    this.GetComponent<Camera>().orthographicSize = this.GetComponent<Camera>().orthographicSize + moveSpeed * Time.deltaTime;
                    if (this.GetComponent<Camera>().orthographicSize < 5)
                    {
                        this.GetComponent<Camera>().orthographicSize = 5;
                    }
                }


            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {

            //FIXME: Make all scenes consider timeController eventually (so this "if" will not be needed).
            if (timeController != null)
            {
                //Edited by Furkan Sonmez, if the camera is in perspective, then the usual code is called, but if it is not, the controls are a bit different
                if (this.GetComponent<Camera>().orthographic == false)
                {
                    targetCameraPosition -= Vector3.up * moveSpeed * Time.deltaTime / timeController.timeVelocity;
                }
                else
                {
                    this.GetComponent<Camera>().orthographicSize = this.GetComponent<Camera>().orthographicSize - moveSpeed * Time.deltaTime / timeController.timeVelocity;
                    if (this.GetComponent<Camera>().orthographicSize < 5)
                    {
                        this.GetComponent<Camera>().orthographicSize = 5;
                    }
                }


                // targetCameraPosition += Vector3.up * moveSpeed * Time.deltaTime / timeController.timeVelocity;
            }
            else
            {

                //Edited by Furkan Sonmez, if the camera is in perspective, then the usual code is called, but if it is not, the controls are a bit different
                if (this.GetComponent<Camera>().orthographic == false)
                {
                    targetCameraPosition -= Vector3.up * moveSpeed * Time.deltaTime;
                }
                else
                {
                    this.GetComponent<Camera>().orthographicSize = this.GetComponent<Camera>().orthographicSize - moveSpeed * Time.deltaTime;
                    if (this.GetComponent<Camera>().orthographicSize < 5)
                    {
                        this.GetComponent<Camera>().orthographicSize = 5;
                    }
                }


            }



            //FIXME: Make all scenes consider timeController eventually (so this "if" will not be needed).
            // if (timeController != null)
            //     targetCameraPosition -= Vector3.up * moveSpeed * Time.deltaTime / timeController.timeVelocity;
            // else
            //     targetCameraPosition -= Vector3.up * moveSpeed * Time.deltaTime;
        }

        RaycastHit upRayHit = new RaycastHit();

        if (!Physics.Raycast(new Ray(targetCameraPosition, Vector3.down), out upRayHit, 10000f, 1 << 9)) // 9 is the ground layer
            Physics.Raycast(new Ray(targetCameraPosition, Vector3.up), out upRayHit, 10000f, 1 << 9);

        if (targetCameraPosition.y < upRayHit.point.y + 5)
        {
            targetCameraPosition.y = upRayHit.point.y + 5;
        }

        // Interpolates to the target point
        gameObject.transform.position = gameObject.transform.position + (targetCameraPosition - gameObject.transform.position) * 0.2f;

        if (interpolatingToNewPosition)
        {
            Debug.Log("Interpolating " + delta);
            this.transform.position = Vector3.Lerp(oldPosition.position, newPosition.position, delta);
            this.transform.rotation = Quaternion.Lerp(oldPosition.rotation, newPosition.rotation, delta);
            delta += 0.05f;

            if (delta >= 1.0f)
            {
                delta = 0.0f;
                interpolatingToNewPosition = false;
            }
        }

        //		RaycastHit rayHit = new RaycastHit();
        //		Physics.Raycast(new Ray(gameObject.transform.position, Vector3.down), out rayHit, 10000f, 1 << 9); // 9 is the ground layer
        //		float distToGround = rayHit.distance;
        //		
        //		gameObject.transform.position+=Vector3.up*(desiredDistFromGround-distToGround);
    }

    /// <summary>
    /// Calculates the mouse position in 3d space.
    /// </summary>
    private void setMousePositions()
    {

        mouseScreenPosition = Input.mousePosition;
        mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit = new RaycastHit();
        Physics.Raycast(mouseRay, out rayHit, 10000f, 1 << 9); // 9 is the ground layer
        mouseWorldPosition = rayHit.point;

    }

    /// <summary>
    /// Gets the position of the mouse in 3D space.
    /// </summary>
    /// <returns></returns>
    public Vector3 getMouseColliderPos()
    {
        return mouseWorldPosition;
    }

    /// <summary>
    /// Requests to move the camera smoothly to a hot point.
    /// </summary>
    /// <param name="hotPointName">Name of the hot point.</param>
    /// <param name="zoomFactor">Amount of zoom (higher values will zoom the camera more)</param>
    public void FocusOnHotPoint(string hotPointName, float zoomFactor = 1.0f)
    {
        //Debug.Log("Moving smoothly to " + hotPointName);

        targetPoint.transform.position = GameObject.Find(hotPointName).transform.position;

        if (targetPoint != null)
        {
            log.Info("Camera focused on hot point " + hotPointName);
            requestToTargetPoint = true;
        }
    }

    /// <summary>
    /// Requests to move the camera smoothly to a hot point.
    /// </summary>
    /// <param name="hotPointTransform">Transform of the hot point.</param>
    /// <param name="zoomFactor">Amount of zoom (higher values will zoom the camera more)</param>
    public void FocusOnHotPoint(Transform hotPointTransform, float zoomFactor = 1.0f)
    {
        //Debug.Log("Moving smoothly to " + hotPointName);
        targetPoint.transform.position = hotPointTransform.position;
        targetPoint.transform.rotation = hotPointTransform.rotation;

        if (targetPoint != null)
        {
            log.Info("Camera focused on hot point " + hotPointTransform.ToString());
            this.zoomFactor = zoomFactor;
            requestToTargetPoint = true;
        }
    }

    /// <summary>
    /// Interpolates the current camera position to a new position specify by the given Transform.
    /// </summary>
    /// <param name="startingCameraPosition">Transform containing the new position.</param>
    internal void InterpolateTransformTo(Transform startingCameraPosition)
    {
        Debug.Log("Preparing interpolation");
        oldPosition = this.transform;
        newPosition = startingCameraPosition;
        interpolatingToNewPosition = true;
    }
}









