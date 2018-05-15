using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

    // nodes within the graph
    public ArrayList nodes = new ArrayList();

    // Empty constructor
    public Graph() {}

    // Add a node to the graph
    public void addNode(Node n) {
        nodes.Add(n);
    }

    // Check if a node is in the graph
    public Node checkNode(string nodeID) {
        if(nodes.Count == 0) {
            return null;
        }
        for(int i = 0; i < nodes.Count; ++i) {
            Node n = nodes[i] as Node;
            if(n.NodeID1 == nodeID) {
                return n;
            }
        }
        return null;
    }

    public void updateNode(Node node) {
        for (int i = 0; i < nodes.Count; ++i) {
            Node n = nodes[i] as Node;
            if (n.NodeID1 == node.NodeID1) {
                nodes[i] = node;
            }
        }
    }

    public override String ToString() {
        String s = "";
        for (int i = 0; i < nodes.Count; ++i) {
            Node n = nodes[i] as Node;
            s += n.NodeID1 + " ";
            ArrayList adj = n.getAdjacencies();
            for(int j = 0; j < n.getAdjacencies().Count; j++) {
                Node a = (Node)adj[j];
                s += a.NodeID1 + " ";
            }
            s += "\n";
        }
        return s;
    }
}
