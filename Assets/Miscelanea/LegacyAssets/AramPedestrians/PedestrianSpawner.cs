using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PedestrianSpawner : MonoBehaviour
{

    public Transform ObjectToSpawn;
    public int NumberOfSpawns = 10;
    public GameObject[] Roads;
    [Compact]
    public Vector2 SpeedRange = new Vector2(3, 5);
    public List<GameObject> RoadsTempList;
    //public List<int> SpawnPoints;
    public static System.Random random;
    public SimulationRecorder PedestrianRecorder;
    public bool RecordPedestrianSimulation = true;
    void Start()
    {
        PedestrianRecorder = GameObject.FindObjectOfType(typeof(SimulationRecorder)) as SimulationRecorder;
        random = new System.Random(System.DateTime.Now.Millisecond);
        Random.seed = System.DateTime.Now.Millisecond;
        //SpawnPoints = new List<int>();
        Roads = GameObject.FindGameObjectsWithTag(GlobalConstants.Tags.Line)
            .Where(i =>
                i.name.Contains("footway") ||
                i.name.Contains("cycleway") ||
                i.name.Contains("steps") ||
                i.name.Contains("pedestrian")).ToArray();
        Roads = Roads.OrderBy(i => Vector3.Distance(Vector3.zero, i.transform.position)).ToArray();
        RoadsTempList = Roads.ToList();
        StartCoroutine(Spawn(NumberOfSpawns));
        //Spawn(NumberOfSpawns);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Spawn(int TotalSpawns)
    {
        for (int i = 0; i < TotalSpawns; i++)
        {
            if (RoadsTempList.Count == 0)
                break;

            var tmp = random.Next(0, RoadsTempList.Count - 1);

            NavMeshHit hit;
            NavMesh.SamplePosition(RoadsTempList[tmp].transform.position, out hit, 50, 1);
            var g = (Transform)GameObject.Instantiate(ObjectToSpawn, hit.position, Quaternion.identity);
            g.GetComponent<NavMeshAgent>().speed = Random.Range(SpeedRange.x, SpeedRange.y);
            g.name = "SpawnObject" + i;

            RoadsTempList.RemoveAt(tmp);
            //RoadsTempList.Where(i=>i.transform.position== Roads[tmp].transform.position)
            if (i % 10 == 0)
                yield return new WaitForSeconds(0.05f);
        }
        if (RecordPedestrianSimulation)
        {
            PedestrianRecorder.Pedestrians =
                   GameObject.FindGameObjectsWithTag(GlobalConstants.AI_Tags.Pedestrian)
                   .Select(i => i.GetComponent<NavMeshAgent>()).ToArray();
            PedestrianRecorder.StartRecording();
        }

    }
    public void SpawnAt(Vector3 Position)
    { }
}
