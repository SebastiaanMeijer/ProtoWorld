/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief Snap functions for TransformUtil
 * \details All snap functions are separated in this partial class
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Snap to grid in X direction
	/// </summary>
	private static void SnapX()
	{
		foreach(Transform t in Selection.transforms)
		{	
			SnapToGrid(true,false,false, t, grid, gridOffset);
		}
	}
	
	/// <summary>
	/// Snap to grid in Y direction
	/// </summary>
	private static void SnapY()
	{
		foreach(Transform t in Selection.transforms)
		{	
			SnapToGrid(false,true,false, t, grid, gridOffset);
		}		
	}
	
	/// <summary>
	/// Snap to grid in Z direction
	/// </summary>
	private static void SnapZ()
	{
		foreach(Transform t in Selection.transforms)
		{	
			SnapToGrid(false,false,true, t, grid, gridOffset);
		}
	}
	
	/// <summary>
	/// Snap to grid in XZ direction
	/// </summary>
	private static void SnapXZ()
	{
		foreach(Transform t in Selection.transforms)
		{	
			SnapToGrid(true,false,true, t, grid, gridOffset);
		}
	}
	
	/// <summary>
	/// Snap to grid in YZ direction
	/// </summary>
	private static void SnapYZ()
	{
		foreach(Transform t in Selection.transforms)
		{	
			SnapToGrid(false,true,true, t, grid, gridOffset);
		}
	}
	
	/// <summary>
	/// Snap to grid in XY direction
	/// </summary>
	private static void SnapXY()
	{
		foreach(Transform t in Selection.transforms)
		{	
			SnapToGrid(true,true,false, t, grid, gridOffset);
		}
	}
	
	/// <summary>
	/// Snap to grid in XYZ direction
	/// </summary>
	private static void SnapXYZ()
	{
		foreach(Transform t in Selection.transforms)
		{	
			SnapToGrid(true,true,true, t, grid, gridOffset);
		}
	}
	
	/// <summary>
	/// Performs a snap to grid
	/// </summary>
	/// <param name="snapx">
	/// A <see cref="System.Boolean"/>
	/// </param>
	/// <param name="snapy">
	/// A <see cref="System.Boolean"/>
	/// </param>
	/// <param name="snapz">
	/// A <see cref="System.Boolean"/>
	/// </param>
	/// <param name="aTransform">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="aGrid">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="anOffset">
	/// A <see cref="Vector3"/>
	/// </param>
	private static void SnapToGrid(bool snapx, bool snapy, bool snapz, 
	                               Transform aTransform, Vector3 aGrid, Vector3 anOffset)
	{
		if (aTransform != null)
		{
			Undo.RegisterUndo(aTransform, "Snap " + aTransform.name);
			
			Vector3 position = aTransform.position;
	
			if (snapx)
			{
				position.x = position.x - ((float)Math.IEEERemainder((double)(position.x-anOffset.x), (double)(grid.x)));
			}
			if (snapy)
			{
				position.y = position.y - ((float)Math.IEEERemainder((double)(position.y-anOffset.y), (double)(grid.y)));
			}
			if (snapz)
			{
				position.z = position.z - ((float)Math.IEEERemainder((double)(position.z-anOffset.z), (double)(grid.z)));
			}
			aTransform.position = position;
		}
	}
}
