/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* 
* FLASH PEDESTRIAN SIMULATOR
* FlashAddPedestriansUIController.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Controller for the UI interface to add Flash Pedestrians during game time.
/// </summary>
public class FlashAddPedestriansUIController : MonoBehaviour
{
    /// <summary>
    /// Input field where the player inputs the number of pedestrians to add. 
    /// </summary>
    public InputField numPedInputField;

    /// <summary>
    /// Flash Pedestrian Spawners of the scene. 
    /// </summary>
    private FlashPedestriansSpawner[] spawners;

    private FlashPedestriansGlobalParameters pedGlobal;

    /// <summary>
    /// Script awakening. 
    /// </summary>
	void Awake ()
    {
        spawners = FindObjectsOfType<FlashPedestriansSpawner>();
        pedGlobal = FindObjectOfType<FlashPedestriansGlobalParameters>();
	}

    /// <summary>
    /// Adds new pedestrians to the scene. They will be equally distribute among the existing spawners.
    /// </summary>
    public void AddPedestrians()
    {
        if (spawners.Length > 0 && numPedInputField != null)
        {
            //Try parsing the value in the input field
            int numPedestrians;
            int.TryParse(numPedInputField.text, out numPedestrians);

            // Distribute new pedestrians equally among the spawners
            if (numPedestrians > 0)
            {
                int pedPerSpawner =
                    Mathf.FloorToInt((Mathf.Floor(numPedestrians / (float)spawners.Length)) 
                    / (float) pedGlobal.numberOfPedestriansPerAgent);

                foreach (FlashPedestriansSpawner S in spawners)
                    S.AddPedestrians(pedPerSpawner);
            }
        }
    }
}
