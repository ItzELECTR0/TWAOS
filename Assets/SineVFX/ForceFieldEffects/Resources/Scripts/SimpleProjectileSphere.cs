using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectileSphere : MonoBehaviour {

    public float force;

    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);
        Destroy(gameObject, 2.5f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
