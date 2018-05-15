using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {

	// The two path nodes the user needs to find a path to. 
	private Transform a, b;
	private GraphVisualizer graph; 

	// Use this for initialization
	void Start () {
		graph = FindObjectOfType<GraphVisualizer> ();
		a = transform.GetChild (0);
		b = transform.GetChild (1);
	}
	
	// Update is called once per frame
	void Update () {
		// Hit Enter to create the path
		if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
			PathMarker[] markers = FindObjectsOfType<PathMarker>();
			foreach (PathMarker p in markers) {
				Destroy (p.gameObject);
			}
			ArrayList allNodes = graph.returnGraphNodes ();
			GameObject g1 = allNodes [Random.Range (0, allNodes.Count)] as GameObject;
			GameObject g2 = allNodes [Random.Range (0, allNodes.Count)] as GameObject;
			Vector3 vA = g1.transform.position;
			Vector3 vB = g2.transform.position;
			a.transform.position = vA;
			b.transform.position = vB;
		}
	}
}
