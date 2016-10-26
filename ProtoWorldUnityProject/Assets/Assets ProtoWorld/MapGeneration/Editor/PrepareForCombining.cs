/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * GAPSLABS EXTENDED EDITOR
 * Aram Azhari
 * 
 * Reviewed by Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class PrepareForCombining : Editor
{
    /// -----------------------------------------------------------------------
    /// MENU ITEMS FOR OPTIMIZING ELEMENTS 
    /// -----------------------------------------------------------------------

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Group Close Buildings")]
    public static void PrepareForCombiningBuildings()
    {
        TurnBuildingCollidersOff();
        PrepareForCombiningMethod("Buildings", "Building", false);
        CombineMeshes("Buildings", "Building", false);
        TurnBuildingCollidersOn();
    }

    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Group Close Roads")]
    public static void PrepareForCombiningRoads()
    {
        TurnRoadCollidersOff();
        PrepareForCombiningMethod("Lines", "Line", false);
        CombineMeshes("Lines", "Line", false);
        TurnRoadCollidersOn();
    }

    /// <summary>
    /// Turns off the colliders for objects with tag "Building".
    /// </summary>
    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Turn Colliders Off (Buildings)")]
    public static void TurnBuildingCollidersOff()
    {
        var go = GameObject.FindGameObjectsWithTag("Building");
        for (int i = 0; i < go.Length; i++)
        {
            MeshCollider meshCol = go[i].GetComponent<MeshCollider>();
            if (!EditorUtility.DisplayCancelableProgressBar("Turning off colliders - Buildings", "", i / (float)(go.Length)) && meshCol != null)
                meshCol.enabled = false;
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Turns on the colliders for objects with tag "Building".
    /// </summary>
    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Turn Colliders On (Buildings)")]
    public static void TurnBuildingCollidersOn()
    {
        var go = GameObject.FindGameObjectsWithTag("Building");
        for (int i = 0; i < go.Length; i++)
        {
            MeshCollider meshCol = go[i].GetComponent<MeshCollider>();
            if (!EditorUtility.DisplayCancelableProgressBar("Turning on colliders - Buildings", "", i / (float)(go.Length)) && meshCol != null)
                meshCol.enabled = true;
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Turns off the colliders for objects with tag "Line".
    /// </summary>
    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Turn Colliders Off (Roads)")]
    public static void TurnRoadCollidersOff()
    {
        var go = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < go.Length; i++)
        {
            MeshCollider meshCol = go[i].GetComponent<MeshCollider>();
            if (!EditorUtility.DisplayCancelableProgressBar("Turning off colliders - Roads", "", i / (float)(go.Length)) && meshCol != null)
                meshCol.enabled = false;
        }
        EditorUtility.ClearProgressBar();

    }

    /// <summary>
    /// Turns on the colliders for objects with tags "Line".
    /// </summary>
    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Turn Colliders On (Roads)")]
    public static void TurnRoadCollidersOn()
    {
        var go = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < go.Length; i++)
        {
            MeshCollider meshCol = go[i].GetComponent<MeshCollider>();
            if (!EditorUtility.DisplayCancelableProgressBar("Turning on colliders - Roads", "", i / (float)(go.Length)) && meshCol != null)
                meshCol.enabled = true;
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Removes all the colliders of the objects with Building tag. Note that without the colliders, users will not be able to click on the buildings to get more information.
    /// </summary>
    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Remove Building Colliders")]
    static void RemoveBuildingsColliders()
    {
        var go2 = GameObject.FindGameObjectsWithTag("Building");
        for (int i = 0; i < go2.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Removing Colliders - Buildings", "", i / (float)(go2.Length)))
            {
                if (go2[i].GetComponent<Polygon>() != null)
                    DestroyImmediate(go2[i].GetComponent<Polygon>());
                DestroyImmediate(go2[i].GetComponent<MeshCollider>());
            }
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Removes all the colliders of the objects with Line tag. Note that without the colliders, pedestrians in SiPS will not be able to interact with the roads.
    /// </summary>
    [MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Remove Road Colliders")]
    static void RemoveRoadsColliders()
    {
        var go2 = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < go2.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Removing Colliders - Roads", "", i / (float)(go2.Length)))
            {
                if (go2[i].GetComponent<Polygon>() != null)
                    DestroyImmediate(go2[i].GetComponent<Polygon>());
                DestroyImmediate(go2[i].GetComponent<MeshCollider>());
            }
        }
        EditorUtility.ClearProgressBar();
    }

    /// -----------------------------------------------------------------------
    /// MENU ITEMS DEPRECATED
    /// -----------------------------------------------------------------------

    //[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Convert colliders to triggers")]
    static void ConvertLinesToColliderAsTrigger()
    {
        var go2 = GameObject.FindGameObjectsWithTag("Line");
        var go = GameObject.FindGameObjectsWithTag("Building");
        for (int i = 0; i < go2.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Converting to trigger colliders - Roads", "", i / (float)(go2.Length + go.Length)))
            {
                if (go2[i].GetComponent<MeshCollider>() != null)
                    go2[i].GetComponent<MeshCollider>().isTrigger = true;
            }
        }
        for (int i = 0; i < go.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Converting to trigger colliders - Roads", "", (i + go2.Length) / (float)(go2.Length + go.Length)))
            {
                if (go[i].GetComponent<MeshCollider>() != null)
                    go[i].GetComponent<MeshCollider>().isTrigger = true;
            }
        }
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Change Selected object height")]
    static void DecreaseHeight()
    {
        Selection.activeTransform.position = Selection.activeTransform.position + new Vector3(0, -0.3f, 0);
    }

    //[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Show vertices #&v")]
    static void ShowVertices()
    {
        if (Selection.activeTransform.childCount == 0)
        {
            var t = Selection.activeTransform.GetComponent<MeshFilter>().sharedMesh;
            Debug.Log("Vertex count: " + t.vertexCount
                + "\n" + "MaxX:" + t.vertices.Max(i => i.x)
                + "\n" + "MaxZ:" + t.vertices.Max(i => i.z)
                );
        }
        else
        {
            var t = Selection.activeTransform;
            int vertexTotal = 0;
            for (int i = 0; i < t.childCount; i++)
            {
                vertexTotal += t.GetChild(i).GetComponent<MeshFilter>().sharedMesh.vertexCount;
            }
            Debug.Log("Vertex count in children: " + vertexTotal);
        }
    }

    //[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Convert Building colliders to triggers")]
    static void ConvertBuildingsToColliderAsTrigger()
    {
        var go2 = GameObject.FindGameObjectsWithTag("Building");
        for (int i = 0; i < go2.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Converting to trigger colliders - Buildings", "", i / (float)(go2.Length)))
            {
                if (go2[i].GetComponent<MeshCollider>() != null)
                    go2[i].GetComponent<MeshCollider>().isTrigger = true;
            }
        }
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("ProtoWorld Editor/ProtoWorld Essentials/Map Tools/Advanced/Combine Close Roads")]
    static void PrepareForRoadCombine()
    {
        TurnRoadCollidersOff();
        var line = GameObject.Find("Lines");
        if (line == null)
        {
            Debug.LogError("Why is lines object null?");
            return;
        }
        var go = GameObject.FindGameObjectsWithTag("Line").Where(i => i.GetComponent<MeshFilter>() != null)
            .Where(i => i.GetComponent<MeshFilter>().sharedMesh.vertices.Length > 0).ToArray();
        Vector2 scale = new Vector2(go[0].transform.parent.localScale.x, go[0].transform.parent.localScale.z);
        Vector3 scaleVector = new Vector3(scale.x, 1, scale.y);
        Vector3 scaleVectorReverse = new Vector3(1 / scale.x, 1, 1 / scale.y);
        var minX = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].x).Min() * scale.x;
        var minZ = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].z).Min() * scale.y;
        var maxX = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].x).Max() * scale.x;
        var maxZ = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].z).Max() * scale.y;

        Vector3 stepSize = new Vector3(100, 0, 100);
        Vector3 lower = new Vector3(minX, 0, minZ);
        Vector3 upper = new Vector3(maxX, 0, maxZ);
        Vector3 mover = lower;
        //Debug.Log("lower " + lower);
        //Debug.Log("upper " + upper);
        int counter = 1;
        int vertCount = 0;
        for (float ii = lower.x; ii < upper.x; ii += stepSize.x)
            for (float j = lower.z; j < upper.z; j += stepSize.z)
            {
                if (
                    !EditorUtility.DisplayCancelableProgressBar(
                        "Grouping close roads"
                        , string.Format("Row: {0}/{1}, Column: {2}/{3} - {4} %", ii, upper.x, j, upper.z, 100 * (ii / upper.x) + (j / upper.z))
                        , ii / upper.x))
                {
                    vertCount = 0;
                    GameObject temp = new GameObject("group" + counter);
                    temp.transform.parent = line.transform;
                    temp.AddComponent<CombineChildren>();
                    for (int idx = 0; idx < go.Length; idx++)
                    {
                        var currentMesh = go[idx].GetComponent<MeshFilter>().sharedMesh;
                        var verZero = Vector3.Scale(currentMesh.vertices[0], scaleVector);
                        if (verZero.x >= ii && verZero.x <= ii + stepSize.x)
                            if (verZero.z >= j && verZero.z <= j + stepSize.z)
                            {
                                if (vertCount + currentMesh.vertexCount >= 65535)
                                {
                                    counter++;
                                    temp = new GameObject("group" + counter);
                                    temp.transform.parent = line.transform;
                                    temp.AddComponent<CombineChildren>();
                                    vertCount = 0;
                                }
                                go[idx].transform.parent = temp.transform;
                                vertCount += currentMesh.vertexCount;
                            }
                    }
                    counter++;
                }
            }

        EditorUtility.ClearProgressBar();
        var tempList = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        // Cleaning Up
        for (int i = 1; i < tempList.Length; i++)
        {
            if (!EditorUtility.DisplayCancelableProgressBar("Garbage Collection", "Cleaning up empty groups.. " + (100 * i / (float)tempList.Length) + " %", i / (float)tempList.Length))
            {
                if (tempList[i].name.StartsWith("group"))
                    if (tempList[i].transform.childCount == 0)
                        GameObject.DestroyImmediate(tempList[i]);
            }
            else break;
        }
        TurnRoadCollidersOn();
        EditorUtility.ClearProgressBar();
    }

    /// -----------------------------------------------------------------------
    /// AUXILIAR METHODS AND ATTRIBUTES
    /// -----------------------------------------------------------------------

    /// <summary>
    /// Combines the meshs of child objects with a tag 'childTag' in a parent 'ObjectGroup'. This will decrease the number of draw calls and conclusively increase the frame rate.
    /// </summary>
    /// <param name="ObjectGroup">The parent object name</param>
    /// <param name="childTag">The child tag to be filtered by</param>
    /// <param name="autoTurnOffColliders">Decides whether colliders should be turned off and on during the process.</param>
    static void CombineMeshes(string ObjectGroup, string childTag, bool autoTurnOffColliders = true)
    {
        var aramGisObject = GameObject.Find("AramGISBoundaries");
        var mapProperties = aramGisObject.GetComponent<MapBoundaries>();
        var parentObject = GameObject.Find(ObjectGroup);
        CombineChildren[] combinations = null;
		try {
			if(autoTurnOffColliders) {
				TurnRoadCollidersOff();
				TurnBuildingCollidersOff();
			}

			if(parentObject == null) {
				Debug.LogError("Why is " + ObjectGroup + " object null?");
				return;
			}

			combinations = parentObject.GetComponentsInChildren<CombineChildren>();
			for(int i = 0; i < combinations.Length; i++) {
				if(!EditorUtility.DisplayCancelableProgressBar("MeshCombineUtility running... ", "Combining meshes of " + combinations[i].name, 100f * i / combinations.Length)) {
					combinations[i].generateTriangleStrips = false;
					combinations[i].CombinedMeshName = "Combined_" + childTag + "_mesh";
					combinations[i].CombinedMeshTag = "Combined" + ObjectGroup;
					combinations[i].SendMessage("Start");

					// Set the layer to the layer the first matching child is in, which is likely
					// to be the layer all the children are in, since their materials are the same.
					GameObject combinedChild = GameObject.Find(combinations[i].CombinedMeshName);
					for(int index = 0; index < combinations[i].transform.childCount; index++) {
						GameObject child = combinations[i].transform.GetChild(index).gameObject;
						if(child.tag != combinations[i].CombinedMeshTag) {
							if(child.GetComponent<Renderer>().sharedMaterial.name == combinedChild.GetComponent<Renderer>().sharedMaterial.name) {
								combinedChild.layer = child.layer;
								break;
							}
						}
					}
				}
                else break;
            }
        }
        catch (System.Exception ex) { Debug.LogException(ex); }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (autoTurnOffColliders)
                TurnRoadCollidersOn();
            for (int i = 0; i < combinations.Length; i++)
                Object.DestroyImmediate(combinations[i]);
        }
    }

    /// <summary>
    /// Groups the children of the ObjectGroup into blocks of which is defined in CombinationOptimizationSize parameter of MapBoundaries. 
    /// It also covers the Unity limitation of 65535 vertices per mesh, and if this happens the rest of the objects are added to the next group.
    /// </summary>
    /// <param name="ObjectGroup">The parent object of the group. for ex. Buildings, Lines etc.</param>
    /// <param name="childTag">Filters the children by the specified tag</param>
    /// <param name="autoTurnOffColliders">Decides whether colliders should be turned off and on during the process.</param>
    static void PrepareForCombiningMethod(string ObjectGroup, string childTag, bool autoTurnOffColliders = true)
    {
        try
        {
            if (autoTurnOffColliders)
            {
                TurnRoadCollidersOff();
                TurnBuildingCollidersOff();
            }

            var aramGisObject = GameObject.Find("AramGISBoundaries");
            var mapProperties = aramGisObject.GetComponent<MapBoundaries>();

            var parentObject = GameObject.Find(ObjectGroup);
            if (parentObject == null)
            {
                Debug.LogError("Why is " + ObjectGroup + " object null?");
                return;
            }
            var go = GameObject.FindGameObjectsWithTag(childTag).Where(i => i.GetComponent<MeshCollider>() != null && i.GetComponent<MeshFilter>().sharedMesh.vertexCount != 0).ToArray();
            // Undoable.
            Undo.RecordObjects(go, "Combining " + ObjectGroup);

            Vector2 scale = new Vector2(go[0].transform.parent.localScale.x, go[0].transform.parent.localScale.z);
            Vector3 scaleVector = new Vector3(scale.x, 1, scale.y);
            Vector3 scaleVectorReverse = new Vector3(1 / scale.x, 1, 1 / scale.y);
            var minX = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].x + i.transform.position.x).Where(w => !float.IsNaN(w)).Min() * scale.x;
            var minZ = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].z + i.transform.position.z).Where(w => !float.IsNaN(w)).Min() * scale.y;
            var maxX = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].x + i.transform.position.x).Where(w => !float.IsNaN(w)).Max() * scale.x;
            var maxZ = go.Select(i => i.GetComponent<MeshFilter>().sharedMesh.vertices[0].z + i.transform.position.z).Where(w => !float.IsNaN(w)).Max() * scale.y;

            Vector3 stepSize = new Vector3(mapProperties.CombinationOptimizationSize.x, 0, mapProperties.CombinationOptimizationSize.y);
            Vector3 lower = new Vector3(minX, 0, minZ);
            Vector3 upper = new Vector3(maxX, 0, maxZ);
            Vector3 mover = lower;
            Debug.Log("lower " + lower);
            Debug.Log("upper " + upper);
            int counter = 1;
            int vertCount = 0;
            for (float ii = lower.x; ii < upper.x; ii += stepSize.x)
                for (float j = lower.z; j < upper.z; j += stepSize.z)
                {
                    if (
                        !EditorUtility.DisplayCancelableProgressBar(
                            "Grouping close objects"
                            , string.Format("Row: {0}/{1}, Column: {2}/{3} - {4} %", ii, upper.x, j, upper.z, 100 * (ii / upper.x) + (j / upper.z))
                            , ii / upper.x))
                    {
                        vertCount = 0;
                        GameObject temp = new GameObject("group" + counter);
                        temp.transform.parent = parentObject.transform;
                        temp.AddComponent<CombineChildren>();
                        for (int idx = 0; idx < go.Length; idx++)
                        {
                            var currentMesh = go[idx].GetComponent<MeshFilter>().sharedMesh;
                            var verZero = Vector3.Scale(currentMesh.vertices[0] + go[idx].transform.position, scaleVector);
                            if (verZero.x >= ii && verZero.x <= ii + stepSize.x)
                                if (verZero.z >= j && verZero.z <= j + stepSize.z)
                                {
                                    if (vertCount + currentMesh.vertexCount >= 65535)
                                    {
                                        counter++;
                                        temp = new GameObject("group" + counter);
                                        temp.transform.parent = parentObject.transform;
                                        temp.AddComponent<CombineChildren>();
                                        vertCount = 0;
                                    }
                                    go[idx].transform.parent = temp.transform;
                                    vertCount += currentMesh.vertexCount;
                                }
                        }
                        counter++;
                    }
                }

            EditorUtility.ClearProgressBar();
            var tempList = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            // Cleaning Up
            for (int i = 1; i < tempList.Length; i++)
            {
                if (!EditorUtility.DisplayCancelableProgressBar("Garbage Collection", "Cleaning up empty groups.. " + (100 * i / (float)tempList.Length) + " %", i / (float)tempList.Length))
                {
                    if (tempList[i].name.StartsWith("group"))
                        if (tempList[i].transform.childCount == 0)
                            GameObject.DestroyImmediate(tempList[i]);
                }
                else break;
            }
            if (autoTurnOffColliders)
                TurnRoadCollidersOn();
        }
        catch (System.Exception ex) { Debug.LogException(ex); }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
