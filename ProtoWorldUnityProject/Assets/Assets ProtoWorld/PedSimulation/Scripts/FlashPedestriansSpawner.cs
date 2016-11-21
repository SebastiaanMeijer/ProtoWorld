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
using System;

/// <summary>
/// Implementation of the behaviour of a spawner for Flash Pedestrians.
/// </summary>
public class FlashPedestriansSpawner : MonoBehaviour, Loggable
{
    /// <summary>
    /// Maximum number of pedestrians that the object will spawn. 
    /// </summary>
    [Range(0, 10000)]
    public int maxNumberOfPedestriansToSpawn = 1;

    public int id;

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
    /// Array of stations close to the spawner.
    /// </summary>
    private StationController[] stationsNearThisSpawner;

    /// <summary>
    /// Number of pedestrians to be instantiated in cache before starting the simulation.
    /// </summary>
    [Range(1, 10000)]
    public int initialNumberOfPedestriansInCache = 0;

    /// <summary>
    /// Total number of pedestrians generated.
    /// </summary>
    public int numberOfPedestriansGenerated = 0;

    /// <summary>
    /// Total number of pedestrians on destination.
    /// </summary>
    public int numberOfPedestriansOnDestination = 0;

    /// <summary>
    /// Pedestrian cache to optimize instantiation (Object pooling). 
    /// </summary>
    [HideInInspector]
    public Queue pedestrianCache = new Queue();

    /// <summary>
    /// True if pedestrians spawned here will have a limited range of destinations (see property below) (TODO).
    /// </summary>
    //public bool onlyAllowedDestinations = false;

    //public FlashPedestriansDestination[] allowedDestinations;

    /// <summary>
    /// Reference to the script that defines the global parameters for the Flash Pedestrians.
    /// </summary>
    public FlashPedestriansGlobalParameters pedGlobalParameters;

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
    public static int nextIdForPedestrian = 0;

	private Heatmap heatMap;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        initializeSpawner();

		heatMap = FindObjectOfType<Heatmap>();
		LoggableManager.subscribe((Loggable)this);
	}

    public void initializeSpawner()
    {
        // Get the global parameters of Flash Pedestrians
        pedGlobalParameters = GameObject.Find("FlashPedestriansModule").GetComponent<FlashPedestriansGlobalParameters>();
        id = FlashPedestriansGlobalParameters.nextSpawnerId;
        FlashPedestriansGlobalParameters.nextSpawnerId++;
        // Fill the cache with pedestrians
        for (int i = 0; i < initialNumberOfPedestriansInCache; i++)
        {
            GameObject newAgent = Instantiate(pedGlobalParameters.pedestrianObject, Vector3.zero, Quaternion.identity) as GameObject;
            newAgent.transform.SetParent(this.transform, true);
            pedestrianCache.Enqueue(newAgent);
        }

        // Get the stations that are around the spawner
        Collider[] coll = Physics.OverlapSphere(this.transform.position, radiousToCheckStations, 1 << LayerMask.NameToLayer("Stations"));

        List<StationController> aux = new List<StationController>();

        foreach (Collider C in coll)
            aux.Add(C.GetComponent<StationController>());

        stationsNearThisSpawner = aux.ToArray();

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
        {
            InvokeRepeating("SpawnGroupOfPedestrians", 1.0f + UnityEngine.Random.Range(1.0f, 4.0f) + spawningDelayAtStart, spawningFrequencyInSeconds);
        }
    }

    /// <summary>
    /// Generates a group of pedestrians inside the Unity scene. The position will be set 
    /// randomly in a radious around the spawner.
    /// </summary>
    private void SpawnGroupOfPedestrians()
    {
        if (!pedGlobalParameters.flashPedestriansPaused)
        {
            // Create a new pedestrian profile
            FlashPedestriansProfile profile = new FlashPedestriansProfile(pedGlobalParameters.averageSpeed + UnityEngine.Random.Range(-0.5f, 0.5f),
                true /*future use*/, true /*future use*/, UnityEngine.Random.Range(0.0f, 1.0f), false /*future use*/, UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), pedGlobalParameters.sumoCarAwarenessEnabled, TravelPreference.time);
            //Find a destination
            FlashPedestriansDestination destination = null;
            float[] priorityPercentages = getPrioritiesOfAllDestinations();
            float rand = UnityEngine.Random.Range(0.0f, 0.99f);
            for (int i = 0; i < priorityPercentages.Length; i++)
            {
                if (rand < priorityPercentages[i])
                {
                    destination = destinationPoints[i];
                    break;
                }
            }
            if (destination == null)
            {
                Debug.LogWarning("No destination could be found with the list of priorities, avoiding spawning...");
                return;
            }

            //Find a new random spawning point inside the radious defined
            Vector3 spawningPoint = this.transform.position
                + new Vector3(UnityEngine.Random.Range(-spawningArea, spawningArea), 0.0f, UnityEngine.Random.Range(-spawningArea, spawningArea));

            //Move the spawning point to the closest point in the walkable navmesh (is this an expensive operation?)
            //NavMeshHit hit;
            //if (NavMesh.SamplePosition(spawningPoint, out hit, 1000.0f, 1 << NavMesh.GetAreaFromName("footway")))
            //    spawningPoint = hit.position;

            //Find the best itinerary using the travel preferences in the pedestrian profile.
            Itinerary itinerary = flashInformer.FindBestItinerary(spawningPoint, destination, stationsNearThisSpawner, profile.travelPreference);

            //Calculate number of pedestrians to spawn in this iteration
            int pedToSpawn = UnityEngine.Random.Range(minPedestriansPerSpawningIteration, maxPedestriansPerSpawningIteration);

            //Spawn pedestrians
            for (int i = 0; i < pedToSpawn; i++)
            {
                SpawnPedestrian(spawningPoint, profile, destination, itinerary);
            }
        }
    }

	public void SpawnPedestrianFromLog(Vector3 spawningPoint, FlashPedestriansProfile profile, FlashPedestriansDestination destination){
		//Find the best itinerary using the travel preferences in the pedestrian profile.
		Itinerary itinerary = flashInformer.FindBestItinerary(spawningPoint, destination, stationsNearThisSpawner, profile.travelPreference);
        SpawnPedestrian(spawningPoint, profile, destination, itinerary);
		//TODO: recreate destination

	}

    /// <summary>
    /// Spawns a Flash Pedestrian given its profile and its routing objects. 
    /// </summary>
    /// <param name="profile">Profile object that will be used by the pedestrian.</param>
    /// <param name="routing">Routing object that will be used by the pedestrian.</param>
    private void SpawnPedestrian(Vector3 spawningPoint, FlashPedestriansProfile profile, FlashPedestriansDestination destination, Itinerary itinerary)
    {
        GameObject newAgent;

        spawningPoint += new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0.0f, UnityEngine.Random.Range(-1.0f, 1.0f));

        if (pedestrianCache.Count > 0)
        {
            newAgent = (GameObject)pedestrianCache.Dequeue();
            newAgent.transform.position = spawningPoint;
        }
        else
        {
            newAgent = Instantiate(pedGlobalParameters.pedestrianObject, spawningPoint, Quaternion.identity) as GameObject;
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
        controller.spawnerId = id;
        controller.profile = profile;
        controller.routing = new FlashPedestriansRouting(destination, itinerary);
        controller.flashInformer = flashInformer;

		// Provide these from our cached version to improve performance, as this is called a lot during spawning events.
		controller.globalParam = pedGlobalParameters;
		controller.heatMap = heatMap;

		// Subscribe pedestrian to the itinerary informer
		flashInformer.SubscribePedestrian(controller);

        newAgent.name = "flashPedestrian_" + this.name + "_" + "_" + controller.uniqueId;

        newAgent.SetActive(true);

        // Atomic increment of the KPI property
        Interlocked.Increment(ref pedGlobalParameters.numberOfPedestriansOnScenario);

        if (++numberOfPedestriansGenerated >= maxNumberOfPedestriansToSpawn && !spawnPedestriansInInfiniteLoop)
            CancelInvoke();
    }

    /// <summary>
    /// Calculates the percentages of priority for each destination point. 
    /// </summary>
    /// <returns>An array with the accumulate percentages. Each percentage in position i in the 
    /// returned array corresponds with the destination i in the array of destinationPoints.
    /// </returns>
    public float[] getPrioritiesOfAllDestinations()
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
	public LogDataTree getLogData(){
		LogDataTree logData = new LogDataTree (tag,null);
        logData.AddChild(new LogDataTree("ID", id.ToString()));
        logData.AddChild(new LogDataTree("PositionX",transform.position.x.ToString()));
		logData.AddChild(new LogDataTree("PositionY",transform.position.y.ToString()));
		logData.AddChild(new LogDataTree("PositionZ",transform.position.z.ToString()));
		logData.AddChild(new LogDataTree("MaxNumberOfPedestriansToSpawn", maxNumberOfPedestriansToSpawn.ToString()));
		logData.AddChild(new LogDataTree("SpawnPedestriansInInfiteLoop",spawnPedestriansInInfiniteLoop.ToString()));
		logData.AddChild(new LogDataTree("MinPedestriansPerSpawningIteration",minPedestriansPerSpawningIteration.ToString()));
		logData.AddChild(new LogDataTree("MaxPedestriansPerSpawningIteration",maxPedestriansPerSpawningIteration.ToString()));
		logData.AddChild(new LogDataTree("PedestrianSpawnFrequencyInSeconds",spawningFrequencyInSeconds.ToString()));
		logData.AddChild(new LogDataTree("SpawningArea",spawningArea.ToString()));
		logData.AddChild(new LogDataTree("RadiousToCheckStations",radiousToCheckStations.ToString()));
		logData.AddChild(new LogDataTree("SpawningDelayAtStart",spawningDelayAtStart.ToString()));
		logData.AddChild(new LogDataTree("InitialNumberOfPedestriansInCache",initialNumberOfPedestriansInCache.ToString()));
		logData.AddChild(new LogDataTree("NumberOfPedestriansGenerated",numberOfPedestriansGenerated.ToString()));
		logData.AddChild(new LogDataTree("NumberOfPedestriansOnDestination",numberOfPedestriansOnDestination.ToString()));
		return logData;
	}

	public void rebuildFromLog(LogDataTree logData){
		GameObject flashSpawnerObject = GameObject.Instantiate(gameObject) as GameObject;
		FlashPedestriansSpawner flashSpawnerScript = flashSpawnerObject.GetComponent<FlashPedestriansSpawner>();
		Vector3 position = new Vector3();
		position.x = float.Parse(logData.GetChild("PositionX").Value);
		position.y = float.Parse(logData.GetChild("PositionY").Value);
		position.z = float.Parse(logData.GetChild("PositionZ").Value);
		flashSpawnerObject.transform.position = position;
		flashSpawnerScript.transform.position = position;
		flashSpawnerScript.maxNumberOfPedestriansToSpawn = int.Parse(logData.GetChild("MaxNumberOfPedestriansToSpawn").Value);
		flashSpawnerScript.spawnPedestriansInInfiniteLoop = bool.Parse(logData.GetChild("SpawnPedestriansInInfiteLoop").Value);
		flashSpawnerScript.minPedestriansPerSpawningIteration = int.Parse(logData.GetChild("MinPedestriansPerSpawningIteration").Value);
		flashSpawnerScript.maxPedestriansPerSpawningIteration = int.Parse(logData.GetChild("MaxPedestriansPerSpawningIteration").Value);
		flashSpawnerScript.spawningFrequencyInSeconds = float.Parse(logData.GetChild("PedestrianSpawnFrequencyInSeconds").Value);
		flashSpawnerScript.spawningArea = float.Parse(logData.GetChild("SpawningArea").Value);
		flashSpawnerScript.radiousToCheckStations = float.Parse(logData.GetChild("RadiousToCheckStations").Value);
		flashSpawnerScript.spawningDelayAtStart = float.Parse(logData.GetChild("SpawningDelayAtStart").Value);
		flashSpawnerScript.initialNumberOfPedestriansInCache = int.Parse(logData.GetChild("InitialNumberOfPedestriansInCache").Value);
		flashSpawnerScript.numberOfPedestriansGenerated = int.Parse(logData.GetChild("NumberOfPedestriansGenerated").Value);
		flashSpawnerScript.numberOfPedestriansOnDestination = int.Parse(logData.GetChild("NumberOfPedestriansOnDestination").Value);
		flashSpawnerObject.name = "FlashSpawner";
		flashSpawnerObject.transform.parent = GameObject.Find("SpawnerPoints").transform;
		flashSpawnerScript.initializeSpawner();
		flashSpawnerScript.enabled = true;
        print("TEST");
	}

	public  LogPriorities getPriorityLevel()
    {
		return LogPriorities.High;
    }
}


