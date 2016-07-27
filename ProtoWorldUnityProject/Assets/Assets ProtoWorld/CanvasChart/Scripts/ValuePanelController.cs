/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * KPI MODULE
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ValuePanelController : MonoBehaviour
{

    private ChartController controller;
    private RectTransform valuePanel;
    private ColorBlock colorBlock;

    public GameObject valueButtonPrefab;

    private static string valueChildString = "timeData";

    public float MaxValue { get; set; }
    public float MinValue { get; set; }

    // Use this for initialization
    void Start()
    {
        controller = GetComponentInParent<ChartController>();
        valuePanel = transform as RectTransform;
        colorBlock = valueButtonPrefab.GetComponent<Button>().colors;
    }

    // Update is called once per frame
    void Update()
    {
        CheckValueCount();
        UpdateValues();

    }

    private void UpdateValues()
    {
        var minmax = controller.GetMinMaxOfAll();

        for (int i = 0; i < controller.values.Length; i++)
        {
            string name = ChartUtils.NameGenerator(valueChildString, i);
            GameObject obj = valuePanel.Find(name).gameObject;
            colorBlock.disabledColor = controller.seriesColors[i];
            obj.GetComponent<Button>().colors = colorBlock;

            float value = controller.values[i];
            obj.GetComponentInChildren<Text>().text = value.ToString(controller.specifier);

            RectTransform rt = obj.transform as RectTransform;
            float yPos = (value - minmax.yMin) / (minmax.yMax - minmax.yMin) * valuePanel.rect.height;
            yPos = Mathf.Clamp(yPos, 0, valuePanel.rect.height - rt.rect.height);
            rt.localPosition = new Vector3(-5 , yPos);
        }
    }

    private void CheckValueCount()
    {
        while (valuePanel.childCount != controller.SeriesCount)
        {
            if (valuePanel.childCount > controller.SeriesCount)
            {
                string name = ChartUtils.NameGenerator(valueChildString, valuePanel.childCount - 1);
                GameObject obj = valuePanel.Find(name).gameObject;
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
            else if (valuePanel.childCount < controller.SeriesCount)
            {
                GameObject obj = Instantiate(valueButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                obj.name = ChartUtils.NameGenerator(valueChildString, valuePanel.childCount);
                obj.transform.SetParent(valuePanel);
                //obj.transform.localPosition = Vector3.zero;
            }
        }
    }



}
