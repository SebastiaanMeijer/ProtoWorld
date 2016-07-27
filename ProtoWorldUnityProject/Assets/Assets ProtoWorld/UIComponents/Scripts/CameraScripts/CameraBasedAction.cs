/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A class that handles events when the attached object enters or leaves the main camera's frustum.
/// <remarks>The <see cref="CameraFrustumScript"/> needs to be attached to the main camera for this to work.</remarks>
/// </summary>
public class CameraBasedAction : MonoBehaviour
{
    private CameraFrustumScript frustum;
    private Camera mcamera;

    /// <summary>
    /// The script is active if true, inactive otherwise.
    /// </summary>
    public bool UseFrustumForUpdate = true;

    private bool _isInFrustum;

    public bool IsInFrustum
    {
        get { return _isInFrustum; }
    }

    /// <summary>
    /// The caching value to prevent unnecessary method calls.
    /// </summary>
    private bool oldIsInFrustum;

    private List<Renderer> renderers;

    private Bounds renderingBounds;

    public float VicinityDistance = 25f;

    private bool _objectInVicinity;

    private bool _OldobjectInVicinity;

    public delegate void VicinityEventHandler(CameraVisibilityChangeEventArgs e);

    public event VicinityEventHandler VicinityChanged;

    public bool ObjectInVicinity

    { get { return _objectInVicinity; } }

    // Use this for initialization
    void Start()
    {
        _OldobjectInVicinity = _objectInVicinity;
        // Grabbing the renderer bounds
        if (GetComponent<Renderer>() != null)
        {
            //Debug.Log("Renderer of " + name + " was not null.");
            renderingBounds = GetComponent<Renderer>().bounds;
        }
        else // If there was no renderer attached to the immediate children, make a deep search for a renderer.
        {
            renderers = new List<Renderer>();
            GetComponentInChildrenRecursively(transform, ref renderers);
            if (renderers.Count != 0)
            {
                // Calculating an average center, with the size of maximum extends.
                // Note that this needs to be improved so that a union of extents are included.
                var centers = renderers.Select(r => r.bounds.center);
                var extents = renderers.Select(r => r.bounds.extents);
                //var maxs = renderers.Select(r => r.bounds.max);
                renderingBounds =
                    new Bounds(
                    new Vector3(centers.Average(c => c.x), centers.Average(c => c.y), centers.Average(c => c.z)),
                    //new Vector3(maxs.Max(m => m.x), maxs.Max(m => m.y), maxs.Max(m => m.z)) * 2);
                new Vector3(extents.Max(m => m.x), extents.Max(m => m.y), extents.Max(m => m.z)));
            }
        }
        frustum = Camera.main.GetComponent<CameraFrustumScript>();
        if (frustum == null)
            UseFrustumForUpdate = false;
        mcamera = frustum.GetComponent<Camera>();
        PerformAction(true);
    }
    /// <summary>
    /// Recursively gets all the renderers in the children of this transform and add them to the <typeparamref name="rens"/>.
    /// </summary>
    /// <param name="parent">The parent object to start the searching.</param>
    /// <param name="rens">Renderers of children. This will be populated by the method.</param>
    private void GetComponentInChildrenRecursively(Transform parent, ref List<Renderer> rens, int depth = 3)
    {
        if (depth > 3)
            return;
        foreach (Transform childtransform in parent.GetComponentsInChildren<Transform>(true))
        {
            if (!(parent == childtransform))
            {
                var component = childtransform.GetComponent<Renderer>();
                if (component != null)
                    rens.Add(component);
                GetComponentInChildrenRecursively(childtransform, ref rens, ++depth);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (renderingBounds != null)
        {
            renderingBounds.center = transform.position;
            PerformAction(false);
        }
    }
    /// <summary>
    /// Performs the Camera detection.
    /// </summary>
    /// <param name="initialize">If true, in the case the object is outside of frustum VicinityChanged is called repeatedly.
    /// This should only be used from Start() or Awake()
    /// </param>
    private void PerformAction(bool initialize)
    {
        if (UseFrustumForUpdate)
        {
            // Object is in the frustum
            if (UnityEngine.GeometryUtility.TestPlanesAABB(frustum.Frustum, renderingBounds))
            {
                _isInFrustum = true;
                // Object has entered the frustum
                if (_isInFrustum != oldIsInFrustum || initialize)
                {
                    oldIsInFrustum = _isInFrustum;
                    var distance = Vector3.Distance(transform.position, mcamera.transform.position);
                    // Object is in the close vicinity
                    if (distance < VicinityDistance)
                    {
                        _objectInVicinity = true;
                        if (_OldobjectInVicinity != _objectInVicinity || initialize)
                        {
                            _OldobjectInVicinity = _objectInVicinity;
                            if (VicinityChanged != null)
                                VicinityChanged(
                                    new CameraVisibilityChangeEventArgs()
                                    {
                                        ObjectInVicinity = _objectInVicinity,
                                        DetectedDistance = distance,
                                        VicinityDistanceThreshold = VicinityDistance,
                                        Frustum = CameraVisibilityChangeEventArgs.FrustumState.Enter
                                    });
                        }
                    }
                    else // Object is in the far vicinity
                    {
                        _objectInVicinity = false;
                        if (_OldobjectInVicinity != _objectInVicinity || initialize)
                        {
                            _OldobjectInVicinity = _objectInVicinity;
                            if (VicinityChanged != null)
                                VicinityChanged(
                                    new CameraVisibilityChangeEventArgs()
                                    {
                                        ObjectInVicinity = _objectInVicinity,
                                        DetectedDistance = distance,
                                        VicinityDistanceThreshold = VicinityDistance,
                                        Frustum = CameraVisibilityChangeEventArgs.FrustumState.Enter
                                    });
                        }
                    }
                }
                else  // Object has stayed in the frustum
                {
                    var distance = Vector3.Distance(transform.position, mcamera.transform.position);
                    // Object is in the close vicinity
                    if (distance < VicinityDistance)
                    {
                        _objectInVicinity = true;
                        if (_OldobjectInVicinity != _objectInVicinity || initialize)
                        {
                            _OldobjectInVicinity = _objectInVicinity;
                            if (VicinityChanged != null)
                                VicinityChanged(
                                    new CameraVisibilityChangeEventArgs()
                                    {
                                        ObjectInVicinity = _objectInVicinity,
                                        DetectedDistance = distance,
                                        VicinityDistanceThreshold = VicinityDistance,
                                        Frustum = CameraVisibilityChangeEventArgs.FrustumState.StayInside
                                    });
                        }
                    }
                    else // Object is in the far vicinity
                    {
                        _objectInVicinity = false;
                        if (_OldobjectInVicinity != _objectInVicinity || initialize)
                        {
                            _OldobjectInVicinity = _objectInVicinity;
                            if (VicinityChanged != null)
                                VicinityChanged(
                                    new CameraVisibilityChangeEventArgs()
                                    {
                                        ObjectInVicinity = _objectInVicinity,
                                        DetectedDistance = distance,
                                        VicinityDistanceThreshold = VicinityDistance,
                                        Frustum = CameraVisibilityChangeEventArgs.FrustumState.StayInside
                                    });
                        }
                    }
                }
            }
            else // Object is outside the frustum
            {
                _isInFrustum = false;
                if (_isInFrustum != oldIsInFrustum || initialize) // initialize is usually set in the Start();
                {

                    oldIsInFrustum = _isInFrustum;
                    _objectInVicinity = false;
                    _OldobjectInVicinity = _objectInVicinity;
                    var distance = Vector3.Distance(transform.position, mcamera.transform.position);
                    if (VicinityChanged != null)
                        VicinityChanged(
                            new CameraVisibilityChangeEventArgs()
                            {
                                ObjectInVicinity = _objectInVicinity,
                                DetectedDistance = distance,
                                VicinityDistanceThreshold = VicinityDistance,
                                Frustum = CameraVisibilityChangeEventArgs.FrustumState.Leave
                            });
                    else
                    {
                        //Debug.Log("Please make sure the CameraBasedAction script is in higher execution order.");
                    }
                }
            }



        }
    }
    public class CameraVisibilityChangeEventArgs : System.EventArgs
    {
        public enum FrustumState { Enter, Leave, StayInside };
        public FrustumState Frustum { get; set; }
        public bool ObjectInVicinity { get; set; }
        public float DetectedDistance { get; set; }
        public float VicinityDistanceThreshold { get; set; }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(renderingBounds.center, renderingBounds.size);
    //    var distance = Vector3.Distance(transform.position, mcamera.transform.position);
    //    if (distance < VicinityDistance)
    //        Gizmos.DrawLine(transform.position, mcamera.transform.position);
    //}

}
