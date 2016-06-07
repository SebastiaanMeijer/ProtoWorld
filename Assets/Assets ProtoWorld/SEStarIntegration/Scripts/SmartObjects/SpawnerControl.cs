/*
 * 
 * SESTAR INTEGRATION
 * SpawnerControl.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controller for the Smart Object "Spawner".
/// </summary>
public class SpawnerControl : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public int spawnerFrequency = 5;
    private int oldSpawnerFrequency;

    private SEStarSmartObject smartObject;
    private SEStar SEStarControl;

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
        oldSpawnerFrequency = spawnerFrequency;

        Debug.Log("Spawner created");

        smartObject = this.GetComponent<SEStarSmartObject>();
        SEStarControl = GameObject.FindObjectOfType<SEStar>();
        elementUI = GameObject.FindGameObjectWithTag("ElementUI");
        listOfInteractiveSmartObjects = GameObject.FindObjectOfType<ListOfInteractiveSmartObjects>();

        //Put the SmartObject inside the list of interactive SmartObjects
        if (!listOfInteractiveSmartObjects.interactiveSmartObjects.ContainsKey(smartObject.objectName))
            GameObject.FindObjectOfType<ListOfInteractiveSmartObjects>().interactiveSmartObjects.Add(smartObject.objectName, this.gameObject);

        // Treat the metro station cases
        if (smartObject.objectName.StartsWith("Spawner_Porte_de_St_Cloud") || smartObject.objectName.StartsWith("Spawner_Porte_d_Auteuil"))
        {
            // Transform the object to place it in the scene properly
            Vector3 pos = this.transform.position;
            this.transform.position = new Vector3(pos.x, pos.y + 5.0f, pos.z);

            // 60 pedestrians spawned per minute by default
            SEStarControl.ChangeSEStarObjectVariable(smartObject.objectId, smartObject.objectType, "agents_per_minute", 60);

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

            // Set the spawner with a default value
            ChangeSpawnerFrequency(spawnerFrequency);
        }
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            if (spawnerFrequency != oldSpawnerFrequency)
                UpdateSpawnerFrequency();
        }
    }

    /// <summary>
    /// Changes the spawner frequency of this spawner. 
    /// </summary>
    void ChangeSpawnerFrequency(int newFrequency)
    {
        spawnerFrequency = newFrequency;
        oldSpawnerFrequency = newFrequency;

        SEStarControl.ChangeSEStarObjectVariable(smartObject.objectId, smartObject.objectType, "time_between_trains", newFrequency * 60000);

        //Informs the kpi
        kpiManager.InformKPI(kpiKey, (60.0f / (float)newFrequency));
    }

    /// <summary>
    /// Updates the spawner frequency of this spawner and all the spawners of its same type.
    /// </summary>
    void UpdateSpawnerFrequency()
    {
        oldSpawnerFrequency = spawnerFrequency;

        Debug.Log("Updating spawner frequency");

        ChangeSpawnerFrequency(spawnerFrequency);
        log.Info("Change in spawner frequency in " + smartObject.objectType + " : " + spawnerFrequency);

        // Change the variable of all the object of the same type.
        foreach (KeyValuePair<uint, GameObject> element in SEStarControl.smartObjects_)
        {
            SEStarSmartObject obj;
            if ((obj = element.Value.GetComponent<SEStarSmartObject>()) != null)
            {
                if (obj.objectName.StartsWith(smartObject.objectName.Substring(0, 20)) && obj.objectType == smartObject.objectType && obj.objectId != smartObject.objectId)
                {
                    element.Value.GetComponent<SpawnerControl>().ChangeSpawnerFrequency(spawnerFrequency);
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

        // Display the title (especial parsing for the Paris Case)
        if (smartObject.objectName.Contains("Cloud"))
            elementUI.transform.FindChild("Title").GetComponent<UnityEngine.UI.Text>().text = "Porte de Saint Cloud";
        else if (smartObject.objectName.Contains("Auteuil"))
            elementUI.transform.FindChild("Title").GetComponent<UnityEngine.UI.Text>().text = "Porte d'Auteuil";
        else 
            elementUI.transform.FindChild("Title").GetComponent<UnityEngine.UI.Text>().text = smartObject.objectName;

        // Display the information of the object in the UI
        elementUI.transform.FindChild("ParameterTitle").GetComponent<UnityEngine.UI.Text>().text = "Train frequency (min)";
        elementUI.transform.FindChild("ValueText").GetComponent<UnityEngine.UI.Text>().text = spawnerFrequency.ToString();

        // Update the slider
        UnityEngine.UI.Slider slider = elementUI.transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>();
        slider.onValueChanged.RemoveAllListeners();
        slider.minValue = 1;
        slider.maxValue = 10;
        slider.value = spawnerFrequency;
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
        spawnerFrequency = (int)value;
        elementUI.transform.FindChild("ValueText").GetComponent<UnityEngine.UI.Text>().text = ((int)value).ToString();
    }
}
