/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DECISION TREE MODULE
 * DecisionTreeController.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;
using System.IO;

public class DecisionTreeController : MonoBehaviour
{
    public bool useConfigFile = false;

    public string pathDecisionXMLFile = "";

    /// <summary>
    /// Make sure that it is TypeOfIntegration.NoTrafficIntegration in TrafficIntegrationController,
    /// or it will start reading some other simulation if defined in a config file.
    /// </summary>
    void Awake()
    {
        string path = Path.Combine(Application.dataPath, "confDecision.cfg");
        //Debug.Log("This is the path of the decision tree config file: " + path);

        //Use the confDecision.cgf in order to make the project portable
        if (File.Exists(path) == true && useConfigFile)
        {
            Debug.Log("Exists confDecision.cfg");
            string[] allLines = File.ReadAllLines(path);

            foreach (var line in allLines)
            {
                var split = line.Split(' ');
                if (split[0].Equals("pathDecisionXMLFile"))
                {
                    if (!split[1].Equals(null))
                        pathDecisionXMLFile = split[1];
                }
            }
        }
    }
}
