using UnityEngine;
using System.Collections;

public class KPIAvgDelaysPerType : MonoBehaviour
{
    public int avgBusDelay = 0;
    public int avgCarDelay = 0;
    public int avgTramDelay = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        avgBusDelay = getAvgBusDelay();
        avgCarDelay = getAvgCarDelay();
        avgTramDelay = getAvgTramDelay();
    }

    private int getAvgTramDelay()
    {
        return 0;
    }

    private int getAvgCarDelay()
    {
        return 0;
    }

    private int getAvgBusDelay()
    {
        return 0;
    }
}