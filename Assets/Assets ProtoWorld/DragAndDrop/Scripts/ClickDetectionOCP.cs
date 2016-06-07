using UnityEngine;
using System.Collections;

public class ClickDetectionOCP : MonoBehaviour {
	
	static public float sizeWidth;
	static public float sizeHeight;
	static public float xPosition;
	static public float yPosition;
	
	void Start () {
		
		RectTransform rt = GetComponent<RectTransform>();
		sizeWidth = rt.rect.width;
		sizeHeight = rt.rect.height;
		xPosition = rt.rect.x;
		yPosition = rt.rect.y;
		
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
