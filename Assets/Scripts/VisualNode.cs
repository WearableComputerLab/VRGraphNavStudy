using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class VisualNode : MonoBehaviour {

    public bool hasEdge = false;
    public bool hasClusterNodes = false;
    Node node_info;
    // attribute information, is left blank initially
    public string attribute_info = "";
    // The first adjacency for the given node for locality
    VisualNode firstAdjacency;
    public string node_name = "";
    public bool rearranged = false;
    public float distance = 0;
    public int moved = 0;
    public Vector3 node_debug_position;
    public string[] adjVisNodes;
    // default color and highlighted color
    Color c_def, c_highlight;
    // distinct nodes (i.e. circle0 ... circleN)
    public bool distinct_node = false;
    // Node related to a cluster
    private ArrayList clusterNodes = new ArrayList();

    private ArrayList allocatedEdges = new ArrayList();
    private EdgeUpdate[] edgesFound;
    private bool highlighted = false;
    private bool hide_edges = false;
    private bool currently_hidden = false;

    private ArrayList adjacentNodes = new ArrayList();
	// Use this for initialization
	void Start () {
        c_def = GetComponent<MeshRenderer>().material.color;

        // sets color to white
        c_highlight = new Color(c_def.r + 1, c_def.g + 1, c_def.b + 1);

        // make sure the node is in the correct position
        //transform.position = node_info.position;
        //print(node_info.getAdjacencies().Count +" COUNT A" + node_info.NodeID1);
        adjVisNodes = new string[node_info.getAdjacencies().Count];
        for(int i = 0 ; i < adjVisNodes.Length; i++) {
            adjVisNodes[i] = node_info.getAdjacencies()[i].ToString();
        }
        if(transform.childCount > 0 && distinct_node) {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        if (distinct_node) {
            transform.localScale = transform.localScale * 2f;
            attribute_info = " Network Size: " + "\r" + clusterNodes.Count;
        }
        //print(attribute_info.GetHashCode());

    }
	

    // TODO: Likely delete this
    void rearrangeNodes() {
  
    }

    void Update() {
        hasEdge = (allocatedEdges.Count > 0);
        hasClusterNodes = (clusterNodes.Count > 0);
        if (adjVisNodes.Length == 0) {
            GetComponent<Rigidbody>().isKinematic = true;
        } else {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void setLabelVisible() {
        if(transform.childCount > 0)
        transform.GetChild(0).gameObject.SetActive(true);
    }

    float calculateDistance(VisualNode v) {
        return Vector3.Distance(transform.position,v.transform.position);
    }

    public void setNode(Node node) {
        node_info = node;
        node_name = node.NodeID1;

    }

    public string getName() {
        return node_name;
    }
    
    // Set the color for a given node and scale to distiniguish
    public void DistinguishVisualNode(Color col, Vector3 scale) {
        GetComponent<MeshRenderer>().material.color = col;
        transform.localScale = scale;
    }

    // Add function to focus on node when clicked
    void OnMouseOver() {
        //print(gameObject.name);
        //GetComponent<MeshRenderer>().material.color = c_highlight;
    }

    void OnMouseExit() {
       // print(gameObject.name);
        //GetComponent<MeshRenderer>().material.color = c_def;
    }

    public void OnLaserOver() {

        if (highlighted == false) {
            edgesFound = Camera.main.GetComponents<EdgeUpdate>();
            GetComponent<MeshRenderer>().material.color = c_highlight;
            if (allocatedEdges.Count > 0 && !hide_edges) {

                foreach (int g in allocatedEdges) {
                    if (edgesFound[g].isHidden() == false) {
                        edgesFound[g].Highlight();
                    }
                }
            }
            Behaviour b = GetComponent("Halo") as Behaviour;
            b.enabled = true;
            highlighted = true;
        }
    }

    public void OnLaserExit() {
        if (highlighted == true) {
            edgesFound = Camera.main.GetComponents<EdgeUpdate>();
            GetComponent<MeshRenderer>().material.color = c_def;
            Behaviour b = GetComponent("Halo") as Behaviour;
            b.enabled = false;
            if (allocatedEdges.Count > 0) {
                foreach (int g in allocatedEdges) {
                    if (edgesFound[g].isHidden() == false) {
                        edgesFound[g].ExitHighlight();
                    }
                }
            }
            highlighted = false;
        }
    }

    public void OnHideEdges() {
        hide_edges = true;
        edgesFound = Camera.main.GetComponents<EdgeUpdate>();
        GetComponent<MeshRenderer>().material.color = c_def;
        if (allocatedEdges.Count > 0) {
            foreach (int g in allocatedEdges) {
                VisualNode a = edgesFound[g].a.GetComponent<VisualNode>();
                VisualNode b = edgesFound[g].b.GetComponent<VisualNode>();
                edgesFound[g].Hide();
            }
        }
    }

    public void UnHideEdges() {
        hide_edges = false;
        edgesFound = Camera.main.GetComponents<EdgeUpdate>();
        GetComponent<MeshRenderer>().material.color = c_def;
        if (allocatedEdges.Count > 0) {
            foreach (int g in allocatedEdges) {

                VisualNode a = edgesFound[g].a.GetComponent<VisualNode>();
                VisualNode b = edgesFound[g].b.GetComponent<VisualNode>();
                if (a.getCurrentlyHidden() == false && b.getCurrentlyHidden() == false) {
                    edgesFound[g].UnHide();
                }
            }
        }
    }

    public bool hiding() {
        return hide_edges;
    }

    public void setHiding(bool b) {
        hide_edges = b;
    }

    public void setCurrentlyHidden(bool b) {
        currently_hidden = b;
    }

    public bool getCurrentlyHidden() {
        return currently_hidden;
    }

    public Node getNode() {
        return node_info;
    }

    // Return the first adjacency for a given node
    public VisualNode getFirstAdjacency() {
        return firstAdjacency;
    }

    // Set the first adjacency for a given node
    public void setFirstAdjacency(VisualNode v) {
        this.firstAdjacency = v;
    }

    public void addAdjacency(VisualNode n) {
        adjacentNodes.Add(n);
    }

    public ArrayList getAdjacencies() {
        return adjacentNodes;
    }

    // Add child cluster nodes
    public void addClusterNode(VisualNode v) {
        clusterNodes.Add(v);
    }
    // Return cluster nodes
    public ArrayList getClusterNodes() {
        return clusterNodes;
    }

    public void addAllocatedEdge(int edge) {
        allocatedEdges.Add(edge);
    }
    public ArrayList getAllocatedEdges() {
        return allocatedEdges;
    }
    public void setAttributeInfo(string info) {
        this.attribute_info = info;
    }
    public string getAttributeInfo() {
        return this.attribute_info;
    }

    public void ColorWhite() {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void ColorDef() {
        GetComponent<MeshRenderer>().material.color = c_def;
    }
}
