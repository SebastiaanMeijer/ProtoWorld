using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleChartViewClick : MonoBehaviour, IPointerDownHandler
{
    private ChartController controller { get; set; }
    private RectTransform chartRect { get; set; }

    public void Start()
    {
        controller = transform.parent.GetComponent<ChartController>();
        chartRect = transform as RectTransform;
        GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    public void OnPointerDown(PointerEventData data)
    {
        float rectWidth = chartRect.rect.width;
        Vector2 point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(chartRect, data.position, data.pressEventCamera, out point);
        float pos = point.x / rectWidth;
        //Debug.Log("x: " + point.x + " pos: " + pos);
        controller.UpdateValuesAndIndicator(pos);
    }

}
