using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelRotate : MonoBehaviour {
    Camera main;
	// Use this for initialization
	void Start () {
		main = Camera.main;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.LookAt(main.transform.position, Vector3.up);
	}
}
