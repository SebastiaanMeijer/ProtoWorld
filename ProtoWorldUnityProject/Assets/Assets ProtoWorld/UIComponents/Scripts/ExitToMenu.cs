using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour {
	public string menuScenePath;

	public TransitionController transitionController;
	

	public void exitToMenu() {
		transitionController.transitionOut(exit);
	}

	private void exit() {
		SceneManager.LoadSceneAsync(menuScenePath, LoadSceneMode.Single);
	}
}
