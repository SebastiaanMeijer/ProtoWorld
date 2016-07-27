/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using UnityEditor;
using System.IO;

public class TrafficIntegrationMenu : MonoBehaviour
{
    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Advance/Sumo/Read Sumo (.xml)")]
    public static void ReadSumo()
    {
        var trafficDB = FindObjectOfType<TrafficIntegrationData>();

        if (trafficDB != null)
        {
            var path = EditorUtility.OpenFilePanel("Open .xml", "", "xml");
            if (path.Length > 0)
            {
                // Clear the trafficDB
                trafficDB.ResetTrafficDB();

                // Instantiate the simulation reader.
                var reader = SimulationReader.FindSimulationReader();
                reader.SetFileName(path);
                reader.SetUseNetworkStream(false);
                reader.SetIO(new SumoIO());
                reader.SetTrafficDB(trafficDB);

                // Start a threaded read.
                reader.ThreadUpdate();
                reader.TryShowWindow();

                //Debug.Log("ReadFZP: DoPasing thread started...");

            }
        }
    }

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Advance/Sumo/Write Sumo (.xml)")]
    public static void WriteSumo()
    {
        var trafficDB = FindObjectOfType<TrafficIntegrationData>();

        if (trafficDB != null)
        {
            var path = EditorUtility.OpenFilePanel("Open .xml", "", "xml");
            if (path.Length > 0)
            {

            }
        }
    }



    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Advance/Vissim/Read Vissim (.fzp)")]
    public static void ReadVissim()
    {
        var trafficDB = FindObjectOfType<TrafficIntegrationData>();
        if (trafficDB != null)
        {
            var path = EditorUtility.OpenFilePanel("Open .fzp", "", "fzp");
            if (path.Length > 0)
            {
                // Clear the trafficDB
                trafficDB.ResetTrafficDB();

                // Instantiate the simulation reader.
                var reader = SimulationReader.FindSimulationReader();
                reader.SetFileName(path);
                reader.SetUseNetworkStream(false);
                reader.SetIO(new VissimIO());
                reader.SetTrafficDB(trafficDB);

                // Start a threaded read.
                reader.ThreadUpdate();
                reader.TryShowWindow();

                //Debug.Log("ReadFZP: DoPasing thread started...");

            }
        }
    }


    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Advance/Matsim/Read Matsim (Database)")]
    public static void ReadMatSim()
    {
        var trafficDB = FindObjectOfType<TrafficIntegrationData>();
        if (trafficDB != null)
        {
            var aramGis = FindObjectOfType<MapBoundaries>();
            var connectionString = aramGis.GetOverridenConnectionString();

            // Instantiate the simulation reader.
            var reader = SimulationReader.FindSimulationReader();
            if (!reader.IsConnectionOpen(connectionString))
                return;

            trafficDB.ResetTrafficDB();

            reader.SetConnectionString(connectionString);
            reader.SetIO(new MatsimIO());
            reader.SetTrafficDB(trafficDB);
            reader.ThreadUpdate();

            //Debug.Log("ReadMatsim: DoPasing thread started...");

            //var path = EditorUtility.OpenFilePanel("Open .xml", "", "xml");
            //if (path.Length > 0)
            //{
            //    // Clear the trafficDB
            //    trafficDB.ResetTrafficDB();

            //    // Instantiate the simulation reader.
            //    var reader = SimulationReader.FindSimulationReader();
            //    reader.SetFileName(path);
            //    reader.SetUseNetworkStream(false);
            //    reader.SetIO(new MatsimIO());
            //    reader.SetTrafficDB(trafficDB);

            //    // Start a threaded read.
            //    reader.ThreadUpdate();
            //    reader.TryShowWindow();

            //    //Debug.Log("ReadFZP: DoPasing thread started...");

            //}
        }
    }

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Advance/Matsim/Read Input files")]
    public static void ReadMatSimFiles()
    {
        ClearMatSimLinks();
        GenerateMatsimNetworkWindow window = (GenerateMatsimNetworkWindow)EditorWindow.GetWindow(typeof(GenerateMatsimNetworkWindow), true, "Read and Export matsim files to DB");
        window.Show();

    }

    [System.Obsolete("Use ReadMatSimFiles instead")]
    public static void ReadMatSimEventFile()
    {
        ClearMatSimLinks();
        string outputPath = @"C:\Users\admgaming\Desktop\Jay\output";

        if (Directory.Exists(outputPath))
        {
            //var container = new MatSimContainer();
            //container.LoadEvents(outputPath);
            //var xmlPath = Path.Combine(outputPath, "event_sample.xml");
            var xmlPath = Path.Combine(outputPath, "500.events.xml");
            var events = MatsimEvents.ReadXml(xmlPath);
            Debug.Log(events.matsimEvents.Count);
            var vehEvents = events.FindEvents("Veh140021");
            foreach (var ev in vehEvents)
            {
                Debug.Log(ev.ToString());
            }
            //events.DebugLog();

        }
    }

    [System.Obsolete("Use ReadMatSimFiles instead")]
    public static void SaveMatSimEventFile()
    {
        string outputPath = @"C:\Users\admgaming\Desktop\Jay\output";

        if (Directory.Exists(outputPath))
        {
            var events = new MatsimEvents();
            events.CreateTestEvents();
            events.Save(Path.Combine(outputPath, "test_output_events.xml"));

            //events = MatsimEvents.Load(Path.Combine(outputPath, "test_output_events.xml"));
            events = MatsimEvents.ReadXml(Path.Combine(outputPath, "test_output_events.xml"));

            events.DebugLog();
        }
    }

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Advance/Matsim/Clear Network")]
    public static void ClearMatSimLinks()
    {
        var parameters = FindObjectOfType<MatSimParameters>();
        if (parameters != null)
        {
            while (parameters.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(parameters.transform.GetChild(0).gameObject);
            }
        }
    }

    [MenuItem("ProtoWorld Editor/Traffic Integration Module/Advance/ProtoWorld Sims/Read meta(pws.meta)")]
    public static void ReadPws()
    {
        var trafficDB = FindObjectOfType<TrafficIntegrationData>();
        if (trafficDB != null)
        {
            var path = EditorUtility.OpenFilePanel("Open .meta", "", "meta");
            if (path.Length > 0)
            {
                // Clear the trafficDB
                trafficDB.ResetTrafficDB();

                // Instantiate the simulation reader.
                var reader = SimulationReader.FindSimulationReader();
                reader.SetFileName(path);
                reader.SetUseNetworkStream(false);
                reader.SetIO(new ProtoMetaIO());
                reader.SetTrafficDB(trafficDB);

                // Start a threaded read.
                reader.ThreadUpdate();
                reader.TryShowWindow();
            }
        }

    }
}


