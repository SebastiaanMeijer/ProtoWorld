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
using System.Linq;

[CustomEditor(typeof(MeshFilter))]
[CanEditMultipleObjects]
public class MeshFilterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var filter = target as MeshFilter;

        DrawDefaultInspector();
        if (Selection.gameObjects.Length == 1)
        {
            if (filter.sharedMesh != null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("# of Vertices: " + filter.sharedMesh.vertexCount);
                EditorGUILayout.LabelField("# of Triangle Indices: " + filter.sharedMesh.triangles.Length);
                EditorGUILayout.LabelField("# of UVs: " + filter.sharedMesh.uv.Length);
                EditorGUILayout.EndVertical();
            }
        }
        else if (Selection.gameObjects.Length > 1)
        {
            var sel = Selection.gameObjects.Select(s => s.GetComponent<MeshFilter>()).Where(w => w != null).Where(w => w.sharedMesh != null).Select(s => s.sharedMesh).ToArray();
            int verts = 0;
            int tris = 0;
            int uvs = 0;
            for (int i = 0; i < sel.Length; i++)
            {
                verts += sel[i].vertexCount;
                tris += sel[i].triangles.Length;
                uvs += sel[i].uv.Length;
            }
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("# of Vertices: " + verts);
            EditorGUILayout.LabelField("# of Triangle Indices: " + tris);
            EditorGUILayout.LabelField("# of UVs: " + uvs);
            EditorGUILayout.EndVertical();
        }
    }
}
