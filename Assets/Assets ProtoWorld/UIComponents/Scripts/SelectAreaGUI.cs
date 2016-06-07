using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

public class SelectAreaGUI : MonoBehaviour
{
    string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase;
    public GameObject AreaA;
    public GameObject AreaB;
    private RaycastHit hit;
    public Material AreaAMaterial;
    public Material AreaBMaterial;
    public UnityEngine.UI.Slider SliderAreaARadius;
    public UnityEngine.UI.Slider SliderAreaBRadius;
    public UnityEngine.UI.Toggle ToggleAreaA;
    public UnityEngine.UI.Toggle ToggleAreaB;
    public UnityEngine.UI.Text TextStatus;
    public GameObject BusModelAndBehaviour;
    // --- Bus Window
    public UnityEngine.UI.Image ImageBusOptions;
    public UnityEngine.UI.Text TextBusDistanceInputValue;
    public UnityEngine.UI.Text TextBusCapacityInputValue;
    public enum BusAgentType { RealPassengers, MescoPassengers };
    [HideInInspector]
    public BusAgentType SelectedBusAgentType;
    // ---
    [HideInInspector]
    public ColliderDetector AreaADetector;
    [HideInInspector]
    public ColliderDetector AreaBDetector;
    private GlobalSimulationPlannerBaseClass GlobalPlanner;
    [HideInInspector]
    public Dictionary<string, GapslabWCFservice.MemberWCF[]> AllRoutes;
    //[HideInInspector]
    //public List<GapslabWCFservice.MemberWCF[]> AllRoutes;
    //public bool AlwaysShowAreaMarkers = true;
    [HideInInspector]
    public GameObject[] routeObjects;
    private void ChooseConnection()
    {
        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;
    }
    // Use this for initialization	
    void Start()
    {
        ImageBusOptions.gameObject.SetActive(false);
        ChooseConnection();
        GlobalPlanner = GameObject.FindObjectOfType<GlobalSimulationPlannerBaseClass>();
        AllRoutes = new Dictionary<string, GapslabWCFservice.MemberWCF[]>();
        SliderAreaARadius.enabled = false;
        SliderAreaBRadius.enabled = false;
        if (AreaA == null)
        {
            AreaA = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            AreaA.GetComponent<SphereCollider>().isTrigger = true;
            var rigidA = AreaA.AddComponent<Rigidbody>();
            rigidA.isKinematic = true;
            AreaADetector = AreaA.AddComponent<ColliderDetector>();
            AreaADetector.FilterTag = "BusStop";
            AreaA.transform.position = new Vector3(0, 0, 0);
            AreaA.GetComponent<Renderer>().sharedMaterial = AreaAMaterial;
            AreaA.GetComponent<Renderer>().enabled = true;//false;
        }
        if (AreaB == null)
        {
            AreaB = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            AreaB.GetComponent<SphereCollider>().isTrigger = true;
            var rigidB = AreaB.AddComponent<Rigidbody>();
            rigidB.isKinematic = true;
            AreaBDetector = AreaB.AddComponent<ColliderDetector>();
            AreaBDetector.FilterTag = "BusStop";
            AreaB.transform.position = new Vector3(0, 0, 0);
            AreaB.GetComponent<Renderer>().sharedMaterial = AreaBMaterial;
            AreaB.GetComponent<Renderer>().enabled = true;//false;
        }
        if (AreaA.GetComponent<ColliderDetector>() == null)
        {
            AreaADetector = AreaA.AddComponent<ColliderDetector>();
        }
        if (AreaB.GetComponent<ColliderDetector>() == null)
        {
            AreaBDetector = AreaB.AddComponent<ColliderDetector>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (AreaA != null && AreaB != null)
        {
            if (SliderAreaARadius.enabled)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    var hitPosition = hit.point;
                    hitPosition.y = 0;
                    if (Input.GetMouseButton(2))
                    {
                        AreaA.transform.position = hitPosition;
                    }
                }
            }
            if (SliderAreaBRadius.enabled)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    var hitPosition = hit.point;
                    hitPosition.y = 0;
                    if (Input.GetMouseButton(2))
                    {
                        AreaB.transform.position = hitPosition;
                    }
                }
            }

            if (WaitForCompletion && AllRoutes.Count != 0)
            {
                WaitForCompletion = false;
                OnInfoThreadCompleted();
            }

            if (SignalRouteDrawThreadCompletion)
            {
                SignalRouteDrawThreadCompletion = false;
                OnRouteDrawThreadCompleted();
            }
        }
    }
    public void SelectAreaA()
    {
        if (ToggleAreaA.isOn)
        {
            SliderAreaARadius.enabled = true;
        }
        else
            SliderAreaARadius.enabled = false;
    }
    public void SelectAreaB()
    {
        if (ToggleAreaB.isOn)
        {
            SliderAreaBRadius.enabled = true;
        }
        else
            SliderAreaBRadius.enabled = false;
    }
    public void SetRadiusA(float value)
    {
        if (AreaA != null)
            AreaA.transform.localScale = new Vector3(SliderAreaARadius.value, SliderAreaARadius.value, SliderAreaARadius.value);
    }
    public void SetRadiusB(float value)
    {
        if (AreaB != null)
            AreaB.transform.localScale = new Vector3(SliderAreaBRadius.value, SliderAreaBRadius.value, SliderAreaBRadius.value);
    }

    private bool LoadingBusInfo = false;
    private Thread infoThread;
    private string DebugBusList = "";
    private bool WaitForCompletion = false;
    public void GetBussesInArea()
    {
        ImageBusOptions.gameObject.SetActive(false);
        TextStatus.text = "Identifying the busses in area...";
        AllRoutes.Clear();
        var detectedBusses = AreaADetector.CollidedObjects.Select(i => i.GetComponent<BusStopClass>().Id).ToArray();
        WaitForCompletion = true;
        var RouteIds = GlobalSimulationPlannerBaseClass.client.GetRelationsContainingMembers(
            wcfCon,
            detectedBusses,
            0,
            "stop");
        //List<GapslabWCFservice.MemberWCF[]> tempRoutes = new List<GapslabWCFservice.MemberWCF[]>();
        Dictionary<string, GapslabWCFservice.MemberWCF[]> tempRoutesDic = new Dictionary<string, GapslabWCFservice.MemberWCF[]>();
        //for (int i = 0; i < RouteIds.Length; i++)
        //	tempRoutesDic.Add(RouteIds[i],
        //		GlobalSimulationPlannerBaseClass.client.GetRelationMembers(wcfCon, RouteIds[i]));

        infoThread = new Thread(new ThreadStart(() =>
        {
            try
            {
                LoadingBusInfo = true;
                for (int i = 0; i < RouteIds.Length; i++)
                    tempRoutesDic.Add(RouteIds[i],
                        GlobalSimulationPlannerBaseClass.client.GetRelationMembers(wcfCon, RouteIds[i]));
            }
            catch (ThreadAbortException tae) { }
            finally
            {
                AllRoutes = tempRoutesDic;
                LoadingBusInfo = false;
            }
        }));
        if (LoadingBusInfo)
            infoThread.Abort();
        infoThread.Start();




        //var ret = "";
        //Interpolations.MyLog("Getting busses from area A...");
        //for (int i = 0; i < detectedBusses.Length; i++)
        //{
        //	ret += "Stop name: " + detectedBusses[i].Name + "\n";
        //	ret += "Located at: " + detectedBusses[i].Latitude + ", " + detectedBusses[i].Longitude + "\n";
        //	ret += "with bus numbers: [ ";
        //	for (int j = 0; j < detectedBusses[i].RouteReference.Length; j++)
        //		ret += detectedBusses[i].RouteReference[j] + " ";
        //	ret += "]\n\n";
        //}
        //DebugBusList = ret;
    }
    public bool ShowOnlyTheLastRoute = true;
    private bool LoadingBusLineInfo = false;
    private bool SignalRouteDrawThreadCompletion = false;
    private Thread RouteDrawThread;
    private List<GapslabWCFservice.OsmNodeWCF[]> RoutePathsTemp;
    private List<string> RoutePathsIds;
    public void OnInfoThreadCompleted()
    {
        TextStatus.text = "Identifying the routes...";
        Debug.LogWarning("The number of discovered bus routes involving the selected bus stops:" + AllRoutes.Count);

        if (ShowOnlyTheLastRoute)
        {
            var tobeDeleted = GameObject.FindGameObjectsWithTag("BusLine");
            tobeDeleted.For((i, g) => Object.DestroyImmediate(tobeDeleted[i]));
        }

        //// Single Threaded
        //GameObject[] routeObjects = new GameObject[AllRoutes.Count];
        //var routeRoot = GameObject.Find("Bus Lines");
        //if (routeRoot == null)
        //	routeRoot = new GameObject("Bus Lines");
        //routeObjects.For<GameObject>((i, g) =>
        //	{
        //		routeObjects[i] = new GameObject("Bus Line");
        //		routeObjects[i].tag = "BusLine";
        //		routeObjects[i].AddComponent<LineRendererExtended>();
        //		routeObjects[i].transform.parent = routeRoot.transform;
        //	});
        //int count = 0;


        //foreach (var route in AllRoutes.Keys)
        //{
        //	var line = routeObjects[count].GetComponent<LineRendererExtended>();
        //	//var members = AllRoutes[route];
        //	var points = GlobalSimulationPlannerBaseClass.client.GetRoutePath(route, 1, wcfCon);
        //	line.Points = new Vector3[points.Length];
        //	line.Points.For((i, v) => line.Points[i] = CoordinateConvertor.LatLonToVector3(points[i].lat, points[i].lon, 2));
        //	line.Points = line.Points.RemoveDuplicates();
        //	count++;
        //}

        // Multithreaded
        RoutePathsTemp = new List<GapslabWCFservice.OsmNodeWCF[]>();
        RoutePathsIds = new List<string>();

        RouteDrawThread = new Thread(new ThreadStart(() =>
        {
            try
            {
                LoadingBusLineInfo = true;
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                foreach (var route in AllRoutes.Keys)
                {

                    #region server side
                    // var membersTemp = GlobalSimulationPlannerBaseClass.client.GetRoutePath(route, 1, wcfCon);
                    // TESTING
                    var membersTemp = GlobalSimulationPlannerBaseClass.client.GetRoutePathCustom(route, 1, wcfCon);
                    RoutePathsTemp.Add(membersTemp);
                    RoutePathsIds.Add(route);
                    #endregion
                    #region Moved to server side
                    ////RoutePathsTemp.Add(
                    ////	GlobalSimulationPlannerBaseClass.client.GetRoutePath(route, 1, wcfCon)
                    ////	);
                    //var membersTemp = GlobalSimulationPlannerBaseClass.client.GetRelationMembers(wcfCon, route);
                    //List<GapslabWCFservice.OsmNodeWCF> nodesTemp = new List<GapslabWCFservice.OsmNodeWCF>();
                    //// Getting ways
                    //var mt2 = membersTemp.Where(mt => mt.Type == 1).ToList();
                    //// Exclude areas
                    //List<GapslabWCFservice.MemberWCF> tempWayList = new List<GapslabWCFservice.MemberWCF>();
                    //GapslabWCFservice.TagWCF AreaTag = new GapslabWCFservice.TagWCF() { KeyValue = new string[] { "area", "yes" } };
                    //foreach (var m in mt2)
                    //{
                    //    if (GlobalSimulationPlannerBaseClass.client
                    //        .GetWayTagsStockholm(m.ReferenceId, wcfCon)
                    //        .Where(wt => wt.KeyValue[0] == AreaTag.KeyValue[0] && wt.KeyValue[1] == AreaTag.KeyValue[1]).Count() == 0)
                    //    {
                    //        tempWayList.Add(m);
                    //    }
                    //}
                    //mt2 = tempWayList;

                    //// TODO: fix square problem
                    //var AllWaysNodes = mt2.Select(
                    //    wt => GlobalSimulationPlannerBaseClass.client
                    //        .GetWayNodes(wt.ReferenceId, wcfCon).ToArray()).ToArray();


                    //for (int workit = 0; workit < mt2.Count; workit++)
                    //{
                    //    //var currentLine = GlobalSimulationPlannerBaseClass.client
                    //    //	.GetWayNodes(mt2[workit].ReferenceId, wcfCon).ToArray();
                    //    var currentLine = AllWaysNodes[workit];

                    //    if (workit > 1)
                    //    {
                    //        if (nodesTemp[nodesTemp.Count - 1].lat == currentLine[0].lat
                    //         && nodesTemp[nodesTemp.Count - 1].lon == currentLine[0].lon)
                    //        {
                    //            nodesTemp.AddRange(currentLine);
                    //        }
                    //        else
                    //        {
                    //            var rev = new GapslabWCFservice.OsmNodeWCF[currentLine.Length];
                    //            for (int revidx = 0; revidx < rev.Length; revidx++)
                    //                rev[revidx] = currentLine[rev.Length - revidx - 1];
                    //            nodesTemp.AddRange(rev);
                    //            //Interpolations.MyLog(mt2[workit].ReferenceId + " Reverse!");
                    //        }
                    //    }
                    //    else if (workit == 1)
                    //    {
                    //        if (nodesTemp[nodesTemp.Count - 1].lat == currentLine[0].lat
                    //         && nodesTemp[nodesTemp.Count - 1].lon == currentLine[0].lon)
                    //        {
                    //            nodesTemp.AddRange(currentLine);
                    //            // Add the line normally.
                    //        }
                    //        else if (nodesTemp[0].lat == currentLine[currentLine.Length - 1].lat
                    //         && nodesTemp[0].lon == currentLine[currentLine.Length - 1].lon)
                    //        {
                    //            // Reverse the first line
                    //            var rev = new GapslabWCFservice.OsmNodeWCF[nodesTemp.Count];
                    //            for (int revidx = 0; revidx < rev.Length; revidx++)
                    //                rev[revidx] = nodesTemp[rev.Length - revidx - 1];
                    //            nodesTemp.Clear();
                    //            nodesTemp.AddRange(rev);
                    //            nodesTemp.AddRange(currentLine);
                    //        }
                    //        else
                    //        {
                    //            // Reverse the current line
                    //            var rev = new GapslabWCFservice.OsmNodeWCF[currentLine.Length];
                    //            for (int revidx = 0; revidx < rev.Length; revidx++)
                    //                rev[revidx] = currentLine[rev.Length - revidx - 1];
                    //            nodesTemp.AddRange(rev);
                    //        }
                    //    }
                    //    else
                    //        nodesTemp.AddRange(currentLine);

                    //}

                    //RoutePathsTemp.Add(nodesTemp.ToArray());
                    //RoutePathsIds.Add(route);
                    #endregion
                }
                sw.Stop();
                Interpolations.MyLog("Total time: " + (sw.ElapsedMilliseconds / 1000f));
                SignalRouteDrawThreadCompletion = true;
            }
            catch (ThreadAbortException tae) { }
            finally
            {
                //AllRoutes = tempRoutesDic;
                LoadingBusLineInfo = false;
            }
        }));
        if (LoadingBusLineInfo)
            RouteDrawThread.Abort();
        RouteDrawThread.Start();
    }
    public void OnRouteDrawThreadCompleted()
    {
        if (ShowOnlyTheLastRoute)
        {
            var tobeDeleted = GameObject.FindGameObjectsWithTag("BusLine");
            tobeDeleted.For((i, g) => Object.DestroyImmediate(tobeDeleted[i]));
        }

        routeObjects = new GameObject[AllRoutes.Count];
        var routeRoot = GameObject.Find("Bus Lines");
        if (routeRoot == null)
            routeRoot = new GameObject("Bus Lines");
        routeObjects.For<GameObject>((i, g) =>
        {
            routeObjects[i] = new GameObject("Bus Line");
            routeObjects[i].tag = "BusLine";
            routeObjects[i].AddComponent<LineRendererExtended>();
            routeObjects[i].AddComponent<RouteBaseClass>();
            routeObjects[i].transform.parent = routeRoot.transform;
        });

        var BusStops = GameObject.FindObjectsOfType<BusStopClass>();
        var go = GameObject.Find("AramGISBoundaries");
        var mb = go.GetComponent<MapBoundaries>();

        int count = 0;
        int ColorVariation = 0;
        // Single Threaded

        float debugTime = 0;
        foreach (var path in RoutePathsTemp)
        {
            var line = routeObjects[count].GetComponent<LineRendererExtended>();
            var routeClass = routeObjects[count].GetComponent<RouteBaseClass>();
            routeObjects[count].name += "|" + RoutePathsIds[count];
            routeClass.PopulateFromRelation(RoutePathsIds[count]);
            // DEBUG: Strip the path to include only the visible boundary
            //var strippedPath = path.Where(p => p.lat >= mb.minLat && p.lat <= mb.maxLat && p.lon >= mb.minLon && p.lon <= mb.maxLon).ToArray();
            //
            routeClass.Nodes = path;
            var busnodemembers = GlobalSimulationPlannerBaseClass.client
             .GetRelationMembers(wcfCon, RoutePathsIds[count])
             .Where(bn => bn.Type == 0).OrderBy(bn => bn.order).ToArray();
            var busnodesList = new List<BusStopClass>();
            //Interpolations.MyLog("You should not get any nulls.");
            foreach (var busnodemember in busnodemembers)
            {
                Interpolations.MyLog("Relation " + busnodemember.RelationId + " Reference" + busnodemember.ReferenceId);
                if (BusStops.Where(bs => bs.Id == busnodemember.ReferenceId).Count() != 0)
                    busnodesList.Add(BusStops.Where(bs => bs.Id == busnodemember.ReferenceId).First());
                else
                {
                    Debug.LogWarning("Bus id " + busnodemember.ReferenceId + " is outside the simulation boundary and won't be added to the simulation.");
                }
            }
            routeClass.BusNodes = busnodesList.ToArray();
            //var members = AllRoutes[route];
            var points = path;

            line.Points = new Vector3[points.Length];

            line.Points.For((i, v) => line.Points[i] = CoordinateConvertor.LatLonToVector3(points[i].lat, points[i].lon, GlobalSimulationPlannerBaseClass.DefaultBusRouteHeight));

            //line.Points = line.Points.RemoveDuplicates();

            count++;
        }
        TextStatus.text = "A total of " + RoutePathsTemp.Count + " Bus routes were found.";
        if (RoutePathsTemp.Count > 0)
            ImageBusOptions.gameObject.SetActive(true);
        else
            ImageBusOptions.gameObject.SetActive(false);
    }
    public void PlaceBusses()
    {
        if (string.IsNullOrEmpty(TextBusCapacityInputValue.text))
        { TextBusCapacityInputValue.text = GlobalSimulationPlannerBaseClass.DefaultNormalBusCapacity.ToString(); }
        if (string.IsNullOrEmpty(TextBusCapacityInputValue.text))
        { TextBusDistanceInputValue.text = GlobalSimulationPlannerBaseClass.DefaultNormalBusDistance.ToString(); }
        int cap;
        float dist;
        if (!int.TryParse(TextBusCapacityInputValue.text, out cap))
        {
            TextBusCapacityInputValue.text = GlobalSimulationPlannerBaseClass.DefaultNormalBusCapacity.ToString();
            int.TryParse(TextBusCapacityInputValue.text, out cap);
        }
        if (!float.TryParse(TextBusDistanceInputValue.text, out dist))
        {
            TextBusDistanceInputValue.text = GlobalSimulationPlannerBaseClass.DefaultNormalBusDistance.ToString();
            float.TryParse(TextBusDistanceInputValue.text, out dist);
        }

        var routes = routeObjects.Where(ro => !ro.GetComponent<RouteBaseClass>().OutsideOfMapBoundary)
            .Select(ro => ro.GetComponent<RouteBaseClass>()).ToArray();


        switch (SelectedBusAgentType)
        {
            case BusAgentType.RealPassengers:
                {
                    foreach (var route in routes)
                    {
                        var currentBus = ((GameObject)GameObject.Instantiate(BusModelAndBehaviour)).GetComponent<BusWithRealPassengers>();
                        currentBus.DistanceFromThePath = dist;
                        currentBus.Capacity = cap;
                        currentBus.RouteRelationId = route.Id;
                    }
                    break;
                }
            case BusAgentType.MescoPassengers:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    public void SetBusAgentType(string type)
    {
        if (type.ToLower() == "real")
            SelectedBusAgentType = BusAgentType.RealPassengers;
        else if (type.ToLower() == "mesco")
            SelectedBusAgentType = BusAgentType.MescoPassengers;
    }
}
