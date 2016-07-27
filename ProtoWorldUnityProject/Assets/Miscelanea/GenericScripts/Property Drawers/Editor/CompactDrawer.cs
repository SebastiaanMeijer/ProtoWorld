/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

    using UnityEditor;
    using UnityEngine;
     
    [CustomPropertyDrawer(typeof(CompactAttribute))]
    public class CompactDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            System.Type objectType = prop.serializedObject.targetObject.GetType();
            System.Type propertyType = objectType.GetField(prop.name).FieldType;
           
            if (propertyType == typeof(Vector4))
            {
                SerializedProperty xProp = prop.FindPropertyRelative("x");
                SerializedProperty yProp = prop.FindPropertyRelative("y");
                SerializedProperty zProp = prop.FindPropertyRelative("z");
                SerializedProperty wProp = prop.FindPropertyRelative("w");
                Vector4 vector4Value = new Vector4(xProp.floatValue, yProp.floatValue, zProp.floatValue, wProp.floatValue);
               
                EditorGUIUtility.LookLikeControls();
                position.x += 4;
                position.width -= 8;
                EditorGUI.BeginChangeCheck();
                vector4Value = EditorGUI.Vector4Field(position, label.text, vector4Value);
                if (EditorGUI.EndChangeCheck())
                {
                    xProp.floatValue = vector4Value.x;
                    yProp.floatValue = vector4Value.y;
                    zProp.floatValue = vector4Value.z;
                    wProp.floatValue = vector4Value.w;
                }
            }
            else
            {
                switch (prop.propertyType)
                {
                	case SerializedPropertyType.Rect:
                		{
                			Rect rectValue = prop.rectValue;
                			EditorGUIUtility.LookLikeControls();
                			position.x += 4;
                			position.width -= 8;
                			EditorGUI.BeginChangeCheck();
                			rectValue = EditorGUI.RectField(position,label.text,rectValue);
                			if (EditorGUI.EndChangeCheck())
                			{
                				prop.rectValue=rectValue;
                			}
                			break;
                		}
                case SerializedPropertyType.Vector2:
                    {
                        EditorGUIUtility.LookLikeControls();
                        position.x += 4;
                        position.width -= 8;
                        EditorGUI.BeginChangeCheck();
                        Vector2 vector2Value = EditorGUI.Vector2Field(position, label.text, prop.vector2Value);
                        if (EditorGUI.EndChangeCheck())
                        {
                            prop.vector2Value = vector2Value;
                        }
                    }
                    break;
                case SerializedPropertyType.Vector3:
                    {
                        EditorGUIUtility.LookLikeControls();
                        position.x += 4;
                        position.width -= 8;
                        EditorGUI.BeginChangeCheck();
                        Vector3 vector3Value = EditorGUI.Vector3Field(position, label.text, prop.vector3Value);
                        if (EditorGUI.EndChangeCheck())
                        {
                            prop.vector3Value = vector3Value;
                        }
                    }
                    break;
                default:
                    {
                        Debug.LogError("Compact attribute only works for Vector2, Vector3 and Vector4");
                    }
                    break;
                }
            }
        }
       
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
        	if (prop.propertyType== SerializedPropertyType.Rect)
            	return base.GetPropertyHeight(prop, label) * 2 + 4 + 16;
        	else
        		return base.GetPropertyHeight(prop, label) * 2 + 4;
        }
    }