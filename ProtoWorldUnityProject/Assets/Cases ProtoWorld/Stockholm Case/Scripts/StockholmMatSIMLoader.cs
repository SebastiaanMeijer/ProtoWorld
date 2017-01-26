using UnityEngine;
using System.Collections;

public class StockholmMatSIMLoader : MonoBehaviour {
	private StockholmMatSIMParameters parameters;
	private MatsimIO matsimIO;


	public void Awake() {
		parameters = StockholmMatSIMParameters.getInstance();
		//matsimIO = GameObject.FindObjectOfType<MatsimIO>() as MatsimIO;
	}

	public void Start() {
		
	}

	public void Update() {

	}
}
