/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * TRAFFIC INTEGRATION
 * TrafficIntegrationController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

/// <summary>
/// Sets the configuration parameters to integrate a traffic simulation (SUMO, VISSIM or MATSIM) in Unity. 
/// </summary> 
public class TrafficIntegrationController : MonoBehaviour
{
    // Enumerate the options for traffic integration
    public enum TypeOfIntegration
    {
        SumoLiveIntegration,
        SumoFCDFile,
        VissimFZPFile,
        MatsimDatabase,
        PWSimPWSFile,
        DecisionTreeIntegration,
        NoTrafficIntegration
    }

    // Generic properties
    public TypeOfIntegration typeOfIntegration;
    public bool useConfigFile = false;
    public bool useFrustumForUpdate = true;
    public bool useCoordinateConversion = true;

    [HideInInspector]
    [Range(0.1f, 1000)]
    public int timeStepVelocityInMs = 1000;

    // Properties for Sumo Live Integration
    public bool sumoIsRunningInLocalHost = true;
    public string remoteIp = "127.0.0.1";
    public int remotePortForTraci = 3456;
    public int remotePortForListener = 3654;
    public bool vehicleBrakingActive = true;
    public int timeToBrakeInSeconds = 1;
    public int driversPatientInSeconds = 2;
    public int driversAngleOfView = 45;

    // Properties for Sumo FCD File
    public string pathSumoFCDFile = "";

    // Properties for Vissim FZP File
    public string pathVissimFZPFile = "";

    // Properties for Matsim XML File
    public string pathMatSimXMLFile = "";

    // Properties for ProtoWorld PWS Meta File
    public string pathPWSimMetaFile = "";

    // Properties for playing/pausing the traffic simulation in game
    public bool simulationPaused = false;

    /// <summary>
    /// Awakes the script and reads the configuration file (if any).
    /// </summary>
    void Awake()
    {
        string path = Application.dataPath + "/confTraffic.cfg";
        //Debug.Log("This is the path of the traffic config file: " + path);

        //Use the confTraffic.cgf in order to make the project portable
        if (File.Exists(path) == true && useConfigFile)
        {
            Debug.Log("Exists confTraffic.cfg");
            FileStream fs = File.OpenRead(path);
            StreamReader sr = new StreamReader(fs);
            string[] split;
            string line;

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                split = line.Split(new Char[] { ' ' });

                if (split[0].Equals("integrateSUMO"))
                {
                    if (split[1].Equals("true"))
                        typeOfIntegration = TypeOfIntegration.SumoLiveIntegration;
                }
                else if (split[0].Equals("integrateFCDFile"))
                {
                    if (split[1].Equals("true"))
                        typeOfIntegration = TypeOfIntegration.SumoFCDFile;
                }
                else if (split[0].Equals("integrateFZPFile"))
                {
                    if (split[1].Equals("true"))
                        typeOfIntegration = TypeOfIntegration.VissimFZPFile;
                }
                else if (split[0].Equals("integrateMatsimFile"))
                {
                    if (split[1].Equals("true"))
                        typeOfIntegration = TypeOfIntegration.MatsimDatabase;
                }
                else if (split[0].Equals("integratePWsimFile"))
                {
                    if (split[1].Equals("true"))
                        typeOfIntegration = TypeOfIntegration.PWSimPWSFile;
                }
                else if (split[0].Equals("tcpAddress"))
                {
                    if (!split[1].Equals("localhost"))
                    {
                        sumoIsRunningInLocalHost = false;
                        remoteIp = split[1];
                    }
                }
                else if (split[0].Equals("portTraCI"))
                {
                    if (!split[1].Equals(null))
                        remotePortForTraci = int.Parse(split[1]);
                }
                else if (split[0].Equals("portListener"))
                {
                    if (!split[1].Equals(null))
                        remotePortForListener = int.Parse(split[1]);
                }
                else if (split[0].Equals("vehicleBrakingActive"))
                {
                    if (split[1].Equals("false"))
                        vehicleBrakingActive = false;
                }
                else if (split[0].Equals("pathSumoFCDFile"))
                {
                    if (!split[1].Equals(null))
                        pathSumoFCDFile = split[1];
                }
                else if (split[0].Equals("pathVissimFZPFile"))
                {
                    if (!split[1].Equals(null))
                        pathVissimFZPFile = split[1];
                }
                else if (split[0].Equals("pathMatsimXMLFile"))
                {
                    if (!split[1].Equals(null))
                        pathMatSimXMLFile = split[1];
                }
                else if (split[0].Equals("pathPWSimMetaFile"))
                {
                    if (!split[1].Equals(null))
                        pathPWSimMetaFile = split[1];
                }
            }

            sr.Close();
        }
    }
}

