// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;

[AddComponentMenu("Vectrosity/VisibilityControl")]
public class VisibilityControl : MonoBehaviour {

	RefInt m_objectNumber;
	VectorLine vectorLine;
	bool destroyed = false;
	
	public RefInt objectNumber {
		get {return m_objectNumber;}
	}
	
	public void Setup (VectorLine line, bool makeBounds) {
		if (makeBounds) {
			VectorManager.SetupBoundsMesh (gameObject, line);
		}
		
		VectorManager.VisibilitySetup (transform, line, out m_objectNumber);
		vectorLine = line;
	}

	void OnBecameVisible () {
		Vector.Active (vectorLine, true);
		
		// Draw line now, otherwise's there's a 1-frame delay before the line is actually drawn in the next LateUpdate
		VectorManager.DrawArrayLine2 (m_objectNumber.i);
	}
	
	void OnBecameInvisible () {
		Vector.Active (vectorLine, false);
	}
	
	void OnDestroy () {
		if (destroyed) return;	// Paranoia check
		destroyed = true;
		VectorManager.VisibilityRemove (m_objectNumber.i);
		Vector.DestroyLine (ref vectorLine);
	}
}