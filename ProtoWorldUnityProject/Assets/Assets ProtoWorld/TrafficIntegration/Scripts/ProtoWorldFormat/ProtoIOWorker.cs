/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System;

public class ProtoIOWorker
{
    protected TrafficIntegrationData trafficDB;

    protected string fileName;

    protected float startStep, endStep, totalSteps;

    protected BackgroundWorker worker;

    public AutoResetEvent doneEvent;

    public float progressPercentage;

    public bool IsBusy()
    {
        if (worker != null)
            return worker.IsBusy;
        else
            return false;
    }

    public ProtoIOWorker()
    {
        worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        doneEvent = new AutoResetEvent(false);
    }

    public void SetTrafficDB(TrafficIntegrationData trafficDB)
    {
        this.trafficDB = trafficDB;
    }

    public void SetFileInfo(string[] info)
    {
        if (info.Length != 3)
            return;
        SetFileName(info[0]);
        SetStartAndEnd(info[1], info[2]);
    }

    public void SetFileName(string fileName)
    {
        this.fileName = fileName;
    }

    public void SetStartAndEnd(string start, string end)
    {
        startStep = int.Parse(start);
        endStep = int.Parse(end);
        totalSteps = endStep - startStep;
    }

    public void Read()
    {
        worker.DoWork += ReadPws;
        worker.ProgressChanged += ReadProgress;
        worker.RunWorkerCompleted += ReadCompleted;
        worker.RunWorkerAsync(fileName);
    }

    public void RequestStop()
    {
        worker.CancelAsync();
        //Debug.Log("cancelling processing " + fileName);
    }

    void ReadPws(object sender, DoWorkEventArgs e)
    {

        string line = "";
        string[] lineWords, vehWords;
        int ts, VehCount;

        Debug.Log("Start reading " + e.Argument);

        var fileStream = File.OpenRead((string)e.Argument);
        try
        {
            using (var reader = new StreamReader(fileStream))
            {
                while (!worker.CancellationPending && reader.Peek() > -1)
                {
                    //Thread.Sleep(1000); // Test code.

                    line = reader.ReadLine();

                    // skip empty line.
                    if (line.Length < 1)
                        continue;

                    // skip commented line.
                    if (line[0].Equals('*'))
                        continue;

                    // split the line into words.
                    lineWords = line.Split(';');
                    ts = int.Parse(lineWords[0]);
                    worker.ReportProgress(ts);
                    VehCount = int.Parse(lineWords[1]);
                    // parse the words to vehicle info.
                    for (int i = 2; i < lineWords.Length; i++)
                    {
                        if (lineWords[i].Length > 0)
                        {
                            vehWords = lineWords[i].Split(' ');
                            if (vehWords.Length == 4)
                            {
                                // Old format:
                                trafficDB.InsertVehicle(ts, vehWords[0], vehWords[1], vehWords[2], "VEH", vehWords[3]);
                            }
                            else if (vehWords.Length == 5)
                            {
                                // New format:
                                trafficDB.InsertVehicle(ts, vehWords[0], vehWords[1], vehWords[2], vehWords[3], vehWords[4]);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("Error in pws-read\n" + ex.ToString() + "\n" + line);
            //return;
        }
        finally
        {
            if (worker.CancellationPending)
                e.Cancel = true;
            else
                e.Result = fileName + " done!";

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose();
            }

            // Signal that reading is finished.
            doneEvent.Set();
            //Debug.Log("FileStream closed in ProtoIO.");
        }

    }

    void ReadCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Cancelled)
            Debug.Log("User canceled!");
        else if (e.Error != null)
            Debug.Log("Worker exception: " + e.Error.ToString());
        else
            Debug.Log("Complete: " + e.Result);
    }

    void ReadProgress(object sender, ProgressChangedEventArgs e)
    {
        progressPercentage = (e.ProgressPercentage - startStep) / totalSteps * 100f;
        //Debug.Log("Reached: " + pct + "%");
    }
}
