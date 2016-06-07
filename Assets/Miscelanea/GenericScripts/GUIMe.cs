using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;

public class GUIMe : MonoBehaviour
{



    public string className = "int";
    private string oldclassName = "";
    private Type t;
    private int WindowId = 23843429;
    private FieldInfo[] fields;
    private PropertyInfo[] members;
    public Transform target;
    public string component;
    private Component c;
    [Compact]
    public Rect WindowSize = new Rect(100, 100, 400, 400);
    void OnGUI()
    {
        c = target.GetComponent(component);
        t = target.GetComponent(component).GetType();
        members = t.GetProperties().ToArray();
        if (t != null)
            WindowSize = GUILayout.Window(WindowId, WindowSize, GenerateGUI, "Generated from class" + t.Name);

    }
    Vector2 scrollPosition;
    void GenerateGUI(int id)
    {
        if (id == WindowId)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.Label("Members:");
            foreach (var m in members)
            {
                switch (m.PropertyType.Name)
                {

                    case "Vector3":
                        {
                            GUILayout.Label("VECTOR3");
                            break;
                        }
                    default:
                        {
                            GUILayout.Label(m.PropertyType + " " + m.Name + ": " + m.GetValue(c, null).ToString());
                            break; 
                        }
                }
                
            }
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}
