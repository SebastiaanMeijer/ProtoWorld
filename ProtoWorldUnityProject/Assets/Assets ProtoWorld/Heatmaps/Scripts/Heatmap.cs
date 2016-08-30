// Made by Alan Zucconi
// www.alanzucconi.com
// Modified by Furkan Sonmez

using UnityEngine;
using System.Collections;

public class Heatmap : MonoBehaviour
{

	public CameraControl cameraObject;

    public Vector3[] positions;
	public Vector2 properties;
    public float[] radiuses;
    public float[] intensities;
	public Transform[] pedestrians;

    public Material material;

    public int count = 5000;
	public int counted = 0;
	public static float HMIntensity = 0.1f;
	public static float HMRadius = 0.1f;
	public float maxRadius = 20f;
	public float refreshTime = 2f;
	public int minCameraHeight = 100;
	public float heightHM;

	public static bool activeHeatMaps = true; 
	public static bool activatedHM = false;
	public bool zoomedIn;

	public IEnumerator refreshCoroutine;


	/// <summary>
	/// Awake method.
	/// </summary>
	void Awake()
	{
		cameraObject = FindObjectOfType<CameraControl>();




	}


	/// <summary>
	/// Start method.
	/// </summary>
    void Start ()
    {
       
		positions = new Vector3[count];
		pedestrians = new Transform[count];
		material.SetInt("_Points_Length", count);
		for (int i = 0; i < count; i++)
		{
			positions [i] = new Vector3 (0, -1000, 0);
			material.SetVector("_Points" + i.ToString(), positions[i]);
			properties = new Vector2(HMRadius, HMIntensity); // NEW
			material.SetVector("_Properties" + i.ToString(), properties);
		}
		refreshCoroutine = heatmapRefresh ();
		StartCoroutine(refreshCoroutine);
    }

	/// <summary>
	/// Put the following information about the pedestrian into the array of the heatmap method
	/// </summary>
	public void putInArray(float posX, float posY, float posZ, Transform AnObject)
	{

		if (counted > count -1) {
			counted = 0;
		}

		pedestrians [counted] = AnObject;

		counted = counted + 1;

	}


	/// <summary>
	/// Change intensity when slider is moved
	/// </summary>
	public static void changeParameterIntensityHM(float intensity){
		Heatmap.HMIntensity = intensity/50;
	}

	/// <summary>
	/// Change radius when slider is moved
	/// </summary>
	public static void changeParameterRadiusHM(float radius){
		Heatmap.HMRadius = radius/3;
	}
		
	/// <summary>
	/// Update method.
	/// </summary>
	public void Update(){

		if (activatedHM == true) {
			if (activeHeatMaps == false) {
				material.SetInt ("_Points_Length", count);
				for (int i = 0; i < count; i++) {
					positions [i] = new Vector3 (0, -1000, 0);
					material.SetVector ("_Points" + i.ToString (), positions [i]);
					properties = new Vector2 (HMRadius, HMIntensity);
					material.SetVector ("_Properties" + i.ToString (), properties);
				}
			}
			activatedHM = false;
		} 

		if (Input.GetKeyUp (KeyCode.H)) {
			activateDeactivateHM ();
		}

	}


	/// <summary>
	/// Activate or deactivate the heatmap when button is pressed
	/// </summary>
	public void activateDeactivateHM(){
		activatedHM = true;
		if (activeHeatMaps == true) {
			activeHeatMaps = false;

		} else if (activeHeatMaps == false) {
			activeHeatMaps = true;
		}

	}

	/// <summary>
	/// Refresh the heatmap every refreshTime seconds
	/// </summary>
	public IEnumerator heatmapRefresh(){
		
			yield return new WaitForSeconds (refreshTime);
		if (activeHeatMaps) {
			properties.x = HMRadius;
			properties.y = HMIntensity;
				
			transform.position = new Vector3(transform.position.x, heightHM, transform.position.z);
			if (cameraObject.targetCameraPosition.y > minCameraHeight) {
				zoomedIn = false;
				for (int i = 0; i < counted; i++) {
					positions [i] = new Vector3 (pedestrians [i].transform.position.x, heightHM, pedestrians [i].transform.position.z);
					if (pedestrians [i].gameObject.activeSelf == false)
						positions [i] = new Vector3 (0, -1000, 0);
					material.SetVector ("_Points" + i.ToString (), positions [i]);
					material.SetVector ("_Properties" + i.ToString (), properties);
				}
			}
			else if(zoomedIn == false){
				for (int i = 0; i < count; i++) {
					positions [i] = new Vector3 (0, -1000, 0);
					material.SetVector ("_Points" + i.ToString (), positions [i]);
					properties = new Vector2 (HMRadius, HMIntensity);
					material.SetVector ("_Properties" + i.ToString (), properties);
				}
				zoomedIn = true;
			}

		}
			StartCoroutine(heatmapRefresh());

	}

	/// <summary>
	/// Reset the heatmap after the game ends
	/// </summary>
	public void OnDestroy(){
		for (int i = 0; i < count; i++) {
			positions [i] = new Vector3 (0, -1000, 0);
			material.SetVector ("_Points" + i.ToString (), positions [i]);
			properties = new Vector2 (HMRadius, HMIntensity);
			material.SetVector ("_Properties" + i.ToString (), properties);
		}
	}
    
}