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
    private Transform camera;
    private CameraFrustumScript frustum;

    /// <summary>
    /// If true, then the object disappears when not in front of the camera.
    /// </summary>
    public bool useFrustumForUpdate = true;

    /// <summary>
    /// If true, it will flip the orientation of the X axis. 
    /// </summary>
    public bool flipXAxis = false;

    /// <summary>
    /// If true, it will only rotate the object around the Y axis. 
    /// </summary>
    public bool rotateOnlyOnYAxis = false;

    /// <summary>
    /// Initializes the FaceCamera class
    /// </summary>
    void Start()
    {
        camera = Camera.main.transform;
        frustum = camera.GetComponent<CameraFrustumScript>();
        if (frustum == null)
            useFrustumForUpdate = false;
    }

    /// <summary>
    /// Updates the rotation of the attached object to face the camera.
    /// </summary>
    void Update()
    {
        if (!useFrustumForUpdate
            || UnityEngine.GeometryUtility.TestPlanesAABB(frustum.Frustum, GetComponent<Renderer>().bounds))
        {
            transform.LookAt(camera);
            var temp = transform.eulerAngles;

            if (!flipXAxis)
                temp.y += 180;

            if (rotateOnlyOnYAxis)
            {
                temp.x = 0.0f;
                temp.z = 0.0f;
            }

            transform.eulerAngles = temp;
        }
    }
}
