using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(BusAgentBaseClass))]
public class BusBaseClass : MonoBehaviour
{
	// TODO:= Implement GetBusRoute() method. 
	public static int globalIdIndex = 0;
	public string Id;
	public string Name;
	public string LineNumber;
	public int Capacity;
	public bool AtBusStop = false;
	public Color Color;
	[HideInInspector]
	public BusAgentBaseClass BusAgent;
	[HideInInspector]
	public RouteBaseClass BusRoute;
	public string RouteRelationId;
	// Use this for initialization
	public void initBusBase()
	{
		if (BusRoute == null)
			BusRoute = RouteBaseClass.AllRouteObjectsRouteBaseClass.Where(ro => ro.Id == RouteRelationId).First();
		if (BusAgent == null)
			BusAgent = GetComponent<BusAgentBaseClass>();
	}

	
	public bool IsAvailable(params string[] todo)
	{
		return true;
	}
	public virtual int GetPassengerCount()
	{
		throw new System.NotImplementedException("You must override the GetPassengerCount() method.");
	}
}
