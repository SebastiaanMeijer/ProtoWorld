/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * PEDESTRIANS KTH
 * PedestrianKTHSpawner.cs
 * Miguel Ramos Carretero
 * Edited by Furkan Sonmez
 * 
 */

using UnityEngine;
using System.Collections;
using GaPSLabsUnity.StateMachine;
using Npgsql;

/// <summary>
/// Generates PedestrianKTH objects in the scene and sets its initial parameters.
/// </summary>
public class PedestrianKTHSpawner : MonoBehaviour
{
    [HideInInspector]
    [Range(0, 1000)]
    public int numberOfPedestrians = 1;

    [HideInInspector]
    [Range(0.1f, 1.0f)]
    public float speedVariability;

    [HideInInspector]
    [Range(0.1f, 1.0f)]
    public float studyInterestVariability;

    [HideInInspector]
    [Range(0.1f, 1.0f)]
    public float subscriptionProbability;
  
    [HideInInspector]
    [Range(0.1f, 1.0f)]
    public float rumourSusceptibilityVariability;
  
    [HideInInspector]
    [Range(0.1f, 1.0f)]
    public float delayToleranceVariability;
    
    [HideInInspector]
    [Range(0.1f, 1.0f)]
    public float spawningFrequency = 0.5f;

    [HideInInspector]
    public bool UseCoordinateConversion = true;

    [HideInInspector]
    public bool showFloatingBallons = true;

    [HideInInspector]
    public bool useSpawningDatabase = true;

    [HideInInspector]
    public bool spawnOnlyDefaultModel = false;

    [HideInInspector]
    public bool floatingBalloonsEnabled = true;

    public Transform[] spawningPoints;
    public GameObject[] pedestrianObjects;
    public Queue pedestrianCache = new Queue();

    private int NumberOfPedestriansGenerated = 0;
    private PedestriansKTHConfig conf;

	//Added by Furkan
	public static bool activateBikeStations;
	//Change this in the inspector if you dont want the bikestations system to be active
	public bool activateBikeStationsB = true;


    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        conf = GetComponentInParent<PedestriansKTHConfig>();
        numberOfPedestrians = conf.numberOfPedestrians;
        speedVariability = conf.speedVariability;
        studyInterestVariability = conf.studyInterestVariability;
        subscriptionProbability = conf.subscriptionProbability;
        rumourSusceptibilityVariability = conf.rumourSusceptibilityVariability;
        delayToleranceVariability = conf.delayToleranceVariability;
        spawningFrequency = conf.spawningFrequency;
        UseCoordinateConversion = conf.useCoordinateConversion;
        showFloatingBallons = conf.showFloatingBalloons;
        useSpawningDatabase = conf.useDatabaseForSpawning;
        spawnOnlyDefaultModel = conf.spawnOnlyDefaultModel;

		//Added by Furkan, this will determine wether to activate the bikestations system or not
		activateBikeStations = activateBikeStationsB;
    }

    /// <summary>
    /// Starts the script and spawns the pedestrians according to the information 
    /// of the sql database.
    /// </summary>
    void Start()
    {
        if (useSpawningDatabase)
            InvokeRepeating("GeneratePedestrianFromDB", 1.0f, spawningFrequency);
        else
            InvokeRepeating("GeneratePedestrianFromFixedPoints", 1.0f, spawningFrequency);
    }

    /// <summary>
    /// Generates a pedestrian inside the Unity scene. The position will be set 
    /// according to the data contained in the sql database of pedestrian information.
    /// </summary>
    void GeneratePedestrianFromDB()
    {
        //Establish connection with the sql database
        NpgsqlConnection dbcon = EstablishConnectionWithSqlDB();

        //Create the command to query (RandomPoint)
        NpgsqlCommand dbcmd = CreateSqlCommand(dbcon, "SELECT ST_AsText(RandomPoint(geom, 100)) FROM zones WHERE (zone_txt = 720134::varchar)");

        //Query the command to the database
        NpgsqlDataReader reader = dbcmd.ExecuteReader();

        if (reader.Read())
        {
            //Read the random point
            string point = reader.GetString(0);
            string[] parse = point.Split(' ', '(', ')');
            double lat = double.Parse(parse[1]);
            double lon = double.Parse(parse[2]);

            Vector3 v3;
            if (UseCoordinateConversion)
                v3 = CoordinateConvertor.LatLonToVector3(lon, lat);
            else
                v3 = new Vector3((float)lon, 0.0f, (float)lat);

            //Add an offset to fit the Unity scene
            v3.x += 85.0f;
            v3.z -= 290.0f;

            //Try to find the closest point in the navmesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(v3, out hit, 100.0f, 1 << NavMesh.GetNavMeshLayerFromName("footway")))
            {
                SpawnPedestrian(hit.position);
            }
        }

        //Free the resources
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }

    /// <summary>
    /// Generates a pedestrian inside the Unity scene. The position will be set randomly
    /// from one of the spawning points of the scene (classroom points).
    /// </summary>
    void GeneratePedestrianFromFixedPoints()
    {
        SpawnPedestrian(spawningPoints[Random.Range(0, spawningPoints.Length)].position);
    }

    /// <summary>
    /// Auxiliar private method. Spawns a pedestrian in the given spawning point. 
    /// </summary>
    /// <param name="spawningPoint">Vector3 that determines the spawning point.</param>
    private void SpawnPedestrian(Vector3 spawningPoint)
    {
        GameObject newAgent;

        if (pedestrianCache.Count > 0)
        {
            newAgent = (GameObject)pedestrianCache.Dequeue();
            newAgent.transform.position = spawningPoint;
            newAgent.GetComponent<PedestrianKTHController>().Reset();
        }
        else
        {
            if (spawnOnlyDefaultModel)
                newAgent = (GameObject)Instantiate(pedestrianObjects[0], spawningPoint, Quaternion.identity);
            else
                newAgent = (GameObject)Instantiate(pedestrianObjects[Random.Range(1, pedestrianObjects.Length)], spawningPoint, Quaternion.identity);
            newAgent.transform.SetParent(this.transform, true);
            newAgent.name = "student" + NumberOfPedestriansGenerated;
        }

        PedestrianKTHKnowledge pedKnowledge = newAgent.GetComponent<PedestrianKTHKnowledge>();
        PedestrianKTHSteering pedSteering = newAgent.GetComponent<PedestrianKTHSteering>();

        //Customize the parameters of the pedestrian according to the global variances
        pedKnowledge.studyInterest = Random.Range(0.5f - (studyInterestVariability / 2.0f), 0.5f + (studyInterestVariability / 2.0f));
        pedKnowledge.delayTolerance = Random.Range(0.5f - (delayToleranceVariability / 2.0f), 0.5f + (delayToleranceVariability / 2.0f));
        pedKnowledge.rumourSusceptibility = Random.Range(0.5f - (rumourSusceptibilityVariability / 2.0f), 0.5f + (rumourSusceptibilityVariability / 2.0f));
        pedKnowledge.suscribedToModeDB = Random.value <= subscriptionProbability;
        pedSteering.speedVariation = Random.Range(0.5f - (speedVariability / 2.0f), 0.5f + (speedVariability / 2.0f));

        //According to o/d matrix data (TODO := automatize this with parameters)
        float modeChoice = Random.value;
		//modeChoice = 2;

        //Walk
        if (modeChoice < 0.1128){
            pedKnowledge.mode = 0;
			pedKnowledge.animationStateName = "Walk";
		}

        //Bicycle
        else if (modeChoice < 0.1402)
        {
            pedKnowledge.mode = 3;
            newAgent.transform.FindChild("bike").gameObject.SetActive(true);
			pedKnowledge.animationStateName = "Bicycle";
        }

        //Public transport
        else
        {
            if (Random.value < 0.5){
                pedKnowledge.mode = 1;
				pedKnowledge.animationStateName = "Bus";
			}
            else{
                pedKnowledge.mode = 2;
				pedKnowledge.animationStateName = "Metro";
			}
        }

        newAgent.SetActive(true);

        if (NumberOfPedestriansGenerated++ >= numberOfPedestrians)
            CancelInvoke();
    }

    /// <summary>
    /// Opens a connection to the sql database of pedestrian information.
    /// </summary>
    /// <returns>Handler of the sql connection.</returns>
    private NpgsqlConnection EstablishConnectionWithSqlDB()
    {
        string connectionString =
                        "Server=127.0.0.1;Port=5432;Database=GIS_ODE;User Id=postgres;Password=test;";

        //Request a new sql connection
        NpgsqlConnection dbcon = new NpgsqlConnection(connectionString);
        dbcon.Open();

        return dbcon;
    }

    /// <summary>
    /// Creates a new command to be queried to the sql database of pedestrian information.
    /// </summary>
    /// <param name="dbcon">Handler of the connection to the sql database.</param>
    /// <param name="p">String containing the command to query.</param>
    /// <returns>Handler with the command for the sql connection.</returns>
    private NpgsqlCommand CreateSqlCommand(NpgsqlConnection dbcon, string p)
    {
        NpgsqlCommand dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = p;

        return dbcmd;
    }

    /// <summary>
    /// Toggles the visualization of the floating balloons in pedestrians. 
    /// </summary>
    public void ToogleBalloons()
    {
        floatingBalloonsEnabled = !floatingBalloonsEnabled;

        foreach (var p in gameObject.GetComponentsInChildren<PedestrianKTHController>(true))
            p.ToogleBalloon(floatingBalloonsEnabled);
    }
}
