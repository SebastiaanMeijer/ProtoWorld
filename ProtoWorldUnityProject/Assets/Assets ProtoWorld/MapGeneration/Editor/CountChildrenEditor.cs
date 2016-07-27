/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class CountChildrenEditor : Editor
{

    [MenuItem("GameObject/Count children")]
    static void CountChildren()
    {
        var parents = Selection.transforms;
        try
        {
            if (parents.Length != 0)
            {
                int children = 0;
                foreach (var parent in parents)
                {
                    children += parent.GetComponentsInChildren<Transform>().Length;
                }
                EditorUtility.DisplayDialog("Object count", "Total children: " + children, "OK");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

    }

    [MenuItem("GameObject/DuplicateToTenthousand")]
    static void DuplicateTo10Thousand()
    {
        var parents = Selection.transforms;
        try
        {
            if (parents.Length != 0)
            {
                foreach (var parent in parents)
                {
                    var duplicate = (Transform)Object.Instantiate(parent);
                    duplicate.position = new Vector3(10000, 0, 10000);
                }
            }
            EditorUtility.DisplayDialog("Duplication complete", "Duplicated successfully", "OK");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

    }
}
