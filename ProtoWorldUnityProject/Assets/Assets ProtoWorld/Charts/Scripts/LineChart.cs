/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * LINE CHART FOR UNITY
 * LineChart.cs
 * Johnson Ho
 * SUPERCLASS FOR STACKED AREA CHART
 * USE WITH DATA HOLDER
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;


public class LineChart : TimeChart
{

    public override void InitChart()
    {
        base.InitChart();
        chartShader = Shader.Find("Diffuse");
        Identifier = "Line Obj";

        //MaxXValue = 0;
        TimeString = "";
        
    }

    protected override void UpdateData(float[] data)
    {
        // Update the DataSeries of each Line and add the last sampling of all Lines in a Dictionary.
        Dictionary<int, float> dataDict = new Dictionary<int, float>();
        Transform transf = PlotterGroup.transform;
        for (int idx = 0; idx < transf.childCount; idx++)
        {
            string name = MeshUtils.NameGenerator(Identifier, idx);
            GameObject obj = transf.Find(name).gameObject;
            DataSeries pltr = obj.GetComponent<DataSeries>();
            pltr.UpdateDataLength();
            // Sometimes the mData might have been changed during scheduledUpdate
            if (idx < data.Length)
            {
                pltr.Add(data[idx]);
                dataDict.Add(idx, data[idx]);
            }
        }

        // Rearranging the order of the legend due to the last sampling of each Line, from the Dictionary.
        List<KeyValuePair<int, float>> dataList = dataDict.ToList();
        dataList.Sort((firstPair, nextPair) => { return firstPair.Value.CompareTo(nextPair.Value); });
        dataList.Reverse();
        //string str = "";
        legendOrder = new int[dataList.Count];
        for (int i = dataList.Count - 1; i >= 0; i--)
        {
            legendOrder[i] = dataList[i].Key;
        }
    }


    // Update plotters and show if visualizeData[idx] is true;
    // Update the materials before to ensure there is a renderer attached to the child object.
    public override void UpdateMesh()
    {
        Transform trans = PlotterGroup.transform;
        for (int childIdx = 0; childIdx < trans.childCount; childIdx++)
        {
            string name = MeshUtils.NameGenerator(Identifier, childIdx);
            GameObject obj = trans.Find(name).gameObject;
            DataSeries pltr = obj.GetComponent<DataSeries>();

            //// GetRearrangedData make the latest sampling as the last point of the Line chart.
            //// Prepare the input for making the mesh of the each Line.
            //Vector3[] lines = CreateLineFromData(pltr.GetRearrangedData());

            //// Generate Line Mesh in the DataSeries which contain a mesh Renderer.
            //pltr.UpdateMesh(MeshUtils.GenerateLineMesh(lines, currentWidth));

            // Sometimes the materials might have been changed during scheduledUpdate
            if (childIdx < Materials.Length)
                pltr.UpdateMaterial(Materials[childIdx]);

            pltr.UpdateMesh();

            //pltr.UpdateFloatingValue(timeSliderPosition);

            //// TODO: A method that pre-arrange yPos of floating values so that 
            ////       it won't be overlapping (when there are several Lines)

            //// Update the floating value tracing the last sampling of the each Line.
            //// GetRearrangedData() begins with the lastest sampling.
            //pltr.UpdateFloatingValue(lines[0]);

            // Sometimes the visualizeData might have been changed during scheduledUpdate
            if (childIdx < visualizeData.Length)
                pltr.GetComponent<Renderer>().enabled = visualizeData[childIdx];
        }
    }
}