using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Document_Behaviour : MonoBehaviour {

    public SteamVR_TrackedObject controller;
    public int val = 0;
    private RectTransform  ui_canvas;
    public string node_ID , lastID;
    private Image panel;
    private RectTransform panel_transform;
    private Text[] document_information;
    private Transform camera_transform;
    private bool isCluster = false;
    private VisualNode vn;

    // Use this for initialization
    void Awake () {
        panel_transform = GetComponent<RectTransform>();
        panel = GetComponent<Image>();
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        panel_transform.localScale = new Vector3(panel_transform.localScale.x, 0, panel_transform.localScale.z);
        document_information = GetComponentsInChildren<Text>();
        camera_transform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (controller) {
            //transform.parent.parent = controller.transform;
            Quaternion q = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            transform.rotation = new Quaternion(q.x*0.5f, q.y, q.z * 0.5f, q.w);
           
            if (node_ID != null) {
                // expand document
                if (Vector3.Distance(transform.position, camera_transform.position) < 0.6f) {
                    if (panel_transform.localScale.y < 0.7) {
                        panel_transform.localScale += new Vector3(0, 0.05f, 0);
                    }
                    if (panel.color.a < 1) {
                        panel.color += new Color(0, 0, 0, 0.05f);
                    }
                    if (isCluster) {
                        transform.GetChild(3).gameObject.SetActive(true);
                        if (vn.hiding() == true) {
                            transform.GetChild(3).gameObject.GetComponent<Image>().color = Color.black;

                        }
                        if (vn.hiding() == false) {
                            transform.GetChild(3).gameObject.GetComponent<Image>().color = Color.white;

                        }
                    }
                } else {
                    transform.GetChild(3).gameObject.SetActive(false);
                    if (panel_transform.localScale.y > 0) {
                        panel_transform.localScale += new Vector3(0, -0.01f, 0);
                    } else {

                    }
                    if (panel.color.a >= 0) {
                        panel.color += new Color(0, 0, 0, -0.05f);
                    }
                }
            } else {
                    if (panel_transform.localScale.y > 0) {
                        panel_transform.localScale += new Vector3(0, -0.01f, 0);
                    } else {

                    }
                    if (panel.color.a >= 0) {
                        panel.color += new Color(0, 0, 0, -0.05f);
                    }
                    transform.GetChild(3).gameObject.SetActive(false);
            }
        }
	}

    // Create a new document
    public void NewDocument(string id, string attribute_information,Vector3 pos) {
        this.node_ID = id;
        document_information[0].text = id;
        document_information[1].text = attribute_information;
        transform.position = pos;
    }

    public void NewDocument(string id, string attribute_information, Vector3 pos, bool isCluster, VisualNode vn) {
        this.node_ID = id;
        lastID = node_ID;
        document_information[0].text = id;
        document_information[1].text = attribute_information;
        transform.position = pos;
        this.isCluster = isCluster;
        this.vn = vn;
    }

    public void CloseDocument() {
        node_ID = null;
        document_information[1].rectTransform.anchoredPosition = new Vector2(0, -16);
    }

    public RectTransform scollableText() {
        return document_information[1].rectTransform;
    }

    public VisualNode getVisualNode() {
        return vn;
    }

    public string getNodeID() {
        return lastID;
    }
}
