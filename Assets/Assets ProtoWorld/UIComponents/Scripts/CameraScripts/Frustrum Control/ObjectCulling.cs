using UnityEngine;
using System.Collections;

/// <summary>
/// Class for culling building objects when camera distance to the ground is more than MaximumHeight.
/// <para><strong>Note:</strong>The buildings must belong to the layer "Buildings"</para>
/// </summary>
public class ObjectCulling : MonoBehaviour {

	private Camera cam;
	/// <summary>
	/// Gets or sets MaximumHeight. If the camera goes higher than this value, the objects will be culled.
	/// </summary>
	public float MaximumHeight=250;
	// Use this for initialization
	void Start () {
		cam= Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		if (cam.transform.position.y >MaximumHeight)
			HideBuildings();
		else ShowBuildings();
	}
	
	private void ShowBuildings() {
		cam.cullingMask |= 1 << LayerMask.NameToLayer("Buildings");
	}
	 
	// Turn off the bit using an AND operation with the complement of the shifted int:
	private void HideBuildings() {
		cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Buildings"));
	}
	
	// Toggle the bit using a XOR operation:
	private void Toggle() {
		GetComponent<Camera>().cullingMask ^= 1 << LayerMask.NameToLayer("Buildings");
	}
}
