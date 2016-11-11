using UnityEngine;
using System.Collections;

public class KPIDeltaDelaysPerType : MonoBehaviour
{
    public float deltaBusDelay = 0;
    public float deltaCarDelay = 0;
    public float deltaTramDelay = 0;
    public float deltaTrainDelay = 0;
    public float deltaMetroDelay = 0;

    private KPIDelays delays;
    private Transform transLines;

    private float oldBusDelay = 0;
    private float oldTramDelay = 0;
    private float oldTrainDelay = 0;
    private float oldMetroDelay = 0;

    private float timeout = 0;

    // Use this for initialization
    void Start()
    {
        delays = transform.GetComponent<KPIDelays>();
        if (delays != null && delays.transLines != null)
            transLines = delays.transLines;
        else
            transLines = GameObject.Find("TransLines").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //run only once per second, otherwise the delays are not noticable
        timeout += Time.deltaTime;
        if (timeout >= 1)
        {
            deltaCarDelay = getCarDeltaDelay();
            getPublicTransportDeltaDelay();
            timeout--;
        }
    }

    private float getCarDeltaDelay()
    {
        return 0;
    }

    private void getPublicTransportDeltaDelay()
    {
        deltaBusDelay = delays.BusDelay - oldBusDelay;
        deltaTramDelay = delays.TramDelay - oldTramDelay;
        deltaTrainDelay = delays.TrainDelay - oldTrainDelay;
        deltaMetroDelay = delays.MetroDelay - oldMetroDelay;

        oldBusDelay = delays.BusDelay;
        oldTramDelay = delays.TramDelay;
        oldTrainDelay = delays.TrainDelay;
        oldMetroDelay = delays.MetroDelay;
    }
}