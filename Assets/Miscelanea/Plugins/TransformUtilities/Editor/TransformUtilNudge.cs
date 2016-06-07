/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief Nudge functions for TransformUtil
 * \details All nudge functions are separated in this partial class
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Nudge in X direction
	/// </summary>
	private static void NudgeX()
	{
		foreach(Transform t in Selection.transforms)
		{
			NudgeIt(t, new Vector3(nudge.x, 0.0f, 0.0f));
		}
	}
	
	/// <summary>
	/// Nudge in -X direction
	/// </summary>
	private static void NudgeNegX()
	{
		foreach(Transform t in Selection.transforms)
		{
			NudgeIt(t, new Vector3(-nudge.x, 0.0f, 0.0f));
		}
	}
	
	/// <summary>
	/// Nudge in Y direction
	/// </summary>
	private static void NudgeY()
	{
		foreach(Transform t in Selection.transforms)
		{	
			NudgeIt(t, new Vector3(0.0f, nudge.y, 0.0f));
		}
	}
	
	/// <summary>
	/// Nudge in -Y direction
	/// </summary>
	private static void NudgeNegY()
	{
		foreach(Transform t in Selection.transforms)
		{		
			NudgeIt(t, new Vector3(0.0f, -nudge.y, 0.0f));
		}
	}
	
	/// <summary>
	/// Nudge in Z direction
	/// </summary>
	private static void NudgeZ()
	{
		foreach(Transform t in Selection.transforms)
		{		
			NudgeIt(t, new Vector3(0.0f, 0.0f, nudge.z));
		}
	}
	
	/// <summary>
	/// Nudge in -Z direction
	/// </summary>
	private static void NudgeNegZ()
	{
		foreach(Transform t in Selection.transforms)
		{
			NudgeIt(t, new Vector3(0.0f, 0.0f, -nudge.z));
		}
	}
	
	/// <summary>
	/// Moves a given transform in a given direction
	/// </summary>
	/// <param name="aTransform">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="anAmount">
	/// A <see cref="Vector3"/>
	/// </param>
	private static void NudgeIt(Transform aTransform, Vector3 anAmount)
	{
		if (aTransform != null)
		{
			Undo.RegisterUndo(aTransform, "Nudge " + aTransform.name);
			aTransform.Translate(anAmount, nudgeSpace);
		}
	}
}
