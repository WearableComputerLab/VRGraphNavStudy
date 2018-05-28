using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {

	// The two path nodes the user needs to find a path to. 
	private Transform a, b;
	private GraphVisualizer graph; 
	private bool search = false;

	private int depth = 0;
	private int explore = 0;
	private int times = 0;
	private VisualNode v1, v2;
	private GameObject g1;

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
			InitializePathFind ();
			times++;
		}
		if (search) {
			searchGraph();
		}
		if (times > 10) {
			Camera.main.backgroundColor = Color.white;
		}
	}

	private void searchGraph(){
		while (depth < explore) {
			int adjCount = v2.getAdjacencies().Count;
			int randomIndex = Random.Range (0, adjCount);
			v2 = v2.getAdjacencies()[randomIndex] as VisualNode;
			if (v2 == v1) {
				InitializePathFind ();
			}
			depth++;
		} 
		if(depth >= explore){
			Vector3 vA = g1.transform.position;
			Vector3 vB = v2.transform.position;
			a.transform.position = vA;
			b.transform.position = vB;
			search = false;
			v2 = null;
		}
	}

	private void InitializePathFind(){
		search = true;
		PathMarker[] markers = FindObjectsOfType<PathMarker>();
		foreach (PathMarker p in markers) {
			Destroy (p.gameObject);
		}
		ArrayList allNodes = graph.returnGraphNodes ();
		g1 = allNodes [Random.Range (0, allNodes.Count)] as GameObject;
		v1 = g1.GetComponent<VisualNode> ();
		v2 = v1;
		depth = 0;
		explore = Random.Range (2, 6);
	}
}
