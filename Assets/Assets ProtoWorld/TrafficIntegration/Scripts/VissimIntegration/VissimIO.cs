/*
 * 
 * TRAFFIC INTEGRATION MODULE
 * VissimIO.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.IO;
using System;
using System.ComponentModel;

public class VissimIO : SimulationIOBase
{
    static char[] delims = new char[] { ';', ' ' };
    string[] lineWords;
    float time, angle;
    string id, x, y;
    string line;
    bool startVehicleRead = false;

    public override bool CanRead(string fileName)
    {
        if (System.IO.Path.GetExtension(fileName).ToLower().Equals("fzp"))
            return true;
        else
            return false;
    }

    float ConvertAngle(float angle)
    {
        var converted = 180f - angle;
        if (converted < 0)
            converted += 360f;
        return converted;
    }

    /// <summary>
    /// Time to read: 00:02:59.2791442
    /// </summary>
    /// <param name="fileName"></param>
    public override void Read(string fileName)
    {
        //ParallelRead(fileName);
        //if (true)
        //    return;
        var fileStream = File.OpenRead(fileName);
        try
        {
            //using (var buffered = new BufferedStream(fileStream))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    Debug.Log("Start reading " + fileName);
                    startVehicleRead = false;

                    while (!shouldStop && reader.Peek() > -1)
                    {
                        line = reader.ReadLine();
                        if (startVehicleRead)
                        {
                            //AddVehicle(line);

                            lineWords = line.Split(delims);
                            time = float.Parse(lineWords[0]);
                            id = lineWords[1];
                            x = lineWords[2];
                            y = lineWords[3];
                            angle = ConvertAngle(float.Parse(lineWords[8]));

                            TryAddTimeStep(time);
                            InsertVehicle(id, x, y, "default", angle.ToString());

                            //Debug.LogFormat("t: {0} id: {1} x: {2} y: {3} ang: {4}", time, id, x, y, angle);
                            //break;
                        }
                        else
                        {
                            if (line.Contains("$VEHICLE"))
                            {
                                //Debug.Log("$VEHICLE read!");
                                startVehicleRead = true;
                                //continue;
                            }
                        }
                    }
                }
                Debug.Log("Finished reading " + fileName);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("Error in fzp-read\n" + e.ToString());
            return;
        }
        finally
        {
            shouldStop = true;

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose();
            }

            UnityEngine.Debug.Log("FileStream closed in VissimIO.");
        }
    }

    public override bool CanWrite(string fileName)
    {
        // TODO: What do we need to check?
        return false;
    }

    public override void Write(object sender, DoWorkEventArgs e)
    {
        //throw new NotImplementedException();
    }



    //TrafficDictionary trafficDct;
    //string[] allLines;
    //ManualResetEvent[] doneEvents;

    //public override void ParallelRead(string fileName)
    //{
    //    trafficDct = new TrafficDictionary();
    //    allLines = File.ReadAllLines(fileName); // Take approx. 1 min to read.
    //    Debug.Log("done reading file");
    //    doneEvents = new ManualResetEvent[allLines.Length];
    //    for (int i = 0; i < allLines.Length; i++)
    //    {
    //        ThreadPool.QueueUserWorkItem(AddVehicle, i);
    //        doneEvents[i] = new ManualResetEvent(false);
    //    }
    //    WaitHandle.WaitAll(doneEvents);
    //    Debug.Log("All done, dict count: " + trafficDct.GetCount());
    //}

    //void AddVehicle(object threadContext)
    //{
    //    int idx = (int)threadContext;
    //    lineWords = allLines[idx].Split(delims);
    //    time = float.Parse(lineWords[0]);
    //    id = lineWords[1];
    //    x = lineWords[2];
    //    y = lineWords[3];
    //    angle = ConvertAngle(float.Parse(lineWords[8]));
    //    trafficDct.InsertVehicle(time, id, x, y, "default", angle.ToString());
    //    doneEvents[idx].Set();
    //}

    //void AddVehicle(string line)
    //{
    //    lineWords = line.Split(delims);
    //    time = float.Parse(lineWords[0]);
    //    id = lineWords[1];
    //    x = lineWords[2];
    //    y = lineWords[3];
    //    angle = ConvertAngle(float.Parse(lineWords[8]));
    //    TryAddTimeStep(time);
    //    InsertVehicle(id, x, y, defaultType, angle.ToString());
    //}


}


