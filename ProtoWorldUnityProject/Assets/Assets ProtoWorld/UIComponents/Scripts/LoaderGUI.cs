/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
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
