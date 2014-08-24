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
		float x = Input.GetAxis("Horizontal"),
			y = Input.GetAxis("Vertical");

		transform.position += new Vector3(x * Time.deltaTime * panSpeed, y * Time.deltaTime * panSpeed, 0);

		float zoom = camera.orthographicSize + Input.GetAxis("MouseScroll") * -scrollSpeed;
		if (zoom < 1) {
			zoom = 1;
		} else if (zoom > 15) {
			zoom = 15;
		}

		camera.orthographicSize = zoom;
		
	}
}
