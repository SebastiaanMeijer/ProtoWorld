using UnityEngine;

public class ArrowUtils
{
    [System.Obsolete("Use CheckTransportationModuleExist() to check if Transportation exist.")]
    public static void CheckParent()
    {
        var transport = GameObject.Find("Transportation");
        if (transport == null)
        {
            transport = new GameObject("Transportation");
        }
        var rc = transport.GetComponent<RoutingController>();
        if (rc == null)
            transport.AddComponent<RoutingController>();
        var stations = GameObject.Find("Stations");
        if (stations == null)
        {
            stations = new GameObject("Stations");
        }
        var ss = stations.GetComponent<StationStatistics>();
        if (ss == null)
            stations.AddComponent<StationStatistics>();
        stations.transform.SetParent(transport.transform);
        var lines = GameObject.Find("TransLines");
        if (lines == null)
        {
            lines = new GameObject("TransLines");
        }
        var ls = lines.GetComponent<LineStatistics>();
        if (ls == null)
            lines.AddComponent<LineStatistics>();
        lines.transform.SetParent(transport.transform);
        var travelers = GameObject.Find("Travelers");
        if (travelers == null)
        {
            travelers = new GameObject("Travelers");
        }
        var ts = travelers.GetComponent<TravelerStatistics>();
        if (ts == null)
            travelers.AddComponent<TravelerStatistics>();
        travelers.transform.SetParent(transport.transform);
    }

}
