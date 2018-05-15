using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class Dual_Controller_Behaviour : MonoBehaviour {

    public SteamVR_TrackedObject[] trackedControllers = new SteamVR_TrackedObject[2];
    public LaserPointer[] lasers = new LaserPointer[2];
    public Document_Behaviour[] documents = new Document_Behaviour[2];
    public VignetteAndChromaticAberration comfortZoomVignette;

    private SteamVR_Controller.Device controller1;
    private SteamVR_Controller.Device controller2;
    private Vector3 controller1_velocity;
    private Vector3 controller2_velocity;
    public static bool zoomGesture = false;
    private float distance = 0;
    private LineRenderer render_line;
    private LineRenderer arrow_object;
    private LineRenderer dotted_line;
    private Vector3 controller_vector;
    private float direction;
    public float left_right;
    private Transform graph;
    public bool rotateGraph_wController = false;
    private Lever_Behaviour slider;
    public bool userStudyRunning = true;
    public bool leftHanded = false;

    void Start () {
        render_line = gameObject.GetComponent<LineRenderer>();
        render_line.enabled = false;
        arrow_object = transform.GetChild(0).GetComponent<LineRenderer>();
        arrow_object.enabled = false;
        dotted_line = transform.GetChild(1).GetComponent<LineRenderer>();
        dotted_line.enabled = false;
        graph = FindObjectOfType<GraphVisualizer>().transform;
    }
	
	void LateUpdate () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
        controller1 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
        controller2 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
        Debug.DrawRay(trackedControllers[1].transform.position, trackedControllers[1].transform.up * -100 + new Vector3(0,-1,0));
        Vector2 q = controller2.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        Vector2 cont2_axis = controller1.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        bool comfort_zoom = false;
        string[] names = UnityEngine.Input.GetJoystickNames();

        // make sure both controllers are active
        if (controller1 != null && controller2 != null) {
            UpdateDocuments();
            distance = Vector3.Distance(trackedControllers[0].transform.localPosition, trackedControllers[1].transform.localPosition);
            // velocity of the controllers
            controller1_velocity = controller1.velocity;
            controller2_velocity = controller2.velocity;
            Vector3 v_rot = controller2.angularVelocity;

            // Comfort zooming variables
            float z_dist0 = Vector3.Distance(Camera.main.transform.position, trackedControllers[0].transform.position)*2;
            float z_dist1 = Vector3.Distance(Camera.main.transform.position, trackedControllers[1].transform.position)*2;


            // Archery variables
            controller_vector = trackedControllers[1].transform.position - trackedControllers[0].transform.position;
            direction = Vector3.Distance(trackedControllers[0].transform.localPosition, trackedControllers[1].transform.localPosition) * Vector3.Dot(controller_vector.normalized, trackedControllers[0].transform.forward);
            left_right = Vector3.Distance(trackedControllers[0].transform.localPosition, trackedControllers[1].transform.localPosition) * Vector3.Dot(controller_vector.normalized, trackedControllers[0].transform.right);
            if (controller1.GetHairTrigger() && controller2.GetHairTrigger() && lasers[0].studyState == 0 && lasers[1].studyState == 0) {
                zoomGesture = true;               
            }
            /*
            if (controller1.GetHairTrigger() && controller2.GetHairTrigger() && lasers[0].node != null && lasers[1].node != null) {
                dotted_line.SetPosition(0, lasers[0].node.transform.position);
                dotted_line.SetPosition(1, lasers[1].node.transform.position);
                dotted_line.transform.GetChild(0).transform.position = new Vector3((lasers[0].node.transform.position.x + lasers[1].node.transform.position.x) / 2,
                    (lasers[0].node.transform.position.y + lasers[1].node.transform.position.y) / 2,
                    (lasers[0].node.transform.position.z + lasers[1].node.transform.position.z) / 2);
                dotted_line.enabled = true;
            } else {
                //dotted_line.enabled = false;
            }
            */

            // Archery Zoom Check
            if (controller1.GetHairTrigger() == false && controller2.GetHairTrigger() == false) {
                zoomGesture = false;
            }



            if(controller2.GetPress(SteamVR_Controller.ButtonMask.Grip) && userStudyRunning == false) {
                rotateGraph_wController = true;
            } else {
                rotateGraph_wController = false;
            }
            if (zoomGesture) {
                if (leftHanded == false)
                {
                    render_line.SetPosition(0, trackedControllers[0].transform.position);
                    render_line.SetPosition(1, trackedControllers[1].transform.position);
                    arrow_object.SetPosition(0, trackedControllers[0].transform.position);
                    arrow_object.SetPosition(1, trackedControllers[1].transform.position);
                    render_line.enabled = true;

                    arrow_object.enabled = true;
                    if (controller_vector.y < -0.4f)
                    {
                        Camera.main.transform.parent.position += Camera.main.transform.parent.up * -0.003f * (direction + 1);
                    }
                    if (controller_vector.y > 0.4f)
                    {
                        Camera.main.transform.parent.position += Camera.main.transform.parent.up * 0.003f * (direction + 1);
                    }
                    Camera.main.transform.parent.position += controller_vector * 0.08f * (direction + 1);
                } else
                {
                    render_line.SetPosition(0, trackedControllers[1].transform.position);
                    render_line.SetPosition(1, trackedControllers[0].transform.position);
                    arrow_object.SetPosition(0, trackedControllers[1].transform.position);
                    arrow_object.SetPosition(1, trackedControllers[0].transform.position);
                    render_line.enabled = true;

                    arrow_object.enabled = true;
                    print(controller_vector);
                    if (controller_vector.y < -0.4f)
                    {
                        Camera.main.transform.parent.position -= Camera.main.transform.parent.up * -0.003f * (direction + 1);
                    }
                    if (controller_vector.y > 0.4f)
                    {
                        Camera.main.transform.parent.position -= Camera.main.transform.parent.up * 0.003f * (direction + 1);
                    }
                    Camera.main.transform.parent.position -= controller_vector * 0.08f * (direction + 1);
                }
            } else {
                render_line.enabled = false;
                arrow_object.enabled = false;
            }

            if (rotateGraph_wController && userStudyRunning == false) {
                Quaternion rotation = (graph.transform.rotation * (trackedControllers[1].transform.rotation));
                //graph.transform.rotation = trackedControllers[0].transform.rotation;
                graph.transform.rotation *= Quaternion.Euler(v_rot.x,v_rot.y, v_rot.z);
                    //graph.transform.position = trackedControllers[0].transform.position;
            }
            if (q.x > 0.3f) {
               // graph.transform.Rotate(0, 0.5f, 0);
            }
            if (q.x < -0.3f) {
               // graph.transform.Rotate(0, -0.5f, 0);
            }

        }       
	}

    void UpdateDocuments() {
        if (lasers[0].getLaseredNode() != null) {
            VisualNode v = lasers[0].getLaseredNode();
            string id = v.node_name;
            documents[0].gameObject.SetActive(true);
            documents[0].NewDocument(id, v.getAttributeInfo(), v.transform.position,v.distinct_node,v);
        } else {
            documents[0].CloseDocument();
        }
        if (lasers[1].getLaseredNode() != null) {
            VisualNode v = lasers[1].getLaseredNode();
            string id = v.node_name;
            documents[1].gameObject.SetActive(true);
            documents[1].NewDocument(id, v.getAttributeInfo(),v.transform.position,v.distinct_node,v);
        } else {
            documents[1].CloseDocument();
        }
    }

    public void setSlider(Lever_Behaviour lever) {
        slider = lever;
    }

    public Lever_Behaviour getSlider() {
        return slider;
    }


}
