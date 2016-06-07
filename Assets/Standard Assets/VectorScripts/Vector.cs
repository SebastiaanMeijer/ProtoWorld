// Version 1.4.2
// Â©2011 Starscene Software. All rights reserved. Redistribution without permission not allowed.

using UnityEngine;
using System.Collections.Generic;

public class Vector {
	static Camera cam;
	static Transform camTransform;
	static Camera cam3D;
	public static Vector3 oldPosition;
	public static Vector3 oldRotation;
	static int _vectorLayer = 31;
	public static int vectorLayer {
		get {
			return _vectorLayer;
		}
		set {
			_vectorLayer = value;
			if (_vectorLayer > 31) _vectorLayer = 31;
			else if (_vectorLayer < 0) _vectorLayer = 0;
		}
	}
	static int _vectorLayer3D = 0;
	public static int vectorLayer3D {
		get {
			return _vectorLayer3D;
		}
		set {
			_vectorLayer3D = value;
			if (_vectorLayer > 31) _vectorLayer3D = 31;
			else if (_vectorLayer < 0) _vectorLayer3D = 0;
		}
	}
	static float zDist;
	static bool useOrthoCam;
	const float cutoff = .15f;
	static bool error = false;
	static bool lineManagerCreated = false; 
	static LineManager _lineManager;
	public static LineManager lineManager {
		get {
			// This prevents OnDestroy functions that reference VectorManager from creating LineManager again when editor play mode is stopped
			// Checking _lineManager == null can randomly fail, since the order of objects being Destroyed is undefined
			if (!lineManagerCreated) {
				lineManagerCreated = true;
				var lineManagerGO = new GameObject("LineManager");
				_lineManager = lineManagerGO.AddComponent(typeof(LineManager)) as LineManager;
				_lineManager.enabled = false;
				MonoBehaviour.DontDestroyOnLoad(_lineManager);
			}
			return _lineManager;
		}
	}
	static int widthIdxAdd;
	
	public static void LogError (string errorString) {
		Debug.LogError(errorString);
		error = true;
	}
	
	public static Vector3 camTransformPosition {
		get {
			return camTransform.position;
		}
	}
	
	public static Vector3 camTransformEulerAngles {
		get {
			return camTransform.eulerAngles;
		}
	}
	
	public static bool camTransformExists {
		get {
			if (camTransform) return true;
			return false;
		}
	}

	public static Camera SetCamera () {
		return SetCamera (CameraClearFlags.Depth, false);
	}
	
	public static Camera SetCamera (bool useOrtho) {
		return SetCamera (CameraClearFlags.Depth, useOrtho);
	}
	
	public static Camera SetCamera (CameraClearFlags clearFlags) {
		return SetCamera (clearFlags, false);
	}
	
	public static Camera SetCamera (CameraClearFlags clearFlags, bool useOrtho) {
		if (Camera.main == null) {
			LogError("Vector.SetCamera: no camera tagged \"Main Camera\" found");
			return null;
		}
		return SetCamera (Camera.main, clearFlags, useOrtho);
	}
	
	public static Camera SetCamera (Camera thisCamera) {
		return SetCamera (thisCamera, CameraClearFlags.Depth, false);
	}
	
	public static Camera SetCamera (Camera thisCamera, bool useOrtho) {
		return SetCamera (thisCamera, CameraClearFlags.Depth, useOrtho);
	}
	
	public static Camera SetCamera (Camera thisCamera, CameraClearFlags clearFlags) {
		return SetCamera (thisCamera, clearFlags, false);
	}
	
	public static Camera SetCamera (Camera thisCamera, CameraClearFlags clearFlags, bool useOrtho) {
		if (!cam) {
			cam = new GameObject("VectorCam", typeof(Camera)).GetComponent<Camera>();
			MonoBehaviour.DontDestroyOnLoad(cam);
		}
		cam.depth = thisCamera.depth+1;
		cam.clearFlags = clearFlags;
		cam.orthographic = useOrtho;
		useOrthoCam = useOrtho;
		if (useOrtho) {
			cam.orthographicSize = Screen.height/2;
			cam.farClipPlane = 101.1f;
			cam.nearClipPlane = .9f;
		}
		else {
			cam.fieldOfView = 90.0f;
			cam.farClipPlane = Screen.height/2 + .0101f;
			cam.nearClipPlane = Screen.height/2 - .0001f;
		}
		cam.transform.position = new Vector3(Screen.width/2 - .5f, Screen.height/2 - .5f, 0.0f);
		cam.transform.eulerAngles = Vector3.zero;
		cam.cullingMask = 1 << _vectorLayer;
		cam.backgroundColor = thisCamera.backgroundColor;
		
		thisCamera.cullingMask = thisCamera.cullingMask & (-1 ^ (1 << _vectorLayer));
		camTransform = thisCamera.transform;
		cam3D = thisCamera;
		oldPosition = camTransform.position + Vector3.one;
		oldRotation = camTransform.eulerAngles + Vector3.one;
		return cam;
	}
	
	public static void SetCamera3D () {
		if (Camera.main == null) {
			LogError("Vector.SetCamera3D: no camera tagged \"Main Camera\" found. Please call SetCamera3D with a specific camera instead.");
			return;
		}
		SetCamera3D (Camera.main);
	}
	
	public static void SetCamera3D (Camera thisCamera) {
		camTransform = thisCamera.transform;
		cam3D = thisCamera;
		oldPosition = camTransform.position + Vector3.one;
		oldRotation = camTransform.eulerAngles + Vector3.one;
	}
	
	public static void SetVectorCamDepth (int depth) {
		cam.depth = depth;
	}
	
	public static int GetSegmentNumber (VectorLine line) {
		if (line.continuousLine) {
			return (line.points2 == null)? line.points3.Length-1 : line.points2.Length-1;
		}
		else {
			return (line.points2 == null)? line.points3.Length/2 : line.points2.Length/2;
		}
	}
	
	static int GetPointsLength (VectorLine line) {
		return line.points2 != null? line.points2.Length : line.points3.Length;
	}

	static string[] functionNames = {"Vector: SetColors: Length of color", "Vector: SetColorsSmooth: Length of color", "Vector: SetWidths: Length of line widths", "MakeCurveInLine", "MakeSplineInLine", "MakeEllipseInLine"};
	enum FunctionName {SetColors, SetColorsSmooth, SetWidths, MakeCurveInLine, MakeSplineInLine, MakeEllipseInLine}
	
	static bool WrongArrayLength (VectorLine line, int arrayLength, FunctionName functionName) {
		int pointsLength = GetPointsLength (line);
		if (line.continuousLine) {
			if (arrayLength != pointsLength-1) {
				LogError(functionNames[(int)functionName] + " array for " + line.vectorObject.name + " must be length of points array minus one for a continuous line (one entry per line segment)");
				return true;
			}
		}
		else if (arrayLength != pointsLength/2) {
			LogError(functionNames[(int)functionName] + " array in " + line.vectorObject.name + " must be exactly half the length of points array for a discrete line (one entry per line segment)");
			return true;
		}
		return false;
	}
	
	private static bool CheckArrayLength (VectorLine line, FunctionName functionName, int segments, int index) {
		if (segments < 1) {
			LogError("Vector: " + functionNames[(int)functionName] + " needs at least 1 segment");
			return false;
		}
		int linePoints = (line.points2 == null)? line.points3.Length : line.points2.Length;

		if (line.isPoints) {
			if (index + segments > linePoints) {
				if (index == 0) {
					LogError("Vector: " + functionNames[(int)functionName] + ": The number of segments cannot exceed the number of points in the array for " + line.vectorObject.name);
					return false;
				}
				LogError("Vector: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for " + line.vectorObject.name);
				return false;				
			}
			return true;
		}

		if (line.continuousLine) {
			if (index + (segments+1) > linePoints) {
				if (index == 0) {
					LogError("Vector: " + functionNames[(int)functionName] + ": The length of the array for continuous lines needs to be at least the number of segments plus one for " + line.vectorObject.name);
					return false;
				}
				LogError("Vector: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for " + line.vectorObject.name);
				return false;
			}
		}
		else {
			if (index + segments*2 > linePoints) {
				if (index == 0) {
					LogError("Vector: " + functionNames[(int)functionName] + ": The length of the array for discrete lines needs to be at least twice the number of segments for " + line.vectorObject.name);
					return false;
				}
				LogError("Vector: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for " + line.vectorObject.name);
				return false;
			}	
		}
		return true;	
	}
	
	public static void SetColor (VectorLine line, Color color) {
		if (line.lineColors == null) {
			line.SetupVertexColors();
		}
		int end = line.lineColors.Length;
		for (int i = 0; i < end; i++) {
			line.lineColors[i] = color;
		}
		line.mesh.colors = line.lineColors;
	}

	public static void SetColors (VectorLine line, Color[] lineColors) {
		if (line.lineColors == null) {
			line.SetupVertexColors();
		}
		if (!line.isPoints) {
			if (WrongArrayLength (line, lineColors.Length, FunctionName.SetColors)) {
				return;
			}
		}
		else if (lineColors.Length != GetPointsLength(line)) {
			LogError("Vector: SetColors: Length of lineColors array in " + line.vectorObject.name + " must be same length as points array");
			return;
		}
		
		int start = 0;
		int end = lineColors.Length;
		SetStartAndEnd (line, ref start, ref end);
		int idx = start*4;
		
		for (int i = start; i < end; i++) {
			line.lineColors[idx]   = lineColors[i];
			line.lineColors[idx+1] = lineColors[i];
			line.lineColors[idx+2] = lineColors[i];
			line.lineColors[idx+3] = lineColors[i];
			idx += 4;
		}
		line.mesh.colors = line.lineColors;
	}
	
	public static void SetColorsSmooth (VectorLine line, Color[] lineColors) {
		if (line.isPoints) {
			LogError ("Vector: SetColorsSmooth must be used with a line rather than points");
			return;
		}
		if (line.lineColors == null || line.lineColors.Length == 0) {
			LogError ("Vector: SetColorsSmooth needs line colors to work");
			return;
		}
		if (WrongArrayLength (line, lineColors.Length, FunctionName.SetColorsSmooth)) {
			return;
		}
		
		int start = 0;
		int end = lineColors.Length;
		SetStartAndEnd (line, ref start, ref end);
		int idx = start*4;

		line.lineColors[idx++] = lineColors[start];
		line.lineColors[idx++] = lineColors[start];
		line.lineColors[idx++] = lineColors[start];
		line.lineColors[idx++] = lineColors[start];
		for (int i = start+1; i < end; i++) {
			line.lineColors[idx] = lineColors[i-1];
			line.lineColors[idx+1] = lineColors[i-1];
			line.lineColors[idx+2] = lineColors[i];
			line.lineColors[idx+3] = lineColors[i];
			idx += 4;
		}
		line.mesh.colors = line.lineColors;
	}

	static void SetStartAndEnd (VectorLine line, ref int start, ref int end) {
		start = (line.minDrawIndex == 0)? 0 : (line.continuousLine)? line.minDrawIndex : line.minDrawIndex/2;
		if (line.maxDrawIndex > 0) {
			if (line.continuousLine) {
				end = line.maxDrawIndex;
			}
			else {
				end = line.maxDrawIndex/2;
				if (line.maxDrawIndex%2 != 0) {
					end++;
				}
			}
		}
	}
	
	public static void SetWidths (VectorLine line, float[] lineWidths) {
		if (lineWidths == null) {
			LogError("Vector: SetWidths: line widths array must not be null");
			return;
		}
		if (line.isPoints) {
			if (lineWidths.Length != GetPointsLength(line)) {
				LogError("Vector: SetWidths: line widths array must be the same length as the points array for \"" + line.vectorObject.name + "\"");
				return;
			}
		}
		else if (WrongArrayLength (line, lineWidths.Length, FunctionName.SetWidths)) {
			return;
		}
		
		int end = lineWidths.Length;
		line.lineWidths = new float[end];
		for (int i = 0; i < end; i++) {
			line.lineWidths[i] = lineWidths[i] * .5f;
		}
	}
	
	static Material lineMaterial;
	static float lineWidth;
	static int lineDepth;
	static float capLength;
	static Color lineColor;
	static LineType lineType;
	static Joins joins;
	static bool set = false;
	static Vector3 v1 = Vector3.zero;
	static Vector3 v2 = Vector3.zero;
	
	public static void SetLineParameters (Color color, Material mat, float width, float cap, int depth, LineType thisLineType, Joins thisJoins) {
		lineColor = color;
		lineMaterial = mat;
		lineWidth = width;
		lineDepth = depth;
		capLength = cap;
		lineType = thisLineType;
		joins = thisJoins;
		set = true;
	}
	
	private static void PrintMakeLineError () {
		LogError("Vector: Must call SetLineParameters before using MakeLine with these parameters");
	}
	
	public static VectorLine MakeLine (string name, Vector3[] points, Color[] colors) {
		if (!set) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, colors, lineMaterial, lineWidth, lineType, joins);
		line.capLength = capLength;
		line.depth = lineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector2[] points, Color[] colors) {
		if (!set) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, colors, lineMaterial, lineWidth, lineType, joins);
		line.capLength = capLength;
		line.depth = lineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector3[] points, Color color) {
		if (!set) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, color, lineMaterial, lineWidth, lineType, joins);
		line.capLength = capLength;
		line.depth = lineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector2[] points, Color color) {
		if (!set) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, color, lineMaterial, lineWidth, lineType, joins);
		line.capLength = capLength;
		line.depth = lineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector3[] points) {
		if (!set) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, lineColor, lineMaterial, lineWidth, lineType, joins);
		line.capLength = capLength;
		line.depth = lineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector2[] points) {
		if (!set) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, lineColor, lineMaterial, lineWidth, lineType, joins);
		line.capLength = capLength;
		line.depth = lineDepth;
		return line;
	}

	public static VectorLine SetLine (Color color, params Vector2[] points) {
		return SetLine (color, 0.0f, points);
	}

	public static VectorLine SetLine (Color color, float time, params Vector2[] points) {
		if (points.Length < 2) {
			LogError("Vector.SetLine needs at least two points");
			return null;
		}
		VectorLine line = new VectorLine("Line", points, color, null, 1.0f, LineType.Continuous, Joins.None);
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		DrawLine(line);
		return line;
	}

	public static VectorLine SetLine (Color color, params Vector3[] points) {
		return SetLine (color, 0.0f, points);
	}
	
	public static VectorLine SetLine (Color color, float time, params Vector3[] points) {
		if (points.Length < 2) {
			LogError("Vector.SetLine needs at least two points");
			return null;
		}
		VectorLine line = new VectorLine("SetLine", points, color, null, 1.0f, LineType.Continuous, Joins.None);
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		DrawLine(line);
		return line;
	}

	public static VectorLine SetLine3D (Color color, params Vector3[] points) {
		return SetLine3D (color, 0.0f, points);
	}
		
	public static VectorLine SetLine3D (Color color, float time, params Vector3[] points) {
		if (points.Length < 2) {
			LogError("Vector.SetLine3D needs at least two points");
			return null;
		}
		VectorLine line = new VectorLine("SetLine3D", points, color, null, 1.0f, LineType.Continuous, Joins.None);
		DrawLine3DAuto (line, time);
		return line;
	}

	public static VectorLine SetRay (Color color, Vector3 origin, Vector3 direction) {
		return SetRay (color, 0.0f, origin, direction);
	}

	public static VectorLine SetRay (Color color, float time, Vector3 origin, Vector3 direction) {
		VectorLine line = new VectorLine("SetRay", new Vector3[] {origin, new Ray(origin, direction).GetPoint(direction.magnitude)}, color, null, 1.0f, LineType.Continuous, Joins.None);
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		DrawLine(line);
		return line;
	}

	public static VectorLine SetRay3D (Color color, Vector3 origin, Vector3 direction) {
		return SetRay3D (color, 0.0f, origin, direction);
	}

	public static VectorLine SetRay3D (Color color, float time, Vector3 origin, Vector3 direction) {
		VectorLine line = new VectorLine("SetRay3D", new Vector3[] {origin, new Ray(origin, direction).GetPoint(direction.magnitude)}, color, null, 1.0f, LineType.Continuous, Joins.None);
		DrawLine3DAuto (line, time);
		return line;
	}
	
	public static void DrawLine (VectorLine line) {
		DrawLine(line, null);
	}
	
	public static void DrawLine (VectorLine line, Transform thisTransform) {
		if (error || !line.active) return;
		if (line == null || line.vectorObject == null) {
			LogError("Vector.DrawLine: the line must not be null");
			return;
		}
		if (!cam) {
			SetCamera();
			if (!cam) {	// If that didn't work (no camera tagged "Main Camera")
				LogError("Vector.DrawLine: You must call SetCamera before calling DrawLine for " + line.vectorObject.name);
				return;
			}
		}
		if (line.smoothWidth && line.lineWidths.Length == 1 && GetPointsLength(line) > 2) {
			LogError("Vector: DrawLine called with smooth line widths for " + line.vectorObject.name + ", but Vector.SetWidths has not been used");
			return;
		}
	
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		zDist = useOrthoCam? 101-line.depth : Screen.height/2 + ((100.0f - line.depth) * .0001f);
		int end = 0;
		if (line.points2 != null) {
			end = (line.maxDrawIndex == 0)? line.points2.Length-1 : line.maxDrawIndex;
			Line2D (line, end, thisMatrix, useTransformMatrix);
		}
		else {
			end = (line.maxDrawIndex == 0)? line.points3.Length-1 : line.maxDrawIndex;
			if (line.continuousLine) {
				Line3DContinuous (line, end, thisMatrix, useTransformMatrix);
			}
			else {
				Line3DDiscrete (line, end, thisMatrix, useTransformMatrix);
			}
		}
		
		line.mesh.vertices = line.lineVertices;
		if (line.mesh.bounds.center.x != Screen.width/2) {
			SetLineMeshBounds (line);
		}
	}
	
	static void SetLineMeshBounds (VectorLine line) {
		var bounds = new Bounds();
		if (!useOrthoCam) {
			bounds.center = new Vector3(Screen.width/2, Screen.height/2, Screen.height/2);
			bounds.extents = new Vector3(Screen.width*100, Screen.height*100, .1f);
		}
		else {
			bounds.center = new Vector3(Screen.width/2, Screen.height/2, 50.5f);
			bounds.extents = new Vector3(Screen.width*100, Screen.height*100, 51.0f);
		}
		line.mesh.bounds = bounds;
	}
	
	public static void DrawLine3D (VectorLine line) {
		DrawLine3D (line, null);
	}
	
	public static void DrawLine3D (VectorLine line, Transform thisTransform) {
		if (error || !line.active) return;
		if (line == null || line.vectorObject == null) {
			LogError("Vector.DrawLine3D: the line must not be null");
			return;
		}
		if (!cam3D) {
			SetCamera3D();
			if (!cam3D) {
				LogError("Vector.DrawLine3D: You must call SetCamera or SetCamera3D before calling DrawLine3D for " + line.vectorObject.name);
				return;
			}
		}
		if (line.points3 == null) {
			LogError("Vector: DrawLine3D can only be used with a Vector3 array, which " + line.vectorObject.name + " doesn't have");
			return;
		}
		if (line.smoothWidth && line.lineWidths.Length == 1 && line.points3.Length > 2) {
			LogError("Vector: DrawLine3D called with smooth line widths for " + line.vectorObject.name + ", but Vector.SetWidths has not been used");
			return;
		}
		
		if (line.layer == -1) {
			line.vectorObject.layer = _vectorLayer3D;
			line.layer = _vectorLayer3D;
		}
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		int end = (line.maxDrawIndex == 0)? line.points3.Length-1 : line.maxDrawIndex;
		int add = line.continuousLine? 1 : 2;
		int idx = line.minDrawIndex*4;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (line.lineWidths.Length > 1) {
			widthIdx = line.minDrawIndex;
			widthIdxAdd = 1;
		}
		if (!line.continuousLine) {
			idx /= 2;
			widthIdx /= 2;
		}
		var pos1 = Vector3.zero;
		var pos2 = Vector3.zero;
		
		for (int i = line.minDrawIndex; i < end; i += add) {
			if (useTransformMatrix) {
				pos1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i]));
				pos2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i+1]));
			}
			else {
				pos1 = cam3D.WorldToScreenPoint(line.points3[i]);
				pos2 = cam3D.WorldToScreenPoint(line.points3[i+1]);
			}
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;
			Vector3 thisLine = (v1 - v2).normalized;
			Vector3 perpendicular = thisLine * line.lineWidths[widthIdx];
			
			line.screenPoints[idx]   = pos1 - perpendicular;	// Used for Joins.Weld
			line.screenPoints[idx+1] = pos1 + perpendicular;
			line.lineVertices[idx]   = cam3D.ScreenToWorldPoint(line.screenPoints[idx]);
			line.lineVertices[idx+1] = cam3D.ScreenToWorldPoint(line.screenPoints[idx+1]);
			
			if (line.smoothWidth && i < end-add) {
				perpendicular = thisLine * line.lineWidths[widthIdx+1];
			}
			line.screenPoints[idx+2] = pos2 - perpendicular;
			line.screenPoints[idx+3] = pos2 + perpendicular;
			line.lineVertices[idx+2] = cam3D.ScreenToWorldPoint(line.screenPoints[idx+2]);
			line.lineVertices[idx+3] = cam3D.ScreenToWorldPoint(line.screenPoints[idx+3]);
			
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		if (line.weldJoins) {
			if (line.continuousLine) {
				WeldJoins3D (line, line.minDrawIndex*4 + 4, end*4, line.points3[0] == line.points3[line.points3.Length-1]
					&& line.minDrawIndex == 0 && (line.maxDrawIndex == line.points3.Length-1 || line.maxDrawIndex == 0));
			}
			else {
				WeldJoinsDiscrete3D (line, line.minDrawIndex + 1, end, line.points3[0] == line.points3[line.points3.Length-1]
					&& line.minDrawIndex == 0 && (line.maxDrawIndex == line.points3.Length-1 || line.maxDrawIndex == 0));
			}
		}
		
		line.mesh.vertices = line.lineVertices;
		line.mesh.RecalculateBounds();
	}
	
	public static void LineManagerCheckDistance () {
		lineManager.StartCheckDistance();
	}
	
	public static void LineManagerDisable () {
		lineManager.DisableIfUnused();
	}
	
	public static void LineManagerEnable () {
		lineManager.EnableIfUsed();
	}

	public static void DrawLine3DAuto (VectorLine line) {
		DrawLine3DAuto (line, 0.0f, null);
	}

	public static void DrawLine3DAuto (VectorLine line, float time) {
		DrawLine3DAuto (line, time, null);
	}

	public static void DrawLine3DAuto (VectorLine line, Transform thisTransform) {
		DrawLine3DAuto (line, 0.0f, thisTransform);
	}
	
	public static void DrawLine3DAuto (VectorLine line, float time, Transform thisTransform) {
		if (time < 0.0f) time = 0.0f;
		lineManager.AddLine (line, thisTransform, time);
		DrawLine3D (line, thisTransform);
	}
	
	public static void StopDrawingLine3DAuto (VectorLine line) {
		lineManager.RemoveLine (line);
	}
	
	static void Line2D (VectorLine line, int end, Matrix4x4 thisMatrix, bool doTransform) {
		Vector3 p1 = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 p2 = new Vector3(0.0f, 0.0f, 0.0f);
		int add = line.continuousLine? 1 : 2;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (line.lineWidths.Length > 1) {
			widthIdx = line.minDrawIndex;
			widthIdxAdd = 1;
		}
		int idx = line.minDrawIndex*4;
		if (!line.continuousLine) {
			idx /= 2;
			widthIdx /= 2;
		}
		
		if (line.capLength == 0.0f) {
			for (int i = line.minDrawIndex; i < end; i += add) {
				if (doTransform) {
					p1 = thisMatrix.MultiplyPoint3x4(line.points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(line.points2[i+1]);
				}
				else {
					p1 = line.points2[i];
					p2 = line.points2[i+1];
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (line, ref idx, ref widthIdx, ref p1); continue;}
				p2.z = zDist;
				
				v1.x = p2.y; v1.y = p1.x;
				v2.x = p1.y; v2.y = p2.x;
				Vector3 perpendicular = v1 - v2;
				float normalizedDistance = ( 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y)) );
				perpendicular *= normalizedDistance * line.lineWidths[widthIdx];
				line.lineVertices[idx]   = p1 - perpendicular;
				line.lineVertices[idx+1] = p1 + perpendicular;
				if (line.smoothWidth && i < end-add) {
					perpendicular = v1 - v2;
					perpendicular *= normalizedDistance * line.lineWidths[widthIdx+1];
				}
				line.lineVertices[idx+2] = p2 - perpendicular;
				line.lineVertices[idx+3] = p2 + perpendicular;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
			if (line.weldJoins) {
				if (line.continuousLine) {
					WeldJoins (line, line.minDrawIndex*4 + 4, end*4, line.points2[0] == line.points2[line.points2.Length-1]
						&& line.minDrawIndex == 0 && (line.maxDrawIndex == line.points2.Length-1 || line.maxDrawIndex == 0));
				}
				else {
					WeldJoinsDiscrete (line, line.minDrawIndex + 1, end, line.points2[0] == line.points2[line.points2.Length-1]
						&& line.minDrawIndex == 0 && (line.maxDrawIndex == line.points2.Length-1 || line.maxDrawIndex == 0));
				}
			}
		}
		else {
			for (int i = line.minDrawIndex; i < end; i += add) {
				if (doTransform) {
					p1 = thisMatrix.MultiplyPoint3x4(line.points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(line.points2[i+1]);
				}
				else {
					p1 = line.points2[i];
					p2 = line.points2[i+1];
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (line, ref idx, ref widthIdx, ref p1); continue;}
				p2.z = zDist;
				
				Vector3 thisLine = p2 - p1;
				thisLine *= ( 1.0f / Mathf.Sqrt((thisLine.x * thisLine.x) + (thisLine.y * thisLine.y)) );
				p1 -= thisLine * line.capLength;
				p2 += thisLine * line.capLength;

				v1.x = thisLine.y; v1.y = -thisLine.x;
				thisLine = v1 * line.lineWidths[widthIdx];
				line.lineVertices[idx]   = p1 - thisLine;
				line.lineVertices[idx+1] = p1 + thisLine;
				if (line.smoothWidth && i < end-add) {
					thisLine = v1 * line.lineWidths[widthIdx+1];
				}
				line.lineVertices[idx+2] = p2 - thisLine;
				line.lineVertices[idx+3] = p2 + thisLine;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
	}

	static void WeldJoins (VectorLine line, int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			var lineLength = line.lineVertices.Length;
			SetIntersectionPoint (line, lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint (line, lineLength-3, lineLength-1, 1, 3);
		}
		for (int i = start; i < end; i+= 4) {
			SetIntersectionPoint (line, i-4, i-2, i, i+2);
			SetIntersectionPoint (line, i-3, i-1, i+1, i+3);
		}
	}

	static void WeldJoinsDiscrete (VectorLine line, int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			var lineLength = line.lineVertices.Length;
			SetIntersectionPoint (line, lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint (line, lineLength-3, lineLength-1, 1, 3);
		}
		int idx = (start+1) / 2 * 4;
		if (line.points2 != null) {
			for (int i = start; i < end; i+= 2) {
				if (line.points2[i] == line.points2[i+1]) {
					SetIntersectionPoint (line, idx-4, idx-2, idx,   idx+2);
					SetIntersectionPoint (line, idx-3, idx-1, idx+1, idx+3);
				}
				idx += 4;
			}
		}
		else {
			for (int i = start; i < end; i+= 2) {
				if (line.points3[i] == line.points3[i+1]) {
					SetIntersectionPoint (line, idx-4, idx-2, idx,   idx+2);
					SetIntersectionPoint (line, idx-3, idx-1, idx+1, idx+3);
				}
				idx += 4;
			}
		}
	}

	static void SetIntersectionPoint (VectorLine line, int p1, int p2, int p3, int p4) {
		var l1a = line.lineVertices[p1]; var l1b = line.lineVertices[p2];
		var l2a = line.lineVertices[p3]; var l2b = line.lineVertices[p4];
		float d = (l2b.y - l2a.y)*(l1b.x - l1a.x) - (l2b.x - l2a.x)*(l1b.y - l1a.y);
		if (d == 0.0f) return;	// Parallel lines
		float n = ( (l2b.x - l2a.x)*(l1a.y - l2a.y) - (l2b.y - l2a.y)*(l1a.x - l2a.x) ) / d;
		line.lineVertices[p2].x = l1a.x + (n * (l1b.x - l1a.x));
		line.lineVertices[p2].y = l1a.y + (n * (l1b.y - l1a.y));
		line.lineVertices[p3] = line.lineVertices[p2];
	}

	static void WeldJoins3D (VectorLine line, int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			var lineLength = line.lineVertices.Length;
			SetIntersectionPoint3D (line, lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint3D (line, lineLength-3, lineLength-1, 1, 3);
		}
		for (int i = start; i < end; i+= 4) {
			SetIntersectionPoint3D (line, i-4, i-2, i, i+2);
			SetIntersectionPoint3D (line, i-3, i-1, i+1, i+3);
		}
	}

	static void WeldJoinsDiscrete3D (VectorLine line, int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			var lineLength = line.lineVertices.Length;
			SetIntersectionPoint3D (line, lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint3D (line, lineLength-3, lineLength-1, 1, 3);
		}
		int idx = (start+1) / 2 * 4;
		for (int i = start; i < end; i+= 2) {
			if (line.points3[i] == line.points3[i+1]) {
				SetIntersectionPoint3D (line, idx-4, idx-2, idx,   idx+2);
				SetIntersectionPoint3D (line, idx-3, idx-1, idx+1, idx+3);
			}
			idx += 4;
		}
	}

	static void SetIntersectionPoint3D (VectorLine line, int p1, int p2, int p3, int p4) {
		var l1a = line.screenPoints[p1]; var l1b = line.screenPoints[p2];
		var l2a = line.screenPoints[p3]; var l2b = line.screenPoints[p4];
		float d = (l2b.y - l2a.y)*(l1b.x - l1a.x) - (l2b.x - l2a.x)*(l1b.y - l1a.y);
		if (d == 0.0f) return;	// Parallel lines
		float n = ( (l2b.x - l2a.x)*(l1a.y - l2a.y) - (l2b.y - l2a.y)*(l1a.x - l2a.x) ) / d;
		line.screenPoints[p2].x = l1a.x + (n * (l1b.x - l1a.x));
		line.screenPoints[p2].y = l1a.y + (n * (l1b.y - l1a.y));
		line.lineVertices[p2] = cam3D.ScreenToWorldPoint(line.screenPoints[p2]);
		line.lineVertices[p3] = line.lineVertices[p2];
	}
	
	static void Line3DContinuous (VectorLine line, int end, Matrix4x4 thisMatrix, bool doTransform) {
		var pos1 = Vector3.zero;
		var pos2 = doTransform? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[0])) :
									cam3D.WorldToScreenPoint(line.points3[0]);
		pos2.z = pos2.z < cutoff? -zDist : zDist;
		var perpendicular = Vector3.zero;
		float normalizedDistance = 0.0f;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (line.lineWidths.Length > 1) {
			widthIdx = line.minDrawIndex;
			widthIdxAdd = 1;
		}
		int idx = line.minDrawIndex*4;
		if (!line.continuousLine) idx /= 2;
		
		for (int i = line.minDrawIndex; i < end; i++) {
			pos1 = pos2;
			pos2 = doTransform? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i+1])) :
								cam3D.WorldToScreenPoint(line.points3[i+1]);
			if (pos1.x == pos2.x && pos1.y == pos2.y) {Skip (line, ref idx, ref widthIdx, ref pos1); continue;}
			pos2.z = pos2.z < cutoff? -zDist : zDist;
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;
			perpendicular = v1 - v2;
			normalizedDistance = 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y));
			perpendicular *= normalizedDistance * line.lineWidths[widthIdx];
			line.lineVertices[idx]   = pos1 - perpendicular;
			line.lineVertices[idx+1] = pos1 + perpendicular;
			if (line.smoothWidth && i < end-1) {
				perpendicular = v1 - v2;
				perpendicular *= normalizedDistance * line.lineWidths[widthIdx+1];
			}
			line.lineVertices[idx+2] = pos2 - perpendicular;
			line.lineVertices[idx+3] = pos2 + perpendicular;
			idx += 4;
			widthIdx += widthIdxAdd;
		}

		if (line.weldJoins) {
			WeldJoins (line, line.minDrawIndex*4 + 4, end*4, line.points3[0] == line.points3[line.points3.Length-1]
				&& line.minDrawIndex == 0 && (line.maxDrawIndex == line.points3.Length-1 || line.maxDrawIndex == 0));
		}
	}

	static void Line3DDiscrete (VectorLine line, int end, Matrix4x4 thisMatrix, bool doTransform) {
		var pos1 = Vector3.zero;
		var pos2 = Vector3.zero;
		var perpendicular = Vector3.zero;
		float normalizedDistance = 0.0f;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (line.lineWidths.Length > 1) {
			widthIdx = line.minDrawIndex;
			widthIdxAdd = 1;
		}
		int idx = line.minDrawIndex*4;
		if (!line.continuousLine) {
			idx /= 2;
			widthIdx /= 2;
		}

		for (int i = line.minDrawIndex; i < end; i += 2) {
			if (doTransform) {
				pos1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i]));
				pos2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i+1]));
			}
			else {
				pos1 = cam3D.WorldToScreenPoint(line.points3[i]);
				pos2 = cam3D.WorldToScreenPoint(line.points3[i+1]);
			}
			pos1.z = pos1.z < cutoff? -zDist : zDist;
			if (pos1.x == pos2.x && pos1.y == pos2.y) {Skip (line, ref idx, ref widthIdx, ref pos1); continue;}
			pos2.z = pos2.z < cutoff? -zDist : zDist;
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;
			perpendicular = v1 - v2;
			normalizedDistance = 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y));
			perpendicular *= normalizedDistance * line.lineWidths[widthIdx];
			line.lineVertices[idx]   = pos1 - perpendicular;
			line.lineVertices[idx+1] = pos1 + perpendicular;
			if (line.smoothWidth && i < end-2) {
				perpendicular = v1 - v2;
				perpendicular *= normalizedDistance * line.lineWidths[widthIdx+1];
			}
			line.lineVertices[idx+2] = pos2 - perpendicular;
			line.lineVertices[idx+3] = pos2 + perpendicular;
			idx += 4;
			widthIdx += widthIdxAdd;
		}

		if (line.weldJoins) {
			WeldJoinsDiscrete (line, line.minDrawIndex + 1, end, line.points3[0] == line.points3[line.points3.Length-1]
				&& line.minDrawIndex == 0 && (line.maxDrawIndex == line.points3.Length-1 || line.maxDrawIndex == 0));
		}
	}

	public static void SetTextureScale (VectorLine line, float textureScale) {
		SetTextureScale (line, null, textureScale, 0.0f);
	}

	public static void SetTextureScale (VectorLine line, Transform thisTransform, float textureScale) {
		SetTextureScale (line, thisTransform, textureScale, 0.0f);
	}

	public static void SetTextureScale (VectorLine line, float textureScale, float offset) {
		SetTextureScale (line, null, textureScale, offset);
	}
	
	public static void SetTextureScale (VectorLine line, Transform thisTransform, float textureScale, float offset) {
		int pointsLength = GetPointsLength (line);
		int end = line.continuousLine? pointsLength-1 : pointsLength;
		int add = line.continuousLine? 1 : 2;
		int idx = 0;
		int widthIdx = 0;
		widthIdxAdd = line.lineWidths.Length == 1? 0 : 1;
		float thisScale = 1.0f / textureScale;
		
		if (line.points2 != null) {
			for (int i = 0; i < end; i += add) {
				float xPos = thisScale / (line.lineWidths[widthIdx]*2 / (line.points2[i] - line.points2[i+1]).magnitude);
				line.lineUVs[idx++].x = offset;
				line.lineUVs[idx++].x = offset;
				line.lineUVs[idx++].x = xPos + offset;
				line.lineUVs[idx++].x = xPos + offset;
				offset = (offset + xPos) % 1;
				widthIdx += widthIdxAdd;
			}
		}
		else {
			if (!cam3D) {
				SetCamera3D();
				if (!cam3D) {
					LogError("Vector.SetTextureScale: You must call SetCamera3D before calling SetTextureScale");
					return;
				}
			}
			
			var useTransformMatrix = (thisTransform == null)? false : true;
			var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
			var p1 = Vector2.zero;
			var p2 = Vector2.zero;
			for (int i = 0; i < end; i += add) {
				if (useTransformMatrix) {
					p1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i]));
					p2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i+1]));					
				}
				else {
					p1 = cam3D.WorldToScreenPoint(line.points3[i]);
					p2 = cam3D.WorldToScreenPoint(line.points3[i+1]);
				}
				float xPos = thisScale / (line.lineWidths[widthIdx]*2 / (p1 - p2).magnitude);
				line.lineUVs[idx++].x = offset;
				line.lineUVs[idx++].x = offset;
				line.lineUVs[idx++].x = xPos + offset;
				line.lineUVs[idx++].x = xPos + offset;
				offset = (offset + xPos) % 1;
				widthIdx += widthIdxAdd;
			}
		}
		
		line.mesh.uv = line.lineUVs;
	}

	public static void ResetTextureScale (VectorLine line) {
		int end = line.lineUVs.Length;
		
		for (int i = 0; i < end; i += 4) {
			line.lineUVs[i  ].x = 0.0f;
			line.lineUVs[i+1].x = 0.0f;
			line.lineUVs[i+2].x = 1.0f;
			line.lineUVs[i+3].x = 1.0f;
		}
		
		line.mesh.uv = line.lineUVs;
	}
	
	public static void DrawPoints (VectorPoints line) {
		DrawPoints (line, null);
	}
	
	public static void DrawPoints (VectorPoints line, Transform thisTransform) {
		if (error || !line.active) return;
		if (line == null || line.vectorObject == null) {
			LogError("Vector.DrawPoints: the line must not be null");
			return;
		}
		if (!cam) {
			SetCamera();
			if (!cam) {
				LogError("Vector.DrawPoints: You must call SetCamera before calling DrawPoints");
				return;
			}
		}
	
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		zDist = useOrthoCam? 101-line.depth : Screen.height/2 + ((100.0f - line.depth) * .0001f);

		int end = (line.maxDrawIndex == 0)? GetPointsLength(line)-1 : line.maxDrawIndex;
		int idx = line.minDrawIndex*4;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (line.lineWidths.Length > 1) {
			widthIdx = line.minDrawIndex;
			widthIdxAdd = 1;
		}
		var pos1 = Vector3.zero;

		if (line.points2 == null) {
			for (int i = line.minDrawIndex; i <= end; i++) {
				pos1 = useTransformMatrix? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i])) :
										   cam3D.WorldToScreenPoint(line.points3[i]);
				if (pos1.z < cutoff) {
					Skip (line, ref idx, ref widthIdx, ref pos1);
					continue;
				}
				pos1.z = zDist;
				v1.x = v1.y = v2.y = line.lineWidths[widthIdx];
				v2.x = -line.lineWidths[widthIdx];				

				line.lineVertices[idx]   = pos1 + v2;
				line.lineVertices[idx+1] = pos1 - v1;
				line.lineVertices[idx+2] = pos1 + v1;
				line.lineVertices[idx+3] = pos1 - v2;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
		else {
			for (int i = line.minDrawIndex; i <= end; i++) {
				pos1 = useTransformMatrix? thisMatrix.MultiplyPoint3x4(line.points2[i]) : (Vector3)line.points2[i];
				pos1.z = zDist;
				v1.x = v1.y = v2.y = line.lineWidths[widthIdx];
				v2.x = -line.lineWidths[widthIdx];
	
				line.lineVertices[idx]   = pos1 + v2;
				line.lineVertices[idx+1] = pos1 - v1;
				line.lineVertices[idx+2] = pos1 + v1;
				line.lineVertices[idx+3] = pos1 - v2;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
		
		line.mesh.vertices = line.lineVertices;
		if (line.mesh.bounds.center.x != Screen.width/2) {
			SetLineMeshBounds (line);
		}
	}

	public static void DrawPoints3D (VectorPoints line) {
		DrawPoints3D (line, null);
	}

	public static void DrawPoints3D (VectorPoints line, Transform thisTransform) {
		if (error || !line.active) return;
		if (line == null || line.vectorObject == null) {
			LogError("Vector.DrawPoints: the line must not be null");
			return;
		}
		if (!cam3D) {
			SetCamera3D();
			if (!cam3D) {
				LogError("Vector.DrawPoints3D: You must call SetCamera or SetCamera3D before calling DrawPoints3D for " + line.vectorObject.name);
				return;
			}
		}
		if (line.points3 == null) {
			LogError("Vector: DrawPoints3D can only be used with a Vector3 array, which " + line.vectorObject.name + " doesn't have");
			return;
		}
		
		if (line.layer == -1) {
			line.vectorObject.layer = _vectorLayer3D;
			line.layer = _vectorLayer3D;
		}
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;

		int end = (line.maxDrawIndex == 0)? GetPointsLength(line)-1 : line.maxDrawIndex;
		int idx = line.minDrawIndex*4;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (line.lineWidths.Length > 1) {
			widthIdx = line.minDrawIndex;
			widthIdxAdd = 1;
		}
		var pos1 = Vector3.zero;
		
		for (int i = line.minDrawIndex; i <= end; i++) {
			pos1 = useTransformMatrix? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(line.points3[i])) :
									   cam3D.WorldToScreenPoint(line.points3[i]);
			if (pos1.z < cutoff) {
				pos1 = Vector3.zero;
				Skip (line, ref idx, ref widthIdx, ref pos1);
				continue;
			}
			v1.x = v1.y = v2.y = line.lineWidths[widthIdx];
			v2.x = -line.lineWidths[widthIdx];

			line.lineVertices[idx]   = cam3D.ScreenToWorldPoint(pos1 + v2);
			line.lineVertices[idx+1] = cam3D.ScreenToWorldPoint(pos1 - v1);
			line.lineVertices[idx+2] = cam3D.ScreenToWorldPoint(pos1 + v1);
			line.lineVertices[idx+3] = cam3D.ScreenToWorldPoint(pos1 - v2);
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		line.mesh.vertices = line.lineVertices;
		line.mesh.RecalculateBounds();
	}

	static void Skip (VectorLine line, ref int idx, ref int widthIdx, ref Vector3 pos) {
		line.lineVertices[idx] = line.lineVertices[idx+1] = line.lineVertices[idx+2] = line.lineVertices[idx+3] = pos;
		idx += 4;
		widthIdx += widthIdxAdd;
	}
	
	public static void SetDepth (Transform thisTransform, int depth) {
		depth = Mathf.Clamp(depth, 0, 100);
		thisTransform.position = new Vector3(thisTransform.position.x,
											 thisTransform.position.y,
											 useOrthoCam? 101-depth : Screen.height/2 + ((100.0f - depth) * .0001f));		
	}
	
	static int endianDiff1;
	static int endianDiff2;
	static byte[] byteBlock;
	
	public static Vector3[] BytesToVector3Array (byte[] lineBytes) {
		if (lineBytes.Length % 12 != 0) {
			LogError("Vector.BytesToVector3Array: Incorrect input byte length...must be a multiple of 12");
			return new Vector3[0];
		}
		
		SetupByteBlock();
		Vector3[] points = new Vector3[lineBytes.Length/12];
		int idx = 0;
		for (int i = 0; i < lineBytes.Length; i += 12) {
			points[idx++] = new Vector3( ConvertToFloat (lineBytes, i),
										 ConvertToFloat (lineBytes, i+4),
										 ConvertToFloat (lineBytes, i+8) );
		}
		return points;
	}
	
	public static Vector2[] BytesToVector2Array (byte[] lineBytes) {
		if (lineBytes.Length % 8 != 0) {
			LogError("Vector.BytesToVector2Array: Incorrect input byte length...must be a multiple of 8");
			return new Vector2[0];
		}
		
		SetupByteBlock();
		Vector2[] points = new Vector2[lineBytes.Length/8];
		int idx = 0;
		for (int i = 0; i < lineBytes.Length; i += 8) {
			points[idx++] = new Vector2( ConvertToFloat (lineBytes, i),
										 ConvertToFloat (lineBytes, i+4));
		}
		return points;
	}
	
	static void SetupByteBlock () {
		if (byteBlock == null) {byteBlock = new byte[4];}
		if (System.BitConverter.IsLittleEndian) {endianDiff1 = 0; endianDiff2 = 0;}
		else {endianDiff1 = 3; endianDiff2 = 1;}	
	}
	
	// Unfortunately we can't just use System.BitConverter.ToSingle as-is...we need a function to handle both big-endian and little-endian systems
	static float ConvertToFloat (byte[] bytes, int i) {
		byteBlock[    endianDiff1] = bytes[i];
		byteBlock[1 + endianDiff2] = bytes[i+1];
		byteBlock[2 - endianDiff2] = bytes[i+2];
		byteBlock[3 - endianDiff1] = bytes[i+3];
		return System.BitConverter.ToSingle (byteBlock, 0);
	}
	
	public static void DestroyLine (ref VectorLine line) {
		if (line != null) {
			MonoBehaviour.Destroy (line.vectorObject);
			MonoBehaviour.Destroy (line.mesh);
			MonoBehaviour.Destroy (line.meshFilter);
			line = null;
		}
	}

	public static void DestroyObject (ref VectorLine line, GameObject go) {
		DestroyLine (ref line);
		if (go != null) {
			MonoBehaviour.Destroy (go);
		}
	}
	
	public static void MakeRectInLine (VectorLine line, Rect rect) {
		MakeRectInLine (line, new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y-rect.height), 0);
	}

	public static void MakeRectInLine (VectorLine line, Rect rect, int index) {
		MakeRectInLine (line, new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y-rect.height), index);
	}

	public static void MakeRectInLine (VectorLine line, Vector3 topLeft, Vector3 bottomRight) {
		MakeRectInLine (line, topLeft, bottomRight, 0);
	}

	public static void MakeRectInLine (VectorLine line, Vector3 topLeft, Vector3 bottomRight, int index) {
		int linePoints = (line.points2 == null)? line.points3.Length : line.points2.Length;

		if (line.continuousLine) {
			if (index + 5 > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeRectInLine: The length of the array for continuous lines needs to be at least 5 for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeRectInLine with an index of " + index + " would exceed the length of the Vector2 array for " + line.vectorObject.name);
				return;
			}
			if (line.points2 == null) {
				line.points3[index  ] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
				line.points3[index+1] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				line.points3[index+2] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);
				line.points3[index+3] = new Vector3(topLeft.x,	   bottomRight.y, bottomRight.z);
				line.points3[index+4] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
			}
			else {
				line.points2[index  ] = new Vector2(topLeft.x,     topLeft.y);
				line.points2[index+1] = new Vector2(bottomRight.x, topLeft.y);
				line.points2[index+2] = new Vector2(bottomRight.x, bottomRight.y);
				line.points2[index+3] = new Vector2(topLeft.x,	   bottomRight.y);
				line.points2[index+4] = new Vector2(topLeft.x,     topLeft.y);
			}
		}
		
		else {
			if (index + 8 > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeRectInLine: The length of the array for discrete lines needs to be at least 8 for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeRectInLine with an index of " + index + " would exceed the length of the Vector2 array for " + line.vectorObject.name);
				return;
			}
			if (line.points2 == null) {
				line.points3[index  ] = new Vector3(topLeft.x,     topLeft.y,	  topLeft.z);
				line.points3[index+1] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				line.points3[index+2] = new Vector3(topLeft.x,     bottomRight.y, bottomRight.z);
				line.points3[index+3] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);
				line.points3[index+4] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
				line.points3[index+5] = new Vector3(topLeft.x,     bottomRight.y, bottomRight.z);
				line.points3[index+6] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				line.points3[index+7] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);			
			}
			else {
				line.points2[index  ] = new Vector2(topLeft.x,     topLeft.y);
				line.points2[index+1] = new Vector2(bottomRight.x, topLeft.y);
				line.points2[index+2] = new Vector2(topLeft.x,     bottomRight.y);
				line.points2[index+3] = new Vector2(bottomRight.x, bottomRight.y);
				line.points2[index+4] = new Vector2(topLeft.x,     topLeft.y);
				line.points2[index+5] = new Vector2(topLeft.x,     bottomRight.y);
				line.points2[index+6] = new Vector2(bottomRight.x, topLeft.y);
				line.points2[index+7] = new Vector2(bottomRight.x, bottomRight.y);
			}
		}
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius) {
		MakeEllipseInLine (line, origin, Vector3.forward, radius, radius, GetSegmentNumber(line), 0.0f, 0);
	}
	
	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments) {
		MakeEllipseInLine (line, origin, Vector3.forward, radius, radius, segments, 0.0f, 0);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments, float pointRotation) {
		MakeEllipseInLine (line, origin, Vector3.forward, radius, radius, segments, pointRotation, 0);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments, int index) {
		MakeEllipseInLine (line, origin, Vector3.forward, radius, radius, segments, 0.0f, index);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments, float pointRotation, int index) {
		MakeEllipseInLine (line, origin, Vector3.forward, radius, radius, segments, pointRotation, index);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, Vector3 upVector, float radius) {
		MakeEllipseInLine (line, origin, upVector, radius, radius, GetSegmentNumber(line), 0.0f, 0);
	}
	
	public static void MakeCircleInLine (VectorLine line, Vector3 origin, Vector3 upVector, float radius, int segments) {
		MakeEllipseInLine (line, origin, upVector, radius, radius, segments, 0.0f, 0);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation) {
		MakeEllipseInLine (line, origin, upVector, radius, radius, segments, pointRotation, 0);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, Vector3 upVector, float radius, int segments, int index) {
		MakeEllipseInLine (line, origin, upVector, radius, radius, segments, 0.0f, index);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation, int index) {
		MakeEllipseInLine (line, origin, upVector, radius, radius, segments, pointRotation, index);
	}

	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius) {
		MakeEllipseInLine (line, origin, Vector3.forward, xRadius, yRadius, GetSegmentNumber(line), 0.0f, 0);
	}
	
	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius, int segments) {
		MakeEllipseInLine (line, origin, Vector3.forward, xRadius, yRadius, segments, 0.0f, 0);
	}
	
	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius, int segments, int index) {
		MakeEllipseInLine (line, origin, Vector3.forward, xRadius, yRadius, segments, 0.0f, index);
	}

	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius, int segments, float pointRotation) {
		MakeEllipseInLine (line, origin, Vector3.forward, xRadius, yRadius, segments, pointRotation, 0);
	}

	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, Vector3 upVector, float xRadius, float yRadius) {
		MakeEllipseInLine (line, origin, upVector, xRadius, yRadius, GetSegmentNumber(line), 0.0f, 0);
	}

	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments) {
		MakeEllipseInLine (line, origin, upVector, xRadius, yRadius, segments, 0.0f, 0);
	}
	
	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, int index) {
		MakeEllipseInLine (line, origin, upVector, xRadius, yRadius, segments, 0.0f, index);
	}

	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation) {
		MakeEllipseInLine (line, origin, upVector, xRadius, yRadius, segments, pointRotation, 0);
	}
	
	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation, int index) {
		if (segments < 3) {
			LogError("Vector: MakeEllipseInLine needs at least 3 segments");
			return;
		}
		if (!CheckArrayLength (line, FunctionName.MakeEllipseInLine, segments, index)) {
			return;
		}
		
		float radians = 360.0f / segments*Mathf.Deg2Rad;
		float p = -pointRotation*Mathf.Deg2Rad;
		
		if (line.continuousLine) {
			int i = 0;
			if (line.points2 == null) {
				var thisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
				for (i = 0; i < segments; i++) {
					line.points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f));
					p += radians;
				}
				if (!line.isPoints) {
					line.points3[index+i] = line.points3[index+(i-segments)];
				}			
			}
			else {
				Vector2 v2Origin = origin;
				for (i = 0; i < segments; i++) {
					line.points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
					p += radians;
				}
				if (!line.isPoints) {
					line.points2[index+i] = line.points2[index+(i-segments)];
				}
			}
		}
		
		else {
			if (line.points2 == null) {
				var thisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
				for (int i = 0; i < segments*2; i++) {
					line.points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f));
					p += radians;
					i++;
					line.points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f));
				}
			}
			else {
				Vector2 v2Origin = origin;
				for (int i = 0; i < segments*2; i++) {
					line.points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
					p += radians;
					i++;
					line.points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
				}
			}
		}
	}

	public static void MakeCurveInLine (VectorLine line, Vector2[] curvePoints) {
		MakeCurveInLine (line, curvePoints, GetSegmentNumber(line), 0);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector2[] curvePoints, int segments) {
		MakeCurveInLine (line, curvePoints, segments, 0);
	}

	public static void MakeCurveInLine (VectorLine line, Vector2[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			LogError("Vector: MakeCurveInLine needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurveInLine (line, curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}

	public static void MakeCurveInLine (VectorLine line, Vector3[] curvePoints) {
		MakeCurveInLine (line, curvePoints, GetSegmentNumber(line), 0);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3[] curvePoints, int segments) {
		MakeCurveInLine (line, curvePoints, segments, 0);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			LogError("Vector: MakeCurveInLine needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurveInLine (line, curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}

	public static void MakeCurveInLine (VectorLine line, Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2) {
		MakeCurveInLine (line, anchor1, control1, anchor2, control2, GetSegmentNumber(line), 0);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments) {
		MakeCurveInLine (line, anchor1, control1, anchor2, control2, segments, 0);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments, int index) {
		if (!CheckArrayLength (line, FunctionName.MakeCurveInLine, segments, index)) {
			return;
		}
		
		if (line.continuousLine) {
			int end = line.isPoints? segments : segments+1;
			if (line.points2 != null) {
				for (int i = 0; i < end; i++) {
					line.points2[index+i] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)i/segments);
				}
			}
			else {
				for (int i = 0; i < end; i++) {
					line.points3[index+i] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)i/segments);
				}
			}
		}
		
		else {
			int idx = 0;
			if (line.points2 != null) {
				for (int i = 0; i < segments; i++) {
					line.points2[index + idx++] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)i/segments);
					line.points2[index + idx++] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)(i+1)/segments);
				}
			}
			else {
				for (int i = 0; i < segments; i++) {
					line.points3[index + idx++] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)i/segments);
					line.points3[index + idx++] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)(i+1)/segments);
				}
			}
		}
	}
	
	private static Vector2 GetBezierPoint (Vector2 anchor1, Vector2 control1, Vector2 anchor2, Vector2 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		
		return new Vector2( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
						    (ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y );
	}

	private static Vector3 GetBezierPoint3D (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		float cz = 3 * (control1.z - anchor1.z);
		float bz = 3 * (control2.z - control1.z) - cz;
		float az = anchor2.z - anchor1.z - cz - bz;
		
		return new Vector3( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
							(ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y,
							(az * (t*t*t)) + (bz * (t*t)) + (cz * t) + anchor1.z );
	}

	public static void MakeSplineInLine (VectorLine line, Vector2[] splinePoints) {
		MakeSplineInLine (line, splinePoints, null, GetSegmentNumber(line), 0, false);
	}

	public static void MakeSplineInLine (VectorLine line, Vector2[] splinePoints, bool loop) {
		MakeSplineInLine (line, splinePoints, null, GetSegmentNumber(line), 0, loop);
	}
	
	public static void MakeSplineInLine (VectorLine line, Vector2[] splinePoints, int segments) {
		MakeSplineInLine (line, splinePoints, null, segments, 0, false);
	}

	public static void MakeSplineInLine (VectorLine line, Vector2[] splinePoints, int segments, bool loop) {
		MakeSplineInLine (line, splinePoints, null, segments, 0, loop);
	}

	public static void MakeSplineInLine (VectorLine line, Vector2[] splinePoints, int segments, int index) {
		MakeSplineInLine (line, splinePoints, null, segments, index, false);
	}

	public static void MakeSplineInLine (VectorLine line, Vector2[] splinePoints, int segments, int index, bool loop) {
		MakeSplineInLine (line, splinePoints, null, segments, index, loop);
	}

	public static void MakeSplineInLine (VectorLine line, Vector3[] splinePoints) {
		MakeSplineInLine (line, null, splinePoints, GetSegmentNumber(line), 0, false);
	}

	public static void MakeSplineInLine (VectorLine line, Vector3[] splinePoints, bool loop) {
		MakeSplineInLine (line, null, splinePoints, GetSegmentNumber(line), 0, loop);
	}
	
	public static void MakeSplineInLine (VectorLine line, Vector3[] splinePoints, int segments) {
		MakeSplineInLine (line, null, splinePoints, segments, 0, false);
	}

	public static void MakeSplineInLine (VectorLine line, Vector3[] splinePoints, int segments, bool loop) {
		MakeSplineInLine (line, null, splinePoints, segments, 0, loop);
	}

	public static void MakeSplineInLine (VectorLine line, Vector3[] splinePoints, int segments, int index) {
		MakeSplineInLine (line, null, splinePoints, segments, index, false);
	}

	public static void MakeSplineInLine (VectorLine line, Vector3[] splinePoints, int segments, int index, bool loop) {
		MakeSplineInLine (line, null, splinePoints, segments, index, loop);
	}
		
	private static void MakeSplineInLine (VectorLine line, Vector2[] splinePoints2, Vector3[] splinePoints3, int segments, int index, bool loop) {
		var pointsLength = (splinePoints2 != null)? splinePoints2.Length : splinePoints3.Length;		
		if (pointsLength < 2) {
			LogError("Vector: MakeSplineInLine needs at least 2 spline points");
			return;
		}
		if (splinePoints2 != null && line.points2 == null) {
			LogError("Vector: MakeSplineInLine was called with a Vector3 spline points array but the line uses Vector2 points");
			return;
		}
		if (splinePoints3 != null && line.points3 == null) {
			LogError("Vector: MakeSplineInLine was called with a Vector2 spline points array but the line uses Vector3 points");
			return;
		}
		if (!CheckArrayLength (line, FunctionName.MakeSplineInLine, segments, index)) {
			return;
		}

		var pointCount = index;
		var numberOfPoints = loop? pointsLength : pointsLength-1;
		var add = 1.0f / segments * numberOfPoints;
		var start = 0.0f;
		var i = 0.0f;
		var j = 0;
		var p0 = 0;
		var p2 = 0;
		var p3 = 0;
		
		for (j = 0; j < numberOfPoints; j++) {
			p0 = j-1;
			p2 = j+1;
			p3 = j+2;
			if (p0 < 0) {
				p0 = loop? numberOfPoints-1 : 0;
			}
			if (loop && p2 > numberOfPoints-1) {
				p2 -= numberOfPoints;
			}
			if (p3 > numberOfPoints-1) {
				p3 = loop? p3-numberOfPoints : numberOfPoints;
			}
			if (line.continuousLine) {
				if (line.points2 != null) {
					for (i = start; i <= 1.0f; i += add) {
						line.points2[pointCount++] = GetSplinePoint (splinePoints2[p0], splinePoints2[j], splinePoints2[p2], splinePoints2[p3], i);
					}
				}
				else {
					for (i = start; i <= 1.0f; i += add) {
						line.points3[pointCount++] = GetSplinePoint3D (splinePoints3[p0], splinePoints3[j], splinePoints3[p2], splinePoints3[p3], i);
					}
				}
			}
			else {
				if (line.points2 != null) {
					for (i = start; i <= 1.0f; i += add) {
						line.points2[pointCount++] = GetSplinePoint (splinePoints2[p0], splinePoints2[j], splinePoints2[p2], splinePoints2[p3], i);
						if (pointCount > index+1 && pointCount < index + (segments*2)) {
							line.points2[pointCount++] = line.points2[pointCount-2];
						}
					}
				}
				else {
					for (i = start; i <= 1.0f; i += add) {
						line.points3[pointCount++] = GetSplinePoint3D (splinePoints3[p0], splinePoints3[j], splinePoints3[p2], splinePoints3[p3], i);
						if (pointCount > index+1 && pointCount < index + (segments*2)) {
							line.points3[pointCount++] = line.points3[pointCount-2];
						}
					}
				}
			}
			start = i - 1.0f;
		}
		// The last point might not get done depending on number of splinePoints and segments, so ensure that it's done here
		if ( (line.continuousLine && pointCount < index + (segments+1)) || (!line.continuousLine && pointCount < index + (segments*2)) ) {
			if (line.points2 != null) {
				line.points2[pointCount] = GetSplinePoint (splinePoints2[p0], splinePoints2[j-1], splinePoints2[p2], splinePoints2[p3], 1.0f);			
			}
			else {
				line.points3[pointCount] = GetSplinePoint3D (splinePoints3[p0], splinePoints3[j-1], splinePoints3[p2], splinePoints3[p3], 1.0f);		
			}
		}
	}

	private static Vector2 GetSplinePoint (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
		float t2 = t*t;
		float t3 = t2*t;
		return new Vector2 (0.5f * ((2.0f*p1.x) + (-p0.x + p2.x)*t + (2.0f*p0.x - 5.0f*p1.x + 4.0f*p2.x - p3.x)*t2 + (-p0.x + 3.0f*p1.x- 3.0f*p2.x + p3.x)*t3),
							0.5f * ((2.0f*p1.y) + (-p0.y + p2.y)*t + (2.0f*p0.y - 5.0f*p1.y + 4.0f*p2.y - p3.y)*t2 + (-p0.y + 3.0f*p1.y- 3.0f*p2.y + p3.y)*t3));
	}
	
	private static Vector3 GetSplinePoint3D (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		float t2 = t*t;
		float t3 = t2*t;
		return new Vector3 (0.5f * ((2.0f*p1.x) + (-p0.x + p2.x)*t + (2.0f*p0.x - 5.0f*p1.x + 4.0f*p2.x - p3.x)*t2 + (-p0.x + 3.0f*p1.x- 3.0f*p2.x + p3.x)*t3),
							0.5f * ((2.0f*p1.y) + (-p0.y + p2.y)*t + (2.0f*p0.y - 5.0f*p1.y + 4.0f*p2.y - p3.y)*t2 + (-p0.y + 3.0f*p1.y- 3.0f*p2.y + p3.y)*t3),
							0.5f * ((2.0f*p1.z) + (-p0.z + p2.z)*t + (2.0f*p0.z - 5.0f*p1.z + 4.0f*p2.z - p3.z)*t2 + (-p0.z + 3.0f*p1.z- 3.0f*p2.z + p3.z)*t3));
	}
		
	public static void MakeTextInLine (VectorLine line, string text, Vector3 startPos, float size) {
		MakeTextInLine (line, text, startPos, size, 1.0f, 1.5f, true);
	}
	
	public static void MakeTextInLine (VectorLine line, string text, Vector3 startPos, float size, bool uppercaseOnly) {
		MakeTextInLine (line, text, startPos, size, 1.0f, 1.5f, uppercaseOnly);
	}
	
	public static void MakeTextInLine (VectorLine line, string text, Vector3 startPos, float size, float charSpacing, float lineSpacing) {
		MakeTextInLine (line, text, startPos, size, charSpacing, lineSpacing, true);
	}
	
	public static void MakeTextInLine (VectorLine line, string text, Vector3 startPos, float size, float charSpacing, float lineSpacing, bool uppercaseOnly) {
		if (line.continuousLine) {
			LogError ("Vector: MakeTextInLine can only be used with a discrete line");
			return;
		}
		int pointsLength = GetPointsLength (line);
		int charPointsLength = 0;
		
		// Get total number of points needed for all characters in the string
		for (int i = 0; i < text.Length; i++) {
			int charNum = System.Convert.ToInt32(text[i]);
			if (charNum == 32 || charNum == 10) continue;
			if (VectorChar.data[charNum] == null) {
				LogError ("Vector.MakeTextInLine: no data found for character '" + text[i] + "'");
				return;
			}
			if (charNum < 0 || charNum > VectorChar.numberOfCharacters) {
				LogError ("Vector.MakeTextInLine: Character '" + text[i] + "' is not valid");
				return;
			}
			if (uppercaseOnly && charNum >= 97 && charNum <= 122) {
				charNum -= 32;
			}
			charPointsLength += VectorChar.data[charNum].Length;
		}
		if (charPointsLength > pointsLength) {
			line.Resize (charPointsLength);
		}
		else if (charPointsLength < pointsLength) {
			ZeroPointsInLine (line, charPointsLength);
		}
		
		float charPos = 0.0f;
		float linePos = 0.0f;
		int idx = 0;
		var scaleVector = new Vector2(size, size);
		var useVector2 = line.points3 == null;

		for (int i = 0; i < text.Length; i++) {
			int charNum = System.Convert.ToInt32(text[i]);
			// Newline
			if (charNum == 10) {
				linePos -= lineSpacing;
				charPos = 0.0f;
			}
			// Space
			else if (charNum == 32) {
				charPos += charSpacing;
			}
			// Character
			else {
				if (uppercaseOnly && charNum >= 97 && charNum <= 122) {
					charNum -= 32;
				}
				int end = VectorChar.data[charNum].Length;
				if (useVector2) {
					for (int j = 0; j < end; j++) {
						line.points2[idx++] = Vector2.Scale(VectorChar.data[charNum][j] + new Vector2(charPos, linePos), scaleVector) + (Vector2)startPos;
					}
				}
				else {
					for (int j = 0; j < end; j++) {
						line.points3[idx++] = Vector3.Scale((Vector3)VectorChar.data[charNum][j] + new Vector3(charPos, linePos, 0.0f), scaleVector) + startPos;
					}
				}
				charPos += charSpacing;
			}
		}
	}

	public static void ZeroPointsInLine (VectorLine line) {
		ZeroPointsInLine (line, 0);
	}

	public static void ZeroPointsInLine (VectorLine line, int index) {
		if (line.points2 != null) {
			if (index >= line.points2.Length) {
				LogError("Vector: index out of range for " + line.vectorObject.name + " when calling ZeroPointsInLine. Index: " + index + ", array length: " + line.points2.Length);
				return;
			}
			for (int i = index; i < line.points2.Length; i++) {
				line.points2[i] = new Vector2(0.0f, 0.0f);
			}
		}
		else {
			if (index >= line.points3.Length) {
				LogError("Vector: index out of range for " + line.vectorObject.name + " when calling ZeroPointsInLine. Index: " + index + ", array length: " + line.points3.Length);
				return;
			}
			for (int i = index; i < line.points3.Length; i++) {
				line.points3[i] = new Vector3(0.0f, 0.0f, 0.0f);
			}
		}
	}
	
	public static void Active (VectorLine line, bool active) {
		line.vectorObject.GetComponent<Renderer>().enabled = active;
		line.active = active;
	}
}

public struct Vector3Pair {
	public Vector3 p1;
	public Vector3 p2;
	public Vector3Pair (Vector3 point1, Vector3 point2) {
		p1 = point1;
		p2 = point2;
	}
}