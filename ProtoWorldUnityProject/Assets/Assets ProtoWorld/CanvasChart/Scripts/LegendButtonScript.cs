/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * KPI Module
 * 
 * Antony Löbker
 */
using UnityEngine;
using UnityEngine.UI;

public class LegendButtonScript : MonoBehaviour
{
    private ChartController controller;
    private Text text;
    private Image image;

    [RangeAttribute(0, 1)]
    public float hiddenAlpha;

    // Use this for initialization
    void Start()
    {
        controller = transform.parent.parent.parent.GetComponent<ChartController>();
        if(controller == null)
            controller = transform.parent.parent.GetComponent<ChartController>();
        text = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (controller.isSeriesHidden(text.text))
        {
            Color orgColor = image.color;
            orgColor.a = hiddenAlpha;
            image.color = orgColor;
        }

        //larger button should show more info
        RectTransform rt = GetComponent<RectTransform>();
        if (rt.sizeDelta.y < 30)
            text.verticalOverflow = VerticalWrapMode.Truncate;
        else
            text.verticalOverflow = VerticalWrapMode.Overflow;
    }

    public void ToggleVisibility()
    {
        //Debug.Log("Toggled visibility for " + text.text);
        if (controller.isSeriesHidden(text.text))
            controller.seriesHidden.Remove(text.text);
        else
            controller.seriesHidden.Add(text.text);
    }
}