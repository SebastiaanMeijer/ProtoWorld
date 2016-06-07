using UnityEngine;
using System.Collections;

public class BusStopClass : MonoBehaviour {

	// TODO:= Implmenet GetBusRoutesAtStop() method.
	public string Id;
	public string Name;
	public string[] RouteReference; // Bus numbers: 73;72;55;4
	public string Zone; // A,B,C or D in case of SL
	public bool Shelter;
	public bool Bench;
	public bool Wheelchair;
	public double Latitude;
	public double Longitude;
	public string NetworkOrOperator; // SL
	public void SetLocalReference(string s)
	{ LocalReference = s; }
	public void SetReference(string s)
	{ Reference = s; }
	private string LocalReference; // A,R,G,....
	private string Reference; // a number
}
