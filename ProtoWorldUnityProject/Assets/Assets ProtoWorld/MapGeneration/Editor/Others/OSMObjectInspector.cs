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
 * Note: This is not used anymore. Look at OSMReaderSQL.cs instead.
 * 
 */

using UnityEngine;
using UnityEditor;
using Aram.OSMParser;
using System.Linq;
using System.Xml;
using GapslabWCFservice;

public class OSMObjectInspector : EditorWindow
{
	static string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase; //.ConnectionDatabase;

	//[MenuItem("Gapslabs GIS Package/Get More Info from database %#i")]
	static void OSMMoreInfoFromDatabaseMenu()
	{
		var go = GameObject.Find("AramGISBoundaries");
		var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;

		if (Selection.activeTransform == null)
			EditorUtility.DisplayDialog("No objects selected", "Please select an object from hierarchy tab.", "Ok");
		else
		{
			var name = Selection.activeTransform.name;
			ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

			var ret = "";
			if (!name.Contains("|"))
			{
				DisplayInfo("Not an OpenStreetMap object.");
				return;
			}
			var o = name.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
			ret += "Type: " + o[0] + "\n";
			ret += "id: " + o[2] + "\n";
			switch (o[1])
			{
				case "0": // Node
					{
						var selectedNode = client.GetNodeInfo(o[2], wcfCon);
						var nodeTags = client.GetNodeTags(o[2], wcfCon);
						ret += "Position: " + "Lat=" + selectedNode.lat + " Lon=" + selectedNode.lon + "\n";
						foreach (var tag in nodeTags)
						{
							ret += "Tag info: " + tag.KeyValue[0] + "=" + tag.KeyValue[1] + "\n";
						}
						break;
					}
				case "1": // Line
				case "2": // Polygon
					{
						var WayTags = client.GetWayTags(o[2], wcfCon);
						foreach (var tag in WayTags)
						{
							ret += "Tag info: " + tag.KeyValue[0] + "=" + tag.KeyValue[1] + "\n";
						}
						//					foreach (var node in way.Nodes)	
						//					{
						//						ret += "Node: "+"ID="+node.id+" Lat="+ node.Position.Lat+" Lon=" + node.Position.Lon+"\n";
						//					}

						break;
					}
				case "3":
					{
						// TODO
						//var selectedPolygon = source.Nodes.Where(i => i.Attribute("id").Value == o[2]).Single();
						ret += "Relations have not been implemented yet.";
						break;
					}
			}
			DisplayInfo(ret);
		}
	}

	//[MenuItem("Gapslabs GIS Package/Get More Info (OSM File)")]
	static void OSMMoreInfoMenu()
	{
		if (Selection.activeTransform == null)
			EditorUtility.DisplayDialog("No objects selected", "Please select an object from hierarchy tab.", "Ok");
		else
		{
			var name = Selection.activeTransform.name;
			OSMSource source = new OSMSource(AramGISGlobalObject.GetGlobalObject().filename);

			var ret = "";
			if (!name.Contains("|"))
			{
				DisplayInfo("Not an OpenStreetMap object.");
				return;
			}
			var o = name.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
			ret += "Type: " + o[0] + "\n";
			ret += "id: " + o[2] + "\n";
			switch (o[1])
			{
				case "0":
					{
						var selectedNode = source.Nodes.Where(i => i.Attribute("id").Value == o[2]).Single();
						OsmNode node = new OsmNode(selectedNode);
						ret += "Position: " + "Lat=" + node.Position.Lat + " Lon=" + node.Position.Lon + "\n";
						foreach (var tag in node.Tags)
						{
							ret += "Tag info: " + tag.Key + "=" + tag.Value + "\n";
						}
						break;
					}
				case "1":
					{
						var selectedLine = source.Ways.Where(i => i.Attribute("id").Value == o[2]).Single();
						Way way = new Way(selectedLine, source.OsmDocument);
						foreach (var tag in way.Tags)
						{
							ret += "Tag info: " + tag.Key + "=" + tag.Value + "\n";
						}
						//					foreach (var node in way.Nodes)	
						//					{
						//						ret += "Node: "+"ID="+node.id+" Lat="+ node.Position.Lat+" Lon=" + node.Position.Lon+"\n";
						//					}

						break;
					}
				case "2":
					{
						// TODO
						//var selectedPolygon = source.Nodes.Where(i => i.Attribute("id").Value == o[2]).Single();
						ret += "Relations have not been implemented yet.";
						break;
					}
			}
			DisplayInfo(ret);
		}
	}

	public static void DisplayInfo(string info)
	{
		EditorUtility.DisplayDialog("Info", info, "Ok");

		Debug.LogWarning(info);
	}


}



