/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedestriansCounter.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class FlashPedestriansCounter : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;

    [HideInInspector]
    public FlashPedestriansSpawner[] spawners;

    [Range(0, 10)]
    public float refreshFrequencyInSeconds = 1.0f;

    private float nextUpdate = 0.0f;
    public int counter;
    private UnityEngine.UI.Text text;

    FlashPedestriansGlobalParameters globalParam;

    LoggerAssembly logger;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();
        globalParam = FindObjectOfType<FlashPedestriansGlobalParameters>();
        spawners = FindObjectsOfType<FlashPedestriansSpawner>();
    }

    // Start script
    void Start()
    {
        if (logger != null && logger.logPedestrians)
        {
            Debug.Log("Logging pedestrians");
            //LOG PEDESTRIAN COUNT LOG INFO
            log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "Pedestrian count log"));
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Pedestrian count"));

            //LOG PEDESTRIAN COUNT CHART INFO
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 0, UIChartTypes.Line.ToString()));
        }
        text = this.gameObject.GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nextUpdate < Time.time)
        {
            nextUpdate += refreshFrequencyInSeconds;

            counter = 0;

            foreach (FlashPedestriansSpawner S in spawners)
            {
                counter += S.numberOfPedestriansGenerated - S.numberOfPedestriansOnDestination;
            }

            // Scale the number of pedestrians
            counter *= globalParam.numberOfPedestriansPerAgent;

            text.text = counter.ToString();

            if (logger != null && logger.logPedestrians)
            {
                //LOG PEDESTRIAN COUNT
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "int", 0, counter));
            }
        }
    }
}
