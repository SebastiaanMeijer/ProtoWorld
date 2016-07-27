/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class ProtoMetaIO : SimulationIOBase
{
    List<string[]> fileInfo;
    List<float> simulationTimes;
    AutoResetEvent[] doneEvents;
    ProtoIOWorker[] workers;

    public override bool CanRead(string fileName)
    {
        return false;
    }

    public override bool CanWrite(string fileName)
    {
        return false;
    }

    void ListsToString()
    {
        foreach (var t in simulationTimes)
        {
            UnityEngine.Debug.Log(t);
        }
        foreach (var f in fileInfo)
        {
            UnityEngine.Debug.Log(f[0] + ", " + f[1] + ", " + f[2]);
        }
    }

    public override void RequestStop()
    {
        if (workers != null)
        {
            foreach (var w in workers)
            {
                w.RequestStop();
            }
        }
    }

    public override string GetStatus()
    {
        string status = "progress:\n";
        if (workers != null)
        {
            for (int i = 0; i < workers.Length; i++)
            {
                var w = workers[i];
                status += "worker " + i + ": " + workers[i].progressPercentage + "%\n";
            }
        }
        return status;
    }

    /// <summary>
    /// Call this to read the meta-file of PWSim.
    /// Initiates a ProtoIOWorker for each PWSim-file.
    /// </summary>
    /// <param name="fileName"></param>
    public override void Read(string fileName)
    {

        try
        {
            //Process[] procs = Process.GetProcessesByName("unity");
            //ProcessThreadCollection threads = procs[0].Threads;
            //foreach (var t in threads)
            //{
            //    UnityEngine.Debug.Log(t.ToString());
            //}

            // Read the meta-file.
            fileInfo = new List<string[]>();
            simulationTimes = new List<float>();
            var path = Path.GetDirectoryName(fileName);
            UnityEngine.Debug.Log("Meta file directory: " + path);
            var lines = File.ReadAllLines(fileName);
            float simTime;
            foreach (var line in lines)
            {
                var words = line.Split(' ');
                if (line.Contains("TIMES"))
                {
                    //Debug.Log("words count: " + words.Length);
                    for (int i = 1; i < words.Length; i++)
                    {
                        if (float.TryParse(words[i], out simTime))
                            simulationTimes.Add(simTime);
                    }
                }
                else if (line.Contains("FILENAME"))
                {
                    var info = new string[] { Path.Combine(path, words[1]), words[2], words[3] };
                    fileInfo.Add(info);
                }
            }

            //DebugLists();
            //trafficDB.ResetTrafficDB();

            // Init trafficDB.
            foreach (var t in simulationTimes)
            {
                trafficDB.InsertNewTimeStep(t);
            }
            UnityEngine.Debug.Log("time steps initiated... " + trafficDB.getNumberOfTimeSteps());

            // Load all files to container without parsing to the traffic database.
            //ParseToContainer();

            // Load and parse all files to traffic database.
            ParseToTrafficDB();

        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("error reading meta-file: " + e.ToString());
        }
        finally
        {
            base.RequestStop();
        }
    }

    protected void ParseToContainer()
    {
        UnityEngine.Debug.Log("Parsing to container!");
        doneEvents = new AutoResetEvent[fileInfo.Count];
        for (int i = 0; i < fileInfo.Count; i++)
        {
            var loader = new SimpleFileIO(); // Init the file loader.
            doneEvents[i] = loader.doneEvent; // Keep track if backgroundWorker is done.
            loader.LoadFile(fileInfo[i][0]); // Start the backgroundWorker.
        }

        // Wait for all loaders to be finished.
        WaitHandle.WaitAll(doneEvents);
        UnityEngine.Debug.Log("All loaders are done!");
    }

    protected void ParseToTrafficDB()
    {
        UnityEngine.Debug.Log("Parsing to traffic DB!");
        // Init workers to read files.
        doneEvents = new AutoResetEvent[fileInfo.Count];
        workers = new ProtoIOWorker[fileInfo.Count];
        for (int i = 0; i < fileInfo.Count; i++)
        {
            var pwsReader = new ProtoIOWorker();
            workers[i] = pwsReader;
            pwsReader.SetTrafficDB(trafficDB);
            pwsReader.SetFileInfo(fileInfo[i]);
            doneEvents[i] = pwsReader.doneEvent;
            //Debug.Log(i + ": start read.");
            pwsReader.Read();
        }

        // Wait for all readers to be finished.
        WaitHandle.WaitAll(doneEvents);
        UnityEngine.Debug.Log("All workers are done!");
    }

    public override void Write(object sender, DoWorkEventArgs e)
    {
        //throw new NotImplementedException();
    }
}

public class SimpleFileIO
{
    protected string[] fileLines;

    protected BackgroundWorker worker;

    public AutoResetEvent doneEvent;

    public void LoadFile(string fileName)
    {
        worker = new BackgroundWorker
        {
            WorkerReportsProgress = false,
            WorkerSupportsCancellation = true
        };
        doneEvent = new AutoResetEvent(false);
        worker.DoWork += LoadLines;
        worker.RunWorkerCompleted += WorkerCompleted;
        worker.RunWorkerAsync(fileName);
    }

    public void LoadLines(object sender, DoWorkEventArgs e)
    {
        var path = e.Argument.ToString();
        fileLines = File.ReadAllLines(path);
        doneEvent.Set();
    }

    public string[] GetLines()
    {
        return fileLines;
    }

    public void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Cancelled)
            UnityEngine.Debug.Log("You canceled!");
        else if (e.Error != null)
            UnityEngine.Debug.Log("Worker exception: " + e.Error.ToString());
        else
            UnityEngine.Debug.Log("Complete: " + e.Result);
    }


}
