using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BlueToothDataRenderer : MonoBehaviour
{
    public enum CSVtype { Haifa, Venice };
    public CSVtype fileType;
    public TextAsset[] files;
    public MapBoundaries boundaries;

    private Material lineMaterial;
    private double realMinX, realMinY, realMaxX, realMaxY, gameMinX, gameMinY, gameMaxX, gameMaxY;

    // Use this for initialization
    void Start()
    {
        lineMaterial = new Material(Shader.Find("SolidColor")); //Particles/Alpha Blended
        if (boundaries == null)
        {
            Transform scen = GameObject.Find("ScenarioModule").transform;
            Transform bound = scen.FindChild("AramGISBoundaries");
            boundaries = bound.GetComponent<MapBoundaries>();
        }

        //get latitude and longitude boundaries
        realMinX = boundaries.dbBoundMinLat;
        realMinY = boundaries.dbBoundMinLon;
        realMaxX = boundaries.dbBoundMaxLat;
        realMaxY = boundaries.dbBoundMaxLon;

        //get x and y boundaries
        gameMinX = boundaries.minMaxX[0] + boundaries.MinPointOnMap.x;
        gameMinY = boundaries.minMaxY[0] + boundaries.MinPointOnMap.z;
        gameMaxX = boundaries.minMaxX[1] + boundaries.MinPointOnMap.x;
        gameMaxY = boundaries.minMaxY[1] + boundaries.MinPointOnMap.z;

        if (fileType == CSVtype.Haifa)
        {
            //loop over all files
            for (int i = 0; i < files.Length; i++)
            {
                TextAsset file = files[i];
                readCSVHaifa(i, file.name, file.text);
            }
        }
        else
        {
            if (files.Length < 2)
            {
                Debug.LogWarning("The Venice type requires two files");
                return;
            }
            TextAsset positions = files[0];
            TextAsset distances = files[1];
            readCSVVenice(positions.text, distances.text);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void readCSVHaifa(int index, string name, string text)
    {
        GameObject parentObj = new GameObject(name);
        parentObj.transform.parent = transform;

        string[] lines = text.Split("\n"[0]);
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            // start_antenna, s_lat, s_long, end_antenna, e_lat, e_long, value
            if (parts.Length < 6 || parts[0] == "start_antenna")
                continue;

            //convert lat/long to ingame positions
            double x1 = double.Parse(parts[1]);
            double y1 = double.Parse(parts[2]);
            double x2 = double.Parse(parts[4]);
            double y2 = double.Parse(parts[5]);
            Vector3 pos1 = realToGameCoords(x1, y1);
            Vector3 pos2 = realToGameCoords(x2, y2);

            float value = float.Parse(parts[6]);
            if (value > 1.5f)
            {
                value = Mathf.Sqrt(value) / 100;
            }

            //create line
            createLine(parentObj.transform, pos1, pos2, value);
        }
    }

    private void readCSVVenice(string text, string text2)
    {
        //flip files if they are in the wrong order
        if (text.Length > text2.Length)
        {
            string temp = text;
            text = text2;
            text2 = temp;
        }

        List<string> ids = new List<string>();
        Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
        string[] parts;

        string[] lines = text.Split("\n"[0]);
        foreach (string line in lines)
        {
            parts = line.Split(',');
            if (parts.Length < 4)
                continue;

            double x1 = double.Parse(parts[1]);
            double y1 = double.Parse(parts[2]);
            Vector3 pos = realToGameCoords(x1, y1);
            positions.Add(parts[0].Trim('"'), pos);
        }

        lines = text2.Split("\n"[0]);
        parts = lines[0].Split(',');
        foreach(string part in parts) {
            ids.Add(part.Trim('"'));
        }

        GameObject parentObj = new GameObject(""+lines[1]);
        parentObj.transform.parent = transform;
        int firstOffset = 1;
        for (int i = 2; i < lines.Length; i++)
        {
            string line = lines[i];
            parts = line.Split(',');
            if (parts.Length == 1)
            {
                if (i > 1000) //arbitrary stop to not have it calculate for like 10 minutes
                    break;
                parentObj = new GameObject("" + lines[i]);
                parentObj.transform.parent = transform;
                firstOffset = i;
                continue;
            }

            for (int j = 0; j < parts.Length; j++)
            {
                float value = float.Parse(parts[j]);
                if (value > 0.1f)
                {
                    Vector3 pos1 = positions[ids[i-firstOffset-1]];
                    Vector3 pos2 = positions[ids[j]];
                    createLine(parentObj.transform, pos1, pos2, value / 3000f);
                }
            }
        }
    }

    private void createLine(Transform parent, Vector3 pos1, Vector3 pos2, float value)
    {
        Vector3[] positions = { pos1, pos2 };

        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = parent;
        lineObj.transform.position = Vector3.zero;

        LineRenderer lineRend = lineObj.AddComponent<LineRenderer>();
        lineRend.SetPositions(positions);
        lineRend.material = lineMaterial;
        lineRend.material.color = getLineColor(value); //colors[value % colors.Length];
        lineRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRend.receiveShadows = false;
        lineRend.SetWidth(20, 20);
    }

    private Vector3 realToGameCoords(double posX, double posY)
    {
        double newX = (((posX - realMinX) / (realMaxX - realMinX)) * -(gameMaxX - gameMinX)) - gameMinX;
        double newY = (((posY - realMinY) / (realMaxY - realMinY)) * (gameMaxY - gameMinY)) - gameMinY;
        return new Vector3((float)newX, 10, (float)newY);
    }

    private Color getLineColor(float val)
    {
        val = Mathf.Clamp(val * 3, 0, 3);
        if(val <= 1f) {
            return Color.Lerp(Color.blue, Color.green, val);
        } else if(val <= 2f) {
            return Color.Lerp(Color.green, Color.yellow, val - 1f);
        } else {
            return Color.Lerp(Color.yellow, Color.red, val - 2f);
        }
    }
}
