using UnityEngine;

public class ExitToDesktop : MonoBehaviour {
	public TransitionController transitionController;


	public void exitToDesktop() {
		transitionController.transitionOut(exit);
	}

	private void exit() {
		// TODO This is a hack to prevent Unity from crashing upon exit. It is likely caused by one
		// of the modules as it only happens in the case scenes, but no errors are being logged.
		// For now, this will have to do, as we are closing the application anwyays.
		if(!Application.isEditor) {
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

		//Application.Quit();
	}
}
