using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
// source: https://www.raywenderlich.com/149239/htc-vive-tutorial-unity
public class LaserPointer : MonoBehaviour {

    // disable when running a user study - stops grabbing nodes
    public bool userStudyRunning = true;
	// path markers for path finding task
	public GameObject pathMarker;

	private Miniature_Camera_Behaviour mini = null;

    // reference to cluster behaviour
    private ClusterBehaviour c;

	public GameObject circle;
    private float laserDist = 0.1f;

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    private GameObject laser;
    public Image target;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public bool touchPadDown = false;
    public static int num_of_nodes = 0;
    public RectTransform CanvasRect;
    private RectTransform documentText;
    private Vector3 velocity;
    private Transform hitTransform = null;
    private VisualNode lasered_node;
    private Box_SaveButton save_button;

    private GraphVisualizer graph;
    private Quaternion rotateGraphTo;
    private Lever_Behaviour lev;
    public Transform node;
    private bool pulling = false;
    private float t = 0.5f;
    private float laser_t = 0;
    private float doc_time = 0;
    private float doc_move = 0;
    public float cz_t = 0.2f;
    private Vector3 pickedUpNodePos;
    public bool comfort_zoom = false;
    // 
    public int studyState = 0;
    private Vector2 q;
	private Transform nodeParent;
    public VignetteAndChromaticAberration comfortZoomVignette;
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    public StudyAssign studyAssign;
    void Awake() {
        trackedObj = transform.parent.GetComponent<SteamVR_TrackedObject>();
		if (laserPrefab != null) {
			laser = Instantiate (laserPrefab);
		}
        laserTransform = laser.transform;
        laser.gameObject.name = trackedObj.name + " laser";
        graph = FindObjectOfType<GraphVisualizer>();
        rotateGraphTo = graph.transform.rotation;
        if (userStudyRunning) {
            studyAssign = FindObjectOfType<StudyAssign>();
        }
    }

    void Update() {
        q = Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
		if (laserPrefab != null) {
			laser.SetActive (true);
		}
                         // Comfort Zoom Check
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && q.y <= -0.7f && studyState == 0) {
                //comfort_zoom = true;
            } else {
                if(Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) == false) { 
                    //comfort_zoom = false;
                }
                if (studyState == 0)
                {
                    //comfort_zoom = false;
                }
            }
            if(studyState == 1)
            {
                if(studyState == 1 && Controller.GetHairTriggerUp()) {
                    comfort_zoom = false;
                }
                if (studyState == 1 && Controller.GetHairTrigger())
                {
                    comfort_zoom = true;
                }
            }
            if (comfort_zoom) {
                cz_t = 0.2f;
                float z_dist0 = Vector3.Distance(Camera.main.transform.position, trackedObj.transform.position)*2;
                comfortZoomVignette.enabled = true;
                Camera.main.transform.parent.position += trackedObj.transform.forward * 0.01f * z_dist0;
                Transform c1 = transform.parent;
                GetComponent<MeshRenderer>().enabled = false;
                transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                comfortZoomVignette.intensity = 0.8f;
                foreach (Transform child in c1) {
                    child.gameObject.layer = 5;
                }
                foreach (Transform child in c1.GetChild(0)) {
                    child.gameObject.layer = 5;
                }

            }
            if(comfort_zoom == false) {
                cz_t -= Time.deltaTime;
                if (Controller.GetHairTrigger() == false && cz_t < 0)
                {
                    Transform c1 = transform.parent;
                    GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<MeshRenderer>().enabled = true;
                    transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    if (comfortZoomVignette.intensity > -0.1f)
                    {
                        comfortZoomVignette.intensity -= 2 * Time.deltaTime;
                    }
                    else
                    {
                        comfortZoomVignette.enabled = false;
                    }
                    foreach (Transform child in c1)
                    {
                        child.gameObject.layer = 0;
                    }
                    foreach (Transform child in c1.GetChild(0))
                    {
                        child.gameObject.layer = 0;
                    }
                }
            }
    }

    // Switching between controls
    private void StudyController()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            studyState = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            studyState = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            studyState = 2;
        }
        if(studyAssign != null)
        {
            if (userStudyRunning && studyAssign == null)
            {
                studyAssign = FindObjectOfType<StudyAssign>();
            }
            if (studyAssign.currentNavigation.Equals("TELE"))
            {
                studyState = 2;
            }
            if (studyAssign.currentNavigation.Equals("ZOOM"))
            {
                studyState = 1;
            }
        }
    }

    private void ShowLaser(RaycastHit hit) {

        //laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);

       
        //laserTransform.position = trackedObj.transform.position;
        //laserTransform.LookAt(hitPoint);
        //laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
          //  laserDist);
        if (hit.transform.name != "Bounds") {
            if (hit.transform.tag == "Node" || hit.transform.tag == "StudyNode") {
                if (Controller.GetPress(SteamVR_Controller.ButtonMask.Grip) && studyState == 0 || studyState == 2 && Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
                    Camera_System camera_sys = Camera.main.GetComponent<Camera_System>();
                    Vector3 teleportPosition = hit.point;
                    camera_sys.FadeOutIn(teleportPosition);

                }
                if (lasered_node == null && laser_t <= 0 && userStudyRunning == false) {
                    lasered_node = hit.transform.GetComponent<VisualNode>();
                    lasered_node.OnLaserOver();
                    if(laser_t <= 0)
                        laser_t = 1f;
                } else {
                    if (lasered_node != null) {
                        if (hit.transform == lasered_node.transform && laser_t < 0.5f && laser_t > 0.25f && lasered_node != null) {
                            lasered_node.OnLaserExit();
                            lasered_node = null;

                        } else {
                            lasered_node.OnLaserExit();
                            lasered_node = hit.transform.GetComponent<VisualNode>();
                            lasered_node.OnLaserOver();
                            if (laser_t <= 0)
                                laser_t = 1f;
                        }
                    }
                }
            }

            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
                hitTransform = hit.transform;
                if (hitTransform.tag != "cluster") {
                    // controller move right
                    if (velocity.x > 0.1f) {
                        //hitTransform.position += new Vector3(0.1f, 0, 0);
                    }
                    // controller move left
                    if (velocity.x < -0.1f) {
                        //hitTransform.position += new Vector3(-0.1f, 0, 0);
                    }
                }
                if(hitTransform.tag == "cluster" && userStudyRunning == false) {
                    if (c == null) {
                        c = hit.transform.GetComponent<ClusterBehaviour>();
                        c.Highlight();
                    } else {
                        c.ExitHighlight();
                        print("exit_highlight");
                        c = null;
                        c = hit.transform.GetComponent<ClusterBehaviour>();
                        c.Highlight();
                    }
                }

            } else {
            }
        } else {
        }
       
    }
    void FixedUpdate() {
        //get the device associated with that tracked object (which is how you access buttons and stuff)
        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index);

        //get the velocity
        //velocity = device.transform.rot.eulerAngles;
        //print(comfortZoomVignette.intensity);
        //print(q);
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)== false) {
            //graph.transform.rotation = Quaternion.Lerp(graph.transform.rotation, rotateGraphTo, 0.01f);
            pulling = false;
			if(mini != null) {
				if(nodeParent != null){
					mini.transform.parent = nodeParent;
				}
				mini.setMoving(false);
			}
        }
        StudyController();
		// Laser code
		if (laser != null && studyState == 2) {
			circle.transform.position = laserTransform.position + laserTransform.forward * (laserDist * 2);
			circle.transform.LookAt (Camera.main.transform);
			laserTransform.position = trackedObj.transform.position;
			laserTransform.rotation = trackedObj.transform.rotation;
			laserTransform.localScale = new Vector3 (laserTransform.localScale.x, laserTransform.localScale.y,
				laserDist);
			if (Controller.GetHairTriggerDown ()) {
				Camera_System camera_sys = Camera.main.GetComponent<Camera_System>();
				Vector3 teleportPosition = transform.position + transform.forward * (laserDist * 2);
				camera_sys.FadeOutIn(teleportPosition);
				camera_sys.setLaserPointer (this);
			}
			if (q.y > 0.4f) {
				laserDist += Time.deltaTime * 0.5f;
			}

			if (q.y < -0.4f && laserDist > 0.1f) {
				laserDist -= Time.deltaTime * 0.5f;
			}
		}
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger) && save_button != null) {
            save_button.pushButton();
        }
		// Creates a path marker
		if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
			Instantiate(pathMarker,transform.position+new Vector3(0,0.01f,0),pathMarker.transform.rotation);
		}

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger) && node != null && Dual_Controller_Behaviour.zoomGesture == false && save_button == null) {
            if(userStudyRunning == false) { 
                pulling = true;
            }
            if(node.transform.tag == "NavigationCamera")
            {
                pulling = true;
            }
			if(node.transform.tag == "pathMarker")
			{
				pulling = true;
			}

            Vector3 v_rot = Controller.angularVelocity;
            Vector3 v_pos = Controller.velocity;


            if (node.transform.name == "filter_cube") {              
                Vector3 new_rot = new Vector3(0, v_rot.y, 0);
                node.transform.rotation *= Quaternion.Euler(new_rot);
                node.transform.position = transform.position + new Vector3(0, 0.1f, 0);
                //node.transform.rotation = transform.rotation;
            }
            if (node.transform.tag == "NavigationCamera") {
                //Power_Button_Behaviour p = node.transform.root.GetComponentInChildren<Power_Button_Behaviour>();
                mini = node.transform.GetComponent<Miniature_Camera_Behaviour>();
                mini.setMoving(true);
                mini.setCone(transform);
                //graph.transform.rotation = transform.rotation;
                print("mini = "+mini);
                Vector3 new_rot = new Vector3(0, 0, v_rot.z);
                //node.transform.localRotation *= Quaternion.Euler(new_rot);

				node.transform.parent = transform;
				//Camera.main.transform.parent.localPosition += Controller.velocity * 0.1f;
            } else {
                if(mini != null) {
                    mini.setMoving(false);
                    //mini = null;
                }
            }
            if (node.transform.tag == "Lever") {
                lev = node.GetComponent<Lever_Behaviour>();
                if (lev.isHorizontial) {
                    if (v_pos.x > 0.1f) {
                        lev.movetoPos2();
                    }
                    if (v_pos.x < -0.1f) {
                        lev.movetoPos1();
                    }
                }
                if (lev.isVertical) {
                    if (v_pos.y > 0.1f) {
                        lev.movetoPos1();
                    }
                    if (v_pos.y < -0.1f) {
                        lev.movetoPos2();
                    }
                }
            }
            if(node.transform.tag == "cluster" && !userStudyRunning || node.transform.tag == "Node" && !userStudyRunning) {
                if(userStudyRunning == false) { 
                    node.transform.position = transform.position + new Vector3(0, 0.01f, 0);
                    if (Controller.velocity.magnitude > 1.5f && node.transform.tag == "Node")
                    {
                        //node.transform.GetComponent<Rigidbody>().useGravity = true;
                        node.transform.GetComponent<Rigidbody>().velocity = Controller.velocity;
                        node = null;
                    }
                }
            }
            if(node.transform.tag == "Untagged" || node.transform.tag == "SaveBox" && save_button == null
				|| node.transform.tag == "pathMarker") {
				Destroy (node.gameObject);
				node = null;
            }
        } else {
            if(lev != null) {
                lev.movetoStartPos();
                lev = null;
            }

        }

        if (!pulling && node != null) {
            t -= Time.deltaTime;
            if (t <= 0) {
                node = null;
                t = 0.5f;
            }
        }
        if (documentText) {
            if(Controller.velocity.y > 0.4f && doc_time <= 0) {
                doc_move = 1;
                doc_time = 0.5f;
            }
            if (Controller.velocity.y < -0.4f && doc_time <= 0) {
                doc_move = -1;
                doc_time = 0.5f;
            }
        }
        if(doc_time > -0.01f && documentText != null) {
            documentText.anchoredPosition += new Vector2(0, 0.05f * doc_move);
            doc_time -= Time.deltaTime;
        }

        if(laser_t >= 0) {
            laser_t -= Time.deltaTime;
        }

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && q.y > -0.7f ) {
            touchPadDown = true;
            RaycastHit hit;
            laser.SetActive(true);

            if (target != null) {
                target.enabled = true;
            }

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
                hitPoint = hit.point;
                ShowLaser(hit);
            }

        } else {

            touchPadDown = false;

        }
    }

    void OnCollisionStay(Collision other) {
        if (node == null) {
            node = other.transform;
			nodeParent = other.transform.parent;
            pickedUpNodePos = node.transform.position;
            print(node);
        } 
        if(other.gameObject.tag == "CloseButton") {
            lasered_node = null;
        }
//        print(other.transform);
    }

    void OnTriggerEnter(Collider other) {
        if (userStudyRunning == false)
        {
            if (other.tag == "Document")
            {
                documentText = other.GetComponent<Document_Behaviour>().scollableText();
                foreach (Transform t in transform.GetChild(0).GetComponentInChildren<Transform>())
                {
                    t.gameObject.layer = 5;
                }
            }
            if (other.tag == "SaveBox")
            {
                Cardboard_Box_Behaviour box = other.GetComponent<Cardboard_Box_Behaviour>();
                if (node != null)
                    box.inBox();
            }
            if (other.tag == "SaveButton")
            {
                Box_SaveButton btn = other.GetComponent<Box_SaveButton>();
                save_button = btn;
                btn.Highlight();
                print("selected save button");
            }
        }
        if (other.tag == "StudyNode") {
            StudyNode s = other.gameObject.GetComponent<StudyNode>();
            s.highlight();
            s.createEdges();
            print("Found Study Node and Method should be called");
        }
        if (other.tag == "NavigationCamera")
        {
            node = other.transform.parent;
			nodeParent = other.transform.parent.parent;
        }

        if (node == null && other.tag != "SaveButton" && userStudyRunning == false)
        node = other.transform.parent;
        //print(other.transform +" triggered");
    }

    void OnTriggerExit(Collider other) {
        if (userStudyRunning == false)
        {
            if (other.tag == "Document")
            {
                documentText = null;
                foreach (Transform t in transform.GetChild(0).GetComponentInChildren<Transform>())
                {
                    t.gameObject.layer = 0;
                }
            }
            if (other.tag == "SaveBox")
            {
                Cardboard_Box_Behaviour box = other.GetComponent<Cardboard_Box_Behaviour>();
                if (node != null)
                {
                    box.outBox();
                    if (node.transform.tag != "SaveBox" && node.transform.tag != "SaveButton")
                    {
                        node.transform.position = pickedUpNodePos;
                        box.AddNode(node.GetComponent<VisualNode>());
                    }
                }
            }
            if (other.tag == "SaveButton")
            {
                Box_SaveButton btn = other.GetComponent<Box_SaveButton>();
                save_button = null;
                btn.ExitHighlight();
                print("left save button");
            }
        }
        if (other.tag == "StudyNode") {
            StudyNode s = other.gameObject.GetComponent<StudyNode>();
            s.highlight_studynode = true;
        }
        if (other.tag == "NavigationCamera")
        {
            node = other.transform.parent;
        }
        if (node == null && other.tag != "SaveButton" && userStudyRunning == false)
            node = other.transform.parent;

    }

    public VisualNode getLaseredNode() {
        if (lasered_node != null) {
            return lasered_node;
        } else {
            return null;
        }
    } 

	public void resetLaserPointer(){
		this.laserDist = 0.1f;
	}

}
