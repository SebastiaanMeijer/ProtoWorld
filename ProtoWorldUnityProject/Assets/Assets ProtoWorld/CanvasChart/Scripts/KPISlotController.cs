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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class KPISlotController : MonoBehaviour
{
    public int slotid;
    public string activekpi;

    private ChartPanelController controller;

    public ChartController active_chart;

    public Dropdown dropdown;

    private Text id_text;
	
	void Start ()
	{
	    controller = GameObject.Find("ChartPanel").GetComponent<ChartPanelController>();
	    id_text = transform.Find("ID").GetComponent<Text>();

	    dropdown.onValueChanged.RemoveAllListeners();
	    dropdown.onValueChanged.AddListener(delegate { OnKPISelect(dropdown); });
	}

    void Update ()
    {
        id_text.text = "Chart " + (slotid + 1);
    }

    public void OnKPISelect(Dropdown dropdown)
    {
        string old_kpi = activekpi;
        string new_kpi = dropdown.options[dropdown.value].text;

        controller.KpiSelect(this, old_kpi, new_kpi);
    }


    public void FillOptions(List<ChartController> charts)
    {
        //Fill the dropdown array with all charts, unique chart names only
        dropdown.options.Clear();
        foreach (ChartController chart in charts)
        {
            AddKpiOption(chart.name);
        }

        //None option
        AddKpiOption("None");
    }


    public void AddKpiOption(string kpi)
    {
        if (!ContainsKpi(kpi))
        {
            dropdown.options.Add(new Dropdown.OptionData(kpi));
        }
    }

    public bool ContainsKpi(string kpi)
    {
        return dropdown.options.Any(data => kpi.Equals(data.text));
    }

    public void SetDropdown()
    {
        string kpi = "None";
        if(active_chart != null) kpi = active_chart.name;
        dropdown.value = GetKpiOptionValue(kpi);
        dropdown.captionText.text = kpi;
    }

    public int GetKpiOptionValue(string kpi)
    {
        foreach (var option in dropdown.options){
            if (option.text.Equals(kpi)) return dropdown.options.IndexOf(option);
        }
        return -1;
    }

    public void RemoveSlot()
    {
        controller.RemoveSlot(slotid);
    }
}
