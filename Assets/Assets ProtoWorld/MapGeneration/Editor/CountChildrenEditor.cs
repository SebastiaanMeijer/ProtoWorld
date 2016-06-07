using UnityEngine;
using UnityEditor;
using System.Collections;

public class CountChildrenEditor : Editor
{

    [MenuItem("GameObject/Count children")]
    static void CountChildren()
    {
        var parents = Selection.transforms;
        try
        {
            if (parents.Length != 0)
            {
                int children = 0;
                foreach (var parent in parents)
                {
                    children += parent.GetComponentsInChildren<Transform>().Length;
                }
                EditorUtility.DisplayDialog("Object count", "Total children: " + children, "OK");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

    }

    [MenuItem("GameObject/DuplicateToTenthousand")]
    static void DuplicateTo10Thousand()
    {
        var parents = Selection.transforms;
        try
        {
            if (parents.Length != 0)
            {
                foreach (var parent in parents)
                {
                    var duplicate = (Transform)Object.Instantiate(parent);
                    duplicate.position = new Vector3(10000, 0, 10000);
                }
            }
            EditorUtility.DisplayDialog("Duplication complete", "Duplicated successfully", "OK");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

    }
}
