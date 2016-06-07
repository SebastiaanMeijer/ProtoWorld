using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(KPIFeeder))]
public class KPIFeederEditor : Editor
{
    KPIFeeder m_Feeder;

    SerializedObject m_Object;
    SerializedProperty m_Property;

    void OnEnable()
    {
        m_Feeder = target as KPIFeeder;
    }

    public override void OnInspectorGUI()
    {
        // Draw KPIFeeder to be able to set the gameobject of interest.
        EditorGUILayout.BeginVertical("box");
        DrawDefaultInspector();
        EditorGUILayout.EndVertical();

        // Draw the chosen kpis.
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Chosen KPIs: " + m_Feeder.kpiStrings.Count, EditorStyles.boldLabel);
        GUI.color = Color.red;
        if (GUILayout.Button("Clear KPIs"))
        {
            m_Feeder.RemoveAllKPIs();
        }
        GUI.color = Color.yellow;
        if (GUILayout.Button("Apply settings"))
        {
            // When the feeder is a prefab, the settings are reverted for some reason.
            PrefabUtility.DisconnectPrefabInstance(m_Feeder);

            m_Object = new SerializedObject(m_Feeder);
            m_Property = m_Object.GetIterator();
            m_Object.ApplyModifiedProperties();

            //m_Feeder.ApplySettings();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        for (int i = 0; i < m_Feeder.kpiStrings.Count; i++)
        {
            GUI.color = Color.white;
            EditorGUILayout.BeginHorizontal();
            m_Feeder.kpiNames[i] = EditorGUILayout.TextField(m_Feeder.kpiNames[i]);
            m_Feeder.kpiColors[i] = EditorGUILayout.ColorField(m_Feeder.kpiColors[i]);
            EditorGUILayout.EndHorizontal();

            GUI.color = Color.green;
            if (GUILayout.Button(m_Feeder.GetButtonString(i)))
            {
                m_Feeder.RemoveKPI(m_Feeder.kpiStrings[i]);
            }
        }
        EditorGUILayout.EndVertical();
        // Draw the public variables in the gameobject of interest.
        for (int i = 0; i < m_Feeder.gameObjects.Count; i++)
        {
            GUI.color = Color.yellow;
            var go = m_Feeder.gameObjects[i];
            if (go == null)
                continue;

            m_Object = new SerializedObject(go);
            m_Property = m_Object.GetIterator();
            EditorGUILayout.BeginVertical("box");
            var foundProp = false;
            while (m_Property.Next(true))
            {
                if (m_Property.propertyType.Equals(SerializedPropertyType.ObjectReference))
                {
                    var refVal = m_Property.objectReferenceValue;
                    if (refVal is MonoBehaviour)
                    {
                        var path = m_Property.objectReferenceValue.ToString();
                        if (!path.Contains("UI."))
                        {
                            var props = new SerializedObject(m_Property.objectReferenceValue).GetIterator();
                            while (props.NextVisible(true))
                            {
                                var propType = props.propertyType;
                                if (!propType.Equals(SerializedPropertyType.ObjectReference))
                                {
                                    if (propType.Equals(SerializedPropertyType.Integer) || propType.Equals(SerializedPropertyType.Float))
                                    {
                                        foundProp = true;
                                        var str = path + ":" + props.name + "." + go.GetHashCode();
                                        if (!m_Feeder.kpiStrings.Contains(str))
                                        {
                                            GUI.color = Color.cyan;
                                            if (GUILayout.Button(props.displayName))
                                            {
                                                m_Feeder.AddKPI(str, props.displayName, propType.ToString());
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    GUI.color = Color.white;
                                    EditorGUILayout.PropertyField(props);
                                }
                            }
                            props.Reset();
                        }

                    }
                }
            }
            if (!foundProp)
            {
                EditorGUILayout.LabelField("Nothing chartable found in " + go.name);
            }
            EditorGUILayout.EndVertical();

            m_Property.Reset();

            // Apply the property, handle undo
            m_Object.ApplyModifiedProperties();
        }



    }
}
