/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * TRAFFIC INTEGRATION
 * TrafficIntegrationInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI for the TrafficIntegrationController.
/// </summary>
[CustomEditor(typeof(TrafficIntegrationController))]
public class TrafficIntegrationInspectorEditor : Editor
{
    public SerializedProperty
        useConfigFile_prop,
        useFrustumForUpdate_prop,
        useCoordinateConversion_prop,
        typeOfIntegration_prop,
        timeStepVelocityInMs_prop,
        sumoIsRunningInLocalHost_prop,
        remoteIp_prop,
        remotePortForTraci_prop,
        remotePortForListener_prop,
        vehicleBrakingActive_prop,
        timeToBrakeInSeconds_prop,
        driversPatientInSeconds_prop,
        driversAngleOfView_prop,
        pathSumoFCDFile_prop,
        pathVissimFZPFile_prop,
        pathMatSimXMLFile_prop,
        pathPWSimMetaFile_prop,
        simulationPaused_prop;

    /// <summary>
    /// Setup the SerializedProperties
    /// </summary>
    void OnEnable()
    {
        useConfigFile_prop = serializedObject.FindProperty("useConfigFile");
        useFrustumForUpdate_prop = serializedObject.FindProperty("useFrustumForUpdate");
        useCoordinateConversion_prop = serializedObject.FindProperty("useCoordinateConversion");
        typeOfIntegration_prop = serializedObject.FindProperty("typeOfIntegration");
        timeStepVelocityInMs_prop = serializedObject.FindProperty("timeStepVelocityInMs");
        sumoIsRunningInLocalHost_prop = serializedObject.FindProperty("sumoIsRunningInLocalHost");
        remoteIp_prop = serializedObject.FindProperty("remoteIp");
        remotePortForTraci_prop = serializedObject.FindProperty("remotePortForTraci");
        remotePortForListener_prop = serializedObject.FindProperty("remotePortForListener");
        vehicleBrakingActive_prop = serializedObject.FindProperty("vehicleBrakingActive");
        timeToBrakeInSeconds_prop = serializedObject.FindProperty("timeToBrakeInSeconds");
        driversPatientInSeconds_prop = serializedObject.FindProperty("driversPatientInSeconds");
        driversAngleOfView_prop = serializedObject.FindProperty("driversAngleOfView");
        pathSumoFCDFile_prop = serializedObject.FindProperty("pathSumoFCDFile");
        pathVissimFZPFile_prop = serializedObject.FindProperty("pathVissimFZPFile");
        pathMatSimXMLFile_prop = serializedObject.FindProperty("pathMatSimXMLFile");
        pathPWSimMetaFile_prop = serializedObject.FindProperty("pathPWSimMetaFile");
        simulationPaused_prop = serializedObject.FindProperty("simulationPaused");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(typeOfIntegration_prop);

        TrafficIntegrationController.TypeOfIntegration st = (TrafficIntegrationController.TypeOfIntegration)typeOfIntegration_prop.enumValueIndex;

        switch (st)
        {
            case TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration:
                EditorGUILayout.PropertyField(sumoIsRunningInLocalHost_prop, new GUIContent("Sumo Is Running In Local Host"));
                EditorGUILayout.PropertyField(remoteIp_prop, new GUIContent("Remote Ip"));
                EditorGUILayout.PropertyField(remotePortForTraci_prop, new GUIContent("Remote Port For Traci"));
                EditorGUILayout.PropertyField(remotePortForListener_prop, new GUIContent("Remote Port For Listener"));
                EditorGUILayout.PropertyField(vehicleBrakingActive_prop, new GUIContent("Vehicle Braking Active"));
                EditorGUILayout.PropertyField(timeToBrakeInSeconds_prop, new GUIContent("Time To Brake In Seconds"));
                EditorGUILayout.PropertyField(driversPatientInSeconds_prop, new GUIContent("Drivers Patient In Seconds"));
                EditorGUILayout.PropertyField(driversAngleOfView_prop, new GUIContent("Drivers Angle Of View"));
                EditorGUILayout.PropertyField(useConfigFile_prop);
                EditorGUILayout.PropertyField(useFrustumForUpdate_prop);
                EditorGUILayout.PropertyField(useCoordinateConversion_prop);
                EditorGUILayout.PropertyField(timeStepVelocityInMs_prop);
                break;

            case TrafficIntegrationController.TypeOfIntegration.SumoFCDFile:
                EditorGUILayout.PropertyField(pathSumoFCDFile_prop, new GUIContent("Path of Sumo FCD File"));
                EditorGUILayout.PropertyField(useConfigFile_prop);
                EditorGUILayout.PropertyField(useFrustumForUpdate_prop);
                EditorGUILayout.PropertyField(useCoordinateConversion_prop);
                EditorGUILayout.PropertyField(timeStepVelocityInMs_prop);
                break;

            case TrafficIntegrationController.TypeOfIntegration.VissimFZPFile:
                EditorGUILayout.PropertyField(pathVissimFZPFile_prop, new GUIContent("Path of Vissim FZP File"));
                EditorGUILayout.PropertyField(useConfigFile_prop);
                EditorGUILayout.PropertyField(useFrustumForUpdate_prop);
                EditorGUILayout.PropertyField(useCoordinateConversion_prop);
                EditorGUILayout.PropertyField(timeStepVelocityInMs_prop);
                break;

            case TrafficIntegrationController.TypeOfIntegration.MatsimDatabase:
                EditorGUILayout.PropertyField(pathMatSimXMLFile_prop, new GUIContent("Path of MatSim XML File"));
                EditorGUILayout.PropertyField(useConfigFile_prop);
                EditorGUILayout.PropertyField(useFrustumForUpdate_prop);
                EditorGUILayout.PropertyField(useCoordinateConversion_prop);
                EditorGUILayout.PropertyField(timeStepVelocityInMs_prop);
                break;

            case TrafficIntegrationController.TypeOfIntegration.PWSimPWSFile:
                EditorGUILayout.PropertyField(pathPWSimMetaFile_prop, new GUIContent("Path of ProtoWorld PWS Meta File"));
                EditorGUILayout.PropertyField(useConfigFile_prop);
                EditorGUILayout.PropertyField(useFrustumForUpdate_prop);
                EditorGUILayout.PropertyField(useCoordinateConversion_prop);
                EditorGUILayout.PropertyField(timeStepVelocityInMs_prop);
                //EditorGUILayout.PropertyField(simulationPaused_prop);
                break;

            case TrafficIntegrationController.TypeOfIntegration.DecisionTreeIntegration:
                EditorGUILayout.PropertyField(useFrustumForUpdate_prop);
                EditorGUILayout.PropertyField(useCoordinateConversion_prop);
                EditorGUILayout.PropertyField(timeStepVelocityInMs_prop);
                //EditorGUILayout.PropertyField(simulationPaused_prop);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
