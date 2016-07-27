/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * KPI MODULE
 * Johnson Ho
 * 
 */

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