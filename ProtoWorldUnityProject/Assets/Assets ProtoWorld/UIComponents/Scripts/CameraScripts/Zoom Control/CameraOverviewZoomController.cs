/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
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
    private LoggerAssembly loggerAssembly;

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
        loggerAssembly = FindObjectOfType<LoggerAssembly>();
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        camera = Camera.main;
        stationUIctrl = FindObjectOfType<FlashStationUIController>();
    }

    /// <summary>
    /// Starts the script.
    /// </summary>
    void Start()
    {
        if (loggerAssembly != null && loggerAssembly.logCameraChanges)
        {
            log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", "Camera zoomed out log"));
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Camera zoomed out statistics"));
        }

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

            if (loggerAssembly != null && loggerAssembly.logCameraChanges)
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, "Zoomed out"));

            if (ebpc != null)
                ebpc.AddEvent(Time.time, "Zoomed out");
        }
    }
}
