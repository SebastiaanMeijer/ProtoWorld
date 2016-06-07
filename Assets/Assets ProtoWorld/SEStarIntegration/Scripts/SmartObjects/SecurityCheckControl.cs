/*
 * 
 * SESTAR INTEGRATION
 * SecurityCheckControl.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controller for the Smart Object "Security Check".
/// </summary>
public class SecurityCheckControl : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Security level in the barrier. 
    /// Range of control: 1 (15s), 2 (30s), 3 (45s), 4 (60s).
    /// </summary>
    public int securityLevel = 1;
    private int oldSecurityControlTime;

    private SEStarSmartObject smartObject;
    private SEStar SEStarControl;
    private Transform blockObject;
    private Vector3 initialScale;

    private float period = 1.0f;
    private float nextActionTime = 0.0f;

    public string kpiManagerName;
    private KPIManager kpiManager;
    private int kpiKey;

    private ListOfInteractiveSmartObjects listOfInteractiveSmartObjects;

    private GameObject elementUI;

    /// <summary>
    /// Initializes the script.
    /// </summary>
    void Start()
    {
        oldSecurityControlTime = securityLevel;

        Debug.Log("Security check object created");

        smartObject = this.GetComponent<SEStarSmartObject>();
        SEStarControl = GameObject.FindObjectOfType<SEStar>();
        elementUI = GameObject.FindGameObjectWithTag("ElementUI");
        listOfInteractiveSmartObjects = GameObject.FindObjectOfType<ListOfInteractiveSmartObjects>();

        //Put the SmartObject inside the list of interactive SmartObjects
        if (!listOfInteractiveSmartObjects.interactiveSmartObjects.ContainsKey(smartObject.objectName))
            GameObject.FindObjectOfType<ListOfInteractiveSmartObjects>().interactiveSmartObjects.Add(smartObject.objectName, this.gameObject);

        var blockObject = transform.FindChild("Block");

        // Try to find the KPI Manager:
        try
        {
            kpiManager = GameObject.Find(kpiManagerName).GetComponent<KPIManager>();
            kpiKey = kpiManager.RegisterNewInformer();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("The KPIManager could not be found: " + e.ToString());
        }

        //Change control duration to the value by default
        ChangeControlDuration(securityLevel);
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            if (securityLevel != oldSecurityControlTime)
                UpdateSecurityControlTime();
        }
    }

    /// <summary>
    /// Changes the control duration of this security check. 
    /// Range of control: 1 (15s), 2 (30s), 3 (45s), 4 (60s).
    /// </summary>
    void ChangeControlDuration(int newLevel)
    {
        //Debug.Log("Security check changing waiting time to " + controlDurationInMs);
        securityLevel = newLevel;
        oldSecurityControlTime = newLevel;

        SEStarControl.ChangeSEStarObjectVariable(smartObject.objectId, smartObject.objectType, "control_duration", newLevel * 15000);

        //Informs the kpi
        kpiManager.InformKPI(kpiKey, newLevel);

        //TODO This method will handle the changes in graphics as well
    }

    /// <summary>
    /// Updates the security control time of this barrier and all the barriers of its same type.
    /// </summary>
    public void UpdateSecurityControlTime()
    {
        oldSecurityControlTime = securityLevel;

        Debug.Log("Updating security control time");

        ChangeControlDuration(securityLevel);
        log.Info("Change in security control time in " + smartObject.objectType + " : " + securityLevel);

        // Change the variable of all the object of the same type.
        foreach (KeyValuePair<uint, GameObject> element in SEStarControl.smartObjects_)
        {
            SEStarSmartObject obj;
            if ((obj = element.Value.GetComponent<SEStarSmartObject>()) != null)
            {
                if (obj.objectName == smartObject.objectName && obj.objectType == smartObject.objectType && obj.objectId != smartObject.objectId)
                {
                    element.Value.GetComponent<SecurityCheckControl>().ChangeControlDuration(securityLevel);
                }
            }
        }
    }

    /// <summary>
    /// Handler for when the mouse is over the object. 
    /// </summary>
    void OnMouseOver()
    {
        // Left button updates the UI
        if (Input.GetMouseButtonDown(0) && elementUI != null)
            VinculateElementUI();
    }


    /// <summary>
    /// Auxiliar method to update the Element UI to display the information of this object.
    /// </summary>
    public void VinculateElementUI()
    {
        log.Info("Security control " + smartObject.objectType + " selected");

        // Display the information of the object in the UI
        elementUI.transform.FindChild("Title").GetComponent<UnityEngine.UI.Text>().text = smartObject.objectName;
        elementUI.transform.FindChild("ParameterTitle").GetComponent<UnityEngine.UI.Text>().text = "Security Control Level:";
        elementUI.transform.FindChild("ValueText").GetComponent<UnityEngine.UI.Text>().text = securityLevel.ToString();

        // Update the slider
        UnityEngine.UI.Slider slider = elementUI.transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>();
        slider.onValueChanged.RemoveAllListeners();
        slider.minValue = 1;
        slider.maxValue = 4;
        slider.value = securityLevel;
        slider.onValueChanged.AddListener(LinkSliderToParameter);

        // Fade in the elementUI
        elementUI.GetComponent<FadingElementUI>().fadeInCanvas();
    }

    /// <summary>
    /// Auxiliar method to link the slider in the element UI with the main parameter of the object.
    /// </summary>
    /// <param name="value"></param>
    public void LinkSliderToParameter(float value)
    {
        securityLevel = (int)value;
        elementUI.transform.FindChild("ValueText").GetComponent<UnityEngine.UI.Text>().text = ((int)value).ToString();
    }
}

