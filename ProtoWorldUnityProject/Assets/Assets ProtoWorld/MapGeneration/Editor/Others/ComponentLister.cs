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


public class ComponentLister : EditorWindow
{
    private Hashtable sets = new Hashtable();
    private Vector2 scrollPosition;



    //[MenuItem("Component/Component lister")]
    public static void Launch()
    {
        EditorWindow window = GetWindow(typeof(ComponentLister));
        window.Show();
    }



    public void UpdateList()
    {
        Object[] objects;

        sets.Clear();

        objects = FindObjectsOfType(typeof(Component));
        foreach (Component component in objects)
        {
            if (!sets.ContainsKey(component.GetType()))
            {
                sets[component.GetType()] = new ArrayList();
            }

            ((ArrayList)sets[component.GetType()]).Add(component.gameObject);
        }
    }



    public void OnGUI()
    {
        GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));
        GUILayout.Label("Components in scene:");
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh"))
        {
            UpdateList();
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (System.Type type in sets.Keys)
        {
            GUILayout.Label(type.Name + ":");
            foreach (GameObject gameObject in (ArrayList)sets[type])
            {
                if (GUILayout.Button(gameObject.name))
                {
                    Selection.activeObject = gameObject;
                }
            }
        }

        GUILayout.EndScrollView();
    }
}
