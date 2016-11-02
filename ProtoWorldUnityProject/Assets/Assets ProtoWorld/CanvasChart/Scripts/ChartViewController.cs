/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
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
                DrawPieChart();
                break;
            case UIChartTypes.StackedArea:
                DrawStackedLineChart();
                break;
            case UIChartTypes.Line:
                DrawLineChart();
                break;
            default:
                //Debug.Log("Please choose a ChartType.");
                break;
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
        float mBarSector = chartHolder.rect.width/controller.SeriesCount;
        float mBarWidth = mBarSector*0.67f;

        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            float x = (idx + 0.5f)*mBarSector;
            float y = controller.values[idx]/controller.GetTotalMaxValue()*mBarHeight;
            Vector3[] lines = new Vector3[] {new Vector3(x, 0), new Vector3(x, y)};
            Mesh lineMesh = ChartUtils.GenerateLineMesh(lines, mBarWidth);

            string name = ChartUtils.NameGenerator(chartChildString, idx);
            GameObject obj = chartHolder.Find(name).gameObject;
            CanvasRenderer renderer = obj.GetComponent<CanvasRenderer>();

            renderer.Clear();
            if (!controller.isSeriesHidden(idx))
            {
                renderer.SetMaterial(controller.materials[idx], null);
                renderer.SetMesh(lineMesh);
            }

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

    private void DrawPieChart()
    {
        float total = 0;
        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            if (controller.isSeriesHidden(idx))
                continue;
            total += controller.values[idx];
        }
        if (total > 0.0001f)
        {
            float startangle = 0;
            float width = chartHolder.rect.width, height = chartHolder.rect.height;

            for (int idx = 0; idx < controller.SeriesCount; idx++)
            {
                string name = ChartUtils.NameGenerator(chartChildString, idx);
                GameObject obj = chartHolder.Find(name).gameObject;
                CanvasRenderer renderer = obj.GetComponent<CanvasRenderer>();

                renderer.Clear();

                if (controller.isSeriesHidden(idx))
                    continue;

                float part = controller.values[idx] / total;

                Vector2 center = new Vector2(width / 2f, height / 2f);
                float radius = Mathf.Min(width, height) / 2f;
                float angle = part * Mathf.PI * 2f;
                Mesh pieMesh = ChartUtils.CreatePieSectorMesh(center, radius, startangle, angle);
                startangle += angle;

                renderer.SetMaterial(controller.materials[idx], null);
                renderer.SetMesh(pieMesh);
            }
        }
    }

    /// <summary>
    /// Implementation to draw line chart based on the time-data series.
    /// Only draw if the count of time-data is at least 2.
    /// </summary>
    private void DrawLineChart()
    {
        Rect bounds = controller.GetMinMaxOfAll();

        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            var dataCollection = controller.DataContainer.GetTimedDataCollection(idx);

            Vector3[] lines = ChartUtils.CreateLinesFromData(dataCollection, chartHolder, bounds);
            Mesh lineMesh = ChartUtils.GenerateLineMesh(lines, 1.5f);

            //Debug.Log("Line | " + toString(lines));
            
            string name = ChartUtils.NameGenerator(chartChildString, idx);

            GameObject obj = chartHolder.Find(name).gameObject;
            CanvasRenderer renderer = obj.GetComponent<CanvasRenderer>();

            renderer.Clear();
            if (!controller.isSeriesHidden(idx))
            {
                renderer.SetMaterial(controller.materials[idx], null);
                renderer.SetMesh(lineMesh);
            }
        }
    }

    private void DrawStackedLineChart()
    {
        Vector3[] baselines = new Vector3[0];
        Rect bounds = controller.GetMinMaxOfAll();

        //define the baseline
        Vector3[] oldlines = new Vector3[2];
        oldlines[0] = new Vector3(0, 0, 0);
        oldlines[1] = new Vector3(chartHolder.rect.width, 0, 0);

        for (int idx = controller.SeriesCount - 1; idx >= 0; idx--)
        {
            string name = ChartUtils.NameGenerator(chartChildString, idx);
            GameObject obj = chartHolder.Find(name).gameObject;
            CanvasRenderer renderer = obj.GetComponent<CanvasRenderer>();

            renderer.Clear();

            if (controller.isSeriesHidden(idx))
                continue;

            List<TimedData> dataCollection = controller.DataContainer.GetTimedDataCollection(idx);

            //Vector3[] lines = ChartUtils.CreateLinesFromData(dataCollection, chartHolder, bounds);
            Vector3[] lines = ChartUtils.CreateLineFromData(dataCollection, chartHolder, bounds);
            if (baselines.Length == 0)
            {
                baselines = lines;
            }
            else
            {
                for (int i = 0; i < baselines.Length; i++)
                {
                    baselines[i] += new Vector3(0, lines[i].y, 0);
                }
                lines = (Vector3[]) baselines.Clone();
            }
            //Mesh lineMesh = ChartUtils.GenerateLineMesh(lines, 1);
            Debug.Log("Stacked "+idx+" | " + toString(lines));
            Debug.Log("Stacked old "+idx+" | " + toString(oldlines));
            //"(" + x.x + "," + x.y + "), "
            Mesh lineMesh = ChartUtils.GenerateStackedLineMesh(lines, oldlines);

            renderer.SetMaterial(controller.materials[idx], null);
            renderer.SetMesh(lineMesh);

            oldlines = (Vector3[]) lines.Clone();
        }
    }

    private string toString(Vector3[] points) {
        string outstring = "Points: ";
        foreach(Vector3 p in points) {
            outstring += "(" + p.x.ToString("f1") + "," + p.y.ToString("f1") + "), ";
        }
        return outstring;
    }
}