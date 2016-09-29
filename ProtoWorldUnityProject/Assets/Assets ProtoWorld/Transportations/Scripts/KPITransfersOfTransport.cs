using UnityEngine;
using System.Collections;

public class KPITransfersOfTransport : MonoBehaviour
{
    //Rome and Venice
    public int pedestriansIn = 0;
    public int pedestriansOut = 0;

    //Rome and Hiafa
    public int busIn = 0;
    public int busOut = 0;

    //Rome and Hiafa
    public int carIn = 0;
    public int carOut = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //I guess this isn't the right approach tho, also we need some sort of avg value
        //Or keep the value on the same value for the time unit period.
        //Or, as a noticed in KPIAdditionalData script, create combo methods.
        //As long we don't execute the same loop 10 times, that would be silly :D

        pedestriansOut = getPedestriansOut();
        pedestriansIn = getPedestriansIn();

        busIn = getBusIn();
        busOut = getBusOut();

        carIn = getCarIn();
        carOut = getCarOut();
    }

    private int getCarOut()
    {
        return 0;
    }

    private int getCarIn()
    {
        return 0;
    }

    private int getBusOut()
    {
        return 0;
    }

    private int getBusIn()
    {
        return 0;
    }

    private int getPedestriansIn()
    {
        return 0;
    }

    private int getPedestriansOut()
    {
        return 0;
    }
}