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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleChartViewClick : MonoBehaviour, IPointerDownHandler
{
    private ChartController controller { get; set; }
    private RectTransform chartRect { get; set; }

    public void Start()
    {
        controller = transform.parent.parent.GetComponent<ChartController>();
        chartRect = transform as RectTransform;
        GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    public void OnPointerDown(PointerEventData data)
    {
        float rectWidth = chartRect.rect.width;
        Vector2 point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(chartRect, data.position, data.pressEventCamera, out point);
        float pos = point.x / rectWidth;
        //Debug.Log("x: " + point.x + " pos: " + pos);
        controller.UpdateValuesAndIndicator(pos);
    }

}
