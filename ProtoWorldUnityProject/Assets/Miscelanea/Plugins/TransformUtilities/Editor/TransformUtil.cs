/*! \mainpage

\author Stig Olavsen <stig.olavsen@randomrnd.com>
\author http://www.RandomRnD.com
\date © 2011-August-05
\version 1.2.2012.04.22
 
\section About

TransformUtil is a set of utilities for the Unity editor that
allows you to easily nudge, rotate, flip and snap objects to a grid using
keyboard shortcuts, or editor buttons.

\section Features
 
 - Show a grid for any two axis XZ, YZ and YX independently or simultaneously
   with independent spacing in any direction and a separate color for
   each grid if wanted.
 - Nudge, Rotate, Flip and Snap using a custom editor window, menu items or
   keyboard shortcuts. Shortcuts can be customized by editing the 
   TransformUtilMenuItem.cs file (see
   http://unity3d.com/support/documentation/ScriptReference/MenuItem.html
   for information about how to set your own shortcuts)
 
\section Usage

In the menu, open the TransformUtil window under Edit --> TransformUtil -->
Open Window.

\image html window.png

The Preferences tab will allow you to change the behaviour of the utilities,
and the Controls tab contains buttons to nudge, rotate, flip and snap.

As any other Unity window, this can be docked anywhere in your editor.

All controls can also be activated from the menu, where you can also see the
keyboard shortcut assigned to the function.

\image html menu.png

Note that the grid and snap functions are independent of Unitys internal grid
and snap, and that control+mouse still uses Unitys internal grid and not
the one provided by TransformUtil (you can however in most cases set the
Unity grid to be of equal size).

The default keyboard shortcuts are as follows:
	
 - Nudge Z: Alt+i
 - Nudge -Z: Alt+k
 - Nudge X: Alt+l
 - Nudge -X: Alt+j
 - Nudge Y: Alt+Shift+i
 - Nudge -Y: Alt+Shift+k
 - Snap XYZ: Alt+,
 - Yaw Right: Alt+o
 - Yaw Left: Alt+u
 - Pitch Up: Alt+Shift+o
 - Pitch Down: Alt+Shift+u
 - Roll Right: Alt+Cmd/Control+o
 - Roll Left: Alt+Cmd/Control+u
 - Reset Rotation: Alt+p
 - Flip X: Alt+Cmd/Control+l
 - Flip Y: Alt+Cmd/Control+i
 - Flip Z: Alt+Cmd/Control+k

\section Changelog
 
 - 1.2 Changed default keybindings to be less intrusive
 - 1.1 Added undo functionality and possibility to operate on more than one
   selected transform
 - 1.0 Initial release 

\section Lisence

  Copyright (C) 2011 Stig Olavsen

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
     
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
     
  3. This notice may not be removed or altered from any source distribution.

  Stig Olavsen <stig.olavsen@randomrnd.com>
  
*/

/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief Main functionallity for TransformUtil
 * \details Main functionality for TransformUtil in this partial class
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

/// <summary>
/// The TransformUtil class contains all functionallity.
/// 
/// Definition is separated in multiple files using a partial
/// class definition.
/// </summary>
public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Unity OnEnable function, uses undocumented onSceneGUIDelegate
	/// to enable drawing the grid in the scene view with DrawInScene() function.
	/// </summary>
	void OnEnable()
	{
		SceneView.onSceneGUIDelegate = DrawInScene;	
	}
	
	
	/// <summary>
	/// Shows the config and control window in Unity.
	/// </summary>
	private static void SettingsGUI()
	{
		TransformUtil tu = (TransformUtil) GetWindow<TransformUtil>();
		tu.title = "Transform Util";
		tu.Show();
	}
	
	
	/// <summary>
	/// This is the delegate function given to SceneView.onSceneGUIDelegate
	/// which allows us to draw the grid in the scene view.
	/// </summary>
	/// <param name="sv">
	/// A <see cref="SceneView"/>
	/// </param>
	static void DrawInScene(SceneView sv)
	{
		if (showGridXZ)
		{
			DrawXZGrid();
		}
		if (showGridYX)
		{
			DrawYXGrid();
		}
		if (showGridYZ)
		{
			DrawYZGrid();
		}
	}
}
