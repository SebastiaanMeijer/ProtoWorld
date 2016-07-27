/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controller for the Smart Object "Security Check".
/// </summary>
public class RoadBlockControl : MonoBehaviour
{
    public bool eastGatesOpen = true;
    public bool northGatesOpen = true;
    public bool westGatesOpen = true;
    public bool southGatesOpen = true;

    private bool oldEastGatesOpen = true;
    private bool oldNorthGatesOpen = true;
    private bool oldWestGatesOpen = true;
    private bool oldSouthGatesOpen = true;

    //public int securityControlTime = 15;
    //private int oldSecurityControlTime = 15;

    private SEStarSmartObject smartObject;
    private SEStar SEStarControl;
    private Transform blockObject;
    private Vector3 initialScale;

    private float period = 1.0f;
    private float nextActionTime = 0.0f;

    public string kpiManagerName;
    private KPIManager kpiManager;
    private int kpiKey;

    /// <summary>
    /// Starts the script. 
    /// </summary>
    void Start()
    {
        Debug.Log("Road Block object created");

        smartObject = this.GetComponent<SEStarSmartObject>();
        SEStarControl = GameObject.FindObjectOfType<SEStar>();

        //this.GetComponent<UIDynamicStructure>().Title = smartObject.objectName;
        
        //Change road block state to default (everything open)
        //SESTar must fix first the proper behaviour of pedestrians
        //UpdateRoadBlockState();
    }

    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            if (eastGatesOpen != oldEastGatesOpen || northGatesOpen != oldNorthGatesOpen
                || westGatesOpen != oldWestGatesOpen || southGatesOpen != oldSouthGatesOpen)
            {
                UpdateRoadBlockState();
            }
        }
    }

    /// <summary>
    /// Updates the state of the road block according to the public boolean parameters of this script. 
    /// </summary>
    public void UpdateRoadBlockState()
    {
        oldEastGatesOpen = eastGatesOpen;
        oldWestGatesOpen = westGatesOpen;
        oldNorthGatesOpen = northGatesOpen;
        oldSouthGatesOpen = southGatesOpen;

        Debug.Log("Updating road block state");

        string state = "Filtering";

        // Calculate the string name of the state according to the booleans
        if (eastGatesOpen)
            state += "_ABCDEFG";
        if (northGatesOpen)
            state += "_HI";
        if (southGatesOpen)
            state += "_JKLM";
        if (westGatesOpen)
            state += "_NOP";

        Debug.Log("Changing state of the road block to " + state);

        if (state == "Filtering")
            // If no boolean active, road block is blocked
            SEStarControl.ChangeSEStarObjectState(smartObject.objectId, "Blocked");
        else
            // Else, apply the state calculated
            SEStarControl.ChangeSEStarObjectState(smartObject.objectId, state);

        //TODO handle the graphics
    }
}

