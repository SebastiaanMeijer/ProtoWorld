using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ChartPanelController : MonoBehaviour
{
    internal List<ChartController> charts;

    public ChartController[] ordering;

    private Transform slotPanel;
    public GameObject slotPrefab;
	
	void Start ()
	{
	    charts = new List<ChartController>();
	    slotPanel = GameObject.Find("SlotPanel").transform;

	    foreach (Transform kpichart in transform)
	    {
	        ChartController controller = kpichart.GetComponent<ChartController>();
	        if (controller != null) charts.Add(controller);
	    }

	    var i = 0;

	    // Create a kpislot object for each item in the ordering.
	    foreach (ChartController chart in ordering)
	    {
	        GameObject kpi_obj = Instantiate(slotPrefab, transform.position, Quaternion.identity) as GameObject;
	        KPISlotController kpislot = kpi_obj.GetComponent<KPISlotController>();

	        kpislot.dropdown.options.Clear();
	        kpislot.transform.SetParent(slotPanel);
	        kpislot.active_chart = chart;
	        kpislot.activekpi = chart.name;
	        kpislot.FillOptions(charts);
	        kpislot.slotid = i++;
	        kpislot.SetDropdown();
	        kpi_obj.name = "KPISlot " + kpislot.slotid;
	    }

		// Start with the slot panel disabled.
		SlotPanelController scontroller = slotPanel.GetComponent<SlotPanelController>();
        scontroller.active = false;
	}

	
	void Update ()
	{
	    // Disable each chart that is not selected in the slots.
	    foreach (ChartController chart in charts)
	    {
	        if(!ordering.Contains(chart)) chart.gameObject.SetActive(false);
	    }

	    Vector3 origin = transform.position;
	    float height = slotPanel.GetComponent<RectTransform>().sizeDelta.y;

	    foreach (ChartController chart in ordering)
	    {
	        if (chart == null) continue;
	        chart.gameObject.SetActive(true);
	        Vector3 pos = origin;
	        pos.y -= height;
	        chart.transform.position = pos;

	        if (chart.contentPanel.activeSelf) height += 200;
	        else height += 25;
	    }
	}

    // Toggle the slotpanel
    public void ToggleSlotConfig()
    {
        SlotPanelController scontroller = slotPanel.GetComponent<SlotPanelController>();
        scontroller.active = !scontroller.active;
    }

    // Mass assignment of the states of the contentpanels (on/off)
    public void SetAll(bool val)
    {
        foreach (ChartController controller in charts)
        {
            ToolbarScript script = controller.gameObject.transform.FindChild("Toolbar").GetComponent<ToolbarScript>();
            script.ToggleVisibility(val);
        }  
    }

    public void KpiSelect(KPISlotController slotController, string oldkpi, string newkpi)
    {
        // Grab the old/new chart controller.
        if (newkpi.Equals("None"))
        {
            ordering[slotController.slotid] = null;
            return;
        }
        ChartController newChart = charts.FirstOrDefault(chart => chart.name.Equals(newkpi));
        ChartController oldChart = charts.FirstOrDefault(chart => chart.name.Equals(oldkpi));

        // Reset the dropdown to the old item if the wanted chart is already in a slot
        if (ordering.Contains(newChart))
        {
            slotController.SetDropdown();
        }
        else {
            // Set the state of the previous contentPanel to the new one
            bool was_active = oldChart.contentPanel.activeSelf;
            newChart.contentPanel.SetActive(was_active);

            // Finally, set the new chart
            slotController.active_chart = newChart;
            slotController.activekpi = newChart.name;
            ordering[slotController.slotid] = newChart;
        }
    }
}
