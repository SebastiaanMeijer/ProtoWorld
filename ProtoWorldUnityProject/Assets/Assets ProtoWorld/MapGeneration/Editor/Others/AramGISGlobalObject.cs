/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * GAPSLABS EXTENDED EDITOR
 * Aram Azhari
 * 
 * Reviewed by Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using Aram.OSMParser;
using System.Linq;
using System.Xml;

public class AramGISGlobalObject : MonoBehaviour {
	
	public string filename;
	public OSMSource source;
	// Use this for initialization
	void Start () {
	
	}

	public static AramGISGlobalObject GetGlobalObject()
	{
		if (GameObject.Find("AramGISGlobalObject")==null)
		{
			string fn = EditorUtility.OpenFilePanel("Select the Open Street Map (OSM) file", "", "osm");
			if (string.IsNullOrEmpty(fn))
			{
				EditorUtility.DisplayDialog("No OSM info found","Please select an OSM source first.","Ok");
				return null;
			}
			else
			{
				GameObject globalObject=new GameObject("AramGISGlobalObject");
				var a=globalObject.AddComponent<AramGISGlobalObject>();
				
				a.filename=fn;
				a.source=new OSMSource(a.filename);
				return a;
			}
		}
		else
		{
			return GameObject.Find("AramGISGlobalObject").GetComponent<AramGISGlobalObject>();
		}
			
	}
	// Update is called once per frame
	void Update () {
	
	}
}
