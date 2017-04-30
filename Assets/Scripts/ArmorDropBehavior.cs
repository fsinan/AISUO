using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorDropBehavior : MonoBehaviour {

    public float revSpeed = 45f;
    public int armorAmount = 50;
    public float lifeSpan = 5f;

    private float lifeTime;

	// Use this for initialization
	void Start ()
    {
        lifeTime = 0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.up, revSpeed * Time.deltaTime);

        lifeTime += Time.deltaTime;

        if(lifeTime >= lifeSpan)
        {
            Destroy(gameObject);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerHealth>().armor < other.gameObject.GetComponent<PlayerHealth>().maxArmor)
        {
            other.gameObject.GetComponent<PlayerHealth>().AddArmor(armorAmount);

            Destroy(gameObject);
        }
    }
}
