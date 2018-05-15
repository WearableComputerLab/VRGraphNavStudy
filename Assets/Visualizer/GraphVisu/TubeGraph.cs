using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class TubeGraph : MonoBehaviour {

    public Mesh edgeMesh;
	public Material NodesMaterial;
	public Material EdgesMaterial;

	public float scaleSphere = 15f;
	public float tubeWidth = 0.10f;
	private Mesh mesh;
	//public float graphScaleFactor = 0.1f;

	public Color nodeColor = Color.blue;
	public Color edgeColor= Color.red;

	private List<Vector3> allVertices = new List<Vector3>();
	private List<int> allIndexes = new List<int>();

	private List<GameObject> selectedEdges = new List<GameObject>();
	private GameObject selectedNode;

	private List<GameObject> labelsGameObjects = new List<GameObject>();

	//size Oculus Rift = 0.009f*1.15f
	// 0.01035
	//size CAVE2 = 0.07f*1.15f
	// 0.0805

	float scaleGraph = 0.01035f;

	//centroid of the graph
	Vector3 centroid = Vector3.zero;

	public bool interactiveEditing = false;
	public bool viewerMode = false;

	//graph data structures
	private List<int> adj = new List<int>();
	private List<Vector3> verticesToLink = new List<Vector3>();
	private List<GameObject>[] nodeEdges; // = new List<GameObject>[]();

	private bool canEditNode = false;

	private Vector3 Vector3Unit = Vector3.one; // new Vector3(1.0f,1.0f,1.0f);
	private List<int> path = new List<int>();
	private List<List<int>> triangles = new List<List<int>>();

	public static int currentGraph = 0;
	public static bool transitionGraph = false;
	public static bool updateGraph = false;
	private Quaternion myRotation = Quaternion.identity;

	private List<GameObject> edgesPath = new List<GameObject>();
	private bool illuminatedPath = false;

	public GameObject cameraLeft = null;

	//data graph
	public int chainLength = 17;


	//============= geometry UNITY : generated orientated tubes for edges =========

	//creates an orientated tube

	void generateGraphTubes(List<Vector3> vertices, List<int> adjency, float radius)
	{

		for(int i=0;i<adjency.Count; i+=2)
		{
			int index0 = adjency[i];
			int index1 = adjency[i+1];

			Vector3 A = vertices[index0];
			Vector3 B = vertices[index1];

			GameObject cyl = generateOrientatedCylinder(A,B,radius);

			// edge name is pair of nodes
			cyl.name = "("+index0.ToString()+","+index1.ToString()+")";

			nodeEdges[index0].Add(cyl);
			nodeEdges[index1].Add(cyl);
		}
	}


	// ========================= CLEAN / LOAD graph game objects ===============================

	private void cleanGraphObjects()
	{
		List<GameObject> graphComponents = new List<GameObject>();
		foreach (Transform child in transform) {
			graphComponents.Add(child.gameObject);
		}
		graphComponents.ForEach(child => Destroy(child));

		if(adj!=null)
		adj.Clear();
		if(verticesToLink!=null)
		verticesToLink.Clear();
		//List<GameObject>[] nodeEdges;
		//nodeEdges = null;
		if(path!=null)
		path.Clear(); // = new List<int>();

		if(triangles !=null)
		triangles.Clear();

	}

	void loadAGraph(int graph)//string positionsBinFile, string matrixBinFile)
	{
		//clean every object
		//cleanGraphObjects();

		//reset the scale
		transform.localScale = Vector3.one;

		//reset the orientation
		transform.rotation = Quaternion.identity;

		//reset the translation
		transform.position = Vector3.zero;

		if(interactiveEditing)
		loadDataFromTextFile(graph+".txt");
		else
			loadDataFromBinFile("Graph"+graph+".bin");//"positions"+graph+".bin","matrix"+graph+".bin","path"+graph+".bin","rotation"+graph+".bin");

		//get centroid
		centroid = UtilMath.getCentroid(verticesToLink);
		
		//offset of the graph game object position
		GameObject centroidGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		centroidGO.GetComponent<Renderer>().material.color = Color.red;
		centroidGO.transform.position = centroid;
		centroidGO.transform.localScale = new Vector3(scaleSphere,scaleSphere,scaleSphere);
		centroidGO.transform.parent = transform;
		
		//scale the graph
		transform.localScale = transform.localScale * scaleGraph;

		//recenter it ...
		transform.Translate(-centroidGO.transform.position);
		
		if(!interactiveEditing)
		{
			//disable centroid ....
			centroidGO.SetActive(false);
		}

		//rotate it correctly ...
		float angle = 0.0F;
		Vector3 axis = Vector3.zero;
		myRotation.ToAngleAxis(out angle, out axis);

		transform.RotateAround(Vector3.zero, axis,angle);

		edgesPath.Clear();

		//scan the path
		if(path!= null)
		scanEdgesNames(path);

		//scan the tris
		foreach (var item in triangles) {
			scanEdgesNames(item);	
		}

	
	}

	/*=========================================== UNITY GEOMETRY PRIMITIVE BASED FUNCTTIONS ==============================*/

	private void scanEdgesNames(List<int> edges)
	{
		for(int i=0;i<edges.Count-1;i++)
		{
			string nameOfEdge = "("+edges[i].ToString()+","+edges[i+1].ToString()+")";
			
			GameObject currentEdge = GameObject.Find(nameOfEdge);

			if(currentEdge == null)
			{
				//find the reverse name ...
				nameOfEdge = "("+edges[i+1].ToString()+","+edges[i].ToString()+")";
				currentEdge = GameObject.Find(nameOfEdge);
				
			}

			edgesPath.Add(currentEdge);
		}

	}

	private void illuminatePath(Color illuminationColor)
	{
		if(edgesPath != null)
		//get all edges in the path
		foreach (var item in edgesPath) {
			if(item!=null)
				item.GetComponent<Renderer>().material.color = illuminationColor;
		}
	}

	private void orientateCylinderGameObject(Vector3 A, Vector3 B, GameObject goCylAB)
	{
		Vector3 midP =  (A + B) / 2f; 

		goCylAB.transform.position = midP;

		Vector3 origVec = Vector3.up;  //new Vector3(0, 1.0f, 0f);
		Vector3 targetVec = new Vector3();
		targetVec = B-A;
		
		var l = targetVec.magnitude/2f;

		Vector3 localScale = goCylAB.transform.localScale;
		localScale.y = l *1f/scaleGraph ; //* 1f/(scaleGraph);
		goCylAB.transform.localScale = localScale ; // = new Vector3(tubeWidth, l, tubeWidth);
		
		targetVec.Normalize();
		
		float angle = Mathf.Acos( Vector3.Dot(origVec,targetVec));
		
		Vector3 axis = new Vector3();
		axis = Vector3.Cross(origVec, targetVec);
		axis.Normalize();

		goCylAB.transform.rotation = Quaternion.AngleAxis((angle)*Mathf.Rad2Deg,axis);
		//goCylAB.transform.localRotation = Quaternion.AngleAxis((angle)*Mathf.Rad2Deg,axis); 
		goCylAB.transform.parent = transform;	

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

	/*=========================================== CREATE FROM DATASET ===================================================*/
	
	List<int> createLineAdjency(int[,] matrix)
	{
		List<int> adjencyList = new List<int>();

		for(int i=0; i < matrix.GetLength(0) ;i++)
		{
			for(int j=0; j < matrix.GetLength(1);j++)
			{
				if(matrix[i,j] == 1 && i>j )
				{
					//therese is a connection between vertex at pos i and vertex at pos i
					adjencyList.Add(i);
					adjencyList.Add(j);
				}
			}

		}
		return adjencyList;
	}

	void DrawNodeAndLabel (Vector3 pos, string textLabel, int id)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.name = id.ToString();

		go.AddComponent<Rigidbody>();
		go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

		go.AddComponent<MyCollisionHandler>();

		go.transform.position = pos;

		go.GetComponent<Renderer>().material = NodesMaterial;
		go.transform.localScale = new Vector3(scaleSphere,scaleSphere,scaleSphere);
		go.transform.parent = transform;

		//check if current node is start or end of path

	
		if(path!=null)
		{
		if(path[0] == id || path[path.Count-1]==id)
			go.GetComponent<Renderer>().material.color = Color.green;
		else
			go.GetComponent<Renderer>().material.color = nodeColor;
		}


		//label
		//generate a game object with label if not empty string
		if(textLabel != "")
		{
		Shader shaderText = Shader.Find("GUI/Text Shader");
		GameObject label = GameObject.CreatePrimitive(PrimitiveType.Quad);
		
		Vector3 posL = pos + new Vector3(0f,1.6f,-1.5f);

		label.transform.position = pos;
		 
		label.transform.parent = transform;

		var textMesh = label.gameObject.AddComponent<TextMesh>();
		//textMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		var meshRenderer = label.gameObject.GetComponent<MeshRenderer>();
		
		//meshRenderer.material = textMesh.font.material; //load "font material" Material newMat = Resources.Load("DEV_Orange", typeof(Material)) as Material;
		meshRenderer.material.shader = shaderText;
		meshRenderer.material.color = Color.black;
		//textMesh.fontSize = 12;
		//textMesh.alignment = TextAlignment.Center;
		//textMesh.anchor = TextAnchor.MiddleCenter;
		//textMesh.text = textLabel;

		label.transform.position = posL;
		labelsGameObjects.Add(label);
		//label.transform.localScale = new Vector3(scaleSphere,scaleSphere,scaleSphere);
		}
	}

	void createTubesFromDataUnityGeometry(float radius, List<Vector3> _positions, int[,] matrixAdjency, bool fromBinary)
	{
		//init node edge list strutcture
		nodeEdges = new List<GameObject>[_positions.Count];

		if(nodeEdges!=null)
		for (int i=0;i<nodeEdges.Length;i++)
			nodeEdges[i] = new List<GameObject>();

		if(!fromBinary) //in binary file, adjency has been calculated already
		adj = createLineAdjency (matrixAdjency);

		for (int i=0; i<_positions.Count; i++) 
		{
			if(!fromBinary)
			{	
				verticesToLink.Add(_positions[i]);
			}

			DrawNodeAndLabel(_positions[i], i.ToString(),i );

		}
		generateGraphTubes(verticesToLink,adj,radius);

	}


	// ================================================ CALLBACKS ==========================================
	// =================================== INTERACTION WITH GRAPH ==========================================

	void HandleOnGraphChange (int graph)
	{
		Debug.Log(">> load " + graph);
		loadAGraph(graph);
	}

	void HandleOnExitEdit()
	{
		canEditNode = false;

	}

	void HandleOnNodeExited (int nodeId)
	{
		canEditNode = false;

		foreach(GameObject cyl in selectedEdges)
		{
			cyl.GetComponent<Renderer>().material.color = edgeColor;
		}
	}

	void HandleOnNodeEditing (int nodeId, Vector3 pos)
	{
		if(canEditNode)
		foreach (GameObject cylEd in selectedEdges)
		{
			//need to retrieve each end of each cylinders here ...

			// 1 -> decode the name of the edge
			string edgeName = cylEd.name;
			string[] verts = edgeName.Split(new char[]{'(',')',','});

			string end = "";

			foreach (string ind in verts) {
			if (ind != nodeId.ToString() && (ind!=""))
					end = ind;
			}

			GameObject sphereEnd = GameObject.Find(end);
			Vector3 posEnd = sphereEnd.transform.position;

			orientateCylinderGameObject(posEnd, pos, cylEd);
			selectedNode.transform.position = pos;

			verticesToLink[nodeId] =  pos * 1.0f/scaleGraph + centroid;

		}
	}

	void HandleOnNodeEntered (int nodeId)
	{
		canEditNode = true;
		selectedNode = GameObject.Find(nodeId.ToString());


		//selectedEdges.Clear();
		selectedEdges = nodeEdges[nodeId];
		
		foreach(GameObject cyl in selectedEdges)
		{
			cyl.GetComponent<Renderer>().material.color = Color.green;
		}
	}

	/*========================================== PARSING FUNCS ====================================================*/

	List<Vector3> convertPositions (string positionsString)
	{
		string[] positionS = positionsString.Split('\t');

		List<Vector3> result = new List<Vector3>();

		for (int i=0;i<positionS.Length-3;i+=3)
		{
			float x= float.Parse(positionS[i]);
			float y = float.Parse(positionS[i+1]);
			float z = float.Parse(positionS[i+2]);

			result.Add(new Vector3(x,y,z));
		}

		return result;
	}

	List<Vector3> scaleAndTranslatePositions(List<Vector3> sourcePositions, float scale, Vector3 translatePosition)
	{
		List<Vector3> scaledlist = new List<Vector3>();

		foreach (Vector3 item in sourcePositions)
		{
			Vector3 temp = item;

			temp -= translatePosition;
			temp = item * scale;
			 

			scaledlist.Add(temp);
		}

		return scaledlist;
	}

	int[,] convertAdjencyMatrix(string matrix, int dimension)
	{
		int[,] value = new int[dimension,dimension];
		string[] rows = matrix.Split(';');

		for(int i=0;i<dimension;i++)
		{
			string[] rowsValues = rows[i].Split(',');

			for(int j=0; j<dimension; j++)
			{
				value[i,j] = int.Parse(rowsValues[j]);
			}
		}

		return value;
	}

	void debugMatrix(int[,] value)
	{
		for(int i=0;i<value.GetUpperBound(0)/10;i++)
			for(int j=0; j<value.GetUpperBound(0)/10;j++)
		{
			Debug.Log(value[i,j]);
		}
	}

	/*========================================== DEBUG FUNC ====================================================*/

	GameObject debugCube(Vector3 pos)
	{
		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cube.transform.position = pos;
		cube.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		return cube;

	}

	void debugGenericList(ICollection l)
	{
		string outputdebug = "";
		
		foreach (var id in l)
			outputdebug+= id.ToString() + " , ";
		
		Debug.Log (outputdebug);
		Debug.Log ("=====================================================================================");
	}

	void showPathLength()
	{
		GameObject go = GameObject.Find("PathLabel");
		if(path != null)
		{
		int pathLength = path.Count-1;
		string disp = "path length: "+pathLength.ToString();
		if(go!=null)
		go.GetComponentInChildren<TextMesh>().text = disp; 
		}
	}

	// = "testSerializedStuff.foo"

	// =========================================== EXPERIMENT SECTION ====================================================
	// =========================================== LOADING FUNCTIONS =====================================================

	void loadGraphFromBinaryFile(string graphFileName)
	{
		UnitySerializer theSerializer = new UnitySerializer();

		byte[] data = theSerializer.LoadFromFile(graphFileName);

		//1 read nb positions
		int nbPos = theSerializer.DeserializeInt(data);
//		Debug.Log ("nb positions to read : " + nbPos);
		//read nbPos Vector3
		for(int i=0;i<nbPos;i++)
		{
			Vector3 t = theSerializer.DeserializeVector3(data);
			verticesToLink.Add(t);
		}

		int nbElements = theSerializer.DeserializeInt(data);

//		Debug.Log ("nb elemts in matrix to read : " + nbElements);

		for(int i=0;i<nbElements;i++)
		{
			int res = theSerializer.DeserializeInt(data);
			adj.Add(res); // = ;
		}

		int nbPaths = theSerializer.DeserializeInt(data);

		for(int i=0;i<nbPaths;i++)
		{
			int res = theSerializer.DeserializeInt(data);
			path.Add(res); // = ;
		}

		int nbTris = theSerializer.DeserializeInt(data);

		List<int> verticesTris = new List<int> ();

		for(int i=0;i<nbTris;i++)
		{
			int res = theSerializer.DeserializeInt(data);
			verticesTris.Add(res);
		}

		//reconstruct the tris ...
		for(int i=0; i<verticesTris.Count;i+=3)
		{
			List<int> localTri = new List<int>();

			localTri.Add(verticesTris[i]);
			localTri.Add(verticesTris[i+1]);
			localTri.Add(verticesTris[i+2]);

			triangles.Add(localTri);
		}

		//read the transform
		myRotation = theSerializer.DeserializeQuaternion(data);
	}

	void loadGraphFromBinFile(string positionsFileName, string matrixFileName, string pathFileName, string rotationFileName)
	{
		//load saved graph values
		UnitySerializer usPositions = new UnitySerializer();
		UnitySerializer usMatrix = new UnitySerializer();
		UnitySerializer usPath = new UnitySerializer();
		UnitySerializer usRotation = new UnitySerializer();

		byte[] dataPositions = usPositions.LoadFromFile(positionsFileName);
		byte[] dataMatrix = usMatrix.LoadFromFile(matrixFileName);
		byte[] dataPath = usPath.LoadFromFile(pathFileName);
		byte[] dataRotation = usRotation.LoadFromFile(rotationFileName);

		verticesToLink = getListV3fromBinaryData(dataPositions,usPositions);
		adj = getIntListFromBinaryData(dataMatrix,usMatrix);
		path = getIntListFromBinaryData(dataPath,usPath);
		myRotation = usRotation.DeserializeQuaternion(dataRotation);

	}
	
	List<Vector3> getListV3fromBinaryData(byte[] data, UnitySerializer us)
	{
		List<Vector3> resL = new List<Vector3>();

		for(int i=0;i<data.Length/12;i++)
		{
			Vector3 t = us.DeserializeVector3(data);
			resL.Add(t); // = ;
			//s+=t.ToString()+"\n";
			//Debug.Log(t);
		}
		//Debug.Log(s);
		return resL;
	}

	List<int> getIntListFromBinaryData(byte[] data, UnitySerializer us)
	{
		List<int> result = new List<int>();

		//UnitySerializer usMatrix = new UnitySerializer();

		for(int i=0;i<data.Length/4;i++)
		{
			int res = us.DeserializeInt(data);
			result.Add(res); // = ;
		}

		return result;
	}

	void saveGraphToBinaryFile(int graph)
	{
		UnitySerializer theSerializer = new UnitySerializer();

		//serialize nb position
		int nbPos = verticesToLink.Count;
		theSerializer.Serialize(nbPos);

		//serialize positions
		foreach (Vector3 v3 in verticesToLink) {
			
			theSerializer.Serialize(v3);
		}

		int nbElement = adj.Count;
		theSerializer.Serialize(nbElement);

		//serialize matrix connectivity
		foreach(int matElement in adj)
		{
			theSerializer.Serialize(matElement);
		}

		int nbPath = path.Count;
		theSerializer.Serialize(nbPath);
		//serialize the path
		foreach (int p in path) {
			theSerializer.Serialize(p);
		}

		int nbtris = triangles.Count*3;
		theSerializer.Serialize(nbtris);

		//serializee the triangles
		foreach (List<int> tris in triangles) {
			foreach (var vert in tris) {
				theSerializer.Serialize(vert);	
			}

		}

		//serialize the transform
		theSerializer.Serialize(transform.rotation);

		//save the bytes in a file
		theSerializer.SaveInFile("Graph"+graph+".bin");


	}

	void saveGraphToBinFile(int graph)
	{
		// save verticesToLink
		UnitySerializer usPositions = new UnitySerializer();
		UnitySerializer usMatrix = new UnitySerializer();
		UnitySerializer usPath = new UnitySerializer();
		UnitySerializer usRotation = new UnitySerializer();

		foreach (Vector3 v3 in verticesToLink) {
		
			usPositions.Serialize(v3);
		}

		foreach(int matElement in adj)
		{
			usMatrix.Serialize(matElement);
		}

		foreach (int p in path) {
			usPath.Serialize(p);
		}

		usRotation.Serialize(transform.rotation);

		usPositions.SaveInFile("positions"+graph+".bin");
		usMatrix.SaveInFile("matrix"+graph+".bin");
		usPath.SaveInFile("path"+graph+".bin");
		usRotation.SaveInFile("rotation"+graph+".bin");
	}

	List<List<int>> convertTriangleList(string tris)
	{
		string[] nodes = tris.Split(',');
		List<List<int>> returnList = new List<List<int>>();

		for(int i=0;i<nodes.Length;i+=3)
		{
			List<int> localTriangle = new List<int>();
			localTriangle.Add(int.Parse(nodes[i]));
			localTriangle.Add(int.Parse(nodes[i+1]));
			localTriangle.Add(int.Parse(nodes[i+2]));
			returnList.Add(localTriangle);
		}

		return returnList;
	}
	
	List<int> convertPath (string str)
	{
		string[] nodes = str.Split(',');
		List<int> toReturn = new List<int>();
		
		for (int i = 0; i < nodes.Length; i++) {
			toReturn.Add(int.Parse(nodes[i]));
		}
		
		return toReturn;
		
	}

	private void disconnectNodes(ref int[,] matrix, int i, int j)
	{
		matrix[i,j]=0;
	}

	void loadDataFromTextFile(string file)
	{
		//open file
		string positionsAndMatrix = UtilMath.openFile(file); //@"C:\Users\maxc\Documents\OculusCave Design\ExpeGraphs\nodesMatrix0.txt");
		
		string[] positionsAndMatrixSplit = positionsAndMatrix.Split('#');

		List<Vector3> positionsGraph = convertPositions(positionsAndMatrixSplit[0]);

		int[,] adjencyMatrix = convertAdjencyMatrix(positionsAndMatrixSplit[1],positionsGraph.Count);

		if(interactiveEditing)
		{
		disconnectNodes(ref adjencyMatrix, 0, chainLength);
		disconnectNodes(ref adjencyMatrix, chainLength, 0);
		}

		//get the path
		if(positionsAndMatrixSplit[2].Length >0)
		path = convertPath(positionsAndMatrixSplit[2]);

		//triangles
		//test length of the split file to see if there are triangles ...
		if(positionsAndMatrixSplit.Length > 3)
		{
			//read the triangles
			triangles = convertTriangleList(positionsAndMatrixSplit[3]);
		}

		createTubesFromDataUnityGeometry(scaleSphere,positionsGraph,adjencyMatrix,false);

	}
	
	void loadDataFromBinFile(string fileName) //string positionsFile, string adjencyFile, string pathFile, string rotationFile)
	{
		//open file
		loadGraphFromBinaryFile(fileName);
		//loadGraphFromBinFile(positionsFile,adjencyFile,pathFile,rotationFile);
		createTubesFromDataUnityGeometry(scaleSphere, verticesToLink, null, true); // positionsGraph,adjencyMatrix,true);
	}


	// ================================ UNITY FUNCTIONS ============================================
	// =============================================================================================

	// Use this for initialization
	void Start () {

		//init the data structure...
		verticesToLink = new List<Vector3>();

		loadAGraph(0);

		//Listen to graph change events for the expriment
		//ExperimentController.OnGraphChange+= HandleOnGraphChange;
		//UDPReceive.OnNextGraph += HandleOnNextGraph;
		//add the listeners if the graph is interactive ...

		if(interactiveEditing)
		{
			//listen to collisions events ...
			MyCollisionHandler.OnNodeEntered += HandleOnNodeEntered;
			MyCollisionHandler.OnNodeEditing += HandleOnNodeEditing;
			MyCollisionHandler.OnNodeExited+= HandleOnNodeExited;
//			EditorNode.OnExitEdit += HandleOnExitEdit;
			
		}

		//recenter the headset
	//	OVRManager.display.RecenterPose();

	}

	void HandleOnNextGraph (int nextGraph)
	{
		loadAGraph(nextGraph);
	}

	Vector3 up = new Vector3(0f,0f,1f);

	// Update is called once per frame
	void Update () {
	
		//save node positions...
		//1 : Get all the spheres
		//2 : save X,Y,Z

		//billboard the labels
		foreach (GameObject item in labelsGameObjects) {

			item.transform.LookAt(item.transform.position + cameraLeft.transform.rotation * Vector3.forward,
			                 cameraLeft.transform.rotation * Vector3.up);

			//item.transform.RotateAround(Vector3.up,-180f);
			
		}
		if(transitionGraph)
		{
			//clean every object
			cleanGraphObjects();

			transitionGraph = false;
		}

		//load a graph if required ...
		if(updateGraph)
		{
		//disable anspad if enabled
		
		GameObject go = GameObject.Find("DigiPad");
		if(go != null)
		go.SetActive(false);
			//go.GetComponentInChildren<TextMesh>().renderer.material.color = Color.yellow;
		//go.GetComponentInChildren<TextMesh>().text = "Answer: " ;
		
		
		loadAGraph(currentGraph);
		updateGraph = false;
		}

		if(interactiveEditing)
		{

		if(Input.GetKeyDown(KeyCode.S))
		{
				//saveGraphToBinFile(currentGraph);
				saveGraphToBinaryFile(currentGraph);
		}
		

		}

		if(viewerMode || interactiveEditing)
		{
		if(Input.GetKeyDown(KeyCode.RightControl))
		{
			illuminatedPath = !illuminatedPath;
			if(illuminatedPath)
				illuminatePath(Color.yellow);
			else
				illuminatePath(edgeColor);
		}

			//TODO : remove for expe ...
			if(Input.inputString == "0" || 
			   Input.inputString == "1" ||
			   Input.inputString == "2" ||
			   Input.inputString == "3" ||
			   Input.inputString == "4" ||
			   Input.inputString == "5" ||
			   Input.inputString == "6" ||
			   Input.inputString == "7" ||
			   Input.inputString == "8" ||
			   Input.inputString == "9")
			{
				cleanGraphObjects();
				currentGraph = int.Parse(Input.inputString);
				
				loadAGraph(currentGraph);
				showPathLength();
			}
		}


	
	}
}
