/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashFreeMessageWindowController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for the UI canvas to visualize a window to write free messages during the gameplay (no side effect on simulation). 
/// </summary>
public class FlashFreeMessageWindowController : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    private bool titlelegendsLogged = false;

    public bool raiseAwarenessOnMessageSent = false;

    public GameObject messageWindow;

    [HideInInspector]
    public EventButtonPanelController ebpc;

    FlashStationUIController stationUI;
    FlashPedestriansInformer pedInformer;

    void Awake()
    {
        logSeriesId = LoggerAssembly.GetLogSeriesId();
        stationUI = FindObjectOfType<FlashStationUIController>();
        pedInformer = FindObjectOfType<FlashPedestriansInformer>();

    }

    void Start()
    {
        if (ebpc == null)
            ebpc = FindObjectOfType<EventButtonPanelController>();
    }

    public void OpenMessageWindow()
    {
        //Close first an other window
        if (stationUI != null)
            stationUI.CloseStationUIWindow();

        if (messageWindow != null)
        {
            EnableMessageWindow();
            messageWindow.GetComponent<FadingElementUI>().fadeInCanvas();
        }
    }

    public void CloseMessageWindow(bool fastClosing = false)
    {
        if (messageWindow != null)
        {
            messageWindow.transform.Find("DropdownFreeMessages").GetComponent<UnityEngine.UI.Dropdown>().value = 0;
            messageWindow.transform.Find("TextMessage").GetComponentInChildren<UnityEngine.UI.InputField>().text = "";

            if (fastClosing)
            {
                if (messageWindow.activeInHierarchy)
                {
                    messageWindow.GetComponent<FadingElementUI>().hideCanvasInmediately();
                    DisableMessageWindow();
                }
            }
            else
            {
                if (messageWindow.activeInHierarchy)
                {
                    messageWindow.GetComponent<FadingElementUI>().fadeOutCanvas();
                    Invoke("DisableMessageWindow", 0.5f);
                }
            }
        }
    }

    public void ContinueButtonHandler()
    {
        if (messageWindow != null)
        {
            //LOG STATION STATUS
            if (ebpc != null)
                ebpc.AddEvent(Time.time, "Info message sent to pedestrians");

            //LOG ANNOUNCED MESSAGE
            var message = messageWindow.transform.Find("TextMessage").GetComponentInChildren<UnityEngine.UI.InputField>().text;
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "string", 1, message));

            //Raise more awareness on pedestrians every time a new message is sent
            if (raiseAwarenessOnMessageSent && pedInformer != null)
            {
                if (messageWindow.GetComponentInChildren<FlashMessageDropdownUIController>().GetMessageSelected().highInfluence)
                {
                    pedInformer.globalParam.percOfPedSubscribed += Random.Range(0.03f, 0.05f);
                    pedInformer.InformPedestriansSubscribed();
                }
                else
                {
                    pedInformer.globalParam.percOfPedSubscribed += Random.Range(0.01f, 0.02f);
                    pedInformer.InformPedestriansSubscribed();
                }
            }

            //Close the confirmation windows and clean the message
            CloseButtonHandler();
        }
    }

    public void CloseButtonHandler()
    {
        //GetComponent<FadingElementUI>().fadeOutCanvas();
        CloseMessageWindow(); //In case it was open
    }

    private void EnableMessageWindow()
    {
        if (messageWindow != null)
            messageWindow.SetActive(true);
    }

    private void DisableMessageWindow()
    {
        if (messageWindow != null)
            messageWindow.SetActive(false);
    }
}
