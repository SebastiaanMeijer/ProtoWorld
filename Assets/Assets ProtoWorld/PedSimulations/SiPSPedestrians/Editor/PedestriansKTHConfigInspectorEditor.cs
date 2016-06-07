/*
 * 
 * PEDESTRIANS KTH
 * PedestriansKTHConfigInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI in the ModeDB Inspector.
/// </summary>
[CustomEditor(typeof(PedestriansKTHConfig))]
public class PedestriansKTHConfigInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PedestriansKTHConfig script = (PedestriansKTHConfig)target;

        if (GUILayout.Button("Print metro schedule"))
        {
            script.transform.GetComponentInChildren<ModeDB>().PrintMetroSchedule();
        }

        if (GUILayout.Button("Print bus schedule"))
        {
            script.transform.GetComponentInChildren<ModeDB>().PrintBusSchedule();
        }
    }
}