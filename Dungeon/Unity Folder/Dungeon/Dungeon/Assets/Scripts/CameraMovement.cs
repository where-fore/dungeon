using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	[SerializeField]
	private Vector3 defaultCameraPosition;

	[SerializeField]
	private float defaultOrthographicSize;


	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void moveToDefaultPosition() {

		transform.position = defaultCameraPosition;
		gameObject.GetComponent<Camera>().orthographicSize = defaultOrthographicSize;

	}

	public void zoomTo(Vector3 target) {

		// Create a move target that is on the default z plane.
		Vector3 cameraTarget = new Vector3(target.x, target.y, defaultCameraPosition.z);
		transform.position = cameraTarget;

		gameObject.GetComponent<Camera>().orthographicSize = 2f;

	}
}
