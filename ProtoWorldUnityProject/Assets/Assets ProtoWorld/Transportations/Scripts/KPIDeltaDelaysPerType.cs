using UnityEngine;
using System.Collections;

public class KPIDeltaDelaysPerType : MonoBehaviour
{
    public int deltaBusDelay = 0;
    public int deltaCarDelay = 0;
    public int deltaTramDelay = 0;

    // Use this for initialization
    void Start()
    {
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