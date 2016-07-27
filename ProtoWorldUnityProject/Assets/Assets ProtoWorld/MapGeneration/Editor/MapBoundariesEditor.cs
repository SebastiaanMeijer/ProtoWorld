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

[CustomEditor(typeof(MapBoundaries))]
public class MapBoundariesEditor : Editor
{
    static string wcfCon;

    //public override void OnInspectorGUI()
    //{
    //    var t = target as MapBoundaries;

    //    if (GUILayout.Button("Generate map"))
    //    {
    //        OSMReaderSQL.Create2ServerSide();
    //        EditorUtility.SetDirty(target);
    //    }

    //    if (GUILayout.Button("Calculate boundaries"))
    //    {
    //        ChooseConnection();
    //        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
    //        Debug.Log(wcfCon);
    //        var b = client.GetBounds(wcfCon);
    //        Debug.Log("bound maxlon " + b.maxlon);
    //        var latDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.maxlat, b.minlon) * 1000;
    //        var lonDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.minlat, b.maxlon) * 1000;
    //        //var latDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.maxLat, t.minLon) * 1000;
    //        //var lonDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.minLat, t.maxLon) * 1000;
    //        Debug.Log("Latitude in kilometers: " + latDist + " ,in float: " + (float)latDist);
    //        Debug.Log("Longitude in kilometers: " + lonDist + " ,in float: " + (float)lonDist);
    //        var maxX = (float)latDist;
    //        var maxY = (float)lonDist;
    //        t.minMaxX[1] = maxX;
    //        t.minMaxY[1] = maxY;

    //        //Get the DB boundaries, so they can be used locally in play mode (without the need of WCF) -- Miguel R. C.
    //        var databaseBounds = client.GetBounds(wcfCon);
    //        t.dbBoundMinLat = databaseBounds.minlat;
    //        t.dbBoundMaxLat = databaseBounds.maxlat;
    //        t.dbBoundMinLon = databaseBounds.minlon;
    //        t.dbBoundMaxLon = databaseBounds.maxlon;

    //        EditorUtility.SetDirty(target);
    //    }

    //    DrawDefaultInspector();
    //    // base.OnInspectorGUI();
    //}

    public static string ChooseConnection()
    {
        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;
        return wcfCon;
    }
}
