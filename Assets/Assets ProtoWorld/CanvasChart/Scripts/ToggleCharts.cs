using UnityEngine;
using System.Collections;

public class ToggleCharts : MonoBehaviour
{

    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}