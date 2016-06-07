//Furkan Sonmez

using UnityEngine;
using System.Collections;

public class DeleteObjectClass : MonoBehaviour{
	//this part is for the deletionsystem of panel that contains the objects list
	//these are the static floats that store the RectTransform values
	public static float xPosition;
	public static float width;
	public static float yPosition;
	public static float height;
	
	public void Start(){
		//the RectTransform values will be stored into the static floats at the start of the game
		RectTransform rt = GetComponent<RectTransform>();
		xPosition = transform.position.x;
		yPosition = transform.position.y;
		width = rt.rect.width;
		height = rt.rect.height;

	}
	
}