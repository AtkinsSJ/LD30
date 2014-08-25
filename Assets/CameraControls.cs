using UnityEngine;
using System.Collections;

// TODO: Click-and-drag to pan
public class CameraControls : MonoBehaviour {

	public float panSpeed = 5;
	public float scrollSpeed = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// Zoom with the mouse wheel
		float zoom = camera.orthographicSize + Input.GetAxis("MouseScroll") * -scrollSpeed;
		if (zoom < 1) {
			zoom = 1;
		} else if (zoom > 15) {
			zoom = 15;
		}

		// Move with arrows/WASD
		float x = Input.GetAxis("Horizontal"),
			y = Input.GetAxis("Vertical");

		transform.position += new Vector3(x * Time.deltaTime * panSpeed, y * Time.deltaTime * panSpeed, 0);

		camera.orthographicSize = zoom;
		
		// Right-click and drag the camera
		if (Input.GetMouseButton(1)) {
			x = Input.GetAxis("MouseX");
			y = Input.GetAxis("MouseY");
			transform.position -= new Vector3(x, y, 0) * zoom * (3f/56f);
		}
	}
}
