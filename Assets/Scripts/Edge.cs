using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge {

    private Transform a;
	private Transform b;
	private Color c;
	private Transform transformRef;

	public Edge(Transform a, Transform b){
		this.a = a;
		this.b = b;
	}

	public void setEdge(Transform a, Transform b) {
        this.a = a;
        this.b = b;     
    }

	public Vector3 getA(){
		return a.position;
	}

	public Vector3 getB(){
		return b.position;
	}

	public void setColor(Color c){
		this.c = c;
	}

	public Color getColor(){
		return c;
	}

	public void setTransformRef(Transform t){
		this.transformRef = t;
	}

	public Transform getTransformRef(){
		return transformRef;
	}

}
