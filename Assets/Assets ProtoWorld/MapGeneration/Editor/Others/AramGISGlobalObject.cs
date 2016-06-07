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
