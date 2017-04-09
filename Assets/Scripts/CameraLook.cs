using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour {

	public GameObject target;
	public float damping = 1.0f;

	private float mouseSensitivity;

	private Vector3 offset;

	// Use this for initialization
	void Start () 
	{
		Cursor.visible = false;
		
		offset = transform.position - target.transform.position;
		mouseSensitivity = target.GetComponent<PlayerMovement> ().mouseSensitivity;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		// Rotate around the character
		transform.RotateAround (Vector3.zero, Vector3.up, Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime);

		// Position behind the character
		transform.position = target.transform.position - target.transform.forward * offset.magnitude + Vector3.up * 3f;

		// Now. finally, look at the character
		transform.LookAt(target.transform.position + Vector3.up * 2f);
	}
}
