/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * StationInteraction.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the interaction for the station object. One click will select it, two clicks will select it and zoom the camera in.
/// This script must be attached to an object with a collider.
/// </summary>
public class StationInteraction : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    private bool titlelegendsLogged = false;

    GameObject stationUI;
    StationController stationController;
    Camera camera;
    public float zoomFactor = 5.0f;
    public EventButtonPanelController ebpc;

    public Material materialForOpenStation;
    public Material materialForCloseStation;

    FlashFreeMessageWindowController messageWindow;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();

        stationUI = GameObject.Find("StationUI");
        stationController = this.GetComponent<StationController>();
        camera = Camera.main;
        messageWindow = FindObjectOfType<FlashFreeMessageWindowController>();

        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();

        if (stationController != null)
        {
            UpdateStationMaterial(stationController.outOfService);
        }
    }

    /// <summary>
    /// Handler for when the mouse is over the object. 
    /// </summary>
    void OnMouseOver()
    {
        // Left button updates the UI
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject() /*This condition avoids clicks passing through the GUI elements*/)
        {
            camera.GetComponent<CameraControl>().FocusOnHotPoint(this.transform, zoomFactor);

            // Close any other window that is open
            if (messageWindow)
                messageWindow.CloseMessageWindow();

            if (stationUI != null && stationController != null)
            {
                stationUI.GetComponent<FadingElementUI>().fadeOutCanvas();
                Invoke("VinculateToStationUI", 0.5f);

                //LOG STATION LOG INFO
                if (!titlelegendsLogged)
                {
                    log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", stationController.stationName + " clicked log"));
                    log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, stationController.stationName + " checked statistics"));
                    titlelegendsLogged = true;
                }
                // LOG STATION CHECKED
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, stationController.stationName + " checked"));

            }

            if (ebpc != null && stationController != null)
                ebpc.AddEvent(Time.time, "Clicked on " + stationController.stationName, false);

        }
    }

    /// <summary>
    /// Auxiliar method to update the Element UI to display the information of this object.
    /// </summary>
    public void VinculateToStationUI()
    {
        //log.Info("Station " + stationController.stationName + " selected"); FIXME

        FlashStationUIController stationUICtrl = stationUI.transform.GetComponent<FlashStationUIController>();

        //// Display the information of the station in the UI
        stationUICtrl.stationController = stationController;
        stationUICtrl.CloseConfirmationWindow(true); //In case it was open
        stationUICtrl.UpdateStationStateLabels(stationController.outOfService); //Only for Station UI v2.0
        //stationUI.transform.FindChild("CloseStationToggle").GetComponent<UnityEngine.UI.Toggle>().isOn = stationController.outOfService; //Only for Station UI v1.0
        stationUI.transform.FindChild("Title").GetComponent<UnityEngine.UI.Text>().text = stationController.stationName;
        stationUI.transform.FindChild("QueuingValue").GetComponent<UnityEngine.UI.Text>().text = stationController.queuing.ToString();

        // Fade in the station UI
        stationUICtrl.ShowStationUIWindow();
    }

    /// <summary>
    /// Updates the material of the station depending on its state. 
    /// </summary>
    /// <param name="isOutOfService">True if the station is out of service.</param>
    public void UpdateStationMaterial(bool isOutOfService)
    {
        LODGroup lodgroup = this.gameObject.GetComponent<LODGroup>();
        LOD[] lods;

        if (lodgroup != null && materialForOpenStation != null && materialForCloseStation != null)
        {
            lods = lodgroup.GetLODs();

            foreach (LOD L in lods)
            {
                Renderer[] renderers = L.renderers;
                foreach (Renderer R in renderers)
                {
                    if (!isOutOfService)
                        R.material = materialForOpenStation;
                    else
                        R.material = materialForCloseStation;
                }
            }
        }
    }
}
