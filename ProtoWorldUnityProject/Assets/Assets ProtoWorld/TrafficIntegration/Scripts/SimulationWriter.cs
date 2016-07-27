/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.IO;

public class SimulationWriter : MonoBehaviour
{
    //protected TrafficIntegrationData trafficDB;
    //public string fileName;
    //public SimulationIOBase simulationIO;
    //private static string moduleName = "SimulationWriter";

    //void Awake()
    //{

    //}


    //public void SetFileName(string fileName)
    //{
    //    this.fileName = fileName;
    //}

    //public void SetIO(SimulationIOBase simulationIO)
    //{
    //    this.simulationIO = simulationIO;
    //}

    //public void Init()
    //{
    //    trafficDB = FindObjectOfType<TrafficIntegrationData>();
    //    var writer = TryAddWriterToScene();
    //}

    //public void WorkerUpdate()
    //{
    //    if (Initialized())
    //    {
    //        simulationIO.SetTrafficDB(trafficDB);
    //        simulationIO.Write(fileName);
    //    }
    //}

    //public bool Initialized()
    //{
    //    if (simulationIO != null && trafficDB != null)
    //    {
    //        if (File.Exists(fileName))
    //            return true;
    //    }
    //    return false;
    //}

    //public static SimulationWriter TryAddWriterToScene()
    //{
    //    var reader = FindObjectOfType<SimulationWriter>();
    //    if (reader == null)
    //    {
    //        var controller = FindObjectOfType<TrafficIntegrationController>();
    //        var go = new GameObject(moduleName);
    //        go.transform.SetParent(controller.transform);
    //        reader = go.AddComponent<SimulationWriter>();
    //    }
    //    return reader;
    //}
}
