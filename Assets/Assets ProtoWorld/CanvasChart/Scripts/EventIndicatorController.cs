using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventIndicatorController : MonoBehaviour {

    private ChartController controller;
    private Slider eventIndicator;
    private GameObject eventHandleArea;
    private Text eventButtonText;

    // Use this for initialization
    void Start () {
        controller = GetComponentInParent<ChartController>();
        eventIndicator = GetComponent<Slider>();
        eventHandleArea = transform.FindChild("Handle Slide Area").gameObject;
        eventButtonText = transform.FindChild("Handle Slide Area/Handle/EventButton/Text").GetComponent<Text>();
        SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetActive(bool active)
    {
        eventHandleArea.SetActive(active);
    }

    public void SetMinTime(float time)
    {
        eventIndicator.minValue = time;
    }

    public void SetMaxTime(float time)
    {
        eventIndicator.maxValue = time;
    }

    public void SetTime(float time)
    {
        //eventIndicator.value = time;
        //eventButtonText.text = time.ToString();
        SetActive(true);
        eventIndicator.value = Mathf.Clamp(time, eventIndicator.minValue, eventIndicator.maxValue);
        SetText(eventIndicator.value);
    }

    /// <summary>
    /// Used by the slider "On Value Changed"
    /// </summary>
    public void HandleValueChanged()
    {
        if (eventIndicator.value > 0)
        {
            SetText(eventIndicator.value);
        }
    }

    public void SetText(float time)
    {
        eventButtonText.text = ChartUtils.SecondsToTime(time);
    }

}
