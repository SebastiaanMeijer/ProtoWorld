/*
 * 
 * CAMERA CONTROL
 * CameraOverviewZoomController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for the UI camera button. when clicked, the camera will zoom out to the given overview point of the scenario. 
/// </summary>
public class CameraOverviewZoomController : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    
    public GameObject overviewPoint;
    
    [Range(0.5f, 10.0f)]
    public float zoomFactor = 1.0f;

    public bool changeViewAtStart = false;

    public EventButtonPanelController ebpc;

    private FlashStationUIController stationUIctrl;
    
    Camera camera;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        camera = Camera.main;
        stationUIctrl = FindObjectOfType<FlashStationUIController>();
    }

    /// <summary>
    /// Starts the script.
    /// </summary>
    void Start()
    {
        log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "Camera zoomed out log"));
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Camera zoomed out statistics"));

        if (overviewPoint != null && changeViewAtStart)
            camera.GetComponent<CameraControl>().FocusOnHotPoint(overviewPoint.transform, zoomFactor);

        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();

    }

    /// <summary>
    /// Changes the view of the camera to the overview position.
    /// </summary>
    public void ChangeView()
    {
        if (overviewPoint != null)
        {
            camera.GetComponent<CameraControl>().FocusOnHotPoint(overviewPoint.transform, zoomFactor);

            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "Zoomed out"));

            if (ebpc != null)
                ebpc.AddEvent(Time.time, "Zoomed out");
        }
    }
}
