/*
 * 
 * STACKED AREA CHART FOR UNITY
 * StackedAreaChart.cs
 * Johnson Ho
 * USE WITH DATA HOLDER
 * 
 */

using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class StackedAreaChart : TimeChart
{
    public override void InitChart()
    {
        base.InitChart();
        chartShader = Shader.Find("Diffuse");
        Identifier = "Area Obj";

        //MaxXValue = 0; // Start time = 0
        maxYValue = 1; // Always 100 %

    }

    protected override void UpdateData(float[] data)
    {
        float sum = data.Sum();

        Transform transf = PlotterGroup.transform;
        for (int idx = 0; idx < transf.childCount; idx++)
        {
            string name = MeshUtils.NameGenerator(Identifier, idx);
            GameObject obj = transf.Find(name).gameObject;
            DataSeries pltr = obj.GetComponent<DataSeries>();
            pltr.UpdateDataLength();
            if (idx < data.Length)
            {
                pltr.Add(data[idx] / sum);
            }
        }
        legendOrder = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            legendOrder[i] = data.Length - 1 - i;
        }
    }

    public override void UpdateYValues()
    {
        string[] ys = new string[yDivisions];
        float yJump = maxYValue / yDivisions;
        for (int i = 0; i < yDivisions; i++)
        {
            ys[i] = (maxYValue - i * yJump).ToString("#,0.%");
        }
        YValues = ys;
    }

    //public override void UpdateXValues()
    //{
    //    string[] xs = new string[xDivisions];
    //    float xJump = numberOfSamples / xDivisions * updateFrequency;
    //    for (int i = 0; i < xDivisions; i++)
    //    {
    //        xs[i] = (MaxXValue - i * xJump).ToString(specifier);
    //    }
    //    XValues = xs;
    //}

    // Max is always 100 %.
    public override void UpdateMaxY()
    { }

    // Update plotters and show if visualizeData[idx] is true;
    // Update the materials before to ensure there is a renderer attached to the child object.
    public override void UpdateMesh()
    {
        float[] yStartPos = new float[numberOfSamples];
        for (int i = 0; i < numberOfSamples; i++)
        {
            yStartPos[i] = 0;

        }
        Vector3[] lines = new Vector3[numberOfSamples * 2];




        Transform trans = PlotterGroup.transform;

        float xScale = chartSize.x / (numberOfSamples);
        float yScale = chartSize.y;

        for (int childIdx = 0; childIdx < trans.childCount; childIdx++)
        {
            string name = MeshUtils.NameGenerator(Identifier, childIdx);
            GameObject obj = trans.Find(name).gameObject;
            DataSeries pltr = obj.GetComponent<DataSeries>();

            // Sometimes the materials might have been changed during scheduledUpdate
            if (childIdx < Materials.Length)
                pltr.UpdateMaterial(Materials[childIdx]);

            // Sometimes the visualizeData might have been changed during scheduledUpdate
            if (childIdx < visualizeData.Length)
            {
                pltr.GetComponent<Renderer>().enabled = visualizeData[childIdx];
                if (!visualizeData[childIdx])
                    continue;
            }

            float[] currentSeries = pltr.GetRearrangedData();

            int xIdx = numberOfSamples;
            int lineIdx = 0;
            float xPos, yPos;
            for (int idx = 0; idx < currentSeries.Length; idx++)
            {
                xPos = (xIdx - .5f) * xScale;
                yPos = yStartPos[idx];
                lines[lineIdx++] = new Vector3(xPos, yPos);
                yPos += currentSeries[idx] * yScale;
                lines[lineIdx++] = new Vector3(xPos, yPos);
                yStartPos[idx] = yPos;
                xIdx--;
            }


            pltr.UpdateMesh(MeshUtils.GenerateLineMesh(lines, xScale));

        }
    }
}

