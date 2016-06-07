/*
 * 
 * ELEMENT UI
 * SelectSmartObjectFromElementUI.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Auxiliar script for the UI interface that automates the selection of interactive SmartObjects. This script works along with ListOfInteractiveSmartObjects.
/// </summary>
public class SelectSmartObjectFromElementUI : MonoBehaviour 
{
    public string smartObjectNameToSelect = "?";

	public void SelectSmarObject()
    {
        GameObject smartObjectSelected = GameObject.Find("SmartObjects").GetComponent<ListOfInteractiveSmartObjects>().interactiveSmartObjects[smartObjectNameToSelect];

        if (smartObjectSelected != null)
        {
            if (smartObjectSelected.GetComponent<SEStarSmartObject>().objectName.StartsWith("Security"))    
                smartObjectSelected.GetComponent<SecurityCheckControl>().VinculateElementUI();

            else if (smartObjectSelected.GetComponent<SEStarSmartObject>().objectName.StartsWith("Spawner"))
                smartObjectSelected.GetComponent<SpawnerControl>().VinculateElementUI();
        }    
    }
}
