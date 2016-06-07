/*
 * 
 * PEDESTRIANS KTH
 * PedestrianKTHKnowledgeInspectorEditor.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Creates a GUI in the PedestrianKTHKnowledge Inspector.
/// </summary>
[CustomEditor(typeof(PedestrianKTHKnowledge))]
public class PedestrianKTHKnowledgeInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PedestrianKTHKnowledge script = (PedestrianKTHKnowledge)target;

        if (GUILayout.Button("Print current knowledge"))
        {
           script.PrintKnowledge(); 
        }
    }
}