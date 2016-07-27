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
 * VVisFixPointsSQLite.cs
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
/// Script that loads and handles VVIS fix points from an SQLite DB.
/// </summary>
public class VVisFixPointsSQLite : MonoBehaviour
{
    public string dbPathFromStreamingAsset = "/SQLiteDB/pointsDB";
    public GameObject pointObject;

    public bool loadPointsInGameTime = true;
    public bool hidePointsAfterLoading = true;

    private string fulldbPath = "";

    private SqliteConnection dbConnection;
    private Thread thread;

    // Point format: [lat, lon, count].
    private List<Vector3> points = new List<Vector3>();

    private bool pointsReadyToBeDrawn = false;

    private GameObject pointHolder;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {       
        if (loadPointsInGameTime)
            AddPointsFromDB();
    }

    /// <summary>
    /// Add fix points on the map from the SQLite DB. 
    /// </summary>
    public void AddPointsFromDB()
    {
        DeletePoints();

        pointHolder = new GameObject("Points");

        StartCoroutine(DrawPoints());

        fulldbPath = Application.streamingAssetsPath + dbPathFromStreamingAsset;

        thread = new Thread(new ThreadStart(GetPointsFromSQLiteDatabase));
        thread.Start();

        // Is this reliable as an alternative to coroutine?
        IEnumerator e = DrawPoints();
        while (e.MoveNext()) ;
    }

    /// <summary>
    /// Remove fix points from the map. 
    /// </summary>
    public void DeletePoints()
    {
        var fixPoints = GameObject.Find("Points");

        if (fixPoints != null)
            GameObject.DestroyImmediate(fixPoints);

        points.Clear();
        pointsReadyToBeDrawn = false;
    }

    /// <summary>
    /// Auxiliary method for threading.
    /// Gets the fix points from the SQLite database.
    /// </summary>
    void GetPointsFromSQLiteDatabase()
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

        UnityEngine.Debug.Log("Database with fix points ready to use");

        VVisQueryRequest request = new VVisQueryRequest("pointRequest", "select * from points order by no", "");

        SqliteCommand query = new SqliteCommand(request.query, dbConnection);

        Stopwatch diagnosticTime = new Stopwatch();
        diagnosticTime.Start();

        try
        {
            UnityEngine.Debug.Log("Running query...");

            points.Clear();

            SqliteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                UnityEngine.Debug.Log(String.Format("{0}, {1}, {2}", reader["lat"], reader["long"], reader["count"]));

                Vector3 v = new Vector3(
                    float.Parse(reader["lat"].ToString()),
                    float.Parse(reader["long"].ToString()),
                    int.Parse(reader["count"].ToString()));

                points.Add(v);
            }

            pointsReadyToBeDrawn = true;
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogWarning("The query could not be completed: " + e.Message);
        }
        finally
        {
            diagnosticTime.Stop();

            UnityEngine.Debug.Log("Time needed to run the query (s): " + diagnosticTime.Elapsed.TotalSeconds);
        }

        dbConnection.Close();
    }

    /// <summary>
    /// Coroutine for drawing the points in the map.
    /// </summary>
    IEnumerator DrawPoints()
    {
        while (!pointsReadyToBeDrawn)
            yield return new WaitForSeconds(0.1f);

        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        CoordinateConvertor.Initialize(client, FindObjectOfType<MapBoundaries>());

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 V = points[i];
            Vector3 position = CoordinateConvertor.LatLonToVector3(V.x, V.y);

            var newPoint = Instantiate(pointObject, position, Quaternion.identity) as GameObject;

            newPoint.name = "fixPoint" + i;
            newPoint.transform.parent = pointHolder.transform;

            var pointInfo = newPoint.GetComponent<VVisFixPointInfo>();

            if (pointInfo != null)
            {
                List<string> info = new List<string>();
                info.Add("count: " + V.z.ToString());
                pointInfo.StorePointInfo(info);
            }

            newPoint.SetActive(true);
        }

        if (hidePointsAfterLoading)
            HideLoadedPoints();

        pointsReadyToBeDrawn = false;
    }

    /// <summary>
    /// Hides the points.
    /// </summary>
    internal void HideLoadedPoints()
    {
        pointHolder.SetActive(false);
    }

    /// <summary>
    /// Shows the points.
    /// </summary>
    internal void ShowLoadedPoints()
    {
        pointHolder.SetActive(true);
    }
}
