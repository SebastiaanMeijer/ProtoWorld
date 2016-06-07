using UnityEngine;
using UnityEditor;
using System.Collections;

namespace GaPSLabsUnity.StateMachine
{
    [CustomEditor(typeof(StateMachineExperiment))]
    public class StateMachineExperimentEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            var o = target as StateMachineExperiment;
            var color = GUI.color;

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField("Current state:");
            GUI.color = Color.green;
            EditorGUILayout.LabelField(o.CurrentState);
            GUI.color = color;
            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("Time:");
                GUI.color = Color.green;
                EditorGUILayout.LabelField(Time.time + "");
                GUI.color = color;
                EditorGUILayout.EndHorizontal();
                this.Repaint();
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Class Session");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Start", GUILayout.MaxWidth(50));
            o.Session.Start = EditorGUILayout.FloatField(o.Session.Start);

            EditorGUILayout.LabelField("Finish", GUILayout.MaxWidth(50));
            o.Session.Finish = EditorGUILayout.FloatField(o.Session.Finish);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            base.OnInspectorGUI();

        }

    }
}