Author:
    Stig Olavsen <stig.olavsen@randomrnd.com> 
    http://www.RandomRnD.com 

Date:
    © 2011-August-05 

Version:
    1.1.2011.08.05

About
=====

TransformUtil is a set of utilities for the Unity editor that allows you to easily nudge, rotate, flip and snap objects to a grid using keyboard shortcuts, or editor buttons.


Features
========

    * Show a grid for any two axis XZ, YZ and YX independently or simultaneously with independent spacing in any direction and a separate color for each grid if wanted.
    * Nudge, Rotate, Flip and Snap using a custom editor window, menu items or keyboard shortcuts. Shortcuts can be customized by editing the TransformUtilMenuItem.cs file (see http://unity3d.com/support/documentation/ScriptReference/MenuItem.html for information about how to set your own shortcuts)


Usage
=====

In the menu, open the TransformUtil window under Edit --> TransformUtil --> Open Window.

The Preferences tab will allow you to change the behaviour of the utilities, and the Controls tab contains buttons to nudge, rotate, flip and snap.

As any other Unity window, this can be docked anywhere in your editor.

All controls can also be activated from the menu, where you can also see the keyboard shortcut assigned to the function.

Note that the grid and snap functions are independent of Unitys internal grid and snap, and that control+mouse still uses Unitys internal grid and not the one provided by TransformUtil (you can however in most cases set the Unity grid to be of equal size).

The default keyboard shortcuts are as follows:

    Nudge Z: i
    Nudge -Z: k
    Nudge X: l
    Nudge -X: j
    Nudge Y: shift+i
    Nudge -Y: shift+k
    Snap XYZ: ,
    Yaw Right: o
    Yaw Left: u
    Pitch Up: alt+o
    Pitch Down: alt+u
    Roll Right: shift+o
    Roll Left: shift+u
    Reset Rotation: alt+p
    Flip X: alt+l
    Flip Y: alt+i
    Flip Z: alt+k


Changelog
=========

    1.1 Added undo functionality and possibility to operate on more than one selected transform
    1.0 Initial release


Lisence
=======

Copyright (C) 2011 Stig Olavsen

This software is provided 'as-is', without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software.

Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.

2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.

3. This notice may not be removed or altered from any source distribution.

Stig Olavsen <stig.olavsen@randomrnd.com>
