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

[CustomEditor(typeof(SEStar))]
public class SEStarEditor : Editor
{
    /// <summary>
    /// Inspector button of the Unity Editor to calculate coordinate difference.
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Calculate coordinate difference"))
            CalculateCoordinateDifference();

        DrawDefaultInspector();
    }

    /// <summary>
    /// Calculates the coordinate difference between SeStar and the Unity scene. 
    /// </summary>
    private void CalculateCoordinateDifference()
    {
        var sestar = target as SEStar;
        var go = GameObject.Find("AramGISBoundaries");
        var mapboundaries = go.GetComponent<MapBoundaries>();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        CoordinateConvertor.Initialize(client, mapboundaries);

        sestar.CalculatedUnityReference = CoordinateConvertor.LatLonToVector3(sestar.ReferenceLatitudeLongitude.x, sestar.ReferenceLatitudeLongitude.y);

        if (sestar.OverrideReferenceOffsetByAnObject)
            sestar.CalculatedDifference = sestar.ReferenceOffsetObject.position;
        else
            sestar.CalculatedDifference = sestar.CalculatedUnityReference - sestar.ReferenceSEStarCartesian.ThalesCartesianToUnity();

        EditorUtility.SetDirty(target);
    }
}
