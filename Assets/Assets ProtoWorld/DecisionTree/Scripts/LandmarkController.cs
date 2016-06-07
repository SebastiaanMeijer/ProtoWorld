using UnityEngine;
using System.Collections;

public class LandmarkController : MonoBehaviour 
{
    public int landmarkId;
    public string landmarkTag;

	void Awake () 
    {
        this.transform.FindChild("Tag").GetComponent<TextMesh>().text = landmarkTag;
	}
}
