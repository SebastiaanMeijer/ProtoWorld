/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * CAMERA UTILITIES
 * FaceCamera.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// The FaceCamera class. When attached to an object, it will keep looking at the camera. 
/// This is usually used on billboard objects such as bus stop signs and traffic lights..
/// </summary>
public class FaceCamera : MonoBehaviour
{
    //private Transform camera;
    //private CameraFrustumScript frustum;
    public CameraControl camera;
    public Quaternion currentCameraRotation;

    ///// <summary>
    ///// If true, then the object disappears when not in front of the camera.
    ///// </summary>
    //public bool useFrustumForUpdate = true;

    ///// <summary>
    ///// If true, it will flip the orientation of the X axis. 
    ///// </summary>
    //public bool flipXAxis = false;

    ///// <summary>
    ///// If true, it will only rotate the object around the Y axis. 
    ///// </summary>
    //public bool rotateOnlyOnYAxis = false;

    /// <summary>
    /// Initializes the FaceCamera class
    /// </summary>
    //void Start()
    //{
    //    camera = Camera.main.transform;
    //    frustum = camera.GetComponent<CameraFrustumScript>();
    //    if (frustum == null)
    //        useFrustumForUpdate = false;
    //}

    /// <summary>
    /// Updates the rotation of the attached object to face the camera.
    /// </summary>
    //void Update()
    //{
    //    //if (!useFrustumForUpdate
    //    //    || UnityEngine.GeometryUtility.TestPlanesAABB(frustum.Frustum, GetComponent<Renderer>().bounds))
    //    //{
    //    //    transform.LookAt(camera);
    //    //    var temp = transform.eulerAngles;

    //    //    if (!flipXAxis)
    //    //        temp.y += 180;

    //    //    if (rotateOnlyOnYAxis)
    //    //    {
    //    //        temp.x = 0.0f;
    //    //        temp.z = 0.0f;
    //    //    }

    //    //    transform.eulerAngles = temp;
    //    //}
    //}

    // Use this for initialization
    void Start()

    {
        camera = Camera.main.GetComponent<CameraControl>();
        currentCameraRotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraRotationChanged())
        {
            gameObject.transform.rotation = calculateIconRotation();
        }
    }

    //checks if the camera has changed position, if yes set current cameraposition
    bool cameraRotationChanged()
    {
        if (currentCameraRotation != Camera.main.transform.rotation)
        {
            currentCameraRotation = Camera.main.transform.rotation;
            return true;
        }
        else
        {
            return false;
        }
    }

    //calculates the font rotation so it rotates with the camera
    Quaternion calculateIconRotation()
    {
        //Quaternion fontRotation = new Quaternion();
        //fontRotation = Camera.main.transform.rotation;
        return Camera.main.transform.rotation;
    }
}
