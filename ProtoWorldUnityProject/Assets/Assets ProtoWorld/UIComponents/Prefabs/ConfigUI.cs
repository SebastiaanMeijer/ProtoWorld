using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigUI : MonoBehaviour
{

    private bool active = false;
    public List<GameObject> gameObjects;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
	        
	}


    public void Toggle()
    {
        active = !active;
        foreach (GameObject o in gameObjects)
        {
            if(o != null) o.SetActive(active);
        }
    }

}
