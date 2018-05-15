using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cluster_Filter_Button : MonoBehaviour {

    // Document Behaviour associated with cluster button
    Document_Behaviour d;
    private GraphVisualizer graph;
    private VisualNode visualnode;

    // Filtering and unfiltering nodes
    int off = 0;

    void Start () {
        d = transform.parent.GetComponent<Document_Behaviour>();
        graph = FindObjectOfType<GraphVisualizer>();
    }
	
    // on interacting with button
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Controller") {
            if (d.getVisualNode().hiding() == true) {
                off = 1;
                gameObject.GetComponent<Image>().color = Color.black;
                print("on");
            }
            if (d.getVisualNode().hiding() == false) {
                off = 0;
                gameObject.GetComponent<Image>().color = Color.white;
                print("off");
            }
            // unhighlight previous node
            if(visualnode != null && d.getVisualNode() != visualnode) {
                visualnode.transform.parent.GetComponent<ClusterBehaviour>().ExitHighlight();
            }


            // set UI to white

            // Get cluster nodes and cluster class

            ClusterBehaviour c = d.getVisualNode().transform.parent.GetComponent<ClusterBehaviour>();
            ArrayList clusterNodes = c.returnClusterNodes();
            // if filtering cluster nodes
            if (off == 0) {
                d.getVisualNode().setHiding(true);
                foreach (VisualNode v in clusterNodes) {
                    if (v != d.getVisualNode()) {
                        graph.addIgnoreNode(v);
                    }
                }
                foreach (VisualNode v in clusterNodes) {
                    if (v != d.getVisualNode()) {
                        v.OnHideEdges();
                    }
                }
            }
            // if unfiltering cluster nodes
            if (off == 1) {
                d.getVisualNode().setHiding(false);
                foreach (VisualNode v in clusterNodes) {
                    if (v != d.getVisualNode()) {
                        graph.removeIgnoreNode(v);
                    }
                }
                foreach (VisualNode v in clusterNodes) {
                    if (v != d.getVisualNode()) {
                        v.UnHideEdges();
                    }
                }
            }
            off++;
            // reset back to 0 if greater than 1
            if(off > 1) {
                off = 0;
            }
            c.RecreateCluster();
            visualnode = d.getVisualNode();
        }
    }

    // on leaving button
    void OnCollisionExit(Collision other) {
        if (other.gameObject.tag == "Controller") {
            // set UI to black

            
        }
    }
}
