/*
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
