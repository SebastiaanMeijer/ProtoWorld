using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlashCreator : MonoBehaviour
{

    [Range(1, 150)]
    public float gizmoRadius = 50;
    [HideInInspector]
    public FlashPedestriansSpawner[] spawners;
    [HideInInspector]
    public FlashPedestriansDestination[] destinations;
    [HideInInspector]
    public GameObject selectedObject;
    [HideInInspector]
    public GameObject selectedClone;
    [HideInInspector]
    public Vector3 undoPosition;

    // Use this for initialization
    void Start()
    {
        Destroy(this);
    }

    public void UndoChanges()
    {
        if (IsObjectSelected())
        {
            bool foundSpawnOrDest = false;
            MoveEditObject(undoPosition);
            var spawnClone = selectedClone.GetComponent<FlashPedestriansSpawner>();
            if (spawnClone != null)
            {
                UndoComponent<FlashPedestriansSpawner>(spawnClone, selectedObject);
                foundSpawnOrDest = true;
            }

            var destinationClone = selectedClone.GetComponent<FlashPedestriansDestination>();
            if (destinationClone != null)
            {
                UndoComponent<FlashPedestriansDestination>(destinationClone, selectedObject);
                foundSpawnOrDest = true;
            }
            if (!foundSpawnOrDest)
                Debug.Log("Strange: No spawner and destionation?");
        }
    }

    T UndoComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.GetComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            //Debug.Log(field.Name);
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    public void RemoveEditObject()
    {
        DestroyImmediate(selectedObject);
        ReleaseEditObject();
    }

    public void ReleaseEditObject()
    {
        selectedObject = null;
        undoPosition = Vector3.zero;
        DestroyImmediate(selectedClone);
        selectedClone = null;
    }

    public void MoveEditObject(Vector3 position)
    {
        selectedObject.transform.position = position;
    }

    public void SetEditObject(GameObject selected)
    {
        selectedObject = selected;
        undoPosition = selectedObject.transform.position;
        selectedClone = Instantiate(selected);
        selectedClone.transform.SetParent(transform);
    }

    public bool IsObjectSelected()
    {
        return (selectedObject != null);
    }

    public void SetSelectedObjectIfHit(RaycastHit hit)
    {
        if (spawners != null)
        {
            foreach (var sp in spawners)
            {
                if (Vector3.Distance(hit.point, sp.transform.position) <= gizmoRadius)
                {
                    SetEditObject(sp.gameObject);
                    return;
                }
            }
        }

        if (destinations != null)
        {
            foreach (var dt in destinations)
            {
                if (Vector3.Distance(hit.point, dt.transform.position) <= gizmoRadius)
                {
                    SetEditObject(dt.gameObject);
                    return;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (spawners != null && spawners.Length > 0)
        {
            foreach (var spawner in spawners)
            {
                if (selectedObject != null && spawner.gameObject.Equals(selectedObject))
                    DrawGizmo(selectedObject, Color.yellow, Color.black, gizmoRadius);
                else
                    DrawGizmo(spawner.gameObject, Color.green, Color.black, gizmoRadius);
            }
        }
        if (destinations != null && destinations.Length > 0)
        {
            foreach (var destination in destinations)
            {
                if (selectedObject != null && destination.gameObject.Equals(selectedObject))
                    DrawGizmo(selectedObject, Color.yellow, Color.black, gizmoRadius);
                else
                    DrawGizmo(destination.gameObject, Color.white, Color.black, gizmoRadius);
            }
        }
    }

    void DrawGizmo(GameObject gameObject, Color wireColor, Color solidColor, float radius)
    {
        if (gameObject == null)
            return;
        solidColor.a = 0.5f;
        Gizmos.color = solidColor;
        Gizmos.DrawSphere(gameObject.transform.position, radius);
        wireColor.a = 1f;
        Gizmos.color = wireColor;
        Gizmos.DrawWireSphere(gameObject.transform.position, radius);
    }
}
