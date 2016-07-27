using UnityEngine;
using System.Collections;

public class TrainSpawnDelete : MonoBehaviour
{

    public TimeController timeController;
    public UnityEngine.UI.Text timeRemainingText;

    public GameObject trainToSpawn;
    public bool stationActive;
    public string StationName;

    public float countSpeed = 1f;
    public int batchNumber = 0;
    public int trainEstBatchN = -1;
    public float realSecond = 1f;
    public float trainTimer = 0f;
    public float trainEstTimer = 0f;
    public float timeToNewTrain = 60f;
    public float trainEstArrTime = 0f;

    public static int TrainScenarioNumber = 1;

    public GameObject[] ToUtrecht;
    public GameObject[] ToDZfromA;
    public GameObject[] ToDZfromU;

    void Awake()
    {
        if (timeController == null)
            timeController = FindObjectOfType<TimeController>();
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(addNewTrain());
        //StartCoroutine(counting());
    }

    // Update is called once per frame
    void Update()
    {
        trainTimer = timeController.gameTime - batchNumber * timeToNewTrain;
        trainEstTimer = timeController.gameTime - trainEstBatchN * timeToNewTrain;

        realSecond = countSpeed * timeController.timeVelocity;
        if (timeController.IsPaused() == false)
        {
            //	realSecond = realCountSpeed * Time.deltaTime;
        }

        if (trainTimer >= timeToNewTrain)
        {
            StartCoroutine(addNewTrain());
            trainTimer = 0f;
            batchNumber = batchNumber + 1;
        }

        trainEstArrTime = timeToNewTrain - timeController.gameTime + batchNumber * timeToNewTrain;

        int hours = Mathf.FloorToInt(trainEstArrTime / 3600F);
        int minutes = Mathf.FloorToInt(trainEstArrTime / 60F - hours * 60);
        int seconds = Mathf.FloorToInt(trainEstArrTime % 60);

        if (timeRemainingText != null)
            timeRemainingText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

    }

    public IEnumerator counting()
    {
        yield return new WaitForSeconds(1);
        trainTimer = trainTimer + realSecond;
        StartCoroutine(counting());
    }


    public IEnumerator addNewTrain()
    {
        yield return new WaitForSeconds(1);
        if (stationActive == true)
        {
            Instantiate(trainToSpawn, this.transform.position, this.transform.rotation);
        }
    }
}
