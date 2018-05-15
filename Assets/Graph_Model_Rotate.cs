using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph_Model_Rotate : MonoBehaviour {
    public Vector3 rotate;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotate.x, rotate.y*Time.deltaTime, rotate.z);
	}
}
