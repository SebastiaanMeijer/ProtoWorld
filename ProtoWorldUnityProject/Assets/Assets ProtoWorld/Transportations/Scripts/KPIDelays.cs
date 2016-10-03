using UnityEngine;
using System.Collections;

public class KPIDelays : MonoBehaviour
{

    public float tramDelay;
    public Transform tramTransform;     

	// Use this for initialization
	void Start () {
	    tramTransform = GameObject.Find("Tram_KTH Express").transform;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    tramDelay = getTramDelay();
	}

    private float getTramDelay()
    {
        float delay = 0;
        foreach (Transform tram in tramTransform)
        {
            VehicleController vc = tram.GetComponent<VehicleController>();
            delay += vc.delay;
        }
        return delay;
    }

}
