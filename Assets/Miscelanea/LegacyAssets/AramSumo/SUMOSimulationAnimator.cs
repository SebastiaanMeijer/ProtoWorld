using System.Collections;
using System.Globalization;
using GapslabWCFservice;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;



//[ExecuteInEditMode()]
public class SUMOSimulationAnimator : MonoBehaviour
{

	public int GUIWindowId = 1234;
	[Compact]
	public Rect GUIWindow = new Rect(100, 100, 100, 100);
	private MapBoundaries mapBoundaries;
	private BoundsWCF GameBoundaries;
	public BoundsWCF boundsTemp;
	private float[] GameBoundLat;
	private float[] GameBoundLon;
	private float[] MinMaxLat;
	private float[] MinMaxLon;
	private Vector3 MinPointOnMap;
	private string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase;
	public ServiceGapslabsClient client;
	public bool SimulationLoaded = false;
	int direction = -1;
	public string Description;
	public Material CarMaterial;
	public Transform CarObject;
	public int UpdateCarsPerTurn = 20;
	public string SelectedSimulation = @"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\bufferoutput - 50.xml";
	public int ObtainedSimulationID = -1;
	public string ObjectIDPostfix = "";
	public string WindowTitle = "SUMO Simulation Animator";
	public bool AssumeUnityCoordinates = false;
	public Vector2 UnityScaleFactorDivider = new Vector2(1, 1);
	public bool SwapXY = false;
	private bool AskForUpdate = false;


	// Use this for initialization
	void Start()
	{
		var go = GameObject.Find("AramGISBoundaries");
		var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;

		ObtainedSimulationID = -1;
		Initialize();
		if (UnityScaleFactorDivider.x == 0 || UnityScaleFactorDivider.y == 0)
			UnityScaleFactorDivider = new Vector2(1, 1);
	}
	float CleanUpTimer = 0;
	public bool Threaded = false;
	// Update is called once per frame
	void Update()
	{
		if (SimulationLoaded)
			if (Input.GetKeyDown(KeyCode.P))
				StartCoroutine(UpdateTimeStepCars());
		if (AskForUpdate)
		{
			AskForUpdate = false;
			UpdateCurrentTimeStep = false;

			//Loom.QueueOnMainThread(() =>
			//{
			timing = System.Diagnostics.Stopwatch.StartNew();
			CurrentTimeStep = client.GetTimestepAtList(CurrentTimeStepIndex, ObtainedSimulationID);
			timing.Stop();
			Interpolations.MyLog("Timestep retrieved in " + timing.ElapsedMilliseconds + " milliseconds.");
			//});

			StartCoroutine(UpdateTimeStepCars());
		}
		//if (!Updating && Time.time - CleanUpTimer > 1)
		//{
		//    CleanUp();
		//    CleanUpTimer = Time.time;
		//}
		if (AutoPlay && IsLoadingNextTimeAllowed)
		{
			//if (Threaded)
			//    Interpolations.MyLog("Thread state when asking for the next update:" +updateThread.ThreadState);
			if (Time.time - OldTime > TimestepIntervalInSeconds)
			{
				OldTime = Time.time;
				if (PlayForward)
				{
					if (CurrentTimeStepIndex < totalTimesteps - 1)
						CurrentTimeStepIndex++;
					else AutoPlay = false;
				}
				else
				{
					if (CurrentTimeStepIndex > 0)
						CurrentTimeStepIndex--;
					else AutoPlay = false;
				}
			}
		}


	}

	void OnGUI()
	{
		GUIWindow = GUILayout.Window(GUIWindowId, GUIWindow, ShowWindow, WindowTitle);

	}

	public float TimestepIntervalInSeconds = 1;
	public string tempTime = "1";
	public GapslabWCFservice.TimeStep CurrentTimeStep;
	public int CurrentTimeStepIndex = 0;
	private int oldTimeStepIndex = 0;
	private int totalTimesteps;
	private bool simulationIndexControl = false;
	private float ShortDelay = 0.1f;
	private float LongDelay = 0.3f;
	private bool UpdateCurrentTimeStep = false;
	private bool DestroyAfterLifetimeEnds = false;
	private float Lifetime = 30;
	private bool Updating = false;
	private float OldTime = 0;
	public bool AutoPlay = false;
	public bool PlayForward = true;
	private bool RequestingSimulationLoad = false;
	private bool IsLoadingNextTimeAllowed = true;
	//public bool SmoothPlayBack = false;
	//public float SmoothParameter = 0.5f;
	// Errors on parmeters of the service?
	// Visit http://answers.unity3d.com/questions/270072/failure-to-convert-parameter-when-serializing.html
	void ShowWindow(int WindowId)
	{
		GUIStyle labelCenter = new GUIStyle(GUI.skin.label);
		labelCenter.alignment = TextAnchor.LowerCenter;
		if (WindowId == GUIWindowId)
		{
			if (!SimulationLoaded && GUILayout.Button("Load simulation"))
			{
				if (!RequestingSimulationLoad)
				{
					RequestingSimulationLoad = true;
					client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
					// client.LoadSUMOFCDSimulation(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMOData\fcdoutput.xml");
					//client.LoadSUMOFCDSimulation(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\bufferoutput - 50.xml");
					//client.LoadSUMOFCDSimulation(@"C:\Users\admgaming\Desktop\Dropbox\GaPSLabs\SUMO Packet Tester\bufferoutput - 500.xml");
					//client.LoadSUMOFCDSimulation(@"C:\Users\admgaming\Desktop\Notable Software\iMobility\stkhlm-taxi.csv");
					//client.LoadSUMOFCDSimulation(SelectedSimulation);

					ObjectIDPostfix = "__Simulator";
					ObtainedSimulationID = client.LoadSUMOFCDSimulationList(SelectedSimulation, ObjectIDPostfix);

				}
			}
			if (RequestingSimulationLoad)
			{
				if (client.IsSimulationLoadedList(ObtainedSimulationID))
				{
					SimulationLoaded = true;
					RequestingSimulationLoad = false;
					Description = "Total timesteps: " + client.GetTotalTimestepsList(ObtainedSimulationID);
					totalTimesteps = client.GetTotalTimestepsList(ObtainedSimulationID);
					CurrentTimeStep = client.GetTimestepAtList(0, ObtainedSimulationID);
					Interpolations.MyLog("Total timesteps: " + client.GetTotalTimestepsList(ObtainedSimulationID));
				}
				else
				{
					GUILayout.Label("Simulation Requested - Loading simulation..");
				}
			}
			if (SimulationLoaded)
			{
				if (!string.IsNullOrEmpty(Description))
					GUILayout.Label(Description);
				if (CurrentTimeStep.Vehicles != null)
				{
					//Debug.LogError("Null vehicle found");
					GUILayout.Label("Time (sim secs): " + CurrentTimeStep.time + "\n" + "# of Vehicles: " + CurrentTimeStep.Vehicles.Length);
				}
				GUILayout.Button("Start simulation");
				GUILayout.BeginHorizontal();
				GUILayout.Label("Delay between timesteps in seconds:");
				tempTime = GUILayout.TextField(tempTime);
				if (!float.TryParse(tempTime, NumberStyles.Any, CultureInfo.InvariantCulture, out TimestepIntervalInSeconds))
				{
					TimestepIntervalInSeconds = 1;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.RepeatButton("<<") && CurrentTimeStepIndex > 0)
				{
					if (!simulationIndexControl)
					{
						CurrentTimeStepIndex--;
						UpdateCurrentTimeStep = true;
						simulationIndexControl = true;
						StartCoroutine("DelayRepeatButton", ShortDelay);
					}
				}
				if (GUILayout.RepeatButton("<") && CurrentTimeStepIndex > 0)
				{
					if (!simulationIndexControl)
					{
						CurrentTimeStepIndex--;
						UpdateCurrentTimeStep = true;
						simulationIndexControl = true;
						StartCoroutine("DelayRepeatButton", LongDelay);
					}
				}
				GUILayout.Label(CurrentTimeStepIndex + "", labelCenter, GUILayout.MinWidth(30));
				if (GUILayout.RepeatButton(">") && CurrentTimeStepIndex < totalTimesteps)
				{
					if (!simulationIndexControl)
					{
						CurrentTimeStepIndex++;
						UpdateCurrentTimeStep = true;
						simulationIndexControl = true;
						StartCoroutine("DelayRepeatButton", LongDelay);
					}
				}
				if (GUILayout.RepeatButton(">>") && CurrentTimeStepIndex < totalTimesteps)
				{
					if (!simulationIndexControl)
					{
						CurrentTimeStepIndex++;
						UpdateCurrentTimeStep = true;
						simulationIndexControl = true;
						StartCoroutine("DelayRepeatButton", ShortDelay);
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Play Backward"))
				{
					AutoPlay = true;
					PlayForward = false;
				}
				if (GUILayout.Button("Stop"))
				{
					AutoPlay = false;
				}
				if (GUILayout.Button("Play Forward"))
				{
					AutoPlay = true;
					PlayForward = true;
				}
				GUILayout.EndHorizontal();
				CurrentTimeStepIndex = Mathf.RoundToInt(GUILayout.HorizontalSlider(CurrentTimeStepIndex, 0, totalTimesteps - 1));
				if (UpdateCurrentTimeStep || oldTimeStepIndex != CurrentTimeStepIndex)
				{
					//UpdateCurrentTimeStep = false;
					//timing = System.Diagnostics.Stopwatch.StartNew();
					//CurrentTimeStep = client.GetTimestepAtList(CurrentTimeStepIndex, ObtainedSimulationID);
					//timing.Stop();
					//Interpolations.MyLog("Timestep retrieved in " + timing.ElapsedMilliseconds + " milliseconds.");
					//StartCoroutine(UpdateTimeStepCars());
					AskForUpdate = true;
				}
				oldTimeStepIndex = CurrentTimeStepIndex;
				GUILayout.BeginHorizontal();
				DestroyAfterLifetimeEnds = GUILayout.Toggle(DestroyAfterLifetimeEnds, "Destroy objects after ");
				if (DestroyAfterLifetimeEnds)
				{
					var tempLifetime = GUILayout.TextField(Lifetime + "", 4);
					var oldLifetime = Lifetime;
					try
					{
						Lifetime = float.Parse(tempLifetime);
					}
					catch (Exception e)
					{
						Lifetime = oldLifetime;
					}
					GUILayout.Label("seconds.");
				}
				GUILayout.EndHorizontal();
				// SmoothPlayBack = GUILayout.Toggle(SmoothPlayBack, "Smooth Playback");

			}

			GUI.DragWindow(new Rect(0, 0, 10000, GUIWindow.height));
		}
	}
	private IEnumerator DelayRepeatButton(float Seconds)
	{
		yield return new WaitForSeconds(Seconds);
		simulationIndexControl = false;
	}

	System.Diagnostics.Stopwatch timing;
	System.Diagnostics.Stopwatch PerformanceControl;
	public int ComputationHandoverAfterMilliseconds = 80;
	private System.Threading.Thread updateThread;
	//void UpdateTimeStepCarsSingleThread()
	//{
	//		Updating = true;

	//		if (CurrentTimeStep.Vehicles != null)
	//		{

	//				int RunCount = 0;
	//				PerformanceControl = System.Diagnostics.Stopwatch.StartNew();
	//				IsLoadingNextTimeAllowed = false;
	//				var currentSceneList = GameObject.FindGameObjectsWithTag(GlobalConstants.AI_Tags.Car)
	//						.Where(i => i.name.EndsWith(ObjectIDPostfix + ObtainedSimulationID))
	//						.Select(i => i.transform)
	//						.ToDictionary(f => f.name);
	//				#region     workload
	//				var cv = CurrentTimeStep.Vehicles.Select(i => new MinimalVehicle(i.Id, i.Latitude, i.Longitude)).ToArray();
	//				updateThread= Loom.StartSingleThread(
	//				delegate()
	//				{
	//						for (int i = 0; i < cv.Length; i++)
	//						{
	//								cv[i].TempVec = new Vector3();
	//								if (!AssumeUnityCoordinates)
	//								{
	//										var result = Interpolations.SimpleInterpolation(cv[i].Latitude, cv[i].Longitude, ref boundsTemp, GameBoundLat, GameBoundLon);
	//										cv[i].TempVec = new Vector3(direction * result[0], 1, result[1]) - MinPointOnMap;
	//										cv[i].TempVec.x *= mapBoundaries.Scale.x;
	//										cv[i].TempVec.z *= mapBoundaries.Scale.y;
	//								}
	//								else
	//								{
	//										if (!SwapXY)
	//										{
	//												cv[i].TempVec.x = cv[i].Longitude;
	//												cv[i].TempVec.y = 1;
	//												cv[i].TempVec.z = cv[i].Latitude;
	//												cv[i].TempVec.x /= UnityScaleFactorDivider.x;
	//												cv[i].TempVec.z /= UnityScaleFactorDivider.y;
	//										}
	//										else
	//										{
	//												cv[i].TempVec.z = cv[i].Longitude;
	//												cv[i].TempVec.y = 1;
	//												cv[i].TempVec.x = cv[i].Latitude;
	//												cv[i].TempVec.z /= UnityScaleFactorDivider.x;
	//												cv[i].TempVec.x /= UnityScaleFactorDivider.y;
	//										}
	//								}
	//								cv[i].found = currentSceneList.ContainsKey(cv[i].Id);
	//												//Transform seekingCar;
	//												//if (currentSceneList.TryGetValue(cv[i].Id, out seekingCar))
	//												//{
	//												//    seekingCar.position = cv[i].TempVec;
	//												//    return true; //cv[i].found = true;
	//												//}
	//												//else return false;

	//						}
	//						Loom.DispatchToMainThread(delegate()
	//						{
	//								for (int ii = 0; ii < cv.Length; ii++)
	//								{
	//										if (!cv[ii].found)
	//										{
	//												if (CarObject == null)
	//												{
	//														GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
	//														c.GetComponent<BoxCollider>().isTrigger = true;
	//														c.renderer.sharedMaterial = CarMaterial;
	//														c.name = cv[ii].Id;
	//														c.tag = "Car";
	//														c.transform.position = cv[ii].TempVec;
	//														var life = c.AddComponent<ObjectLifeTime>();
	//														c.AddComponent<NavMeshObstacle>();
	//														life.LastUpdate = DateTime.Now;
	//												}
	//												else
	//												{
	//														var c = ((Transform)GameObject.Instantiate(CarObject, cv[ii].TempVec, Quaternion.identity)).gameObject;
	//														c.name = cv[ii].Id;
	//														c.tag = "Car";
	//														var life = c.AddComponent<ObjectLifeTime>();
	//														// c.AddComponent<NavMeshObstacle>();
	//														life.LastUpdate = DateTime.Now;
	//												}
	//										}
	//										else
	//										{
	//												// the car exists
	//												try
	//												{
	//														currentSceneList[cv[ii].Id].position = cv[ii].TempVec;
	//												}
	//												catch (Exception e)
	//												{
	//														Debug.LogWarning("Should not throw error");
	//														Debug.LogException(e);
	//												}
	//										}
	//								}

	//								CleanUp(ref currentSceneList, ref CurrentTimeStep);
	//								currentSceneList = null;
	//								IsLoadingNextTimeAllowed = true;
	//								Updating = false;
	//						});


	//				}, System.Threading.ThreadPriority.AboveNormal);
	//				#endregion

	//		}


	//}

	//void OnUpdateCarCompleteParallel(MinimalVehicle[] allV)
	//{
	//		#region ah
	//		Loom.DispatchToMainThread(delegate()
	//		{
	//				var currentSceneList = GameObject.FindGameObjectsWithTag(GlobalConstants.AI_Tags.Car)
	//				.Where(i => i.name.EndsWith(ObjectIDPostfix + ObtainedSimulationID))
	//				.Select(i => i.transform)
	//				.ToDictionary(f => f.name);
	//				CleanUp(ref currentSceneList, ref CurrentTimeStep);
	//				currentSceneList = null;

	//				IsLoadingNextTimeAllowed = true;
	//				Updating = false;
	//				for (int ii = 0; ii < allV.Length; ii++)
	//				{
	//						if (!allV[ii].found)
	//						{
	//								if (CarObject == null)
	//								{
	//										GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
	//										c.GetComponent<BoxCollider>().isTrigger = true;
	//										c.renderer.sharedMaterial = CarMaterial;
	//										c.name = allV[ii].Id;
	//										c.tag = "Car";
	//										c.transform.position = allV[ii].TempVec;
	//										var life = c.AddComponent<ObjectLifeTime>();
	//										c.AddComponent<NavMeshObstacle>();
	//										life.LastUpdate = DateTime.Now;
	//								}
	//								else
	//								{
	//										var c = ((Transform)GameObject.Instantiate(CarObject, allV[ii].TempVec, Quaternion.identity)).gameObject;
	//										c.name = allV[ii].Id;
	//										c.tag = "Car";
	//										var life = c.AddComponent<ObjectLifeTime>();
	//										// c.AddComponent<NavMeshObstacle>();
	//										life.LastUpdate = DateTime.Now;
	//								}
	//						}
	//				}
	//		});
	//		#endregion
	//}

	IEnumerator UpdateTimeStepCars()
	{
		Updating = true;
		bool found = false;
		if (CurrentTimeStep.Vehicles != null)
		{
			int RunCount = 0;
			PerformanceControl = System.Diagnostics.Stopwatch.StartNew();
			IsLoadingNextTimeAllowed = false;
			var currentSceneList = GameObject.FindGameObjectsWithTag(GlobalConstants.AI_Tags.Car)
					.Where(i => i.name.EndsWith(ObjectIDPostfix + ObtainedSimulationID))
					.Select(i => i.transform)
					.ToDictionary(f => f.name);

			var cv = CurrentTimeStep.Vehicles;
			for (int vidx = 0; vidx < cv.Length; vidx++)
			{
				if (PerformanceControl.ElapsedMilliseconds > ComputationHandoverAfterMilliseconds) // If it has been running the code for more than 100ms
				{
					yield return new WaitForEndOfFrame();
					PerformanceControl.Reset();
					PerformanceControl.Start();
					//Debug.LogWarning("Handed over to the renderer after "+RunCount+" updates.");
					RunCount = 0;
				}
				//if (RunCount % UpdateCarsPerTurn == 0)
				//    yield return new WaitForEndOfFrame();// WaitForSeconds(0.05f);
				RunCount++;

				Vector3 TempVec;
				if (!AssumeUnityCoordinates)
				{

					var result = Interpolations.SimpleInterpolation(cv[vidx].Latitude, cv[vidx].Longitude, ref boundsTemp, GameBoundLat, GameBoundLon);
					TempVec = new Vector3(direction * result[0], 1, result[1]) - MinPointOnMap;
					//TempVec = Vector3.Scale(TempVec, new Vector3(mapBoundaries.Scale.x, 1, mapBoundaries.Scale.y));
					TempVec.x *= mapBoundaries.Scale.x;
					TempVec.z *= mapBoundaries.Scale.y;
				}
				else
				{
					TempVec = new Vector3();
					if (!SwapXY)
					{
						TempVec.x = cv[vidx].Longitude;
						TempVec.y = 1;
						TempVec.z = cv[vidx].Latitude;
						TempVec.x /= UnityScaleFactorDivider.x;
						TempVec.z /= UnityScaleFactorDivider.y;
					}
					else
					{
						TempVec.z = cv[vidx].Longitude;
						TempVec.y = 1;
						TempVec.x = cv[vidx].Latitude;
						TempVec.z /= UnityScaleFactorDivider.x;
						TempVec.x /= UnityScaleFactorDivider.y;
					}
				}

				Transform seekingCar;
				if (currentSceneList.TryGetValue(cv[vidx].Id, out seekingCar))
				{
					//if (seekingCar.position != TempVec)
					seekingCar.position = TempVec;
					found = true;
				}
				if (!found)
				{
					if (CarObject == null)
					{
						GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
						c.GetComponent<BoxCollider>().isTrigger = true;
						c.GetComponent<Renderer>().sharedMaterial = CarMaterial;
						c.name = cv[vidx].Id;
						c.tag = "Car";
						c.transform.position = TempVec;
						var life = c.AddComponent<ObjectLifeTime>();
						c.AddComponent<NavMeshObstacle>();
						life.LastUpdate = DateTime.Now;
					}
					else
					{
						var c = ((Transform)GameObject.Instantiate(CarObject, TempVec, Quaternion.identity)).gameObject;
						c.name = cv[vidx].Id;
						c.tag = "Car";
						var life = c.AddComponent<ObjectLifeTime>();
						// c.AddComponent<NavMeshObstacle>();
						life.LastUpdate = DateTime.Now;
					}
					//var car = c.AddComponent<SUMOCar>();
					//car.Vehicle = v;
				}
				found = false;
			}
			//CleanUp(ref currentSceneList, ref CurrentTimeStep);
			currentSceneList = null;

			IsLoadingNextTimeAllowed = true;
		}

		Updating = false;
	}
    //void CleanUp(ref Dictionary<string, Transform> exhaustList, ref TimeStep current)
    //{
    //    if (CurrentTimeStep != null)
    //        if (CurrentTimeStep.Vehicles != null)
    //        {
    //            if (!DestroyAfterLifetimeEnds)
    //            {
    //                var vehicles = current.Vehicles.Select(i => i.Id).ToArray();
    //                (from c in exhaustList where !vehicles.Contains(c.Value.name) select c)
    //                        .ForEach(p => GameObject.DestroyImmediate(p.Value.gameObject));
    //                //foreach (var n in removeThese)
    //                //    GameObject.DestroyImmediate(n.Value.gameObject);
    //            }
    //            else
    //            {
    //                var currentTime = DateTime.Now;
    //                foreach (var n in exhaustList)
    //                    if (currentTime - n.Value.GetComponent<ObjectLifeTime>().LastUpdate > TimeSpan.FromSeconds(Lifetime))
    //                        GameObject.DestroyImmediate(n.Value.gameObject);
    //            }
    //        }
    //}
	void Initialize()
	{
		client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

		Interpolations.MyLog("SUMO simulation initializing..");
		var go = GameObject.Find("AramGISBoundaries");
		mapBoundaries = go.GetComponent<MapBoundaries>();

		boundsTemp = client.GetBounds(wcfCon);

		MinMaxLat = new float[2];
		MinMaxLon = new float[2];
		// Setting local values for target boundaries (Openstreetmap database). Used in interpolation as destination boundary.
		MinMaxLat[0] = (float)boundsTemp.minlat;
		MinMaxLat[1] = (float)boundsTemp.maxlat;
		MinMaxLon[0] = (float)boundsTemp.minlon;
		MinMaxLon[1] = (float)boundsTemp.maxlon;



		// Setting local values for 3d world boundaries. Used in interpolation as source boundary
        GameBoundaries = new BoundsWCF();
		GameBoundaries.minlat = mapBoundaries.minMaxX[0];
		GameBoundaries.maxlat = mapBoundaries.minMaxX[1];
		GameBoundaries.minlon = mapBoundaries.minMaxY[0];
		GameBoundaries.maxlon = mapBoundaries.minMaxY[1];
        //if (mapBoundaries.CorrectAspectRatio)
        //{
        //    var aspectRatio = System.Math.Abs(boundsTemp.maxlat - boundsTemp.minlat) / System.Math.Abs(boundsTemp.maxlon - boundsTemp.minlon);
        //    GameBoundaries.maxlon = (float)(GameBoundaries.maxlat / aspectRatio);
        //}
        GameBoundLat = new float[2];
		GameBoundLat[0] = (float)GameBoundaries.minlat;
		GameBoundLat[1] = (float)GameBoundaries.maxlat;
		GameBoundLon = new float[2];
		GameBoundLon[0] = (float)GameBoundaries.minlon;
		GameBoundLon[1] = (float)GameBoundaries.maxlon;


       

		float[] MinPointOnArea =
				Interpolations.SimpleInterpolation(
						(float)mapBoundaries.minLat,
						(float)mapBoundaries.minLon,
						boundsTemp,
						GameBoundLat, GameBoundLon);
		MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);

		Interpolations.MyLog("SUMO simulation initialized successfully.");
	}

	void OnDisable()
	{
		if (ObtainedSimulationID != -1)
			client.DisposeSimulation(ObtainedSimulationID.ToString());
		client = null;
		GC.Collect();
	}
}
