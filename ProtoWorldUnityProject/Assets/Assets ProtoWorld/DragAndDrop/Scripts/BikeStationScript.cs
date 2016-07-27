/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * BIKES FOR DRAG AND DROP SYSTEM 
 * BikeStationScript.cs
 * Furkan Sonmez
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Script that controls the behaviour of bike stations in the simulation. 
/// </summary>
public class BikeStationScript : MonoBehaviour
{
    //Amount of bikestations set to 100 as default, increase to add more bikestations.
    public static Vector3[] bikeStationPosition = new Vector3[100];
    public static Transform[] bikeStationTransform = new Transform[100];
    public static int bikeStationAmt;
    public static bool newBikeStation;

    public int capacityNumber;
    public int bikeStationNum;
    public int bikeRadius = 120;
    public Material materialOutOfBikes;

    public bool showFloatingNumber = true;

    private Material originalMaterial;
    private float nextTimeToInform;
    private float deltaTimeToInform = 10.0f;
    private FlashPedestriansInformer flashInformer;
    private float floatingValueLerp = 1.0f;
    private TextMesh floatingNumber;
    private ObjectData objectData;
    private GameObject controlPanel;
    private bool isSelected = false;

    /// <summary>
    /// Start method.
    /// </summary>
    void Start()
    {
        //Everytime a new bikeStation is spawned, add 1 to the total amount of bikestations
        bikeStationNum = bikeStationAmt;
        bikeStationAmt = bikeStationAmt + 1;

        //Save the original material (for later use in MarkOutOfBikes)
        originalMaterial = this.GetComponent<MeshRenderer>().material;

        nextTimeToInform = deltaTimeToInform;

        floatingNumber = this.GetComponentInChildren<TextMesh>();

        objectData = this.GetComponent<ObjectData>();

        controlPanel = GameObject.Find("ObjectControlPanelUI");

        flashInformer = FindObjectOfType<FlashPedestriansInformer>();

        RegisterBikeStationInInformer();

        isSelected = true;
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        if (floatingNumber != null && floatingValueLerp <= 1.0f)
        {
            floatingNumber.transform.localPosition = new Vector3(0.0f, Mathf.Lerp(2.0f, 10.0f, floatingValueLerp), 0.0f);
            floatingNumber.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, floatingValueLerp));
            floatingValueLerp += 0.02f;
        }

        if (isSelected && objectData != null)
        {
            objectData.ObjectName = "Bike Station " + bikeStationNum;
            objectData.ObjectDescription = "Number of bikes left on this station: " + this.gameObject.GetComponent<BikeStationScript>().capacityNumber;
            objectData.ObjectDescriptionText.text = this.gameObject.GetComponent<ObjectData>().ObjectDescription;
        }
    }

    /// <summary>
    /// Sends out the location of the new bikeStation to all pedestrians and make the newConditions boolean true.
    /// </summary>
    public void SendOutLocation()
    {
        //store the position and transform in the arrays 
        bikeStationPosition[bikeStationNum] = transform.position;
        bikeStationTransform[bikeStationNum] = this.transform;
        GlobalConditionsScript.newConditions = true;
    }

    /// <summary>
    /// Register the bike station in the itinerary informer if it exists. 
    /// </summary>
    public void RegisterBikeStationInInformer()
    {
        if (flashInformer != null)
            flashInformer.RegisterBikeStationPosition(this);
    }

    /// <summary>
    /// Pick a bike (method to be called by a pedestrian).
    /// </summary>
    internal void PickBike()
    {
        if (capacityNumber > 0)
        {
            capacityNumber--;

            if (showFloatingNumber)
                ShowFloatingText(capacityNumber.ToString());

            if (capacityNumber == 0)
                MarkOutOfBikes(true);
        }
    }

    /// <summary>
    /// Show information about the bike station on a floating text over it. 
    /// </summary>
    /// <param name="floating">Text to show floating.</param>
    private void ShowFloatingText(string floating)
    {
        floatingNumber.text = floating;
        floatingValueLerp = 0.0f;
    }

    /// <summary>
    /// Adds new bikes to the station.
    /// </summary>
    /// <param name="i">Number of bikes to add.</param>
    public void AddBikes(int i)
    {
        if (capacityNumber == 0)
            MarkOutOfBikes(false);

        capacityNumber += i;

        if (showFloatingNumber)
            ShowFloatingText("+ " + i);
    }

    /// <summary>
    /// Removes bikes from the station.
    /// </summary>
    /// <param name="i">Number of bikes to remove.</param>
    public void RemoveBikes(int i)
    {
        if (capacityNumber > 0)
        {
            if (capacityNumber > i)
                capacityNumber -= i;
            else
                capacityNumber = 0;

            if (showFloatingNumber)
                ShowFloatingText("- " + i);

            if (capacityNumber == 0)
                MarkOutOfBikes(true);
        }
    }

    /// <summary>
    /// Selects the bike station.
    /// </summary>
    public void Selected()
    {
        isSelected = true;
        HideControlPanel(false);
    }

    /// <summary>
    /// Deselects the bike station.
    /// </summary>
    public void Deselected()
    {
        isSelected = false;
        HideControlPanel(true);
    }

    /// <summary>
    /// Remove the bike station from the scene.
    /// </summary>
    public void RemoveBikeStation()
    {
        capacityNumber = 0;
        MarkOutOfBikes(true);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Hides/Shows the control panel (if it exists in the scene).
    /// </summary>
    /// <param name="hide">Boolean to decide if the control panel should be hidden.</param>
    private void HideControlPanel(bool hide)
    {
        if (controlPanel != null)
        {
            if (hide)
                controlPanel.GetComponent<FadingElementUI>().fadeOutCanvas();
            else
                controlPanel.GetComponent<FadingElementUI>().fadeInCanvas();
        }
    }

    /// <summary>
    /// Changes the graphics of the bike station to show that there are no more bikes on it. 
    /// </summary>
    /// <param name="p">If true, it will mark it as out of bikes.</param>
    private void MarkOutOfBikes(bool markOut)
    {
        if (materialOutOfBikes != null)
        {
            if (markOut)
                this.GetComponent<MeshRenderer>().material = materialOutOfBikes;
            else
                this.GetComponent<MeshRenderer>().material = originalMaterial;
        }
    }
}
