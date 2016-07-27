/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DATA SERIES FOR UNITY
 * DataSeries.cs
 * Johnson Ho
 * USE WITH TIMECHART
 * 
 */

using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public class DataSeries : MonoBehaviour
{
    public float[] series = new float[0];

    private int dataIdx = -1;
    private bool dataFull = false;
    private TimeChart timeChart;

    public float LastValue
    {
        get
        {
            if (dataIdx >= 0)
                return series[dataIdx];
            else
                return 0;
        }
    }

    /// <summary>
    /// Get the value at the % of the series length.
    /// </summary>
    /// <param name="percentage"> 0.0 to 1.0 </param>
    /// <returns></returns>
    public float GetValue(float percentage)
    {
        int endIdx = series.Length - 1;
        int pIdx = (int) Math.Round(percentage * endIdx);
        int idx = (dataIdx + pIdx < endIdx) ? pIdx : (dataIdx + pIdx - endIdx);
        return series[idx];
    }

    public void UpdateDataLength()
    {
        int newLength = timeChart.numberOfSamples;
        if (newLength == series.Length)
            return;

        float[] tempData = new float[newLength];
        for (int i = 0; i < newLength; i++)
        {
            tempData[i] = (i < series.Length) ? series[i] : 0;
        }
        series = tempData;
    }

    public void Init(TimeChart c)
    {
        timeChart = c;
        series = new float[timeChart.numberOfSamples];
        // Make sure that all points start with zero.
        for (int i = 0; i < series.Length; i++)
            series[i] = 0;

        transform.localPosition = new Vector3();

        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
    }

    public float GetMax()
    {
        float[] values = (float[])series.Clone();
        return values.Max();
    }

    public void SetData(float[] data)
    {
        series = data;
        dataIdx = series.Length - 1;
    }

    public void Add(float value)
    {
        dataIdx++;
        if (dataIdx >= series.Length)
        {
            dataIdx = 0;
            dataFull = true;
        }
        series[dataIdx] = value;
        timeChart.TimeSliderPosition = 1.0f;
    }

    // 0-index is the latest value.
    public float[] GetRearrangedData()
    {
        float[] reArranged = new float[series.Length];
        int i = 0;
        for (int idx = dataIdx; idx >= 0; idx--)
        {
            reArranged[i++] = series[idx];
        }
        for (int idx = series.Length - 1; idx > dataIdx; idx--)
        {
            reArranged[i++] = series[idx];
        }
        return reArranged;
    }

    // Saving the code just in case.
    [System.Obsolete("The line looks horrible using LineRenderer, use UpdateLineMesh(Material m) instead.")]
    public void UpdateLineandMaterial(Material m)
    {
        LineRenderer line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.SetVertexCount(series.Length);
        line.material = m;
        line.SetColors(m.color, m.color);
        line.SetWidth(timeChart.lineWidth, timeChart.lineWidth);
        int xPos = series.Length - 1;
        float xScale = timeChart.chartSize.x / series.Length;
        float yScale = timeChart.chartSize.y / timeChart.maxYValue;
        for (int i = dataIdx; i >= 0; i--)
        {
            line.SetPosition(xPos, new Vector3(xPos * xScale, series[i] * yScale));
            xPos--;
        }

        if (dataFull)
        {
            for (int i = series.Length - 1; i > dataIdx; i--)
            {
                line.SetPosition(xPos, new Vector3(xPos * xScale, series[i] * yScale));
                xPos--;
            }
        }
    }

    public void UpdateMesh(Mesh m)
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        filter.mesh.Clear();
        filter.mesh = m;
    }
    public void UpdateMaterial(Material m)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = m;
    }

    public void UpdateMesh()
    {
        // GetRearrangedData make the latest sampling as the last point of the Line chart.
        // Prepare the input for making the mesh of the each Line.
        Vector3[] lines = CreateLineFromData(GetRearrangedData());

        // Generate Line Mesh in the DataSeries which contain a mesh Renderer.
        UpdateMesh(MeshUtils.GenerateLineMesh(lines, timeChart.lineWidth));

        // TODO: A method that pre-arrange yPos of floating values so that 
        //       it won't be overlapping (when there are several Lines)

        // Update the floating value tracing the last sampling of the each Line.
        // GetRearrangedData() begins with the lastest sampling.
        //UpdateFloatingValue(lines[0]);
        UpdateFloatingValue(lines, timeChart.TimeSliderPosition);
    }

    private Vector3[] CreateLineFromData(float[] data)
    {
        if (data.Length < 1)
            return null;

        Vector3[] lines = new Vector3[data.Length * 2];

        int xIdx = data.Length - 1;
        float xScale = timeChart.chartSize.x / (data.Length - 1);
        float yScale = (timeChart.maxYValue == 0) ? 0 : timeChart.chartSize.y / timeChart.maxYValue;
        int lineIdx = 0;
        for (int idx = 0; idx < data.Length - 1; idx++)
        {
            lines[lineIdx++] = new Vector3(xIdx * xScale, data[idx] * yScale);
            lines[lineIdx++] = new Vector3((xIdx - 1) * xScale, data[idx + 1] * yScale);
            xIdx--;
        }

        return lines;
    }

    public void UpdateFloatingValue(Vector3[] lines, float percentage)
    {
        // lines is rearranged starting with the last sampling at 0-index.
        // therefore 1 minus percentage...

        int idx = (int) Math.Round((1-percentage) * (lines.Length - 1));
        //print(percentage);
        //print(idx);
        UpdateFloatingValue(lines[idx], GetValue(percentage).ToString(timeChart.specifier));
    }

    // Create only when this is called.
    // The color of the text is intentionally little darker than the line/area chart color.
    public void UpdateFloatingValue(Vector3 point, string value)
    {
        if (point == null)
            return;

        if (float.IsNaN(point.x) || float.IsNaN(point.y))
            return;

        Transform trf = transform.Find("float value");
        if (trf == null)
        {
            GameObject obj = Instantiate(timeChart.axisText, new Vector3(0, 0, -1000), Quaternion.identity) as GameObject;
            obj.name = "float value";
            trf = obj.transform;
            trf.parent = transform;
        }
        TextMesh mesh = trf.GetComponent<TextMesh>();
        mesh.text = value;
        mesh.color = Color.Lerp(GetComponent<MeshRenderer>().material.color, Color.black, 0.1f);

        float halfX = timeChart.chartSize.x / 2;
        float halfY = timeChart.chartSize.y / 2;

        if (point.y > halfY)
        {
            if (point.x > halfX)
                mesh.anchor = TextAnchor.UpperRight;
            else
                mesh.anchor = TextAnchor.UpperLeft;
        }
        else
        {
            if (point.x > halfX)
                mesh.anchor = TextAnchor.LowerRight; 
            else
                mesh.anchor = TextAnchor.LowerLeft;
        }

        //mesh.transform.localPosition = new Vector3(timeChart.chartSize.x - 5, point.y);
        mesh.transform.localPosition = new Vector3(point.x, point.y);
    }
}

