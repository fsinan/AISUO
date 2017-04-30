using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {

	private LineRenderer lineRenderer;
	private GameObject weaponBarrel;
	private Light barrelLight;

	public Image crossHair;
	public float timeBetweenShots = 0.5f;
	public float effectTimeProportion = 0.2f;
	public float shootDistance = 100.0f;
	public int shotDamage = 34;

	private float shootTimer = 0f;

	// Use this for initialization
	void Start () 
	{
		lineRenderer = GetComponent<LineRenderer> ();
		weaponBarrel = GameObject.Find ("barrel");
		barrelLight = weaponBarrel.GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (!GameManager.isGamePaused)
        {
            // Increase timer
            shootTimer += Time.deltaTime;

            // If enough time passed since the last shot
            if (Input.GetButton("Fire1") && shootTimer >= timeBetweenShots)
            {
                Shoot();
            }

            // If it is time to end the shooting effects
            if (shootTimer >= timeBetweenShots * effectTimeProportion)
            {
                DisableEffects();
            }
        }
	}

	protected void SetRayLine(Vector3 start, Vector3 end)
	{
		Vector3[] positions = new Vector3[2];

		positions [0] = start;
		positions [1] = end;

		lineRenderer.SetPositions (positions);
	}

	protected void DisableEffects()
	{
		lineRenderer.enabled = false;
		barrelLight.enabled = false;
	}

	protected void Shoot()
	{
		// Reset the timer
		shootTimer = 0f;

		// Play sound

		// Crosshair position relative to the anchor
		Vector3 crosshairPosition = new Vector3(crossHair.rectTransform.anchoredPosition.x * crossHair.canvas.scaleFactor + Screen.width/2,
												crossHair.rectTransform.anchoredPosition.y * crossHair.canvas.scaleFactor + Screen.height/2, 0);

		Ray ray = Camera.main.ScreenPointToRay (crosshairPosition);

		int layerMask = 1 << 8;
		layerMask = ~layerMask;

		RaycastHit targetHit;
		Physics.Raycast (ray, out targetHit, shootDistance, layerMask);

		// Ray casting to see if we hit something
		RaycastHit hit;
		ray = new Ray (weaponBarrel.transform.position, targetHit.point - weaponBarrel.transform.position);

		if (Physics.Raycast (ray, out hit, shootDistance)) 
		{
			// Draw a line from the barrel to the impact point
			// And some light effect
			SetRayLine (weaponBarrel.transform.position, hit.point);
			lineRenderer.enabled = true;
			barrelLight.enabled = true;

			// If it's an enemy, do things like damaging
			GameObject hitObject = hit.transform.gameObject;

			if (hitObject.tag == "Enemy") 
			{
				// Particle effects, maybe?

				// Let's make some damage
				hitObject.GetComponent<EnemyBehavior>().TakeDamage(shotDamage);
			} 
			else 
			{
				// We may add some effect to show the impact
			}
		}
		else 
		{
			// There is no hit, just draw the line from the barrel to far away
			// And the light effect again
			SetRayLine (weaponBarrel.transform.position, weaponBarrel.transform.position + transform.forward * shootDistance);
			lineRenderer.enabled = true;
			barrelLight.enabled = true;
		}
	}
}
