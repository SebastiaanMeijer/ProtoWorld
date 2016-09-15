using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LegendButtonScript : MonoBehaviour
{
    private ChartController controller;
    private Text text;
    private Image image;

    // Use this for initialization
    void Start()
    {
        controller = transform.parent.parent.parent.GetComponent<ChartController>();
        text = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
    }

    public void ToggleVisibility()
    {
        if (controller.seriesHidden.Contains(text.text)) controller.seriesHidden.Remove(text.text);
        else controller.seriesHidden.Add(text.text);
    }
}