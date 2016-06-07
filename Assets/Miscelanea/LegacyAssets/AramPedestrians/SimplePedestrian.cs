//#define DEBUG_PEDESTRIAN
#define ENABLE_AI
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SimplePedestrian : MonoBehaviour
{
    public string id = "";
    public static GameObject[] Roads;
    public NavMeshAgent Agent;
    [Compact]
    public Vector3 Destination;
    public float MinStoppingDistance = 0.003f;
    public NavMeshPathStatus Status;
    [Compact]
    public Vector3 Velosity;
    [Compact]
    public Vector3 DesiredVelocity;
    public PedestrianSpawner Spawner;
    private float OldVelocity;
    private float OldTime = 0;
    public float DistanceToDestination = 0;
    // Use this for initialization
    void Start()
    {
        Spawner = GameObject.FindObjectOfType(typeof(PedestrianSpawner)) as PedestrianSpawner;
        Roads = Spawner.Roads;
        Agent = GetComponent<NavMeshAgent>();
        OldVelocity = Agent.velocity.magnitude;
#if ENABLE_AI
        GoTo(DecideTheNextDestination());
#endif
    }

    RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
#if ENABLE_AI
        //Debug.Log(Vector3.Distance(transform.position, Destination));

        Status = Agent.pathStatus;
        Velosity = Agent.desiredVelocity;
        DesiredVelocity = Agent.desiredVelocity;
        DistanceToDestination = Vector3.Distance(transform.position, Destination);
        //if (DistanceToDestination < MinStoppingDistance)
        //{
        //    GoTo(DecideTheNextDestination());
        //}

        if (Velosity.magnitude == 0 && OldVelocity == 0 && Agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
#if DEBUG_PEDESTRIAN
            Debug.Log(name + ": Repositioning | " + transform.position + ", " + Agent.destination);
#endif

            RepositionYourSelf();
        }

        if (Velosity.magnitude == 0 && Agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (Agent.remainingDistance < MinStoppingDistance)
            {
#if DEBUG_PEDESTRIAN
                Debug.Log(name + " not moving: Repositioning ");
                Debug.Log(transform.position + ", " + Agent.destination);
#endif
                //RepositionYourSelf();
                //GoTo(DecideTheNextDestination());
            }
            else
            {
#if DEBUG_PEDESTRIAN
                Debug.Log(name + " not moving: Changing destination.");
#endif
                RepositionYourSelf();
                GoTo(DecideTheNextDestination());
            }

        }

        if (Agent.remainingDistance != Mathf.Infinity)
        {
            if (Vector3.Distance(transform.position, Destination) < MinStoppingDistance)
            {
                GoTo(DecideTheNextDestination());
                Agent.Resume();
            }
        }
        if (Time.time - OldTime > 15) // Updating the check velocity
        {
            if (Velosity.magnitude == 0 && OldVelocity == 0) // Means that the object hasen't moved
            {
                RepositionYourSelf();
                GoTo(DecideTheNextDestination());
                Debug.Log("PLEASE DON'T HAPPEN OFTEN! :(");
            }
            OldVelocity = Agent.velocity.magnitude;
            OldTime = Time.time;
        }

#endif
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        Destination = hit.point;
        //        GoTo(Destination);

        //    }
        //}
    }

    public void GoTo(Vector3 destination)
    {
        Destination = destination;
        Agent.SetDestination(Destination);
    }

    public void RepositionYourSelf()
    {
        // var tmp = Random.Range(0, Roads.Length - 1);
        var tmp = PedestrianSpawner.random.Next(0, Roads.Length - 1);
        NavMeshHit hit;
        NavMesh.SamplePosition(Roads[tmp].transform.position, out hit, 50, 1);
        transform.position = hit.position;
    }

    private bool DebugWrite = true;
    public Vector3 DecideTheNextDestination()
    {
        //var CloseRoads = from r in Roads.Select(i => new { i.transform.position })
        //                 orderby Vector3.Distance(transform.position, r.position) ascending
        //                 select r.position;

        var tmp = PedestrianSpawner.random.Next(0, Roads.Length - 1);
        NavMeshHit hit;
        NavMesh.SamplePosition(Roads[tmp].transform.position, out hit, 50, 1);
        return hit.position;
    }
}
