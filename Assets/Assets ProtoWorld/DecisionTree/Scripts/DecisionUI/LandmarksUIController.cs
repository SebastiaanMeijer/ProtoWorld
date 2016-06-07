using UnityEngine;
using System.Collections;

public class LandmarksUIController : MonoBehaviour 
{
    public float zoomFactor = 0.5f;
    private LandmarkController[] landmarks;
    private RectTransform destCanvas;
    public GameObject templateButton;

	void Awake () 
    {
        destCanvas = this.GetComponent<RectTransform>();
        landmarks = FindObjectsOfType<LandmarkController>();
        
        // Adapt the size of the panel
        destCanvas.sizeDelta = new Vector2(destCanvas.sizeDelta.x, destCanvas.sizeDelta.y + destCanvas.sizeDelta.y * landmarks.Length + 10f);

        // Create a button for each landmark
        for (int i = 0; i < landmarks.Length; i++)
        {
            // Instantiate a new button
            GameObject landmarkButton = Instantiate(templateButton) as GameObject;

            // Get the needed components of the button object
            RectTransform canvasBox = landmarkButton.GetComponent<RectTransform>();
            UnityEngine.UI.Button button = landmarkButton.transform.FindChild("Button").GetComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.Text buttonText = button.transform.FindChild("Text").GetComponent<UnityEngine.UI.Text>();
            LandmarkButtonController buttonController = landmarkButton.GetComponent<LandmarkButtonController>();

            // Position the button according to its parent object
            landmarkButton.transform.SetParent(this.transform, false);
            canvasBox.localPosition = new Vector3(0, -canvasBox.sizeDelta.y * (i + 1), 0);

            // Change the destination text
            buttonText.text = landmarks[i].landmarkTag;

            // Set the gameobject reference and the zoomfactor
            buttonController.landmarkObject = landmarks[i].gameObject;
            buttonController.zoomFactor = zoomFactor;

            //Set the slider active
            landmarkButton.SetActive(true);
        }
	}
}
