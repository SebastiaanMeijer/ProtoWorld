using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class RouteBaseClass : MonoBehaviour
{

    /// <summary>
    /// The relation id of the route
    /// </summary>
    public string Id;
    /// <summary>
    /// The name of the route
    /// </summary>
    public string Name;
    /// <summary>
    /// The origin of the route
    /// </summary>
    public string From;
    /// <summary>
    /// The destination of the route
    /// </summary>
    public string To;
    /// <summary>
    /// The network (ie. SL SJ etc)
    /// </summary>
    public string Network;
    /// <summary>
    /// The referencing line (ie. Bus line 52)
    /// </summary>
    public string Reference;
    /// <summary>
    /// The type of the route (ie. Bus, train, bicycle etc)
    /// </summary>
    public string Route;
    /// <summary>
    /// This holds the value 'route' for the routes.
    /// </summary>
    public string RelationType;
    public GapslabWCFservice.OsmNodeWCF[] Nodes;
    public BusStopClass[] BusNodes;
    public List<Vector3[]> DebugProjectPaths;
    public Vector3[] BusNodesClosetsPointOnLine;
    [HideInInspector]
    public static GameObject[] AllRouteObjects;
    [HideInInspector]
    public static RouteBaseClass[] AllRouteObjectsRouteBaseClass;
    [HideInInspector]
    public Vector3[] Points;
    [HideInInspector]
    public static bool lockRoute = false;
    [HideInInspector]
    public bool OutsideOfMapBoundary = false;
    public void Start()
    {

        if (AllRouteObjects == null && lockRoute != true)
        {
            lockRoute = true;
            AllRouteObjects = GameObject.FindObjectOfType<SelectAreaGUI>().routeObjects;
            AllRouteObjectsRouteBaseClass = AllRouteObjects.Select(ro => ro.GetComponent<RouteBaseClass>()).ToArray();
            lockRoute = false;
        }
        if (Points == null)
            Points = GetComponent<LineRendererExtended>().Points;

        var lineren = GetComponent<LineRendererExtended>();
        lineren.Show = false;
        lineren.Points = Points.RemoveConsecutiveDuplicates();
        lineren.line.SetVertexCount(Points.Length);
        lineren.Show = true;
        Points = lineren.Points;

        SmoothenPath(GlobalSimulationPlannerBaseClass.PathSmoothingMaximumDistance, SmoothPathType.Linear);
        
        ProjectTheBusNodesOnThePath();
    }
    public void ProjectTheBusNodesOnThePath()
    {
        // This is the list of closest projected points on the paths to the bus stops.
        List<Vector3> tempClosestPointList = new List<Vector3>();
        int debugCount = 0;
        DebugProjectPaths = new List<Vector3[]>();
        foreach (var busnode in BusNodes)
        {
            var pos = busnode.transform.position;
            pos.y = Points[0].y;
            // Make the first point to be the minimum.
            var minPoint = Points[0];
            var minDistance = Vector3.Distance(minPoint, pos);
            DebugProjectPaths.Add(new Vector3[2]);
            int index = 0;
            for (int i = 0; i < Points.Length - 1; i++)
            {
                var current = pos.ProjectToLine(Points[i], Points[i + 1]);
                var currentD = Vector3.Distance(current, pos);
                if (current.IsInRangeIgnoreY(Points[i], Points[i + 1]))
                    if (currentD < minDistance)
                    {
                        minPoint = current;
                        minDistance = currentD;
                        DebugProjectPaths[debugCount][0] = Points[i];
                        DebugProjectPaths[debugCount][1] = Points[i + 1];
                    }
            }
            tempClosestPointList.Add(minPoint);
            debugCount++;

        }
        BusNodesClosetsPointOnLine = tempClosestPointList.ToArray();
        //string s = "";
        //foreach (var n in tempClosestPointList)
        //	s += n.ToString() + "\n";
        //Interpolations.MyLog(s);
    }
    public Vector3[] ProjectTheBusNodesOnTheLane(Vector3[] lane)
    {
        // This is the list of closest projected points on the paths to the bus stops.
        List<Vector3> tempClosestPointList = new List<Vector3>();
        int debugCount = 0;
        DebugProjectPaths = new List<Vector3[]>();
        foreach (var busnode in BusNodes)
        {
            var pos = busnode.transform.position;
            pos.y = lane[0].y;
            // Make the first point to be the minimum.
            var minPoint = lane[0];
            var minDistance = Vector3.Distance(minPoint, pos);
            DebugProjectPaths.Add(new Vector3[2]);
            int index = 0;
            for (int i = 0; i < lane.Length - 1; i++)
            {
                var current = pos.ProjectToLine(lane[i], lane[i + 1]);
                var currentD = Vector3.Distance(current, pos);
                if (current.IsInRangeIgnoreY(lane[i], lane[i + 1]))
                    if (currentD < minDistance)
                    {
                        minPoint = current;
                        minDistance = currentD;
                        DebugProjectPaths[debugCount][0] = lane[i];
                        DebugProjectPaths[debugCount][1] = lane[i + 1];
                    }
            }
            tempClosestPointList.Add(minPoint);
            debugCount++;

        }
        return tempClosestPointList.ToArray();
    }
    public void PopulateFromRelation(string RelationId)
    {
        GlobalSimulationPlannerBaseClass.Init();
        var tags = GlobalSimulationPlannerBaseClass.client.GetRelationTags(RelationId, GlobalSimulationPlannerBaseClass.wcfCon);
        Id = RelationId;
        var from = tags.Where(t => t.KeyValue != null && t.KeyValue[0].ToLower() == "from").FirstOrDefault();
        var nname = tags.Where(t => t.KeyValue != null && t.KeyValue[0].ToLower() == "name").FirstOrDefault();
        var network = tags.Where(t => t.KeyValue != null && t.KeyValue[0].ToLower() == "network").FirstOrDefault();
        var reference = tags.Where(t => t.KeyValue != null && t.KeyValue[0].ToLower() == "ref").FirstOrDefault();
        var route = tags.Where(t => t.KeyValue != null && t.KeyValue[0].ToLower() == "route").FirstOrDefault();
        var to = tags.Where(t => t.KeyValue != null && t.KeyValue[0].ToLower() == "to").FirstOrDefault();
        var type = tags.Where(t => t.KeyValue != null && t.KeyValue[0].ToLower() == "type").FirstOrDefault();
        if (from != null)
            this.From = from.KeyValue[1];
        if (nname != null)
            this.Name = nname.KeyValue[1];
        if (network != null)
            this.Network = network.KeyValue[1];
        if (reference != null)
            this.Reference = reference.KeyValue[1];
        if (route != null)
            this.Route = route.KeyValue[1];
        if (to != null)
            this.To = to.KeyValue[1];
        if (type != null)
            this.RelationType = type.KeyValue[1];
    }
    /// <summary>
    /// The smoothing algorithm to be used with SmoothenPath method.
    /// </summary>
    public enum SmoothPathType
    {
        /// <summary>
        /// Linear interpolation
        /// </summary>
        Linear,
        /// <summary>
        /// Spherical interpolation
        /// </summary>
        Spherical
    };
    /// <summary>
    /// Smoothens the path for animation.
    /// </summary>
    /// <param name="Step">The maximum distance between the points</param>
    /// <param name="PathType">Smoothing method. Currently, only the linear method is implemented.</param>
    public void SmoothenPath(Vector3 Step, SmoothPathType PathType)
    {
        var lineRenderer = GetComponent<LineRendererExtended>();
        var currentPoints = lineRenderer.Points;
        List<Vector3> SmoothedPath = new List<Vector3>();
        for (int i = 0; i < currentPoints.Length - 1; i++)
        {
            SmoothedPath.Add(currentPoints[i]);
            var distanceBetweenThePoints = Vector3.Distance(currentPoints[i + 1], currentPoints[i]);
            var MaximumDistance = Step.magnitude;
            if (distanceBetweenThePoints > MaximumDistance)
            {
                var noOfSteps = distanceBetweenThePoints / MaximumDistance;
                var currentStepSize = (currentPoints[i + 1] - currentPoints[i]) / noOfSteps;
                int counter = 1;
                while (counter <= noOfSteps)
                {
                    SmoothedPath.Add(currentPoints[i] + currentStepSize * counter++);
                }
            }
        }
        lineRenderer.Show = false;
        lineRenderer.line.SetVertexCount(SmoothedPath.Count);
        lineRenderer.Points = SmoothedPath.ToArray();
        Points = GetComponent<LineRendererExtended>().Points;
        lineRenderer.Show = true;
    }
    public Vector3[] GetLane(float DistanceFromTheCenter)
    {
        bool right = DistanceFromTheCenter >= 0;
        LineMath[] lineSegments = new LineMath[Points.Length - 1];
        AramRoadMath[] laneSegments = new AramRoadMath[Points.Length - 1];
        LineMath[] ResultLane = new LineMath[Points.Length - 1];
        for (int i = 0; i < lineSegments.Length; i++)
        {
            lineSegments[i] = new LineMath(Points[i], Points[i + 1]);
            var stPerp = lineSegments[i].PerpendicularAtAPointOnLine(lineSegments[i].p1);
            var enPerp = lineSegments[i].PerpendicularAtAPointOnLine(lineSegments[i].p2);
            if (float.IsNaN(stPerp.m) || float.IsNaN(enPerp.m))
                Debug.LogError("The perpendicular line has infinity slope. TODO");
            var st = stPerp.FindPointsAt(DistanceFromTheCenter, lineSegments[i].p1);
            var en = enPerp.FindPointsAt(DistanceFromTheCenter, lineSegments[i].p2);
            if (st[0].CheckForNoNaN()
              || st[1].CheckForNoNaN()
              || en[0].CheckForNoNaN()
              || en[0].CheckForNoNaN())
                Debug.LogError("At i=" + i + " and " + (i + 1) + " we get for st and en" + "\n" +
                    st[0] + "\n" +
                    st[1] + "\n" +
                    en[0] + "\n" +
                    en[1] + "\n"
                    );
            laneSegments[i] = new AramRoadMath() 
            { 
                Line = lineSegments[i], 
                StartingPoints = st, 
                EndingPoints = en 
            };
            laneSegments[i].UpdateLogic();
        }
        for (int i = 0; i < laneSegments.Length; i++)
        {

            if (i == 0) // For the first lane segment
            {
                if (right)
                {
                    var intersection = new Vector3();
                    intersection = laneSegments[i].RightLine.Intersect(laneSegments[i + 1].RightLine);
                    if (intersection.CheckForNoNaN())
                    {
                        intersection = laneSegments[i].enR;
                        Debug.LogWarning("NaN happened on the first line segment");
                    }
                    ResultLane[i] = new LineMath(laneSegments[i].stR, intersection);
                }
                else
                {
                    var intersection = new Vector3();
                    intersection = laneSegments[i].LeftLine.Intersect(laneSegments[i + 1].LeftLine);
                    if (intersection.CheckForNoNaN())
                    {
                        intersection = laneSegments[i].enL;
                        Debug.LogWarning("NaN happened on the first line segment");
                    }
                    ResultLane[i] = new LineMath(laneSegments[i].stL, intersection);
                }
            }
            else if (i == laneSegments.Length - 1) // For the last lane segment
            {
                if (right)
                {
                    var intersection = new Vector3();
                    intersection = laneSegments[i].RightLine.Intersect(laneSegments[i - 1].RightLine);
                    if (intersection.CheckForNoNaN())
                    {
                        intersection = laneSegments[i].stR;
                        Debug.LogWarning("NaN happened on the " + i + "th index line segment");
                    }
                    ResultLane[i] = new LineMath(intersection, laneSegments[i].enR);
                }
                else
                {
                    var intersection = new Vector3();
                    intersection = laneSegments[i].LeftLine.Intersect(laneSegments[i - 1].LeftLine);
                    if (intersection.CheckForNoNaN())
                    {
                        intersection = laneSegments[i].stL;
                        Debug.LogWarning("NaN happened on the " + i + "th index line segment");
                    }
                    ResultLane[i] = new LineMath(intersection, laneSegments[i].enL);
                }
            }
            else // For the middle lane segments
            {
                if (right)
                {
                    //var intersection1 = laneSegments[i].RightLine.Intersect(laneSegments[i - 1].RightLine);
                    var intersection1 = laneSegments[i].RightLine.Intersect(ResultLane[i - 1]);
                    var intersection2 = laneSegments[i].RightLine.Intersect(laneSegments[i + 1].RightLine);

                    if (intersection1.CheckForNoNaN())
                    {
                        intersection1 = laneSegments[i - 1].RightLine.p2;
                        //Debug.LogWarning("NaN happened on the " + i + "th index line segment INTERSECTION 1");
                    }
                    if (intersection2.CheckForNoNaN())
                    {
                        intersection2 = laneSegments[i + 1].RightLine.p1;
                        //Debug.LogWarning("NaN happened on the " + i + "th index line segment INTERSECTION 2");
                    }
                    if (!intersection1.EqualsManual(ResultLane[i - 1].p2,true))
                    {
                       // Debug.LogWarning("NAAAAAAAAAAAAAAAAAAT!");
                        intersection1 = ResultLane[i - 1].p2;
                    }
                    ResultLane[i] = new LineMath(intersection1, intersection2);
                }
                else
                {
                    //var intersection1 = laneSegments[i].LeftLine.Intersect(laneSegments[i - 1].LeftLine);
                    var intersection1 = laneSegments[i].LeftLine.Intersect(ResultLane[i - 1]);
                    var intersection2 = laneSegments[i].LeftLine.Intersect(laneSegments[i + 1].LeftLine);
                    if (intersection1.CheckForNoNaN())
                    {
                        intersection1 = laneSegments[i - 1].LeftLine.p2;
                        Debug.LogWarning("NaN happened on the " + i + "th index line segment INTERSECTION 1");
                    }
                    if (intersection2.CheckForNoNaN())
                    {
                        intersection2 = laneSegments[i + 1].LeftLine.p1;
                        Debug.LogWarning("NaN happened on the " + i + "th index line segment INTERSECTION 2");
                    }
                    if (!intersection1.EqualsManual(ResultLane[i - 1].p2, true))
                    {
                        Debug.LogWarning("NAAAAAAAAAAAAAAAAAAT!");
                        intersection1 = ResultLane[i - 1].p2;
                    }
                    ResultLane[i] = new LineMath(intersection1, intersection2);
                }
            }
        }
        Vector3[] result = new Vector3[ResultLane.Length + 1];
        for (int i = 0; i < ResultLane.Length; i++)
            result[i] = ResultLane[i].p1;
        result[result.Length - 1] = ResultLane[ResultLane.Length - 1].p2;
        return result;
    }
}
