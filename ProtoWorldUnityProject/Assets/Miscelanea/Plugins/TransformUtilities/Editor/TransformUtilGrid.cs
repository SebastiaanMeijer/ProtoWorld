/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief Grid functions for TransformUtil
 * \details The grid functions are separated in this partial class
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Draw a grid in the XZ plane
	/// </summary>
	private static void DrawXZGrid()
	{
		DrawGrid(gridOffset, gridLines, Vector3.right * grid.x, Vector3.forward * grid.z,
		         gridXZColor, gridXColor, gridZColor);
	}
	
	/// <summary>
	/// Draw a grid in the YZ plane
	/// </summary>
	private static void DrawYZGrid()
	{
		DrawGrid(gridOffset, gridLines, Vector3.forward * grid.x, Vector3.up * grid.y,
		         gridYZColor, gridZColor, gridYColor);
	}
	
	/// <summary>
	/// Draw a grid in the YX plane
	/// </summary>
	private static void DrawYXGrid()
	{
		DrawGrid(gridOffset, gridLines, Vector3.up * grid.y, Vector3.left * grid.z,
		         gridYXColor, gridYColor, gridXColor);
	}
	
	/// <summary>
	/// Draws a grid in the two specified directions.
	/// </summary>
	/// <param name="anOffset">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="aNumberOfGridLines">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="aDirection1">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="aDirection2">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="aGridColor">
	/// A <see cref="Color"/>
	/// </param>
	/// <param name="aDirection1CenterColor">
	/// A <see cref="Color"/>
	/// </param>
	/// <param name="aDirection2CenterColor">
	/// A <see cref="Color"/>
	/// </param>
	private static void DrawGrid(Vector3 anOffset, int aNumberOfGridLines, Vector3 aDirection1, Vector3 aDirection2,
	                             Color aGridColor, Color aDirection1CenterColor, Color aDirection2CenterColor)
	{
		int s = aNumberOfGridLines / 2;
		for (int x = -s; x <= s; x++)
		{
			for (int y = -s; y <= s; y++)
			{
				if (y == 0)
				{
					Handles.color = aDirection1CenterColor;
				}
				else
				{
					Handles.color = aGridColor;
				}
				Vector3 pos = anOffset + ((x * aDirection1) + (y * aDirection2));
				if (x < s)
					Handles.DrawLine(pos, pos+aDirection1);
				if (x == 0)
				{
					Handles.color = aDirection2CenterColor;
				}
				else
				{
					Handles.color = aGridColor;
				}
				if (y < s)
					Handles.DrawLine(pos, pos+aDirection2);
			}
		}
	}	
}
