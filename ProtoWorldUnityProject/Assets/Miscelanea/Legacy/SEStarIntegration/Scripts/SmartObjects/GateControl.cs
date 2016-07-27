/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class GateControl : MonoBehaviour
{
    private SEStarSmartObject smartObject;
    private SEStar SEStarControl;
    private Transform blockObject;
    private Vector3 initialScale;
    private bool isOpen;

    void Start()
    {
        //Debug.Log("Exit barrier created");

        //smartObject = this.GetComponent<UnitySmartObject>();
        //SEStarControl = GameObject.FindObjectOfType<SEStar>();

        //var blockObject = transform.FindChild("Block");

        //if (blockObject == null)
        //    Debug.LogError("The block object of the Ticketbarriers cannot be null");
        //else
        //    ChangeBarrier(true); // Open by default
    }

    //public void ChangeBarrier(bool open)
    //{
    //    // Problem: The prefab is modified and therefor the consequtive gameobjects are hitting null for blockObject.
    //    Debug.Log("CHANGE BARRIER RUNNING");

    //    if (blockObject == null)
    //    {
    //        blockObject = transform.FindChild("Block");
    //        initialScale = blockObject.localScale;
    //    }

    //    if (open)
    //    {
    //        blockObject.gameObject.SetActive(false);// blockObject.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //        isOpen = true;
    //    }
    //    else
    //    {
    //        blockObject.gameObject.SetActive(true);// blockObject.localScale = initialScale;
    //        isOpen = false;
    //    }
    //}

    //void OnMouseOver()
    //{
    //    // Left button changes its state
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Debug.Log("Exit barrier " + smartObject.objectId + " changing state");

    //        if (isOpen)
    //            SEStarControl.ChangeSEStarObjectState(smartObject.objectId, "Blocked");
    //        else
    //            SEStarControl.ChangeSEStarObjectState(smartObject.objectId, "Normal");

    //        ChangeBarrier(!isOpen);

    //    }
    //}
}
