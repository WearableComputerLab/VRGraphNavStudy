using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEdge : MonoBehaviour {

    public enum EdgeType { NormalRig, CurvedRig, LineRenderer }
    public EdgeType edgeType;
    Transform anchor1, anchor2, anchorMid;
    public int hashCode = 0;

    // for physics displacement
    public Rigidbody r1, r2;
    public float Length = 2f;
    public float EdgeLength;
    public float SpringK = 12.2f;
    public Vector3 force, force2;
    public float force_ns, acting_force;
    public int exe_force = 0;
    public int adjCount1, adjCount2;
    // stop checking time
    public bool stopTimeCheck = false;
    // line renderer
    public LineRenderer renderEdge;
    public Color def_color;
    float tint_alpha;
	private EdgeManager edgeManager;

    private void Awake() {
        if (edgeType == EdgeType.NormalRig) {
            anchor1 = transform.GetChild(0).GetChild(0);
            anchor2 = transform.GetChild(0).GetChild(0).GetChild(0);
        }
        if (edgeType == EdgeType.CurvedRig) {
            anchor1 = transform.GetChild(0).GetChild(0);
            anchorMid = transform.GetChild(0).GetChild(0).GetChild(0);
            anchor2 = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        }
		edgeManager = Camera.main.GetComponent<EdgeManager>();
    }
    private void Start() {
        if (edgeType != EdgeType.LineRenderer) {
            r1 = anchor1.parent.GetComponent<Rigidbody>();
            r2 = anchor2.parent.GetComponent<Rigidbody>();

        } else {
            renderEdge = GetComponent<LineRenderer>();
            if (anchor1 != null && anchor2 != null) {
                r1 = anchor1.GetComponent<Rigidbody>();
                r2 = anchor2.GetComponent<Rigidbody>();

            }
        }
        r1.interpolation = RigidbodyInterpolation.Interpolate;
        r2.interpolation = RigidbodyInterpolation.Interpolate;
        r1.drag = 2.5f;
        r2.drag = 2.5f;
        r1.useGravity = false;
        r2.useGravity = false;
		if (GraphVisualizer.graph_layout_recognized == true) {
			Edge e = new Edge(r1.transform,r2.transform);
			e.setTransformRef (anchor1.transform);
			edgeManager.addEdges(e);
			Destroy(gameObject);      
		}
    }



    private void FixedUpdate() {



        if (stopTimeCheck == false && GraphVisualizer.graph_layout_recognized == false) {
            if (GraphVisualizer.graph_layout_recognized) {
                stopTimeCheck = true;
                GraphVisualizer.layout_edges++;
            }
            // get absolute magnitude of two force vectors
            force_ns = Mathf.Abs(force.magnitude + force2.magnitude);
            EdgeLength = Vector3.Distance(anchor1.position, anchor2.position);
            // stop spring graph when force is < 0.03f
            if (exe_force == 1 && Time.timeSinceLevelLoad > 10 && EdgeLength < 1f) {
                stopTimeCheck = true;
                GraphVisualizer.layout_edges++;
                exe_force = 0;
            }
            if (r1 != null && r2 != null && stopTimeCheck == false && GraphVisualizer.graph_layout_recognized == false) {
                ApplyHookesLaw();
            } else {
                //if (graph_vis.QuadrantLayout) {
                //    PolarCoord();
                //}
                stopTimeCheck = true;
            }
        } else {
            GraphVisualizer.layout_edges++;
            //EdgeUpdate e = Camera.main.gameObject.AddComponent<EdgeUpdate>();
            //e.setNodePositions(anchor1, anchor2);
            //e.setE(Camera.main.GetComponents<EdgeUpdate>().Length - 1);
            //r1.isKinematic = true;
            //r2.isKinematic = true;
            //graph_vis.addEdges(e);
			Edge e = new Edge(r1.transform,r2.transform);
			e.setTransformRef (r1.transform);
			edgeManager.addEdges(e);
            Destroy(gameObject);          
        }

    }
    // based on Andrew's code
    private void ApplyHookesLaw() {
        exe_force = 1;
        Vector3 d = r2.transform.position - r1.transform.position;
        float displacement = Length - d.magnitude;

        Vector3 dir = d.normalized;
        force = SpringK * dir * displacement * -Length;
        force2 = SpringK * dir * displacement * Length;
        r1.AddForce((force / adjCount1) * (Time.deltaTime * 200));
        r2.AddForce((force2 / adjCount2) * (Time.deltaTime * 200));
    }

    private void PolarCoord() {
        float radius = 2;
        // calculate polar coord for node 1
        float theta = 2 * Mathf.PI * 0 - Mathf.Acos(r1.position.x / radius);
        float x = radius * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta);
        r1.position = new Vector3(x, r1.position.y, z);
        // calculate polar coord for node 2
        theta = 2 * Mathf.PI * 0 - Mathf.Acos(r2.position.x / radius);
        x = radius * Mathf.Cos(-theta);
        z = radius * Mathf.Sin(-theta);
        r2.position = new Vector3(x, r2.position.y, z);

    }


    // Dynamically moves the edge when moved
    void recalculateEdge() {
        /* Vector3 a = a_transform.position;
         Vector3 b = b_transform.position;
         Vector3 midP =  (a + b) / 2f; 

         GameObject goCylAB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
         goCylAB.transform.position = midP;

         Vector3 origVec = new Vector3(0, 1.0f, 0f);
         Vector3 targetVec = new Vector3();
         targetVec = b-a;

         var l = targetVec.magnitude/2f;

         targetVec.Normalize();

         float angle = Mathf.Acos( Vector3.Dot(origVec,targetVec));

         Vector3 axis = new Vector3();
         axis = Vector3.Cross(origVec, targetVec);
         axis.Normalize();

         goCylAB.transform.localRotation = Quaternion.AngleAxis((angle)*Mathf.Rad2Deg,axis); 
         goCylAB.transform.parent = transform;

         // curve cylinder
         //goCylAB.GetComponent<MeshFilter>().mesh = edgeMesh;
         Mesh cylinder = goCylAB.GetComponent<MeshFilter>().mesh;
         Vector3[] v = cylinder.vertices;
         for(int i = 0; i < v.Length; i++) {
                 //v[i] += Vector3.up * Time.deltaTime;
         }
         cylinder.vertices = v;
         cylinder.RecalculateBounds ();*/
    }

    public void setAnchor1(Transform node) {
        if (edgeType != EdgeType.LineRenderer) {
            anchor1.transform.position = node.position;
            anchor1.transform.parent = node;
            anchor1.name = "Edge Anchor: " + node;
        } else {
            anchor1 = node;
        }

    }

    public void setAnchor2(Transform node) {
        if (edgeType != EdgeType.LineRenderer) {
            anchor2.transform.position = node.position;
            anchor2.transform.parent = node;
            anchor2.name = "Edge Anchor: " + node;
            anchor2.name = "Edge Anchor: " + node;
        } else {
            anchor2 = node;
        }

    }

    public void setMidAnchor(Transform nodeA, Transform nodeB) {
        if (anchorMid != null) {
            anchorMid.name = "Midpoint: " + nodeA + " " + nodeB;
            Vector3 a = nodeA.position;
            Vector3 b = nodeB.position;
            float x = a.x + b.x;
            float y = a.y + b.y;
            float z = a.z + b.z;
            Vector3 mid = new Vector3(x / 2, y / 2, z / 2);
            anchorMid.transform.position = mid;
            int r_x = Random.Range(-48, -20);
            int r_y = Random.Range(-23, -10);
            int r_z = Random.Range(5, 109);
            anchorMid.rotation *= Quaternion.Euler(r_x, r_y, r_z);
        }
    }

    public void setAdjCount(int adj1, int adj2) {
        this.adjCount1 = adj1;
        this.adjCount2 = adj2;
    }

    public void setHashCode(int h) {
        hashCode = h;
    }

}
