/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEditor;
using UnityEngine;
using System;

/// <summary>
/// Creates a popup list with the provided values.
/// </summary>

[CustomPropertyDrawer(typeof(PopupAttribute))]
public class PopupDrawer : PropertyDrawer
{
    PopupAttribute popupAttribute { get { return ((PopupAttribute)attribute); } }
    int index;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Checks to see what is the type of the provided values and acts accordingly.
        if (popupAttribute.variableType == typeof(int[]))
        {
            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(position, label.text, property.intValue, popupAttribute.list);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = index;
            }
        }
        else if (popupAttribute.variableType == typeof(float[]))
        {
            EditorGUI.BeginChangeCheck();
            // Checks all items in the provided list, to see if any of them is a match with the property value, if so assigns that value to the index.
            for (int i = 0; i < popupAttribute.list.Length; i++)
            {
                if (property.floatValue == Convert.ToSingle(popupAttribute.list[i]))
                {
                    index = i;
                }
            }
            index = EditorGUI.Popup(position, label.text, index, popupAttribute.list);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = Convert.ToSingle(popupAttribute.list[index]);
            }
        }
        else if (popupAttribute.variableType == typeof(string[]))
        {
            EditorGUI.BeginChangeCheck();
            // Checks all items in the provided list, to see if any of them is a match with the property value, if so assigns that value to the index.
            for (int i = 0; i < popupAttribute.list.Length; i++)
            {
                if (property.stringValue == popupAttribute.list[i])
                {
                    index = i;
                }
            }
            index = EditorGUI.Popup(position, label.text, index, popupAttribute.list);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = popupAttribute.list[index];
            }
        }
        else
        {
            EditorGUI.LabelField(position, "ERROR READ CONSOLE FOR MORE INFO");
        }
    }
}