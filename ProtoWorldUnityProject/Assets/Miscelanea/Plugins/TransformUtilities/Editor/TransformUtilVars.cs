/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief File containing variables for TransformUtil class
 * \details All variables are separated in this partial class definition
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Nudge amount, if not equal to grid
	/// </summary>
	private static Vector3 nudge = new Vector3(1.0f, 1.0f, 1.0f);
	
	/// <summary>
	/// Rotation amount
	/// </summary>
	private static float rotateAmount = 22.5f;
	
	/// <summary>
	/// Grid size in XYZ
	/// </summary>
	private static Vector3 grid = new Vector3(1.0f, 1.0f, 1.0f);
	
	/// <summary>
	/// Show XZ grid floor
	/// </summary>
	private static bool showGridXZ = false;
	
	/// <summary>
	/// Show YZ grid floor
	/// </summary>
	private static bool showGridYZ = false;
	
	/// <summary>
	/// Show YX grid floor
	/// </summary>
	private static bool showGridYX = false;
	
	/// <summary>
	/// Number of displayed grid lines
	/// </summary>
	private static int gridLines = 10;
	
	/// <summary>
	/// Grid offset from origin
	/// </summary>
	private static Vector3 gridOffset = new Vector3(0.0f, 0.0f, 0.0f);
	
	/// <summary>
	/// Color used for XZ grid floor
	/// </summary>
	private static Color gridXZColor = new Color(0.8f, 0.8f, 0.8f, 0.2f);
	
	/// <summary>
	/// Color used for YZ grid floor
	/// </summary>
	private static Color gridYZColor = new Color(0.8f, 0.8f, 0.8f, 0.2f);
	
	/// <summary>
	/// Color used for YX grid floor
	/// </summary>
	private static Color gridYXColor = new Color(0.8f, 0.8f, 0.8f, 0.2f);
	
	/// <summary>
	/// Color used for grid X center line
	/// </summary>
	private static Color gridXColor = new Color(0.8f, 0.0f, 0.0f, 0.4f);
	
	/// <summary>
	/// Color used for grid Y center line
	/// </summary>
	private static Color gridYColor = new Color(0.0f, 0.8f, 0.0f, 0.4f);
	
	/// <summary>
	/// Color used for grid Z center line
	/// </summary>
	private static Color gridZColor = new Color(0.0f, 0.0f, 0.8f, 0.4f);
	
	/// <summary>
	/// Rotation space for rotation functions
	/// </summary>
	private static Space rotationSpace = Space.Self;
	
	/// <summary>
	/// Nudge space for nudge function
	/// </summary>
	private static Space nudgeSpace = Space.World;
}