using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vive_Camera_Projector : MonoBehaviour {

    WebCamTexture texture = new WebCamTexture();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        texture.deviceName = WebCamTexture.devices[0].name;
        texture.Play();
       GetComponent<MeshRenderer>().material.mainTexture = texture;
        //print(WebCamTexture.devices[0].name);
	}
}
