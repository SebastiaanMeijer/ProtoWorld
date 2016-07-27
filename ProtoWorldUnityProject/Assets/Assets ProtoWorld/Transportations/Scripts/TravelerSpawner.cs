/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TravelerSpawner : MonoBehaviour
{

    private RoutingController routingController;

    [Range(1, 20)]
    public float travelerSpawnFrequency = 1;
    [Range(0, 1000)]
    public int travelersPerSpawn = 20;
    List<Transform> spawnPoints;
    float startTime;

    GameObject spawners;
    GameObject travelers;
    Queue<GameObject> oldTravelers;

    // Use this for initialization
    void Start()
    {
        oldTravelers = new Queue<GameObject>();
        routingController = GetComponentInParent<RoutingController>();
        spawners = GameObject.Find("Spawners");
        travelers = GameObject.Find("Travelers");
        InitTravelerSpawn();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in travelers.transform)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                oldTravelers.Enqueue(child.gameObject);
            }
        }
        if (spawnPoints.Count > 0)
        {
            if (Time.time - startTime > travelerSpawnFrequency)
            {
                SpawnTravelers(travelersPerSpawn, TravelPreference.time);
                startTime = Time.time;
            }
        }
    }

    private void InitTravelerSpawn()
    {
        spawnPoints = new List<Transform>();
        if (spawners != null)
        {
            foreach (Transform child in spawners.transform)
            {
                spawnPoints.Add(child);
            }
            if (spawnPoints.Count < 1)
            {
                Debug.LogWarning("There is no spawnController in the scene!");
            }
        }
    }

    public Vector3 RandomSpawnPoint(Transform transform)
    {
        float radius = 2;
        float x = UnityEngine.Random.Range(-radius, radius);
        float z = UnityEngine.Random.Range(-radius, radius);
        return new Vector3(x, 0, z) + transform.position;
    }

    void SpawnTraveler(int start, int end, TravelPreference preference = TravelPreference.time)
    {
        var itinerary = routingController.GetItinerary(start, end, preference);
        int index = Mathf.FloorToInt(UnityEngine.Random.Range(0, spawnPoints.Count));

        //if (itinerary == null)
        //    return;

        GameObject newTraveler = TravelerController.CreateGameObject(itinerary);
        newTraveler.transform.SetParent(travelers.transform);
        newTraveler.transform.position = RandomSpawnPoint(spawnPoints[index]);
        //Debug.Log(itinerary);
    }

    void SpawnTravelers(int numberOfSpawns, TravelPreference preference)
    {
        for (int i = 0; i < numberOfSpawns; i++)
        {
            int s1 = routingController.GetRandomStationId();
            int s2;
            do
            {
                s2 = routingController.GetRandomStationId();
            } while (s1 == s2);

            SpawnTraveler(s1, s2, preference);
            //SpawnTraveler(5, 11, TravelPreference.transit);
        }
    }
}
