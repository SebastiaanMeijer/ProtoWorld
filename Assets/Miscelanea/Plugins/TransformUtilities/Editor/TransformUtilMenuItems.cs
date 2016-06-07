/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \author http://www.RandomRnD.com
 * \date © 2011-August-05
 * \brief Menu items for TransformUtil
 * \details All menu items are separated in this partiall class
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public partial class TransformUtil : EditorWindow
{
	/// <summary>
	/// Opens the preferences and control window
	/// </summary>
	[MenuItem("Edit/Transform Util/Open Window", false, 300000)]
	static void OnMenuSettings()
	{
		SettingsGUI();
	}
	
	/// <summary>
	/// Nudge Z Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Nudge Z &i", false, 310001)]
	static void OnMenuNudgeZ()
	{
		NudgeZ();
	}
	
	/// <summary>
	/// Nudge -Z Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Nudge -Z &k", false, 310002)]
	static void OnMenuNudgeNegZ()
	{
		NudgeNegZ();
	}

	/// <summary>
	/// Nudge X Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Nudge X &l", false, 310003)]
	static void OnMenuNudgeX()
	{
		NudgeX();
	}

	/// <summary>
	/// Nudge -X Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Nudge -X &j", false, 310004)]
	static void OnMenuNudgeNegX()
	{
		NudgeNegX();
	}	
		
	/// <summary>
	/// Nudge Y Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Nudge Y #&i", false, 310005)]
	static void OnMenuNudgeY()
	{
		NudgeY();
	}
	
	/// <summary>
	/// Nudge -Y Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Nudge -Y #&k", false, 310006)]
	static void OnMenuNudgeNegY()
	{
		NudgeNegY();
	}
	
	/// <summary>
	/// Snap in X direction Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Snap X", false, 320001)]
	static void OnMenuSnapX()
	{
		SnapX();
	}

	/// <summary>
	/// Snap in Y direction Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Snap Y", false, 320001)]
	static void OnMenuSnapY()
	{
		SnapY();
	}

	/// <summary>
	/// Snap in Z direction Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Snap Z", false, 320001)]
	static void OnMenuSnapZ()
	{
		SnapZ();
	}
	
	/// <summary>
	/// Snap in XZ direction Menu Item
	/// </summary>	
	[MenuItem("Edit/Transform Util/Snap XZ", false, 320001)]
	static void OnMenuSnapXZ()
	{
		SnapXZ();
	}
	
	/// <summary>
	/// Snap in YZ direction Menu Item
	/// </summary>	
	[MenuItem("Edit/Transform Util/Snap YZ", false, 320001)]
	static void OnMenuSnapYZ()
	{
		SnapYZ();
	}

	/// <summary>
	/// Snap in XY direction Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Snap XY", false, 320001)]
	static void OnMenuSnapXY()
	{
		SnapXY();
	}
	
	/// <summary>
	/// Snap in XYZ direction Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Snap XYZ &,", false, 320001)]
	static void OnMenuSnapXYZ()
	{
		SnapXYZ();
	}
	
	/// <summary>
	/// Yaw Right Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Yaw Right &o", false, 330001)]
	static void OnMenuYawRight()
	{
		YawRight();
	}
	
	/// <summary>
	/// Yaw Left Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Yaw Left &u", false, 330002)]
	static void OnMenuYawLeft()
	{
		YawLeft();
	}
	
	/// <summary>
	/// Pitch Up Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Pitch Up &#o", false, 330003)]
	static void OnMenuPitchUp()
	{
		PitchUp();
	}
	
	/// <summary>
	/// Pitch Down Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Pitch Down &#u", false, 330004)]
	static void OnMenuPitchDown()
	{
		PitchDown();
	}
	
	/// <summary>
	/// Roll Right Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Roll Right &%o", false, 330005)]
	static void OnMenuRollRight()
	{
		RollRight();
	}
	
	/// <summary>
	/// Roll Left Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Roll Left &%u", false, 330006)]
	static void OnMenuRollLeft()
	{
		RollLeft();
	}
	
	/// <summary>
	/// Reset Rotation Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Reset Rotation &p", false, 340001)]
	static void OnMenuResetRotation()
	{
		ResetRotation();
	}
	
	/// <summary>
	/// Flip X Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Flip X &%l", false, 350001)]
	static void OnMenuFlipX()
	{
		FlipX();
	}
	
	/// <summary>
	/// Flip Y Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Flip Y &%i", false, 350002)]
	static void OnMenuFlipY()
	{
		FlipY();
	}
	
	/// <summary>
	/// Flip Z Menu Item
	/// </summary>
	[MenuItem("Edit/Transform Util/Flip Z &%k", false, 350003)]
	static void OnMenuFlipZ()
	{
		FlipZ();
	}
}
