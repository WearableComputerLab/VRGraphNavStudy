using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    // node information abstract
    public string NodeID1;
    public string NodeID2;
    public ArrayList adjacencys = new ArrayList();
    public Vector3 position;
    private Transform transform;

    // Set and get the transform parent 
    public Transform Transform {
        get {
            return transform;
            }

        set {
            transform = value;
            }
    }

    // node constructor
    public Node(string nid1, string nid2) {
        NodeID1 = nid1;
        NodeID2 = nid2;
    }

    // return node information
    public string getNodeID1() {
        return NodeID1;
    }
    public string getNodeID2() {
        return NodeID2;
    }

    // add adjacencies
    public void addAdjacency(Node n) {
        adjacencys.Add(n);
    }

    // return adjacencies
    public ArrayList getAdjacencies() {
        return adjacencys;
    }
    // Add positional info to the node data structure
    public void setVector3(Vector3 vector) {
        position = vector;
    }

    public override string ToString() {
        return NodeID1;
    }
}
