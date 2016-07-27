/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveToDiskEditor : Editor 
{
    [MenuItem("GameObject/Load Mesh/Load Mesh (Custom Format)", false, 1)]
    static void ReadAssetFromDisk()
    {
        var result = EditorUtility.OpenFilePanel("Load...", "", "asset");
        Debug.Log(result);

        if (!string.IsNullOrEmpty(result))
        {
            SaveToDisk.ReadAssetFromDisk(result, "");
        }
    }
    [MenuItem("GameObject/Load Mesh/Load Mesh (Obj)", false, 1)]
    static GameObject ReadAssetFromDiskAsObj()
    {
        var result = EditorUtility.OpenFilePanel("Load...", "", "obj");
        Debug.Log(result);
        if (!string.IsNullOrEmpty(result))
        {
            return SaveToDisk.ReadObjFromDisk(result, "");
        }
        return new GameObject();
    }

	[MenuItem("GameObject/Save Mesh/Save Mesh Selected (Custom Format)",false,1)]
	 static void SaveAssetToDisk()
	 {
	 	if (Selection.activeTransform!=null)
	 	{
		 	Debug.Log (Selection.activeTransform.name);
		 	var result=EditorUtility.SaveFilePanel("Save...","",Selection.activeTransform.name,"asset");
		 	Debug.Log(result);
		 	
		 	if (!string.IsNullOrEmpty( result)){
		 		SaveToDisk.SaveAssetToDisk(Selection.activeTransform.GetComponent<MeshFilter>(),result,"asset");
		 	}
	 	}
	 }
	 
	 [MenuItem("GameObject/Save Mesh/Save Mesh Selected (Obj)",false,1)]
	 static void SaveAssetToDiskAsObj()
	 {
	 	if (Selection.activeTransform!=null)
	 	{
		 	Debug.Log (Selection.activeTransform.name);
		 	var result=EditorUtility.SaveFilePanel("Save...","",Selection.activeTransform.name,"obj");
		 	Debug.Log(result);
		 	
		 	if (!string.IsNullOrEmpty( result)){
		 		SaveToDisk.SaveObjToDisk(Selection.activeTransform.GetComponent<MeshFilter>(),result);
		 	}
	 	}
	 }
}
