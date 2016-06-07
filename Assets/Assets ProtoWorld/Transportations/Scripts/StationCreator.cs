using UnityEngine;
using System.Collections.Generic;

public class StationCreator : MonoBehaviour
{
    [HideInInspector]
    public Color normalColor = new Color(0, 0, 1, .5f);
    [HideInInspector]
    public Color editColor = new Color(1, 1, 0, .5f);

    [Range(1, 150)]
    public float stationGizmoRadius = 50;

    protected Dictionary<int, StationController> stationControllers;
    private static int NextStationId = 0;
    private StationController editingStation;
    private Vector3 undoPosition;
    private string undoName;

    public bool HasEditingStation { get { return (editingStation != null); } }

    /// <summary>
    /// Remove this script in game.
    /// </summary>
    void Start()
    {
        Destroy(this);
    }

    public void UpdateNextStationId(int id)
    {
        if (id >= NextStationId)
        {
            NextStationId = id + 1;
        }
    }

    public int GetNextStationId()
    {
        while (stationControllers.ContainsKey(NextStationId))
        {
            NextStationId++;
        }
        return NextStationId;
    }

    /// <summary>
    /// Return the number of stations in the dictionary.
    /// </summary>
    /// <returns></returns>
    public int GetStationCount()
    {
        if (stationControllers == null)
            return 0;
        else
            return stationControllers.Count;
    }

    /// <summary>
    /// Setting the station that will be edited. 
    /// Disable the collider, otherwise the station won't touch the ground in edit mode.
    /// </summary>
    /// <param name="station"></param>
    public void SetEditStation(StationController station)
    {
        if (station == null)
            return;
        editingStation = station;
        undoPosition = editingStation.transform.position;
        undoName = GetEditStationName();
        editingStation.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// Unselect the editing station.
    /// Enable the collider otherwise the Spawner can't find the station.
    /// </summary>
    public void ReleaseEditStation()
    {
        if (editingStation == null)
            return;
        editingStation.GetComponent<Collider>().enabled = true;
        editingStation = null;
    }

    /// <summary>
    /// Used by the editor to set the station-gameobjects found in the scene.
    /// </summary>
    /// <param name="transStations"></param>
    public void SetStations(StationController[] controllers)
    {
        stationControllers = new Dictionary<int, StationController>();
        foreach (var station in controllers)
        {
            stationControllers.Add(station.id, station);
            UpdateNextStationId(station.id);
        }

    }

    /// <summary>
    /// Add a new station to the scene and renumber the id based on existing ids.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="name"></param>
    /// <returns>StationController of the created station</returns>
    public StationController AddNewStation(Vector3 point, string name)
    {
        var station = StationController.CreateStation(GetNextStationId(), point, name);

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(station, "Prefab instantiated...");
#endif

        station.transform.SetParent(this.transform);
        stationControllers.Add(station.id, station);
        station.GetComponent<Collider>().enabled = true;
        //SetEditStation(station);
        return station;
    }

    /// <summary>
    /// Add a new station to the scene and put it as a child to Stations-gameobject.
    /// </summary>
    /// <param name="station"></param>
    /// <returns></returns>
    public StationController AddNewStation(Vector3 point)
    {
        var station = StationController.CreateStation(GetNextStationId(), point);

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(station, "Prefab instantiated...");
#endif

        station.transform.SetParent(this.transform);
        stationControllers.Add(station.id, station);
        station.GetComponent<Collider>().enabled = true;
        //SetEditStation(station);
        return station;
    }


    /// <summary>
    /// Set a new position of the station.
    /// </summary>
    /// <param name="position"></param>
    public void MoveEditingStation(Vector3 position)
    {
        editingStation.transform.position = position;
    }

    /// <summary>
    /// Draw the station as a gizmo in editor-mode.
    /// Two overlapping partly transparent cube will be drawn using 
    /// the defined normalColor and editColor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (stationControllers != null && stationControllers.Count > 0)
        {
            foreach (var station in stationControllers.Values)
            {
                if (station.Equals(editingStation))
                    DrawStation(editingStation.gameObject, editColor, normalColor, stationGizmoRadius);
                else
                    DrawStation(station.gameObject, normalColor, editColor, stationGizmoRadius);
            }
        }
    }

    /// <summary>
    /// Revert to the previous position and station name.
    /// </summary>
    public void RevertEditStation()
    {
        if (editingStation == null)
            return;
        MoveEditingStation(undoPosition);
        SetEditStationName(undoName);
    }

    /// <summary>
    /// Remove the editing station from the scene.
    /// </summary>
    public void RemoveStation()
    {
        if (editingStation == null)
            return;
        stationControllers.Remove(editingStation.id);

#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(editingStation.gameObject);
#endif

    }

    /// <summary>
    /// Get the station name, which is stored in StationController.
    /// </summary>
    /// <param name="station"></param>
    /// <returns></returns>
    public string GetStationName(StationController station)
    {
        if (station == null)
            return "no station name";
        string stationName = station.stationName;
        if (stationName == null)
            return "unknown station name";
        else
            return stationName;
    }

    /// <summary>
    /// Get the name of the editing station.
    /// </summary>
    /// <returns></returns>
    public string GetEditStationName()
    {
        return GetStationName(editingStation);
    }

    /// <summary>
    /// Set a new name (in StationController) to the editing station.
    /// </summary>
    /// <param name="stationName"></param>
    public void SetEditStationName(string stationName)
    {
        if (editingStation == null)
            return;
        editingStation.GetComponent<StationController>().SetStationName(stationName);
    }

    /// <summary>
    /// Draw the station as a gizmo in editor-mode.
    /// Two overlapping partly transparent cube will be drawn using 
    /// outerColor and innerColor, radius defines how big the cubes are.
    /// </summary>
    /// <param name="station"></param>
    /// <param name="solidColor"></param>
    /// <param name="wireColor"></param>
    /// <param name="radius"></param>
    public static void DrawStation(GameObject station, Color solidColor, Color wireColor, float radius)
    {
        if (station == null)
            return;
        wireColor.a = 1f;
        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(station.transform.position, Vector3.one * radius);
        solidColor.a = 0.6f;
        Gizmos.color = solidColor;
        Gizmos.DrawCube(station.transform.position, Vector3.one * radius);
    }

    public static void DrawWalkingDistance(GameObject station, Color solidColor, Color wireColor, float radius)
    {
        if (station == null)
            return;
        solidColor.a = 0.2f;
        Gizmos.color = solidColor;
        Gizmos.DrawSphere(station.transform.position, radius);
        wireColor.a = 0.2f;
        Gizmos.color = wireColor;
        Gizmos.DrawWireSphere(station.transform.position, radius);

    }

    /// <summary>
    /// Based on RaycastHit to get the closest station in scene.
    /// It wlll check if the collider of a station is hit first,
    /// then check whether the hit.point is within a certain radius from the station.
    /// It will return null if both of the above return no station.
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public StationController GetClosestStationTo(RaycastHit hit)
    {
        if (stationControllers == null)
            return null;

        var controller = hit.collider.GetComponent<StationController>();
        if (controller != null)
        {
            //Debug.Log(hit.collider.gameObject.name);
            return controller;
        }
        foreach (var station in stationControllers.Values)
        {
            if (Vector3.Distance(hit.point, station.transform.position) <= stationGizmoRadius)
            {
                //Debug.Log(hit.point);
                return station;
            }
        }

        return null;
    }

    public static StationCreator AttachToGameObject()
    {
        var go = GameObject.Find("Stations");
        var creator = go.GetComponent<StationCreator>();
        if (creator == null)
            creator = go.AddComponent<StationCreator>();
        return creator;
    }
}

