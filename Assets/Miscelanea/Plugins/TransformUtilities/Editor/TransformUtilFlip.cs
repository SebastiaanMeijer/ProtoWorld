/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief Flip functions for TransformUtil
 * \details The flip functions are separated in this partial class
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Flip active transform in X direction
	/// </summary>
	private static void FlipX()
	{
		foreach(Transform t in Selection.transforms)
		{
			Undo.RegisterUndo(t, "Flip " + t.name);
			t.localScale = 
				new Vector3(-t.localScale.x,
				            t.localScale.y,
				            t.localScale.z);
		}
	}
	
	/// <summary>
	/// Flip active transform in Y direction
	/// </summary>
	private static void FlipY()
	{
		foreach(Transform t in Selection.transforms)
		{
			Undo.RegisterUndo(t, "Flip " + t.name);
			t.localScale = 
				new Vector3(t.localScale.x,
				            -t.localScale.y,
				            t.localScale.z);
		}
	}

	/// <summary>
	/// Flip active transform in Z direction
	/// </summary>
	private static void FlipZ()
	{
		foreach(Transform t in Selection.transforms)
		{
			Undo.RegisterUndo(t, "Flip " + t.name);
			t.localScale = 
				new Vector3(t.localScale.x,
				            t.localScale.y,
				            -t.localScale.z);
		}
	}
}
