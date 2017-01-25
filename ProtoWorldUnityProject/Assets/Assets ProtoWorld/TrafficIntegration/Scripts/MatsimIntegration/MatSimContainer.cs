/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using MightyLittleGeodesy.Positions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MatSimContainer : MonoBehaviour
{
    public MatSimNetwork matSimNetwork;

    public MatSimSchedule matSimSchedule;

    public MatSimsVehicles vehicleDefinition;

    public MatsimEvents matSimEvents;

    public string connectionString;

    void Start()
    {
        var aramGis = FindObjectOfType<MapBoundaries>();
        connectionString = aramGis.GetOverridenConnectionString();
    }

    public void LoadEvents(string path)
    {
        //var eventsPath = Path.Combine(path, "500.events.xml");
        //var eventsPath = Path.Combine(path, "event_sample.xml");
        //matSimEvents = MatsimEvents.Load(eventsPath);

        matSimEvents = MatsimEvents.ReadXml(path);
        //Debug.Log("events loaded!");

        //Debug.Log(matSimEvents.lines[0]);
        //Debug.Log(matSimEvents.lines[1]);

    }

    public void LoadNetwork(string path)
    {
        var networkPath = Path.Combine(path, "network.xml");
        matSimNetwork = MatSimNetwork.Load(networkPath);
        Debug.Log("network loaded!");
    }

    public void LoadSchedule(string path)
    {
        var schedulePath = Path.Combine(path, "transitScheduleHagastadenMin.xml");
        matSimSchedule = MatSimSchedule.Load(schedulePath);
        Debug.Log("schedule loaded!");

    }

    public void LoadVehicles(string path)
    {
        var vehiclePath = Path.Combine(path, "vehiclesHagastadenMin.xml");
        vehicleDefinition = MatSimsVehicles.Load(vehiclePath);
        Debug.Log("vehicles loaded!");

    }

    /// <summary>
    /// Export matsim network to Postgre db.
    /// </summary>
    /// <param name="connectionString"></param>
    public void ExportEventsToPostgre(string connectionString)
    {
        if (matSimEvents == null)
        {
            Debug.Log("Network is null or empty");
            return;
        }

        matSimEvents.ExportToPostgreSQL(connectionString);
        Debug.Log("Export network done!");
    }

    /// <summary>
    /// Export matsim network to Postgre db.
    /// </summary>
    /// <param name="connectionString"></param>
    public void ExportNetworkToPostgre(string connectionString)
    {
        if (matSimNetwork == null)
        {
            Debug.Log("Network is null or empty");
            return;
        }
        matSimNetwork.ExportToPostgreSQL(connectionString);

        Debug.Log("Export network done!");
    }

    /// <summary>
    /// Export matsim schedule to Postgre db.    
    /// </summary>
    /// <param name="connectionString">Server={0};Port={1};Database={2};User Id={3};Password={4}</param>
    public void ExportScheduleToPostgre(string connectionString)
    {
        if (matSimSchedule == null)
        {
            Debug.Log("Schedule is null or empty");
            return;
        }
        matSimSchedule.ExportToPostgreSQL(connectionString);

        Debug.Log("Export schedule done!");
    }

    public void ExportVehiclesToPostgre(string connectionString)
    {
        if (vehicleDefinition == null)
            return;
        vehicleDefinition.ExportToPostgreSQL(connectionString);

        Debug.Log("Export vehicles done!");
    }

}

