/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraBasedAction))]
public class AIControllerWithLOD : MonoBehaviour
{
    private Animator lowpolyAnimator;
    private Animator highpolyAnimator;
    public GameObject HighPoly;
    public GameObject LowPoly;
    [HideInInspector]
    public SkinnedMeshRenderer[] highRender;
    [HideInInspector]
    public SkinnedMeshRenderer[] lowRender;
    private Camera mcamera;
    public static float StopAnimationDistance = 50f;
    public bool Visible = true;
    private CameraBasedAction cameraAction;
    // Use this for initialization
    void Start()
    {
        lowpolyAnimator = LowPoly.transform.GetComponent<Animator>();
        highpolyAnimator = HighPoly.transform.GetComponent<Animator>();
        highRender = HighPoly.GetComponentsInChildren<SkinnedMeshRenderer>();
        lowRender = LowPoly.GetComponentsInChildren<SkinnedMeshRenderer>();
        animationFunction = DefaultAnimationSpeed;
        mcamera = Camera.main;
        cameraAction = GetComponent<CameraBasedAction>();
        cameraAction.VicinityChanged += cameraAction_VicinityChanged;
    }

   
    void cameraAction_VicinityChanged(CameraBasedAction.CameraVisibilityChangeEventArgs e)
    {
        if (Visible)
        {
            switch (e.Frustum)
            {
                case CameraBasedAction.CameraVisibilityChangeEventArgs.FrustumState.Enter:
                    {
                        if (e.ObjectInVicinity)
                        {
                            LowPoly.SetActive(false);
                            HighPoly.SetActive(true);
                            HighPoly.transform.position = transform.position;
                            HighPoly.transform.rotation = transform.rotation;
                            highpolyAnimator.speed = animationFunction(this);
                            if (onUpdateFunction != null)
                                onUpdateFunction(this);
                        }
                        else
                        {
                            HighPoly.SetActive(false);
                            LowPoly.SetActive(true);
                            LowPoly.transform.position = transform.position;
                            LowPoly.transform.rotation = transform.rotation;
                            lowpolyAnimator.speed = animationFunction(this);
                            if (onUpdateFunction != null)
                                onUpdateFunction(this);
                        }
                        break;
                    }
                case CameraBasedAction.CameraVisibilityChangeEventArgs.FrustumState.Leave:
                    {
                        LowPoly.SetActive(false);
                        HighPoly.SetActive(false);
                        break;
                    }
                case CameraBasedAction.CameraVisibilityChangeEventArgs.FrustumState.StayInside:
                    {
                        if (e.ObjectInVicinity)
                        {
                            LowPoly.SetActive(false);
                            HighPoly.SetActive(true);
                            HighPoly.transform.position = transform.position;
                            HighPoly.transform.rotation = transform.rotation;
                            highpolyAnimator.speed = animationFunction(this);
                            if (onUpdateFunction != null)
                                onUpdateFunction(this);
                        }
                        else
                        {
                            HighPoly.SetActive(false);
                            LowPoly.SetActive(true);
                            LowPoly.transform.position = transform.position;
                            LowPoly.transform.rotation = transform.rotation;
                            lowpolyAnimator.speed = animationFunction(this);
                            if (onUpdateFunction != null)
                                onUpdateFunction(this);
                        }
                        break;
                    }
                default:
                    { break; }
            }
        }
        else // Object is not active
        {
            LowPoly.SetActive(false);
            HighPoly.SetActive(false);
        }
    }

    public delegate float AnimationSpeedCalculationDelegate(AIControllerWithLOD aiController);
    public delegate void OnUpdateDelegate(AIControllerWithLOD aiController);
    private AnimationSpeedCalculationDelegate animationFunction;
    private OnUpdateDelegate onUpdateFunction;

    public void SetAnimationFunction(AnimationSpeedCalculationDelegate function)
    {
        animationFunction = function;
    }
    public void SetOnUpdateFunction(OnUpdateDelegate function)
    {
        onUpdateFunction = function;
    }
    public float DefaultAnimationSpeed(AIControllerWithLOD aiController)
    { return 1; }
    public void SetAnimationParameter(string Name, float value)
    {
        if (LowPoly.activeSelf)
            lowpolyAnimator.SetFloat(Name, value);
        else if (HighPoly.activeSelf)
            highpolyAnimator.SetFloat(Name, value);
    }
    void Update()
    {
        if (Visible)
        {

            if (highpolyAnimator.enabled)
            {
                highpolyAnimator.speed = animationFunction(this);
                HighPoly.transform.position = transform.position;
                HighPoly.transform.rotation = transform.rotation;
            }
            if (lowpolyAnimator.enabled)
            {
                lowpolyAnimator.speed = animationFunction(this);
                LowPoly.transform.position = transform.position;
                LowPoly.transform.rotation = transform.rotation;
            }
            if (onUpdateFunction != null)
                    onUpdateFunction(this);
        }

    }
}
