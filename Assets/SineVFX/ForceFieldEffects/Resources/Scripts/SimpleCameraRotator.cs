using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraRotator : MonoBehaviour {

    public float rotationSpeed = -15f;
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(0f, Time.deltaTime * rotationSpeed, 0f);
    }
}
