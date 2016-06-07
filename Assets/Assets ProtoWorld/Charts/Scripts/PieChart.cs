/*
 * 
 * PIE CHART FOR UNITY
 * PieChart.cs
 * Johnson Ho
 * USE WITH DATA HOLDER
 * 
 */

using UnityEngine;
using System;
using System.Collections.Generic;

public class PieChart : BaseChart
{

    public int mSlices = 360;
    public float mRotationAngle = 90f;
    public float mRadius = 0;

        /// <summary>
    /// Maybe an unnecessary contructor
    /// </summary>
    //public PieChart() : base(ChartTypes.Pie)
    //{

    //}

    public override void InitChart()
    {
        base.InitChart();
        chartShader = Shader.Find("Diffuse");
        ChartObject.name = "The Pie Chart";
        ChartObject.AddComponent<MeshFilter>();
        ChartObject.GetComponent<MeshFilter>().mesh.name = String.Format("Pie Chart Mesh");
        ChartObject.AddComponent<MeshRenderer>();

    }

    public override void UpdateMaxY()
    {
        // Max is always 100 %.
    }

    public override void UpdateYValues()
    {
        // Not applicable.
    }

    public override void UpdateXValues()
    {
        // Not applicable.
    }

    public override void UpdateMesh()
    {
        showAxis = false;
        ChartObject.GetComponent<MeshFilter>().mesh.Clear();

        if (!showAllData)
            return;

        if (Holder.mData.Length != Materials.Length)
            return;

        List<float> dataList = new List<float>();
        List<Material> matList = new List<Material>();
        for (int i = 0; i < Holder.mData.Length; i++)
        {
            if (visualizeData[i])
            {
                dataList.Add(Holder.mData[i]);
                matList.Add(Materials[i]);
            }
        }

        float[] data = dataList.ToArray();
        int length = data.Length;
        Vector2 size = chartSize;
        Vector3 offset = new Vector3(size.x / 2, size.y / 2);
        mRadius = Mathf.Min(offset.x, offset.y);

        // Calculate sum of data values
        float sumOfData = 0;
        foreach (float value in data)
        {
            if (value > 0)
                sumOfData += value;
        }
        if (sumOfData <= 0)
        {
            //Debug.LogWarning("PieChart: Data sum <= 0");
            return;
        }

        // Determine how many triangles in slice
        int[] slice = new int[length];
        int numOfTris = 0;
        int numOfSlices = 0;
        int countedSlices = 0;

        // Caluclate slice size 
        for (int i = 0; i < length; i++)
        {
            numOfTris = (int)((data[i] / sumOfData) * mSlices);
            if (numOfTris < 0)
                numOfTris = 0;
            slice[numOfSlices++] = numOfTris;
            countedSlices += numOfTris;
        }
        // Check that all slices are counted.. if not -> add/sub to/from biggest slice..
        int idxOfLargestSlice = 0;
        int largestSliceCount = 0;
        for (int i = 0; i < length; i++)
        {
            if (largestSliceCount < slice[i])
            {
                idxOfLargestSlice = i;
                largestSliceCount = slice[i];
            }
        }

        // Check validity for pie chart
        if (countedSlices == 0)
        {
            Debug.LogWarning("PieChart: Slices == 0");
            return;
        }

        // Adjust largest dataset to get proper slice 
        slice[idxOfLargestSlice] += mSlices - countedSlices;

        // Check validity for pie chart data
        if (slice[idxOfLargestSlice] <= 0)
        {
            Debug.LogWarning("PieChart: Largest pie <= 0");
            return;
        }

        // Init vertices and triangles arrays
        Vector3[] mVertices = new Vector3[mSlices * 3];
        Vector3[] mNormals = new Vector3[mSlices * 3];
        Vector2[] mUvs = new Vector2[mSlices * 3];
        int[] mTriangles = new int[mSlices * 3];

        ChartObject.GetComponent<MeshFilter>().mesh.Clear();

        // Roration offset (to get star point to "12 o'clock")
        float rotOffset = mRotationAngle / 360f * 2f * Mathf.PI;

        // Calc the points in circle
        float angle;
        float[] x = new float[mSlices];
        float[] y = new float[mSlices];

        for (int i = 0; i < mSlices; i++)
        {
            angle = i * 2f * Mathf.PI / mSlices;
            x[i] = (Mathf.Cos(angle + rotOffset) * mRadius) + offset.x;
            y[i] = (Mathf.Sin(angle + rotOffset) * mRadius) + offset.y;
        }

        // Generate mesh with slices (vertices and triangles)
        for (int i = 0; i < mSlices; i++)
        {
            mVertices[i * 3 + 0] = offset;
            mVertices[i * 3 + 1] = new Vector3(x[i], y[i], 0f);
            // This will ensure that last vertex = first vertex..
            mVertices[i * 3 + 2] = new Vector3(x[(i + 1) % mSlices], y[(i + 1) % mSlices], 0f);

            mNormals[i * 3 + 0] = MeshUtils.mNormal;
            mNormals[i * 3 + 1] = MeshUtils.mNormal;
            mNormals[i * 3 + 2] = MeshUtils.mNormal;

            mUvs[i * 3 + 0] = new Vector2(offset.x, offset.y);
            mUvs[i * 3 + 1] = new Vector2(x[i], y[i]);
            // This will ensure that last uv = first uv..
            mUvs[i * 3 + 2] = new Vector2(x[(i + 1) % mSlices], y[(i + 1) % mSlices]);

            mTriangles[i * 3 + 0] = i * 3 + 0;
            mTriangles[i * 3 + 1] = i * 3 + 2;
            mTriangles[i * 3 + 2] = i * 3 + 1;
        }


        // Assign verts, norms, uvs and tris to mesh and calc normals
        ChartObject.GetComponent<MeshFilter>().mesh.vertices = mVertices;
        ChartObject.GetComponent<MeshFilter>().mesh.normals = mNormals;
        ChartObject.GetComponent<MeshFilter>().mesh.uv = mUvs;
        //mesh.triangles = triangles;

        ChartObject.GetComponent<MeshFilter>().mesh.subMeshCount = length;

        int[][] subTris = new int[length][];

        countedSlices = 0;

        // Set sub meshes
        for (int i = 0; i < length; i++)
        {
            // Every triangle has three veritces..
            subTris[i] = new int[slice[i] * 3];

            // Add tris to subTris
            for (int j = 0; j < slice[i]; j++)
            {
                subTris[i][j * 3 + 0] = mTriangles[countedSlices * 3 + 0];
                subTris[i][j * 3 + 1] = mTriangles[countedSlices * 3 + 1];
                subTris[i][j * 3 + 2] = mTriangles[countedSlices * 3 + 2];

                countedSlices++;
            }
            ChartObject.GetComponent<MeshFilter>().mesh.SetTriangles(subTris[i], i);
        }
        ChartObject.GetComponent<MeshRenderer>().materials = matList.ToArray();
    }
}

