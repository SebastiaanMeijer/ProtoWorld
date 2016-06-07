using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LegendViewController : MonoBehaviour
{

    private ChartController controller;
    private RectTransform rectTransform;

    public GameObject legendButtonPrefab;

    // Use this for initialization
    void Start()
    {
        controller = GetComponentInParent<ChartController>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLegends();
    }

    void UpdateLegends()
    {
        while (transform.childCount != controller.SeriesCount)
        {
            if (transform.childCount > controller.SeriesCount)
            {
                GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
            }
            else if (transform.childCount < controller.SeriesCount)
            {
                GameObject obj = Instantiate(legendButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                obj.transform.SetParent(transform);
            }
        }
        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            GameObject obj = transform.GetChild(idx).gameObject;
            Image img = obj.GetComponent<Image>();
            img.color = controller.seriesColors[idx];
            Text txt = obj.GetComponentInChildren<Text>();
            txt.text = controller.seriesNames[idx];
        }
    }

    void OldUpdate()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        float deltaX = rectTransform.rect.width / controller.DataContainer.SeriesCount;
        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            GameObject obj = Instantiate(legendButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            obj.transform.SetParent(transform);

            // Position is ordered by Horizontal Layout Group in legendArea.
            Image img = obj.GetComponent<Image>();
            img.color = controller.seriesColors[idx];
            Text txt = obj.GetComponentInChildren<Text>();
            txt.text = controller.seriesNames[idx];
        }

    }
}
