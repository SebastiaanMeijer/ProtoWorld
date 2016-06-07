using UnityEngine;
using System.Collections;

public class KPIParameters : MonoBehaviour
{
    public bool showPedestrians;
    public bool showArrived;
    public bool showTraveling;
    public bool showQueuing;
    public bool showVehicles;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
