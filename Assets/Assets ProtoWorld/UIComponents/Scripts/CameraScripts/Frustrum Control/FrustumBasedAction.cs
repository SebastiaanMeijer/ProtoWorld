using UnityEngine;
using System.Collections;

/// <summary>
/// A class that handles events when the attached object enters or leaves the main camera's frustum.
/// <remarks>The <see cref="CameraFrustumScript"/> needs to be attached to the main camera for this to work.</remarks>
/// </summary>
public class FrustumBasedAction : MonoBehaviour
{

    private CameraFrustumScript frustum;
    /// <summary>
    /// The script is active if true, inactive otherwise.
    /// </summary>
    public bool UseFrustumForUpdate = true;
    /// <summary>
    /// The reference component that will be used to call the <see cref="ActionMethodWhenEnteredFrustum"/> and <see cref="ActionMethodWhenExitFrustum"/> methods.
    /// </summary>
    public string ActionScript;
    private Component ActionScriptBehaviour;
    /// <summary>
    /// This method is called when the object enters the frustum. It belongs to <see cref="ActionScript"/>
    /// </summary>
    public string ActionMethodWhenEnteredFrustum;
    /// <summary>
    /// This method is called when the object exits the frustum. It belongs to <see cref="ActionScript"/>
    /// </summary>
    public string ActionMethodWhenExitFrustum;
    private bool _isInFrustum;
    /// <summary>
    /// 
    /// </summary>
    public bool IsInFrustum
    {
        get { return _isInFrustum; }
    }
    /// <summary>
    /// The caching value to prevent unnecessary method calls.
    /// </summary>
    private bool oldIsInFrustum;
    private Renderer ren;
    // Use this for initialization
    void Start()
    {
        ren = GetComponent<Renderer>() != null ? GetComponent<Renderer>() : GetComponentInChildren<Renderer>();
        frustum = Camera.main.GetComponent<CameraFrustumScript>();
        if (frustum == null)
            UseFrustumForUpdate = false;

        ActionScriptBehaviour = GetComponent(ActionScript);
        if (ActionScriptBehaviour == null)
        {
            //Obsolete method, and there is no FrustumBasedAction.cs in the folder.
            //UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/Scripts/GaPSLabs/FrustumSystem/FrustumBasedAction.cs (53,13)", ActionScript);
            ActionScriptBehaviour = GetComponent(ActionScript);
        }   
        if (UseFrustumForUpdate)
        {
            if (UnityEngine.GeometryUtility.TestPlanesAABB(frustum.Frustum, ren.bounds))
            {
                _isInFrustum = true;
                oldIsInFrustum = _isInFrustum;
                if (ActionScriptBehaviour == null)
                    Debug.Log("There is no " + ActionScript);
                else
                {
                    // Debug.Log(name + " START Entered the frustum.");
                    ActionScriptBehaviour.SendMessage(ActionMethodWhenEnteredFrustum);
                }
            }
            else
            {
                _isInFrustum = false;
                oldIsInFrustum = _isInFrustum;
                // Debug.Log(name + " START Exited the frustum.");
                ActionScriptBehaviour.SendMessage(ActionMethodWhenExitFrustum);
                oldIsInFrustum = _isInFrustum;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UseFrustumForUpdate)
            if (UnityEngine.GeometryUtility.TestPlanesAABB(frustum.Frustum, ren.bounds))
            {
                // Entered the frustum
                _isInFrustum = true;
                if (_isInFrustum != oldIsInFrustum)
                {
                    oldIsInFrustum = _isInFrustum;
                    if (ActionScriptBehaviour == null)
                        Debug.Log("There is no " + ActionScript);
                    else
                    {
                        //Debug.Log("Entered the frustum.");
                        ActionScriptBehaviour.SendMessage(ActionMethodWhenEnteredFrustum);
                    }
                }
            }
            else
            {
                _isInFrustum = false;
                if (_isInFrustum != oldIsInFrustum)
                {
                    oldIsInFrustum = _isInFrustum;
                    //Debug.Log("Exited the frustum.");
                    ActionScriptBehaviour.SendMessage(ActionMethodWhenExitFrustum);
                }
            }
    }
}
