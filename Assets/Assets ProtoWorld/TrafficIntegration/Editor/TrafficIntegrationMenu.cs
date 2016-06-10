/*
 * 
 * TRAFFIC INTEGRATION
 * TrafficIntegrationMenu.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using UnityEditor;
using System.IO;

public class TrafficIntegrationMenu : MonoBehaviour
{
    [MenuItem("Traffic Integration/Sumo/Read Sumo (.xml)", false, 1)]
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

    [MenuItem("Traffic Integration/Sumo/Write Sumo (.xml)", false, 1)]
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



    [MenuItem("Traffic Integration/Vissim/Read Vissim (.fzp)", false, 1)]
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


    [MenuItem("Traffic Integration/Matsim/Read Matsim (.xml)", false, 1)]
    public static void ReadMatSim()
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
                reader.SetIO(new MatsimIO());
                reader.SetTrafficDB(trafficDB);

                // Start a threaded read.
                reader.ThreadUpdate();
                reader.TryShowWindow();

                //Debug.Log("ReadFZP: DoPasing thread started...");

            }
        }
    }

    [MenuItem("Traffic Integration/Matsim/Build Network", false, 2)]
    public static void ReadMatSimFiles()
    {
        ClearMatSimLinks();
        string inputPath = @"C:\Users\admgaming\Desktop\Jay\input";


        if (Directory.Exists(inputPath))
        {
            var container = new MatSimContainer();

            container.LoadSchedule(inputPath);
            container.ExportScheduleToPostgre();

            //container.Load(path);
            //container.BuildNetwork();
        }
    }

    [MenuItem("Traffic Integration/Matsim/Load Event", false, 3)]
    public static void ReadMatSimEventFile()
    {
        ClearMatSimLinks();
        string outputPath = @"C:\Users\admgaming\Desktop\Jay\output";

        if (Directory.Exists(outputPath))
        {
            var container = new MatSimContainer();

            container.LoadEvents(outputPath);
        }
    }

    [MenuItem("Traffic Integration/Matsim/Clear Network", false, 3)]
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

    [MenuItem("Traffic Integration/ProtoWorld Sims/Read meta(pws.meta)", false, 1)]
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


