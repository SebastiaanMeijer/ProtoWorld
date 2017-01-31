/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* 
* FLASH PEDESTRIAN SIMULATOR
* FlashPedestriansSpawner.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;
using GaPSLabsUnity.StateMachine;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Implementation of the behaviour of a spawner for Flash Pedestrians.
/// </summary>
public class FlashPedestriansSpawner : MonoBehaviour
{
    /// <summary>
    /// Maximum number of pedestrians that the object will spawn. 
    /// </summary>
    [Range(0, 10000)]
    public int maxNumberOfPedestriansToSpawn = 1;

    /// <summary>
    /// If true, pedestrians will be spawned in an infinite loop. 
    /// </summary>
    public bool spawnPedestriansInInfiniteLoop = false;

    /// <summary>
    /// Min number of pedestrians to spawn per each spawning iteration.
    /// </summary>
    [Range(1, 1000)]
    public int minPedestriansPerSpawningIteration = 1;

    /// <summary>
    /// Max number of pedestrians to spawn per each spawning iteration.
    /// </summary>
    [Range(1, 1000)]
    public int maxPedestriansPerSpawningIteration = 1;

    /// <summary>
    /// Frequency of spawning iterations. 
    /// </summary>
    [Range(0.01f, 100)]
    public float spawningFrequencyInSeconds = 1;

    /// <summary>
    /// Radious around the spawner points where pedestrians can be spawned. 
    /// </summary>
    [Range(1, 10000)]
    public float spawningArea = 1;

    /// <summary>
    /// Radious in which the spawner will check for stations to find routes for pedestrians.
    /// </summary>
    [Range(1, 10000)]
    public float radiousToCheckStations = 1000;

    /// <summary>
    /// Delay before the spawner starts working.
    /// </summary>
    [Range(1, 100)]
    public float spawningDelayAtStart = 0;

    /// <summary>
    /// Number of pedestrians to be instantiated in cache before starting the simulation.
    /// </summary>
    [Range(1, 10000)]
    public int initialNumberOfPedestriansInCache = 0;

    /// <summary>
    /// Total number of pedestrians generated.
    /// </summary>
    [HideInInspector]
    public int numberOfPedestriansGenerated = 0;

    /// <summary>
    /// Total number of pedestrians on destination.
    /// </summary>
    [HideInInspector]
    public int numberOfPedestriansOnDestination = 0;

    /// <summary>
    /// Pedestrian cache to optimize instantiation (Object pooling). 
    /// </summary>
    [HideInInspector]
    public Queue pedestrianCache = new Queue();

    /// <summary>
    /// Number of spawning points (generated randomly in the point radious).
    /// </summary>
    [Range(1, 10000)]
    public int numberOfSpawningPoints = 1;

    /// <summary>
    /// Radious around the spawner where spawning points can be placed.
    /// </summary>
    [Range(1, 100000)]
    public float radiousToPutSpawningPoints = 1;

    /// <summary>
    /// True to visualize in the scene the spawning radious.
    /// </summary>
    public bool showSpawningArea = false;

    /// <summary>
    /// Initial spawning batch of pedestrians from the start of the game. 
    /// </summary>
    [Range(0, 10000)]
    public int initialSpawningBatch = 0;

    /// <summary>
    /// Material of the spawning points.
    /// </summary>
    public Material spawningMaterial;

    /// <summary>
    /// True if spawners should be visualize in the game. 
    /// </summary>
    public bool visualizeSpawners = false;

    /// <summary>
    /// True to track pedestrians in the heatmap layer.
    /// </summary>
    public bool trackPedestriansInHeatmap = false;

    ///// <summary>
    ///// True if pedestrians spawned here will have a limited range of destinations (see property below) (TODO).
    ///// </summary>
    //public bool onlyAllowedDestinations = false;

    //public FlashPedestriansDestination[] allowedDestinations;

    public bool showDebugInfo = false;

    /// <summary>
    /// Array of stations close to the spawner.
    /// </summary>
    private StationController[] stationsNearThisSpawner;

    /// <summary>
    /// Points where the spawning is happening.
    /// </summary>
    private Vector3[] spawningPoints;

    /// <summary>
    /// Reference to the script that defines the global parameters for the Flash Pedestrians.
    /// </summary>
    private FlashPedestriansGlobalParameters pedGlobalParameters;

    /// <summary>
    /// Itinerary informer that handles the commuting of pedestrians.
    /// </summary>
    private FlashPedestriansInformer flashInformer;

    /// <summary>
    /// Destination entries where pedestrians can go. 
    /// </summary>
    private FlashPedestriansDestination[] destinationPoints;

    /// <summary>
    /// Static int to get the unique ids for the pedestrians.
    /// </summary>
    private static int nextIdForPedestrian = 0;

    /// <summary>
    /// The time controller of the game. 
    /// </summary>
    private TimeController timeCtrl;

    /// <summary>
    /// The main camera controller of the game.
    /// </summary>
    private CameraControl cameraCtrl;

    /// <summary>
    /// True if spawning initial batch is currently active. 
    /// </summary>
    private bool spawningInitialBatch = false;

    /// <summary>
    /// The heatmap controller of the game.
    /// </summary>
    private HeatmapLayer.HeatmapController heatmapCtrl;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        Debug.Log("Initializing pedestrian spawner...");

        // Get the time controller
        timeCtrl = FindObjectOfType<TimeController>();

        // Get the camera controller
        cameraCtrl = FindObjectOfType<CameraControl>();

        // Get the global parameters of Flash Pedestrians
        pedGlobalParameters = GetComponentInParent<FlashPedestriansGlobalParameters>();

        // Get the heatmap controller
        heatmapCtrl = FindObjectOfType<HeatmapLayer.HeatmapController>();

        // Fill the cache with pedestrians
        for (int i = 0; i < initialNumberOfPedestriansInCache; i++)
        {
            GameObject newAgent = Instantiate(pedGlobalParameters.pedestrianObject[Random.Range(0, pedGlobalParameters.pedestrianObject.Length)],
                Vector3.zero, Quaternion.identity) as GameObject;

            newAgent.transform.SetParent(this.transform, true);
            pedestrianCache.Enqueue(newAgent);
        }

        // Get the stations that are around the spawner
        Collider[] coll = Physics.OverlapSphere(this.transform.position, radiousToCheckStations, 1 << LayerMask.NameToLayer("Stations"));

        List<StationController> aux = new List<StationController>();

        foreach (Collider C in coll)
            aux.Add(C.GetComponent<StationController>());

        stationsNearThisSpawner = aux.ToArray();

        // Set the spawning points around the spawner
        spawningPoints = new Vector3[numberOfSpawningPoints];
        for (int i = 0; i < numberOfSpawningPoints; i++)
        {
            Vector3 point = new Vector3(
                            this.transform.position.x + Random.Range(-radiousToPutSpawningPoints, radiousToPutSpawningPoints),
                            0f,
                            this.transform.position.z + Random.Range(-radiousToPutSpawningPoints, radiousToPutSpawningPoints));

            //Move the spawning point to the closest point in the walkable navmesh
            NavMeshHit hit;

            NavMesh.SamplePosition(point, out hit, 1000.0f,
                  1 << NavMesh.GetAreaFromName("footway") | 1 << NavMesh.GetAreaFromName("residential")
                  | 1 << NavMesh.GetAreaFromName("cycleway") | 1 << NavMesh.GetAreaFromName("Pedestrian")
                  | 1 << NavMesh.GetAreaFromName("step") | 1 << NavMesh.GetAreaFromName("TrafficRoads") 
                  | 1 << NavMesh.GetAreaFromName("Walkable"));

            point = hit.position;

            spawningPoints[i] = point;

            //Visualize the spawner
            var spawner = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spawner.transform.parent = this.transform;
            spawner.transform.localScale = new Vector3(2f, 0.1f, 2f);

            spawner.gameObject.name = "SpawningPoint" + i;

            if (spawningMaterial != null)
                spawner.GetComponent<MeshRenderer>().material = spawningMaterial;

            if (!visualizeSpawners)
                spawner.GetComponent<MeshRenderer>().enabled = false;

            spawner.transform.position = point;
        }

        //Debug.Log(this.gameObject.name +  " has found " + stationsNearThisSpawner.Length 
        //    + " stations nearby");

        // Get the itinerary controller
        flashInformer = FindObjectOfType<FlashPedestriansInformer>();

        // Get the destinations
        destinationPoints = FindObjectsOfType<FlashPedestriansDestination>();
    }

    /// <summary>
    /// Starts the script and spawns the pedestrians.
    /// </summary>
    void Start()
    {
        if (maxNumberOfPedestriansToSpawn > 0 || spawnPedestriansInInfiniteLoop)
            StartCoroutine(SpawnInitialBatchAndInvoke());
    }

    /// <summary>
    /// Spawn the initial batch of pedestrians defined by <see cref="initialSpawningBatch"/>.
    /// </summary>
    IEnumerator SpawnInitialBatchAndInvoke()
    {
        // Show loading splash
        timeCtrl.ShowLoadingIcon(true);
        timeCtrl.RequestPauseGame();
        //cameraCtrl.BlockPlayerControls(0f, true);
        spawningInitialBatch = true;

        yield return new WaitForSeconds(1f);

        while (numberOfPedestriansGenerated < initialSpawningBatch)
        {
            SpawnGroupOfPedestrians();
            yield return new WaitForSeconds(0.1f);
        }

        // Hide loading splash
        timeCtrl.ShowLoadingIcon(false);
        timeCtrl.RequestResumeGame();
        //cameraCtrl.UnblockPlayerControls();
        spawningInitialBatch = false;

        InvokeRepeating("SpawnGroupOfPedestrians", Random.Range(0.1f, 1.0f) + spawningDelayAtStart, spawningFrequencyInSeconds);
    }

    /// <summary>
    /// Generates a group of pedestrians inside the Unity scene. The position will be set 
    /// randomly in a radious around the spawner.
    /// </summary>
    private void SpawnGroupOfPedestrians()
    {
        if (!pedGlobalParameters.flashPedestriansPaused || spawningInitialBatch)
        {
            // Create a new pedestrian profile
            FlashPedestriansProfile profile = new FlashPedestriansProfile(pedGlobalParameters.averageSpeed + Random.Range(-0.5f, 0.5f),
                true /*future use*/, true /*future use*/, Random.Range(0.0f, 1.0f), false /*future use*/,
                Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), pedGlobalParameters.sumoCarAwarenessEnabled,
                (Random.value < pedGlobalParameters.percOfPedWillingToTakeTransport) ? TravelPreference.time : TravelPreference.noPublicTransport);

            //Find a destination
            FlashPedestriansDestination destination = null;
            int destinationIndex = 0;
            float[] priorityPercentages = GetPrioritiesOfAllDestinations();
            float rand = Random.Range(0.0f, 0.99f);
            for (int i = 0; i < priorityPercentages.Length; i++)
            {
                if (rand < priorityPercentages[i])
                {
                    destination = destinationPoints[i];

                    //Get one of the destination points within the destination choosen
                    destinationIndex = Random.Range(0, destination.numberOfDestinationPoints);

                    break;
                }
            }
            if (destination == null)
            {
                Debug.LogWarning("No destination could be found with the list of priorities, avoiding spawning...");
                return;
            }

            //Find a new random spawning point inside the radious defined
            Vector3 spawningPoint = spawningPoints[Random.Range(0, numberOfSpawningPoints)];

            //Vector3 spawningPoint = this.transform.position
            //   + new Vector3(Random.Range(-spawningArea, spawningArea), 
            //   0f, Random.Range(-spawningArea, spawningArea));

            //Find the best itinerary using the travel preferences in the pedestrian profile.
            Itinerary itinerary = flashInformer.FindBestItinerary(spawningPoint, destination, stationsNearThisSpawner, profile.travelPreference, destinationIndex);

            if (showDebugInfo)
                Debug.Log(itinerary.Print());

            //Calculate number of pedestrians to spawn in this iteration
            int pedToSpawn = Random.Range(minPedestriansPerSpawningIteration, maxPedestriansPerSpawningIteration);

            //Spawn pedestrians
            for (int i = 0; i < pedToSpawn; i++)
            {
                SpawnPedestrian(spawningPoint, profile, destination, itinerary, destinationIndex);
            }
        }
    }

    /// <summary>
    /// Spawns a Flash Pedestrian given its profile and its routing objects. 
    /// </summary>
    /// <param name="profile">Profile object that will be used by the pedestrian.</param>
    /// <param name="routing">Routing object that will be used by the pedestrian.</param>
    /// <param name="routing">Index of the destination point within the destination.</param>
    private void SpawnPedestrian(Vector3 spawningPoint, FlashPedestriansProfile profile,
        FlashPedestriansDestination destination, Itinerary itinerary, int destIndex = 0)
    {
        GameObject newAgent;

        spawningPoint += new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));

        if (pedestrianCache.Count > 0)
        {
            newAgent = (GameObject)pedestrianCache.Dequeue();
            newAgent.transform.position = spawningPoint;
        }
        else
        {
            newAgent = Instantiate(pedGlobalParameters.pedestrianObject[Random.Range(0, pedGlobalParameters.pedestrianObject.Length)],
                spawningPoint, Quaternion.identity) as GameObject;

            newAgent.transform.SetParent(this.transform, true);
        }

        if (pedGlobalParameters.rumoursEnabled || pedGlobalParameters.bikesEnabled)
        {
            BoxCollider col = newAgent.GetComponent<BoxCollider>();

            if (col != null)
                col.enabled = true;
            else
                newAgent.AddComponent<BoxCollider>();
        }

        FlashPedestriansController controller = newAgent.GetComponent<FlashPedestriansController>();

        controller.uniqueId = nextIdForPedestrian++;
        controller.profile = profile;
        controller.routing = new FlashPedestriansRouting(destination, itinerary, destIndex);
        controller.flashInformer = flashInformer;

        // Subscribe pedestrian to the itinerary informer
        flashInformer.SubscribePedestrian(controller);

        newAgent.name = "flashPedestrian_" + this.name + "_" + "_" + controller.uniqueId;

        newAgent.SetActive(true);

		// HACK: Stockholm case hack.
        // Track the pedestrian in the heatmap
//        if (trackPedestriansInHeatmap && heatmapCtrl != null)
//           heatmapCtrl.TrackNewElement(newAgent);

        // Atomic increment of the KPI property
        Interlocked.Add(ref pedGlobalParameters.numberOfPedestriansOnScenario, pedGlobalParameters.numberOfPedestriansPerAgent);

        if (++numberOfPedestriansGenerated >= maxNumberOfPedestriansToSpawn && !spawnPedestriansInInfiniteLoop)
            CancelInvoke();
    }

    /// <summary>
    /// Calculates the percentages of priority for each destination point. 
    /// </summary>
    /// <returns>An array with the accumulate percentages. Each percentage in position i in the 
    /// returned array corresponds with the destination i in the array of destinationPoints.
    /// </returns>
    public float[] GetPrioritiesOfAllDestinations()
    {
        //Get the sum of all the priorities
        float prioritySum = 0;
        foreach (FlashPedestriansDestination D in destinationPoints)
        {
            prioritySum += D.destinationPriority;
        }

        //Get the percentage of each destination and put it on an array
        float[] priorities = new float[destinationPoints.Length];
        float accumulatedPriority = 0;
        for (int i = 0; i < destinationPoints.Length; i++)
        {
            priorities[i] = destinationPoints[i].destinationPriority / prioritySum + accumulatedPriority;
            accumulatedPriority = priorities[i];
        }

        return priorities;
    }

    /// <summary>
    /// Adds new pedestrians to the total amount of the spawner. 
    /// </summary>
    /// <param name="pedPerSpawner">Number of pedestrians to add.</param>
    public void AddPedestrians(int pedPerSpawner)
    {
        maxNumberOfPedestriansToSpawn += pedPerSpawner;

        //Restart the invoke (to cover cases where Invoke has been cancel)
        CancelInvoke();
        InvokeRepeating("SpawnGroupOfPedestrians", 1.0f, spawningFrequencyInSeconds);
    }

    /// <summary>
    /// Draw gizmos on scene.
    /// </summary>
    void OnDrawGizmos()
    {
        if (showSpawningArea)
        {
            Gizmos.color = new Color32(255, 0, 0, 64);
            Gizmos.DrawSphere(transform.position, radiousToPutSpawningPoints);
        }
    }
}


