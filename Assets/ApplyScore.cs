using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyScore : MonoBehaviour {

	private Text txt;
	public CaptureSphere cs;
	public NodesCompleted n;
	private StudyManager studyManager;
	private Recorder record;
	// Use this for initialization
	void Start () {
		txt = GetComponent<Text> ();
		studyManager = FindObjectOfType<StudyManager>();
		record = FindObjectOfType<Recorder>();
	}
	
	// Update is called once per frame
	void Update () {
		if (cs != null) {
			if (cs.getNodesCollected () < cs.studySize+1) {
				txt.text = "find node: " + cs.getNodesCollected ();
			} else {
				txt.text = "complete";
				Camera.main.backgroundColor = Color.white;
				if (studyManager != null) {
					if(record != null){
						record.outputToCSV();
					}
					studyManager.incrementScene();
				}
			}
		} 
		if (n != null) {
			if (n.nodesCompleted < n.totalNodes) {
				txt.text = "find node: " + n.nodesCompleted;
			} else {
				txt.text = "complete";
				Camera.main.backgroundColor = Color.white;
				if (studyManager != null) {
					if(record != null){
						record.outputToCSV();
					}
					studyManager.incrementScene();
				}
			}
		}
	}
}
