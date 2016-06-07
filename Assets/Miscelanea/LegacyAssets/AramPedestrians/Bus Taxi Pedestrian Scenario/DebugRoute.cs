using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRendererExtended))]
public class DebugRoute : MonoBehaviour
{
    public string route;
    public GapslabWCFservice.OsmNodeWCF[] nodes;
    public Vector3[] points;
    int nodesToDraw = 2;
    LineRendererExtended line;
    // Use this for initialization
    void Start()
    {
        line = GetComponent<LineRendererExtended>();
        Interpolations.MyLog("HEre...");
        GlobalSimulationPlannerBaseClass.Init();
        nodes = GlobalSimulationPlannerBaseClass.client.GetRoutePath(route, 1, GlobalSimulationPlannerBaseClass.wcfCon);
        Interpolations.MyLog("# of NODES IN TEST DEBUG: " + nodes.Length);
        if (nodesToDraw < 2)
            nodesToDraw = 2;
        points = new Vector3[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
            points[i] = CoordinateConvertor.LatLonToVector3(nodes[i].lat, nodes[i].lon, 2);
        points = points.RemoveConsecutiveDuplicates();
        nodesToDraw = Mathf.Clamp(nodesToDraw, 2, nodes.Length);
        line.Points = new Vector3[nodesToDraw];
        System.Array.Copy(points, line.Points, nodesToDraw);

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Equals))
        {
            nodesToDraw++;
            nodesToDraw = Mathf.Clamp(nodesToDraw, 2, nodes.Length);
            line.Points = new Vector3[nodesToDraw];
            line.line.SetVertexCount(nodesToDraw);
            System.Array.Copy(points, line.Points, nodesToDraw);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus))
        {
            nodesToDraw--;
            nodesToDraw = Mathf.Clamp(nodesToDraw, 2, nodes.Length);
            line.Points = new Vector3[nodesToDraw];
            line.line.SetVertexCount(nodesToDraw);
            System.Array.Copy(points, line.Points, nodesToDraw);
        }
    }
}
