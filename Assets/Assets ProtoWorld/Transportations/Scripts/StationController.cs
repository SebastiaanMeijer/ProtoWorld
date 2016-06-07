using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

//[System.Serializable]
public class StationController : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;

    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int id;

    /// <summary>
    /// Station name.
    /// </summary>
    public string stationName;

    /// <summary>
    /// Keep a list of lines going through this station.
    /// </summary>
    public HashSet<LineController> lineThruThisStation = new HashSet<LineController>();

    /// <summary>
    /// Indicate whether this station is in or out of service.
    /// </summary>
    public bool outOfService;

    public Dictionary<string, Queue<TravelerController>> lineQueues;

    public int capacity;

    public int queuing;

    [System.Serializable]
    public class QueueStats
    {
        [HideInInspector]
        public string name;
        public int queuing;
    }

    public List<QueueStats> queueStats;

    [Range(0, 10)]
    public float LogUpdateRateInSeconds = 1.0f;
    private float nextLogUpdate = 0.0f;

    [Range(1, 10000)]
    public float radiusToCheckStations = 1000;

    /// <summary>
    /// Array of colliders containing the stations close to this station.
    /// </summary>
    private StationController[] stationsNearThisStation;

    public static StationController CreateStation(int id, Vector3 point, string name = "", bool loadStationWithLOD = true)
    {
        GameObject sgo = null;
        StationController controller = null;

#if UNITY_EDITOR
        if (loadStationWithLOD)
        {
            sgo = PrefabUtility.InstantiatePrefab(Resources.Load("StationWithLOD")) as GameObject;
            controller = sgo.GetComponent<StationController>();
        }
        else
        {
            sgo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            controller = sgo.AddComponent<StationController>();
        }
        PrefabUtility.DisconnectPrefabInstance(sgo);

        sgo.name = id.ToString();
        sgo.transform.position = point;
        sgo.tag = "TransStation";
        sgo.layer = LayerMask.NameToLayer("Stations");

        controller.SetId(id);
        controller.stationName = name;
        controller.outOfService = false;
        controller.capacity = int.MaxValue;
        controller.radiusToCheckStations = 1000;

        var tgo = new GameObject(name);
        tgo.transform.SetParent(sgo.transform);
        tgo.transform.localEulerAngles = new Vector3(90, -135, 0);
        tgo.transform.localPosition = new Vector3(0, 100, 0);

        var textMesh = tgo.AddComponent<TextMesh>();
        textMesh.text = controller.GetIdAndName();
        textMesh.anchor = TextAnchor.LowerCenter;
        textMesh.color = Color.black;
        textMesh.fontSize = 20;
        textMesh.fontStyle = FontStyle.Bold;
        tgo.transform.localScale *= 25;
#endif

        return controller;
    }

    public static GameObject CreateGameObject(BaseStation station, bool loadStationWithLOD = true)
    {
        var controller = CreateStation(station.id, new Vector3(station.x, station.y, station.z), station.name, loadStationWithLOD);
        if (controller == null)
            return null;
        else
            return controller.gameObject;
    }

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        //LOG STATION LOG INFO
        log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", GetIdAndName() + " log"));
        //LOG STATION QUEUING CHART INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 0, UIChartTypes.Line.ToString()));
        //LOG STATION LOG INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, GetIdAndName() + " queuing"));

        Collider[] coll = Physics.OverlapSphere(this.transform.position, radiusToCheckStations * 10, 1 << LayerMask.NameToLayer("Stations"));

        List<StationController> aux = new List<StationController>();

        // Add all the stations found except for this one
        foreach (Collider C in coll)
        {
            StationController station = C.GetComponent<StationController>();

            if (!station.Equals(this))
                aux.Add(station);
        }

        stationsNearThisStation = aux.ToArray();
    }

    void Start()
    {
        nextLogUpdate = Time.time + LogUpdateRateInSeconds;
    }

    /// <summary>
    /// Update the statistics of queuing people.
    /// </summary>
    void Update()
    {
        queuing = GetHeadCount();

        if (nextLogUpdate < Time.time)
        {
            nextLogUpdate += LogUpdateRateInSeconds;

            //LOG STATION QUEUING
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "int", 0, queuing));
        }
    }

    /// <summary>
    /// Empty the station if out of service after all other Updates have been called.
    /// </summary>
    void LateUpdate()
    {
        if (outOfService)
            EmptyTravelersAtStation();
    }

    /// <summary>
    /// Assign automatically the unique identifier
    /// </summary>
    /// <returns></returns>
    public string SetId(int id)
    {
        this.id = id;
        return GetId();
    }

    /// <summary>
    /// Get the unique identifier
    /// </summary>
    /// <returns></returns>
    public string GetId()
    {
        return id.ToString();
    }

    /// <summary>
    /// Called by RoutingModel to set up queues for different lines that go through the station.
    /// Even Walking and Bike lines are included
    /// </summary>
    public void InitLineQueues()
    {
        if (lineThruThisStation == null)
        {
            Debug.LogError("lineThruThisStation == null");
            return;
        }
        lineQueues = new Dictionary<string, Queue<TravelerController>>();
        foreach (var line in lineThruThisStation)
        {
            string keyString = LineController.MakeKeyString(line.id, LineDirection.OutBound);
            if (!lineQueues.ContainsKey(keyString))
                lineQueues.Add(keyString, new Queue<TravelerController>()); //Queue for outbound.
            keyString = LineController.MakeKeyString(line.id, LineDirection.InBound);
            if (!lineQueues.ContainsKey(keyString))
                lineQueues.Add(keyString, new Queue<TravelerController>()); //Queue for inbound.
        }
        foreach (var queue in lineQueues)
        {
            queueStats.Add(new QueueStats
            {
                name = queue.Key,
                queuing = 0
            });
        }
    }

    /// <summary>
    /// This will look at what queue the traveler should go to and
    /// put it in that queue.
    /// </summary>
    /// <param name="traveler"></param>
    public void QueueTraveler(TravelerController traveler)
    {
        if (outOfService)
        {
            traveler.ArrivedAt(this);
        }
        else
        {
            string lineId = traveler.GetNextLineQueueId();
            Queue<TravelerController> queue;
            if (lineQueues.TryGetValue(lineId, out queue))
            {
                queue.Enqueue(traveler);
            }
            else
            {
                Debug.LogWarningFormat("TravelerController went to the wrong station?!");
            }
            //Debug.Log(ToString());
        }
    }

    /// <summary>
    /// This will go through the list of travelers and "tell" them that they have arrived to this station.
    /// </summary>
    /// <param name="disembarkers"></param>
    public void HandleDisembarkers(List<TravelerController> disembarkers)
    {
        //Get them out on the street or transit.
        foreach (var disembarker in disembarkers)
        {
            disembarker.ArrivedAt(this);
        }
        disembarkers.Clear();
    }

    /// <summary>
    /// Get how many people is queuing for the different lines 
    /// and also the total number of people queuing.
    /// </summary>
    /// <returns></returns>
    public int GetHeadCount()
    {
        queuing = 0;
        Queue<TravelerController> temp;
        if (lineQueues != null)
        {
            foreach (var queue in queueStats)
            {
                if (lineQueues.TryGetValue(queue.name, out temp))
                {
                    queue.queuing = temp.Count();
                    queuing += queue.queuing;
                }
            }
        }
        return queuing;
    }

    public void SetStationName(string stationName)
    {
        this.stationName = stationName;
        var textMesh = GetComponentInChildren<TextMesh>();
        textMesh.name = stationName;
        textMesh.text = GetIdAndName();
    }

    /// <summary>
    /// Returns a string with the id and name of this station.
    /// </summary>
    /// <returns></returns>
    public string GetIdAndName()
    {
        return gameObject.name + ") " + stationName;
    }

    /// <summary>
    /// Returns a string with the id and name of this station, plus the name of the queues.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string str = "";
        if (lineQueues != null)
        {
            foreach (var q in lineQueues)
            {
                str += "(Q " + q.Key + "): " + q.Value.Count + ", ";
            }
        }
        return "Station: " + gameObject.name + ") " + stationName + " " + str;
    }

    /// <summary>
    /// As long as there is only one way to travel between two stations.
    /// Return -1 if the station is not on the same line.
    /// Return -1 if the stations are the same one.
    /// </summary>
    /// <param name="first">First station</param>
    /// <param name="second">Second station</param>
    /// <returns></returns>
    public static int GetCommonLineId(StationController first, StationController second)
    {
        if (first.Equals(second))
            return -1;
        foreach (var line in first.lineThruThisStation)
        {
            if (second.lineThruThisStation.Contains(line))
            {
                return line.id;
            }
        }
        return -1;
    }

    /// <summary>
    /// Comparing the station name of this and the other station.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(StationController other)
    {
        if (other == null)
            return false;

        if (this.id == other.id)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Empty the travelers in the queues when the station is out of service.
    /// By calling the ArrivedAt(station) will make them check for a new itinerary.
    /// </summary>
    internal void EmptyTravelersAtStation()
    {
        foreach (var queue in lineQueues.Values)
        {
            if (queue.Count > 0)
            {
                var traveler = queue.Dequeue();
                traveler.ArrivedAt(this);
            }
        }
    }

    /// <summary>
    /// Return a list of StationController that are closest to the stations.
    /// The Length might varies.
    /// </summary>
    /// <returns></returns>
    public StationController[] GetClosestStations()
    {
        return stationsNearThisStation;
    }


}
