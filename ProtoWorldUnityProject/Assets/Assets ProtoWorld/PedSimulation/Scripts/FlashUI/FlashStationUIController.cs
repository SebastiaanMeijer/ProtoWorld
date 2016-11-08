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
 * FlashStationUIController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for the UI canvas to visualize the parameters of the current station selected and allow the user to control if the station is open or close. 
/// </summary>
public class FlashStationUIController : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    private bool titlelegendsLogged = false;

    public UnityEngine.UI.Text pedQueuing;

    public UnityEngine.UI.Text title;
    public GameObject confirmationWindow;

    [HideInInspector]
    public StationController stationController;

    FlashPedestriansInformer flashInformer;
    FlashPedestriansGlobalParameters globalParam;

    public EventButtonPanelController ebpc;

    FlashFreeMessageWindowController freeMessageWindow;

    private bool isUp = false;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();
        globalParam = FindObjectOfType<FlashPedestriansGlobalParameters>();
        flashInformer = FindObjectOfType<FlashPedestriansInformer>();
        freeMessageWindow = FindObjectOfType<FlashFreeMessageWindowController>();
    }

    void Start()
    {
        MoveStationUIUp();

        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();
    }

    void Update()
    {
        if (stationController != null && pedQueuing != null && globalParam != null)
        {
            //LOG STATION LOG INFO
            if (!titlelegendsLogged)
            {
                log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", stationController.stationName + " log"));
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, stationController.stationName + " status"));
                log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 1, stationController.stationName + " messages"));
                titlelegendsLogged = true;
            }

            pedQueuing.text = (stationController.queuing * globalParam.numberOfPedestriansPerAgent).ToString();
        }
    }

    public void ChangeStationState(bool value)
    {
        if (stationController != null)
        {
            // Get the routing controller from the grandfather of stationController and closes the station
            stationController.GetComponentInParent<RoutingController>().SetStationOutOfService(stationController, value);
            flashInformer.InformPedestriansSubscribed();
            stationController.SendMessage("UpdateStationMaterial", value);
        }
    }

    public void UpdateStationStateLabels(bool value)
    {
        this.transform.Find("StateOpen").gameObject.SetActive(!value);
        this.transform.Find("StateClose").gameObject.SetActive(value);
        this.transform.Find("StationStateButton").GetComponentInChildren<UnityEngine.UI.Text>().text = (value ? "Open" : "Close") + " station";
    }

    public void OpenConfirmationWindow()
    {
        // Close any other window that is open
        if (freeMessageWindow)
            freeMessageWindow.CloseMessageWindow();

        if (stationController != null && confirmationWindow != null && title != null)
        {
            EnableConfirmationWindow();

            if (!stationController.outOfService)
            {
                // Requesting confirmation for closing station
                confirmationWindow.transform.Find("ActionDescription").GetComponent<UnityEngine.UI.Text>().text = "Closing station " + title.text;
                confirmationWindow.transform.Find("ClosingDropdownMessages").gameObject.SetActive(true);
                confirmationWindow.transform.Find("OpeningDropdownMessages").gameObject.SetActive(false);
            }
            else
            {
                // Requesting confirmation for opening station
                confirmationWindow.transform.Find("ActionDescription").GetComponent<UnityEngine.UI.Text>().text = "Opening station " + title.text;
                confirmationWindow.transform.Find("ClosingDropdownMessages").gameObject.SetActive(false);
                confirmationWindow.transform.Find("OpeningDropdownMessages").gameObject.SetActive(true);
            }

            confirmationWindow.GetComponent<FadingElementUI>().fadeInCanvas();
        }
    }

    public void CloseConfirmationWindow(bool fastClosing = false)
    {
        if (confirmationWindow != null)
        {
            confirmationWindow.transform.Find("ClosingDropdownMessages").GetComponent<UnityEngine.UI.Dropdown>().value = 0;
            confirmationWindow.transform.Find("OpeningDropdownMessages").GetComponent<UnityEngine.UI.Dropdown>().value = 0;
            confirmationWindow.transform.Find("TextMessage").GetComponentInChildren<UnityEngine.UI.InputField>().text = "";

            if (fastClosing)
            {
                if (confirmationWindow.activeInHierarchy)
                {
                    confirmationWindow.GetComponent<FadingElementUI>().hideCanvasInmediately();
                    DisableConfirmationWindow();
                }
            }
            else
            {
                if (confirmationWindow.activeInHierarchy)
                {
                    confirmationWindow.GetComponent<FadingElementUI>().fadeOutCanvas();
                    Invoke("DisableConfirmationWindow", 0.5f);
                }
            }
        }
    }

    public void ContinueConfirmationWindow()
    {
        if (stationController != null && confirmationWindow != null)
        {
            bool closingStation = !stationController.outOfService;
            ChangeStationState(closingStation);
            UpdateStationStateLabels(closingStation);

            //LOG STATION STATUS
            var status = (stationController.outOfService) ? " closed" : " opened";
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "action", 0, status));

            if (ebpc != null && stationController != null)
                ebpc.AddEvent(Time.time, stationController.stationName + status);

            //LOG ANNOUNCED MESSAGE
            var message = confirmationWindow.transform.Find("TextMessage").GetComponentInChildren<UnityEngine.UI.InputField>().text;
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "string", 1, message));

            // Close the confirmation windows and clean the message
            CloseConfirmationWindow();
        }
    }

    internal void ShowStationUIWindow()
    {
        GetComponent<FadingElementUI>().fadeInCanvas();
        MoveStationUIDown();
    }

    public void CloseStationUIWindow()
    {
        GetComponent<FadingElementUI>().fadeOutCanvas();
        CloseConfirmationWindow(true); //In case it was open
        Invoke("MoveStationUIUp", 1.0f);
    }

    private void MoveStationUIUp()
    {
        RectTransform transf = this.gameObject.GetComponent<RectTransform>();

        if (!isUp)
        {
            transf.position = new Vector3(transf.position.x, transf.position.y + 200, transf.position.z);
            isUp = true;
        }
    }

    private void MoveStationUIDown()
    {
        RectTransform transf = this.gameObject.GetComponent<RectTransform>();

        if (isUp)
        {
            transf.position = new Vector3(transf.position.x, transf.position.y - 200, transf.position.z);
            isUp = false;
        }
    }

    private void EnableConfirmationWindow()
    {
        if (confirmationWindow != null)
            confirmationWindow.SetActive(true);
    }

    private void DisableConfirmationWindow()
    {
        if (confirmationWindow != null)
            confirmationWindow.SetActive(false);
    }
}
