using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ChangeButtonText : MonoBehaviour {

	public Text ButtonText;
	public string[] buttonText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeButtonText(){

		ButtonText.text = buttonText [Heatmap.heatmapNumber-1];

	}

}
