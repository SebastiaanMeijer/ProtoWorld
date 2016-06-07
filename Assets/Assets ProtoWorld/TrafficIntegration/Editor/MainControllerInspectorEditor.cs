/*
 * 
 * SUMO COMMUNICATION
 * MainControllerInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI in the MainController Inspector for communication with SUMO.
/// </summary>
[CustomEditor(typeof(SumoMainController))]
public class MainControllerInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SumoMainController script = (SumoMainController)target;

        if (GUILayout.Button("Run simulation"))
        {
           script.RunSimulation(); 
        }

        if (GUILayout.Button("Pause simulation"))
        {
            script.PauseSimulation();
        }

        if (GUILayout.Button("Run single step"))
        {
            script.RunSingleStep();
        }
    }
}