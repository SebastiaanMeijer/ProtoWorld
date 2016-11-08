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

public class EventButtonPanelController : MonoBehaviour {

    public GameObject prefabEventButton;

    private float buttonSpacing = 2;

    private float buttonPosition;

    private float buttonHeight;

    private float parentHeight;

    private int eventCounts;

    public GameObject eventTextObject;

    private UnityEngine.UI.Text eventText;

    private string eventlegendString;

    // Use this for initialization
    void Awake ()
    {
        if (prefabEventButton == null)
        {
            Debug.LogError("prefabEventButton missing.");
        }
        if (eventTextObject == null)
        {
            eventTextObject = GameObject.Find("EventLegend");
        }
        buttonPosition = -buttonSpacing;
        buttonHeight = prefabEventButton.GetComponent<RectTransform>().rect.height;

        eventCounts = 0;
        eventText = eventTextObject.GetComponentInChildren<UnityEngine.UI.Text>();
        eventlegendString = eventText.text;

        parentHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        //Debug.Log("parentheight: " + parentHeight);
    }

    /// <summary>
    /// Enter a time and message to show them in the EventPanel.
    /// The text is by default bold, but can be changed for minor event.
    /// </summary>
    /// <param name="time">The timestamp</param>
    /// <param name="message">The message you want to show</param>
    /// <param name="bold">Whether the text should be bold</param>
    public void AddEvent(float time, string message, bool bold = true)
    {
        eventText.text = eventlegendString + " " + ++eventCounts;

        GameObject buttonObject = Instantiate(prefabEventButton, Vector3.zero, Quaternion.identity) as GameObject;
        buttonObject.transform.SetParent(transform);
        buttonObject.transform.localPosition = new Vector3(0, buttonPosition);

        buttonPosition -= buttonHeight + buttonSpacing;

        UnityEngine.UI.Text buttonText = buttonObject.GetComponentInChildren<UnityEngine.UI.Text>();
        
        buttonText.text = string.Concat(" #", eventCounts + ") " + ChartUtils.SecondsToTime(time), "\r\n [", message, "]");

        if (bold)
            buttonText.fontStyle = FontStyle.Bold;

        RectTransform panelTransform = transform as RectTransform;
        panelTransform.sizeDelta = new Vector2(panelTransform.rect.width, Mathf.Abs(buttonPosition));
        panelTransform.localPosition = new Vector3(panelTransform.localPosition.x, Mathf.Abs(buttonPosition) - parentHeight);
    }
}
