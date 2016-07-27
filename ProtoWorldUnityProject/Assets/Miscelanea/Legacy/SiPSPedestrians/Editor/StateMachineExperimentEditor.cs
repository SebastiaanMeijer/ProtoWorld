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