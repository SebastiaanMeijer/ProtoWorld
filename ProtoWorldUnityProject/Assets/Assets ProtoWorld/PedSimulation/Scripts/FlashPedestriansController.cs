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
* FlashPedestriansController.cs
* Miguel Ramos Carretero
* Johnson Ho
* 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class FlashPedestriansController : TravelerController, Loggable
{
	public int uniqueId;

    public int spawnerId;

	public FlashPedestriansProfile profile;

	public FlashPedestriansRouting routing;

	[HideInInspector]
	public BikeStationScript targetedBikeStation;

	[Range(0, 10000)]
	public float radiousToCheckStations = 1000;

	[HideInInspector]
	public NavMeshAgent navAgent;

	[HideInInspector]
	public FlashPedestriansInformer flashInformer;

	private Animator FSM;

	private bool goingToDestination = false;

	private bool goingToStation = false;

	private bool takingABike = false;

	private bool goingBikingToDestination = false;

	private float carAwarenessTimer = 0.0f;
	private float carAwarenessFrequency = 1.0f;
	private float bikeAwarenessTimer = 0.0f;
	private float bikeAwarenessFrequency = 5.0f;

	private int roadMask;

	private bool embarked = false;

	private Transform nextPoint;

	private int stopIndex;

	[HideInInspector]
	public FlashPedestriansGlobalParameters globalParam;

	public Material materialForRumourCaught;

	public bool visualizeRumoursCaught = true;

	public bool balloonsEnabled = false;

	private Transform balloons;

	private FlashPedestriansGlobalParameters.WeatherConditions currentWeather;

	private bool isPause = false;

	[HideInInspector]
	public Heatmap heatMap;

	/// <summary>
	/// Awake method.
	/// </summary>
	void Awake()
	{
        initializePedestrian();
        LoggableManager.subscribe((Loggable)this);
    }

    public void initializePedestrian()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        FSM = gameObject.GetComponent<Animator>();
        balloons = transform.Find("Balloons");
    }

	/// <summary>
	/// Start method.
	/// </summary>
	void Start()
	{
		ResetStopIndex();
		navAgent.speed = profile.speed;
		navAgent.radius = 0.75f + UnityEngine.Random.Range(-0.25f, 0.25f); //Randomize "personal space"

		if (routing != null)
		if (routing.itinerary != null)
			stations = routing.itinerary.WayPoints;

		// Calculate the road mask (used if car awareness is active)
		roadMask = 1 << NavMesh.GetAreaFromName("vehicle road") | 1 << NavMesh.GetAreaFromName("residential") | 1 << NavMesh.GetAreaFromName("service") | 1 << NavMesh.GetAreaFromName("crosswalk");

		UpdateInfoBalloon();

		currentWeather = FlashPedestriansGlobalParameters.WeatherConditions.DefaultWeather;

		//Needed to put info about object into heatmaps array
		if(heatMap != null)
			heatMap.putInArray (this.transform.position.x, this.transform.position.y, this.transform.position.z, this.transform);


		Renderer rsp = GetComponentInParent<Renderer> ();
		//Deactivate render if zoomed out
		if (rsp.enabled != true) {
			StartCoroutine (LateStart (0.5f));
		}

	}

	IEnumerator LateStart(float waitTime){
		yield return new WaitForSeconds (waitTime);

		Renderer[] rs = GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in rs)
			r.enabled = false;

	}





	/// <summary>
	/// Update method.
	/// </summary>
	void Update()
	{
		// Check if the flash pedestrian is paused
		if (globalParam != null)
		{
			if (globalParam.flashPedestriansPaused && !isPause)
			{
				navAgent.speed = 0;
				isPause = true;
			}
			else if (!globalParam.flashPedestriansPaused && isPause)
			{
				navAgent.speed = profile.speed;

				if (goingBikingToDestination)
					navAgent.speed += 3;

				isPause = false;
			}
		}

		if (!isPause)
		{

			// Check the weather and update pedestrian behaviour accordingly
			if (currentWeather != flashInformer.globalParam.currentWeatherCondition)
			{
				ChangePedestrianBehaviourOnWeather(flashInformer.globalParam.currentWeatherCondition);
			}

			// If going to station, check if pedestrian has arrived to station
			if (goingToStation && Vector3.Distance(this.transform.position, nextPoint.position) < 4.0f)
			{
				// Arrives at the station
				goingToStation = false;
				FSM.SetBool("OnStation", true);
			}

			// If going to destination, check if pedestrian has arrived to destination
			if (goingToDestination && Vector3.Distance(this.transform.position, routing.destinationPoint.transform.position) < 4.0f)
			{
				// Arrives at the destination
				goingToDestination = false;
				FSM.SetBool("OnDestination", true);
			}

			// If going to take a bike, check if pedestrian has arrived to bike station
			if (takingABike && Vector3.Distance(this.transform.position, targetedBikeStation.transform.position) < 4.0f)
			{
				// Arrives at the bike station
				takingABike = false;
				FSM.SetBool("OnBikeStation", true);
			}

			// If going to take a bike an arrived and there is no bike station there, bike station has moved
			if (takingABike && (navAgent.remainingDistance < 3.0f || navAgent.isPathStale))
			{
				navAgent.ResetPath();
				navAgent.SetDestination(targetedBikeStation.transform.position);
			}

			// If going to destination biking, check if pedestrian has arrived to destination
			if (goingBikingToDestination && Vector3.Distance(this.transform.position, routing.destinationPoint.transform.position) < 4.0f)
			{
				// Arrives at the destination
				goingBikingToDestination = false;
				FSM.SetBool("OnDestination", true);
			}

			// If car awareness is active and pedestrian is walking or cycling, check if it is on a road
			if (profile.carAwareness && (goingToStation || goingToDestination || takingABike || goingBikingToDestination))
			{
				if (carAwarenessTimer > carAwarenessFrequency)
				{
					NavMeshHit navHit;
					if (navAgent.enabled && navAgent.hasPath && !navAgent.SamplePathPosition(-1, 2.0f, out navHit))
					{
						if ((navHit.mask & roadMask) != 0)
						{
							// Send message to the cars around
							InformVehiclesAround();
						}
					}

					carAwarenessTimer = 0.0f;
				}

				carAwarenessTimer += Time.deltaTime;
			}

			// If bikes are enabled, check if there are bike stations around
			if (flashInformer.globalParam.bikesEnabled && (!takingABike || !goingBikingToDestination))
			{
				// If the pedestrian is willing to take a bike, it will look for bike stations nearby. 
				if ((goingToStation || goingToDestination) && bikeAwarenessTimer > bikeAwarenessFrequency
					&& profile.chanceOfTakingABike + flashInformer.globalParam.percOfPedTakingBikes * profile.weatherFactorOnTakingBikes > 1.0f)
				{
					targetedBikeStation = flashInformer.FindBikeStationNearby(this.transform.position);

					if (targetedBikeStation != null && targetedBikeStation.capacityNumber > 0)
					{
						navAgent.enabled = false;
						GoToBikeStation();
					}

					bikeAwarenessTimer = 0.0f;
				}

				bikeAwarenessTimer += Time.deltaTime;
			}
		}
	}

	/// <summary>
	/// Send the pedestrian to check a bike station and try to grab a bike.
	/// </summary>
	private void GoToBikeStation()
	{
		//Decrease the stopIndex in case there are no more bikes and the pedestrian needs to take back its previous itinerary.
		stopIndex--;

		navAgent.enabled = true;
		navAgent.ResetPath();
		navAgent.SetDestination(targetedBikeStation.transform.position);

		goingToStation = false;
		goingToDestination = false;
		takingABike = true;

		FSM.SetBool("Biking", true);

		UpdateInfoBalloon();
	}

	/// <summary>
	/// Takes a bike from the bike station and go to the destination riding a bike. 
	/// </summary>
	internal void GoBikingToDestination()
	{
		takingABike = false;
		goingBikingToDestination = true;
		profile.speed += 3;
		navAgent.speed = profile.speed;

		Transform bike = this.transform.FindChild("bike");
		if (bike != null)
			bike.gameObject.SetActive(true);

		navAgent.ResetPath();
		navAgent.SetDestination(routing.destinationPoint.transform.position);

		UpdateInfoBalloon();
	}

	/// <summary>
	/// Pedestrian has arrived to a bike station out of bikes, so it will return to its normal itinerary. 
	/// </summary>
	internal void BikeStationIsEmpty()
	{
		navAgent.ResetPath();
		takingABike = false;

		// Spread the rumour around
		if (flashInformer.globalParam.rumoursEnabled)
			SpreadNoBikesRumour(targetedBikeStation);

		FSM.SetBool("OnBikeStation", false);
		FSM.SetBool("Biking", false);
	}

	/// <summary>
	/// Compute the navigation of the pedestrian according to its routing information.
	/// </summary>
	public void WalkToNextPoint()
	{
		navAgent.enabled = true;
		//navAgent.ResetPath();

		nextPoint = routing.GetPointOfTheItinerary(++stopIndex);

		if (nextPoint != null)
		{
			navAgent.SetDestination(nextPoint.position);
			goingToStation = true;
		}
		else
		{
			navAgent.SetDestination(routing.destinationPoint.transform.position);
			goingToDestination = true;
		}

		UpdateInfoBalloon();
	}

	/// <summary>
	/// Return the current stop where the pedestrian is at.
	/// </summary>
	/// <returns>Current stop or null if any.</returns>
	public override StationController CurrentStop()
	{
		return routing.GetStationAt(stopIndex);
	}

	/// <summary>
	/// This method is called only when the pedestrian has arrived to a certain station walking.
	/// </summary>
	internal void ArrivedWalkingAtStation()
	{
		ArrivedAt(routing.GetStationAt(stopIndex));
	}

	/// <summary>
	/// Controls the behaviour when the pedestrian arrives to a station. 
	/// </summary>
	/// <param name="station">Station where the pedestrian has arrived.</param>
	public override void ArrivedAt(StationController station)
	{
		// Update the current stop where the pedestrian is
		stopIndex = routing.GetIndexFrom(station);

		// Check if the itinerary is still valid in the routing
		if (station.outOfService || stopIndex == -1 || !routing.itinerary.IsValid(stopIndex))
		{
			RedoRouteFromStation(station);
			if (embarked)
				Disembark(station);
		}
		else
		{
			//Get the information of the next part of the route
			var info = routing.GetRouteInfoFrom(station);

			if (info == null || info.Category == LineCategory.Walk)
			{
				// The pedestrian will walk from this point
				FSM.SetBool("OnStation", false);
				if (embarked)
					Disembark(station);
			}
			else
			{
				// The pedestrian will take the commute from this point
				station.QueueTraveler(this);
			}
		}
	}

	/// <summary>
	/// Recomputes a new route in the itinerary from a certain station.
	/// </summary>
	/// <param name="station">Station where the new itinerary should start.</param>
	private void RedoRouteFromStation(StationController station)
	{
		// The current itinerary is not valid anymore: change route
		RedoRouteFromCurrentPosition(station.GetClosestStations());
		FSM.SetBool("OnStation", false);
	}

	/// <summary>
	/// Recomputes a new route in the itinerary from a certain point. 
	/// </summary>
	/// <param name="stationsNearby">List os stations nearby (null by default).</param>
	private void RedoRouteFromCurrentPosition(StationController[] stationsNearby = null)
	{
		UpdateInfoBalloon();

		navAgent.enabled = false;
		ResetStopIndex();

		if (stationsNearby != null)
			routing = new FlashPedestriansRouting(routing.destinationPoint,
				flashInformer.FindBestItinerary(this.transform.position, routing.destinationPoint, stationsNearby, profile.travelPreference));
		else
			//In this case, the pedestrian will find the nearby stations from its point
			routing = new FlashPedestriansRouting(routing.destinationPoint,
				flashInformer.FindBestItinerary(this.transform.position, routing.destinationPoint, StationsNearCurrentPosition(), profile.travelPreference));

		if (routing != null)
		if (routing.itinerary != null)
			stations = routing.itinerary.WayPoints;
	}

	/// <summary>
	/// Finds the stations near the current position of the pedestrian.
	/// </summary>
	/// <returns>List of stations nearby.</returns>
	private StationController[] StationsNearCurrentPosition()
	{
		// Get the stations that are around the spawner
		Collider[] coll = Physics.OverlapSphere(this.transform.position, radiousToCheckStations, 1 << LayerMask.NameToLayer("Stations"));

		List<StationController> aux = new List<StationController>();

		foreach (Collider C in coll)
			aux.Add(C.GetComponent<StationController>());

		return aux.ToArray();
	}

	/// <summary>
	/// Reset the stop index that controls the itinerary.
	/// </summary>
	private void ResetStopIndex()
	{
		stopIndex = -1;
	}

	/// <summary>
	/// Disembark the pedestrian from a certain station.
	/// </summary>
	/// <param name="station">Station from which the pedestrian will disembark.</param>
	public void Disembark(StationController station)
	{
		this.GetComponent<LODGroup>().enabled = true;
		this.transform.position = station.transform.position;
		embarked = false;

		UpdateInfoBalloon();
	}

	/// <summary>
	/// Embarks the pedestrian into a mode. This method is always called by StationController.
	/// </summary>
	/// <remarks>Why disabling the navAgent instead of calling ResetPath? -> When the agent 
	/// is "teleported" to another position after commuting, we need a clean and fresh
	/// navAgent to calculate the next path to go. If we keep this navAgent and we just call
	/// navAgent.ResetPath, when we respawned it there is the risk of a navAgent with its
	/// current position set wrong, which will incur in a wrong positioning after appearing
	/// again.</remarks>
	internal override void Embark()
	{
		//navAgent.ResetPath();
		navAgent.enabled = false;

		this.GetComponent<LODGroup>().enabled = false;
		this.transform.position = Vector3.zero;

		embarked = true;

		UpdateInfoBalloon(true);
	}

	/// <summary>
	/// Overriden method from TravellerController. Needed for correct behaviour when embarking.
	/// </summary>
	public override string GetNextLineQueueId()
	{
		return routing.GetNextLineQueueId(stopIndex);
	}

	/// <summary>
	/// Overriden method from TravellerController. Needed for correct behaviour when embarking.
	/// </summary>
	public override List<StationController> GetStagesTransits()
	{
		return routing.GetStatesTransits();
	}

	/// <summary>
	/// Recycles the pedestrian and keeps the gameobject on a queue for a future spawning. 
	/// </summary>
	internal void Recycle()
	{
		navAgent.ResetPath();
		stopIndex = -1;
		this.gameObject.transform.position = Vector3.zero;
		this.gameObject.GetComponentInParent<FlashPedestriansSpawner>().pedestrianCache.Enqueue(this.gameObject);
		this.gameObject.GetComponentInParent<FlashPedestriansSpawner>().numberOfPedestriansOnDestination++;
		flashInformer.UnsuscribePedestrian(this);

		Transform bike = this.transform.FindChild("bike");
		if (bike != null)
			bike.gameObject.SetActive(false);

		if(globalParam != null)
		{
			// Atomic operations for the KPI properties
			Interlocked.Increment(ref globalParam.numberOfPedestrianReachingDestination);
			Interlocked.Decrement(ref globalParam.numberOfPedestriansOnScenario);
		}

		this.gameObject.SetActive(false);
	}

	/// <summary>
	/// For pedestrians that are subscribed, checks the new information on the 
	/// routing system and reacts accordingly, finding any new optimal route. 
	/// </summary>
	internal bool CheckNewRoutingInfo()
	{
		bool itineraryChanged = false;

		if (goingToStation)
		{
			// Find a better itinerary
			Itinerary newItinerary = flashInformer.FindBestItinerary(this.transform.position, routing.destinationPoint, StationsNearCurrentPosition(), profile.travelPreference);

			if (!routing.itinerary.Equals(newItinerary))
			{
				// Spread the rumour
				if (flashInformer.globalParam.rumoursEnabled)
					SpreadItineraryRumour(routing.itinerary, newItinerary);

				//The itinerary has changed (the pedestrian redid its route)
				goingToStation = false;

				UpdateInfoBalloon();

				navAgent.enabled = false;
				ResetStopIndex();
				routing = new FlashPedestriansRouting(routing.destinationPoint, newItinerary);
				itineraryChanged = true;
				WalkToNextPoint();

				return true;
			}
		}

		return itineraryChanged;
	}

	/// <summary>
	/// Checks for vehicles around and informs them about the position of the pedestrian. 
	/// </summary>
	private void InformVehiclesAround()
	{
		Collider[] vehiclesAround = Physics.OverlapSphere(this.transform.position, 20.0f, LayerMask.GetMask("Vehicle"));

		foreach (var v in vehiclesAround)
		{
			v.SendMessage("PedestrianCrossingRoad", this.transform.position);
		}
	}

	/// <summary>
	/// Spreads around the current rumour that the pedestrian has in its knowledge. 
	/// </summary>
	internal void SpreadItineraryRumour(Itinerary oldItinerary, Itinerary newItinerary)
	{
		Collider[] pedestriansAffected = Physics.OverlapSphere(this.transform.position, flashInformer.globalParam.radiusOfSpreadingRumours, LayerMask.GetMask("Pedestrian"));

		Tuple<Itinerary, Itinerary> rumour = new Tuple<Itinerary, Itinerary>(oldItinerary, newItinerary);

		foreach (var p in pedestriansAffected)
		{
			if (p.transform == this.transform)
				continue;

			p.SendMessage("CheckItineraryRumour", rumour, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	/// Check a new rumour that comes from another pedestrian.
	/// </summary>
	/// <param name="rumour">Tuple containing the old itinerary (Item1) followed by the pedestrian spreading the rumour and the new alternative taken (Item2). </param>
	public void CheckItineraryRumour(Tuple<Itinerary, Itinerary> rumour)
	{
		// Check if the pedestrian is not subscribed to the app, 
		// if the rumour is concerning its own itinerary,
		// if the pedestrian will believe the rumour and
		// if the pedestrian is going to a station. 
		if (profile.chanceOfSubscription + flashInformer.globalParam.percOfPedSubscribed > 1.0f &&
			routing.itinerary.Equals(rumour.Item1) &&
			profile.chanceOfBelievingRumours + flashInformer.globalParam.percOfPedSusceptibleToRumours > 1.0f &&
			goingToStation)
		{
			//Trusting the rumour and changing the route
			goingToStation = false;
			navAgent.enabled = false;
			ResetStopIndex();
			routing = new FlashPedestriansRouting(routing.destinationPoint, rumour.Item2);
			WalkToNextPoint();

			//Change in visualization (for testing purposes)
			if (visualizeRumoursCaught && materialForRumourCaught != null)
			{
				LODGroup lodgroup = GetComponent<LODGroup>();
				LOD[] lods;

				if (lodgroup != null)
				{
					lods = lodgroup.GetLODs();

					foreach (LOD L in lods)
					{
						Renderer[] renderers = L.renderers;
						foreach (Renderer R in renderers)
						{
							R.material = materialForRumourCaught;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Spreads around the rumour that there are no more bikes on a certain bike station.
	/// </summary>
	/// <param name="targetedBikeStation">Bike station that is out of bikes.</param>
	internal void SpreadNoBikesRumour(BikeStationScript targetedBikeStation)
	{
		Collider[] pedestriansAffected = Physics.OverlapSphere(this.transform.position, flashInformer.globalParam.radiusOfSpreadingRumours, LayerMask.GetMask("Pedestrian"));

		foreach (var p in pedestriansAffected)
		{
			if (p.transform == this.transform)
				continue;

			p.SendMessage("CheckNoBikesRumour", targetedBikeStation, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	/// Check a rumour about a station out of bikes that comes from another pedestrian.
	/// </summary>
	/// <param name="rumour">Bike station that is out of bikes.</param>
	public void CheckNoBikesRumour(BikeStationScript stationOutOfBikes)
	{
		if (takingABike && targetedBikeStation.Equals(stationOutOfBikes))
		{
			// Arrives at the bike station
			takingABike = false;
			FSM.SetBool("OnBikeStation", true);
		}
	}

	/// <summary>
	/// Update the balloon info in the pedestrian. 
	/// </summary>
	private void UpdateInfoBalloon(bool disableAllBalloons = false)
	{
		if (balloonsEnabled && balloons != null)
		{
			balloons.Find("MetroBalloon").gameObject.SetActive(false);
			balloons.Find("WalkBalloon").gameObject.SetActive(false);
			balloons.Find("ThinkingBalloon").gameObject.SetActive(false);
			balloons.Find("BicycleBalloon").gameObject.SetActive(false);

			if (!disableAllBalloons)
			{
				if (goingToStation)
				{
					balloons.Find("MetroBalloon").gameObject.SetActive(true);
				}
				else if (goingToDestination)
				{
					balloons.Find("WalkBalloon").gameObject.SetActive(true);
				}
				else if (goingBikingToDestination)
				{
					balloons.Find("BicycleBalloon").gameObject.SetActive(true);
				}
				else
				{
					balloons.Find("ThinkingBalloon").gameObject.SetActive(true);
				}
			}
		}
	}

	/// <summary>
	/// Adapts the behaviour of the pedestrian according to the current weather conditions.
	/// </summary>
	/// <param name="newWeatherCondition">New weather condition.</param>
	private void ChangePedestrianBehaviourOnWeather(FlashPedestriansGlobalParameters.WeatherConditions newWeatherCondition)
	{
		// Set the behaviour related to the new weather condition
		switch (newWeatherCondition)
		{
		// On sunny weather: chance of taking bikes x1.5 and default speed:
		case FlashPedestriansGlobalParameters.WeatherConditions.SunnyWeather:
			profile.weatherFactorOnTakingBikes = 1.5f;
			navAgent.speed = profile.speed;
			break;

			// On windy weather: chance of taking bikes x0.75 and reduce speed:
		case FlashPedestriansGlobalParameters.WeatherConditions.WindyWeather:
			profile.weatherFactorOnTakingBikes = 0.75f;
			navAgent.speed = profile.speed - 0.5f;
			break;

			// On rainy weather: chance of taking bikes x0.5 and increase speed:
		case FlashPedestriansGlobalParameters.WeatherConditions.RainyWeather:
			profile.weatherFactorOnTakingBikes = 0.5f;
			navAgent.speed = profile.speed + 0.5f;
			break;

			// On default weather: chance of taking bikes x1.0 and default speed:
		default:
			profile.weatherFactorOnTakingBikes = 1.0f;
			navAgent.speed = profile.speed;
			break;
		}

		currentWeather = newWeatherCondition;
	}

    public LogDataTree getLogData()
    {
        LogDataTree logData = new LogDataTree(tag, null);
        logData.AddChild(new LogDataTree("ID", uniqueId.ToString()));
        logData.AddChild(new LogDataTree("SpawnerID", spawnerId.ToString()));
        logData.AddChild(new LogDataTree("PositionX", transform.position.x.ToString()));
        logData.AddChild(new LogDataTree("PositionY", transform.position.y.ToString()));
        logData.AddChild(new LogDataTree("PositionZ", transform.position.z.ToString()));
        logData.AddChild(new LogDataTree("RadiousToCheckStations", radiousToCheckStations.ToString()));
        logData.AddChild(new LogDataTree("GoingToDestination", goingToDestination.ToString()));
        logData.AddChild(new LogDataTree("GoingToStation", goingToStation.ToString()));
        logData.AddChild(new LogDataTree("TakingABike", takingABike.ToString()));
        logData.AddChild(new LogDataTree("GoingBikingToDestination", goingBikingToDestination.ToString()));
        logData.AddChild(new LogDataTree("CarAwarenessTimer", carAwarenessTimer.ToString()));
        logData.AddChild(new LogDataTree("CarAwarenessFrequency", carAwarenessFrequency.ToString()));
        logData.AddChild(new LogDataTree("BikeAwarenessTimer", bikeAwarenessTimer.ToString()));
        logData.AddChild(new LogDataTree("BikeAwarenessFrequency", bikeAwarenessFrequency.ToString()));
        logData.AddChild(new LogDataTree("RoadMask", roadMask.ToString()));
        logData.AddChild(new LogDataTree("Embarked", embarked.ToString()));
        logData.AddChild(new LogDataTree("StopIndex", stopIndex.ToString()));
        logData.AddChild(new LogDataTree("VisualizeRumoursCaught", visualizeRumoursCaught.ToString()));
        logData.AddChild(new LogDataTree("BalloonsEnabled", balloonsEnabled.ToString()));
        logData.AddChild(new LogDataTree("IsPause", isPause.ToString()));

        LogDataTree routingData = new LogDataTree("Routing", null);
        routingData.AddChild(new LogDataTree("DestinationName", routing.destinationPoint.destinationName));
        logData.AddChild(routingData);

        if (profile != null)
        {
            LogDataTree profileData = new LogDataTree("Profile", null);
            profileData.AddChild(new LogDataTree("CarAwareness",profile.carAwareness.ToString()));
            profileData.AddChild(new LogDataTree("ChanceOfBelievingRumours", profile.chanceOfBelievingRumours.ToString()));
            profileData.AddChild(new LogDataTree("ChanceOfSubscription", profile.chanceOfSubscription.ToString()));
            profileData.AddChild(new LogDataTree("ChanceOfTakingABike", profile.chanceOfTakingABike.ToString()));
            profileData.AddChild(new LogDataTree("EnglishSpeaker", profile.englishSpeaker.ToString()));
            profileData.AddChild(new LogDataTree("ItalianSpeaker", profile.italianSpeaker.ToString()));
            profileData.AddChild(new LogDataTree("Speed", profile.speed.ToString()));
            profileData.AddChild(new LogDataTree("WeatherFactorOnTakingBikes", profile.weatherFactorOnTakingBikes.ToString()));
            profileData.AddChild(new LogDataTree("WillingToChangeDestination", profile.willingToChangeDestination.ToString()));
            profileData.AddChild(new LogDataTree("TravelPreference", profile.travelPreference.ToString()));
            logData.AddChild(profileData);
        }
        if (nextPoint != null)
        {
            logData.AddChild(new LogDataTree("NextPointPositionX", nextPoint.transform.position.x.ToString()));
            logData.AddChild(new LogDataTree("NextPointPositionY", nextPoint.transform.position.y.ToString()));
            logData.AddChild(new LogDataTree("NextPointPositionZ", nextPoint.transform.position.z.ToString()));
        }
        return logData;
    }

    public void rebuildFromLog(LogDataTree logData)
    {
        GameObject flashPedestrianObject = GameObject.Instantiate(gameObject) as GameObject;
        FlashPedestriansController flashPedestrianScript = flashPedestrianObject.GetComponent<FlashPedestriansController>();
        Vector3 position = new Vector3();
        flashPedestrianScript.uniqueId = int.Parse(logData.GetChild("ID").Value);
        flashPedestrianScript.spawnerId = int.Parse(logData.GetChild("SpawnerID").Value);

        position.x = float.Parse(logData.GetChild("PositionX").Value);
        position.y = float.Parse(logData.GetChild("PositionX").Value);
        position.z = float.Parse(logData.GetChild("PositionX").Value);
        flashPedestrianScript.transform.position = position;
        flashPedestrianObject.transform.position = position;

        flashPedestrianScript.radiousToCheckStations = float.Parse(logData.GetChild("RadiousToCheckStations").Value);
        flashPedestrianScript.goingToDestination = bool.Parse(logData.GetChild("GoingToDestination").Value);
        flashPedestrianScript.goingToStation = bool.Parse(logData.GetChild("GoingToStation").Value);
        flashPedestrianScript.takingABike = bool.Parse(logData.GetChild("TakingABike").Value);
        flashPedestrianScript.goingBikingToDestination = bool.Parse(logData.GetChild("GoingBikingToDestination").Value);

        flashPedestrianScript.carAwarenessTimer = float.Parse(logData.GetChild("CarAwarenessTimer").Value);
        flashPedestrianScript.carAwarenessFrequency = float.Parse(logData.GetChild("CarAwarenessFrequency").Value);
        flashPedestrianScript.bikeAwarenessTimer = float.Parse(logData.GetChild("BikeAwarenessTimer").Value);
        flashPedestrianScript.bikeAwarenessFrequency = float.Parse(logData.GetChild("BikeAwarenessFrequency").Value);

        flashPedestrianScript.roadMask = int.Parse(logData.GetChild("RoadMask").Value);
        flashPedestrianScript.embarked = bool.Parse(logData.GetChild("Embarked").Value);
        flashPedestrianScript.stopIndex = int.Parse(logData.GetChild("StopIndex").Value);
        flashPedestrianScript.visualizeRumoursCaught = bool.Parse(logData.GetChild("VisualizeRumoursCaught").Value);
        flashPedestrianScript.balloonsEnabled = bool.Parse(logData.GetChild("BalloonsEnabled").Value);
        flashPedestrianScript.isPause = bool.Parse(logData.GetChild("IsPause").Value);

        if (logData.containsKey("Profile"))
        {
            LogDataTree profileData = logData.GetChild("Profile");
            bool carAwareness = bool.Parse(profileData.GetChild("CarAwareness").Value);
            float chanceOfBelievingRumours = float.Parse(profileData.GetChild("ChanceOfBelievingRumours").Value);
            float chanceOfSubscription = float.Parse(profileData.GetChild("ChanceOfSubscription").Value);
            float chanceOfTakingABike = float.Parse(profileData.GetChild("ChanceOfTakingABike").Value);
            bool englishSpeaker = bool.Parse(profileData.GetChild("EnglishSpeaker").Value);
            bool italianSpeaker = bool.Parse(profileData.GetChild("ItalianSpeaker").Value);
            float speed = float.Parse(profileData.GetChild("Speed").Value);
            float weatherFactorOnTakingBikes = float.Parse(profileData.GetChild("WeatherFactorOnTakingBikes").Value);
            bool willingToChangeDestination = bool.Parse(profileData.GetChild("WillingToChangeDestination").Value);
            TravelPreference travelPreference = (TravelPreference)Enum.Parse(typeof(TravelPreference), profileData.GetChild("TravelPreference").Value.ToString());
            flashPedestrianScript.profile = new FlashPedestriansProfile(speed, englishSpeaker, italianSpeaker, chanceOfBelievingRumours,
                willingToChangeDestination, chanceOfTakingABike, chanceOfBelievingRumours, carAwareness, travelPreference);
        }
        if (logData.containsKey("NextPointX"))
        {
            Vector3 nextPoint = new Vector3();

            nextPoint.x = float.Parse(logData.GetChild("NextPointX").Value);
            nextPoint.y = float.Parse(logData.GetChild("NextPointY").Value);
            nextPoint.z = float.Parse(logData.GetChild("NextPointZ").Value);

            GameObject tempNextPoint = new GameObject();
            tempNextPoint.transform.position = nextPoint;

            flashPedestrianScript.nextPoint = tempNextPoint.transform;
        }
        //GET THE DESTINATION//
        foreach (GameObject flashDestinationObject in GameObject.FindGameObjectsWithTag("PedestrianDestination"))
        {
            foreach (FlashPedestriansDestination flashDestinationScript in flashDestinationObject.GetComponents<FlashPedestriansDestination>())
            {
                if (flashDestinationScript.destinationName == logData.GetChild("Routing").GetChild("DestinationName").Value)
                {
                    flashPedestrianScript.routing = new FlashPedestriansRouting(flashDestinationScript, new Itinerary(null));
                    break;
                }
            }
        }
        //GET THE ORIGINAL SPAWNER//
        foreach (GameObject flashSpawnerObject in GameObject.FindGameObjectsWithTag("PedestrianSpawner"))
        {
            foreach (FlashPedestriansSpawner flashSpawnerScript in flashSpawnerObject.GetComponents<FlashPedestriansSpawner>())
            {
                if (flashSpawnerScript.id == flashPedestrianScript.spawnerId)
                {
                    //flashSpawnerScript.SpawnPedestrianFromLog(flashPedestrianScript);
                    break;
                }
            }
        }
    }

    public LogPriorities getPriorityLevel()
    {
        return LogPriorities.Default;
    }
}
