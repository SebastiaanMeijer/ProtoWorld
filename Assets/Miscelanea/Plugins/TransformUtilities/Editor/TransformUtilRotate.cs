/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief Rotate functions for TransformUtil
 * \details All rotate functions are separated in this partiall class
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Transform Yaw Right
	/// </summary>	
	private static void YawRight()
	{
		foreach(Transform t in Selection.transforms)
		{	
			RotateIt(t, new Vector3(0.0f, rotateAmount, 0.0f));
		}
	}
	
	/// <summary>
	/// Transform Yaw Left
	/// </summary>	
	private static void YawLeft()
	{
		foreach(Transform t in Selection.transforms)
		{	
			RotateIt(t, new Vector3(0.0f, -rotateAmount, 0.0f));
		}
	}
	
	/// <summary>
	/// Transform Roll Left
	/// </summary>	
	private static void RollLeft()
	{
		foreach(Transform t in Selection.transforms)
		{		
			RotateIt(t, new Vector3(rotateAmount, 0.0f, 0.0f));
		}
	}
	
	/// <summary>
	/// Transform Roll Right
	/// </summary>	
	private static void RollRight()
	{
		foreach(Transform t in Selection.transforms)
		{		
			RotateIt(t, new Vector3(-rotateAmount, 0.0f, 0.0f));
		}
	}
	
	/// <summary>
	/// Transform Pitch Up
	/// </summary>	
	private static void PitchUp()
	{
		foreach(Transform t in Selection.transforms)
		{
			RotateIt(t, new Vector3(0.0f, 0.0f, rotateAmount));
		}
	}
	
	/// <summary>
	/// Transform Pitch Down
	/// </summary>	
	private static void PitchDown()
	{
		foreach(Transform t in Selection.transforms)
		{
			RotateIt(t, new Vector3(0.0f, 0.0f, -rotateAmount));
		}		
	}
	
	/// <summary>
	/// Reset transform rotation
	/// </summary>	
	private static void ResetRotation()
	{
		foreach(Transform t in Selection.transforms)
		{
			t.rotation = Quaternion.identity;
		}
	}
	
	/// <summary>
	/// Rotate a transform by a given angle
	/// </summary>	
	private static void RotateIt(Transform aTransform, Vector3 anAngle)
	{
		if (aTransform != null)
		{
			Undo.RegisterUndo(aTransform, "Rotate " + aTransform.name);
			aTransform.Rotate(anAngle, rotationSpace);
		}
	}
}
