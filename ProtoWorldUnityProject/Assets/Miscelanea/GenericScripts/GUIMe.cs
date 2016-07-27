/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
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
