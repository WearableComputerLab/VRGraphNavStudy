using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterBehaviour : MonoBehaviour {
    private ArrayList nodes;
    private ArrayList ignoreNodes;
    Color def, highlight;
    Material def_mat;
    bool makeNodesVisibile = false;
    bool off, clustered;
    // debugging highlight
    int highlight_cluster = 0;
    float t = 10f;
    // cluster size for debugging
    public int cluster_size = 0;
    private GameObject cluster_center;
    // default color for edges
    private Color def_color;
    GameObject pivot;
    GameObject cluster_text;
    private GraphVisualizer graph;
    private float saturation = 1;
	// Use this for initialization
	void Awake () {
        if (GetComponent<MeshRenderer>()) {
            def = GetComponent<MeshRenderer>().material.color;
            highlight = Color.yellow;
            def_mat = GetComponent<MeshRenderer>().material;
        }
        transform.tag = "cluster";
        ignoreNodes = new ArrayList();
        graph = FindObjectOfType<GraphVisualizer>();
    }

    private void Start() {
        cluster_size = nodes.Count;
        foreach (VisualNode v in nodes) {
            //GameObject g = v.gameObject;
            //g.transform.parent = transform;
        }
        cluster_center.SetActive(false);
        if (cluster_center.transform.childCount > 0) {
            cluster_text = cluster_center.transform.GetChild(0).gameObject;
            cluster_text.transform.parent = transform;
        }
        pivot = new GameObject();
        pivot.transform.position = cluster_center.transform.position;
        pivot.name = transform.name;
        transform.parent = pivot.transform;
        pivot.transform.parent = FindObjectOfType<GraphVisualizer>().transform;
        //make sure cluster center's parent is transform
        cluster_center.transform.parent = transform;
        if (GetComponent<MeshRenderer>()) {
            def = GetComponent<MeshRenderer>().material.color;
            highlight = Color.yellow;
            def_mat = GetComponent<MeshRenderer>().material;
        }
    }

    public void Highlight() {
        Debug.Log("Cluster = " + transform.name + " size = "+cluster_size);
        GetComponent<MeshRenderer>().material.color = highlight;
        cluster_text.transform.parent = cluster_center.transform;
        if (!makeNodesVisibile && nodes.Count < 1000) {
            transform.GetComponent<MeshRenderer>().enabled = false;
            foreach(VisualNode v in nodes) {
                if (graph.getIgnoreNodes().Contains(v) == false) {
                    v.gameObject.SetActive(true);
                    v.setLabelVisible();
                    ArrayList edge = v.getAllocatedEdges();
                    if (edge.Count > 0) {
                        GameObject index0 = edge[0] as GameObject;
                        //def_color = index0.GetComponent<LineRenderer>().material.GetColor("_TintColor");
                        //foreach (GameObject g in edge) {
                        //LineRenderer l = g.GetComponent<LineRenderer>();
                        //Color def_color = g.GetComponent<LineRenderer>().material.GetColor("_TintColor");
                        //l.material.SetColor("_TintColor", new Color(def_color.r,def_color.g,def_color.b,0.02f));
                        //}
                    }
                }
            }
            BoxCollider[] colliders = GetComponents<BoxCollider>();
            foreach(BoxCollider b in colliders) {
                b.enabled = false;
            }
            makeNodesVisibile = true;
        }
    }

    void FixedUpdate() {
        if (off) {
            RecreateCluster();
            if (clustered) {
                // add code back later..
                off = false;
                clustered = false;
            }
        }
        
        if(highlight_cluster == 1) {
            t -= Time.deltaTime;
            if(t < 0) {
                ExitHighlight();
                highlight_cluster = 0;
            }
        }
        
    }

    public void ExitHighlight() {
        //off = true;
        cluster_text.transform.parent = transform;
        GetComponent<MeshRenderer>().material.color = def;
        print("exit_highlight");
        if (makeNodesVisibile) {
            transform.GetComponent<MeshRenderer>().enabled = true;
            foreach (VisualNode v in nodes) {
                v.gameObject.SetActive(false);
                ArrayList edge = v.getAllocatedEdges();
                if (edge.Count > 0) {
                    //foreach (GameObject g in edge) {
                        //Color def_color = g.GetComponent<LineRenderer>().material.GetColor("_TintColor");
                        //LineRenderer l = g.GetComponent<LineRenderer>();
                        //l.material.SetColor("_TintColor", def_color);
                    //}
                }
            }
            BoxCollider[] colliders = GetComponents<BoxCollider>();
            foreach (BoxCollider b in colliders) {
                b.enabled = true;
            }
            makeNodesVisibile = false;
        }
        RecreateCluster();
    }
    // set cluster nodes for reference
    public void setArrayList(ArrayList a) {
        nodes = a;
    }

    // add node to cluster nodes
    public void addClusterNode(VisualNode n) {
        nodes.Add(n);
    }

    // return nodes assigned to the clusters
    public ArrayList returnClusterNodes() {
        return nodes;
    }

    // Add a node to ignore when updating a cluster
    public void addIgnoreNode(VisualNode v) {
        ignoreNodes.Add(v);
        v.gameObject.SetActive(false);
    }
    // Remove a node to ignore when updating a cluster
    public void removeIgnoreNode(VisualNode v) {
        ignoreNodes.Remove(v);
    }

    public void RecreateCluster() {
            transform.parent = null;
            // get ignore nodes length
            int ignore_nodes_length = ignoreNodes.Count;
            // setup meshfilters for child objects
            MeshFilter[] meshFilters = new MeshFilter[nodes.Count];           
            // combine instance mesh array
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            // go through each child node and add to meshFilter array
            int k = 0;
            BoxCollider[] colliders = gameObject.GetComponents<BoxCollider>();

            foreach (VisualNode v in nodes) {
                if (graph.getIgnoreNodes().Contains(v) == false) {
                    GameObject g = v.gameObject;
                    meshFilters[k] = g.GetComponent<MeshFilter>();
                    Matrix4x4 cluster_transform = transform.worldToLocalMatrix;
                    combine[k].mesh = meshFilters[k].sharedMesh;
                    combine[k].transform = cluster_transform * meshFilters[k].transform.localToWorldMatrix;
                    colliders[k].center = combine[k].transform.GetPosition();
                    colliders[k].isTrigger = true;
                } 
                k++;
            }
            GetComponent<MeshFilter>().mesh = new Mesh();
            // combine all the meshes in the mesh array "combine"
            GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            GetComponent<MeshRenderer>().material = def_mat;
            // assign random color to cluster
            Color col = new Color(def.r * saturation, def.g * saturation, def.b * saturation);
            GetComponent<MeshRenderer>().material.color = col;
            pivot.transform.position = cluster_center.transform.position;
            transform.parent = pivot.transform;    

            clustered = true;
    }

    // Mouse Unclustering for Debugging (Don't need Vive Controllers)
    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            //highlight_cluster = 1;
            //Highlight();
            //t = 3f;
        }
    }

    // not currently being used due to crashes
    private void ClusterSplit() {
         // --- Split the cluster ---
        /* Theres an issue with cluster lengths > 50 and crashing
         * clusters are graphically split, but spatially still the same. 
        */
        if (cluster_size > 50) {
            // calculate amount of clusters needed to split
            float num = cluster_size/50;
            int cluster_rounded = Convert.ToInt32(num);
            // for each cluster needed create and add node length
            for (int i = 0; i < cluster_rounded; ++i) {  

                // Create new cluster object
                GameObject cluster = new GameObject();
                // setup meshfilters for child objects
                MeshFilter[] meshFilters = new MeshFilter[50];
                // combine instance mesh array
                CombineInstance[] combine = new CombineInstance[meshFilters.Length];
                // Array for new cluster
                ArrayList new_nodes = new ArrayList();
                for(int k = 50 * i; k < k +50; ++k) {
                    if(k < nodes.Count){
                        new_nodes.Add(nodes[k]);
                    }
                }
                // set the parent of the cluster to visualization object
                cluster.transform.parent = this.transform;
                // set the name of the cluster
                cluster.transform.name = transform.name + " cluster " +i;
                // add meshfilter to new cluster 
                cluster.AddComponent<MeshFilter>();
                cluster.AddComponent<MeshRenderer>();
                cluster.GetComponent<MeshFilter>().mesh = new Mesh();
                // combine all the meshes in the mesh array "combine"
                cluster.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
                cluster.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
                // assign random color to cluster
                cluster.GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV(0,1f,0.5f,1f,0.5f,1f);
                // add clustercode
                cluster.AddComponent<ClusterBehaviour>();
                ClusterBehaviour c = cluster.GetComponent<ClusterBehaviour>();
                c.setArrayList(new_nodes);
                // lastly, add collider
                cluster.AddComponent<MeshCollider>();
            }
            // delete gameobject 

        }
    }

    // Set cluster centre to be visible on start
    public void setClusterCentre(GameObject g) {
        cluster_center = g;
    }

    public void Dim() {
        saturation = 0.3f;
    }

    public void Brighten() {
        saturation = 1f;
    }
}
