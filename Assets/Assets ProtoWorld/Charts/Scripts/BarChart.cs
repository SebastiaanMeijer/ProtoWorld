/*
 * 
 * BAR CHART FOR UNITY
 * BarChart.cs
 * Johnson Ho
 * USE WITH DATA HOLDER
 * 
 */

using UnityEngine;
using System;

public class BarChart : BaseChart
{
    public string Name { get; set; }

    protected float mBarWidth = .2f;
    protected float mBarSpacing = .1f;
    protected float mBarHeight = .5f;
    protected float mBarSector;
    protected Vector3[] bars;

    /// <summary>
    /// Maybe an unnecessary contructor
    /// </summary>
    //public BarChart() : base(ChartTypes.Bar)
    //{

    //}

    public override void InitChart()
    {
        base.InitChart();
        chartShader = Shader.Find("Diffuse");
        ChartObject.name = "The Bar Chart";
        ChartObject.AddComponent<MeshFilter>();
        ChartObject.GetComponent<MeshFilter>().mesh.name = String.Format("Bar Chart Mesh");
        ChartObject.AddComponent<MeshRenderer>();

    }

    public override void UpdateXValues()
    {
        string[] xs = new string[xDivisions];
        xDivisions = Holder.dataNames.Length;
        for (int i = 0; i < xDivisions; i++)
        {
            xs[i] = Holder.dataNames[i];
        }
        XValues = xs;
    }

    public override void UpdateMesh()
    {
        //base.UpdateChart();
        ChartObject.GetComponent<MeshFilter>().mesh.Clear();

        if (!showAllData)
            return;

        if (maxYValue == 0)
        {
            //Debug.LogWarning("nothing to plot");
            return;
        }

        float[] data = Holder.mData;
        int length = data.Length;

        mBarHeight = chartSize.y;
        mBarSector = chartSize.x / length;
        mBarWidth = mBarSector * 0.67f;

        bars = new Vector3[length * 2];

        for (int i = 0; i < length; i++)
        {
            if (data[i] == 0)
                continue;
            float x = (i + 0.5f) * mBarSector;
            bars[i * 2] = new Vector3(x, data[i] / maxYValue * mBarHeight);
            bars[i * 2 + 1] = new Vector3(x, 0);
        }

        Mesh tempMesh = MeshUtils.GenerateLineMesh(bars, mBarWidth);
        ChartObject.GetComponent<MeshFilter>().mesh.subMeshCount = length;
        ChartObject.GetComponent<MeshFilter>().mesh.vertices = tempMesh.vertices;
        ChartObject.GetComponent<MeshFilter>().mesh.normals = tempMesh.normals;
        ChartObject.GetComponent<MeshFilter>().mesh.uv = tempMesh.uv;

        int triIdx = 0;

        for (int i = 0; i < length; i++)
        {
            int[] mTrias = new int[6];

            mTrias[0] = tempMesh.triangles[triIdx++];
            mTrias[1] = tempMesh.triangles[triIdx++];
            mTrias[2] = tempMesh.triangles[triIdx++];
            mTrias[3] = tempMesh.triangles[triIdx++];
            mTrias[4] = tempMesh.triangles[triIdx++];
            mTrias[5] = tempMesh.triangles[triIdx++];

            if (visualizeData[i])
                ChartObject.GetComponent<MeshFilter>().mesh.SetTriangles(mTrias, i);

        }

        ChartObject.GetComponent<MeshRenderer>().materials = Materials;

        Destroy(tempMesh);
    }
}

