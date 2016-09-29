using UnityEngine;
using System.Collections;

public class KPIQueuePerType : MonoBehaviour
{
    public int busQueue = 0;

    private StationStatistics stationStatistics;
    public int trainQueue = 0;

    private LineStatistics lineStatistics;
    public int tramQueue = 0;

    // Use this for initialization
    void Start()
    {
        stationStatistics = GetComponentInChildren<StationStatistics>();
        lineStatistics = GetComponentInChildren<LineStatistics>();
    }

    // Update is called once per frame
    void Update()
    {
        busQueue = getBusQueue();
        trainQueue = stationStatistics.totalQueuing;
        tramQueue = lineStatistics.totalQueuing;
    }

    private int getBusQueue()
    {
        return 0;
    }
}