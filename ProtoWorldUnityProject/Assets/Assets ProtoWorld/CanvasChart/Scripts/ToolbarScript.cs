﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolbarScript : MonoBehaviour {

    private ChartController controller;

    // Icons
    public Texture iconMaximize;
    public Texture iconMinimize;
    public RawImage img;

    void Start()
    {
        controller = GetComponentInParent<ChartController>();
        img = transform.FindChild("MinimizeButton").FindChild("RawImage").GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update ()
    {
        img.texture = controller.contentPanel.activeSelf ? iconMinimize : iconMaximize;
    }

    public void ChangeChartType(int id)
    {
        if (controller == null)
            return;
        switch (id)
        {
            case 0:
                controller.chartType = UIChartTypes.Bar;
                break;
            case 1:
                //Pie
                controller.chartType = UIChartTypes.Pie;
                break;
            case 2:
                //Line
                controller.chartType = UIChartTypes.Line;
                break;
            case 3:
                //stacked
                controller.chartType = UIChartTypes.StackedArea;
                break;
            default:
                controller.chartType = UIChartTypes.ToBeDecided;
                break;
        }
    }

    public void ToggleVisibility()
    {
        controller.contentPanel.SetActive(!controller.contentPanel.activeSelf);
        ClearRenderer();
    }

    private void ClearRenderer()
    {
        //Clear the CanvasRenderer, else the axis/lines remain visible.
        Transform content = controller.transform.Find("Content");
        Transform chartView = content.Find("ChartView");
        Transform chartHolder = chartView.transform.Find("ChartHolder");

        //Clear all renderers
        CanvasRenderer[] renderers = chartHolder.gameObject.GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer canvasRenderer in renderers)
            canvasRenderer.Clear();

        //Also clear the axis
        Transform axisHolder = chartView.transform.Find("AxisHolder");
        CanvasRenderer renderer = axisHolder.gameObject.GetComponent<CanvasRenderer>();
        renderer.Clear();
    }

    public void ToggleVisibility(bool val)
    {
        controller.contentPanel.SetActive(val);
        ClearRenderer();
    }

    public void ToggleLegend()
    {
        Transform content = controller.transform.Find("Content");
        Transform legendView = content.Find("Legend");

        RectTransform contentRectTransform = content.GetComponent<RectTransform>();

        bool active = legendView.gameObject.activeSelf;
        legendView.gameObject.SetActive(!active);

        if (active) contentRectTransform.offsetMax = new Vector2(0, contentRectTransform.offsetMax.y);
        else contentRectTransform.offsetMax = new Vector2(-100, contentRectTransform.offsetMax.y);
    }
}