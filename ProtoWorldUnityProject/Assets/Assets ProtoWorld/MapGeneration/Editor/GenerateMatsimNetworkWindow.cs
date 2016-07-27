/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class GenerateMatsimNetworkWindow : EditorWindow
{
    public string serverAddress = "127.0.0.1";
    public string serverPort = "5432";
    public string serverUserId = "postgres";
    public string serverDatabaseName = "gis_ARAM_stockholms_lan";
    public string serverPassword = "test";
    public bool useDefaultValues = true;
    public string connectionString;
    public string status = "";

    string path = @"C:\Users\admgaming\Desktop\Jay\";

    void OnGUI()
    {
        useDefaultValues = EditorGUILayout.Toggle("use MapBoundaries values", useDefaultValues);
        var aramGis = FindObjectOfType<MapBoundaries>();
        if (useDefaultValues)
        {
            EditorGUILayout.LabelField("address", aramGis.serverAddress);
            EditorGUILayout.LabelField("port", aramGis.serverPort);
            EditorGUILayout.LabelField("user id", aramGis.serverUserId);
            EditorGUILayout.LabelField("password", aramGis.serverPassword);
            EditorGUILayout.LabelField("database name", aramGis.serverDatabaseName);
            connectionString = aramGis.GetOverridenConnectionString();
        }
        else
        {
            serverAddress = EditorGUILayout.TextField("address", serverAddress);
            serverPort = EditorGUILayout.TextField("port", serverPort);
            serverUserId = EditorGUILayout.TextField("user id", serverUserId);
            serverPassword = EditorGUILayout.TextField("password", serverPassword);
            serverDatabaseName = EditorGUILayout.TextField("database name", serverDatabaseName);
            connectionString = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", serverAddress, serverPort, serverDatabaseName, serverUserId, serverPassword);
        }
        GUILayout.Space(20);

        if (GUILayout.Button("Read matsim event files "))
        {
            var go = ProtoWorldMenu.AddTrafficIntegModule();
            if (go == null)
            {
                Debug.Log("Something wrong when adding 'Traffic Integration Module'");
                return;
            }

            MatSimContainer container = go.GetComponent<MatSimContainer>();
            if (container == null)
            {
                container = go.AddComponent<MatSimContainer>();
            }

            path = EditorUtility.OpenFilePanel("Matsim event file", path, "");

            if (File.Exists(path))
            {
                status = "Read events...";
                Debug.Log(status);
                container.LoadEvents(path);

                //var vehEvents = container.matSimEvents.FindEvents("Veh140021");
                //foreach (var ev in vehEvents)
                //{
                //    Debug.Log(ev.ToString());
                //}
            }
        }

        if (GUILayout.Button("Export events to Postgre DB "))
        {
            var go = ProtoWorldMenu.AddTrafficIntegModule();
            if (go == null)
            {
                Debug.Log("Something wrong when adding 'Traffic Integration Module'");
                return;
            }

            var container = go.GetComponent<MatSimContainer>();
            if (container == null)
            {
                Debug.Log("Something wrong when getting 'MatSimContainer'");
                return;
            }
            
            //string connectionString = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", serverAddress, serverPort, serverDatabaseName, serverUserId, serverPassword);
            status = "Exporting events...";
            container.ExportEventsToPostgre(connectionString);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Read matsim files "))
        {
            var go = ProtoWorldMenu.AddTrafficIntegModule();
            if (go == null)
            {
                Debug.Log("Something wrong when adding 'Traffic Integration Module'");
                return;
            }

            MatSimContainer container = go.GetComponent<MatSimContainer>();
            if (container == null)
            {
                container = go.AddComponent<MatSimContainer>();
            }

            path = EditorUtility.OpenFolderPanel("Matsim input files folder", path, "");

            status = "Read network...";
            Debug.Log(status);
            container.LoadNetwork(path);

            status = "Read schedules...";
            Debug.Log(status);
            container.LoadSchedule(path);

            status = "Read vehicles...";
            Debug.Log(status);
            container.LoadVehicles(path);

            status = "Done reading network, schedule & vehicles...";

        }

        if (GUILayout.Button("Export network to Postgre DB "))
        {
            var go = ProtoWorldMenu.AddTrafficIntegModule();
            if (go == null)
            {
                Debug.Log("Something wrong when adding 'Traffic Integration Module'");
                return;
            }

            var container = go.GetComponent<MatSimContainer>();
            if (container == null)
            {
                Debug.Log("Something wrong when getting 'MatSimContainer'");
                return;
            }
            //Debug.Log("Found 'MatSimContainer', proceeding...");
            //string connectionString = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", serverAddress, serverPort, serverDatabaseName, serverUserId, serverPassword);
            //Debug.Log(connectionString);
            status = "Exporting network...";
            container.ExportNetworkToPostgre(connectionString);

            status = "Done exporting network.";
        }

        if (GUILayout.Button("Export schedule to Postgre DB "))
        {
            var go = ProtoWorldMenu.AddTrafficIntegModule();
            if (go == null)
            {
                Debug.Log("Something wrong when adding 'Traffic Integration Module'");
                return;
            }

            var container = go.GetComponent<MatSimContainer>();
            if (container == null)
            {
                Debug.Log("Something wrong when getting 'MatSimContainer'");
                return;
            }
            //string connectionString = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", serverAddress, serverPort, serverDatabaseName, serverUserId, serverPassword);
            status = "Exporting schedule...";
            container.ExportScheduleToPostgre(connectionString);

            status = "Done exporting schedule.";
        }

        if (GUILayout.Button("Export vehicles to Postgre DB "))
        {
            var go = ProtoWorldMenu.AddTrafficIntegModule();
            if (go == null)
            {
                Debug.Log("Something wrong when adding 'Traffic Integration Module'");
                return;
            }

            var container = go.GetComponent<MatSimContainer>();
            if (container == null)
            {
                Debug.Log("Something wrong when getting 'MatSimContainer'");
                return;
            }
            //string connectionString = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", serverAddress, serverPort, serverDatabaseName, serverUserId, serverPassword);
            status = "Exporting vehicles...";
            container.ExportVehiclesToPostgre(connectionString);

            status = "Done exporting vehicles.";
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Close"))
        {
            Close();
        }
        EditorGUILayout.LabelField(status);

    }


}