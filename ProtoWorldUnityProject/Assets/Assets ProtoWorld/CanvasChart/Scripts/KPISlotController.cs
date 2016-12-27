﻿using UnityEngine;
using System.Collections;
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
