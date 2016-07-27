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
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    public Vector2 minSize;
    public Vector2 maxSize;

    private RectTransform rectTransform;
    private Vector2 currentPointerPosition;
    private Vector2 previousPointerPosition;

    void Awake()
    {
        rectTransform = transform.parent.GetComponent<RectTransform>();
        maxSize = rectTransform.sizeDelta;
    }

    public void OnPointerDown(PointerEventData data)
    {
        rectTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out previousPointerPosition);
    }

    public void OnDrag(PointerEventData data)
    {
        if (rectTransform == null)
            return;

        Vector2 sizeDelta = rectTransform.sizeDelta;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);
        Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

        sizeDelta += new Vector2(resizeValue.x, -resizeValue.y);
        //sizeDelta = new Vector2(
        //    Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
        //    Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
        //    );

        rectTransform.sizeDelta = sizeDelta;

        previousPointerPosition = currentPointerPosition;
    }
}