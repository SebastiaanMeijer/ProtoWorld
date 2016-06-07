using UnityEngine;
using System.Collections;

public class ShowQueNumbers : MonoBehaviour {

	public UnityEngine.UI.Text queTrainToArnhem;
	public UnityEngine.UI.Text queRepToUtrecht1;
	public UnityEngine.UI.Text queRepToUtrecht2;
	public UnityEngine.UI.Text queRegToUtrecht;

	public StationStatistics stationStatistics;

	public int queToArnhem;
	public int queToUtrechtReg;
	public int queToUtrechtRep1;
	public int queToUtrechtRep2;


	// Use this for initialization
	void Awake () {
		stationStatistics = GameObject.Find("Stations").GetComponent<StationStatistics>();
	}
	
	// Update is called once per frame
	void Update () {

		queToArnhem = stationStatistics.QuePerStation[0];
		queToUtrechtReg = stationStatistics.QuePerStation[6];
		queToUtrechtRep1 = stationStatistics.QuePerStation[7];
		queToUtrechtRep2 = stationStatistics.QuePerStation[2];

		queTrainToArnhem.text = "" +queToArnhem;
		queRegToUtrecht.text = ""+queToUtrechtReg;
		queRepToUtrecht1.text = ""+(queToUtrechtRep1 + queToUtrechtRep2);
		queRepToUtrecht2.text = ""+(queToUtrechtRep1 + queToUtrechtRep2);

	
	}
}
