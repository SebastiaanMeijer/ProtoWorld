using UnityEngine;
using System.Collections;

public class HighLightWhenMouseOver : MonoBehaviour {

	private Color startcolor;
	public Color highLightColor;

	void OnMouseEnter()
	{
		startcolor = GetComponent<Renderer>().material.color;
		//renderer.material.color = renderer.material.color + highLightColor;
	}
	void OnMouseExit()
	{
		//renderer.material.color = startcolor;
	}
}
