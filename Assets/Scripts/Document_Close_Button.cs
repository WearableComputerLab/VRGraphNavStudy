using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Document_Close_Button : MonoBehaviour {
    Document_Behaviour d;
    public StudyNode s;
    public string nodeID;
	// Use this for initialization
	void Start () {
        d = transform.parent.GetComponent<Document_Behaviour>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Controller") {
            gameObject.GetComponent<Image>().color = Color.white;
        }
    }
    void OnCollisionExit(Collision other) {
        if (other.gameObject.tag == "Controller") {
            if(s != null) {
                print("hit the close button");
                print(s.getID() +" : "+d.getNodeID());
                if(s.getID().Equals(d.getNodeID())) {
                    print("call method");
                    s.reachedNode();
                }
            }
            d.CloseDocument();
            gameObject.GetComponent<Image>().color = Color.black;

        }
    }

    public void setStudyNode(StudyNode s) {
        this.s = s;
        nodeID = s.getID();
    }
}
