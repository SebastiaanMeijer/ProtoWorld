using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class LoaderGUI : MonoBehaviour {

	public string[] scenes;
	public Texture2D logo;
	public Vector2 TextureSize=new Vector2(250,100);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUIStyle style=new GUIStyle(GUI.skin.box);
		GUIStyle bigFont=new GUIStyle(GUI.skin.button);
		bigFont.fontSize=25;
		GUILayout.BeginVertical(style);
		GUILayout.Label(logo,GUILayout.Width(TextureSize.x),GUILayout.Height(TextureSize.y));
		foreach(var s in scenes)
		{
			if (GUILayout.Button(s,bigFont))
				Application.LoadLevel(s);
			GUILayout.Space(20);
		}
		if (GUILayout.Button("Quit",bigFont))
			Application.Quit();
		GUILayout.EndVertical();
	}
}
