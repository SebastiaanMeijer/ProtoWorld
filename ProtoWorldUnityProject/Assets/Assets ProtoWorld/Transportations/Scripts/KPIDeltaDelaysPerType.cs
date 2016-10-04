using UnityEngine;
using System.Collections;

public class KPIDeltaDelaysPerType : MonoBehaviour
{
    public int deltaBusDelay = 0;
    public int deltaCarDelay = 0;
    public int deltaTramDelay = 0;

    private KPIDelays delays;

    // Use this for initialization
    void Start()
    {
        delays = transform.GetComponent<KPIDelays>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaBusDelay = getBusDeltaDelay();
        deltaCarDelay = getCarDeltaDelay();
        deltaTramDelay = getTramDeltaDelay();
    }

    private int getTramDeltaDelay()
    {
        return 0;
    }

    private int getCarDeltaDelay()
    {
        return 0;
    }

    private int getBusDeltaDelay()
    {
        return 0;
    }
}