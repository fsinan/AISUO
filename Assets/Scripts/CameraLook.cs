using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour {

	public GameObject target;
	public GameObject lookTarget;
	public float damping = 1.0f;
	public float tolerance = 0.2f;

	private Vector3 offset;
	public float mouseSensitivity = 2f;
    float totalVertical;

    public float lowVerticalLimit = -50f;
    public float hiVerticalLimit = 60f;

	// Use this for initialization
	void Start () 
	{
		offset = transform.position - lookTarget.transform.position;
        totalVertical = 0f;
	}

	void LateUpdate () 
	{
        if (!GameManager.isGamePaused)
        {
            float horizontal = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 100;
            float vertical = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * 100;

            if(!(totalVertical + vertical >= lowVerticalLimit && totalVertical + vertical <= hiVerticalLimit))
            {
                vertical = 0f;
            }

            totalVertical += vertical;

            target.transform.Rotate(0, horizontal, 0);
            lookTarget.transform.Rotate(vertical, 0, 0);

            float desiredAngle_X = lookTarget.transform.eulerAngles.x;
            float desiredAngle_Y = lookTarget.transform.eulerAngles.y;

            Quaternion rotation = Quaternion.Euler(-desiredAngle_X, desiredAngle_Y, 0);

            Vector3 expectedPosition = lookTarget.transform.position + (rotation * offset);

            // Check if it is going through something
            Ray ray = new Ray(lookTarget.transform.position, (expectedPosition - lookTarget.transform.position).normalized);
            RaycastHit hitInfo;
            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hitInfo, offset.magnitude, layerMask))
            {
                // Position between the character and the object
                transform.position = hitInfo.point + (lookTarget.transform.position - expectedPosition).normalized * tolerance;
            }
            else
            {
                // Position behind the character
                transform.position = expectedPosition;
            }

            // Now. finally, look at the character
            transform.LookAt(lookTarget.transform.position);
        }
	}
}
