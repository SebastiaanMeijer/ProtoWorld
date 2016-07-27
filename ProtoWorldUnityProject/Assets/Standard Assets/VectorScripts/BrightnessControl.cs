using UnityEngine;

[AddComponentMenu("Vectrosity/BrightnessControl")]
public class BrightnessControl : MonoBehaviour {

	RefInt m_objectNumber;
	VectorLine vectorLine;
	bool useLine = false;	// Normally false, since Visibility scripts take care of this
	bool destroyed = false;
	
	public RefInt objectNumber {
		get {return m_objectNumber;}
	}

	public void Setup (VectorLine line, bool useLine) {
		if (line.lineColors == null) {
			Debug.LogError ("In order to use Brightness.Fog, the line \"" + line.vectorObject.name + "\" must contain segment colors");
			return;
		}
		m_objectNumber = new RefInt(0);
		VectorManager.CheckDistanceSetup (transform, line, line.lineColors[0], m_objectNumber);
		VectorManager.SetDistanceColor (m_objectNumber.i);
		if (useLine) {	// Only if there are no Visibility scripts being used
			this.useLine = true;
			vectorLine = line;
		}
	}
	
	public void SetUseLine (bool useLine) {
		this.useLine = useLine;
	}
	
	// Force the color to be set when becoming visible
	void OnBecameVisible () {
		VectorManager.SetOldDistance (m_objectNumber.i, -1);
		VectorManager.SetDistanceColor (m_objectNumber.i);
		if (!useLine) return;
		Vector.Active (vectorLine, true);
	}
	
	public void OnBecameInvisible () {
		if (!useLine) return;
		Vector.Active (vectorLine, false);
	}
	
	void OnDestroy () {
		if (destroyed) return;	// Paranoia check
		destroyed = true;
		VectorManager.DistanceRemove (m_objectNumber.i);
		if (useLine) {
			Vector.DestroyLine (ref vectorLine);
		}
	}
}