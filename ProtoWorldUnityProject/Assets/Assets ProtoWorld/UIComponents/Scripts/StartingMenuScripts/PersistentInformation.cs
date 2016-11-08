/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * STARTING MENU
 * PersistentInformation.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Class that preserves persistent information between scenes. 
/// </summary>
public class PersistentInformation : MonoBehaviour
{
    /// <summary>
    /// Population number.
    /// </summary>
    [Range(0, 1000000)]
    public int populationNumber = 500;

    /// <summary>
    /// Population employment rate.
    /// </summary>
    [Range(0f, 1f)]
    public float employmentRate = 0.8f;

    /// <summary>
    /// Public transport frequency in seconds.
    /// </summary>
    [Range(1, 3600)]
    public int publicTransportFrequencyInSec = 300;

    /// <summary>
    /// Public transport fare per trip.
    /// </summary>
    [Range(0, 100)]
    public float farePerTrip = 10;

    /// <summary>
    /// Script awakening.
    /// </summary>
    void Awake()
    {
        // This gameobject will not be destroyed on new scene loaded 
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Change the population number value. 
    /// </summary>
    /// <param name="value">New value for the population number.</param>
    public void ChangePopulationNumber(float value)
    {
        populationNumber = (int) value;
    }

    /// <summary>
    /// Change the employment rate value. 
    /// </summary>
    /// <param name="value">New value for the employment rate.</param>
    public void ChangeEmploymentRate(float value)
    {
            employmentRate = value;
    }

    /// <summary>
    /// Change the public transport frequency value. 
    /// </summary>
    /// <param name="value">New value for the public transport frequency.</param>
    public void ChangePublicTransportFrequency(float value)
    {
        publicTransportFrequencyInSec = (int) value;
    }

    /// <summary>
    /// Change the fare value. 
    /// </summary>
    /// <param name="value">New value for the fare.</param>
    public void ChangeFare(float value)
    {
        farePerTrip = value;
    }
}
