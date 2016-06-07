using System.Collections;
using UnityEngine;

class DragTransformBackup : MonoBehaviour
{
	
	private bool dragging = true;
	private float distance;
	public Vector3 hoveringover;
	
	
	
	void OnMouseDown()
	{
		distance = Vector3.Distance(transform.position,    Camera.main.transform.position);
		dragging = true;
	}
	
	void OnMouseUp()
	{
		dragging = false;
	}
	
	void Update()
	{
		if (dragging)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 rayPoint = ray.GetPoint(distance);
			//transform.position = rayPoint + hoveringover;
			transform.position = rayHitPositionClass.hitLocation + hoveringover;
		}
	}
}