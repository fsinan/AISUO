using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacing : MonoBehaviour {

	public Camera lookCamera;

	// Use this for initialization
	void Start () 
	{
        lookCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(transform.position + lookCamera.transform.rotation * Vector3.forward,
			lookCamera.transform.rotation * Vector3.up);
	}
}
