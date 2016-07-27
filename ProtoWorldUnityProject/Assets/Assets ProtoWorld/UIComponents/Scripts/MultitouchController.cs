/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MultitouchController : MonoBehaviour
{
    [Range(0.01f, 1.0f)]
    public float touchSensivity = 0.01f;
    
    public Camera camera;

    private bool blockNavigation = false;

    private CameraControl cameraControl;
    private Vector3 worldStartPoint;

    // Use this for initialization
    void Start()
    {
        camera = this.GetComponent<Camera>();
        cameraControl = FindObjectOfType<CameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraControl != null)
        {
            Touch[] touches = Input.touches;

            switch (Input.touchCount)
            {
                // One touch
                case 1:

                    if (!blockNavigation)
                    {
                        Debug.Log("Single touching");

                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                            worldStartPoint = GetWorldPoint(Input.GetTouch(0).position);

                        if (Input.GetTouch(0).phase == TouchPhase.Moved)
                        {
                            Vector2 touchPosition = Input.GetTouch(0).position;
                            Vector3 worldPoint = GetWorldPoint(touchPosition);
                            Vector3 worldDelta = worldPoint - worldStartPoint;

                            cameraControl.UpdateCameraXZPosition(-worldDelta.x, -worldDelta.z);
                            
                            // Log (only for the development built!)
                            //Debug.LogError(System.String.Format("Touch point: {0}, {1}", touchPosition.x, touchPosition.y));
                            //Debug.LogError(System.String.Format("World Point: {0}, {1}, {2}", worldPoint.x, worldPoint.y, worldPoint.z));
                            //Debug.LogError(System.String.Format("World Start: {0}, {1}, {2}", worldStartPoint.x, worldStartPoint.y, worldStartPoint.z));
                            //Debug.LogError(System.String.Format("Delta: {0}, {1}, {2}", worldDelta.x, worldDelta.y, worldDelta.z));
                        }

                        //if (touches[0].tapCount == 2)
                        //{
                        //    // Zoom in the position raycasted on the map
                        //    Debug.Log("Doubletap: Focusing on position touched");
                        //    cameraControl.FocusOnHotPoint(GetWorldPoint(Input.touches[0].position), Quaternion.identity);
                        //}
                    }

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
    private Vector3 GetWorldPoint(Vector2 screenPoint)
    {
        RaycastHit hit;
        Physics.Raycast(camera.ScreenPointToRay(screenPoint), out hit);
        return hit.point;
    }

    /// <summary>
    /// Blocks/unblocks the navigation with the touch interface.
    /// </summary>
    /// <param name="block">True if the touch navigation should be blocked.</param>
    public void BlockNavigation(bool block)
    {
        blockNavigation = block;
    }
}
