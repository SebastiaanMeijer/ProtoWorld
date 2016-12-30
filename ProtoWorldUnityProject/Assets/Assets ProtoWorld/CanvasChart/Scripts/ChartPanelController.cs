/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * KPI Module
 * 
 * Nathan van Ofwegen
 */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChartPanelController : MonoBehaviour
{
    internal List<ChartController> charts;

    public List<ChartController> ordering;
    private List<ChartController> original_ordering;


    private Transform slotPanel;
    public GameObject slotPrefab;
    private Transform AddSlot;
	
	void Start ()
	{
	    charts = new List<ChartController>();
	    slotPanel = GameObject.Find("KPISlotPanel").transform;
	    AddSlot = GameObject.Find("AddSlot").transform;

        original_ordering = new List<ChartController>(ordering);

	    //Fetch all KPICharts in the scene
	    foreach (Transform kpichart in transform)
	    {
	        ChartController controller = kpichart.GetComponent<ChartController>();
	        if (controller != null) charts.Add(controller);
	    }

	    //Generate all slots from the ordering setted in the scene.
        GenerateKpiSlots(ordering);

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

    //Creates a new KPISlot bases on a chartcontroller, if the chartcontroller is null
    //the KPISlot sets the selected item to "None".
    private GameObject CreateKpiSlot(ChartController chart, int slotid)
    {
        GameObject kpi_obj = Instantiate(slotPrefab, transform.position, Quaternion.identity) as GameObject;
        KPISlotController kpislot = kpi_obj.GetComponent<KPISlotController>();

        kpislot.dropdown.options.Clear();
        kpislot.transform.SetParent(slotPanel);
        kpislot.active_chart = chart;
        kpislot.slotid = slotid;
        kpi_obj.name = "KPISlot " + kpislot.slotid;

        if (chart != null) kpislot.activekpi = chart.name;
        kpislot.FillOptions(charts);
        kpislot.SetDropdown();

        return kpi_obj;
    }

    //Generate slots from a chart list.
    void GenerateKpiSlots(List<ChartController> charts)
    {
        var i = 0;
        // Create a kpislot object for each item in the ordering.
        foreach (ChartController chart in charts) CreateKpiSlot(chart, i++);

        //So the transform is at the bottom of the list
        AddSlot.SetAsLastSibling();
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

    //Gets called when a new chart is selected from a KPI Slot.
    public void KpiSelect(KPISlotController slotController, string oldkpi, string newkpi)
    {

        ChartController oldChart = null;
        // Grab the old/new chart controller.
        if (newkpi.Equals("None"))
        {
            ordering[slotController.slotid] = null;
            return;
        }
        if (!oldkpi.Equals("None"))
        {
            oldChart = charts.FirstOrDefault(chart => chart.name.Equals(oldkpi));
        }

        ChartController newChart = charts.FirstOrDefault(chart => chart.name.Equals(newkpi));


        // Reset the dropdown to the old item if the wanted chart is already in a slot
        if (ordering.Contains(newChart))
        {
            slotController.SetDropdown();
        }
        else {
            // Set the state of the previous contentPanel to the new one
            bool was_active = true;
            if (oldChart != null) was_active = oldChart.contentPanel.activeSelf;
            newChart.contentPanel.SetActive(was_active);

            // Finally, set the new chart
            slotController.active_chart = newChart;
            slotController.activekpi = newChart.name;
            ordering[slotController.slotid] = newChart;
        }
    }

    //Add a new slot
    public void AddSlotPanel()
    {
        GameObject kpislot = CreateKpiSlot(null, ordering.Count);
        ordering.Add(kpislot.GetComponent<ChartController>());
        AddSlot.SetAsLastSibling();
    }

    //Remove a slot from the slotpanel, called by the KPISlotController
    public void RemoveSlot(int slotid)
    {
        //Remove the slot at slotid position
        ordering.RemoveAt(slotid);

        //Reorder the remaining slotid's
        int i = 0;
        foreach (Transform SlotPanel in slotPanel)
        {
            if (!SlotPanel.gameObject.name.StartsWith("KPISlot") ||
                SlotPanel.gameObject.name.Equals("KPISlot " + slotid)) continue;
            KPISlotController slot = SlotPanel.GetComponent<KPISlotController>();
            slot.slotid = i;
            SlotPanel.gameObject.name = "KPISlot " + i;
            Text text = SlotPanel.FindChild("ID").GetComponent<Text>();
            text.text = "Slot " + i;
            i++;
        }

        //Remove the prefab
        GameObject obj = GameObject.Find("KPISlot " + slotid);
        Destroy(obj);
    }

    //Set the kpi slots back to the initial values.
    public void RestoreSlots()
    {
        //Set the original ordering back to the ordering array
        ordering = new List<ChartController>(original_ordering);

        //Remove all KPISlots
        foreach (Transform kpi_slot in slotPanel)
        {
            if(kpi_slot.name.StartsWith("KPISlot")) Destroy(kpi_slot.gameObject);
        }

        //And generate them again from the chart list.
        GenerateKpiSlots(ordering);
    }
}
