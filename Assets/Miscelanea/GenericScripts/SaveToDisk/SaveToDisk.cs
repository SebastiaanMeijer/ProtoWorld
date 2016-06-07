using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public  class SaveToDisk 
{
	
	 public static void SaveAssetToDisk(MeshFilter meshFilter,string filePath,string Extension)
	 {
	 	var bin= MeshSerializer.WriteMesh(meshFilter.sharedMesh,true); 		
	 	File.WriteAllBytes(filePath,bin);
	 }	 
	 public static void SaveObjToDisk(MeshFilter meshFilter,string filePath)
	 {
	 	ObjExporter.MeshToFile(meshFilter,filePath);
	 }
	 public static void SaveMetaData(string Value,string filePath)
	 {
	 	File.WriteAllText(filePath,Value);
	 }
	 public static GameObject ReadObjFromDisk(string filePath,string alternativeName)
	 {
	 	var m=ObjImporter.ImportFile(filePath);
	 	GameObject g=new GameObject(System.IO.Path.GetFileNameWithoutExtension(filePath));
	 	if (!string.IsNullOrEmpty(alternativeName))
	 		g.name=alternativeName;
		var mf= g.AddComponent<MeshFilter>();
 		mf.sharedMesh=m;
 		var coll=g.AddComponent<MeshCollider>();
 		coll.sharedMesh=m;
 		g.AddComponent<MeshRenderer>();
 		return g;
	 }
	 public static GameObject ReadAssetFromDisk(string filePath,string alternativeName)
	 {
	 	var bin=File.ReadAllBytes(filePath);
	 	Mesh m= MeshSerializer.ReadMesh(bin);
	 	
	 	GameObject g=new GameObject(System.IO.Path.GetFileNameWithoutExtension(filePath));
	 	if (!string.IsNullOrEmpty(alternativeName))
	 		g.name=alternativeName;
		var mf= g.AddComponent<MeshFilter>();
 		mf.sharedMesh=m;
 		var coll=g.AddComponent<MeshCollider>();
 		coll.sharedMesh=m;
 		g.AddComponent<MeshRenderer>();
 		return g;
	 }
}
