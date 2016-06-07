using UnityEngine;
using System;

public class FileFeed : IFeedable
{
    private readonly BaseChart chart;

    public FileFeed(BaseChart chart)
    {
        this.chart = chart;
    }

    public void Update()
    {
        //if (chart.isLiveFeed)
        //{
        //    ToLiveFeed();
        //    return;
        //}

        // Updates based on user input at DataHolder
        chart.UpdateVisualArrays();
        chart.UpdateNames();
        chart.UpdateMaterials();

        // Other updates based on user input at BaseChart
        //chart.UpdateMaxXWithTime();
        chart.UpdateMaxY();
        chart.UpdateXValues(chart.StartTime, chart.EndTime);
        chart.UpdateYValues();
        chart.UpdateMesh();
        chart.UpdateBackground();
        chart.UpdatePosition(); // Update due to margins
    }

    public void ToFileFeed()
    {
        Debug.Log("Can't switch to same feed");
    }

    public void ToLiveFeed()
    {
        chart.ToLiveFeed();
    }

    public void SwitchFeed()
    {
        ToLiveFeed();
    }
}