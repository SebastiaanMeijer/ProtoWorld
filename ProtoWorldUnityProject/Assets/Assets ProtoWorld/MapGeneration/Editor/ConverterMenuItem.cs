/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using UnityEditor;

/// <summary>
/// Menu item that converts any 3D point in the map into a lat-lon coordinates. 
/// This was used for testing the converter. 
/// </summary>
public class ConverterMenuItem : EditorWindow
{
    Vector3 vector3;
    float latitude;
    float longitude;

    //[MenuItem("Gapslabs Extended Editor/Test/Vector3ToLatLonConverter")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ConverterMenuItem window = (ConverterMenuItem)EditorWindow.GetWindow(typeof(ConverterMenuItem));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Coordinate Convertor Tool", EditorStyles.boldLabel);

        vector3 = EditorGUILayout.Vector3Field("3D point to convert", vector3);

        if (GUILayout.Button("Convert point to lat-lon"))
        {
            float[] conversion = CoordinateConvertor.Vector3ToLatLon(vector3, FindObjectOfType<MapBoundaries>());
            latitude = conversion[0];
            longitude = conversion[1];
        }

        latitude = EditorGUILayout.FloatField("latitude", latitude);
        longitude = EditorGUILayout.FloatField("longitude", longitude);
    }
}
