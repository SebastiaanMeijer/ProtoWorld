using UnityEngine;
using System.Collections;

/// <summary>
/// Quit key class, simply quits the current scene when the user presses escape button, or on Android when the user presses the back button.
/// </summary>
public class QuitKey : MonoBehaviour {

	/// <summary>
	/// Specifies whether the program should return to the level at index 0 or just quit completely.
	/// </summary>
	public static bool BackToMain=true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (BackToMain)
				Application.LoadLevel(0);
			else
				Application.Quit();
		}
	}
}
