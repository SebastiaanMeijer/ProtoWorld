using UnityEngine;
using System;

public abstract class TimeChart : BaseChart
{
    public float lineWidth = 1.5f;
    protected string Identifier { get; set; }
    public GameObject PlotterGroup { get; set; }
    public float TimeSliderPosition { get; set; }

    public override void InitChart()
    {
        base.InitChart();
        PlotterGroup = ChartObject.transform.Find("MeshGroup").gameObject;

        if (PlotterGroup == null)
            Debug.LogError("Not fucking work!");
    }

    protected void AddPlotter(int index)
    {
        GameObject obj = new GameObject(MeshUtils.NameGenerator(Identifier, index));
        obj.layer = LayerMask.NameToLayer("UI");
        obj.transform.SetParent(PlotterGroup.transform);
        DataSeries ds = obj.AddComponent<DataSeries>();
        ds.Init(this);
    }

    protected void RemovePlotter(int index)
    {
        string name = MeshUtils.NameGenerator(Identifier, index);
        GameObject obj = PlotterGroup.transform.Find(name).gameObject;
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

    public override void UpdateData()
    {
        // First update so that the number of children is coherrent with the number of lines.
        UpdateChildCount();
        UpdateData(Holder.mData);
    }

    public void SetData(int index, float[] data)
    {
        string name = MeshUtils.NameGenerator(Identifier, index);
        GameObject obj = PlotterGroup.transform.Find(name).gameObject;
        DataSeries ds = obj.GetComponent<DataSeries>();
        if (ds != null)
        {
            ds.SetData(data);
        }
    }
    
    public void SetDataSeriesCount(int count)
    {
        Transform transf = PlotterGroup.transform;
        while (count != transf.childCount)
        {
            //Debug.Log(transform.childCount);
            if (count > transf.childCount)
            {
                AddPlotter(transf.childCount);
            }
            else if (count < transf.childCount)
            {
                RemovePlotter(transf.childCount - 1);
            }
        }
    }

    protected void UpdateChildCount()
    {
        SetDataSeriesCount(Holder.mData.Length);
    }

    protected abstract void UpdateData(float[] data);

    // Get the maxValue of all DataSeries(not just the max of the latest sampling).
    public override void UpdateMaxY()
    {
        Transform transf = PlotterGroup.transform;
        float tempMax = 0;
        for (int idx = 0; idx < transf.childCount; idx++)
        {
            string name = MeshUtils.NameGenerator(Identifier, idx);
            GameObject obj = transf.Find(name).gameObject;
            DataSeries pltr = obj.GetComponent<DataSeries>();
            float lineMax = pltr.GetMax();
            if (lineMax > tempMax)
                tempMax = lineMax;
        }
        if (yValueCanOnlyIncrease)
            maxYValue = (tempMax > maxYValue) ? tempMax : maxYValue;
        else
            maxYValue = (tempMax == 0) ? maxYValue : tempMax;
    }

    // ElapsedTime is based on the UpdateFrequency.
    public override void UpdateMaxXWithTime()
    {
        MaxXValue = ElapsedTime;
    }

    public override void UpdateXValues(float min, float max)
    {
        string[] xs = new string[xDivisions];
        float xJump = (max-min) / xDivisions;
        for (int i = 0; i < xDivisions; i++)
        {
            xs[i] = (max - i * xJump).ToString(specifier);
        }
        XValues = xs;
    }

    //  X-axis is updated based on elapsed time.
    // Or the XVaues
    public override void UpdateXValues()
    {
        // Might have some unwanted effect !!!!
        if (TimeString.Length > 0)
        {
            xDivisions = 1;
            XValues = new string[] { TimeString };
        }
        else
        {
            float max = ElapsedTime;
            float min = ElapsedTime - numberOfSamples * updateFrequency;
            UpdateXValues(min, max);

            //string[] xs = new string[xDivisions];
            //float xJump = numberOfSamples / xDivisions * updateFrequency;
            //for (int i = 0; i < xDivisions; i++)
            //{
            //    xs[i] = (MaxXValue - i * xJump).ToString(specifier);
            //}
            //XValues = xs;
        }
    }
}