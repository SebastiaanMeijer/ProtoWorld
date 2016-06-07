/*
 * 
 * BIKES FOR DRAG AND DROP SYSTEM 
 * RangeCircleScript.cs
 * Furkan Sonmez
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class draws the circle around bike stations to showcast the radius of influence. 
/// </summary>
public class RangeCircleScript : MonoBehaviour 
{
    [HideInInspector]
	public Transform circleCanvas;

    [HideInInspector]
	public static bool circleBool;

    /// <summary>
    /// Use this for initialization
    /// </summary>
	void Start () 
    {
        circleCanvas = GetComponentInChildren<Transform>().FindChild("CircleCanvas");
		
        //make the circleBool true whenever a new bikestation is added
		if(rayHitPositionClass.gameStartedBool == true)
			circleBool = true;
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () 
    {
		//if the circleBool is true set the circleCanvas to true
		if(circleBool == true)
			circleCanvas.gameObject.SetActive (true);
		else
			circleCanvas.gameObject.SetActive (false);
	}

	/// <summary>
	/// Whenever a bikeStation is selected, this function is called
	/// </summary>
	public void SelectedCircle()
    {
		circleBool = true;
	}

	/// <summary>
	/// Whenever a bikeStation is deselected, this function is called
	/// </summary>
	public void DeselectedCircle()
    {
		circleBool = false;
	}
}
