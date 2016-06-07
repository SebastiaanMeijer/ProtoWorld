using UnityEngine;
using System.Collections;

public class FlashPedQueuingFeeder : MonoBehaviour 
{
    StationController station;
    public UnityEngine.UI.Text text;
    FlashPedestriansGlobalParameters globalParam;

	void Awake () 
    {
        station = this.gameObject.GetComponent<StationController>();
        globalParam = FindObjectOfType<FlashPedestriansGlobalParameters>();
	}
	
	void Update () 
    {
        if (station != null && text != null)
        {
            text.text = station.stationName + ": " + (station.queuing * globalParam.numberOfPedestriansPerAgent).ToString();
        }
	}
}
