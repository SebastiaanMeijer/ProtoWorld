using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class UVEditor : EditorWindow {
	
	[MenuItem ("Window/UV Editor")]
    private static void Init () {
        EditorWindow.GetWindow (typeof (UVEditor));
    }
	
	private float oldScaleX, oldScaleY;
	
	bool DirectionY = true;
	bool snapDecimal = true;
	GameObject currentSelection;
	GameObject oldSelection = null;	
	Vector2[]  selectionUVData;
	MeshFilter currentSelectionMeshFilter;
	Texture2D  selectionTexture;
	float activeScaleX = 1;
	float activeScaleY = 1;
	int tileCount = 1;
	
	float zoomfactor = 1;
	Vector2 scrollpos;
	Rect 	areaVisible;
	Rect 	areaRect;
	List<int> selectedUV = new List<int>();
	bool scaleLocked = false;
	
	GUIStyle 	lockIconOn;
	GUIStyle 	lockIconOff;
	
	void OnDestroy()
	{
		Save();
	}
	
	void GenerateStyles( ) {
		if (lockIconOn== null)  {
			
			Texture2D iconOn = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/lockOn.psd", typeof(Texture2D));
			lockIconOn = new GUIStyle(EditorStyles.label);
			lockIconOn.normal.background = iconOn;
			lockIconOn.padding = new RectOffset(0,0,0,-10);
			lockIconOn.overflow = new RectOffset(0,-4,5,5);
			lockIconOn.fixedHeight = 6;
			lockIconOn.fixedWidth = 12;
		}
		if (lockIconOff== null)  {
			Texture2D iconOff =(Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/lockOff.psd", typeof(Texture2D));
			lockIconOff = new GUIStyle(EditorStyles.label);
			lockIconOff.normal.background = iconOff;
			lockIconOff.padding = new RectOffset(0,0,0,-10);
			lockIconOff.overflow = new RectOffset(0,-4,5,5);
			lockIconOff.fixedHeight = 6;
			lockIconOff.fixedWidth = 12;
		}
		
		
	}
	
	void Reset() {
		if (currentSelectionMeshFilter != null)
			selectionUVData = currentSelectionMeshFilter.sharedMesh.uv;
		activeScaleX = 1;
		activeScaleY = 1;
		selectedUV.Clear();
	}
	
	void OnInspectorUpdate() {
        Repaint();
    }
	
	void OnGUI() {
		GenerateStyles();
		
		GUILayout.Label("UVEditor V1.0", EditorStyles.boldLabel);
		GUILayout.Label("Made by Pixelstudio", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.LookLikeControls(120,100);
		EditorGUILayout.PrefixLabel("Current Selection :");
		if (currentSelection != null)  {

			GUILayout.Label(currentSelection.name);
			GUI.enabled = true;
			if (currentSelectionMeshFilter == null) {
				GUI.enabled = false;	
				GUILayout.Label("No meshFilter found!");
			} else if (currentSelection.GetComponent<Renderer>() == null) {
				GUI.enabled = false;
				GUILayout.Label("No renderer");
			}else if (currentSelection.GetComponent<Renderer>().sharedMaterial == null) {
				GUI.enabled = false;
				GUILayout.Label("No material");
			} 
			
			if (selectionUVData == null) {
				GUI.enabled = false;	
				GUILayout.Label("No uvdata found!");
			}
			
			
		}
		else {
			GUILayout.Label("None");
			GUI.enabled = false;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.LookLikeControls(20,20);
		DirectionY = EditorGUILayout.Toggle("Y",DirectionY);
		if (GUILayout.Button("Apply planer face UV"))
			{
				ApplyPlanerUV();
			}
		EditorGUILayout.EndHorizontal();
			if (GUILayout.Button("Apply"))
			{
				Reset();
			}
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Scale UV's", EditorStyles.boldLabel);
		EditorGUIUtility.LookLikeControls(90,20);
		snapDecimal = EditorGUILayout.Toggle("snap decimal", snapDecimal);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Scale X " + Math.Round(activeScaleX,1).ToString(), GUILayout.Width(90) );
		activeScaleX = GUILayout.HorizontalSlider(activeScaleX,0.01f,2f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal(GUILayout.Height(0));
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("", (scaleLocked ? lockIconOn : lockIconOff ) )) {
				scaleLocked = !scaleLocked;
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Scale Y " + Math.Round(activeScaleY,1).ToString(), GUILayout.Width(90) );
		activeScaleY = GUILayout.HorizontalSlider(activeScaleY,0.01f,2f);
		EditorGUILayout.EndHorizontal();
		
		
		
		RenderUVEditor();
		
		Rect savePos = new Rect(0,areaRect.yMax, areaRect.width, 30);
		if (GUI.Button(savePos, "Save")) 
			Save();
		
		HandleMouse();
		
	}

	void Update() {
		currentSelection = Selection.activeGameObject;
		if (currentSelection != null){ 
			if (oldSelection != currentSelection) {
				currentSelectionMeshFilter = currentSelection.GetComponent<MeshFilter>();
				if (currentSelectionMeshFilter != null) {
					selectionUVData = currentSelectionMeshFilter.sharedMesh.uv;
					Reset();
					if (currentSelection.GetComponent<Renderer>().sharedMaterial.mainTexture != null)
						selectionTexture =(Texture2D) currentSelection.GetComponent<Renderer>().sharedMaterial.mainTexture;
				}
				oldSelection = currentSelection;
			}
		}
		
		if (currentSelection!= null) {
			if (currentSelectionMeshFilter!=null) {
				ApplyScaledUV();	
			}
			
		}
		Repaint();
	}
	
	void Save() {
		if ((currentSelection== null) || (currentSelectionMeshFilter == null) ) 
			return;
		
		AssetDatabase.Refresh();
		
		int id = currentSelection.GetInstanceID();
		Mesh currentSelectionMesh = currentSelectionMeshFilter.sharedMesh;
		
		string p = AssetDatabase.GetAssetPath(currentSelectionMesh);
		
		string toDelete = "";
		if ((p.Contains(".assets")) && (!p.Contains(id.ToString()))) {
			toDelete = p;
		}
		
		Mesh newMesh = new Mesh();
		newMesh.vertices = currentSelectionMesh.vertices;
		newMesh.triangles = currentSelectionMesh.triangles;
		newMesh.colors = currentSelectionMesh.colors;
		newMesh.tangents = currentSelectionMesh.tangents;
		newMesh.normals = currentSelectionMesh.normals;
		newMesh.uv = currentSelectionMesh.uv;
		newMesh.uv2 = currentSelectionMesh.uv2;
		newMesh.uv2 = currentSelectionMesh.uv2;
		
		newMesh.RecalculateBounds();
		newMesh.RecalculateNormals();
		
		if (p != "") p = System.IO.Path.GetDirectoryName(p);
		else p = "Assets";
		
		string newPath = p + "/"  + currentSelection.name  + "_" + id +".assets";
		
		AssetDatabase.CreateAsset(newMesh, newPath);
		
		currentSelectionMeshFilter.sharedMesh = newMesh;
		MeshCollider mC = currentSelection.GetComponent<MeshCollider>();
		if (mC != null)
			mC.sharedMesh = newMesh;
		
		currentSelectionMesh = newMesh;
		
		if (toDelete!= "") {
			AssetDatabase.DeleteAsset(toDelete);
		}
		
		EditorUtility.SetDirty(currentSelection);
		AssetDatabase.Refresh();
	
	}
	
	void ApplyPlanerUV() {
		Vector2[] planarUvs = new Vector2[currentSelectionMeshFilter.sharedMesh.vertices.Length];
		
		
		
		for(int i=0;i<currentSelectionMeshFilter.sharedMesh.vertices.Length;i++){
			float y=0;
			if (DirectionY) {
				y = currentSelectionMeshFilter.sharedMesh.vertices[i].y;
			} else{
				y = currentSelectionMeshFilter.sharedMesh.vertices[i].z;	
			}
				
			
			planarUvs[i] = new Vector2(currentSelectionMeshFilter.sharedMesh.vertices[i].x ,y  );
		}
		
		
		currentSelectionMeshFilter.sharedMesh.uv = planarUvs;
		selectionUVData = planarUvs;
		Reset();
		
		EditorUtility.SetDirty(currentSelection);
	}	
	
	void ApplyScaledUV() {
		if ((activeScaleX == oldScaleX) && (activeScaleY == oldScaleY)) {
			return;	
		}
		if (scaleLocked && (activeScaleY == oldScaleY)) activeScaleY = activeScaleX;
		if (scaleLocked && (activeScaleX == oldScaleX)) activeScaleX = activeScaleY;
		
		Vector2[] planarUvs = new Vector2[currentSelectionMeshFilter.sharedMesh.vertices.Length];
		
		for (int i = 0; i < selectionUVData.Length; i++) {
			
			if (snapDecimal) {
				activeScaleX = (float) Math.Round(activeScaleX,1);
				activeScaleY = (float) Math.Round(activeScaleY,1);
			}
			
			float x = selectionUVData[i].x * activeScaleX;
			float y = selectionUVData[i].y * activeScaleY;
			planarUvs[i] = new Vector2(x, y);
		}
		
		currentSelectionMeshFilter.sharedMesh.uv = planarUvs;
		
		
		oldScaleX = activeScaleX;
		oldScaleY = activeScaleY;
		
		EditorUtility.SetDirty(currentSelection);
	}
	
	void RenderUVEditor() {
		if (currentSelectionMeshFilter == null) return;
		
		Rect nw = GUILayoutUtility.GetLastRect();
		areaRect = new Rect(nw.x,nw.yMax +30,nw.width,nw.width);
		RenderEditorMenu(new Rect(areaRect.x,areaRect.y-20, areaRect.width, 20));
		float viewBase = 1600;
		float zoom = zoomfactor * 200;
		float viewSize= viewBase ;
		areaVisible = new Rect(-600,-600, viewSize , viewSize );
		scrollpos = GUI.BeginScrollView(areaRect,scrollpos,areaVisible);
		Rect iRect = new Rect(0,0,zoom,zoom );
		if (selectionTexture != null){ 
			RenderBackgroundTexture(iRect, areaRect.width, areaRect.height);
		}
		if (selectionUVData != null) {
			for (int i = 0; i < currentSelectionMeshFilter.sharedMesh.uv.Length; i++) {
				Rect btnRect = BtnPosition(currentSelectionMeshFilter.sharedMesh.uv[i],iRect, areaRect.width, areaRect.height);
				if (selectedUV.Contains(i)) GUI.color = Color.green; else GUI.color = Color.gray;
				if (GUI.Button(btnRect,"*")) {
					if (!selectedUV.Contains(i)) selectedUV.Add(i);
			
				}
			}
		}
		
		GUI.EndScrollView();
		
		
	}
	
	void RenderBackgroundTexture(Rect area, float areaWidth, float areaHeight) {
			
		float midOffsetX = areaWidth/2;
		float midOffsetY = areaHeight/2;

		for (int x = -tileCount; x < tileCount; x++) {
			for (int y = -tileCount; y < tileCount; y++) {
				
				float offsetX = midOffsetX + x * (area.width );
				float offsetY = midOffsetY + y * (area.height);
				
				Rect texturePosition = new Rect( offsetX, offsetY, area.width , area.height );
				
				GUI.DrawTexture(texturePosition, selectionTexture);	
			}
		}
	}
	
	Rect BtnPosition(Vector2 pos, Rect area, float areaWidth, float areaHeight) {
		float xMid = areaWidth/2;
		float yMid = areaHeight/2 ;
		float btnSize = 10f;
		
		return new Rect(xMid + (pos.x) * (area.width ) - (btnSize/2),
					    yMid + (pos.y) * (area.height)  - (btnSize/2),
						btnSize,
						btnSize);
		
	}
	
	void HandleMouse() {
		if (Event.current!= null) {
			if ((areaRect.Contains(Event.current.mousePosition))) {
				
				if (Event.current.type == EventType.MouseDrag) {
					Vector2 d = Event.current.delta * (0.005f / zoomfactor);	
					
					for (int i = 0; i < selectedUV.Count; i++) {
						selectionUVData[selectedUV[i]] += d;
					}
					currentSelectionMeshFilter.sharedMesh.uv = selectionUVData;			
					EditorUtility.SetDirty(currentSelection);
				}
				
				if (Event.current.clickCount == 2)
					selectedUV.Clear();
			}
		}
		
		
	}
	
	void RenderEditorMenu(Rect pos) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Zoom : ", GUILayout.Width(50));
		zoomfactor = GUILayout.HorizontalSlider(zoomfactor,0.5f,3);
		if (zoomfactor>5) zoomfactor = Mathf.Ceil(zoomfactor);
		GUILayout.Label(Math.Round(zoomfactor,1).ToString(), GUILayout.Width(25));
		if (GUILayout.Button(".5X", GUILayout.Width(35) )) zoomfactor = 0.5f;
		if (GUILayout.Button("1X" , GUILayout.Width(35))) zoomfactor = 1f;
		if (GUILayout.Button("2X" , GUILayout.Width(35))) zoomfactor = 2f;
		if (GUILayout.Button("5X" , GUILayout.Width(35))) zoomfactor = 5f;
		GUILayout.EndHorizontal();
	}
	
	
	
}
