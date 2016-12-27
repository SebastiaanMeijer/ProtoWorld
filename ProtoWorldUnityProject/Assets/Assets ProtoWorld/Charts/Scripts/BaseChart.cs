/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * BASE CHART FOR UNITY
 * BaseChart.cs
 * Johnson Ho
 * SUPERCLASS FOR ALL CHARTS
 * USE WITH DATA HOLDER
 * 
 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ChartTypes
{
    Base,
    Camera,
    Bar,
    Pie,
    Line,
    StackedArea,
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(DataHolder))]
public abstract class BaseChart : MonoBehaviour
{
    //public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public Vector3 position = new Vector3(-40, -45, 120);
    public Vector2 chartSize = new Vector2(100, 100);

    [HideInInspector]
    public Vector2 origo = new Vector2();
    [HideInInspector]
    public Vector4 margins = new Vector4(10, 10, 10, 10);

    public bool showAllData = true;
    public bool showBackground = true;
    public bool showAxis = false;
    public bool showLegend = false;

    public Shader chartShader { get; set; }
    public GameObject ChartObject { get; set; }
    public BackgroundPlotter background { get; set; }
    public AxesPlotter axesPlotter { get; set; }
    public LegendPlotter legends { get; set; }

    [HideInInspector]
    public bool[] visualizeData; // Not used for now.
    [HideInInspector]
    public int[] legendOrder; // Not used for now.

    public float maxYValue = 100; // User can set the initial (preferred MaxY)
    public bool yValueCanOnlyIncrease = false; // USer can set whether MaxY can only Increase or AutoUpdate

    [Range(1, 20)]
    public int xDivisions = 2;

    [Range(1, 20)]
    public int yDivisions = 2;

    public DataHolder Holder { get; set; }
    public string[] YValues { get; set; }
    public float MaxXValue { get; set; }
    public string[] XValues { get; set; }
    public Material[] Materials { get; set; }
    public string[] LegendTexts { get; set; }

    public GameObject axisText; // User has to put a TextPrefab in Inspector.
    public GameObject legendText; // User has to put a TextPrefab in Inspector.
    public string specifier = "#,0.0"; // How to format the X- and Y-values.

    // How many samples to show?
    [Range(10, 600)]
    public int numberOfSamples = 60;
    //// How often do we update add a new sample (second)?
    [Range(0, 10)]
    public float updateFrequency = 0.1f;
    public float ElapsedTime { get; set; }

	private WaitForSeconds waitForUpdate;

    //// Log related variables
    //public static int LinechartCounter = 0;
    //public int MyChartIdx { get; protected set; }
    //public ChartTypes MyChartType { get; set; }

    public string TimeString { get; set; }

    private float startTime = -1;
    public float StartTime
    {
        get { return startTime; }
        set { startTime = (startTime < 0) ? value : startTime; }
    }
    public float EndTime { get; set; }

    public IFeedable currentFeed { get; set; }
    public LiveFeed liveFeed { get; set; }
    public FileFeed fileFeed { get; set; }

    void Awake()
    {
        liveFeed = new LiveFeed(this);
        fileFeed = new FileFeed(this);
    }

    void Start()
    {
        ElapsedTime = 0;

		waitForUpdate = new WaitForSeconds(updateFrequency);

        InitChart();
        InitBackground();

        if (gameObject.GetComponent<Camera>() != null)
            ToLiveFeed();
        else if (GetComponent<RectTransform>() != null)
            ToFileFeed();
        if (currentFeed != null)
            StartCoroutine(ScheduledUpdate());
        else
            Debug.LogError("Couldn't set up a currentFeed, missing Camera or RectTransform.");
    }

    public IEnumerator ScheduledUpdate()
    {
        float start;
        while (true)
        {
            start = Time.time;
            currentFeed.Update();
            yield return waitForUpdate;
            ElapsedTime += Time.time-start;
        }
    }

    public void ToLiveFeed()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        if (camera != null)
        {
            axisText.GetComponent<TextMesh>().characterSize = 1;
            AttachToCamera(camera);
            currentFeed = liveFeed;
        }
    }

    public void ToFileFeed()
    {
        RectTransform rectTrans = GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            axisText.GetComponent<TextMesh>().characterSize = 1.7f;
            updateFrequency = 0;
            AttachToCanvas(rectTrans);
            currentFeed = fileFeed;
        }
    }

    public virtual void InitChart()
    {
        Holder = gameObject.GetComponent("DataHolder") as DataHolder;
        if (Holder == null)
        {
            Debug.LogError("no DataHolder?!");
            return;
        }

        ChartObject = transform.Find("ChartObject").gameObject;

        if (ChartObject == null)
            Debug.LogError("No ChartObject found! Must have ChartObject as child with MeshGroup as GrandChild");
    }

    public void AttachToCanvas(RectTransform rectTrans)
    {

        chartSize.x = rectTrans.rect.width - margins.x - margins.z;
        chartSize.y = rectTrans.rect.height - margins.y - margins.w;
    }

    public void AttachToCamera(Camera camera)
    {
        CameraHelper.SetCameraPosition(camera);

        ChartObject.transform.SetParent(camera.transform);
        ChartObject.transform.localPosition = position;
    }

    public void InitBackground()
    {
        GameObject obj = new GameObject("Background & Axes");
        obj.transform.parent = ChartObject.transform;
        obj.transform.localPosition = new Vector3(0, 0, -.1f);

        background = obj.AddComponent<BackgroundPlotter>();
        background.Init(this);
        axesPlotter = obj.AddComponent<AxesPlotter>();
        axesPlotter.Init(this);
        legends = obj.AddComponent<LegendPlotter>();
        legends.Init(this);
    }

    public void UpdateBackground()
    {
        background.UpdatePlot();
        axesPlotter.UpdatePlot();
        legends.UpdatePlot();
    }

    public void UpdatePosition()
    {
        RectTransform rectTrans = GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            chartSize.x = rectTrans.rect.width - margins.x - margins.z;
            chartSize.y = rectTrans.rect.height - margins.y - margins.w;
            ChartObject.transform.localPosition = new Vector3(margins.x, margins.y);
        }
        else if (GetComponent<Camera>() != null)
        {
            ChartObject.transform.localPosition = position;
        }

    }

    public virtual void UpdateNames()
    {
        LegendTexts = (string[])Holder.dataNames.Clone();
    }

    public virtual void UpdateData()
    {
        // Left empty for BarChart and PieChart as they don't have to do any update on Data.
        //log.Info("BaseChart UpdateData");
    }

    public virtual void UpdateMaxXWithTime()
    {
        // Left empty for BarChart and PieChart as they don't have to do any update.
    }

    public virtual void UpdateXValues(float min, float max)
    {
        // BarChart and PieChart is not applicable.
    }

    // Left for subclasses to implement
    public abstract void UpdateXValues();

    public abstract void UpdateMesh();

    //Y-axis values are updated according to the number of division
    public virtual void UpdateYValues()
    {
        string[] ys = new string[yDivisions];
        float yJump = maxYValue / yDivisions;
        for (int i = 0; i < yDivisions; i++)
        {
            ys[i] = (maxYValue - i * yJump).ToString(specifier);
        }
        YValues = ys;
    }

    public virtual void UpdateMaxY()
    {
        maxYValue = 0;
        foreach (float value in Holder.mData)
        {
            if (value > maxYValue)
                maxYValue = value;
        }
    }

    public void UpdateVisualArrays()
    {
        if (Holder.mData.Length == 0)
            return;

        // Create a zero length array so it will activate the next If-statement.
        if (visualizeData == null)
            visualizeData = new bool[0];

        if (visualizeData.Length != Holder.mData.Length)
        {
            bool[] newBools = new bool[Holder.mData.Length];

            for (int i = 0; i < newBools.Length; i++)
            {
                if (i < visualizeData.Length)
                    newBools[i] = visualizeData[i];
                else
                    newBools[i] = true;
            }
            visualizeData = newBools;
        }
    }

    // The subclass will call this to update the material array.
    public virtual void UpdateMaterials()
    {
        Color32[] colors = Holder.chartColors;
        if (colors.Length == 0)
            return;

        if (Materials == null)
            Materials = new Material[0];

        if (Materials.Length != colors.Length)
        {
            Material[] newMaterials = new Material[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                if (i < Materials.Length)
                    newMaterials[i] = Materials[i];
                else
                {
                    newMaterials[i] = new Material(Shader.Find("Diffuse"));
                    newMaterials[i].color = colors[i];
                }
            }
            Materials = newMaterials;
        }
        else
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Materials[i].color = colors[i];
            }
        }
    }
}


