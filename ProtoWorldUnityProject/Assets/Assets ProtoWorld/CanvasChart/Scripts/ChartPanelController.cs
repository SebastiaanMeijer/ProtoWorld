using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChartPanelController : MonoBehaviour
{

    private List<ChartController> charts;
    
	// Use this for initialization
	void Start ()
	{

	    charts = new List<ChartController>();

	    foreach (Transform kpichart in transform)
	    {
	        ChartController controller = kpichart.GetComponent<ChartController>();
	        if (controller != null) charts.Add(controller);
	    }

	}
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 origin = transform.position;
	    int height = 0;

        foreach (ChartController chart in charts)
        {
            Vector3 pos = origin;
            pos.y -= height;
	        chart.transform.position = pos;

	        if (chart.contentPanel.active) height += 200;
	        else height += 25;
	    }

	}

    public void setAll(bool val)
    {
        foreach (ChartController controller in charts)
        {
            ToolbarScript scrtipt = controller.gameObject.transform.FindChild("Toolbar").GetComponent<ToolbarScript>();
            scrtipt.ToggleVisibility(val);
            //controller.contentPanel.SetActive(val);
        }  
    }
}
