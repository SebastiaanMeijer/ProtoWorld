using UnityEngine;
using System.Collections;


public static class ChartPositionHelper {


    private static float xPos = 0;
    private static float yPos = -1000;

    public static void SetChartPosition(BaseChart chart)
    {
        chart.transform.position = new Vector2(xPos, yPos);
        xPos += 500;
    }
}
