/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DATA HOLDER FOR UNITY
 * DataHolder.cs
 * Johnson Ho
 * USE WITH BASE CHART
 * 
 */

using System;
using UnityEngine;

public class DataHolder : MonoBehaviour
{

    public float[] mData;

    public Color32[] chartColors;

    public string[] dataNames;

    private void Awake()
    {
        //log.Info("Awake");

        if (mData == null || mData.Length == 0)
            mData = new float[1];
        if (chartColors == null || chartColors.Length == 0)
            chartColors = new Color32[1];
        if (dataNames == null || dataNames.Length == 0)
            dataNames = new string[1];
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckKeyDown();
        UpdateLegeneds();
        UpdateColor();
    }

    public void SetData(float[] data)
    {
        mData = data;
    }

    void UpdateLegeneds()
    {
        if (dataNames.Length < mData.Length)
        {
            string[] newNames = new string[mData.Length];
            for (int i = 0; i < newNames.Length; i++)
            {
                if (i < dataNames.Length)
                    newNames[i] = dataNames[i];
                else
                    newNames[i] = String.Format("{0}", i);
            }
            dataNames = newNames;
        }
    }

    void UpdateColor()
    {
        if (chartColors.Length < mData.Length)
        {
            Color32[] newColors = new Color32[mData.Length];
            for (int i = 0; i < newColors.Length; i++)
            {
                if (i < chartColors.Length)
                    newColors[i] = chartColors[i];
                else
                    newColors[i] = new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)255);
            }
            chartColors = newColors;
        }
        else
        {
            // Make sure that all colors is not transparent.
            for (int i = 0; i < chartColors.Length; i++)
            {
                chartColors[i].a = (byte)255;
            }
        }
    }

    void CheckKeyDown()
    {
        if (Input.GetKeyDown("a"))
        {
            //string str = "'a' pressed";
            //log.Info(str);
            int length = mData.Length;
            if (length > 0)
            {
                SetData(GenerateRandomValues(length));
            }
            else
            {
                Debug.Log("Data array length = 0");
            }
        }
    }

    float[] GenerateRandomValues(int length)
    {
        float[] targets = new float[length];
        for (int i = 0; i < length; i++)
        {
            targets[i] = UnityEngine.Random.Range(0f, 150f);
        }
        return targets;
    }
}
