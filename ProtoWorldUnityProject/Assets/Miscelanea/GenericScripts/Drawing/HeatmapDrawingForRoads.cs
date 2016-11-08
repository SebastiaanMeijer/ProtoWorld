using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeatmapDrawingForRoads : MonoBehaviour
{
	public Color headColor;
	//public Color middleColor;
	public Color tailColor;

	public bool activateHeatmapDrawing = true;

	private float increment = 0f;

	private GameObject[] objects;

	// Use this for initialization
	void Start ()
	{
		//Find the lines in the game
		objects = GameObject.FindGameObjectsWithTag ("Line");	

		//Change the shader of the lines to Vertex Color

		var s = Shader.Find ("Vertex color unlit");

		foreach (GameObject G in objects)
			G.GetComponent<Renderer> ().material.shader = s;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (activateHeatmapDrawing) 
		{ 
			var leftColor = Color.Lerp (headColor, tailColor, increment);
			var rightColor = Color.Lerp (tailColor, headColor, increment);

			foreach (GameObject G in objects) 
			{
				//Apply the heatmap here to each object
				var mFilter = G.GetComponent<MeshFilter> ();

				var n = mFilter.mesh.vertexCount;
				//Debug.Log ("Number of vertices: " + n);

				var colors = new List<Color> ();

				colors.Add (leftColor);
				colors.Add (leftColor);

				for (int i = 2; i < n - 2; i++) 
				{
					colors.Add (Color.Lerp (leftColor, rightColor, (float)(i + i % 2 - 1) / (float)(n - 2)));
				}

				colors.Add (rightColor);
				colors.Add (rightColor);

				mFilter.mesh.SetColors (colors);
			}
	
			increment += 0.01f;

			if (increment > 1) 
			{
				Color aux = headColor;
				headColor = tailColor;
				tailColor = aux;

				increment = 0f;
			}
		}
	}
}
