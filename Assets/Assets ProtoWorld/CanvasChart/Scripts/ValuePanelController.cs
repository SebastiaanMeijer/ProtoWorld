using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ValuePanelController : MonoBehaviour
{

    private ChartController controller;
    private RectTransform valuePanel;
    private ColorBlock colorBlock;

    public GameObject valueButtonPrefab;

    private static string valueChildString = "timeData";

    public float MaxValue { get; set; }
    public float MinValue { get; set; }

    // Use this for initialization
    void Start()
    {
        controller = GetComponentInParent<ChartController>();
        valuePanel = transform as RectTransform;
        colorBlock = valueButtonPrefab.GetComponent<Button>().colors;
    }

    // Update is called once per frame
    void Update()
    {
        CheckValueCount();
        UpdateValues();

    }

    private void UpdateValues()
    {
        var minmax = controller.GetMinMaxOfAll();

        for (int i = 0; i < controller.values.Length; i++)
        {
            string name = ChartUtils.NameGenerator(valueChildString, i);
            GameObject obj = valuePanel.Find(name).gameObject;
            colorBlock.disabledColor = controller.seriesColors[i];
            obj.GetComponent<Button>().colors = colorBlock;

            float value = controller.values[i];
            obj.GetComponentInChildren<Text>().text = value.ToString(controller.specifier);

            RectTransform rt = obj.transform as RectTransform;
            float yPos = (value - minmax.yMin) / (minmax.yMax - minmax.yMin) * valuePanel.rect.height;
            yPos = Mathf.Clamp(yPos, 0, valuePanel.rect.height - rt.rect.height);
            rt.localPosition = new Vector3(-5 , yPos);
        }
    }

    private void CheckValueCount()
    {
        while (valuePanel.childCount != controller.SeriesCount)
        {
            if (valuePanel.childCount > controller.SeriesCount)
            {
                string name = ChartUtils.NameGenerator(valueChildString, valuePanel.childCount - 1);
                GameObject obj = valuePanel.Find(name).gameObject;
                if (obj)
                {
                    // Gameobject will not be destroyed until after Update()
                    // hence it must detach with parent before Destroy()
                    obj.transform.parent = null;
                    Destroy(obj);
                }
                else
                    Debug.LogError(String.Format("{0} not found?!", name));
            }
            else if (valuePanel.childCount < controller.SeriesCount)
            {
                GameObject obj = Instantiate(valueButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                obj.name = ChartUtils.NameGenerator(valueChildString, valuePanel.childCount);
                obj.transform.SetParent(valuePanel);
                //obj.transform.localPosition = Vector3.zero;
            }
        }
    }



}
