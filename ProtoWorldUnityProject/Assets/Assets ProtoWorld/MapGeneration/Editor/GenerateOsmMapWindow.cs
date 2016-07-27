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

public class GenerateOsmMapWindow : EditorWindow
{
    double[] coordinates = { 0.0f, 0.0f, 0.0f, 0.0f };

    string[] dbConnection = { "127.0.0.1", "5432", "postgre", "GIS", "test" };

    bool generateBuildings = true;
    bool generateRoads = true;
    bool generateWater = false;

    MapBoundaries mapproperties;

    void Awake()
    {
        //Get MapBoundaries
        var globalO = GameObject.Find("AramGISBoundaries");

        if (globalO == null)
        {
            Debug.LogError("ProtoWorld Essentials are missing! To add ProtoWorld Essentials go to menu: ProtoWorldEditor > ProtoWorld Essentials > Add Essentials");
            return;
        }

        mapproperties = globalO.GetComponent<MapBoundaries>();

        //Fill coordinates
        coordinates[0] = mapproperties.minLat;
        coordinates[1] = mapproperties.maxLat;
        coordinates[2] = mapproperties.minLon;
        coordinates[3] = mapproperties.maxLon;

        //Fill db information
        dbConnection[0] = mapproperties.serverAddress;
        dbConnection[1] = mapproperties.serverPort;
        dbConnection[2] = mapproperties.serverUserId;
        dbConnection[3] = mapproperties.serverDatabaseName;
        dbConnection[4] = mapproperties.serverPassword;
    }

    void OnGUI()
    {
        if (mapproperties == null)
            Close();

        GUILayout.Label("OSM Coordinates:", EditorStyles.boldLabel);

        coordinates[0] = EditorGUILayout.DoubleField("Min latitude", coordinates[0]);
        coordinates[1] = EditorGUILayout.DoubleField("Max latitude", coordinates[1]);
        coordinates[2] = EditorGUILayout.DoubleField("Min longitude", coordinates[2]);
        coordinates[3] = EditorGUILayout.DoubleField("Max longitude", coordinates[3]);

        GUILayout.Space(10f);

        GUILayout.Label("DB Connection Parameters:", EditorStyles.boldLabel);

        dbConnection[0] = EditorGUILayout.TextField("DB address", dbConnection[0]);
        dbConnection[1] = EditorGUILayout.TextField("DB port", dbConnection[1]);
        dbConnection[2] = EditorGUILayout.TextField("DB user id", dbConnection[2]);
        dbConnection[3] = EditorGUILayout.TextField("DB name", dbConnection[3]);
        dbConnection[4] = EditorGUILayout.TextField("DB password", dbConnection[4]);

        GUILayout.Space(10f);

        GUILayout.Label("Map Generation Options:", EditorStyles.boldLabel);

        generateBuildings = EditorGUILayout.Toggle("Generate buildings", generateBuildings);
        generateRoads = EditorGUILayout.Toggle("Generate roads", generateRoads);
        generateWater = EditorGUILayout.Toggle("Generate water areas", generateWater);

        GUILayout.Space(10f);

        if (GUILayout.Button("Generate map"))
        {
            mapproperties.minLat = coordinates[0];
            mapproperties.maxLat = coordinates[1];
            mapproperties.minLon = coordinates[2];
            mapproperties.maxLon = coordinates[3];

            mapproperties.serverAddress = dbConnection[0];
            mapproperties.serverPort = dbConnection[1];
            mapproperties.serverUserId = dbConnection[2];
            mapproperties.serverDatabaseName = dbConnection[3];
            mapproperties.serverPassword = dbConnection[4];

            Close();

            // Run the OSMReaderSQL methods
            if (generateBuildings && generateRoads)
            {
                OSMReaderSQL.Create2ServerSide();
            }
            else if (generateBuildings && !generateRoads)
            {
                OSMReaderSQL.Create2_2();
            }
            else if (!generateBuildings && generateRoads)
            {
                OSMReaderSQL.Create2_3();
            }
            if (generateWater)
            {
                OSMReaderSQL.CreateWaterAreas();
            }
        }
    }
}

public class GenerateOSMRoadsWithFilterWindow : EditorWindow
{
    string[] tags = { "motorway", "trunk", "primary", "secondary", "tertiary", "residential", "service" };

    bool[] tagChecks = { false, false, false, false, false, false, false };

    void OnGUI()
    {
        GUILayout.Label("Note: ProtoWorld Essentials is required. The parameters from MapBoundaries (in AramGISBoundaries) needs to be already set. Notice that this tool is experimental: it may produce unexpected malfunctioning.", EditorStyles.helpBox);

        GUILayout.Space(10f);

        GUILayout.Label("Road tags:", EditorStyles.boldLabel);

        for (int i = 0; i < tagChecks.Length; i++)
        {
            tagChecks[i] = EditorGUILayout.Toggle(tags[i], tagChecks[i]);
        }

        GUILayout.Space(10f);
        
        if (GUILayout.Button("Generate roads"))
        {
            List<string> strs = new List<string>();
            for (int i = 0; i < tagChecks.Length; i++)
            {
                if (tagChecks[i])
                    strs.Add(tags[i]);
            }
            OSMReaderSQL.Import(strs.ToArray());
            Close();
        }
    }
}