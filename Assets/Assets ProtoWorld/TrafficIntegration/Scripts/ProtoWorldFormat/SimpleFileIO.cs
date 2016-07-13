/*
 * 
 * TRAFFIC INTEGRATION MODULE
 * SimpleFileIO.cs
 * Johnson Ho
 * 
 */

using System.ComponentModel;
using System.Threading;
using System.IO;

/// <summary>
/// Class that read a files and store every line in an array.
/// </summary>
/// <remarks>The class is called by ProtoMetaIO but not in use.</remarks>
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
