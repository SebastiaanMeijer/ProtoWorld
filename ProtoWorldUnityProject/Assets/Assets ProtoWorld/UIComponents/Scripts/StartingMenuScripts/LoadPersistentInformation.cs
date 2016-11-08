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
 * LoadPersistentInformation.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Loads the persistent information in the scene. 
/// </summary>
public class LoadPersistentInformation : MonoBehaviour
{
    /// <summary>
    /// Finds the objects and loads the information.
    /// </summary>
    void Awake()
    {
        var pers = FindObjectOfType<PersistentInformation>();

        if (pers != null)
        {
            //Set the amount of population using public transport
            var globalParam = FindObjectOfType<FlashPedestriansGlobalParameters>();
            globalParam.percOfPedWillingToTakeTransport = pers.farePerTrip == 0 ? 1.0f : 0.75f;

            //Distribute the population among the pedestrian spawners
            FlashPedestriansSpawner[] spawners = FindObjectsOfType<FlashPedestriansSpawner>();
            int popPerSpawner = Mathf.FloorToInt((pers.populationNumber / (float)globalParam.numberOfPedestriansPerAgent) / (float)spawners.Length);
            foreach (var S in spawners)
            {
                S.maxNumberOfPedestriansToSpawn = popPerSpawner;
                S.initialSpawningBatch = popPerSpawner;
            }
       
            //Set the frequency of the public transport
            SpawnerLineController[] lines = FindObjectsOfType<SpawnerLineController>();
            foreach (var L in lines)
            {
                L.useGlobalSpawnRate = false;
                L.localSpawnRate = pers.publicTransportFrequencyInSec;
                L.spawnFromStart = true;
            }

            //Set the destinations priorities according to employability
            FlashPedestriansDestination[] destinations = FindObjectsOfType<FlashPedestriansDestination>();
            foreach (var D in destinations)
            {
                if (D.isDestinationForWork)
                {
                    if (pers.employmentRate == 0)
                        D.destinationPriority = 0;
                    else if (pers.employmentRate <= 0.5)
                        D.destinationPriority = 5;
                    else
                        D.destinationPriority = 10;
                }

                else if (D.isDestinationForLeisure)
                {
                    if (pers.employmentRate == 0)
                        D.destinationPriority = 10;
                    else if (pers.employmentRate <= 0.5)
                        D.destinationPriority = 5;
                    else
                        D.destinationPriority = 0;
                }
            }
        }
    }
}

