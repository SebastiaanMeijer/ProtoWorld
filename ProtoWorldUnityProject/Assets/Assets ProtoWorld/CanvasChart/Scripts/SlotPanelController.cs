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
