using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyNode : MonoBehaviour {

    private VisualNode v;
    public StudyObject study_obj;
    public Transform miniatureNode;
    public StudyNode next;
    public StudyEdges edges;
    public StudyText studyPrompt;
    public Material edgeMaterial;
    private string NodeID;
    private float time_taken = 0;
    private bool countTime = true;
    public bool highlight_studynode = false;
    public bool createEdgesDebug = false;
    private float restart = 10;
    private ArrayList edgesCreated = new ArrayList();
    private World_Miniature_Graph wim;
    private Miniature_Camera_Behaviour cam;
    public float WIMdivider = 10;

    // 
    private float nextActionTime = 0.0f;
    public float period = 2f;

    // Use this for initialization
    void Start () {
        wim = FindObjectOfType<World_Miniature_Graph>();
        cam = FindObjectOfType<Miniature_Camera_Behaviour>();
	}
	
	// Update is called once per frame
	void Update () {
		if(v != null) {
            transform.position = v.transform.position;
        }
        if(countTime) {
           time_taken += Time.deltaTime*1000;
        }
        if(highlight_studynode) {
            highlight();
            //restart -= Time.deltaTime;
            if(study_obj.studyStage < 3) {
                restart = 0;
            }
            if(restart <= 0 || Input.GetKeyDown(KeyCode.E)) {
                deleteEdges();
                highlight_studynode = false;
                reachedNode();
            }
        } else {
            exit_highlight();
            restart = 10;
        }
        if(miniatureNode != null) {
            SteamVR_Controller.Device controller1 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
            SteamVR_Controller.Device controller2 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
            if (controller1.GetHairTrigger() == false && controller1.GetHairTrigger() == false) {
                Vector3 magnitude = (transform.position - wim.C) / 15;
                Vector3 Sm = wim.Cm + magnitude;
                miniatureNode.transform.position = Sm;
            }
        }

	}

    public void setTarget(VisualNode v) {
        this.v = v;
        NodeID = v.getNode().NodeID1;
    }

    public void setNext(StudyNode next) {
        this.next = next;
    }

    public void reachedNode() {
        if(next != null) { 
            study_obj.setCurrentNode(next);
            next.countTime = true;
            next.gameObject.SetActive(true);
            if(next.miniatureNode != null) {
                next.miniatureNode.gameObject.SetActive(true);
            }
        } else {
            study_obj.setStage();
        }
        countTime = false;
        this.gameObject.SetActive(false);
        if (miniatureNode != null) {
            miniatureNode.gameObject.SetActive(false);
        }
    }

    public string getID() {
        return NodeID;
    }

    public float getTime_Taken() {
        return time_taken;
    }

    public void resetTime() {
        time_taken = 0;
    }

    public void highlight() {
       GetComponent<Light>().color = Color.white;
        if(study_obj.studyStage > 2) {
            studyPrompt.moveUI(transform.position);
        }
    }

    public void exit_highlight() {
       GetComponent<Light>().color = Color.red;
    }

    private void OnCollisionEnter(Collision other) {
        if(other.transform.tag == "Controller") {
            highlight();
            print("controller collided with study node");
        }
    }

    private void OnCollisionExit(Collision other) {
        if(other.transform.tag == "Controller") {
            exit_highlight();
        }
    }

    // Create thicker edges when highlighting
    public void createEdges() {
        ArrayList adjNodes = v.getAdjacencies();
        int adjCount = adjNodes.Count;
        for(int i = 0; i < adjCount; i++) {
            GameObject g = new GameObject();
            g.AddComponent<LineRenderer>();
            LineRenderer l = g.gameObject.GetComponent<LineRenderer>();
            l.startWidth = 0.005f; l.endWidth = 0.005f;
            VisualNode adjNode = (VisualNode) adjNodes[i];
            l.SetPosition(0,transform.position);    
            l.SetPosition(1,adjNode.transform.position);   
            l.material = edgeMaterial;
            g.AddComponent<StudyEdges>();
            g.GetComponent<StudyEdges>().setEdge(transform,adjNode.transform);
            edgesCreated.Add(g.GetComponent<StudyEdges>());
        }
    }

    // delete all edges
    private void deleteEdges() {
        print("delete method called");
        if(edgesCreated.Count > 0) { 
            foreach(StudyEdges g in edgesCreated) { 
                if(g != null) { 
                    g.destroy = true;
                    Destroy(g.gameObject);
                    if(g.destroy == true) {
                        print("g.destory == true");
                    }
                    print("destroyed");
                }
            }
        }
    }




}
