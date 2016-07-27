/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
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
