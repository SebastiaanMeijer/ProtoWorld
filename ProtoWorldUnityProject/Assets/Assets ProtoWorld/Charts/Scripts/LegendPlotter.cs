/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * LEGEND PLOTTER FOR UNITY
 * LegendPlotter.cs
 * Johnson Ho
 * USE WITH BASE CHART
 * 
 */

using UnityEngine;



public class LegendPlotter : BasePlotter
{
    private Rect rect;

    public Rect BackgroundRect
    {
        get { return rect; }
        private set { rect = value; }
    }

    private float width;
    private float height;
    private float legendMargin = 10;

    protected override void GeneratePlot()
    {
        plotObject = new GameObject("The Legends");
        plotObject.transform.parent = this.transform;
        plotObject.transform.localPosition = new Vector3(0, 0, 0.1f);
        plotObject.layer = LayerMask.NameToLayer("UI");

        MeshRenderer background = plotObject.AddComponent<MeshRenderer>();
        background.material = new Material(Shader.Find("Diffuse"));

        Mesh mesh = plotObject.AddComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.name = "Legend Background";
    }


    public override void UpdatePlot()
    {
        if (chart.showLegend)
        {
            // Be careful not to disable the script permanently.
            plotObject.SetActive(true);

            UpdateTextCount();
            UpdateTexts();

            Rect bgRect = GetComponentInParent<BackgroundPlotter>().BackgroundRect;
            rect = new Rect(bgRect.xMax, bgRect.yMin + chart.margins.y, width, height);
            MeshUtils.UpdateRectMesh(plotObject.GetComponent<MeshFilter>().mesh, rect);
        }
        else
        {
            plotObject.SetActive(false);
        }
    }

    protected virtual void UpdateTextCount()
    {
        UpdateTextCount(plotObject, objectString, chart.Holder.mData.Length, TextAnchor.LowerLeft);
    }

    protected virtual void UpdateTexts()
    {
        float xPos = rect.xMin + legendMargin;
        float yPos = rect.yMin;
        float maxWidth = 0;
        float lastHeight = 0;
        for (int i = chart.Holder.mData.Length - 1; i >= 0; i--)
        {

            string name = MeshUtils.NameGenerator(objectString, i);
            Transform transform = plotObject.transform.Find(name);
            transform.localPosition = new Vector3(xPos, yPos);
            TextMesh mesh = transform.gameObject.GetComponent<TextMesh>();
            int idx = i;
            if (chart.legendOrder.Length > 0)
                idx = chart.legendOrder[i];
            mesh.text = chart.LegendTexts[idx];
            mesh.color = chart.Materials[idx].color;
            mesh.fontSize = chart.legendText.GetComponent<TextMesh>().fontSize;
            lastHeight = mesh.GetComponent<Renderer>().bounds.extents.y * 2;
            yPos += lastHeight;
            if (maxWidth < mesh.GetComponent<Renderer>().bounds.extents.x)
                maxWidth = mesh.GetComponent<Renderer>().bounds.extents.x;

        }
        width = (legendMargin + maxWidth) * 2;
        height = yPos;
    }
}

