/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashMessageDropdownUIController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controller for the UI canvas to visualize a dropdown list of custom messages when opening/closing stations. 
/// </summary>
public class FlashMessageDropdownUIController : MonoBehaviour
{
    public UnityEngine.UI.InputField inputField;
    public PredefinedMessage[] optionList;

    [HideInInspector]
    public UnityEngine.UI.Dropdown messageDropdown;

    void Awake()
    {
        messageDropdown = this.GetComponent<UnityEngine.UI.Dropdown>();
        messageDropdown.value = 0;
        FillOptionList();
    }

    void FillOptionList()
    {
        messageDropdown.ClearOptions();
        List<string> titleList = new List<string>();

        for (int i = 0; i < optionList.Length; i++)
            titleList.Add(optionList[i].title);

        messageDropdown.AddOptions(titleList);
    }

    public void UpdateInputField()
    {
        string text = optionList[messageDropdown.value].text;

        FlashStationUIController stationUI = this.transform.GetComponentInParent<FlashStationUIController>();

        if (stationUI != null)
            text = text.Replace("[station]", stationUI.stationController.stationName);

        inputField.text = text;
    }

    public PredefinedMessage GetMessageSelected()
    {
        return optionList[messageDropdown.value];
    }
}

/// <summary>
/// Class that defines a PredefinedMessage. 
/// </summary>
[System.Serializable]
public class PredefinedMessage
{
    public string title;
    public string text;
    public bool highInfluence;
}
