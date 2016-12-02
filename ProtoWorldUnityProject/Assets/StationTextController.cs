using UnityEngine;
using System.Collections;

public class StationTextController : MonoBehaviour {

    public int fontSizeFactor = 5;
    public TextMesh textMesh;
    public MeshRenderer meshRenderer;
    public CameraControl camera;
    public Vector3 currentTargetCameraPosition;
    public Quaternion currentCameraRotation;

	// Use this for initialization
	void Start () {
        //get station text
        foreach (Transform child in transform)
        {
            if (child.GetComponent<TextMesh>() != null)
            {
                textMesh = child.GetComponent<TextMesh>();
                meshRenderer = child.GetComponent<MeshRenderer>();
            }
        }

        camera = Camera.main.GetComponent<CameraControl>();

        currentTargetCameraPosition = camera.targetCameraPosition;
        currentCameraRotation = Camera.main.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (cameraChanged() && textMesh != null)
        {
            //changefontsizeherehereherehere
            textMesh.fontSize = calculateFontSize();
            textMesh.transform.rotation = calculateFontRotation();
            if (removeLabel() && meshRenderer.enabled)
            {
                meshRenderer.enabled = false;
            }
            else if (!removeLabel() && !meshRenderer.enabled)
            {
                meshRenderer.enabled = true;
            }
        }
	}

    //checks if the camera has changed position, if yes set current cameraposition
    bool cameraChanged()
    {
        if ((camera.targetCameraPosition != currentTargetCameraPosition )|| (currentCameraRotation != Camera.main.transform.rotation))
        {
            currentTargetCameraPosition = camera.targetCameraPosition;
            currentCameraRotation = Camera.main.transform.rotation;
            return true;
        }
        else
        {
            return false;
        }
    }

    //check wether its better to remove the title
    bool removeLabel()
    {
        if (textMesh.transform.position.y + 100 > currentTargetCameraPosition.y)
        {
            return true;
        }
        return false;
    }

    //calculates the font rotation so it rotates with the camera
    Quaternion calculateFontRotation()
    {
        //Quaternion fontRotation = new Quaternion();
        //fontRotation = Camera.main.transform.rotation;
        return Camera.main.transform.rotation;
    }

    //calculates the font size needed with the current camera distance (uses the factor declared above)
    int calculateFontSize()
    {
        return (int)Mathf.Ceil(camera.targetCameraPosition.y / 100 * fontSizeFactor);
    }

    
}
