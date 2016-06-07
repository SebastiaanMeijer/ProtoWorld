using UnityEngine;
using System;

public class LiveFeed : IFeedable
{
    private readonly BaseChart chart;

    public LiveFeed(BaseChart chart)
    {
        this.chart = chart;
    }

    public void Update()
    {
        //if (!chart.isLiveFeed)
        //{
        //    ToFileFeed();
        //    return;
        //}

        // Updates based on user input at DataHolder
        chart.UpdateVisualArrays();
        chart.UpdateNames();
        chart.UpdateMaterials();

        // Live feed from DataHolder
        chart.UpdateData();

        // Other updates based on user input at BaseChart
        chart.UpdateMaxY();
        //chart.UpdateMaxXWithTime();
        chart.UpdateXValues();
        chart.UpdateYValues();
        chart.UpdateMesh();
        chart.UpdateBackground();
        chart.UpdatePosition(); // Update due to change to position in BaseChart.
    }

    public void ToFileFeed()
    {
        chart.ToFileFeed();
    }

    public void ToLiveFeed()
    {
        Debug.Log("Can't switch to same feed");
    }

    public void SwitchFeed()
    {
        ToFileFeed();
    }
}