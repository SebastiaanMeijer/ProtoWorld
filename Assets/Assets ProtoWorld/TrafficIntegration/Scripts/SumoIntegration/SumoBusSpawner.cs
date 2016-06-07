/*
 * 
 * SUMO COMMUNICATION
 * SumoBusSpawner.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the spawning of new buses into the SUMO simulation.
/// </summary>
public class SumoBusSpawner : MonoBehaviour
{
    [Range(1, 60)]
    public float spawningFrequencyInMinutes = 15;
    public bool spawnBusesAtTheBeginning = true;

    public int numberOfBusesPerSpawningIteration = 4;

    public SumoBusParam[] sumoBus;

    private float nextUpdateInMinutes;
    private SumoMainController sumoControl;
    private TimeController timeControl;
    private TrafficIntegrationBusUIController busUIControl;
    private int nextIdNumber = 0;

    [System.Serializable] 
    public struct SumoBusParam
    {
        public string vehId;
        public string vehType;
        public string routeId;
        public double departPosition;
        public double departSpeed;
        public byte departLane;
    }

    void Awake()
    {
        busUIControl = FindObjectOfType<TrafficIntegrationBusUIController>(); 
        timeControl = FindObjectOfType<TimeController>();
        sumoControl = FindObjectOfType<SumoMainController>();
        nextUpdateInMinutes = spawningFrequencyInMinutes;
    }

    void Start()
    {
        if (spawnBusesAtTheBeginning)
        {
            for (int i = 0; i < numberOfBusesPerSpawningIteration; i++)
                Invoke("SpawnNewBus", 5.0f);
        }
    }

    void Update()
    {
        if (timeControl.gameTime > nextUpdateInMinutes * 60)
        {
            for (int i = 0; i < numberOfBusesPerSpawningIteration; i++)
                SpawnNewBus();

            nextUpdateInMinutes += spawningFrequencyInMinutes;
        }
    }

    public void ChangeSpawningFrequency(float value)
    {
        float timeOfLastSpawning = (Mathf.FloorToInt(nextUpdateInMinutes / spawningFrequencyInMinutes) - 1) * spawningFrequencyInMinutes;
        Debug.Log("Time of last spawning: " + timeOfLastSpawning);

        nextUpdateInMinutes = timeOfLastSpawning + value;
        Debug.Log("Next update in minutes: " + nextUpdateInMinutes);

        if (nextUpdateInMinutes * 60 < timeControl.gameTime)
            nextUpdateInMinutes = timeControl.gameTime / 60.0f;

        spawningFrequencyInMinutes = value;
    }

    public void SpawnNewBus()
    {
        // Select the bus to spawn depending on the selected option in the BusUIController
        int i = busUIControl.GetCurrentOptionActive() - 1;

        // Spawn the new bus in SUMO
        sumoControl.AddNewVehicle(sumoBus[i].vehId + "_" + nextIdNumber++, sumoBus[i].vehType, sumoBus[i].routeId, -2, sumoBus[i].departPosition, sumoBus[i].departSpeed, sumoBus[i].departLane);
    }
}
