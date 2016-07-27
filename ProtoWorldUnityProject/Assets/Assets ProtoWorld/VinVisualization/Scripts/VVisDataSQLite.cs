/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * VIN VISUALIZATION
 * VVisDataSQLite.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Script that controls the opening and querying of the VVIS SQLite database.
/// </summary>
public class VVisDataSQLite : MonoBehaviour
{
    public string dbPathFromStreamingAsset = "/SQLiteDB/trajectoriesDB";
    private string fulldbPath = "";

    private VVisVisualizer visualizer;
    private SqliteConnection dbConnection;
    private Thread thread;

    private bool dbIsReady = false;
    private bool queryError = false;
    private bool lastQueryCompleted = true;
    private List<string> selectedRows;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        visualizer = FindObjectOfType<VVisVisualizer>();
        selectedRows = new List<string>();

        fulldbPath = Application.streamingAssetsPath + dbPathFromStreamingAsset;
    }

    /// <summary>
    /// Starts the script and opens the SQLite database.
    /// </summary>
    void Start()
    {
        thread = new Thread(new ThreadStart(OpenSQLiteDatabase));
        thread.Start();
    }

    /// <summary>
    /// Checks if the db is ready to be queried.
    /// </summary>
    /// <returns>True if db is ready.</returns>
    internal bool IsDBReady()
    {
        return dbIsReady;
    }

    /// <summary>
    /// Requests the SQLite database to run a query.
    /// </summary>
    /// <param name="requestor">Owner of the requested call. A message 
    /// RequestDataSelection_CallBack will be sent to this gameObject with a List of strings
    /// containing the selection.</param>
    /// <param name="queryObj">VVIS query object containing the query information.</param>
    internal void RequestQuery(GameObject requestor, VVisQueryRequest queryObj)
    {
        if (dbIsReady && lastQueryCompleted)
        {
            thread.Abort();
            thread = new Thread(() => ExecuteQuery(queryObj));
            UnityEngine.Debug.Log("Thread for requesting query created");
            lastQueryCompleted = false;
            thread.Start();

            StartCoroutine(CallbackToRequestor(requestor));
        }
    }

    /// <summary>
    /// Auxiliary method for threading.
    /// Executes the given query and fills the selectedRows list with the results.
    /// </summary>
    /// <param name="queryObj">VVIS query object containing the query information.</param>
    void ExecuteQuery(VVisQueryRequest queryObj)
    {
        SqliteCommand query = new SqliteCommand(queryObj.query, dbConnection);

        Stopwatch diagnosticTime = new Stopwatch();
        diagnosticTime.Start();

        try
        {
            UnityEngine.Debug.Log("Running query...");

            selectedRows.Clear();

            SqliteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                UnityEngine.Debug.Log(String.Format("{0}", reader[1]));
                selectedRows.Add(String.Format("{0}", reader[1]));
            }

        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogWarning("The query could not be completed: " + e.Message);
            queryError = true;
        }
        finally
        {
            diagnosticTime.Stop();

            UnityEngine.Debug.Log("Time needed to run the query (s): " + diagnosticTime.Elapsed.TotalSeconds);

            lastQueryCompleted = true;
        }
    }

    /// <summary>
    /// Auxiliary method for threading.
    /// Opens the SQLite database.
    /// </summary>
    void OpenSQLiteDatabase()
    {
        UnityEngine.Debug.Log("This is the SQL DB path: " + fulldbPath);

        dbConnection = new SqliteConnection("URI=file:" + fulldbPath + "; Version=3;");

        try
        {
            dbConnection.Open();
        }
        catch (InvalidOperationException e)
        {
            UnityEngine.Debug.LogWarning("An exception has been catched and ignored. This is just a workaround for now, not 100% reliable! (by Miguel R. C.)\n Exception message:" + e.Message);
        }

        UnityEngine.Debug.Log("Database loaded and ready");

        dbIsReady = true;
    }

    /// <summary>
    /// Coroutine for callback.
    /// </summary>
    /// <param name="requestor">Receiver of the callback.</param>
    IEnumerator CallbackToRequestor(GameObject requestor)
    {
        while (!lastQueryCompleted)
            yield return new WaitForSeconds(0.1f);

        VVISQueryResults results = new VVISQueryResults(selectedRows, true);
        results.SetAsQueryError(queryError);
        queryError = false;

        requestor.SendMessage("RequestQuery_CallBack", results);
    }

    /// <summary>
    /// Releases the resources when the object containing the script is being destroyed.
    /// </summary>
    void OnDestroy()
    {
        dbConnection.Close();
        if (thread != null)
            thread.Abort();
    }

    /// <summary>
    /// When application quits, aborts the thread and close the db. 
    /// </summary>
    void OnApplicationQuit()
    {
        dbConnection.Close();
        if (thread != null)
            thread.Abort();
    }
}