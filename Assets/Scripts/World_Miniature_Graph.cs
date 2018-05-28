using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World_Miniature_Graph : MonoBehaviour {
    public SteamVR_TrackedObject tracker;
    private SteamVR_Controller.Device vr_device;
    public GraphVisualizer graph;
    ClusterBehaviour[] clusters;
    private GameObject miniature;
    private bool createdMiniature = false;
    public Transform[] miniStudyNodes;
	private float y_angular_velocity = 0;
    // WM Calculations
    private Renderer Rcluster;
    private Renderer WMcluster;
    public Vector3 C;
    public Vector3 Cm;
	private Transform origin;
	private Miniature_Camera_Behaviour miniCam;

	private void Awake(){
		miniCam = FindObjectOfType<Miniature_Camera_Behaviour>();
	}

	// Update is called once per frame
	private void Update () {
		
        // Get the right most controller
        SteamVR_Controller.Device controller2 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
        vr_device = SteamVR_Controller.Input((int)tracker.index);
     //   if(vr_device.angularVelocity.y > 0.02f) { 
            transform.rotation = Quaternion.Lerp(transform.rotation, tracker.transform.rotation, 20 * Time.deltaTime);
            transform.position = tracker.transform.position +  tracker.transform.forward * 0.2f;
            transform.GetChild(2).LookAt(Camera.main.transform);
     //   }
		y_angular_velocity = vr_device.angularVelocity.y;

        if (graph.graphIsClustered() && !createdMiniature) {

            clusters = graph.transform.GetComponentsInChildren<ClusterBehaviour>();
            miniature = new GameObject();
            miniature.transform.name = "Miniature Graph";
            miniature.transform.parent = transform.GetChild(0);
            miniature.transform.localPosition = graph.transform.position / 15;
			origin = new GameObject ("origin").transform;
			origin.parent = transform.GetChild(0);
			origin.transform.localPosition = new Vector3 (0, 0, 0);
            MeshFilter[] meshFilters = new MeshFilter[clusters.Length];
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            Material[] mat = new Material[clusters.Length];
            for (int k = 0; k < clusters.Length; k++) {
                GameObject g = clusters[k].gameObject;
                GameObject j = new GameObject();
                mat[k] = g.GetComponent<MeshRenderer>().material;
                meshFilters[k] = g.GetComponent<MeshFilter>();
                combine[k].mesh = meshFilters[k].sharedMesh;
                combine[k].subMeshIndex = k;
                combine[k].transform = meshFilters[k].transform.localToWorldMatrix;
                j.AddComponent<MeshFilter>();
                j.AddComponent<MeshRenderer>();
                j.GetComponent<MeshRenderer>().material = mat[k];
                j.GetComponent<MeshFilter>().mesh = combine[k].mesh;
                j.layer = 5;
                j.transform.parent = miniature.transform;
                j.transform.localPosition = Vector3.zero;
                if (k == 0) {
                    Rcluster = g.GetComponent<Renderer>();
                    WMcluster = j.GetComponent<Renderer>();
                }
            }
			// check if empty
			if (miniStudyNodes [0] != null) {
				for (int k = 0; k < miniStudyNodes.Length; k++) {
					miniStudyNodes [k].transform.parent = miniature.transform;
				}
			}
			// identify study node and make parent of WIM
			WIMNode wimNode = GetComponentInChildren<WIMNode>();
			wimNode.transform.parent = miniature.transform;
            miniature.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, -90));
            miniature.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            createdMiniature = true;
        }

        // if the miniature has been created, and the user is rotating the graph
        //if (createdMiniature && controller2.GetPress(SteamVR_Controller.ButtonMask.Grip)) {
        if (miniature != null && miniCam.Moving) {
			graph.transform.rotation = miniature.transform.rotation;
        }
        //}
        CalculateWMVectors();
    }

    private void CalculateWMVectors() {
        if(Rcluster != null && WMcluster != null) {
            C = Rcluster.bounds.center;
            Cm = WMcluster.bounds.center;
        }
    }

	public float getYAngularVelocity(){
		return y_angular_velocity;
	}

	public Vector3 returnWIMWorldOrigin(){
		return origin.position;
	}
}