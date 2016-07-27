using UnityEngine;
using UnityEditor;
using System.Collections;

public class UnityInspectorWindow : EditorWindow
{

	[MenuItem("Window/Unity Inspector",false,1)]
	static void Init()
	{
		UnityInspectorWindow window=EditorWindow.GetWindow(typeof(UnityInspectorWindow),false,"Unity Inspector") as UnityInspectorWindow;
	}
	void OnGUI()
	{
		GUIStyle labelStyle=new GUIStyle(GUI.skin.label);
		labelStyle.wordWrap=true;
		labelStyle.alignment= TextAnchor.MiddleLeft;
		GUIStyle labelStyleR=new GUIStyle(GUI.skin.label);
		//labelStyleR.normal.textColor=Color.gray;
		labelStyleR.wordWrap=true;
		labelStyleR.fontStyle= FontStyle.Bold;
		labelStyleR.alignment= TextAnchor.MiddleRight;
		GUIStyle beginHorizontalStyle= new GUIStyle(GUI.skin.textArea);
		EditorGUILayout.BeginVertical(beginHorizontalStyle);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Genuine check available? " ,labelStyle);
		EditorGUILayout.LabelField(Application.genuineCheckAvailable+"",labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Is genuine? ",labelStyle);
		EditorGUILayout.LabelField(Application.genuine+"",labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Has user authorization for microphone? "  ,labelStyle);
		EditorGUILayout.LabelField(""+Application.HasUserAuthorization( UserAuthorization.Microphone),labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Has user authorization for webcam? "  ,labelStyle);
		EditorGUILayout.LabelField(""+Application.HasUserAuthorization( UserAuthorization.WebCam),labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Is pro license? "  ,labelStyle);
		EditorGUILayout.LabelField(""+ Application.HasProLicense(),labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Version: " ,labelStyle );
		EditorGUILayout.LabelField("" + Application.unityVersion,labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Platform: " ,labelStyle );
		EditorGUILayout.LabelField("" + Application.platform,labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Device Model: " ,labelStyle );
		EditorGUILayout.LabelField("" + SystemInfo.deviceModel,labelStyleR);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}
}
