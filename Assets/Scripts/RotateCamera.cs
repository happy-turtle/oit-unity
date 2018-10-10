using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour {

    private Vector3 lookAt = new Vector3(0.0f, 12.0f, 0.0f);
    public float speed = 10.0f;
    
	// Update is called once per frame
	void Update () {
        transform.RotateAround(lookAt, transform.up, speed * Time.deltaTime);
        transform.LookAt(lookAt);
	}
}
