using UnityEngine;

public class SpriteController : MonoBehaviour {
	public float scale = 1.0f;
	public float hideDistance = 10.0f;


	public void Update () {
		transform.rotation = Camera.main.transform.rotation;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		Sprite sprite = spriteRenderer.sprite;

		Vector3 center = sprite.bounds.center;
		Vector3 minimum = sprite.bounds.min;

		Vector3 centerPosition = transform.TransformPoint(center);
		Vector3 minimumPosition = transform.TransformPoint(minimum);

		Vector3 screenCenterPosition = Camera.main.WorldToScreenPoint(centerPosition);
		Vector3 screenMinimumPosition = Camera.main.WorldToScreenPoint(minimumPosition);

		screenCenterPosition.z = 0.0f;
		screenMinimumPosition.z = 0.0f;

		float pixelDistance = Mathf.Sqrt(sprite.texture.height * sprite.texture.height / 4.0f + sprite.texture.width * sprite.texture.width / 4.0f);
		float screenDistance = Vector3.Distance(screenCenterPosition, screenMinimumPosition);

		float localScale = pixelDistance * scale / screenDistance;

		transform.localScale = new Vector3(localScale, localScale, localScale);

		spriteRenderer.enabled = Vector3.Distance(Camera.main.transform.position, transform.position) > hideDistance;
	}
}
