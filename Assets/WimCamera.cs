using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WimCamera : MonoBehaviour {

    public Transform wim;
    public Transform wimCamera;

    public Transform world; // the real world we want to move
    public Transform worldCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
            
        Vector3 delta = (wim.position - wimCamera.position);
        delta = wimCamera.InverseTransformVector(delta);

        Vector3 wimForward = wim.TransformDirection(Vector3.forward);
        wimForward = wimCamera.InverseTransformDirection(wimForward);

        world.position = worldCamera.position + worldCamera.TransformVector(delta);
        world.forward = wimForward;

	}
}
