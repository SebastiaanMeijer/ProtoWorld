using UnityEngine;
using System.Collections;

public class BusWithMescoPassengers : BusBaseClass {

	private int CurrentNumberOfPassengers;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override int GetPassengerCount()
	{
		return CurrentNumberOfPassengers;
	}
}
