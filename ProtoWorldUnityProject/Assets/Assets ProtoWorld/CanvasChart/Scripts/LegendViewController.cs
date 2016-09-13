/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * KPI MODULE
 * Johnson Ho
 * 
 */

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
                GameObject obj =
                    Instantiate(legendButtonPrefab, transform.localPosition, Quaternion.identity) as GameObject;
                obj.transform.SetParent(transform);
                obj.transform.position = transform.position;
            }
        }

        for (int idx = 0; idx < controller.SeriesCount; idx++)
        {
            GameObject obj = transform.GetChild(idx).gameObject;
            Image img = obj.GetComponent<Image>();
            img.color = controller.seriesColors[idx];
            Text txt = obj.GetComponentInChildren<Text>();
            txt.gameObject.transform.position = obj.transform.position;
            txt.text = controller.seriesNames[idx];
        }
    }

    void OldUpdate()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        float deltaX = rectTransform.rect.width/controller.DataContainer.SeriesCount;
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