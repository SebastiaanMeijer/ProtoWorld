/*
 * 
 * CANVAS CHART MODULE
 * AxisController.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the axis object to visualize axis in the chart.
/// </summary>
public class AxisController : MonoBehaviour
{

    private ChartController controller;
    private RectTransform axisHolder;
    private CanvasRenderer canvasRenderer;

    private Text yMaxText;
    private Text yMinText;
    private Text xMaxText;
    private Text xMinText;

    private GameObject yZero;

    private Mesh axisMesh;
    private Vector3[] lines;

    /// <summary>
    /// Init all necessary references.
    /// </summary>
    void Start()
    {
        controller = GetComponentInParent<ChartController>();
        axisHolder = GetComponent<RectTransform>();
        canvasRenderer = GetComponent<CanvasRenderer>();
        yMaxText = axisHolder.FindChild("YMaxText").GetComponent<Text>();
        yMinText = axisHolder.FindChild("YMinText").GetComponent<Text>();
        xMaxText = axisHolder.FindChild("XMaxText").GetComponent<Text>();
        xMinText = axisHolder.FindChild("XMinText").GetComponent<Text>();

        yZero = axisHolder.FindChild("YZeroText").gameObject;

    }

    /// <summary>
    /// Update the axis lines and the min max X- and Y-values.
    /// </summary>
    void Update()
    {
        switch (controller.chartType)
        {
            case UIChartTypes.Bar:
            case UIChartTypes.Pie:
                xMaxText.gameObject.SetActive(false);
                xMinText.gameObject.SetActive(false);
                break;
            case UIChartTypes.Line:
            case UIChartTypes.StackedArea:
                xMaxText.gameObject.SetActive(true);
                xMinText.gameObject.SetActive(true);
                break;
        }

        UpdateAxis();
        UpdateText();
    }

    /// <summary>
    /// Redraw the axis based on the lenght and height of the chart area.
    /// </summary>
    private void UpdateAxis()
    {
        lines = new Vector3[4];
        lines[0] = Vector3.zero;
        lines[1] = new Vector3(0, axisHolder.rect.height);
        lines[2] = lines[0];
        lines[3] = new Vector3(axisHolder.rect.width, 0);
        axisMesh = ChartUtils.GenerateLineMesh(lines, 2f);

        //canvasRenderer.Clear();
        canvasRenderer.SetMaterial(ChartUtils.BlackMaterial, null);
        canvasRenderer.SetMesh(axisMesh);
    }

    /// <summary>
    /// Update the xMin, xMax, yMin, yMax 
    /// and indicate where y=0 is only when yMin is less than 0 and yMAx is more than 0.
    /// </summary>
    private void UpdateText()
    {
        if (controller.SeriesCount > 0)
        {
            var minmax = controller.GetMinMaxOfAll();

            yMaxText.text = minmax.yMax.ToString(controller.specifier);
            yMinText.text = minmax.yMin.ToString(controller.specifier);
            xMaxText.text = ChartUtils.SecondsToTime(minmax.xMax);
            xMinText.text = ChartUtils.SecondsToTime(minmax.xMin);

            if (minmax.yMin < 0 && minmax.yMax > 0)
            {
                yZero.SetActive(true);
                yZero.GetComponent<Text>().text = "0";
                float totalY = minmax.yMax - minmax.yMin;
                float yPos = -minmax.yMin / totalY * axisHolder.rect.height;
                var transform = yZero.transform as RectTransform;
                transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, yPos);
            }
            else
            {
                yZero.SetActive(false);
            }
        }
    }


}
