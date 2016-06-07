using UnityEngine;
using System.Collections;

public class TrainMovement : MonoBehaviour
{

    public TimeController timeController;

    public GameObject[] WayPoints;
    public GameObject currentWayPoint;

    public float trainSpeed = 50;
    public float realMoveSpeed = 55;
    public int currentWayPointNr = 0;
    public Vector3 offSet;
    public bool atStation = false;
    public bool stationReached = false;

    public float trainStartStopTime = 0f;
    public float trainStopTimer = 0f;
    public float totalTrainWaitTime = 30f;
    public float closeCrossRoadBeforeTime = 10f;

    public float previousTime;
    public float thisTime;
    public float gameTimeDeltaTime;
    public float realDeltaTime;


    public string fromStation;


    public static int crossRoadClosed = 0;


    // Use this for initialization
    void Start()
    {
        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();

        WayPoints = GameObject.Find("SpawnPointEndPointArnhem").GetComponent<TrainSpawnDelete>().ToDZfromA;
    }

    // Update is called once per frame
    void Update()
    {

        if (WayPoints[currentWayPointNr].name == "Waypoint1 (16)" && crossRoadClosed == 0)
        {
            crossRoadClosed = 1;
            //Debug.Log("CROSSROAD CLOSED");
        }

        if (currentWayPointNr < 1 && stationReached)
            Destroy(this);

        currentWayPoint = WayPoints[currentWayPointNr];
        if (atStation)
        {
            if (timeController.gameTime - trainStartStopTime > totalTrainWaitTime - closeCrossRoadBeforeTime)
            {
                crossRoadClosed = 1;
                //Debug.Log("CROSSROAD CLOSED");
            }



            if (timeController.gameTime - trainStartStopTime > totalTrainWaitTime)
            {
                //Debug.Log("Leave station now");
                atStation = false;
                stationReached = true;
                if (TrainSpawnDelete.TrainScenarioNumber == 1)
                {
                    currentWayPointNr = currentWayPointNr - 1;
                }
                if (TrainSpawnDelete.TrainScenarioNumber == 2)
                {
                    currentWayPointNr = currentWayPointNr - 1;
                }
                if (TrainSpawnDelete.TrainScenarioNumber == 3)
                {
                    currentWayPointNr = currentWayPointNr + 1;
                }
            }

        }



        //if(currentWayPointNr >= WayPoints.Length - 1)
        //    Destroy(this);

        this.transform.rotation = WayPoints[currentWayPointNr].transform.rotation;

        thisTime = timeController.gameTime;


        gameTimeDeltaTime = thisTime - previousTime;
        //realDeltaTime = Time.deltaTime;

        previousTime = timeController.gameTime;
        float newSpeed = trainSpeed * gameTimeDeltaTime;

        transform.position = Vector3.MoveTowards(transform.position, WayPoints[currentWayPointNr].transform.position + offSet, newSpeed);
        /*
        realMoveSpeed = trainSpeed * timeController.timeVelocity;
        if(timeController.IsPaused() == false){
        float step = realMoveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, WayPoints[currentWayPointNr].transform.position + offSet, step);
        }
        */

        if (Vector3.Distance(transform.position, WayPoints[currentWayPointNr].transform.position) < 5 && atStation == false)
        {

            if (WayPoints[currentWayPointNr].gameObject.tag == "TrainStation" && atStation == false)
            {
                Debug.Log("ARRIVED AT STATION!");
                atStation = true;
                trainStartStopTime = timeController.gameTime;
                if (WayPoints[currentWayPointNr].gameObject.name == "StationDriebergenZeistFUtrecht")
                    fromStation = "FromUtrecht";
                else
                    fromStation = "FromArnhem";
            }


            if (atStation == false)
            {
                if (TrainSpawnDelete.TrainScenarioNumber == 1 && stationReached)
                {
                    currentWayPointNr = currentWayPointNr - 1;
                }
                else if (TrainSpawnDelete.TrainScenarioNumber == 2 && stationReached)
                {
                    currentWayPointNr = currentWayPointNr - 1;
                }
                else if (TrainSpawnDelete.TrainScenarioNumber == 3 && stationReached)
                {
                    currentWayPointNr = currentWayPointNr + 1;
                }
                else
                {
                    currentWayPointNr = currentWayPointNr + 1;
                }
            }
        }

    }
}