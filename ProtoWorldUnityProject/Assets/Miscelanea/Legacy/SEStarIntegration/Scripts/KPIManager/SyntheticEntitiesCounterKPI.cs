/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * SESTAR INTEGRATION
 * SyntheticEntitiesCounterKPI.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Auxiliar script for the KPI manager that counts the number of a certain type of pedestrian according to the SEStar simulation.
/// </summary>
public class SyntheticEntitiesCounterKPI : MonoBehaviour 
{
    public string typeOfPedestrianToCount = null;
    public bool countPedestriansCreated;
    public bool countPedestriansDeleted;

    private SEStar SEStarControl;
    private KPIManager kpiManagerForCounting;
    private int kpiKeyForCounting;
    private int counter = 0;

    /// <summary>
    /// Initializes the script.
    /// </summary>
    void Start()
    {
        SEStarControl = GameObject.FindObjectOfType<SEStar>();
        kpiManagerForCounting = this.transform.GetComponent<KPIManager>();
        kpiKeyForCounting = kpiManagerForCounting.RegisterNewInformer();
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        int updatedCounter;

        if (countPedestriansCreated && countPedestriansDeleted)
            updatedCounter = SEStarControl.GetNumberOfSyntheticEntitiesInSimulation(typeOfPedestrianToCount);
        else if (countPedestriansCreated)
            updatedCounter = SEStarControl.GetNumberOfSyntheticEntitiesCreated(typeOfPedestrianToCount);
        else if (countPedestriansDeleted)
            updatedCounter = SEStarControl.GetNumberOfSyntheticEntitiesDeleted(typeOfPedestrianToCount);
        else
            updatedCounter = 0;

        if (updatedCounter != counter)
        {
            counter = updatedCounter;
            kpiManagerForCounting.InformKPI(kpiKeyForCounting, counter);
        }
    }
}
