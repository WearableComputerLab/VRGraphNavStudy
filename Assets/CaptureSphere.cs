using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Capture sphere.
/// 	Collects nodes for Simple Find and Complex Find for traversal experiments. 
/// </summary>
public class CaptureSphere : MonoBehaviour {

	// Collider Arrays
	private Collider[] colliders;
	public Transform[] nodePositions;
	private ArrayList nodePosArr = new ArrayList ();
	public Transform[] studyNodes;

	// Assignment of Colliders
	private int nodeIndex = 0;

	[Header("Size of Nodes Captured and Cluster Point")]
	// Cluster Variables
	public int studySize = 10;
	public int grabClusterPoint = 5;

	// Reference to cone on Vive Controller
	public Transform conePoint;

	// Chain reference
	public CaptureSphere chain;

	// Reference to the current Study Node
	private int studyIndex = 0;
	private double distanceThreshold = 0.02;
	private Transform traversalNode;
	private Transform cluster;
	private int nodesCollected = 1;
	private Recorder recorder;
	private Transform[] nodeReferences;

	[Header("Assign for Complex Traversals")]
	// nodesCollected reference
	public NodesCompleted nodesComplete;

	[Header("WIM Study Node")]
	public Transform wimStudyNode;
	public World_Miniature_Graph wim;
	private bool setWIMPos = false;

	#region Unity Initialization and Loop

	// Use this for initialization
	private void Start () {
		nodePositions = new Transform[studySize];
		// Get graphvisualizer from hierarchy and assign a transform position from a random cluster 
		// in this case the fifth child (2nd cluster).
		GraphVisualizer g = FindObjectOfType<GraphVisualizer> ();
		transform.position = g.transform.GetChild(grabClusterPoint).GetChild(0).GetChild(1).position;
		recorder = FindObjectOfType<Recorder>();
		nodeReferences = new Transform[studyNodes.Length];
		wim = FindObjectOfType<World_Miniature_Graph>();
	}
	
	// Update is called once per frame
	private void Update () {
		FindStudyNodes();
		RunExperiment();
		if(recorder == null){
			recorder = FindObjectOfType<Recorder>();
		}
		// check if studynodes is empty, if it is then re-assign
		if (nodePositions [0] == null) {
			print ("null");
			GraphVisualizer g = FindObjectOfType<GraphVisualizer> ();
			transform.position = g.transform.GetChild (grabClusterPoint).GetChild (0).GetChild (1).position;
		} 
	}

	#endregion

	#region Custom Methods

	// Assign all study nodes within a spherical volume
	private void FindStudyNodes(){
		// Assign study nodes
		if (nodeIndex < studySize) {
			colliders = Physics.OverlapSphere (transform.position, transform.localScale.x);
			foreach (Collider c in colliders) {
				Transform t = c.transform;
				if (cluster == null) {
					cluster = t;
				}
				if (t.childCount > 0 && !t.GetComponent<VisualNode>()) {
					int randomInteger = Random.Range (0, t.childCount);
					if (t.GetChild (randomInteger).parent == cluster && !nodePosArr.Contains(t.GetChild(randomInteger))) {
						nodePositions [nodeIndex] = t.GetChild (randomInteger);
						nodePosArr.Add (t.GetChild (randomInteger));
						studyNodes [nodeIndex].transform.position = nodePositions [nodeIndex].transform.position;
						studyNodes [nodeIndex].gameObject.SetActive (false);
						nodeIndex++;
					}
				}
			}
		}	
	}

	// Runs the experiment
	private void RunExperiment(){
		if (studyIndex < studySize) {
			traversalNode = studyNodes [studyIndex];
			traversalNode.gameObject.SetActive (true);
			if(nodePositions[studyIndex] != null){
				traversalNode.transform.position = nodePositions[studyIndex].position;
			}
			if(wim != null && setWIMPos == false){
			    Vector3 magnitude = (traversalNode.position - wim.C) / 15;
                Vector3 Sm = wim.Cm + magnitude;
                wimStudyNode.transform.position = Sm;
				setWIMPos = true;
			}
		} else {
			if (chain != null) {
				traversalNode.gameObject.SetActive (false);
				chain.gameObject.SetActive (true);
				this.gameObject.SetActive (false);
			}
		}
		if (Vector3.Distance (conePoint.position, traversalNode.position) <= distanceThreshold) {
			if (nodesComplete != null && traversalNode.gameObject.activeSelf == true) {
				nodesComplete.nodesCompleted++;
			}
			if(recorder != null){
				recorder.addTask();
			}
			setWIMPos = false;
			traversalNode.gameObject.SetActive (false); 
			studyIndex++;
			nodesCollected++;
		}
	}

	// Return the amount of nodes collected
	public int getNodesCollected(){
		return nodesCollected;
	}

	#endregion
}
