using UnityEngine;

public class TextController : MonoBehaviour {
	public float scale = 1.0f;
	public float hideDistance = 10.0f;

	public float height = 0.1f;


	public void Update () {
		transform.rotation = Camera.main.transform.rotation;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		
		Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 minimum = new Vector3(-height / 2.0f, -height / 2.0f, 0.0f);

		Vector3 centerPosition = transform.TransformPoint(center);
		Vector3 minimumPosition = transform.TransformPoint(minimum);

		Vector3 screenCenterPosition = Camera.main.WorldToScreenPoint(centerPosition);
		Vector3 screenMinimumPosition = Camera.main.WorldToScreenPoint(minimumPosition);

		screenCenterPosition.z = 0.0f;
		screenMinimumPosition.z = 0.0f;
		
		float screenDistance = Vector3.Distance(screenCenterPosition, screenMinimumPosition);

		float localScale = scale / screenDistance;

		transform.localScale = new Vector3(localScale, localScale, localScale);

		meshRenderer.enabled = Vector3.Distance(Camera.main.transform.position, transform.position) > hideDistance;
	}
}
