using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GraphVisualizer : MonoBehaviour {

	[Header("Study Datasets")]
	public TextAsset[] LargeGraphDatasets;
	public TextAsset[] FeatureGraphDatasets;


	[Header("Graph Type and Variables")]
	public TextAsset dataset;

    // Edge generation type
    public enum EdgeType {MeshGenerate,RigGenerate}
    public EdgeType GeneratedEdge;

    // Graph generation type
    public enum GraphType {Circles, UndirectedGraph}
    public GraphType TypeGraph;
    public int k_clusters = 3;

	[Header("Advanced Options")]

    public bool QuadrantLayout = false;
    // Node mesh reference
    public GameObject node, DynamicEdge;

    // Dataset for input, needs to .txt
    public TextAsset[] attributeDataSet;
    private string[] lines;

    // Input type enumerator
    public enum InputType {StanfordSnap, OtherType}
    public InputType InputData;

    // Edge Color and Material

    public Color edgeColor;
    public Material EdgesMaterial, nodeMat;

    // Graph reference
    Graph graph = new Graph();
    // Nodes in the scene added to array for reference
    // - Will be used for adding a spring algorithm later.
    Dictionary<Node,VisualNode> nodes_in_scene = new Dictionary<Node,VisualNode>();
    Rigidbody[] springNodes;
    ArrayList clusterNodes = new ArrayList();
    ArrayList nodes_already_clustered = new ArrayList();
    ArrayList colombs_nodes = new ArrayList();
    ArrayList edges = new ArrayList();
    ArrayList ignoreNodes = new ArrayList();
    ArrayList clusters = new ArrayList();

    // Node Positions of each node in the scene
    Vector3[] nodePositions;

    // Label for each node gameobject for reference
    private List<GameObject> labelsGameObjects = new List<GameObject>();

    // For edge generation:
    // Vertices to link between nodes
    HashSet<EdgeLink> HashEdges = new HashSet<EdgeLink>();
    public float tubeWidth = 0.10f;

    // Radius of the graph overall
    public int clusterRadius = 50;
    public int overallRadius = 50;
    public Vector3 axisScale = new Vector3(1,3,1);

    //Debugging Variables; to be removed later
    private int doubleUps = 0;
    
    // cluster generation boolean to reduce drawcalls
    private bool clusterAllNodes = false;
    public static bool graph_layout_recognized = false;
    private bool isLoadingLayout = false;
    private bool runColombs = false;
    private float columbsTime = 0;

    // graph layout count
    public static int layout_edges = 0;
    int num_of_edges = 0;
    int cullingmask;

    //startup controller from first menu
    private Startup_Controller startup;
    private string graphData = "";
    private string attributeData = "";

    // Use this for initialization
    void Awake () {
        // layout variables
        double f = 1.500;
        int x_d = (int)((f - (int)f)*100);

        string filename = @".\" + dataset.name + ".layout";

        // Startup Controller Assignments
        startup = FindObjectOfType<Startup_Controller>();
        if (startup != null) {
            graphData = startup.graph_dataset;
            attributeData = startup.attribute_dataset;
            k_clusters = startup.k_clusters;
            QuadrantLayout = startup.quadrant_layout_on;
        }
        print(graphData.Equals(String.Empty));
        //print(x_d);
        if (TypeGraph == GraphType.Circles) {
            InitializeGraphStructure();
        }
        if(TypeGraph == GraphType.UndirectedGraph) {
            InitializeUnGraphStructure();
            print(graph.ToString());
        }

        generateVisualization();

        //printGraph();
        DistinguishNodes();

        RearrangeNodes();

        // Generate the Edges from Node to Node
        if (GeneratedEdge == EdgeType.MeshGenerate){
            generateGraphTubes(nodes_in_scene,1);
        }
        if (GeneratedEdge == EdgeType.RigGenerate){
            generateRigEdges(nodes_in_scene,1);
        }
        for (int i = 0; i < attributeDataSet.Length; ++i) {
            readAttributeData(attributeDataSet[i]);
        }

        print("# of edges not being missed: " +doubleUps);
        print("size of graph: " + nodes_in_scene.Count);

        if (File.Exists(filename)) {
      
            graph_layout_recognized = true;
            isLoadingLayout = true;
            print("found layout");
        }
        
    }
	
    void Start() {
        //CombineMeshes();
        cullingmask = Camera.main.cullingMask;
        //Camera.main.cullingMask = (1 << LayerMask.NameToLayer("Nothing"));
    }


    //Optimisation Test
    void CombineMeshes() {
        // go through each clusterNode parent
        foreach (VisualNode n in clusterNodes) {
            // arraylist to be added to cluster late
            ArrayList nodes = new ArrayList();
            // setup meshfilters for child objects
            MeshFilter[] meshFilters = new MeshFilter[n.getClusterNodes().Count +1];
            // combine instance mesh array
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            //stop center from being combined
            nodes_already_clustered.Add(n);
            // Create a new cluster gameobject
            GameObject newCluster = new GameObject();

            // Color of cluster
            Color col = UnityEngine.Random.ColorHSV(0.1f, 0.8f, 0.7f, 1f, 0.5f, 1f);


            // go through each child node and add to meshFilter array

            // first cluster node
            GameObject c1 = n.gameObject;
            meshFilters[0] = c1.GetComponent<MeshFilter>();
            combine[0].mesh = meshFilters[0].sharedMesh;
            combine[0].transform = meshFilters[0].transform.localToWorldMatrix;
            meshFilters[0].gameObject.SetActive(false);
            nodes.Add(n);
            nodes_already_clustered.Add(n);
            // add boxcollider for each node
            //BoxCollider b = newCluster.AddComponent<BoxCollider>();
            //b.size = new Vector3(0.05f, 0.05f, 0.05f);
            //b.center = c1.transform.position;
            //b.isTrigger = true;
            ArrayList edge = n.getAllocatedEdges();
            if (edge.Count > 0) {
                GameObject index0 = edge[0] as GameObject;
                foreach (GameObject ga in edge) {
                    LineRenderer l = ga.GetComponent<LineRenderer>();
                    l.material.SetColor("_TintColor", new Color(col.r, col.g, col.b, 0.01f));
                }
            }

            int k = 1;

            foreach (VisualNode v in n.getClusterNodes()) {

                if(nodes_already_clustered.Contains(v) == false){
                    GameObject g = v.gameObject;
                    meshFilters[k] = g.GetComponent<MeshFilter>();
                    combine[k].mesh = meshFilters[k].sharedMesh;
                    combine[k].transform = meshFilters[k].transform.localToWorldMatrix;
                    meshFilters[k].gameObject.SetActive(false);
                    nodes.Add(v);
                    nodes_already_clustered.Add(v);
                    // add boxcollider for each node
                    //BoxCollider b1 = newCluster.AddComponent<BoxCollider>();
                    //b1.size = new Vector3(0.05f, 0.05f, 0.05f);
                    //b1.center = g.transform.position;
                    //b1.isTrigger = true;
                    v.transform.parent = n.transform;
                    ArrayList edge1 = v.getAllocatedEdges();
                    if (edge1.Count > 0) {
                        GameObject index0 = edge1[0] as GameObject;
                        foreach (GameObject ga in edge1) {
                            LineRenderer l = ga.GetComponent<LineRenderer>();
                            l.material.SetColor("_TintColor", new Color(col.r, col.g, col.b, 0.01f));
                        }
                    }
                } else {
                }
                k++;

            }

            // set the parent of the cluster to visualization object
            newCluster.transform.parent = this.transform;
            // set the name of the cluster
            newCluster.transform.name = n.transform.name + " cluster";

            // add meshfilter to new cluster 
            newCluster.AddComponent<MeshFilter>();
            newCluster.AddComponent<MeshRenderer>();
            newCluster.GetComponent<MeshFilter>().mesh = new Mesh();
            // combine all the meshes in the mesh array "combine"
            newCluster.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            newCluster.GetComponent<MeshRenderer>().material = nodeMat;
            // assign random color to cluster
            newCluster.GetComponent<MeshRenderer>().material.color = col;
            // add clustercode
            newCluster.AddComponent<ClusterBehaviour>();
            ClusterBehaviour c = newCluster.GetComponent<ClusterBehaviour>();
            n.transform.parent = c.transform;
            c.setArrayList(nodes);
            c.setClusterCentre(n.gameObject);


            // lastly, add collider (trying box colliders)
            //newCluster.AddComponent<MeshCollider>();
        }
		EdgeManager edgeManager = Camera.main.GetComponent<EdgeManager> ();
		ArrayList glEdges = edgeManager.getEdges();
		print (glEdges.Count + "glEdge count");
		for(int i = 0; i < glEdges.Count; i ++) {
			Edge e = (Edge)glEdges[i];
			e.setColor(e.getTransformRef().parent.parent.GetComponent<MeshRenderer>().material.color);
		}

        clusterAllNodes = true;
        graph_layout_recognized = false;

        Camera.main.cullingMask = cullingmask;
    }

	// Update is called once per frame
	void Update () {
        
        // resize graph to visible scale
        transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        if(!clusterAllNodes && layout_edges >= (num_of_edges - num_of_edges/10) && !graph_layout_recognized) {
            runColombs = true;
            
        }
        if (runColombs && !graph_layout_recognized && isLoadingLayout == false) {
           columbsTime += Time.deltaTime;
           ApplyColombsLaw();
           if(columbsTime > 0.1f) {
                runColombs = false;
                if (TypeGraph == GraphType.Circles) {
                    CombineMeshes();
                } else {
                    KMeansClustering();
                }
           }
        }
        if (graph_layout_recognized) {
            LoadLayout();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(1);
        }
        //print(layout_edges +"L , N"+num_of_edges);
    }

    // Create the Graph and Add Nodes
    void InitializeGraphStructure() {
      if (InputData == InputType.StanfordSnap) {
            if (dataset != null) {
                // split each line
                if (graphData.Equals(String.Empty)) {
                    String s = dataset.text.Replace("\r", "\n");
                    lines = s.Split("\n"[0]);
                    print("loading previous dataset");
                } else {
                    graphData.Replace("\r", "\n");
                    lines = graphData.Split("\n"[0]);
                    print("loading menu dataset");
                }
                // loop through each line
                for (int i = 0; i < lines.Length; ++i) {
                    string[] node_features = lines[i].Split(new char[0]);
                    Node current_node = null;
                    //print(node_features[0] + " : current Node");
                    // if node does not exist in graph
                    if (graph.checkNode(node_features[0]) == null && 
                        node_features[0].Equals(String.Empty)==false) {
                        current_node = new Node(node_features[0], "");
                    } else {
                        // if node does exist in graph
                        current_node = graph.checkNode(node_features[0]);

                    }
                    if (current_node != null) {
                        // loop through each property of the text file
                        for (int j = 0; j < node_features.Length; ++j) {
                            //print(node_features[j]);
                            // first node is label
                            if (current_node.NodeID1.Equals(String.Empty)) {
                                print("found empty node " + i);
                            }
                            if (j == 0 || current_node.NodeID1.Equals(String.Empty)) {

                            } else {
                                // if cluster size is lesser than..
                                if (j < 1000) {
                                    if (graph.checkNode(node_features[j]) == null && node_features[j] != "") {
                                        Node adjNode = new Node(node_features[j], "");
                                        adjNode.addAdjacency(current_node);
                                        current_node.addAdjacency(adjNode);
                                        graph.addNode(adjNode);
                                    } else {
                                        if (graph.checkNode(node_features[j]) != null) {
                                            Node nodeFound = graph.checkNode(node_features[j]);
                                            current_node.addAdjacency(nodeFound);
                                            nodeFound.addAdjacency(current_node);
                                            graph.updateNode(nodeFound);
                                        }
                                    }
                                } else {

                                }
                            }
                        }
                        // Add a node to the graph after setting it
                        if (graph.checkNode(node_features[0]) == null) {
                            graph.addNode(current_node);
                        } else {
                            graph.updateNode(current_node);
                        }
                    }


                }
            }
        }
        //axisScale = axisScale * (graph.nodes.Count / 1000);
        //k_clusters = graph.nodes.Count / 100;
    }

    // Create Undirected Graph w/out Circles
    void InitializeUnGraphStructure() {
        if (InputData == InputType.StanfordSnap) {
            if (dataset != null) {
                // split each line
                if (graphData.Equals(String.Empty)) {
                    String s = dataset.text.Replace("\r", "\n");
                    lines = s.Split("\n"[0]);
                    print("loading previous dataset");
                } else {
                    graphData.Replace("\r", "\n");
                    lines = graphData.Split("\n"[0]);
                    print("loading menu dataset");
                }
                // loop through each line
                for (int i = 0; i < lines.Length; ++i) {
                    string[] node_features = lines[i].Split(new char[0]);
                    Node current_node = null;
                    //print(node_features[0] + " : current Node");
                    // if node does not exist in graph
                    if (graph.checkNode(node_features[0]) == null && 
                            node_features[0].Equals(String.Empty)==false) {
                        current_node = new Node(node_features[0], "");
                    } else {
                        // if node does exist in graph
                        current_node = graph.checkNode(node_features[0]);

                    }
                    if (current_node != null) {
                        // loop through each property of the text file
                        for (int j = 0; j < node_features.Length; ++j) {
                            //print(node_features[j]);
                            // first node is label
                            if (current_node.NodeID1.Equals(String.Empty)) {
                                print("found empty node " + i);
                            }
                            if (j == 0 || current_node.NodeID1.Equals(String.Empty)) {

                            } else {
                                // if cluster size is lesser than..
                                if (j < 1000) {
                                    if (graph.checkNode(node_features[j]) == null && node_features[j] != "") {
                                        Node adjNode = new Node(node_features[j], "");
                                        adjNode.addAdjacency(current_node);
                                        current_node.addAdjacency(adjNode);
                                        graph.addNode(adjNode);
                                    } else {
                                        if (graph.checkNode(node_features[j]) != null) {
                                            Node nodeFound = graph.checkNode(node_features[j]);
                                            current_node.addAdjacency(nodeFound);
                                            nodeFound.addAdjacency(current_node);
                                            graph.updateNode(nodeFound);
                                        }
                                    }
                                } else {

                                }
                            }
                        }
                        // Add a node to the graph after setting it
                        if (graph.checkNode(node_features[0]) == null) {
                            graph.addNode(current_node);
                        } else {
                            graph.updateNode(current_node);
                        }
                    }


                }
            }
        }
        //axisScale = axisScale * (graph.nodes.Count / 1000);


    }

    void SimpleClusters(VisualNode n) {
        // Get first adjacency method
        if(n.getNode().getAdjacencies().Count > 0 && n.distinct_node == true) {
            //print("running");
            clusterNodes.Add(n);
                    foreach(Node near_node in n.getNode().getAdjacencies()) {
                        VisualNode near_vis = nodes_in_scene[near_node];
                        // Add child nodes to given cluster node
                        n.getClusterNodes().Add(near_vis);
                        float x_n = UnityEngine.Random.Range(-clusterRadius,clusterRadius);
                        float y_n = UnityEngine.Random.Range(-clusterRadius,clusterRadius);
                        float z_n = UnityEngine.Random.Range(-clusterRadius,clusterRadius);
                        // Generate a midpoint between vectors
                        if(near_vis.getNode().getAdjacencies().Count > 1) {
                            Node secondAdj = near_vis.getNode().getAdjacencies()[0] as Node;
                            VisualNode getPos = nodes_in_scene[secondAdj];
                            Vector3 a = n.transform.position;
                            Vector3 b = getPos.transform.position;
                            float x = a.x + b.x;
                            float y = a.y + b.y;
                            float z = a.z + b.z;
                            Vector3 mid = new Vector3(x/2,y/2,z/2);
                            near_vis.transform.position = mid + new Vector3(x_n/6,y_n/10,z_n/6);
                        } else {
                            near_vis.transform.position = n.transform.position + new Vector3(x_n,y_n,z_n);  
                        }
                        near_vis.getNode().setVector3(near_vis.transform.position);
                    }              
            }
    }

    // Generation Spheres
    void generateVisualization() {
        int count = graph.nodes.Count;
        int index = 0;
        nodePositions = new Vector3[count];
        springNodes = new Rigidbody[count];
        // loop through nodePositions and generate vectors
        for(int i = 0; i < count; ++i) {
            float x = transform.position.x;
            float y = transform.position.y;
            float z = transform.position.z;
            
            nodePositions[i] = new Vector3(x + UnityEngine.Random.Range(-overallRadius*axisScale.x, overallRadius*axisScale.x),y + UnityEngine.Random.Range(-overallRadius*axisScale.y, overallRadius*axisScale.y), z + UnityEngine.Random.Range(-overallRadius*axisScale.z, overallRadius*axisScale.z));

        }

        // Go through each node create a VisualNode
        while(index < count) {
            Node nodebeingadded = (Node)graph.nodes[index];
                GameObject icosphere = Instantiate(node, nodePositions[index], transform.rotation);
                icosphere.transform.parent = transform;
                VisualNode n = icosphere.AddComponent<VisualNode>();
                n.setNode((Node)graph.nodes[index]);
                n.getNode().setVector3(nodePositions[index]);
                n.getNode().Transform = n.transform;
                // Adds positional information to node_info



                DrawLabel(icosphere.transform.position, n.getName(), 0, icosphere);
                icosphere.AddComponent<BoxCollider>();
                icosphere.AddComponent<Rigidbody>();
                springNodes[index] = icosphere.GetComponent<Rigidbody>();
                springNodes[index].constraints = RigidbodyConstraints.FreezeRotation;
                icosphere.transform.parent = this.transform;
                // add to array for reference later
                nodes_in_scene.Add((Node)graph.nodes[index], n);
                colombs_nodes.Add(icosphere);
            index++;
        }
    }

    // Draw label above node (monash)
    void DrawLabel(Vector3 pos, string textLabel, int id, GameObject parent) {
        if (textLabel != "") {
            // Get the shader for the text and create an new gameobject
            Shader shaderText = Shader.Find("GUI/Text Shader");
            GameObject label = new GameObject();
            LabelRotate l = label.AddComponent<LabelRotate>();

            // Calculate the vector position of the node
            Vector3 posL = pos + new Vector3(0f, 1, 0f);
            label.transform.position = pos;
            label.transform.parent = transform;

            // Add textmesh component to the gameobject
            TextMesh textMesh = label.gameObject.AddComponent<TextMesh>();
            textMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            MeshRenderer meshRenderer = label.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material.shader = shaderText;
            Color col = Color.white;
            meshRenderer.material.color = new Color(col.r,col.g,col.b,0.2f);
            textMesh.fontSize = 70;
            textMesh.transform.localScale = new Vector3(-0.05f, 0.05f, 0.05f); // flip x value, might be an issue w/ LabelRotate vectors?
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.text = textLabel;
            label.transform.position = posL;

            // add for reference
            labelsGameObjects.Add(label);
            parent.transform.name = textLabel;

            // make label a child to the given node
            label.transform.parent = parent.transform;
            if(TypeGraph == GraphType.Circles)
            label.SetActive(false);
            
        }
    }
    // Modified MONASH code, rearranged for adjacency lists
    void generateGraphTubes(Dictionary<Node,VisualNode> visual_nodes, float radius)
	{              
		foreach(VisualNode n in visual_nodes.Values)
		{
            Node node = n.getNode();
            for(int k = 0; k < node.getAdjacencies().Count; ++k){
                Node adjNode = (Node) node.getAdjacencies()[k];
			    EdgeLink currentEdge = new EdgeLink(node,adjNode);              
                if(HashEdges.Contains(currentEdge) == false) {
                    HashEdges.Add(currentEdge);

                    //print("created");
                    GameObject cyl = generateOrientatedCylinder(node.position,adjNode.position,radius);
                    //DynamicEdge d = cyl.AddComponent<DynamicEdge>();
                    //d.setTransforms(n.transform,n.transform);
                    cyl.name = node.NodeID1 + " to " + adjNode.NodeID1;
                } else {
                    doubleUps++;
                    
                }
            }
		}        
	}

    public void addEdges(Edge e) {
        edges.Add(e);
    }

    void generateRigEdges(Dictionary<Node,VisualNode> visual_nodes, float radius)
	{              
		foreach(GameObject g in colombs_nodes)
		{
            VisualNode n = g.GetComponent<VisualNode>();
            Node node = n.getNode();
            for(int k = 0; k < node.getAdjacencies().Count; ++k){
                Node adjNode = (Node) node.getAdjacencies()[k];
			    EdgeLink currentEdge = new EdgeLink(node,adjNode);
                VisualNode visualnode_adj = nodes_in_scene[adjNode];
                n.addAdjacency(visualnode_adj);
                bool t = true;           
                if(HashEdges.Contains(currentEdge) == false) {
                    HashEdges.Add(currentEdge);
                    //print("created");
                    GameObject edg = Instantiate(DynamicEdge,Vector3.zero,DynamicEdge.transform.rotation);
                    num_of_edges++;
                    DynamicEdge de = edg.GetComponent<DynamicEdge>();
                    edg.transform.parent = transform;
                    if(de.edgeType != global::DynamicEdge.EdgeType.LineRenderer){
                        edg.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = edgeColor;
                    }
                    de.setAnchor1(n.transform);             
                    de.setAnchor2(adjNode.Transform);
                    de.setMidAnchor(adjNode.Transform, n.transform);
                    de.setHashCode(currentEdge.GetHashCode());
                    de.setAdjCount(node.getAdjacencies().Count,adjNode.getAdjacencies().Count);
                    //DynamicEdge d = cyl.AddComponent<DynamicEdge>();
                    //d.setTransforms(n.transform,n.transform);
                    edg.name = node.NodeID1 + " to " + adjNode.NodeID1;
                } else {
                    doubleUps++;
                    
                }
            }
		}        
	}

    private GameObject generateOrientatedCylinder(Vector3 A, Vector3 B, float radius)
	{
		Vector3 midP =  (A + B) / 2f; 

		GameObject goCylAB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		goCylAB.transform.position = midP;
		
		
		Vector3 origVec = new Vector3(0, 1.0f, 0f);
		Vector3 targetVec = new Vector3();
		targetVec = B-A;
		
		var l = targetVec.magnitude/2f;

		goCylAB.transform.localScale = new Vector3(tubeWidth, l, tubeWidth);
		
		targetVec.Normalize();
		
		float angle = Mathf.Acos( Vector3.Dot(origVec,targetVec));
		
		Vector3 axis = new Vector3();
		axis = Vector3.Cross(origVec, targetVec);
		axis.Normalize();

		goCylAB.transform.localRotation = Quaternion.AngleAxis((angle)*Mathf.Rad2Deg,axis); 
		goCylAB.transform.parent = transform;
	
		//assign material
		EdgesMaterial.color = edgeColor;
		goCylAB.GetComponent<Renderer>().material = EdgesMaterial;
		
        // curve cylinder
        //goCylAB.GetComponent<MeshFilter>().mesh = edgeMesh;
        Mesh cylinder = goCylAB.GetComponent<MeshFilter>().mesh;
        Vector3[] v = cylinder.vertices;
        for(int i = 0; i < v.Length; i++) {
                //v[i] += Vector3.up * Time.deltaTime;
        }
        cylinder.vertices = v;
        cylinder.RecalculateBounds ();
		return goCylAB;
	}

    // Prints all graph nodes for debugging
    void printGraph() {
        // loop through and get each first node ID
        for (int i = 0; i < graph.nodes.Count; ++i) {
            Node n = graph.nodes[i] as Node;
            //print(n.getNodeID1());
        }
    }

    // distinguish nodes in a different format
    void DistinguishNodes() {
        ArrayList stringNodes = new ArrayList();
        ArrayList numberNodes = new ArrayList();
        int total = 0;
        if(TypeGraph == GraphType.UndirectedGraph) {
            //VisualNode vis = nodes_in_scene[graph.nodes[0] as Node];
            //vis.distinct_node = true;

        }
        foreach(VisualNode n in nodes_in_scene.Values) {
            total += 1;
            float number;
            // if can be parsed as a number           
            if (float.TryParse(n.getName(), out number)) {
                numberNodes.Add(n);

            } else {
                // if it can't then increment string nodes
                stringNodes.Add(n);
            }
        }
        // If the number is less significant, the node can be separated from the data mass
        //print(stringNodes.Count + "  " + numberNodes.Count);
        if(stringNodes.Count < numberNodes.Count) {
            foreach(VisualNode v in stringNodes) {
                v.DistinguishVisualNode(Color.blue, v.transform.localScale * 1f);
                v.distinct_node = true;
            }
        }
        if(numberNodes.Count < stringNodes.Count){
            foreach (VisualNode v in numberNodes) {
                v.DistinguishVisualNode(Color.blue, v.transform.localScale * 1f);
                v.distinct_node = true;
            }
        }
    }

    void RearrangeNodes() {
        foreach(VisualNode n in nodes_in_scene.Values) {
            //SimpleClusters(n);
            SimpleClusters(n);
        }
    }

    void ApplyColombsLaw() {
        int nodeCount = colombs_nodes.Count;
        for (int i = 0; i < nodeCount; ++i) {
            for (int j = i; j < nodeCount; ++j) {
                if (i == j && colombs_nodes[i] != null && colombs_nodes[j] != null)
                    continue;
                var g1 = colombs_nodes[i] as GameObject;
                var g2 = colombs_nodes[j] as GameObject;
                if (g1.GetComponent<Rigidbody>() && g2.GetComponent<Rigidbody>()) {
                    var Body1 = g1.GetComponent<Rigidbody>();
                    var Body2 = g2.GetComponent<Rigidbody>();

                    Vector3 d = Body1.position - Body2.position;
                    float distance = d.magnitude + 0.001f;
                    Vector3 direction = d.normalized;

                    if (distance < 5) {
                        var force = (direction * 0.005f) / (distance * distance * 0.1f);
                        Body1.AddForce(force * 0.1f);
                        Body2.AddForce(-force * 0.05f);
                    }
                }
            }
        }
        if (QuadrantLayout) {
            PolarCoord();
        }
    }

    // read attribute information and add to node
    void readAttributeData(TextAsset file) {
        print("executed");
        string attribute_text = file.text;

        if (attributeData.Equals(String.Empty) == false) {
            attribute_text = attributeData;
        }

        string[] str_lines = Regex.Split(attribute_text, "\n|\r|\r\n");
        string[] attribute_information = str_lines[0].Split(null);
        // loop through each line
        // skip initial attribute information in matrix
        for (int i = 1; i < str_lines.Length; ++i) {
            string[] node_features = str_lines[i].Split(null);
            string node_ID = node_features[0];
            Node n = graph.checkNode(node_ID);
            if (n != null) {
                VisualNode v_n = nodes_in_scene[n];
                string attributes = "";
                for (int j = 1; j < node_features.Length; ++j) {
                    if(node_features[j].Contains("0") == false && j < attribute_information.Length)
                    attributes += attribute_information[j] + " : \n" + node_features[j] + "\n";
                }
                v_n.setAttributeInfo(attributes);
            }
        }
    }

    // Save to text file on quitting the application
    void OnApplicationQuit() {
        CreateSaveFile();

    }

    // Create save file
    void CreateSaveFile() {
        string filename = @".\" + dataset.name + ".layout";
        if (!File.Exists(filename)) {           
            TextWriter writer = new StreamWriter(filename,true);
            foreach(GameObject g in colombs_nodes) {
                writer.WriteLine(g.GetComponent<VisualNode>().node_name +" "+g.transform.position.x + " "+g.transform.position.y+" "+ g.transform.position.z);
            }
            Transform camera = Camera.main.transform.parent;
            writer.WriteLine("position " + camera.position.x + " " + camera.position.y + " " + camera.position.z);
            writer.WriteLine("quaternion "+camera.rotation.x + " " + camera.rotation.y + " " + camera.rotation.z + " "+ camera.rotation.w);
            if (!File.Exists(filename)) {
                File.Create(filename).Close();
            }
            writer.Close();
        }
    }

    // Loads layout and sets positions for nodes
    void LoadLayout() {
        string filename = @".\" + dataset.name + ".layout";
        TextReader reader = new StreamReader(filename, true);
        String[] lines = File.ReadAllLines(filename);
        int index = 0;
        foreach(String s in lines) {
            String[] contents = s.Split(' ');
            // if node vector position
            if (index < lines.Length - 2) {
                String id = contents[0];
                float x = float.Parse(contents[1]);
                float y = float.Parse(contents[2]);
                float z = float.Parse(contents[3]);
                Node n = graph.checkNode(id);
                if (n != null) {
                    VisualNode v = nodes_in_scene[n];
                    v.transform.position = new Vector3(x, y, z);
                }
            }
            // camera position
            if (contents[0].Contains("position")) {
                String id = contents[0];
                float x = float.Parse(contents[1]);
                float y = float.Parse(contents[2]);
                float z = float.Parse(contents[3]);
                Camera.main.transform.parent.position = new Vector3(x, y, z);
            }
            // camera rotation
            if (contents[0].Contains("quaternion")) {
                String id = contents[0];
                float x = float.Parse(contents[1]);
                float y = float.Parse(contents[2]);
                float z = float.Parse(contents[3]);
                float w = float.Parse(contents[4]);
                Camera.main.transform.parent.rotation = new Quaternion(x, y, z, w);
            }
            index++;
        }
        if (TypeGraph == GraphType.Circles)
        {
            CombineMeshes();
        }
        else
        {
            KMeansClustering();
        }
    }

    void KMeansClustering() {

        print("k-means started..");

        // calculate centroid points
        int k_means = k_clusters;
        int[] k_calculations = new int[k_means];
        for(int i = 0; i < k_means; i++) {
            decimal f = Decimal.Divide(i , k_means);
            float division = (float)f;
            k_calculations[i] = Mathf.RoundToInt(colombs_nodes.Count * (division));
            print("k means calculation: "+ f + " = " + i + " / " + k_means);
        }
        print("step 0, k_means = "+k_means);
        int k_num = 0;
        foreach(int num in k_calculations) {
            //print(k_num +" : "+num);
            k_num++;
        }

        // Assigning centroids
        GameObject[] centroids = new GameObject[k_means];
        VisualNode[] vs_c = new VisualNode[k_means];
        ArrayList[] clusternodes = new ArrayList[k_means];
        for (int i = 0; i < k_means; i++) {
            centroids[i] = (GameObject)colombs_nodes[k_calculations[i]];
            vs_c[i] = centroids[i].GetComponent<VisualNode>();
            vs_c[i].distinct_node = true;
            Vector3 localScale = vs_c[i].transform.localScale;
            if (transform.localScale.x < 2) {
                //vs_c[i].transform.localScale = new Vector3(localScale.x * 2, localScale.y * 2, localScale.z * 2);
            }
            clusternodes[i] = new ArrayList();
            clusternodes[i].Add(vs_c[i]);
        }
        print("step 1");

        // Assigning nodes to centroids on Euclidean Distance
        foreach (GameObject g in colombs_nodes) {
            if(centroids.Contains(g) == false) {
                VisualNode n = g.GetComponent<VisualNode>();
                float[] dist_array = new float[k_means];
                for(int i = 0; i < dist_array.Length; i++) {
                    dist_array[i] = Vector3.Distance(g.transform.localPosition, centroids[i].transform.localPosition);
                }
                float smallest_dist = dist_array.Min();
                int smallest_index = 0;
                for(int i = 0; i < dist_array.Length; i++) {
                    if(dist_array[i] == smallest_dist) {
                        smallest_index = i;
                    }
                }
                clusternodes[smallest_index].Add(n);
            }
        }
        print("step 2");

        // Create Cluster Objects
        GameObject[] clusterObjects = new GameObject[k_means];
        for(int i = 0; i < k_means; i++) {
            clusterObjects[i] = new GameObject();
            clusterObjects[i].AddComponent<ClusterBehaviour>();
            clusterObjects[i].GetComponent<ClusterBehaviour>().setArrayList(clusternodes[i]);
            clusterObjects[i].GetComponent<ClusterBehaviour>().setClusterCentre(centroids[i]);
        }
        print("step 3");

        int cluster_index = 0;
        // Go through the three centroids and generate the clusters
        foreach (GameObject cluster_object in clusterObjects) {

            ClusterBehaviour clu = cluster_object.GetComponent<ClusterBehaviour>();
            // return nodes of cluster c
            ArrayList cluster_array = clu.returnClusterNodes();
            // setup meshfilters for child objects
            MeshFilter[] meshFilters = new MeshFilter[clu.returnClusterNodes().Count + 1];
            // combine instance mesh array
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            // Color of cluster
            Color col = UnityEngine.Random.ColorHSV(0.1f, 0.8f, 0.7f, 1f, 0.5f, 1f);


            // go through each child node and add to meshFilter array
            // first cluster node
            GameObject first_node = clu.gameObject;
            meshFilters[0] = centroids[cluster_index].GetComponent<MeshFilter>();
            combine[0].mesh = meshFilters[0].sharedMesh;
            combine[0].transform = meshFilters[0].transform.localToWorldMatrix;
            meshFilters[0].gameObject.SetActive(false);
            nodes_already_clustered.Add(first_node.GetComponent<VisualNode>());
            // add boxcollider for each node
            BoxCollider b = cluster_object.AddComponent<BoxCollider>();
            b.size = new Vector3(0.05f, 0.05f, 0.05f);
            b.center = clu.transform.position;
            b.isTrigger = true;

            /*
            ArrayList edge = n.getAllocatedEdges();
            if (edge.Count > 0) {
                GameObject index0 = edge[0] as GameObject;
                foreach (GameObject ga in edge) {
                    LineRenderer l = ga.GetComponent<LineRenderer>();
                    l.material.SetColor("_TintColor", new Color(col.r, col.g, col.b, 0.01f));
                }
            }
            */

            int k = 1;
            foreach (VisualNode v in clu.returnClusterNodes()) {

                if (nodes_already_clustered.Contains(v) == false) {
                    GameObject g = v.gameObject;
                    meshFilters[k] = g.GetComponent<MeshFilter>();
                    combine[k].mesh = meshFilters[k].sharedMesh;
                    combine[k].transform = meshFilters[k].transform.localToWorldMatrix;
                    meshFilters[k].gameObject.SetActive(false);
                    //nodes.Add(v);
                    nodes_already_clustered.Add(v);
                    // add boxcollider for each node
                    BoxCollider b1 = cluster_object.AddComponent<BoxCollider>();
                    b1.size = new Vector3(0.05f, 0.05f, 0.05f);
                    b1.center = g.transform.position;
                    b1.isTrigger = true;
                    ArrayList edge1 = v.getAllocatedEdges();
                    v.transform.parent = cluster_object.transform;
                    vs_c[cluster_index].addClusterNode(v);

                    if (edge1.Count > 0) {
                        GameObject index0 = edge1[0] as GameObject;
                        
                        foreach (GameObject ga in edge1) {
                            //LineRenderer l = ga.GetComponent<LineRenderer>();
                            //l.material.SetColor("_TintColor", new Color(col.r, col.g, col.b, 0.01f));
                        }
                    }
                } else {
                }
                k++;

            }

            // set the parent of the cluster to visualization object
            cluster_object.transform.parent = this.transform;
            // set the name of the cluster
            //cluster_object.transform.name = first_node.transform.name + " cluster";

            // add meshfilter to new cluster 
            cluster_object.AddComponent<MeshFilter>();
            cluster_object.AddComponent<MeshRenderer>();
            cluster_object.GetComponent<MeshFilter>().mesh = new Mesh();
            // combine all the meshes in the mesh array "combine"
            cluster_object.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            cluster_object.GetComponent<MeshRenderer>().material = nodeMat;
            // assign random color to cluster
            cluster_object.GetComponent<MeshRenderer>().material.color = col;
            clusters.Add(clu);
            cluster_index++;


        }
		EdgeManager edgeManager = Camera.main.GetComponent<EdgeManager> ();
		ArrayList glEdges = edgeManager.getEdges();
		print (glEdges.Count + "glEdge count");
		for(int i = 0; i < glEdges.Count; i ++) {
			Edge e = (Edge)glEdges[i];
			e.setColor(e.getTransformRef().parent.GetComponent<MeshRenderer>().material.color);
        }
        print("step 4 DONE");
        clusterAllNodes = true;
        graph_layout_recognized = false;

    }

    // Add a node to ignore when updating a cluster
    public void addIgnoreNode(VisualNode v) {
        ignoreNodes.Add(v);
        v.gameObject.SetActive(false);
        v.setCurrentlyHidden(true);
    }
    // Remove a node to ignore when updating a cluster
    public void removeIgnoreNode(VisualNode v) {
        ignoreNodes.Remove(v);
        v.gameObject.SetActive(true);
        v.setCurrentlyHidden(false);
    }
    // Return the ignore nodes
    public ArrayList getIgnoreNodes() {
        return ignoreNodes;
    }

    public bool graphIsClustered() {
        return clusterAllNodes;
    }

    public ArrayList returnGraphNodes() {
        return colombs_nodes;
    }

    public ArrayList returnClusterNodes() {
        return clusters;
    }

    private void PolarCoord() {
        foreach(GameObject g in colombs_nodes) {
            float radius = 2.8f;
            float theta = 2 * Mathf.PI * 0 - Mathf.Acos(g.transform.position.x / radius);
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            g.transform.position = new Vector3(x, g.transform.position.y, z);
            g.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public VisualNode getNode(String node) {
        Node n = new Node(node,"");
        VisualNode vis = nodes_in_scene[n];
        return vis;
    }

}
