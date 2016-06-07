using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct LinesForMesh
{
    public Vector3[] lines;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

}

public static class ChartUtils
{
    public static Vector3[] CreateLinesFromData(List<TimedData> list, RectTransform rectTransform, Rect bounds)
    {
        Vector3[] lines = new Vector3[list.Count * 2];

        float xScale = rectTransform.rect.width / (bounds.xMax - bounds.xMin);
        float yScale = rectTransform.rect.height / (bounds.yMax - bounds.yMin);

        int lineIdx = 0;
        for (int i = 0; i < list.Count - 1; i++)
        {
            lines[lineIdx++] = new Vector3((list[i].time - bounds.xMin) * xScale, (list[i].GetData() - bounds.yMin) * yScale);
            lines[lineIdx++] = new Vector3((list[i + 1].time - bounds.xMin) * xScale, (list[i + 1].GetData() - bounds.yMin) * yScale);
        }
        return lines;
    }


    public static LinesForMesh CreateLinesFromData(List<TimedData> list, RectTransform rectTransform)
    {
        Vector3[] lines = new Vector3[list.Count * 2];

        float startTime = list[0].time;
        float endTime = list[list.Count - 1].time;

        float minValue = list[0].GetData();
        float maxValue = list[0].GetData();
        float value;
        int lineIdx = 0;
        for (int i = 0; i < list.Count - 1; i++)
        {
            lines[lineIdx++] = new Vector3(list[i].time, list[i].GetData());
            value = list[i + 1].GetData();
            lines[lineIdx++] = new Vector3(list[i + 1].time, value);
            if (value < minValue)
                minValue = value;
            if (value > maxValue)
                maxValue = value;
        }

        float xScale = rectTransform.rect.width / (endTime - startTime);
        float yScale = rectTransform.rect.height / (maxValue - minValue);

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].x = (lines[i].x - startTime) * xScale;
            lines[i].y = (lines[i].y - minValue) * yScale;
        }

        LinesForMesh lfm = new LinesForMesh()
        {
            lines = lines,
            minX = startTime,
            maxX = endTime,
            minY = minValue,
            maxY = maxValue
        };

        return lfm;
    }

    public static Mesh GenerateLineMesh(Vector3[] points, float width = 1)
    {
        if ((points.Length % 2) != 0)
        {
            Debug.LogWarning("Points not in pair");
            return null;
        }

        Vector3 mNormal = new Vector3(0f, 0f, -1f);

        Mesh mesh = new Mesh();

        int length = points.Length * 2;

        Vector3[] verts = new Vector3[length];
        Vector3[] norms = new Vector3[length];
        Vector2[] uvs = new Vector2[length];
        int[] trias = new int[points.Length * 3];

        int idx = 0;
        int triIdx = 0;

        for (int i = 0; i < points.Length; i += 2)
        {
            Vector3 vec = (points[i] - points[i + 1]);
            Vector3 crs = Vector3.Cross(mNormal, vec).normalized;
            Vector3 perpendicular = crs * width / 2;

            verts[idx] = points[i] - perpendicular;
            verts[idx + 1] = points[i] + perpendicular;
            verts[idx + 2] = points[i + 1] - perpendicular;
            verts[idx + 3] = points[i + 1] + perpendicular;

            norms[idx] = mNormal;
            norms[idx + 1] = mNormal;
            norms[idx + 2] = mNormal;
            norms[idx + 3] = mNormal;

            uvs[idx] = new Vector2(verts[0].x, verts[0].y);
            uvs[idx + 1] = new Vector2(verts[1].x, verts[1].y);
            uvs[idx + 2] = new Vector2(verts[2].x, verts[2].y);
            uvs[idx + 3] = new Vector2(verts[3].x, verts[3].y);

            trias[triIdx++] = idx;
            trias[triIdx++] = idx + 1;
            trias[triIdx++] = idx + 2;
            trias[triIdx++] = idx + 1;
            trias[triIdx++] = idx + 3;
            trias[triIdx++] = idx + 2;

            idx += 4;
        }
        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.uv = uvs;
        mesh.triangles = trias;
        return mesh;
    }

    public static String NameGenerator(string name, int index)
    {
        return String.Format("{0} {1}", name, index);
    }

    public static Material BlackMaterial 
    {
        get
        {
            Material m = new Material(Shader.Find("UI/Default"));
            m.color = Color.black;
            return m;
        }
    }

    public static string SecondsToTime(float timeInSeconds)
    {
        var time = TimeSpan.FromSeconds(timeInSeconds);
        string str = time.Seconds.ToString("00.") + "s";

        if (time.Hours > 0)
            str = time.Hours.ToString("00.") + "h:" + time.Minutes.ToString("00.") + "m:" + str;
        else if (time.Minutes > 0)
            str = time.Minutes.ToString("00.") + "m:" + str;

        return str;

        //return string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
        //        time.Hours,
        //        time.Minutes,
        //        time.Seconds);
    }
}
