/*
 * 
 * AXES PLOTTER FOR UNITY
 * AxesPlotter.cs
 * Johnson Ho
 * USE WITH BASE CHART
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System;



public class AxesPlotter : BasePlotter
{
    [Range(0.5f, 2.5f)]
    public float lineWidth = 1.5f;

    private float[] YPos { get; set; }
    private float[] XPos { get; set; }
    private float AxisWidth { get; set; }
    private GameObject YAxis { get; set; }
    private GameObject XAxis { get; set; }

    public override void UpdatePlot()
    {
        if (chart.showBackground && chart.showAxis)
        {
            plotObject.SetActive(true);

            UpdateDivisionPosition();
            UpdateAxes();
            UpdateTextCount();
            UpdateTexts();
        }
        else
        {
            plotObject.SetActive(false);
        }
    }

    protected virtual void UpdateDivisionPosition()
    {
        float d = lineWidth / 2;
        Vector2 chartSize = chart.chartSize;
        YPos = new float[chart.yDivisions];
        float yJump = chartSize.y / chart.yDivisions;
        for (int i = 0; i < chart.yDivisions; i++)
        {
            YPos[i] = chartSize.y - d - i * yJump;
        }

        XPos = new float[chart.xDivisions];
        float xJump = chartSize.x / chart.xDivisions;
        for (int i = 0; i < chart.xDivisions; i++)
        {
            XPos[i] = chartSize.x - d - i * xJump;
        }
    }

    private void UpdateAxes()
    {
        Vector2 chartSize = chart.chartSize;
        float d = lineWidth / 2;
        AxisWidth = 3 * lineWidth;
        List<Vector3> lines = new List<Vector3>();
        lines.Add(new Vector3(-d, -lineWidth));
        lines.Add(new Vector3(-d, chartSize.y));
        lines.Add(new Vector3(-lineWidth, -d));
        lines.Add(new Vector3(chartSize.x, -d));

        //float yJump = chartSize.y / chart.yDivisions;
        for (int i = 0; i < chart.yDivisions; i++)
        {
            lines.Add(new Vector3(-lineWidth, YPos[i]));
            lines.Add(new Vector3(-AxisWidth, YPos[i]));
        }

        //float xJump = chartSize.x / chart.xDivisions;
        for (int i = 0; i < chart.xDivisions; i++)
        {
            lines.Add(new Vector3(XPos[i], -lineWidth));
            lines.Add(new Vector3(XPos[i], -AxisWidth));
        }

        Mesh axesMesh = plotObject.GetComponent<MeshFilter>().mesh;
        axesMesh.Clear();
        MeshUtils.UpdateLineMesh(axesMesh, lines.ToArray(), lineWidth);
        axesMesh.name = String.Format("Axis Mesh");

        plotObject.GetComponent<MeshRenderer>().material.color = axisColor;

    }

    protected virtual void UpdateTextCount()
    {
        UpdateTextCount(YAxis, objectString, YPos.Length, TextAnchor.MiddleRight);

        UpdateTextCount(XAxis, objectString, XPos.Length, TextAnchor.UpperRight);
    }

    protected virtual void UpdateTexts()
    {
        //float yJump = chart.maxYValue / chart.yDivisions;
        int fontsize = chart.axisText.GetComponent<TextMesh>().fontSize - chart.yDivisions;
        float xMargin = 0;
        for (int i = 0; i < YPos.Length; i++)
        {
            String name = MeshUtils.NameGenerator(objectString, i);
            Transform yt = YAxis.transform.Find(name);
            yt.localPosition = new Vector3(-AxisWidth * 1.1f, YPos[i]);
            TextMesh mesh = yt.gameObject.GetComponent<TextMesh>();
            mesh.text = chart.YValues[i];
            //mesh.text = (chart.maxYValue - i * yJump).ToString(chart.specifier);
            mesh.color = axisColor;
            mesh.fontSize = fontsize;
            if (xMargin < mesh.GetComponent<Renderer>().bounds.extents.x)
                xMargin = mesh.GetComponent<Renderer>().bounds.extents.x;
        }
        //print(xMargin);
        chart.margins.x = (xMargin * 2 + AxisWidth + lineWidth) * 1.1f;


        //float xJump = chart.maxXValue / chart.xDivisions;
        //fontsize = chart.axisText.GetComponent<TextMesh>().fontSize - chart.xDivisions;
        float yMargin = 0;
        for (int i = 0; i < XPos.Length; i++)
        {
            String name = MeshUtils.NameGenerator(objectString, i);
            Transform xt = XAxis.transform.Find(name);
            xt.localPosition = new Vector3(XPos[i], -AxisWidth * 1.1f);
            TextMesh mesh = xt.gameObject.GetComponent<TextMesh>();
            mesh.text = chart.XValues[i];
            
            //mesh.text = (chart.maxXValue - i * xJump).ToString(chart.specifier);
            mesh.color = axisColor;
            mesh.fontSize = fontsize;
            if (yMargin < mesh.GetComponent<Renderer>().bounds.extents.y)
                yMargin = mesh.GetComponent<Renderer>().bounds.extents.y;
        }
        //print(yMargin);
        chart.margins.y = (yMargin * 2 + AxisWidth + lineWidth) * 1.1f;
    }

    protected override void GeneratePlot()
    {
        plotObject = new GameObject("The Axes");
        plotObject.transform.parent = this.transform;
        plotObject.transform.localPosition = new Vector3(0, 0, -0.1f);
        plotObject.AddComponent<MeshFilter>();

        MeshRenderer axis = plotObject.AddComponent<MeshRenderer>();
        axis.material = new Material(Shader.Find("Diffuse"));

        YAxis = new GameObject("Y Values");
        YAxis.transform.parent = this.transform;
        YAxis.transform.localPosition = new Vector3(0, 0, -0.1f);
        YAxis.layer = LayerMask.NameToLayer("UI");

        XAxis = new GameObject("X Values");
        XAxis.transform.parent = this.transform;
        XAxis.transform.localPosition = new Vector3(0, 0, -0.1f);
        XAxis.layer = LayerMask.NameToLayer("UI");

    }
}

