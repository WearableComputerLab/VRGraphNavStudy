using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyEdges : MonoBehaviour {

    private Transform a,b;
    private LineRenderer lines;
    public bool destroy = false;
    float t = 10;
	// Use this for initialization
	void Start () {
		lines = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(a != null && b != null) {
            lines.SetPosition(0,a.position);
            lines.SetPosition(1,b.position);
        }
        if(destroy == true) {
            Destroy(gameObject);
        }
        t -= Time.deltaTime;
        if(t <= 0) {
            destroy = true;
        }
	}

    public void setEdge(Transform a, Transform b) {
        this.a = a;
        this.b = b;
    }

    public void destroyEdge() {
        Destroy(gameObject);
    }
}
