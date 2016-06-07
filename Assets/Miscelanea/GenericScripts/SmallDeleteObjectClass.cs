using UnityEngine;
using System.Collections;
	
public class SmallDeleteObjectClass : MonoBehaviour{
	//this part is for the deletionsystem of  the title panel only
	//these are the static floats that store the RectTransform values
	public static float SxPosition;
	public static float Swidth;
	public static float SyPosition;
	public static float Sheight;
		
	public void Start(){
		//the RectTransform values will be stored into the static floats at the start of the game
		RectTransform rt = GetComponent<RectTransform>();
		SxPosition = transform.position.x;
		SyPosition = transform.position.y;
		Swidth = rt.rect.width;
		Sheight = rt.rect.height;

	}
	
}