/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedSubscribedSliderController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Controller for the slider of the UI canvas that allows the user to change the percentage of pedestrians subscribed.
/// </summary>
public class FlashPedSubscribedSliderController : MonoBehaviour, IPointerUpHandler
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;

    [HideInInspector]
    public FlashPedestriansGlobalParameters globalFlashParam;

    public EventButtonPanelController ebpc;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        //LOG SUBSCRIBER SLIDER LOG INFO
        log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "Subscriber slider log"));

        //LOG SUBSCRIBER SLIDER VALUE CHART INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "chart type", 1, UIChartTypes.Line.ToString()));

        //LOG SUBSCRIBER SLIDER LOG INFO
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Subscriber slider value changed"));
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 1, "Subscriber slider value"));

        globalFlashParam = FindObjectOfType<FlashPedestriansGlobalParameters>();

        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();
    }



    public void UpdatePercentageOfSubscribed(float value)
    {
        if (globalFlashParam != null)
            globalFlashParam.percOfPedSubscribed = value / 100.0f;

        this.transform.FindChild("ValueText").GetComponent<UnityEngine.UI.Text>().text = value.ToString();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //LOG SUBSCRIBER SLIDER ACTION
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "value changed"));
        //LOG SUBSCRIBER SLIDER VALUE
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "float", 1, globalFlashParam.percOfPedSubscribed));

        if (ebpc != null)
            ebpc.AddEvent(Time.time, "% of subscribers set to " + globalFlashParam.percOfPedSubscribed * 100);

    }

}
