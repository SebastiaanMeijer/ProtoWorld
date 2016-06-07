/*
 * 
 * PEDESTRIANS KTH
 * ModeDBInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI in the ModeDB Inspector.
/// </summary>
[CustomEditor(typeof(ModeDB))]
public class ModeDBInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ModeDB script = (ModeDB)target;

        if (GUILayout.Button("Introduce 30 min delay in buses"))
        {
           script.DelayBuses(1800.0f); 
        }

        if (GUILayout.Button("Introduce 30 min delay in metros"))
        {
            script.DelayMetros(1800.0f);
        }

        if (GUILayout.Button("Cancel next bus"))
        {
            script.CancelNextBus();
        }

        if (GUILayout.Button("Cancel next metro"))
        {
            script.CancelNextMetro();
        }

        if (GUILayout.Button("Print metro schedule"))
        {
            script.PrintMetroSchedule();
        }

        if (GUILayout.Button("Print bus schedule"))
        {
            script.PrintBusSchedule();
        }
    }
}