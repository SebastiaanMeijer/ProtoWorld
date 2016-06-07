using UnityEngine;
using System.Collections;

public class PedestrianCounterInfo : MonoBehaviour {

	public UnityEngine.UI.Text totalPedestrians;
	public UnityEngine.UI.Text activePedestrians;
	public UnityEngine.UI.Text reachedPedestrians;

	public FlashPedestriansGlobalParameters flashPedestriansGlobalParameters;

	// Use this for initialization
	void Awake () {
		flashPedestriansGlobalParameters = GameObject.Find("FlashPedestriansModule").GetComponent<FlashPedestriansGlobalParameters>();
	}
	
	// Update is called once per frame
	void Update () {
	
		totalPedestrians.text = ""+(flashPedestriansGlobalParameters.numberOfPedestriansOnScenario+flashPedestriansGlobalParameters.numberOfPedestrianReachingDestination);
		activePedestrians.text = ""+(flashPedestriansGlobalParameters.numberOfPedestriansOnScenario);
		reachedPedestrians.text = ""+(flashPedestriansGlobalParameters.numberOfPedestrianReachingDestination);



	}
}
