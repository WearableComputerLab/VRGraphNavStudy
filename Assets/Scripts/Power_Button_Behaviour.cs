using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Button_Behaviour : MonoBehaviour {

    public string Query = "";
    public bool ActiveQuery = false;
    public bool isQueryButton = false;
    int on = 0;
    float in_time = 1f;
    bool interact = false;
    public Vector3 backPos;
    Vector3 startPos, startLocPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        startLocPos = transform.localPosition;
        // Set the query button as this if the parent requires a query button
        if (isQueryButton) {
            Filter_Cube_Behaviour f = transform.root.GetComponent<Filter_Cube_Behaviour>();
            f.setQueryButton(this);
        }
    }
	
	// Update is called once per frame
	void LateUpdate () {

        ActiveQuery = (on == 1);

		if(interact == true) {
            in_time -= Time.deltaTime;
            transform.localPosition = backPos;
            if (in_time <= 0) {
                interact = false;
                in_time = 1;
                GetComponent<MeshRenderer>().materials[0].color = Color.green;
            }
        } 

        if(on == 1) {
            transform.localPosition = backPos;
            GetComponent<MeshRenderer>().materials[0].color = Color.green;
            //print("on");
        }

        if (on == 0) {
            transform.localPosition = startLocPos;
            GetComponent<MeshRenderer>().materials[0].color = Color.red;
        }
        
        if(on > 1) {
            on = 0;
        }
	}

    void OnCollisionExit(Collision other) {
        if(other.gameObject.tag == "Controller" && in_time == 1) {
            interact = true;
            on++;
        }
    }
}
