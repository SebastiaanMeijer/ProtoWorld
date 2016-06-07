using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderDetector : MonoBehaviour {

	public List<Transform> CollidedObjects;
	public string FilterTag;
	void Start()
	{
		CollidedObjects = new List<Transform>();
	}
	void OnTriggerStay(Collider otherObject)
	{
		if (string.IsNullOrEmpty(FilterTag))
		{
			if (!CollidedObjects.Contains(otherObject.transform))
				CollidedObjects.Add(otherObject.transform);
		}
		else
		{
			if (otherObject.tag==FilterTag)
				if (!CollidedObjects.Contains(otherObject.transform))
					CollidedObjects.Add(otherObject.transform);
		}
	}
	void OnTriggerExit(Collider otherObject)
	{
		CollidedObjects.Remove(otherObject.transform);
	}
}
