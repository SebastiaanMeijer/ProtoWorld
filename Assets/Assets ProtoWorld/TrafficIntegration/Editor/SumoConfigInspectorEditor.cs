/*
 * 
 * SUMO COMMUNICATION
 * SumoConfigInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI in the SumoConfig Inspector for communication with SUMO.
/// </summary>
[CustomEditor(typeof(SumoConfig))]
public class SumoConfigInspectorEditor : Editor
{
    private int busCounter = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SumoConfig script = (SumoConfig)target;

        if (GUILayout.Button("Run simulation"))
        {
           script.gameObject.GetComponentInChildren<SumoMainController>().RunSimulation(); 
        }

        if (GUILayout.Button("Pause simulation"))
        {
            script.gameObject.GetComponentInChildren<SumoMainController>().PauseSimulation();
        }

        if (GUILayout.Button("Run single step (only if pause)"))
        {
            script.gameObject.GetComponentInChildren<SumoMainController>().RunSingleStep();
        }

        if (GUILayout.Button("Spawn bus"))
        {
            script.gameObject.GetComponentInChildren<SumoMainController>().AddNewVehicle("Bus" + (busCounter++), "BUS", "busRoute", -2, -4, 0, 0);
        }
    }
}