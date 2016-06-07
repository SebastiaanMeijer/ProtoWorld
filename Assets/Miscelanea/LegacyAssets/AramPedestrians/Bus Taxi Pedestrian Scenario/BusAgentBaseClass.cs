using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// This class is in charge of physical movement of the busses.
/// <para>Any other logic should handled in the BusBaseClass or its inherited classes.</para>
/// </summary>
public class BusAgentBaseClass : MonoBehaviour
{

    public Vector3 CurrentDestination;
    private Vector3 PreviousDestination;
    private float waitDuaration;
    private float waitTime;
    private bool wait = false;
    private bool StopToAvoidCollision = false;
    public float SpeedMultiplier = 1;
    public Transform FrontAwarenessOrigin;
    public Transform FrontAwarenessDirection;
    public Transform FrontAwarenessDirectionLeft;
    public Transform FrontAwarenessDirectionRight;
    private RaycastHit hit;
    [HideInInspector]
    public BusBaseClass BusBase;
    // 8.33 m/s or 30 km/h
    // scaling to 4.15
    public float Speed = 4.15f;

    private float LerpTime;
    // NOTE: this number has to be relevant to the distance between the points.
    private float LerpDuration = 1;
    private bool UpdateLerpTimeAllowed = true;

    void Start()
    {
        BusBase = GetComponent<BusBaseClass>();
    }
    // Update is called once per frame
    void Update()
    {
        if (wait && Time.time - waitTime < waitDuaration)
        {
            // Do nothing.
        }
        else
        {
            if (!StopToAvoidCollision)
            {
                wait = false;
                if (Vector3.Distance(transform.position, CurrentDestination) >= GlobalSimulationPlannerBaseClass.AcceptedBusDistanceToDestination)
                {

                    // Movement
                    //Interpolations.MyLog("BusAgent Check for minimum distance and move" + "=TRUE");

                    var lerpPosition = (Time.time - LerpTime) / LerpDuration;

                    //Interpolations.MyLog("Lerp position: " + lerpPosition + " Lerp duration: " + LerpDuration);
                    transform.position = Vector3.Slerp(transform.position, CurrentDestination, lerpPosition);

                    //transform.position = Vector3.Slerp(transform.position, CurrentDestination, Time.deltaTime * Speed);


                }

                // Calculating Rotation
                var oldRotation = transform.rotation;
                transform.LookAt(CurrentDestination);
                var newRotation = transform.rotation;
                transform.rotation = Quaternion.Lerp(oldRotation, newRotation, Time.deltaTime * 2 * SpeedMultiplier);
            }
        }


    }

    void FixedUpdate()
    {

        //Interpolations.MyDrawRay(FrontAwarenessOrigin.position, (FrontAwarenessDirection.position - FrontAwarenessOrigin.position).normalized * GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance, Color.red);

        //if (!wait)
        if (Physics.Raycast(FrontAwarenessOrigin.position, (FrontAwarenessDirection.position - FrontAwarenessOrigin.position).normalized, out hit, GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance))
        {
            StopToAvoidCollision = ShouldAvoidCollision(ref hit);
        }
        else if (Physics.Raycast(FrontAwarenessOrigin.position, (FrontAwarenessDirectionLeft.position - FrontAwarenessOrigin.position).normalized, out hit, GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance))
        {
            StopToAvoidCollision = ShouldAvoidCollision(ref hit);
        }
        else if (Physics.Raycast(FrontAwarenessOrigin.position, (FrontAwarenessDirectionRight.position - FrontAwarenessOrigin.position).normalized, out hit, GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance))
        {
            StopToAvoidCollision = ShouldAvoidCollision(ref hit);
        }
        else StopToAvoidCollision = false;
    }

    private bool ShouldAvoidCollision(ref RaycastHit hit)
    {
        var isbus = hit.transform.GetComponent<BusBaseClass>();
        if (isbus != null)
        {
            if (isbus.BusRoute.Id == BusBase.BusRoute.Id) // If there is another bus on your line, then wait until that moves
                return true;
            else return false;
        }
        else return false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(FrontAwarenessOrigin.position, (FrontAwarenessDirection.position - FrontAwarenessOrigin.position).normalized * GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance);
        Gizmos.DrawWireSphere((FrontAwarenessDirection.position + FrontAwarenessOrigin.position) / 2, GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(FrontAwarenessOrigin.position, (FrontAwarenessDirectionLeft.position - FrontAwarenessOrigin.position).normalized * GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(FrontAwarenessOrigin.position, (FrontAwarenessDirectionRight.position - FrontAwarenessOrigin.position).normalized * GlobalSimulationPlannerBaseClass.BusFrontAwarenessDistance);
        
    }

    public virtual void Wait(float Seconds)
    {
        if (!wait)
        {
            waitTime = Time.time;
            waitDuaration = Seconds;
            wait = true;
        }
    }
    public virtual void Move(Vector3 Destination)
    {
        PreviousDestination = transform.position;
        CurrentDestination = Destination;
        if (UpdateLerpTimeAllowed)
        {
            LerpTime = Time.time;
            var distance = Vector3.Distance(transform.position, Destination);
            LerpDuration = distance / Speed;
            UpdateLerpTimeAllowed = false;
        }
    }
    public void CalculateSpeed()
    { UpdateLerpTimeAllowed = true; }
    public virtual void MoveWithRelativeSpeed(Vector3 Destination)
    {
        CurrentDestination = Destination;
        var Estimateddistance = Vector3.Distance(transform.position, Destination);
        if (Estimateddistance < 1)
            Speed = 1;
        else Speed = Estimateddistance / 2;
    }
    public virtual void Move(double Latitude, double Longitude)
    {
    }
    public bool IsAtDestination(Vector3 Destination)
    {
        if (Vector3.Distance(transform.position, Destination) < GlobalSimulationPlannerBaseClass.AcceptedBusDistanceToDestination)
            return true;
        else return false;
    }
    public bool IsAtDestination(Vector3 Destination, float MaximumAcceptedDistanceToDestination)
    {
        if (Vector3.Distance(transform.position, Destination) <= MaximumAcceptedDistanceToDestination)
            return true;
        else return false;
    }
}
