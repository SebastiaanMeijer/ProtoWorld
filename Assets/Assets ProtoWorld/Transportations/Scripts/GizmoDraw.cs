using UnityEngine;
using System.Collections;

public class GizmoDraw : MonoBehaviour
{

    public static void TransLine(Vector3 start, Vector3 end, Color solidColor, float radius, float linkHeight)
    {
        Gizmos.color = solidColor;
        float dist = Vector3.Distance(start, end);
        int limit = Mathf.RoundToInt(dist / radius);
        for (int i = 0; i < limit; i += 2)
        {
            var t = (float)i / (float)limit;
            var center = Vector3.Lerp(start, end, t);
            center.y = (-4 * Mathf.Pow((t - .5f), 2) + 1) * linkHeight;
            Gizmos.DrawSphere(center, radius);
        }
    }

    public static void WireTransLine(Vector3 start, Vector3 end, Color wireColor, float radius, float linkHeight)
    {
        Gizmos.color = wireColor;
        float dist = Vector3.Distance(start, end);
        int limit = Mathf.RoundToInt(dist / radius);
        for (int i = 0; i < limit; i += 2)
        {
            var t = (float)i / (float)limit;
            var center = Vector3.Lerp(start, end, t);
            center.y = (-4 * Mathf.Pow((t - .5f), 2) + 1) * linkHeight;
            Gizmos.DrawWireSphere(center, radius);
        }
    }

}
