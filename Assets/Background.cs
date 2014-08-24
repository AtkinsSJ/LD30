using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public float sizeRatio = 250f;

	private Camera mainCamera;

	// Use this for initialization
	void Start () {
		mainCamera = transform.GetComponentInParent<Camera>();
	}
	
	// Called after everything else!
	void LateUpdate () {
		float hSize = mainCamera.pixelWidth / sizeRatio,
			vSize = mainCamera.pixelHeight / sizeRatio;
		float scale = Mathf.Max(hSize, vSize);

		transform.localScale = new Vector3(scale, scale, 1) * mainCamera.orthographicSize;
	}
}
