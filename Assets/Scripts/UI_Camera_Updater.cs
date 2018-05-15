using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Camera_Updater : MonoBehaviour {

	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
	}
}
