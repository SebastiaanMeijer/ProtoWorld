using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestProjection : MonoBehaviour
{
	public List<Transform> Line;
	public Transform Point;
	private List<Vector3> ProjectionResult;
	private float MinDistance;
	private Vector3 ClosestProjection;
	int count = 0;
	// Use this for initialization
	void Start()
	{
		ProjectionResult = new List<Vector3>();

		MinDistance = float.MaxValue;

		for (int i = 0; i < Line.Count - 1; i++)
			ProjectionResult.Add(Point.position.ProjectToLine(Line[i].position, Line[i + 1].position));
		for (int i = 0; i < Line.Count - 1; i++)
			if (ProjectionResult[i].IsInRangeIgnoreY(Line[i].position, Line[i + 1].position))
			{
				if (Vector3.Distance(ProjectionResult[i], Point.position) < MinDistance)
				{
					//Interpolations.MyLog(ProjectionResult[i] + " is in range of " + Line[i].position + " and " + Line[i].position);
					MinDistance = Vector3.Distance(ProjectionResult[i], Point.position);
					ClosestProjection = ProjectionResult[i];
				}
			}
	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < Line.Count - 1; i++)
			Debug.DrawLine(Line[i].position, Line[i + 1].position, Color.green);
		for (int i = 0; i < Line.Count - 1; i++)
		{
			if (ProjectionResult[i] != ClosestProjection)
				Debug.DrawLine(Point.position, ProjectionResult[i], Color.yellow);
			else
				Debug.DrawLine(Point.position, ProjectionResult[i], Color.red);
		}

		// Update every 30 frames:
		if (count == 30)
		{
			ProjectionResult.Clear();
			count = 0;
			MinDistance = float.MaxValue;
			for (int i = 0; i < Line.Count - 1; i++)
				ProjectionResult.Add(Point.position.ProjectToLine(Line[i].position, Line[i + 1].position));
			for (int i = 0; i < Line.Count - 1; i++)
				if (ProjectionResult[i].IsInRangeIgnoreY(Line[i].position, Line[i + 1].position))
				{
					if (Vector3.Distance(ProjectionResult[i], Point.position) < MinDistance)
					{
						//Interpolations.MyLog(ProjectionResult[i] + " is in range of " + Line[i].position + " and " + Line[i].position);
						MinDistance = Vector3.Distance(ProjectionResult[i], Point.position);
						ClosestProjection = ProjectionResult[i];
					}
				}
		}
		else
			count++;
	}
}
