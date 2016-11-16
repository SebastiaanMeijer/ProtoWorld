using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ZoomScrollbarMMV : MonoBehaviour {

	public Scrollbar scrollbar;
	public GameObject indicatorImage;
	public Text handleText;
	public RectTransform rt;

	public int height;
	public int lastHeight;
	public float maxHeight;
	public float value;

	public int[] stagesHeights;
	public string[] displayText;
	public static int level;

	public bool fading = false;

	// Use this for initialization
	void Awake () {
	
		scrollbar = this.gameObject.GetComponent<Scrollbar> ();
		maxHeight = Camera.main.GetComponent<CameraControl> ().maxHeight;
        handleText = GameObject.Find("HandleTextMMV").GetComponent<Text>();
        rt = this.GetComponent (typeof(RectTransform)) as RectTransform;
	}

	void Start(){
        

        for (int i = 0; i < stagesHeights.Length; i++) {

			GameObject instantiatedIndicator = Instantiate (indicatorImage, new Vector3 (this.transform.position.x, (stagesHeights[i]/maxHeight)*rt.sizeDelta.y + transform.position.y - rt.sizeDelta.y/2, this.transform.position.z), Quaternion.identity) as GameObject;
			instantiatedIndicator.transform.parent = this.transform;
		}
		StartCoroutine (changeTag (0.1f));
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void sliderChanged(){
		value = scrollbar.value;
		Camera.main.GetComponent<CameraControl> ().targetCameraPosition = new Vector3 (Camera.main.transform.position.x, value*maxHeight, Camera.main.transform.position.z);

	}

	public void zoomedInOut(float heightOfCamera){
		height = (int)heightOfCamera;
		scrollbar.value = height / maxHeight;

		if (fading == false && lastHeight != height) {

			for (int i = 0; i < stagesHeights.Length; i++) {
				if (value * maxHeight > stagesHeights [i]) {
					level = i;
					ZoomScrollbarMMV.level = level;

					handleText.text = "" + stagesHeights [i];

				}
			}
				
			fading = true;
			handleText.color = new Vector4 (handleText.color.r, handleText.color.g, handleText.color.b, 1);

			StartCoroutine (showTag (1f));
			//StartCoroutine (changeTag (0.1f));
		}
	}

	public IEnumerator showTag(float timeOfFade){
			yield return new WaitForSeconds (timeOfFade);
			handleText.color = new Vector4 (handleText.color.r, handleText.color.g, handleText.color.b, 0);
		fading = false;
		lastHeight = height;
			yield return null;
	}

	public IEnumerator changeTag(float timeOfFade){
		yield return new WaitForSeconds (timeOfFade);
		if (lastHeight != height) {
			for (int i = 0; i < stagesHeights.Length; i++) {
				if (value * maxHeight > stagesHeights [i]) {
					level = i;
					ZoomScrollbarMMV.level = level;

					handleText.text = "" + displayText [i];

				}
			}
			lastHeight = height;
		}
		StartCoroutine (changeTag (0.1f));

	}
		
}