/*
 * 
 * BACKGROUND PLOTTER FOR UNITY
 * BackgroundPlotter.cs
 * Johnson Ho
 * USE WITH BASE CHART
 * 
 */

using UnityEngine;

public class BackgroundPlotter : BasePlotter
{
    public Rect BackgroundRect { get; private set; }

    public override void UpdatePlot()
    {
        plotObject.SetActive(chart.showBackground);
        if (chart.showBackground)
        {
            Vector2 chartSize = chart.chartSize;
            Vector2 margins = chart.margins;

            plotObject.GetComponent<MeshRenderer>().material.color = backgroundColor;
            if (chartSize.x > 0 && chartSize.y > 0)
            {
                // This is not OOP but works for now...
                RectTransform rectTransform = chart.GetComponent<RectTransform>();
                if (rectTransform != null )
                {
                    // FileFeed update!
                    Rect rect = rectTransform.rect;
                    BackgroundRect = new Rect(-margins.x, -margins.y, rect.width, rect.height);
                }
                else
                {
                    // LiveFeed Update!
                    BackgroundRect = new Rect(-margins.x, -margins.y, chartSize.x * 1.1f - (-margins.x), chartSize.y * 1.1f - (-margins.y));
                }
                MeshUtils.UpdateRectMesh(plotObject.GetComponent<MeshFilter>().mesh, BackgroundRect);
            }
            else
            {
                Debug.LogWarning("chartSize not set!");
                return;
            }
        }
    }

    protected override void GeneratePlot()
    {
        plotObject = new GameObject("The Background");
        plotObject.layer = LayerMask.NameToLayer("UI");
        plotObject.transform.parent = this.transform;

        plotObject.transform.localPosition = new Vector3(0, 0, 0.2f);
        MeshRenderer background = plotObject.AddComponent<MeshRenderer>();
        background.material = new Material(Shader.Find("Diffuse"));

        Mesh mesh = plotObject.AddComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.name = "Chart Background";
    }
}
