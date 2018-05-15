using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0.1f, 0);
        float mw = Input.GetAxis("Mouse ScrollWheel");
        if(mw > 0 && Camera.main.fieldOfView < 120) {
            transform.GetChild(0).Translate(0,0,5 * Time.deltaTime);
        }
        if(mw < 0 && Camera.main.fieldOfView > 5) {
            try {
                transform.GetChild(0).Translate(0,0,-5 * Time.deltaTime);
            } catch (Exception e) { }
        }
        if(Input.GetAxis("Vertical") > 0.1f) {
            transform.Translate(0,1 * Time.deltaTime,0);
        }
        if(Input.GetAxis("Vertical") < -0.1f) {
            transform.Translate(0,-1 * Time.deltaTime,0);
        }
        if(Input.GetAxis("Horizontal") > 0.1f) {
            transform.Translate(1 * Time.deltaTime,0,0);
        }
        if(Input.GetAxis("Horizontal") < -0.1f) {
            transform.Translate(-1 * Time.deltaTime,0,0);
        }
	}
}
