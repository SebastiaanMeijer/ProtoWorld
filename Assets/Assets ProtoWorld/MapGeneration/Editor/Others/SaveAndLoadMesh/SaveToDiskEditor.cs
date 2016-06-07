using UnityEngine;
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
