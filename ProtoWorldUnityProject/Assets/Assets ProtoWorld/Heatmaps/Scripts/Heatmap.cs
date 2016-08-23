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
	public float intensity = 0.1f;
	public float radius = 0.1f;
	public float maxRadius = 20f;
	public float refreshTime = 2f;
	public int minCameraHeight = 100;

	public bool activeHeatMaps = true; 

	public IEnumerator refreshCoroutine;

	void Awake()
	{
		cameraObject = FindObjectOfType<CameraControl>();

		positions = new Vector3[count];
		//radiuses = new float[count];
		// intensities= new float[count];
		pedestrians = new Transform[count];
		material.SetInt("_Points_Length", count);
		for (int i = 0; i < count; i++)
		{
			positions [i] = new Vector3 (0, -1000, 0);
			//positions[i] += new Vector3(Random.Range(-0.1f,+0.1f), Random.Range(-0.1f, +0.1f)) * Time.deltaTime ;
			material.SetVector("_Points" + i.ToString(), positions[i]);

			// Vector2 properties = new Vector2(radiuses[i], intensities[i]); // OLD
			properties = new Vector2(radius, intensity); // NEW
			material.SetVector("_Properties" + i.ToString(), properties);
		}


	}

    void Start ()
    {
       

        //for (int i = 0; i < positions.Length; i++)
        //{
        //    positions[i] = new Vector2(Random.Range(-0.4f, +0.4f), Random.Range(-0.4f, +0.4f));
        //    radiuses[i] = Random.Range(0f, 0.25f);
       //     intensities[i] = Random.Range(-0.25f, 1f);
        //}
		refreshCoroutine = heatmapRefresh ();
		StartCoroutine(refreshCoroutine);
    }

	public void putInArray(float posX, float posY, float posZ, Transform AnObject)
	{

		if (counted > count -1) {
			counted = 0;
		}

		positions[counted] = new Vector3(posX, this.transform.position.y, posZ);
		//radiuses[counted] = radius;
		//intensities[counted] = intensity;
		pedestrians [counted] = AnObject;

		counted = counted + 1;

	}

	/*
    void FixedUpdate()
    {
		
        material.SetInt("_Points_Length", positions.Length);
        for (int i = 0; i < counted; i++)
        {
			positions [i] = new Vector3 (pedestrians [i].transform.position.x, this.transform.position.y, pedestrians [i].transform.position.z);

            //positions[i] += new Vector3(Random.Range(-0.1f,+0.1f), Random.Range(-0.1f, +0.1f)) * Time.deltaTime ;
            material.SetVector("_Points" + i.ToString(), positions[i]);

           // Vector2 properties = new Vector2(radiuses[i], intensities[i]); // OLD
			Vector2 properties = new Vector2(radius, intensity); // NEW
            material.SetVector("_Properties" + i.ToString(), properties);
        }
        
    }
	*/

	public void Update(){

		if (Input.GetKeyUp (KeyCode.H) && activeHeatMaps == true) {
			activeHeatMaps = false;

			material.SetInt ("_Points_Length", count);
			for (int i = 0; i < count; i++) {
				positions [i] = new Vector3 (0, -1000, 0);
				//positions[i] += new Vector3(Random.Range(-0.1f,+0.1f), Random.Range(-0.1f, +0.1f)) * Time.deltaTime ;
				material.SetVector ("_Points" + i.ToString (), positions [i]);

				// Vector2 properties = new Vector2(radiuses[i], intensities[i]); // OLD
				properties = new Vector2 (radius, intensity); // NEW
				material.SetVector ("_Properties" + i.ToString (), properties);
			}


		} else if (Input.GetKeyUp (KeyCode.H) && activeHeatMaps == false) {
			activeHeatMaps = true;
		}

	}



	public IEnumerator heatmapRefresh(){
		
			yield return new WaitForSeconds (refreshTime);
			//material.SetInt("_Points_Length", count);
		if (activeHeatMaps) {
			properties.x = cameraObject.targetCameraPosition.y/10;
			if (properties.x > maxRadius)
				properties.x = maxRadius;
			
			transform.position = new Vector3(transform.position.x, properties.x, transform.position.z);
			if (cameraObject.targetCameraPosition.y > minCameraHeight)
				for (int i = 0; i < counted; i++) {
					positions [i] = new Vector3 (pedestrians [i].transform.position.x, this.transform.position.y*2 - 10, pedestrians [i].transform.position.z);
					if(pedestrians[i].gameObject.activeSelf == false) 
					positions [i] = new Vector3 (0, -1000, 0);
					//positions[i] += new Vector3(Random.Range(-0.1f,+0.1f), Random.Range(-0.1f, +0.1f)) * Time.deltaTime ;
					material.SetVector ("_Points" + i.ToString (), positions [i]);

					// Vector2 properties = new Vector2(radiuses[i], intensities[i]); // OLD
					//Vector2 properties = new Vector2(radius, intensity); // NEW
					material.SetVector ("_Properties" + i.ToString (), properties);
				}
			else {
				for (int i = 0; i < count; i++) {
					positions [i] = new Vector3 (0, this.transform.position.y, 0);
					//positions[i] += new Vector3(Random.Range(-0.1f,+0.1f), Random.Range(-0.1f, +0.1f)) * Time.deltaTime ;
					material.SetVector ("_Points" + i.ToString (), positions [i]);

					// Vector2 properties = new Vector2(radiuses[i], intensities[i]); // OLD
					properties = new Vector2 (radius, intensity); // NEW
					material.SetVector ("_Properties" + i.ToString (), properties);
				}
			}

		}
			StartCoroutine(heatmapRefresh());

	}
    
}