using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraFrustumScript : MonoBehaviour
{
    [HideInInspector]
    public Plane[] Frustum;

    public float FrustumUpdatePeriodInSeconds = 0.5f;

    private float currentTime;

    // Use this for initialization
    void Start()
    {
        Frustum = UnityEngine.GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
        currentTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - currentTime > FrustumUpdatePeriodInSeconds)
        {
            Frustum = UnityEngine.GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
            currentTime = Time.time;
        }
    }
}
