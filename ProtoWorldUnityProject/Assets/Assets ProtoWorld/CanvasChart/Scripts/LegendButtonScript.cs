using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LegendButtonScript : MonoBehaviour
{
    private ChartController controller;
    private Text text;
    private Image image;

    [RangeAttribute(0, 1)]
    public float hiddenAlpha;

    // Use this for initialization
    void Start()
    {
        controller = transform.parent.parent.parent.GetComponent<ChartController>();
        if(controller == null)
            controller = transform.parent.parent.GetComponent<ChartController>();
        text = GetComponentInChildren<Text>();
        image = GetComponent<Image>();

        //larger button should show more info
        RectTransform rt = GetComponent<RectTransform>();
        if (rt.sizeDelta.y < 25)
            text.verticalOverflow = VerticalWrapMode.Truncate;
        else
            text.verticalOverflow = VerticalWrapMode.Overflow;
    }

    void Update()
    {
        if (controller.isSeriesHidden(text.text))
        {
            Color orgColor = image.color;
            orgColor.a = hiddenAlpha;
            image.color = orgColor;
        }

    }

    public void ToggleVisibility()
    {
        //Debug.Log("Toggled visibility for " + text.text);
        if (controller.isSeriesHidden(text.text))
            controller.seriesHidden.Remove(text.text);
        else
            controller.seriesHidden.Add(text.text);
    }
}