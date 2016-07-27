/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections.Generic;

public class TransLineCreator : StationCreator
{
    public Dictionary<int, LineController> lines;
    [HideInInspector]
    public LineController editingLine;
    [HideInInspector]
    public List<StationController> editingLineStations;
    [HideInInspector]
    public List<float> travelTimes;
    [HideInInspector]
    public LineCategory lineCategory = LineCategory.Metro;
    [HideInInspector]
    public string editLineName = "";

    private const string defaultName = "new line name";

    [Range(1, 150)]
    public float linkHeight = 50;

    [HideInInspector]
    public RoutingController mainRouter;

    [HideInInspector]
    public bool showWalkingDistance = false;

    /// <summary>
    /// Reset the variables used for editing a line
    /// </summary>
    public void ResetEditingInfo()
    {
        editingLine = null;
        editingLineStations = new List<StationController>();
        travelTimes = new List<float>();
        editLineName = "new line name";
    }

    /// <summary>
    /// Save the line using the stations in editingLineStations,
    /// traveling time in travelTimes,
    /// allowTraveling will be true for all legs,
    /// line name from editLineName,
    /// category from lineCategory,
    /// finally, the name of the transline-gameobject is set as the category and the line name.
    /// </summary>
    public void SaveEditingLine()
    {
        if (editingLine == null)
            return;
        var controller = editingLine.GetComponent<LineController>();
        controller.stations.Clear();
        foreach (var station in editingLineStations)
        {
            controller.stations.Add(station.GetComponent<StationController>());
        }
        controller.travelingTimes.Clear();
        controller.allowTraveling = new List<bool>();
        foreach (var time in travelTimes)
        {
            controller.travelingTimes.Add(time);
            controller.allowTraveling.Add(true);
        }
        controller.SetLineName(editLineName);
        controller.SetLineCategory(lineCategory);
        //controller.lineName = editLineName;
        //controller.category = lineCategory;
        //editingLine.name = controller.MakeString();
        //Debug.Log(controller.name);
    }

    /// <summary>
    /// Retrieve the info from an existing transline from the component LineController.
    /// </summary>
    /// <param name="controller"></param>
    public void PrepareLineForEdit(LineController controller)
    {
        ResetEditingInfo();
        editingLine = controller;
        foreach (var station in controller.stations)
        {
            editingLineStations.Add(station);
        }
        foreach (var time in controller.travelingTimes)
        {
            travelTimes.Add(time);
        }
        editLineName = controller.lineName;
        lineCategory = controller.category;
    }

    /// <summary>
    /// Create a new transline.
    /// </summary>
    public void CreateNewLine()
    {
        string sids = "";
        foreach (var station in editingLineStations)
        {
            sids += station.name + ",";
        }
        sids = sids.Remove(sids.Length - 1);
        string tts = "";
        foreach (var time in travelTimes)
        {
            tts += time + ",";
        }
        tts = tts.Remove(tts.Length - 1);

        BaseLine line = new BaseLine()
        {
            id = lines.Count,
            category = lineCategory.ToString(),
            name = editLineName,
            stationIds = sids,
            travelingTimes = tts
        };
        var transLine = LineController.CreateGameObject(line);
        transLine.transform.SetParent(gameObject.transform);
        var controller = transLine.GetComponent<LineController>();
        while (lines.ContainsKey(controller.id))
        {
            controller.id++;
        }
        lines.Add(controller.id, controller);
    }

    /// <summary>
    /// Add a station to the end of the transline.
    /// </summary>
    /// <param name="station"></param>
    public void AddStationToNewLine(StationController station)
    {
        if (station == null)
            return;
        //Debug.Log(GetStationName(station));
        if (!editingLineStations.Contains(station))
        {
            editingLineStations.Add(station);
            if (editingLineStations.Count > 1)
                travelTimes.Add(1);
        }
    }

    /// <summary>
    /// Remove a specific station from the transline.
    /// </summary>
    /// <param name="station"></param>
    public void RemoveStationFromNewLine(StationController station)
    {
        if (station == null)
            return;
        int index = editingLineStations.IndexOf(station);
        editingLineStations.Remove(station);
        if (index >= 0 && index < travelTimes.Count)
        {
            travelTimes.RemoveAt(index);

        }
    }

    /// <summary>
    /// Remove the editing transline. Undo is used.
    /// </summary>
    public void RemoveEditLine()
    {
        if (editingLine == null)
            return;
        lines.Remove(editingLine.GetComponent<LineController>().id);

#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(editingLine.gameObject);
#endif

    }

    /// <summary>
    /// Used by the editor to set the existing transline-gameobjects found in the scene.
    /// The LineControllers of the gameobjects are stored.
    /// </summary>
    /// <param name="translines"></param>
    public void SetLines(GameObject[] translines)
    {
        lines = new Dictionary<int, LineController>();
        foreach (var line in translines)
        {
            var controller = line.GetComponent<LineController>();
            while (lines.ContainsKey(controller.id))
            {
                controller.id++;
            }
            lines.Add(controller.id, controller);
        }
    }

    /// <summary>
    /// Visualize the stations, tranlines and the editing transline.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        DrawStations();
        DrawLines();
        DrawEditLine();
    }

    /// <summary>
    /// Visualize the stations with gizmo.
    /// </summary>
    private void DrawStations()
    {
        if (stationControllers != null && stationControllers.Count > 0)
        {
            foreach (var station in stationControllers.Values)
            {
                DrawStation(station.gameObject, normalColor, editColor, stationGizmoRadius);
                if (showWalkingDistance)
                    DrawWalkingDistance(station.gameObject, Color.white, Color.black, mainRouter.walkingDistance);
            }
        }
    }

    private void DrawEditLine()
    {
        if (editingLineStations != null && editingLineStations.Count > 0)
        {
            for (int i = 1; i < editingLineStations.Count; i++)
            {
                GizmoDraw.WireTransLine(editingLineStations[i - 1].transform.position, editingLineStations[i].transform.position, Color.yellow, stationGizmoRadius * .3f, linkHeight);
            }
        }
    }

    /// <summary>
    /// Visualize the transline with a link of spheres colored with category-specific color, between the stations.
    /// </summary>
    private void DrawLines()
    {
        if (lines != null && lines.Count > 0)
        {
            foreach (var line in lines.Values)
            {
                line.DrawGizmoLine(stationGizmoRadius * .3f, linkHeight, 0.4f);
            }
        }
    }

    public static TransLineCreator AttachToGameObject()
    {
        var go = GameObject.Find("TransLines");
        var creator = go.GetComponent<TransLineCreator>();
        if (creator == null)
            creator = go.AddComponent<TransLineCreator>();
        return creator;
    }
}
