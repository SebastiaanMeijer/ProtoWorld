using UnityEngine;
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
