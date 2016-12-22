using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueToothDataRenderer : MonoBehaviour
{
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

        //loop over all files
        for (int i = 0; i < files.Length; i++)
        {
            TextAsset file = files[i];
            readCSV(i, file.name, file.text);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void readCSV(int index, string name, string text)
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
