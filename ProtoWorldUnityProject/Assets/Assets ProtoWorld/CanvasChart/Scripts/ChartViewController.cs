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
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Attached to the chart area to draw the series.
/// Only Line chart is implemented so far.
/// </summary>
public class ChartViewController : MonoBehaviour
{
    private ChartController controller;
    private RectTransform chartHolder;
    private List<Mesh> meshes;

    public Font font;

    /// <summary>
    /// String use for naming the child for series rendering.
    /// </summary>
    private static string chartChildString = "chartRenderer";

    /// <summary>
    /// Time controller of the game.
    /// </summary>
    private TimeController timeCtrl;

    void Awake()
    {
        timeCtrl = FindObjectOfType<TimeController>();
    }

    /// <summary>
    /// Init important references.
    /// </summary>
    void Start()
    {
        controller = GetComponentInParent<ChartController>();
        chartHolder = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Update the chart based on the time-data series.
    /// </summary>
    void LateUpdate()
    {
        if (!timeCtrl.IsPaused())
        {
            // Make sure that there are enough child gameobject to draw the charts.
            CheckRendererCount();

            // Make sure that there are enough materials for the charts.
            if (controller.SeriesCount > controller.materials.Length)
                return;

            // Draw chart.
            switch (controller.chartType)
            {
                case UIChartTypes.Bar:
                    DrawBarChart();
                    break;
                case UIChartTypes.Pie:
                    break;
                case UIChartTypes.StackedArea:
                    break;
                case UIChartTypes.Line:
                    DrawLineChart();
                    break;
                default:
                    //Debug.Log("Please choose a ChartType.");
                    break;
            }
        }
    }


    /// <summary>
    /// For each series we need to attach a child to the chart object.
    /// </summary>
    private void CheckRendererCount()
    {
        while (chartHolder.childCount != controller.SeriesCount)
        {
            if (chartHolder.childCount > controller.SeriesCount)
            {
                string name = ChartUtils.NameGenerator(chartChildString, chartHolder.childCount - 1);
                GameObject obj = chartHolder.Find(name).gameObject;
                if (obj)
                {
                    // Gameobject will not be destroyed until after Update()
                    // hence it must detach with parent before Destroy()
                    obj.transform.parent = null;
                    Destroy(obj);
                }
                else
                    Debug.LogError(String.Format("{0} not found?!", name));
            }
            else if (chartHolder.childCount < controller.SeriesCount)
            {
                GameObject obj = new GameObject(ChartUtils.NameGenerator(chartChildString, chartHolder.childCount));
                obj.transform.SetParent(chartHolder.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.AddComponent<CanvasRenderer>();
            }
        }
    }
    private void DrawBarChart()
    {
        if (controller.SeriesCount < 1)
            return;

        
        float mBarHeight = chartHolder.rect.height;
        float mBarSector = chartHolder.rect.width / controller.SeriesCount;
        float mBarWidth = mBarSector * 0.67f;

        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            float x = (idx + 0.5f) * mBarSector;
            float y = controller.values[idx] / controller.GetTotalMaxValue() * mBarHeight;
            Vector3[] lines = new Vector3[] { new Vector3(x, 0), new Vector3(x, y) };
            Mesh lineMesh = ChartUtils.GenerateLineMesh(lines, mBarWidth);

            string name = ChartUtils.NameGenerator(chartChildString, idx);
            GameObject obj = chartHolder.Find(name).gameObject;
            CanvasRenderer renderer = obj.GetComponent<CanvasRenderer>();

            renderer.Clear();
            renderer.SetMaterial(controller.materials[idx], null);
            renderer.SetMesh(lineMesh);

            RectTransform rt;
            if (obj.transform.childCount > 0)
            {
                rt = obj.transform.GetChild(0) as RectTransform;

            }
            else
            {
                var go = new GameObject();
                go.transform.SetParent(obj.transform);
                
                var t = go.AddComponent<Text>();
                t.alignment = TextAnchor.MiddleCenter;
                t.color = Color.black;
                t.font = font;

                rt = go.transform as RectTransform;

            }

            rt.localPosition = new Vector3(x, -7);

            var text = obj.GetComponentInChildren<Text>();
            if (text != null)
                text.text = controller.values[idx].ToString();

        }
    }

    /// <summary>
    /// Implementation to draw line chart based on the time-data series.
    /// Only draw if the count of time-data is at least 2.
    /// </summary>
    private void DrawLineChart()
    {
        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            var dataCollection = controller.DataContainer.GetTimedDataCollection(idx);

            Vector3[] lines = ChartUtils.CreateLinesFromData(dataCollection, chartHolder, controller.GetMinMaxOfAll());
            Mesh lineMesh = ChartUtils.GenerateLineMesh(lines, 1.5f);

            string name = ChartUtils.NameGenerator(chartChildString, idx);
            GameObject obj = chartHolder.Find(name).gameObject;
            CanvasRenderer renderer = obj.GetComponent<CanvasRenderer>();

            renderer.Clear();
            renderer.SetMaterial(controller.materials[idx], null); 
            renderer.SetMesh(lineMesh);
        }
    }
}
