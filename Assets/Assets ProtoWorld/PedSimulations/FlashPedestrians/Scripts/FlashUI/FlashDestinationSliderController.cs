/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashDestinationSliderController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Controller for the slider of the UI canvas that allows the user to change the priority in a destination.
/// </summary>
public class FlashDestinationSliderController : MonoBehaviour, IPointerUpHandler
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    private static int destinationSliderCounter = 0;

    private int mySliderId;

    [HideInInspector]
    public FlashPedestriansDestination destination;

    public EventButtonPanelController ebpc;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        //LOG DESTIONATION PRIORITY CHART INFO
        mySliderId = destinationSliderCounter++;
    }

    void Start()
    {
        //LOG DESTIONATION PRIORITY LOG INFO
        log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", destination.destinationName + " priority log"));

        //LOG DESTIONATION PRIORITY CHART INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", mySliderId, UIChartTypes.Line.ToString()));

        //LOG DESTIONATION PRIORITY LOG INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", mySliderId, destination.destinationName + " priority"));

        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();
    }

    public void UpdateDestinationPriority(float value)
    {
        UnityEngine.UI.Text valueText = this.transform.FindChild("ValueText").GetComponent<UnityEngine.UI.Text>();
        destination.destinationPriority = value;
        valueText.text = value.ToString();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //LOG DESTIONATION PRIORITY
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "float", mySliderId, destination.destinationPriority));

        if (ebpc != null)
            ebpc.AddEvent(Time.time, "Priority to " + destination.destinationName + " set to " + destination.destinationPriority);

    }
}
