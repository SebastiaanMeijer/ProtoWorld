using UnityEngine;
using System.Collections;
using UnityEditor;

public class SlotPanelController : MonoBehaviour
{
    private ChartPanelController _chartPanelController;
    internal bool active = true;

	// Use this for initialization
	void Start ()
	{
	    _chartPanelController = GameObject.Find("ChartPanel").GetComponent<ChartPanelController>();
	}
	
	// Update is called once per frame
	void Update ()
	{

	    RectTransform rect = gameObject.GetComponent<RectTransform>();

	    int num_slots = _chartPanelController.ordering.Count;

	    //Set the height depending on the amount of slots (if active)
	    float x = rect.sizeDelta.x;
	    float y = active ? (num_slots + 1) * 35 : 0 ;
	    rect.sizeDelta = new Vector2(x,y);

	    //Set all the children to the state of the slotpanel.
	    //If you set is with gameObject.SetActive() the whole script terminates, so.. the childeren!
	    foreach (Transform obj in transform)
	    {
	        obj.gameObject.SetActive(active);
	    }
	}
}
