//recreated by Neodrop. 
//mailto : neodrop@unity3d.ru
using UnityEngine;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.

Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
*/
//[ExecuteInEditMode()]
[AddComponentMenu("Mesh/Combine Children Extended")]
public class CombineChildrenExtended : MonoBehaviour {
	
	/// Usually rendering with triangle strips is faster.
	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
	/// Best is to try out which value is faster in practice.
    public int frameToWait = 0;
	public bool generateTriangleStrips = true, combineOnStart = true, destroyAfterOptimized = false, castShadow = true, receiveShadow = true, keepLayer = true, addMeshCollider = false;
	
    void Start()
    {
        if (combineOnStart)
        {
            if (frameToWait == 0) 
                Combine();
            else 
                StartCoroutine(CombineLate());
        }
    }

    IEnumerator CombineLate()
    {
        for (int i = 0; i < frameToWait; i++ ) yield return 0;
        Combine();
    }

    [ContextMenu("Combine Now on Childs")]
    public void CallCombineOnAllChilds()
    {
#if UNITY_EDITOR
        if(!Application.isPlaying)
            Undo.RegisterSceneUndo("Combine meshes");
#endif
        CombineChildrenExtended[] c = gameObject.GetComponentsInChildren<CombineChildrenExtended>();
        int count = c.Length;
        for (int i = 0; i < count; i++) if(c[i] != this)c[i].Combine();
        combineOnStart = enabled = false;
    }

	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
    [ContextMenu ("Combine Now")]
	public void Combine () {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            Undo.RegisterSceneUndo("Combine meshes");
#endif
		Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Hashtable materialToMesh= new Hashtable();
		
		for (int i=0;i<filters.Length;i++) {
			MeshFilter filter = (MeshFilter)filters[i];
			Renderer curRenderer  = filters[i].GetComponent<Renderer>();
			MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance ();
			instance.mesh = filter.sharedMesh;
			if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
				Material[] materials = curRenderer.sharedMaterials;
				for (int m=0;m<materials.Length;m++) {
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
					ArrayList objects = (ArrayList)materialToMesh[materials[m]];
					if (objects != null) {
						objects.Add(instance);
					}
					else
					{
						objects = new ArrayList ();
						objects.Add(instance);
						materialToMesh.Add(materials[m], objects);
					}
				}
                if (Application.isPlaying && destroyAfterOptimized && combineOnStart) Destroy(curRenderer.gameObject);
                else if (destroyAfterOptimized) DestroyImmediate(curRenderer.gameObject);
				else curRenderer.enabled = false;
			}
		}
	
		foreach (DictionaryEntry de  in materialToMesh) {
			ArrayList elements = (ArrayList)de.Value;
			MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

			// We have a maximum of one material, so just attach the mesh to our own game object
			if (materialToMesh.Count == 1)
			{
				// Make sure we have a mesh filter & renderer
				if (GetComponent(typeof(MeshFilter)) == null)
					gameObject.AddComponent(typeof(MeshFilter));
				if (!GetComponent("MeshRenderer"))
					gameObject.AddComponent<MeshRenderer>();
	
				MeshFilter filter = (MeshFilter)GetComponent(typeof(MeshFilter));
                if (Application.isPlaying) filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                else filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
				GetComponent<Renderer>().material = (Material)de.Key;
				GetComponent<Renderer>().enabled = true;
                if (addMeshCollider) gameObject.AddComponent<MeshCollider>();
                GetComponent<Renderer>().shadowCastingMode = castShadow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
                GetComponent<Renderer>().receiveShadows = receiveShadow;
			}
			// We have multiple materials to take care of, build one mesh / gameobject for each material
			// and parent it to this object
			else
			{
				GameObject go = new GameObject("Combined mesh");
                if (keepLayer) go.layer = gameObject.layer;
				go.transform.parent = transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent<MeshRenderer>();
				go.GetComponent<Renderer>().material = (Material)de.Key;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				if(Application.isPlaying)filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                else filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                go.GetComponent<Renderer>().shadowCastingMode = castShadow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
                go.GetComponent<Renderer>().receiveShadows = receiveShadow;
                if (addMeshCollider) go.AddComponent<MeshCollider>();
            }
		}	
	}

    [ContextMenu("Save mesh as asset")]
    void SaveMeshAsAsset()
    {
#if UNITY_EDITOR
        var path = EditorUtility.SaveFilePanelInProject("Save mesh asset", "CombinedMesh", "asset", "Select save file path");
        if(!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(GetComponent<MeshFilter>().sharedMesh, path);
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
#endif
    }
}