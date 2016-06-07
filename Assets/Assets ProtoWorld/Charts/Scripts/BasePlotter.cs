/*
 * 
 * BASE PLOTTER FOR UNITY
 * BasePlotter.cs
 * Johnson Ho
 * USE WITH BASE CHART
 * 
 */

using UnityEngine;
using System.Collections;
using System;



public class BasePlotter : MonoBehaviour
{
    public Color backgroundColor = Color.white;
    public Color axisColor = Color.black;

    protected GameObject plotObject;

    protected BaseChart chart;

    protected string objectString = "Object";

    public virtual void Init(BaseChart c)
    {
        chart = c;
        GeneratePlot();
    }

    public virtual void UpdatePlot() { }

    protected virtual void GeneratePlot() { }

    protected virtual void UpdateTextCount(GameObject gameObject, string identifier, int targetCount, TextAnchor anchor)
    {
        Transform transform = gameObject.transform;
        while (transform.childCount != targetCount)
        {

            if (transform.childCount > targetCount)
            {
                string name = MeshUtils.NameGenerator(identifier, transform.childCount - 1);
                GameObject obj = transform.Find(name).gameObject;
                if (obj)
                {
                    obj.transform.parent = null;
                    Destroy(obj.GetComponent<TextMesh>());
                    Destroy(obj);
                }
                else
                    Debug.LogError(String.Format("{0} not found?!", name));
            }
            else if (transform.childCount < targetCount)
            {
                GameObject obj = Instantiate(chart.axisText, new Vector3(0, 0, -1000), Quaternion.identity) as GameObject;
                //obj.name = String.Format("Y{0}", count);
                obj.name = MeshUtils.NameGenerator(identifier, transform.childCount);
                obj.transform.parent = transform;
                TextMesh mesh = obj.GetComponent<TextMesh>();
                mesh.anchor = anchor;
            }
        }
    }
}

