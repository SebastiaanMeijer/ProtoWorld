using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

[CustomEditor(typeof(FlashCreator))]
public class FlashCreatorEditor : Editor
{
    private enum FlashOptions
    {
        Spawner = 0,
        Destination = 1
    }

    private FlashCreator creator;

    private static string spawnerName = "FlashSpawner";
    private static string destinationName = "FlashDestination";
    private static string moduleName = "FlashPedestriansModule";

    private FlashOptions op;
    private static bool editing = false;
    private static bool adding = false;

    void OnEnable()
    {
        creator = target as FlashCreator;
        FindSpawnersAndDestinations();
        CheckModuleExist();
    }

    public override void OnInspectorGUI()
    {
        GUI.color = Color.gray;
        DrawDefaultInspector();
        //creator = target as FlashCreator;
        if (adding || editing)
        {

            if (adding)
            {
                GUI.color = Color.green;
                GUILayout.Label("Choose one of the prefabs:");

                GUI.color = Color.white;
                op = (FlashOptions)EditorGUILayout.EnumPopup("Prefab to create:", op);
                GUILayout.Space(10);

                GUILayout.FlexibleSpace();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Back"))
                {
                    creator.ReleaseEditObject();
                    editing = false;
                    adding = false;
                }
            }

            if (editing)
            {
                GUI.color = Color.green;
                GUILayout.Label("Select an existing spawner or destination");
                GUILayout.Label("in the scene to edit the position...");

                ShowProperties();

                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                if (GUILayout.Button("Remove"))
                {
                    var option = EditorUtility.DisplayDialogComplex(
                        "Spawners and Destinations",
                        "Do you want to remove?",
                        "Yes",
                        "No",
                        "Cancel");
                    if (option == 0)
                    {
                        creator.RemoveEditObject();
                        FindSpawnersAndDestinations();
                        editing = false;
                    }
                }

                GUILayout.FlexibleSpace();
                GUI.color = Color.white;
                if (GUILayout.Button("Undo"))
                {
                    var option = EditorUtility.DisplayDialogComplex(
                        "Spawners and Destinations",
                        "Do you want to undo your changes?",
                        "Yes",
                        "No",
                        "Cancel");
                    if (option == 0)
                    {
                        creator.UndoChanges();
                    }
                }

                GUILayout.FlexibleSpace();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Save and go back"))
                {
                    creator.ReleaseEditObject();
                    editing = false;
                }

            }
        }
        else
        {
            GUILayout.Label("Start by choosing one of the options:");
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                adding = true;
            }
            GUILayout.Space(10);

            GUI.color = Color.green;
            if (GUILayout.Button("Edit"))
            {
                FindSpawnersAndDestinations();
                editing = true;
            }

            GUILayout.FlexibleSpace();

            GUI.color = Color.white;
            GUILayout.Label("Press this button to exit 'Pedestrian Editor'");
            GUI.color = Color.yellow;
            if (GUILayout.Button("Exit"))
            {
                editing = false;
                DestroyImmediate(creator.gameObject);
                Selection.activeGameObject = GameObject.Find(moduleName);

            }
        }
    }

    void ShowProperties()
    {
        if (creator.selectedObject == null)
            return;

        var sObj = new SerializedObject(creator.selectedObject);
        var property = sObj.GetIterator();
        GUI.color = Color.yellow;
        EditorGUILayout.BeginVertical("box");
        while (property.Next(true))
        {
            if (property.propertyType.Equals(SerializedPropertyType.ObjectReference))
            {
                var refVal = property.objectReferenceValue;
                if (refVal is MonoBehaviour)
                {
                    var path = property.objectReferenceValue.ToString();
                    if (path.Contains("FlashPedestrians"))
                    {
                        var m_Object = new SerializedObject(property.objectReferenceValue);
                        var m_Property = m_Object.GetIterator();

                        int propCounter = 0;
                        int propLimit = 0;
                        if (path.Contains("Destination"))
                        {
                            propLimit = 2;
                        }
                        else if (path.Contains("Spawner"))
                        {
                            propLimit = 5;
                        }

                        while (m_Property.NextVisible(true) && propCounter < propLimit)
                        {
                            if (!m_Property.propertyType.Equals(SerializedPropertyType.ObjectReference))
                            {
                                EditorGUILayout.PropertyField(m_Property);
                                propCounter++;
                            }
                        }
                        m_Property.Reset();
                        // Apply the property, handle undo
                        m_Object.ApplyModifiedProperties();
                    }
                }
            }

        }
        EditorGUILayout.EndVertical();
        property.Reset();
    }

    void OnSceneGUI()
    {

        var contorlId = GUIUtility.GetControlID(FocusType.Passive);
        Ray worldRay;
        RaycastHit hit;
        if (Event.current.button == 0)
        {
            switch (Event.current.GetTypeForControl(contorlId))
            {
                case EventType.MouseDown:
                    if (editing)
                    {
                        if (!creator.IsObjectSelected())
                        {
                            worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                            if (Physics.Raycast(worldRay, out hit))
                            {
                                creator.SetSelectedObjectIfHit(hit);
                            }
                            Event.current.Use();
                        }
                    }
                    else if (adding)
                    {
                        worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        if (Physics.Raycast(worldRay, out hit))
                        {
                            AddPrefabToScene(op, hit.point);
                            FindSpawnersAndDestinations();
                            adding = false;
                            creator.SetSelectedObjectIfHit(hit);
                            editing = true;
                        }
                        Event.current.Use();
                    }
                    GUIUtility.hotControl = contorlId;
                    break;
                case EventType.MouseUp:
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    break;
                case EventType.MouseDrag:
                    if (editing)
                    {
                        if (creator.IsObjectSelected())
                        {
                            worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                            if (Physics.Raycast(worldRay, out hit))
                            {
                                creator.MoveEditObject(hit.point);
                            }
                            Event.current.Use();
                        }
                    }
                    GUIUtility.hotControl = contorlId;
                    break;
            }
        }
    }


    private void FindSpawnersAndDestinations()
    {
        creator.spawners = FindObjectsOfType<FlashPedestriansSpawner>();
        creator.destinations = FindObjectsOfType<FlashPedestriansDestination>();
    }

    private void AddPrefabToScene(FlashOptions op, Vector3 point)
    {
        //CheckModuleExist();
        GameObject parent;
        switch (op)
        {
            case FlashOptions.Spawner:
                parent = GameObject.Find("SpawnerPoints");
                //InstantiatePrefab(spawnerPrefab, point, parent);
                InstantiatePrefab(spawnerName, point, parent);
                break;
            case FlashOptions.Destination:
                parent = GameObject.Find("DestinationPoints");
                //InstantiatePrefab(destinationPrefab, point, parent);
                InstantiatePrefab(destinationName, point, parent);
                break;
        }
    }

    private void InstantiatePrefab(string prefab, Vector3 point, GameObject parent = null)
    {
        var instance = ProtoWorldMenu.AddPrefabToScene(prefab);
        instance.transform.position = point;
        if (parent != null)
            instance.transform.SetParent(parent.transform);
        //Undo.RegisterCreatedObjectUndo(instance, "Prefab instantiated...");
    }

    private void InstantiatePrefab(GameObject prefab, Vector3 point, GameObject parent = null)
    {
        var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        PrefabUtility.DisconnectPrefabInstance(instance);
        instance.transform.position = point;
        if (parent != null)
            instance.transform.SetParent(parent.transform);
        //EditorUtility.DisplayDialog("Prefab instantiated...", prefab.name + "was added to the scene", "OK");
    }

    private void CheckModuleExist()
    {
        ProtoWorldMenu.AddModuleIfNotExist(moduleName);
        //var module = GameObject.Find(moduleName);
        //if (module == null)
        //{
        //    PrefabUtility.DisconnectPrefabInstance(PrefabUtility.InstantiatePrefab(pedestrianModule));
        //    //EditorUtility.DisplayDialog("GameObject added...", moduleName + "was added to the scene", "OK");
        //}
    }
}

