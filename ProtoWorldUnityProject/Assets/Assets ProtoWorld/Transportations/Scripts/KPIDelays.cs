using UnityEngine;
using System.Collections;

public class KPIDelays : MonoBehaviour
{

    public float TramDelay;
    public float BusDelay;
    public float TrainDelay;
    public float MetroDelay;

    public Transform transLines;     

	// Use this for initialization
	void Start () {
	    transLines = GameObject.Find("TransLines").transform;
    }
	
	// Update is called once per frame
	void Update ()
	{
        UpdateDelays();
	}

    private void UpdateDelays()
    {
        float tmpTramDelay = 0;
        float tmpTrainDelay = 0;
        float tmpBusDelay = 0;
        float tmpMetroDelay = 0;

        foreach (Transform transLine in transLines)
		{
            foreach (Transform item in transLine)
			{
                VehicleController vc = item.GetComponent<VehicleController>();
                if (transLine.name.StartsWith("Tram_"))
                    tmpTramDelay += vc.delay;

                if (transLine.name.StartsWith("Train_"))
                    tmpTrainDelay += vc.delay;

                if (transLine.name.StartsWith("Bus_"))
                    tmpBusDelay += vc.delay;

                if (transLine.name.StartsWith("Metro_"))
                    tmpMetroDelay += vc.delay;
            }         
        }

        TramDelay = tmpTramDelay;
        BusDelay = tmpBusDelay;
        TrainDelay = tmpTrainDelay;
        MetroDelay = tmpMetroDelay;
    }

}
