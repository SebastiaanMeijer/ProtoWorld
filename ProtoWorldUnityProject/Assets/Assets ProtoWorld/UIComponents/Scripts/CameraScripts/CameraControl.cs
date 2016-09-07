/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * CAMERA CONTROL
 * CameraControl.cs
 * Aram Azhari
 * Miguel Ramos Carretero
 * Furkan Sonmez
 */

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A class for control the camera with an aerial view.
/// </summary>
public class CameraControl : MonoBehaviour
{
    // Logger 
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    // The camera won't go under this plane
    public GameObject backPlane = null;

    // Limit the minimum height of the camera in the y axis
    public float minHeight = 0.0f;

    // Limit the maximum height and borders in the x and y axis
    public float maxHeight = float.MaxValue;
    public float maxUp = float.MinValue;
    public float maxDown = float.MaxValue;
    public float maxRight = float.MaxValue;
    public float maxLeft = float.MinValue;
    public Vector3 temporaryVector3;

    // Options for the camera
    public bool allowRotation = true;
    public bool allowDraggingScene = true;
    public bool restrictMouseRotationToAxisY = true;
    public bool leftShiftIsEnabled = false;
    public bool spacebarIsEnabled = false;

    // Parameters to control the mouse-camera relation
    public bool dragging = false;
    public Vector3 initMouseWorldPosition;
    public Ray initMouseRay;
    public Vector3 initCameraPos;
    public Plane cameraPlane;
    public Vector3 straightDownFromInit;
    public Vector3 asIfHitPoint;
    public Vector3 targetCameraPosition;
    public float moveSpeed = 15;
    private Vector3 mouseWorldPosition;
    private Vector3 mouseScreenPosition;
    private Ray mouseRay;

    public TimeController timeController;

    // Parameters related to camera navigation and overview
    public GameObject targetPoint;
    public GameObject overviewPoint;
    public bool changeViewAtStart = false;
    private bool requestToTargetPoint = false;
    private bool interpolatingToTargetPoint = false;
    public float zoomFactor = 1.0f;
    private Transform oldPosition;
    private Transform newPosition;
    private float delta = 0.0f;
    private bool interpolatingToNewPosition = false;

    private bool blockControls = false;
    private float timeToUnblock = 0;

    private Vector3 keyBoardControlToMove = Vector3.zero;

	public GameObject ScrollbarMMV; 


	void Awake(){

		ScrollbarMMV = GameObject.Find ("ScrollbarMMV");

	}

    /// <summary>
    /// Initializes the camera control logic
    /// </summary>
    void Start()
    {
        targetCameraPosition = gameObject.transform.position;

        if (overviewPoint != null && changeViewAtStart)
            FocusOnOverviewPoint();
    }

    /// <summary>
    /// Updates the camera position according to controls.
    /// </summary>
    void Update()
    {
        #region FocusTargetPoint

        // If requested (by method FocusOnHotPoint()), set the camera coordinates to focus 
        // on the given target point
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

        #endregion FocusTargetPoint

        #region MouseLeftButton

        Vector3 oldMouseAt = mouseWorldPosition;
        Vector3 oldMousePosition = mouseScreenPosition;
        CollectMouseInfo();

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
            }

            //for camera fixed on a terrain trasposed straight up
            RaycastHit rayHit = new RaycastHit();
            Debug.DrawRay(asIfHitPoint, -mouseRay.direction * 100, Color.red);
            bool success = Physics.Raycast(new Ray(asIfHitPoint, -mouseRay.direction), out rayHit, 10000f, 1 << 9); // 9 is the ground layer

            if (success)
            {
                Vector3 movedGroundPoint = rayHit.point;
                targetCameraPosition = movedGroundPoint + (initCameraPos - straightDownFromInit);
            }
        }
        else
        {
            dragging = false;
        }

        #endregion MouseLeftButton

        #region MouseRightButton

        if (Input.GetMouseButton(1) && allowRotation)
        {
            Vector3 posChange = (oldMousePosition - Input.mousePosition);
            Vector3 oldRot = gameObject.transform.rotation.eulerAngles;

            gameObject.transform.RotateAround(Vector3.up, posChange.x * -0.005f);

            if (!restrictMouseRotationToAxisY)
                gameObject.transform.RotateAround(gameObject.transform.right, posChange.y * 0.005f);
        }

        #endregion MouseRightButton

        #region ScrollWheel

        // Controls the height of the camera
        if (!dragging)
            UpdateCameraHeight(Input.GetAxis("Mouse ScrollWheel"));

        #endregion ScrollWheel

        #region ArrowKeys

        CollectMouseInfo();

        if (Input.GetKey(KeyCode.LeftArrow) && !blockControls)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                keyBoardControlToMove -= transform.right * 100;
            else
                keyBoardControlToMove -= transform.right * 10;
        }

        if (Input.GetKey(KeyCode.RightArrow) && !blockControls)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                keyBoardControlToMove += transform.right * 100;
            else
                keyBoardControlToMove += transform.right * 10;

        }

        if (Input.GetKey(KeyCode.UpArrow) && !blockControls)
        {
            if (this.GetComponent<Camera>().orthographic == false)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    keyBoardControlToMove += transform.forward * 100;
                else
                    keyBoardControlToMove += transform.forward * 10;
            }
            else
            {
                keyBoardControlToMove.x = keyBoardControlToMove.x - 10;
            }
        }

        if (Input.GetKey(KeyCode.DownArrow) && !blockControls)
        {
            if (this.GetComponent<Camera>().orthographic == false)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    keyBoardControlToMove -= transform.forward * 100;
                else
                    keyBoardControlToMove -= transform.forward * 10;

            }
            else
            {
                keyBoardControlToMove.x = keyBoardControlToMove.x + 10;
            }
        }

        #endregion ArrowKeys

        #region MoveCamera

        keyBoardControlToMove.Normalize();

        if (keyBoardControlToMove.sqrMagnitude > 0.1f)
        {
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

            keyBoardControlToMove = Vector3.zero;
        }

        RaycastHit upRayHit = new RaycastHit();

        if (!Physics.Raycast(new Ray(targetCameraPosition, Vector3.down), out upRayHit, 10000f, 1 << 9)) // 9 is the ground layer
            Physics.Raycast(new Ray(targetCameraPosition, Vector3.up), out upRayHit, 10000f, 1 << 9);

        if (targetCameraPosition.y < upRayHit.point.y + 5)
        {
            targetCameraPosition.y = upRayHit.point.y + 5;
        }

        #endregion Normalize

        #region Others


        if (targetCameraPosition.z < maxRight && targetCameraPosition.z > maxLeft && targetCameraPosition.x > maxUp && targetCameraPosition.x < maxDown)
        {
            //Debug.Log("INSIDE OF BOUNDARY");

        }
        else
        {
            Debug.Log("OUTSIDE OF BOUNDARY");
            if (targetCameraPosition.z > maxRight)
            {
                Debug.Log("MAXRIGHT");
                targetCameraPosition.z = maxRight - 5;
            }
            if (targetCameraPosition.z < maxLeft)
            {
                Debug.Log("MAXLEFT");
                targetCameraPosition.z = maxLeft + 5;
            }
            if (targetCameraPosition.x < maxUp)
            {
                Debug.Log("MAXUP");
                targetCameraPosition.x = maxUp + 5;
            }
            if (targetCameraPosition.x > maxDown)
            {
                Debug.Log("MAXDOWN");
                targetCameraPosition.x = maxDown - 5;
            }
        }

        // Interpolates to the target point
        gameObject.transform.position = gameObject.transform.position + (targetCameraPosition - gameObject.transform.position) * 0.2f;


        // If there is a block, update the time until unblock
        if (blockControls)
        {
            timeToUnblock -= Time.deltaTime;
            if (timeToUnblock <= 0)
                blockControls = false;
        }

        #endregion Others
    }

    /// <summary>
    /// Collects the mouse information in relation to the scenario.
    /// </summary>
    private void CollectMouseInfo()
    {
        mouseScreenPosition = Input.mousePosition;
        mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit = new RaycastHit();
        Physics.Raycast(mouseRay, out rayHit, 10000f, 1 << 9); // 9 is the ground layer
        mouseWorldPosition = rayHit.point;
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
            //log.Info("Camera focused on hot point " + hotPointTransform.ToString());
            this.zoomFactor = zoomFactor;
            requestToTargetPoint = true;
        }
    }

    /// <summary>
    /// Requests to move the camera smoothly to a hot point.
    /// </summary>
    /// <param name="position">Position of the hot point.</param>
    /// <param name="rotation">Rotation of the hot point.</param>
    /// <param name="zoomFactor">Amount of zoom (higher values will zoom the camera more)</param>
    public void FocusOnHotPoint(Vector3 position, Quaternion rotation, float zoomFactor = 1.0f)
    {
        //Debug.Log("Moving smoothly to " + hotPointName);
        targetPoint.transform.position = position;
        targetPoint.transform.rotation = rotation;

        if (targetPoint != null)
        {
            //log.Info("Camera focused on hot point " + hotPointTransform.ToString());
            this.zoomFactor = zoomFactor;
            requestToTargetPoint = true;
        }
    }

    /// <summary>
    /// Focus the camera on the overview point defined in the public properties.
    /// </summary>
    public void FocusOnOverviewPoint()
    {
        if (overviewPoint != null)
            FocusOnHotPoint(overviewPoint.transform, zoomFactor);
    }

    /// <summary>
    /// Blocks the player controls for a certain amount of seconds.
    /// </summary>
    /// <param name="seconds">Number of seconds the controls are blocked.</param>
    /// <param name="indefinite">If true, the block will be indefinite.</param>
    public void BlockPlayerControls(float seconds, bool indefinite = false)
    {
        blockControls = true;

        if (indefinite)
            timeToUnblock = float.MaxValue; // This is a hack (not really indefinite)
        else
            timeToUnblock = seconds;
    }

    /// <summary>
    /// Unblock the player controls. 
    /// </summary>
    public void UnblockPlayerControls()
    {
        blockControls = false;
        timeToUnblock = 0f;
    }

    /// <summary>
    /// Updates the height of the camera given an increase value.
    /// </summary>
    /// <param name="deltaHeight">Amount of increase in height (negative values decrease the height).</param>
    internal void UpdateCameraHeight(float deltaHeight)
    {
        if (!this.GetComponent<Camera>().orthographic)
        {
            targetCameraPosition += Vector3.up * deltaHeight * -moveSpeed;

            // Clamp the height of the camera between the limits.
            targetCameraPosition = new Vector3(targetCameraPosition.x,
                Mathf.Clamp(targetCameraPosition.y, minHeight, maxHeight),
                targetCameraPosition.z);
        }
        else
        {
            // Clamp the ortographicSize of the camera between the height limits.
            this.GetComponent<Camera>().orthographicSize =
                Mathf.Clamp(this.GetComponent<Camera>().orthographicSize + deltaHeight * -moveSpeed,
                minHeight, maxHeight);
        }

		//ScrollbarMMV.GetComponent<ZoomScrollbarMMV> ().height = targetCameraPosition.y;
		ScrollbarMMV.GetComponent<ZoomScrollbarMMV> ().zoomedInOut(targetCameraPosition.y);

    }

    /// <summary>
    /// Updates the XZ positions of the camera given the values.
    /// </summary>
    /// <param name="deltaX">Amount of increase in X.</param>
    /// <param name="deltaZ">Amount of increase in Z.</param>
    internal void UpdateCameraXZPosition(float deltaX, float deltaZ)
    {
        if (!this.GetComponent<Camera>().orthographic)
        {
            targetCameraPosition += Vector3.forward * deltaX * -moveSpeed;
            targetCameraPosition += Vector3.left * deltaZ * -moveSpeed;
        }
        else
        {
            keyBoardControlToMove.x = keyBoardControlToMove.x + deltaX;
            keyBoardControlToMove.z = keyBoardControlToMove.z + deltaZ;
            //keyBoardControlToMove -= transform.right * deltaZ * 10;
        }
    }
}








